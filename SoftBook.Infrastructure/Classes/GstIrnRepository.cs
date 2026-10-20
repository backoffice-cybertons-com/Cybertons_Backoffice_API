using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.DTOs.GstIrn;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Classes;

public class GstIrnRepository : BaseDAL, IGstIrnRepository
{
    public GstIrnRepository(IConfiguration configuration, ITenantProvider tenantProvider)
        : base(configuration, tenantProvider)
    {
    }

    public async Task<GstIrnSettingsDto?> GetSettingsAsync(int companyId)
    {
        const string sql = """
            SELECT mode, auth_url, e_invoice_url, client_id, client_secret,
                   api_username, api_password, public_key, gstin
            FROM public.tab_gst_irn_settings
            WHERE company_id = @company_id
            """;

        return await ExecuteSingleReaderAsync(sql, cmd =>
        {
            cmd.Parameters.AddWithValue("company_id", companyId);
        }, reader => new GstIrnSettingsDto
        {
            Mode = SafeGetString(reader, "mode") ?? "Mock",
            AuthUrl = SafeGetString(reader, "auth_url"),
            EInvoiceUrl = SafeGetString(reader, "e_invoice_url"),
            ClientId = SafeGetString(reader, "client_id"),
            ClientSecret = SafeGetString(reader, "client_secret"),
            ApiUsername = SafeGetString(reader, "api_username"),
            ApiPassword = SafeGetString(reader, "api_password"),
            PublicKey = SafeGetString(reader, "public_key"),
            Gstin = SafeGetString(reader, "gstin")
        }, System.Data.CommandType.Text);
    }

    public async Task SaveSettingsAsync(int companyId, GstIrnSettingsDto settings)
    {
        const string sql = """
            INSERT INTO public.tab_gst_irn_settings
                (company_id, mode, auth_url, e_invoice_url, client_id, client_secret,
                 api_username, api_password, public_key, gstin, updated_on)
            VALUES
                (@company_id, @mode, @auth_url, @e_invoice_url, @client_id, @client_secret,
                 @api_username, @api_password, @public_key, @gstin, NOW())
            ON CONFLICT (company_id) DO UPDATE SET
                mode = EXCLUDED.mode,
                auth_url = EXCLUDED.auth_url,
                e_invoice_url = EXCLUDED.e_invoice_url,
                client_id = EXCLUDED.client_id,
                client_secret = EXCLUDED.client_secret,
                api_username = EXCLUDED.api_username,
                api_password = EXCLUDED.api_password,
                public_key = EXCLUDED.public_key,
                gstin = EXCLUDED.gstin,
                updated_on = NOW()
            """;

        await ExecuteNonQueryAsync(sql, cmd =>
        {
            cmd.Parameters.AddWithValue("company_id", companyId);
            cmd.Parameters.AddWithValue("mode", settings.Mode ?? "Mock");
            cmd.Parameters.AddWithValue("auth_url", (object?)settings.AuthUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("e_invoice_url", (object?)settings.EInvoiceUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("client_id", (object?)settings.ClientId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("client_secret", (object?)settings.ClientSecret ?? DBNull.Value);
            cmd.Parameters.AddWithValue("api_username", (object?)settings.ApiUsername ?? DBNull.Value);
            cmd.Parameters.AddWithValue("api_password", (object?)settings.ApiPassword ?? DBNull.Value);
            cmd.Parameters.AddWithValue("public_key", (object?)settings.PublicKey ?? DBNull.Value);
            cmd.Parameters.AddWithValue("gstin", (object?)settings.Gstin ?? DBNull.Value);
        }, System.Data.CommandType.Text);
    }

    public async Task<GstIrnLog?> GetBySpUniqueCodeAsync(int companyId, int spUniqueCode)
    {
        const string sql = """
            SELECT irn_id, sp_unique_code, company_id, inv_no, ack_no, ack_dt, irn,
                   signed_invoice, signed_qr_code, qr_image, status, is_mock,
                   error_code, error_message, created_by, created_on
            FROM public.tab_gst_irn
            WHERE company_id = @company_id AND sp_unique_code = @sp_unique_code
            ORDER BY irn_id DESC
            LIMIT 1
            """;

        return await ExecuteSingleReaderAsync(sql, cmd =>
        {
            cmd.Parameters.AddWithValue("company_id", companyId);
            cmd.Parameters.AddWithValue("sp_unique_code", spUniqueCode);
        }, reader => MapLog(reader), System.Data.CommandType.Text);
    }

    public async Task SaveIrnLogAsync(GstIrnLog log)
    {
        const string sql = """
            INSERT INTO public.tab_gst_irn
                (sp_unique_code, company_id, inv_no, ack_no, ack_dt, irn, signed_invoice,
                 signed_qr_code, qr_image, status, is_mock, error_code, error_message,
                 created_by, created_on)
            VALUES
                (@sp_unique_code, @company_id, @inv_no, @ack_no, @ack_dt, @irn, @signed_invoice,
                 @signed_qr_code, @qr_image, @status, @is_mock, @error_code, @error_message,
                 @created_by, @created_on)
            """;

        await ExecuteNonQueryAsync(sql, cmd =>
        {
            cmd.Parameters.AddWithValue("sp_unique_code", log.SpUniqueCode);
            cmd.Parameters.AddWithValue("company_id", log.CompanyId);
            cmd.Parameters.AddWithValue("inv_no", (object?)log.InvNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("ack_no", (object?)log.AckNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("ack_dt", (object?)log.AckDt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("irn", (object?)log.Irn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("signed_invoice", (object?)log.SignedInvoice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("signed_qr_code", (object?)log.SignedQrCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("qr_image", (object?)log.QrImage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("status", log.Status);
            cmd.Parameters.AddWithValue("is_mock", log.IsMock);
            cmd.Parameters.AddWithValue("error_code", (object?)log.ErrorCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("error_message", (object?)log.ErrorMessage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("created_by", (object?)log.CreatedBy ?? DBNull.Value);
            cmd.Parameters.AddWithValue("created_on", (object?)log.CreatedOn ?? DateTime.UtcNow);
        }, System.Data.CommandType.Text);
    }

    private GstIrnLog MapLog(NpgsqlDataReader reader)
    {
        return new GstIrnLog
        {
            IrnId = SafeGetInt(reader, "irn_id"),
            SpUniqueCode = SafeGetInt(reader, "sp_unique_code"),
            CompanyId = SafeGetInt(reader, "company_id"),
            InvNo = SafeGetString(reader, "inv_no"),
            AckNo = reader["ack_no"] == DBNull.Value ? null : Convert.ToInt64(reader["ack_no"]),
            AckDt = reader["ack_dt"] == DBNull.Value ? null : (DateTimeOffset?)reader["ack_dt"],
            Irn = SafeGetString(reader, "irn"),
            SignedInvoice = SafeGetString(reader, "signed_invoice"),
            SignedQrCode = SafeGetString(reader, "signed_qr_code"),
            QrImage = reader["qr_image"] == DBNull.Value ? null : (byte[])reader["qr_image"],
            Status = SafeGetString(reader, "status") ?? "SUCCESS",
            IsMock = SafeGetBool(reader, "is_mock"),
            ErrorCode = SafeGetString(reader, "error_code"),
            ErrorMessage = SafeGetString(reader, "error_message"),
            CreatedBy = SafeGetNullableInt(reader, "created_by"),
            CreatedOn = SafeGetDateTime(reader, "created_on")
        };
    }

    private static bool SafeGetBool(NpgsqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? false : reader.GetBoolean(ordinal);
    }

    private static int? SafeGetNullableInt(NpgsqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }
}
