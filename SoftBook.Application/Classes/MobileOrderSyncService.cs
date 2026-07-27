using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.ViewModels;

namespace SoftBook.Application.Services.Classes
{
    public class MobileOrderSyncService : IMobileOrderSyncService
    {
        private readonly ISalesTransactionService _salesTransactionService;
        private readonly IProductRepository _productRepository;
        private readonly ITenantProvider _tenantProvider;
        private readonly IPartyRepository _partyRepository;
        private readonly IPartyAddressRepository _partyAddressRepository;
        private readonly ISalePurRepository _salePurRepository;

        public MobileOrderSyncService(
            ISalesTransactionService salesTransactionService,
            IProductRepository productRepository,
            ITenantProvider tenantProvider,
            IPartyRepository partyRepository,
            IPartyAddressRepository partyAddressRepository,
            ISalePurRepository salePurRepository)
        {
            _salesTransactionService = salesTransactionService;
            _productRepository = productRepository;
            _tenantProvider = tenantProvider;
            _partyRepository = partyRepository;
            _partyAddressRepository = partyAddressRepository;
            _salePurRepository = salePurRepository;
        }

        public async Task<DbResult> SyncPendingOrdersAsync(int companyId, int? userId = null)
        {
            var syncedCount = 0;
            var linkedCount = 0;
            var errorCount = 0;
            var errors = new List<string>();
            var skipped = new List<string>();

            try
            {
                var connectionString = _tenantProvider.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                    return DbResult.Failure("Tenant connection string is not available. Ensure you are logged in with a valid company context.");

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                // Report orders that exist but are excluded from sync (helps explain "missing" records)
                var excludedOrders = await GetExcludedOrderSummaryAsync(connection, companyId);
                if (excludedOrders.Any())
                    skipped.AddRange(excludedOrders);

                var pendingOrders = await FetchPendingOrdersAsync(connection, companyId);
                if (!pendingOrders.Any())
                {
                    var noPendingMsg = "No pending mobile orders to sync.";
                    if (skipped.Any())
                        noPendingMsg += " Skipped: " + string.Join("; ", skipped);
                    return DbResult.Ok(noPendingMsg);
                }

                var sessionFiscalYearId = _tenantProvider.FiscalYearId;
                var productDetails = (await _productRepository.GetAllWithDetailsAsync())
                    .ToDictionary(p => p.ProductId, p => p);
                var detailColumns = await GetMobOrderDetailColumnsAsync(connection);
                var hasDetailRate = detailColumns.Contains("rate");
                var hasDetailMrp = detailColumns.Contains("mrp");
                var hasDetailDiscount = detailColumns.Contains("discount_per");

                foreach (var order in pendingOrders)
                {
                    try
                    {
                        var salesOrderNo = $"MOB-{order.MobOrderId}";

                        // Already linked to a book sale order (manual entry or prior partial sync)
                        int? existingSpUniqueCode = null;
                        if (order.SpUniqueCode.HasValue
                            && await SaleOrderExistsAsync(connection, companyId, order.SpUniqueCode.Value))
                        {
                            existingSpUniqueCode = order.SpUniqueCode;
                        }
                        else
                        {
                            existingSpUniqueCode = await FindExistingSaleOrderAsync(connection, companyId, salesOrderNo);
                        }

                        if (existingSpUniqueCode.HasValue)
                        {
                            var linked = await LinkMobOrderToSaleAsync(
                                connection, order.MobOrderId, existingSpUniqueCode.Value);
                            if (linked)
                            {
                                linkedCount++;
                                continue;
                            }
                        }

                        var details = await FetchOrderDetailsAsync(
                            connection, order.MobOrderId, hasDetailRate, hasDetailMrp, hasDetailDiscount);

                        if (!details.Any())
                        {
                            errorCount++;
                            errors.Add($"Order {order.MobOrderId} has no line items.");
                            continue;
                        }

                        var effectiveFiscalYearId = order.FiscalYearId ?? sessionFiscalYearId;
                        if (!effectiveFiscalYearId.HasValue)
                        {
                            errorCount++;
                            errors.Add($"Order {order.MobOrderId}: fiscal year is missing on mob_order and in session.");
                            continue;
                        }

                        var party = await _partyRepository.GetByIdAsync(order.DockManagerId);
                        if (party == null)
                        {
                            errorCount++;
                            errors.Add($"Order {order.MobOrderId}: dock manager (party_id {order.DockManagerId}) not found in party master.");
                            continue;
                        }

                        var addresses = await _partyAddressRepository.GetByPartyIdAsync(order.DockManagerId);
                        var billingAddress = addresses.FirstOrDefault(a => a.AddressType == "Billing" && a.IsDefault == true)
                                          ?? addresses.FirstOrDefault(a => a.AddressType == "Billing")
                                          ?? addresses.FirstOrDefault();
                        var shippingAddress = addresses.FirstOrDefault(a => a.AddressType == "Shipping" && a.IsDefault == true)
                                           ?? addresses.FirstOrDefault(a => a.AddressType == "Shipping")
                                           ?? billingAddress;

                        string FormatAddress(PartyAddress? addr)
                        {
                            if (addr == null) return string.Empty;
                            var parts = new List<string?> { party.DisplayName, addr.Street1, addr.Street2, addr.City, addr.State, addr.PinCode };
                            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
                        }

                        var prevOutstanding = await _salePurRepository.GetPartyOutstandingBeforeDateAsync(
                            companyId, order.DockManagerId, order.OrderDate.Date);

                        // Validate FK references — invalid party master values cause insert failures
                        var salespersonId = await ResolveSalespersonIdAsync(connection, companyId, party.SalespersonId);
                        var transporterId = await ResolveTransporterIdAsync(connection, companyId, party.TransporterId);

                        var viewModel = new SalesTransactionViewModel
                        {
                            Header = new SalePur
                            {
                                CompanyId = companyId,
                                InvType = "Sales",
                                InvCategory = "Sales Order",
                                InvoiceSeries = "GST",
                                SalesOrderNo = salesOrderNo,
                                SalesOrderDate = order.OrderDate,
                                InvDate = order.OrderDate,
                                PartyId = order.DockManagerId,
                                Description = "Synced from Mobile Order #" + order.MobOrderId
                                    + (string.IsNullOrEmpty(order.Remarks) ? "" : " - " + order.Remarks),
                                FiscalYearId = effectiveFiscalYearId,
                                BranchId = order.BranchId,
                                CreatedBy = userId,
                                CreatedOn = DateTime.Now,
                                TransporterId = transporterId,
                                SalespersonId = salespersonId,
                                Jurisdiction = party.PlaceOfSupply ?? billingAddress?.City,
                                BillingAddress = FormatAddress(billingAddress),
                                BillingAddress1 = billingAddress?.Street1,
                                BillingAddress2 = billingAddress?.City,
                                ShippingAddress = FormatAddress(shippingAddress),
                                ShippingAddress1 = shippingAddress?.Street1,
                                ShippingAddress2 = shippingAddress?.City,
                                PreviousOutstanding = prevOutstanding
                            },
                            Lines = new List<SalePurTran>()
                        };

                        foreach (var detail in details)
                        {
                            productDetails.TryGetValue(detail.ProductId, out var productDetail);
                            var product = productDetail == null
                                ? await _productRepository.GetByIdAsync(companyId, detail.ProductId)
                                : null;

                            var rate = detail.Rate ?? productDetail?.SellingPrice ?? product?.SellingPrice ?? 0;
                            var mrp = detail.Mrp ?? productDetail?.MRP ?? product?.MRP ?? 0;
                            var discountPer = detail.DiscountPer ?? 0;
                            var hsnCode = productDetail?.HSNOrSACCode ?? productDetail?.HSNCode;

                            // Tax from product master: Tax Preference = Taxable + Tax Group CGST/SGST rates
                            var isTaxable = productDetail?.TaxPreferenceId == 1;
                            var cgstPercent = isTaxable ? (productDetail?.CGSTPer ?? 0) : 0;
                            var sgstPercent = isTaxable ? (productDetail?.SGSTPer ?? 0) : 0;

                            // Match manual sales order grid: Sub Qty = 1, Qty (crt) = mobile quantity
                            var qty = detail.Quantity;
                            var line = new SalePurTran
                            {
                                CompanyId = companyId,
                                ProductId = detail.ProductId,
                                Quantity = qty,
                                Crt = qty,
                                Rate = rate,
                                Mrp = mrp,
                                HsnCode = hsnCode,
                                Unit = productDetail?.UnitId ?? product?.UnitId,
                                SubUnit = productDetail?.UnitId ?? product?.UnitId,
                                SubUnitQuantity = 1,
                                Packing = qty.ToString("0.####"),
                                DiscountPer = discountPer,
                                CgstPercent = cgstPercent,
                                SgstPercent = sgstPercent,
                                ProductDetails = productDetail?.Name ?? product?.Name ?? "Unknown Product",
                                FiscalYearId = effectiveFiscalYearId,
                                BranchId = order.BranchId,
                                CreatedBy = userId,
                                CreatedOn = DateTime.Now
                            };

                            line.Amount = qty * line.Rate * (1 - (line.DiscountPer / 100m));
                            line.CgstAmount = line.Amount * (cgstPercent / 100m);
                            line.SgstAmount = line.Amount * (sgstPercent / 100m);
                            line.TotalAmount = line.Amount + (line.CgstAmount ?? 0) + (line.SgstAmount ?? 0);
                            viewModel.Lines.Add(line);
                        }

                        viewModel.Header.TotalQuantity = viewModel.Lines.Sum(l => l.Crt ?? l.Quantity);
                        viewModel.Header.TaxableAmount = viewModel.Lines.Sum(l => l.Amount);
                        viewModel.Header.CgstAmount = viewModel.Lines.Sum(l => l.CgstAmount ?? 0);
                        viewModel.Header.SgstAmount = viewModel.Lines.Sum(l => l.SgstAmount ?? 0);
                        viewModel.Header.GrandTotal = viewModel.Lines.Sum(l => l.TotalAmount);
                        viewModel.Header.NetInvoiceAmount = viewModel.Header.GrandTotal;
                        viewModel.Header.TotSkuInInvoice = viewModel.Lines.Count;
                        viewModel.Header.CurrentBillAmount = viewModel.Header.GrandTotal;
                        viewModel.Header.FinalOutstanding = (viewModel.Header.PreviousOutstanding ?? 0)
                            + (viewModel.Header.GrandTotal ?? 0);

                        var saveResult = await _salesTransactionService.SaveCompositeAsync(viewModel, userId);
                        if (!saveResult.Success)
                        {
                            errorCount++;
                            errors.Add($"Order {order.MobOrderId} failed: {saveResult.Error}");
                            continue;
                        }

                        var newSpUniqueCode = viewModel.Header.SpUniqueCode;
                        await LinkMobOrderToSaleAsync(connection, order.MobOrderId, newSpUniqueCode);
                        syncedCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"Order {order.MobOrderId} error: {ex.Message}");
                    }
                }

                var message = $"Successfully synced {syncedCount} orders.";
                if (linkedCount > 0)
                    message += $" Linked {linkedCount} existing sale order(s) (manual or prior partial sync).";
                if (skipped.Any())
                    message += " Skipped: " + string.Join("; ", skipped);
                if (errorCount > 0)
                {
                    message += $" Failed to sync {errorCount} orders. Errors: {string.Join(", ", errors)}";
                    return DbResult.Failure(message);
                }

                return DbResult.Ok(message);
            }
            catch (Exception ex)
            {
                return DbResult.Failure($"Failed to sync mobile orders: {ex.Message}");
            }
        }

        private static async Task<List<MobOrderRow>> FetchPendingOrdersAsync(NpgsqlConnection connection, int companyId)
        {
            var pendingOrders = new List<MobOrderRow>();
            using var cmd = new NpgsqlCommand(@"
                SELECT mob_order_id, company_id, dock_manager_id, order_date, remarks,
                       total_quantity, fiscal_year_id, branch_id, sp_unique_code
                FROM public.mob_order
                WHERE company_id = @companyId
                  AND is_locked = true
                  AND is_synced = false", connection);
            cmd.Parameters.AddWithValue("companyId", companyId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                pendingOrders.Add(new MobOrderRow
                {
                    MobOrderId = reader.GetInt64(0),
                    CompanyId = reader.GetInt32(1),
                    DockManagerId = reader.GetInt32(2),
                    OrderDate = reader.GetDateTime(3),
                    Remarks = reader.IsDBNull(4) ? null : reader.GetString(4),
                    TotalQuantity = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                    FiscalYearId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    BranchId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                    SpUniqueCode = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8)
                });
            }

            return pendingOrders;
        }

        private static async Task<List<string>> GetExcludedOrderSummaryAsync(NpgsqlConnection connection, int companyId)
        {
            var summary = new List<string>();

            using (var cmd = new NpgsqlCommand(@"
                SELECT mob_order_id
                FROM public.mob_order
                WHERE company_id = @companyId
                  AND COALESCE(is_synced, false) = false
                  AND COALESCE(is_locked, false) = false
                ORDER BY mob_order_id", connection))
            {
                cmd.Parameters.AddWithValue("companyId", companyId);
                var ids = new List<long>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    ids.Add(reader.GetInt64(0));
                if (ids.Any())
                    summary.Add($"{ids.Count} order(s) not locked yet: {string.Join(", ", ids)}");
            }

            using (var cmd = new NpgsqlCommand(@"
                SELECT mob_order_id, sp_unique_code
                FROM public.mob_order
                WHERE company_id = @companyId
                  AND is_synced = true
                  AND sp_unique_code IS NULL
                ORDER BY mob_order_id", connection))
            {
                cmd.Parameters.AddWithValue("companyId", companyId);
                var ids = new List<long>();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    ids.Add(reader.GetInt64(0));
                if (ids.Any())
                    summary.Add($"{ids.Count} order(s) marked synced but missing sp_unique_code: {string.Join(", ", ids)}");
            }

            return summary;
        }

        private static async Task<HashSet<string>> GetMobOrderDetailColumnsAsync(NpgsqlConnection connection)
        {
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using var cmd = new NpgsqlCommand(@"
                SELECT column_name
                FROM information_schema.columns
                WHERE table_schema = 'public'
                  AND table_name = 'mob_order_detail'", connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                columns.Add(reader.GetString(0));
            return columns;
        }

        private static async Task<List<MobOrderDetailRow>> FetchOrderDetailsAsync(
            NpgsqlConnection connection,
            long mobOrderId,
            bool hasRate,
            bool hasMrp,
            bool hasDiscount)
        {
            var selectParts = new List<string>
            {
                "mob_order_detail_id",
                "product_id",
                "quantity"
            };
            if (hasRate) selectParts.Add("rate");
            if (hasMrp) selectParts.Add("mrp");
            if (hasDiscount) selectParts.Add("discount_per");

            var sql = $@"
                SELECT {string.Join(", ", selectParts)}
                FROM public.mob_order_detail
                WHERE mob_order_id = @mobOrderId";

            var details = new List<MobOrderDetailRow>();
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("mobOrderId", mobOrderId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var idx = 0;
                var row = new MobOrderDetailRow
                {
                    MobOrderDetailId = reader.GetInt64(idx++),
                    ProductId = reader.GetInt32(idx++),
                    Quantity = reader.IsDBNull(idx) ? 0 : reader.GetDecimal(idx)
                };
                idx++;
                if (hasRate)
                {
                    row.Rate = reader.IsDBNull(idx) ? (decimal?)null : reader.GetDecimal(idx);
                    idx++;
                }
                if (hasMrp)
                {
                    row.Mrp = reader.IsDBNull(idx) ? (decimal?)null : reader.GetDecimal(idx);
                    idx++;
                }
                if (hasDiscount)
                {
                    row.DiscountPer = reader.IsDBNull(idx) ? (decimal?)null : reader.GetDecimal(idx);
                }

                details.Add(row);
            }

            return details;
        }

        private static async Task<int?> FindExistingSaleOrderAsync(
            NpgsqlConnection connection,
            int companyId,
            string salesOrderNo)
        {
            using var cmd = new NpgsqlCommand(@"
                SELECT sp_unique_code
                FROM public.tab_sale_pur
                WHERE company_id = @companyId
                  AND sales_order_no = @salesOrderNo
                  AND COALESCE(delete_flag, false) = false
                ORDER BY sp_unique_code DESC
                LIMIT 1", connection);
            cmd.Parameters.AddWithValue("companyId", companyId);
            cmd.Parameters.AddWithValue("salesOrderNo", salesOrderNo);
            var result = await cmd.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? (int?)null : Convert.ToInt32(result);
        }

        private static async Task<bool> SaleOrderExistsAsync(
            NpgsqlConnection connection,
            int companyId,
            int spUniqueCode)
        {
            using var cmd = new NpgsqlCommand(@"
                SELECT 1
                FROM public.tab_sale_pur
                WHERE company_id = @companyId
                  AND sp_unique_code = @spUniqueCode
                  AND COALESCE(delete_flag, false) = false
                LIMIT 1", connection);
            cmd.Parameters.AddWithValue("companyId", companyId);
            cmd.Parameters.AddWithValue("spUniqueCode", spUniqueCode);
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        private static async Task<int?> ResolveSalespersonIdAsync(
            NpgsqlConnection connection,
            int companyId,
            int? salespersonId)
        {
            if (!salespersonId.HasValue)
                return null;

            using var cmd = new NpgsqlCommand(@"
                SELECT 1
                FROM public.tab_salesperson_master
                WHERE company_id = @companyId
                  AND salesperson_id = @salespersonId
                LIMIT 1", connection);
            cmd.Parameters.AddWithValue("companyId", companyId);
            cmd.Parameters.AddWithValue("salespersonId", salespersonId.Value);
            var result = await cmd.ExecuteScalarAsync();
            return result == null ? (int?)null : salespersonId;
        }

        private static async Task<int?> ResolveTransporterIdAsync(
            NpgsqlConnection connection,
            int companyId,
            int? transporterId)
        {
            if (!transporterId.HasValue)
                return null;

            using var cmd = new NpgsqlCommand(@"
                SELECT 1
                FROM public.tab_transporter_master
                WHERE company_id = @companyId
                  AND transporter_id = @transporterId
                LIMIT 1", connection);
            cmd.Parameters.AddWithValue("companyId", companyId);
            cmd.Parameters.AddWithValue("transporterId", transporterId.Value);
            var result = await cmd.ExecuteScalarAsync();
            return result == null ? (int?)null : transporterId;
        }

        private static async Task<bool> LinkMobOrderToSaleAsync(
            NpgsqlConnection connection,
            long mobOrderId,
            int spUniqueCode)
        {
            using var updateCmd = new NpgsqlCommand(@"
                UPDATE public.mob_order
                SET is_synced = true, sp_unique_code = @spUniqueCode
                WHERE mob_order_id = @mobOrderId", connection);
            updateCmd.Parameters.AddWithValue("spUniqueCode", spUniqueCode);
            updateCmd.Parameters.AddWithValue("mobOrderId", mobOrderId);
            return await updateCmd.ExecuteNonQueryAsync() > 0;
        }

        private sealed class MobOrderRow
        {
            public long MobOrderId { get; set; }
            public int CompanyId { get; set; }
            public int DockManagerId { get; set; }
            public DateTime OrderDate { get; set; }
            public string? Remarks { get; set; }
            public decimal TotalQuantity { get; set; }
            public int? FiscalYearId { get; set; }
            public int? BranchId { get; set; }
            public int? SpUniqueCode { get; set; }
        }

        private sealed class MobOrderDetailRow
        {
            public long MobOrderDetailId { get; set; }
            public int ProductId { get; set; }
            public decimal Quantity { get; set; }
            public decimal? Rate { get; set; }
            public decimal? Mrp { get; set; }
            public decimal? DiscountPer { get; set; }
        }
    }
}
