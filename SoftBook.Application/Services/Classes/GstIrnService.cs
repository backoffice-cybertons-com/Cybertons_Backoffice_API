using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using QRCoder;
using SoftBook.Application.Interfaces;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs.GstIrn;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Application.Services.Classes;

public class GstIrnService : IGstIrnService
{
    private static readonly JsonSerializerOptions NicJsonOptions = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly ISalesTransactionService _salesTransactionService;
    private readonly ICompanyProfileRepository _companyProfileRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly IPartyAddressRepository _partyAddressRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMasterRepository _masterRepository;
    private readonly IGstIrnRepository _gstIrnRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GstIrnPayloadBuilder _payloadBuilder = new();
    private readonly GstIrnOptions _defaultOptions;

    public GstIrnService(
        ISalesTransactionService salesTransactionService,
        ICompanyProfileRepository companyProfileRepository,
        IPartyRepository partyRepository,
        IPartyAddressRepository partyAddressRepository,
        IProductRepository productRepository,
        IMasterRepository masterRepository,
        IGstIrnRepository gstIrnRepository,
        IHttpClientFactory httpClientFactory,
        IOptions<GstIrnOptions> defaultOptions)
    {
        _salesTransactionService = salesTransactionService;
        _companyProfileRepository = companyProfileRepository;
        _partyRepository = partyRepository;
        _partyAddressRepository = partyAddressRepository;
        _productRepository = productRepository;
        _masterRepository = masterRepository;
        _gstIrnRepository = gstIrnRepository;
        _httpClientFactory = httpClientFactory;
        _defaultOptions = defaultOptions.Value;
    }

    public async Task<GstIrnSettingsDto> GetSettingsAsync(int companyId)
    {
        var dbSettings = await _gstIrnRepository.GetSettingsAsync(companyId);
        if (dbSettings != null)
        {
            return dbSettings;
        }

        return new GstIrnSettingsDto
        {
            Mode = _defaultOptions.Mode,
            AuthUrl = _defaultOptions.AuthUrl,
            EInvoiceUrl = _defaultOptions.EInvoiceUrl,
            ClientId = _defaultOptions.ClientId,
            ClientSecret = _defaultOptions.ClientSecret,
            ApiUsername = _defaultOptions.ApiUsername,
            ApiPassword = _defaultOptions.ApiPassword,
            PublicKey = _defaultOptions.PublicKey,
            Gstin = _defaultOptions.Gstin
        };
    }

    public Task SaveSettingsAsync(int companyId, GstIrnSettingsDto settings)
        => _gstIrnRepository.SaveSettingsAsync(companyId, settings);

    public async Task<string> BuildPayloadJsonAsync(int companyId, int spUniqueCode)
    {
        var payload = await BuildPayloadAsync(companyId, spUniqueCode);
        return JsonSerializer.Serialize(payload, NicJsonOptions);
    }

    public async Task<GstIrnGenerateResultDto> GenerateAsync(int companyId, int spUniqueCode, int? userId)
    {
        var settings = await GetSettingsAsync(companyId);
        var payload = await BuildPayloadAsync(companyId, spUniqueCode);
        var payloadJson = JsonSerializer.Serialize(payload, NicJsonOptions);
        var invNo = payload.DocDtls?.No;

        if (string.Equals(settings.Mode, "Live", StringComparison.OrdinalIgnoreCase))
        {
            return await GenerateLiveAsync(companyId, spUniqueCode, settings, payloadJson, invNo, userId);
        }

        return await GenerateMockAsync(companyId, spUniqueCode, payloadJson, invNo, userId);
    }

    public async Task<GstIrnStatusDto?> GetStatusAsync(int companyId, int spUniqueCode)
    {
        var log = await _gstIrnRepository.GetBySpUniqueCodeAsync(companyId, spUniqueCode);
        if (log == null)
        {
            return null;
        }

        return new GstIrnStatusDto
        {
            SpUniqueCode = spUniqueCode,
            InvNo = log.InvNo,
            Irn = log.Irn,
            AckNo = log.AckNo,
            AckDt = log.AckDt,
            Status = log.Status,
            ErrorMessage = log.ErrorMessage,
            QrImageBase64 = log.QrImage != null ? Convert.ToBase64String(log.QrImage) : null,
            IsMock = log.IsMock
        };
    }

    private async Task<GenerateIrnReqModel> BuildPayloadAsync(int companyId, int spUniqueCode)
    {
        var transaction = await _salesTransactionService.GetByIdAsync(companyId, spUniqueCode);
        if (transaction?.Header == null)
        {
            throw new InvalidOperationException("Sales invoice not found.");
        }

        if (!string.Equals(transaction.Header.InvType, "Sales", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("E-Invoice can only be generated for sales invoices.");
        }

        var company = await _companyProfileRepository.GetByCompanyIdAsync(companyId)
            ?? throw new InvalidOperationException("Company profile not found.");

        var party = await _partyRepository.GetByIdAsync(transaction.Header.PartyId)
            ?? throw new InvalidOperationException("Party not found.");

        var addresses = await _partyAddressRepository.GetByPartyIdAsync(transaction.Header.PartyId);
        var billing = addresses.FirstOrDefault(a => a.AddressType.Equals("Billing", StringComparison.OrdinalIgnoreCase))
            ?? addresses.FirstOrDefault();
        var shipping = addresses.FirstOrDefault(a => a.AddressType.Equals("Shipping", StringComparison.OrdinalIgnoreCase))
            ?? billing;

        var unitUqcById = await LoadUnitUqcMapAsync(companyId);
        var productNameById = await LoadProductNameMapAsync(companyId, transaction.Lines);

        return _payloadBuilder.Build(
            transaction.Header,
            transaction.Lines,
            company,
            party,
            billing,
            shipping,
            unitUqcById,
            productNameById);
    }

    private async Task<Dictionary<int, string>> LoadUnitUqcMapAsync(int companyId)
    {
        var map = new Dictionary<int, string>();
        var units = await _masterRepository.GetAllAsync("Unit", companyId: companyId);
        foreach (var unit in units)
        {
            if (unit is not IDictionary<string, object> dict)
            {
                continue;
            }

            var id = GetInt(dict, "unit_id", "UnitId", "unitId");
            var uqc = GetString(dict, "uqc", "Uqc", "UQC", "unit_name", "UnitName");
            if (id > 0)
            {
                map[id] = string.IsNullOrWhiteSpace(uqc) ? "OTH" : uqc;
            }
        }

        return map;
    }

    private async Task<Dictionary<int, string>> LoadProductNameMapAsync(int companyId, List<SalePurTran> lines)
    {
        var map = new Dictionary<int, string>();
        foreach (var productId in lines.Where(l => l.ProductId.HasValue).Select(l => l.ProductId!.Value).Distinct())
        {
            var product = await _productRepository.GetByIdAsync(companyId, productId);
            if (product?.Name != null)
            {
                map[productId] = product.Name;
            }
        }

        return map;
    }

    private async Task<GstIrnGenerateResultDto> GenerateMockAsync(
        int companyId, int spUniqueCode, string payloadJson, string? invNo, int? userId)
    {
        var ackNo = Random.Shared.NextInt64(100000000000, 999999999999);
        var ackDt = DateTimeOffset.UtcNow;
        var irn = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{spUniqueCode}-{ackNo}-{invNo}"))).ToLowerInvariant();
        var signedQr = $"MOCK-QR|{irn}|{invNo}|{ackNo}";
        var qrImage = GenerateQrCode(signedQr);

        await _gstIrnRepository.SaveIrnLogAsync(new GstIrnLog
        {
            SpUniqueCode = spUniqueCode,
            CompanyId = companyId,
            InvNo = invNo,
            AckNo = ackNo,
            AckDt = ackDt,
            Irn = irn,
            SignedQrCode = signedQr,
            QrImage = qrImage,
            Status = "SUCCESS",
            IsMock = true,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        });

        return new GstIrnGenerateResultDto
        {
            Success = true,
            Mode = "Mock",
            Message = $"Mock IRN generated successfully - Ack No: {ackNo}",
            SpUniqueCode = spUniqueCode,
            InvNo = invNo,
            Irn = irn,
            AckNo = ackNo,
            AckDt = ackDt,
            SignedQrCode = signedQr,
            QrImageBase64 = Convert.ToBase64String(qrImage),
            PayloadJson = payloadJson
        };
    }

    private async Task<GstIrnGenerateResultDto> GenerateLiveAsync(
        int companyId, int spUniqueCode, GstIrnSettingsDto settings,
        string payloadJson, string? invNo, int? userId)
    {
        if (string.IsNullOrWhiteSpace(settings.AuthUrl) ||
            string.IsNullOrWhiteSpace(settings.EInvoiceUrl) ||
            string.IsNullOrWhiteSpace(settings.ClientId) ||
            string.IsNullOrWhiteSpace(settings.ClientSecret) ||
            string.IsNullOrWhiteSpace(settings.ApiUsername) ||
            string.IsNullOrWhiteSpace(settings.ApiPassword) ||
            string.IsNullOrWhiteSpace(settings.PublicKey) ||
            string.IsNullOrWhiteSpace(settings.Gstin))
        {
            return new GstIrnGenerateResultDto
            {
                Success = false,
                Mode = "Live",
                Message = "Live e-Invoice credentials are not configured. Set Client ID, Secret, API username/password, public key, GSTIN and URLs in GST IRN settings.",
                SpUniqueCode = spUniqueCode,
                InvNo = invNo,
                PayloadJson = payloadJson
            };
        }

        try
        {
            var aesKey = GstIrnCryptoService.GenerateSecureKey();
            var authPayload = new GstIrnAuthRequest
            {
                Password = settings.ApiPassword,
                AppKey = Convert.ToBase64String(aesKey),
                UserName = settings.ApiUsername,
                ForceRefreshAccessToken = false
            };

            var authJson = JsonSerializer.Serialize(authPayload, NicJsonOptions);
            var encryptedAuth = GstIrnCryptoService.EncryptWithPublicKey(
                Convert.ToBase64String(Encoding.UTF8.GetBytes(authJson)),
                settings.PublicKey);

            var authRequest = new GstIrnRequestPayload { Data = encryptedAuth };
            var client = _httpClientFactory.CreateClient("GstIrn");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("client-id", settings.ClientId);
            client.DefaultRequestHeaders.Add("client-secret", settings.ClientSecret);
            client.DefaultRequestHeaders.Add("gstin", settings.Gstin);

            var authResponse = await client.PostAsJsonAsync(settings.AuthUrl, authRequest, NicJsonOptions);
            var authBody = await authResponse.Content.ReadAsStringAsync();
            if (!authResponse.IsSuccessStatusCode)
            {
                return FailLive(spUniqueCode, invNo, payloadJson, $"Auth API failed: {authBody}");
            }

            var authResult = JsonSerializer.Deserialize<GstIrnAuthResponse>(authBody, NicJsonOptions);
            if (authResult?.Data?.Sek == null || string.IsNullOrWhiteSpace(authResult.Data.AuthToken))
            {
                var err = authResult?.ErrorDetails?.FirstOrDefault()?.ErrorMessage ?? authBody;
                return FailLive(spUniqueCode, invNo, payloadJson, $"Auth failed: {err}");
            }

            var sek = GstIrnCryptoService.DecryptBySymmetricKey(authResult.Data.Sek, aesKey);
            var sekBytes = Convert.FromBase64String(sek);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("client-id", settings.ClientId);
            client.DefaultRequestHeaders.Add("client-secret", settings.ClientSecret);
            client.DefaultRequestHeaders.Add("gstin", settings.Gstin);
            client.DefaultRequestHeaders.Add("user_name", settings.ApiUsername);
            client.DefaultRequestHeaders.Add("AuthToken", authResult.Data.AuthToken);

            var encryptedInvoice = GstIrnCryptoService.EncryptBySymmetricKey(
                Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson)),
                sek);

            var invoiceRequest = new GstIrnRequestPayload { Data = encryptedInvoice };
            var invoiceResponse = await client.PostAsJsonAsync(settings.EInvoiceUrl, invoiceRequest, NicJsonOptions);
            var invoiceBody = await invoiceResponse.Content.ReadAsStringAsync();
            if (!invoiceResponse.IsSuccessStatusCode)
            {
                return FailLive(spUniqueCode, invNo, payloadJson, $"Generate IRN API failed: {invoiceBody}");
            }

            using var doc = JsonDocument.Parse(invoiceBody);
            var root = doc.RootElement;
            var status = root.TryGetProperty("Status", out var statusEl) ? statusEl.GetString() : null;
            if (status != "1")
            {
                var errMsg = ExtractErrors(root);
                await SaveErrorLog(companyId, spUniqueCode, invNo, errMsg, userId);
                return new GstIrnGenerateResultDto
                {
                    Success = false,
                    Mode = "Live",
                    Message = errMsg,
                    SpUniqueCode = spUniqueCode,
                    InvNo = invNo,
                    PayloadJson = payloadJson
                };
            }

            var encryptedData = root.GetProperty("Data").GetString() ?? string.Empty;
            var decryptedData = GstIrnCryptoService.DecryptBySymmetricKey(encryptedData, sekBytes);
            var invoiceJson = Encoding.UTF8.GetString(Convert.FromBase64String(decryptedData));
            var result = JsonSerializer.Deserialize<GstIrnAuthData>(invoiceJson, NicJsonOptions)
                ?? throw new InvalidOperationException("Unable to parse IRN response.");

            var qrImage = string.IsNullOrWhiteSpace(result.SignedQrCode)
                ? Array.Empty<byte>()
                : GenerateQrCode(result.SignedQrCode);

            await _gstIrnRepository.SaveIrnLogAsync(new GstIrnLog
            {
                SpUniqueCode = spUniqueCode,
                CompanyId = companyId,
                InvNo = invNo ?? result.InvNo,
                AckNo = result.AckNo,
                AckDt = result.AckDt,
                Irn = result.Irn,
                SignedInvoice = result.SignedInvoice,
                SignedQrCode = result.SignedQrCode,
                QrImage = qrImage,
                Status = "SUCCESS",
                IsMock = false,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            });

            return new GstIrnGenerateResultDto
            {
                Success = true,
                Mode = "Live",
                Message = $"IRN generated successfully - Ack No: {result.AckNo}",
                SpUniqueCode = spUniqueCode,
                InvNo = invNo ?? result.InvNo,
                Irn = result.Irn,
                AckNo = result.AckNo,
                AckDt = result.AckDt,
                SignedQrCode = result.SignedQrCode,
                QrImageBase64 = qrImage.Length > 0 ? Convert.ToBase64String(qrImage) : null,
                PayloadJson = payloadJson
            };
        }
        catch (Exception ex)
        {
            return FailLive(spUniqueCode, invNo, payloadJson, ex.Message);
        }
    }

    private static GstIrnGenerateResultDto FailLive(int spUniqueCode, string? invNo, string payloadJson, string message)
        => new()
        {
            Success = false,
            Mode = "Live",
            Message = message,
            SpUniqueCode = spUniqueCode,
            InvNo = invNo,
            PayloadJson = payloadJson
        };

    private async Task SaveErrorLog(int companyId, int spUniqueCode, string? invNo, string message, int? userId)
    {
        await _gstIrnRepository.SaveIrnLogAsync(new GstIrnLog
        {
            SpUniqueCode = spUniqueCode,
            CompanyId = companyId,
            InvNo = invNo,
            Status = "FAILED",
            IsMock = false,
            ErrorMessage = message,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow
        });
    }

    private static string ExtractErrors(JsonElement root)
    {
        if (!root.TryGetProperty("ErrorDetails", out var errors) || errors.ValueKind != JsonValueKind.Array)
        {
            return "IRN generation failed.";
        }

        var messages = errors.EnumerateArray()
            .Select(e => e.TryGetProperty("ErrorMessage", out var m) ? m.GetString() : null)
            .Where(m => !string.IsNullOrWhiteSpace(m));
        return string.Join(", ", messages);
    }

    private static byte[] GenerateQrCode(string qrText)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(data);
        return qrCode.GetGraphic(20);
    }

    private static int GetInt(IDictionary<string, object> dict, params string[] keys)
    {
        foreach (var key in keys)
        {
            var match = dict.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                continue;
            }

            var value = dict[match];
            if (value is int i)
            {
                return i;
            }

            if (value is long l)
            {
                return (int)l;
            }

            if (int.TryParse(value?.ToString(), out var parsed))
            {
                return parsed;
            }
        }

        return 0;
    }

    private static string? GetString(IDictionary<string, object> dict, params string[] keys)
    {
        foreach (var key in keys)
        {
            var match = dict.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                return dict[match]?.ToString();
            }
        }

        return null;
    }
}
