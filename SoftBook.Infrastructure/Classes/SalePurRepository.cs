using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Classes
{
    public class SalePurRepository : BaseDAL, ISalePurRepository
    {
        public SalePurRepository(IConfiguration configuration, ITenantProvider tenantProvider) 
            : base(configuration, tenantProvider)
        {
        }

        private static void AddQuantityParameter(NpgsqlCommand cmd, string name, decimal? value)
        {
            var parameter = cmd.Parameters.Add(name, NpgsqlTypes.NpgsqlDbType.Numeric);
            parameter.Precision = 18;
            parameter.Scale = 4;
            parameter.Value = (object?)value ?? DBNull.Value;
        }

        //public async Task<decimal> GetPartyOutstandingAsync(int companyId, int partyId)
        //{
        //    return await ExecuteScalarAsync<decimal>("fn_get_party_outstanding", cmd =>
        //    {
        //        cmd.Parameters.AddWithValue("p_company_id", companyId);
        //        cmd.Parameters.AddWithValue("p_party_id", partyId);
        //    });
        //}

        public async Task<List<SalePur>> GetAllAsync(int companyId)
        {
            var rows = await ExecuteReaderAsync("public.fn_sale_pur", cmd =>
            {
                var dummy = new SalePur { CompanyId = companyId };
                cmd.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "GETALL" });
                AddSalePurParameters(cmd, dummy);
                // Ensure p_user_id is provided to match function signature
                cmd.Parameters.Add(new NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object?)_tenantProvider?.UserId ?? DBNull.Value });
            }, MapSalePur);

            // Global fiscal-year filtering (set by UI modal into tenant context)
            if (_tenantProvider?.FiscalYearId != null)
            {
                int fyId = _tenantProvider.FiscalYearId.Value;
                return rows.Where(r => r.FiscalYearId == fyId).ToList();
            }

            // If fiscal year is not selected yet, don't show any sales/purchase data.
            // Modal selection is mandatory, so returning an empty list avoids showing unfiltered data behind the blur.
            return new List<SalePur>();
        }

        public async Task<SalePur?> GetByIdAsync(int companyId, int spUniqueCode)
        {
            var header = await ExecuteSingleReaderAsync("public.fn_sale_pur", cmd =>
            {
                var dummy = new SalePur { CompanyId = companyId, SpUniqueCode = spUniqueCode };
                cmd.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "GETBYID" });
                AddSalePurParameters(cmd, dummy);
                // Ensure p_user_id is provided to match function signature
                cmd.Parameters.Add(new NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object?)_tenantProvider?.UserId ?? DBNull.Value });
            }, MapSalePur);

            if (header == null) return null;
            if (_tenantProvider?.FiscalYearId == null) return null;

            if (header.FiscalYearId != _tenantProvider.FiscalYearId.Value)
            {
                return null;
            }

            return header;
        }

        // changes made by Hetak dt 20052026 - shared transaction wrapper for sale/purchase header, lines, and JV.
        public async Task<DbResult> ExecuteInTransactionAsync(Func<IDbTransaction, Task<DbResult>> operation)
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await operation(transaction);
                if (!result.Success)
                {
                    await transaction.RollbackAsync();
                    return result;
                }

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return HandleException(ex);
            }
        }

        public async Task<DbResult> InsertAsync(SalePur entity, int? userId = null, IDbTransaction? transaction = null)
        {
            try
            {
                void BuildParameters(NpgsqlCommand cmd)
                {
                    cmd.Parameters.AddWithValue("p_action", "INSERT");
                    AddSalePurParameters(cmd, entity);
                    cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
                }

                var result = transaction is NpgsqlTransaction npgsqlTransaction
                    ? await ExecuteSingleReaderAsync("public.fn_sale_pur", BuildParameters, MapSalePur, npgsqlTransaction)
                    : await ExecuteSingleReaderAsync("public.fn_sale_pur", BuildParameters, MapSalePur);

                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // changes made by Hetak dt 20052026 - invoice number advisory lock stays inside the same transaction.
        public async Task<DbResult> InsertWithNextInvoiceNumberAsync(SalePur entity, int? userId = null, IDbTransaction? transaction = null)
        {
            if (transaction is NpgsqlTransaction externalTransaction)
            {
                try
                {
                    await AssignNextInvoiceNumberAsync(externalTransaction.Connection!, externalTransaction, entity);

                    var result = await ExecuteSingleReaderAsync("public.fn_sale_pur", cmd =>
                    {
                        cmd.Parameters.AddWithValue("p_action", "INSERT");
                        AddSalePurParameters(cmd, entity);
                        cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
                    }, MapSalePur, externalTransaction);

                    return DbResult.Ok(result);
                }
                catch (Exception ex)
                {
                    return HandleException(ex);
                }
            }

            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();
            await using var localTransaction = await connection.BeginTransactionAsync();

            try
            {
                await AssignNextInvoiceNumberAsync(connection, localTransaction, entity);

                var result = await ExecuteSingleReaderAsync("public.fn_sale_pur", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "INSERT");
                    AddSalePurParameters(cmd, entity);
                    cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
                }, MapSalePur, localTransaction);

                await localTransaction.CommitAsync();
                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                await localTransaction.RollbackAsync();
                return HandleException(ex);
            }
        }

        // -----------------------------------------------------------------------
        // Hetak Modi 13-06-2026
        // REASON: This method intentionally keeps inline SQL and CANNOT be moved
        // to a database function.
        //
        // It uses pg_advisory_xact_lock() which acquires a transaction-level
        // advisory lock. For the lock to protect the invoice number generation,
        // it MUST run on the same NpgsqlConnection and within the same
        // NpgsqlTransaction as the INSERT statement that follows.
        // If moved to a DB function, the lock would be released as soon as the
        // function returns — completely breaking the concurrency protection and
        // causing duplicate invoice numbers under concurrent requests.
        // -----------------------------------------------------------------------
        private async Task AssignNextInvoiceNumberAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, SalePur entity)
        {
            var (prefix, width) = await GetInvoiceNumberFormatAsync(connection, transaction, entity);
            var fiscalYearKey = entity.FiscalYearId?.ToString() ?? "0";
            var lockKey = $"sale-pur-inv:{entity.CompanyId}:{fiscalYearKey}:{entity.InvType}:{prefix}";

            await using (var lockCommand = new NpgsqlCommand("SELECT pg_advisory_xact_lock(hashtext(@lock_key)::bigint)", connection, transaction))
            {
                lockCommand.Parameters.Add("lock_key", NpgsqlTypes.NpgsqlDbType.Text).Value = lockKey;
                await lockCommand.ExecuteNonQueryAsync();
            }

            var pattern = $"^{Regex.Escape(prefix)}([0-9]+)$";
            int lastNumber;

            await using (var maxCommand = new NpgsqlCommand(@"
                SELECT COALESCE(MAX((regexp_match(inv_no, @pattern))[1]::integer), 0)
                FROM public.tab_sale_pur
                WHERE company_id = @company_id
                  AND inv_type = @inv_type
                  AND COALESCE(delete_flag, false) = false
                  AND inv_no ~ @pattern
                  AND (@fiscal_year_id IS NULL OR fiscal_year_id = @fiscal_year_id)", connection, transaction))
            {
                maxCommand.Parameters.Add("pattern", NpgsqlTypes.NpgsqlDbType.Text).Value = pattern;
                maxCommand.Parameters.Add("company_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.CompanyId;
                maxCommand.Parameters.Add("inv_type", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.InvType ?? DBNull.Value;
                maxCommand.Parameters.Add("fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.FiscalYearId ?? DBNull.Value;

                var result = await maxCommand.ExecuteScalarAsync();
                lastNumber = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }

            var nextNumber = lastNumber + 1;
            entity.InvNo = $"{prefix}{nextNumber.ToString().PadLeft(width, '0')}";
            entity.InvoiceSeriesIncrementNumber = nextNumber;
        }

        // -----------------------------------------------------------------------
        // Hetak Modi 13-06-2026
        // REASON: This method intentionally keeps inline SQL and CANNOT be moved
        // to a database function.
        //
        // The SQL regex pattern used here (e.g. '^INV([0-9]+)$') is built
        // dynamically in C# using Regex.Escape(prefix) at runtime, and then
        // passed as a parameterized @pattern value — making it fully safe from
        // SQL injection. The pattern depends on the existing invoice series in
        // the database, which is only known after reading live data.
        //
        // Additionally, this query must share the same connection + transaction
        // as AssignNextInvoiceNumberAsync so it participates in the advisory lock
        // scope. Moving it to a DB function would break this coupling.
        // -----------------------------------------------------------------------
        private async Task<(string Prefix, int Width)> GetInvoiceNumberFormatAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, SalePur entity)
        {
            await using var command = new NpgsqlCommand(@"
                SELECT parts[1] AS prefix, LENGTH(parts[2]) AS width
                FROM (
                    SELECT regexp_match(inv_no, '^(.*?)([0-9]+)$') AS parts
                    FROM public.tab_sale_pur
                    WHERE company_id = @company_id
                      AND inv_type = @inv_type
                      AND COALESCE(delete_flag, false) = false
                      AND inv_no ~ '^(.*?)([0-9]+)$'
                      AND (@fiscal_year_id IS NULL OR fiscal_year_id = @fiscal_year_id)
                ) parsed
                WHERE parts IS NOT NULL
                GROUP BY parts[1], LENGTH(parts[2])
                ORDER BY COUNT(*) DESC, MAX((parts[2])::integer) DESC
                LIMIT 1", connection, transaction);

            command.Parameters.Add("company_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.CompanyId;
            command.Parameters.Add("inv_type", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.InvType ?? DBNull.Value;
            command.Parameters.Add("fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.FiscalYearId ?? DBNull.Value;

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (reader.GetString(0), reader.GetInt32(1));
            }

            return GetInvoiceNumberFormat(entity.InvNo, entity.InvoiceSeries);
        }

        private static (string Prefix, int Width) GetInvoiceNumberFormat(string? invoiceNumber, string? invoiceSeries)
        {
            var trimmedInvoiceNumber = invoiceNumber?.Trim() ?? string.Empty;
            var match = Regex.Match(trimmedInvoiceNumber, @"^(.*?)(\d+)$");

            if (match.Success)
            {
                return (match.Groups[1].Value, match.Groups[2].Value.Length);
            }

            var prefix = invoiceSeries?.Trim() ?? string.Empty;
            return (prefix, 1);
        }

        public async Task<DbResult> UpdateAsync(SalePur entity, int? userId = null, IDbTransaction? transaction = null)
        {
            try
            {
                void BuildParameters(NpgsqlCommand cmd)
                {
                    cmd.Parameters.AddWithValue("p_action", "UPDATE");
                    AddSalePurParameters(cmd, entity);
                    cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
                }

                var result = transaction is NpgsqlTransaction npgsqlTransaction
                    ? await ExecuteSingleReaderAsync("public.fn_sale_pur", BuildParameters, MapSalePur, npgsqlTransaction)
                    : await ExecuteSingleReaderAsync("public.fn_sale_pur", BuildParameters, MapSalePur);

                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> DeleteAsync(int companyId, int spUniqueCode, int? userId = null)
        {
            try
            {
                await ExecuteNonQueryAsync("public.fn_sale_pur", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "DELETE");
                    cmd.Parameters.AddWithValue("p_sp_unique_code", spUniqueCode);
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                    cmd.Parameters.AddWithValue("p_deleted_by", userId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_user_id", userId ?? (object)DBNull.Value);
                });
                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<List<SalePurTran>> GetLineItemsAsync(int spUniqueCode)
        {
            return await ExecuteReaderAsync("public.fn_sal_pur_tran", cmd =>
            {
                cmd.Parameters.AddWithValue("p_action", "GETBYPARENT");
                cmd.Parameters.AddWithValue("p_sp_unique_code", spUniqueCode);
            }, MapSalePurTran);
        }

        public async Task<DbResult> InsertLineItemAsync(SalePurTran entity, IDbTransaction? transaction = null)
        {
            try
            {
                void BuildParameters(NpgsqlCommand cmd)
                {
                    cmd.Parameters.AddWithValue("p_action", "INSERT");
                    AddSalePurTranParameters(cmd, entity);
                }

                var result = transaction is NpgsqlTransaction npgsqlTransaction
                    ? await ExecuteSingleReaderAsync("public.fn_sal_pur_tran", BuildParameters, MapSalePurTran, npgsqlTransaction)
                    : await ExecuteSingleReaderAsync("public.fn_sal_pur_tran", BuildParameters, MapSalePurTran);

                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> UpdateLineItemAsync(SalePurTran entity)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_sal_pur_tran", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "UPDATE");
                    AddSalePurTranParameters(cmd, entity);
                }, MapSalePurTran);
                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> DeleteLineItemAsync(int spTranUniqueCode)
        {
            try
            {
                var deletedBy = _tenantProvider?.UserId;
                await ExecuteNonQueryAsync("public.fn_sal_pur_tran", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "DELETE");
                    cmd.Parameters.AddWithValue("p_sp_tran_unique_code", spTranUniqueCode);
                    cmd.Parameters.AddWithValue("p_deleted_by", deletedBy ?? (object)DBNull.Value);
                });
                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // [ADDED 2026-05-06] Implementation for soft-deleting all lines of an invoice
        public async Task<DbResult> DeleteLineItemsByParentAsync(int spUniqueCode, int? userId = null, IDbTransaction? transaction = null)
        {
            try
            {
                void BuildParameters(NpgsqlCommand cmd)
                {
                    cmd.Parameters.AddWithValue("p_action", "DELETE_BY_PARENT");
                    cmd.Parameters.AddWithValue("p_sp_unique_code", spUniqueCode);
                    cmd.Parameters.AddWithValue("p_deleted_by", userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
                }

                if (transaction is NpgsqlTransaction npgsqlTransaction)
                {
                    await ExecuteNonQueryAsync("public.fn_sal_pur_tran", BuildParameters, npgsqlTransaction);
                }
                else
                {
                    await ExecuteNonQueryAsync("public.fn_sal_pur_tran", BuildParameters);
                }

                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private SalePur MapSalePur(NpgsqlDataReader reader)
        {
            return new SalePur
            {
                SpUniqueCode = SafeGetInt(reader, "sp_unique_code"),
                CompanyId = SafeGetInt(reader, "company_id"),
                InvoiceSeries = SafeGetString(reader, "invoice_series"),
                InvoiceSeriesIncrementNumber = reader.IsDBNull(reader.GetOrdinal("invoice_series_increment_number")) ? (int?)null : SafeGetInt(reader, "invoice_series_increment_number"),
                InvType = SafeGetString(reader, "inv_type"),
                InvSubType = SafeGetString(reader, "inv_sub_type"),
                InvCategory = SafeGetString(reader, "inv_category"),
                SalesOrderNo = SafeGetString(reader, "sales_order_no"),
                SalesOrderDate = SafeGetDateTime(reader, "sales_order_date"),
                PoOrderNo = SafeGetString(reader, "po_order_no"),
                PoOrderDate = SafeGetDateTime(reader, "po_order_date"),
                BillId = reader.IsDBNull(reader.GetOrdinal("bill_id")) ? (int?)null : SafeGetInt(reader, "bill_id"),
                DeliveryNo = reader.IsDBNull(reader.GetOrdinal("delivery_no")) ? (int?)null : SafeGetInt(reader, "delivery_no"),
                DeliveryDate = SafeGetDateTime(reader, "delivery_date"),
                VehicleNo = SafeGetString(reader, "vehicle_no"),
                TransporterId = reader.IsDBNull(reader.GetOrdinal("transporter_id")) ? (int?)null : SafeGetInt(reader, "transporter_id"),
                SalespersonId = reader.IsDBNull(reader.GetOrdinal("salesperson_id")) ? (int?)null : SafeGetInt(reader, "salesperson_id"),
                UnitId = reader.IsDBNull(reader.GetOrdinal("unit_id")) ? (int?)null : SafeGetInt(reader, "unit_id"),
                BankId = reader.IsDBNull(reader.GetOrdinal("bank_id")) ? (int?)null : SafeGetInt(reader, "bank_id"),
                ProductId = reader.IsDBNull(reader.GetOrdinal("product_id")) ? (int?)null : SafeGetInt(reader, "product_id"),
                TaxId = reader.IsDBNull(reader.GetOrdinal("tax_id")) ? (int?)null : SafeGetInt(reader, "tax_id"),
                NetInvoiceAmount = reader.IsDBNull(reader.GetOrdinal("net_invoice_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("net_invoice_amount")),
                GrandTotal = reader.IsDBNull(reader.GetOrdinal("grand_total")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("grand_total")),
                BillingAddress = SafeGetString(reader, "billing_address"),
                BillingAddress1 = SafeGetString(reader, "billing_address1"),
                BillingAddress2 = SafeGetString(reader, "billing_address2"),
                ShippingAddress = SafeGetString(reader, "shipping_address"),
                ShippingAddress1 = SafeGetString(reader, "shipping_address1"),
                ShippingAddress2 = SafeGetString(reader, "shipping_address2"),
                TotDiscount = reader.IsDBNull(reader.GetOrdinal("tot_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tot_discount")),
                TotSkuInInvoice = SafeGetInt(reader, "tot_sku_in_invoice"),
                InvNo = SafeGetString(reader, "inv_no"),
                InvDate = SafeGetDateTime(reader, "inv_date") ?? DateTime.MinValue,
                DueDate = SafeGetDateTime(reader, "due_date"),
                PartyId = SafeGetInt(reader, "party_id"),
                TermsId = reader.IsDBNull(reader.GetOrdinal("terms_id")) ? (int?)null : SafeGetInt(reader, "terms_id"),
                Description = SafeGetString(reader, "description"),
                TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
                IgstPercent = reader.IsDBNull(reader.GetOrdinal("igst_percent")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("igst_percent")),
                IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
                CgstPercent = reader.IsDBNull(reader.GetOrdinal("cgst_percent")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("cgst_percent")),
                CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
                SgstPercent = reader.IsDBNull(reader.GetOrdinal("sgst_percent")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("sgst_percent")),
                SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
                RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("round_off")),
                PreviousOutstanding = reader.IsDBNull(reader.GetOrdinal("previous_outstanding")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("previous_outstanding")),
                CurrentBillAmount = reader.IsDBNull(reader.GetOrdinal("current_bill_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("current_bill_amount")),
                FinalOutstanding = reader.IsDBNull(reader.GetOrdinal("final_outstanding")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("final_outstanding")),
                Jurisdiction = SafeGetString(reader, "jurisdiction"),
                FiscalYearId = reader.IsDBNull(reader.GetOrdinal("fiscal_year_id")) ? (int?)null : SafeGetInt(reader, "fiscal_year_id"),
                BranchId = reader.IsDBNull(reader.GetOrdinal("branch_id")) ? (int?)null : SafeGetInt(reader, "branch_id"),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? (int?)null : SafeGetInt(reader, "created_by"),
                ModifiedBy = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? (int?)null : SafeGetInt(reader, "modified_by"),
                DeletedBy = reader.IsDBNull(reader.GetOrdinal("deleted_by")) ? (int?)null : SafeGetInt(reader, "deleted_by"),
                DeleteFlag = SafeGetBool(reader, "delete_flag"),
                CreatedOn = SafeGetDateTime(reader, "created_on"),
                ModifiedOn = SafeGetDateTime(reader, "modified_on"),
                DeletedOn = SafeGetDateTime(reader, "deleted_on"),
                RateDiff = reader.IsDBNull(reader.GetOrdinal("rate_diff")) ? 0 : reader.GetDecimal(reader.GetOrdinal("rate_diff")),
                InvoiceTotal = reader.IsDBNull(reader.GetOrdinal("invoice_total")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("invoice_total")),
                FinalInvoiceAmount = reader.IsDBNull(reader.GetOrdinal("final_invoice_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("final_invoice_amount")),
                TotalQuantity = reader.IsDBNull(reader.GetOrdinal("total_quantity")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_quantity"))
            };
        }

        private SalePurTran MapSalePurTran(NpgsqlDataReader reader)
        {
            return new SalePurTran
            {
                SpTranUniqueCode = SafeGetInt(reader, "sp_tran_unique_code"),
                SpUniqueCode = SafeGetInt(reader, "sp_unique_code"),
                ProductId = reader.IsDBNull(reader.GetOrdinal("product_id")) ? (int?)null : SafeGetInt(reader, "product_id"),
                ProductDetails = SafeGetString(reader, "product_details"),
                BatchDate = SafeGetDateTime(reader, "batch_date"),
                BatchNo = SafeGetString(reader, "batch_no"),
                Mrp = reader.IsDBNull(reader.GetOrdinal("mrp")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("mrp")),
                SubUnit = reader.IsDBNull(reader.GetOrdinal("sub_unit")) ? (int?)null : SafeGetInt(reader, "sub_unit"),
                SubUnitQuantity = reader.IsDBNull(reader.GetOrdinal("sub_unit_quantity")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("sub_unit_quantity")),
                Unit = reader.IsDBNull(reader.GetOrdinal("unit")) ? (int?)null : SafeGetInt(reader, "unit"),
                Quantity = reader.GetDecimal(reader.GetOrdinal("quantity")),
                Rate = reader.GetDecimal(reader.GetOrdinal("rate")),
                DiscountPer = reader.GetDecimal(reader.GetOrdinal("discount_per")),
                Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                IgstPercent = reader.IsDBNull(reader.GetOrdinal("igst_percent")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("igst_percent")),
                IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
                CgstPercent = reader.IsDBNull(reader.GetOrdinal("cgst_percent")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("cgst_percent")),
                CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
                SgstPercent = reader.IsDBNull(reader.GetOrdinal("sgst_percent")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("sgst_percent")),
                SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
                Crt = reader.IsDBNull(reader.GetOrdinal("crt")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("crt")),
                Packing = SafeGetString(reader, "packing"),
                HsnCode = SafeGetString(reader, "hsn_code"),
                FiscalYearId = reader.IsDBNull(reader.GetOrdinal("fiscal_year_id")) ? (int?)null : SafeGetInt(reader, "fiscal_year_id"),
                BranchId = reader.IsDBNull(reader.GetOrdinal("branch_id")) ? (int?)null : SafeGetInt(reader, "branch_id"),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? (int?)null : SafeGetInt(reader, "created_by"),
                ModifiedBy = reader.IsDBNull(reader.GetOrdinal("modified_by")) ? (int?)null : SafeGetInt(reader, "modified_by"),
                DeletedBy = reader.IsDBNull(reader.GetOrdinal("deleted_by")) ? (int?)null : SafeGetInt(reader, "deleted_by"),
                DeleteFlag = SafeGetBool(reader, "delete_flag"),
                CreatedOn = SafeGetDateTime(reader, "created_on"),
                ModifiedOn = SafeGetDateTime(reader, "modified_on"),
                DeletedOn = SafeGetDateTime(reader, "deleted_on"),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("total_amount")),
                RateDiff = reader.IsDBNull(reader.GetOrdinal("rate_diff")) ? 0 : reader.GetDecimal(reader.GetOrdinal("rate_diff")),
                Cogs = reader.IsDBNull(reader.GetOrdinal("cogs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cogs")),
                DealerPrice = reader.IsDBNull(reader.GetOrdinal("dealer_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("dealer_price")),
                SpCommUnit = reader.IsDBNull(reader.GetOrdinal("sp_comm_unit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sp_comm_unit"))
            };
        }

        private void AddSalePurParameters(NpgsqlCommand cmd, SalePur entity)
        {
            var currentUserId = _tenantProvider?.UserId;
            var createdBy = entity.CreatedBy ?? currentUserId;
            var modifiedBy = entity.ModifiedBy ?? currentUserId;

            cmd.Parameters.Add("p_sp_unique_code", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.SpUniqueCode == null || entity.SpUniqueCode == 0 ? DBNull.Value : entity.SpUniqueCode;
            cmd.Parameters.Add("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.CompanyId;
            cmd.Parameters.Add("p_invoice_series", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.InvoiceSeries ?? DBNull.Value;
            cmd.Parameters.Add("p_invoice_series_increment_number", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.InvoiceSeriesIncrementNumber ?? DBNull.Value;
            cmd.Parameters.Add("p_inv_type", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.InvType ?? DBNull.Value;
            cmd.Parameters.Add("p_inv_sub_type", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.InvSubType ?? DBNull.Value;
            cmd.Parameters.Add("p_inv_category", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.InvCategory ?? DBNull.Value;
            cmd.Parameters.Add("p_sales_order_no", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.SalesOrderNo ?? DBNull.Value;
            cmd.Parameters.Add("p_sales_order_date", NpgsqlTypes.NpgsqlDbType.Date).Value = (object?)entity.SalesOrderDate ?? DBNull.Value;
            cmd.Parameters.Add("p_po_order_no", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.PoOrderNo ?? DBNull.Value;
            cmd.Parameters.Add("p_po_order_date", NpgsqlTypes.NpgsqlDbType.Date).Value = (object?)entity.PoOrderDate ?? DBNull.Value;
            cmd.Parameters.Add("p_bill_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.BillId ?? DBNull.Value;
            cmd.Parameters.Add("p_delivery_no", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.DeliveryNo ?? DBNull.Value;
            cmd.Parameters.Add("p_delivery_date", NpgsqlTypes.NpgsqlDbType.Date).Value = (object?)entity.DeliveryDate ?? DBNull.Value;
            cmd.Parameters.Add("p_vehicle_no", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.VehicleNo ?? DBNull.Value;
            cmd.Parameters.Add("p_transporter_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.TransporterId ?? DBNull.Value;
            cmd.Parameters.Add("p_unit_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.UnitId ?? DBNull.Value;
            cmd.Parameters.Add("p_bank_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.BankId ?? DBNull.Value;
            cmd.Parameters.Add("p_product_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.ProductId ?? DBNull.Value;
            cmd.Parameters.Add("p_tax_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.TaxId ?? DBNull.Value;
            cmd.Parameters.Add("p_net_invoice_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.NetInvoiceAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_grand_total", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.GrandTotal ?? DBNull.Value;
            cmd.Parameters.Add("p_billing_address", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)entity.BillingAddress ?? DBNull.Value;
            cmd.Parameters.Add("p_billing_address1", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.BillingAddress1 ?? DBNull.Value;
            cmd.Parameters.Add("p_billing_address2", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.BillingAddress2 ?? DBNull.Value;
            cmd.Parameters.Add("p_shipping_address", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)entity.ShippingAddress ?? DBNull.Value;
            cmd.Parameters.Add("p_shipping_address1", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.ShippingAddress1 ?? DBNull.Value;
            cmd.Parameters.Add("p_shipping_address2", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.ShippingAddress2 ?? DBNull.Value;
            cmd.Parameters.Add("p_tot_discount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.TotDiscount;
            cmd.Parameters.Add("p_tot_sku_in_invoice", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.TotSkuInInvoice;
            cmd.Parameters.Add("p_inv_no", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.InvNo ?? DBNull.Value;
            cmd.Parameters.Add("p_inv_date", NpgsqlTypes.NpgsqlDbType.Date).Value = entity.InvDate;
            cmd.Parameters.Add("p_due_date", NpgsqlTypes.NpgsqlDbType.Date).Value = (object?)entity.DueDate ?? DBNull.Value;
            cmd.Parameters.Add("p_party_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.PartyId;
            cmd.Parameters.Add("p_terms_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.TermsId ?? DBNull.Value;
            cmd.Parameters.Add("p_description", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)entity.Description ?? DBNull.Value;
            cmd.Parameters.Add("p_taxable_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.TaxableAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_igst_percent", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.IgstPercent ?? DBNull.Value;
            cmd.Parameters.Add("p_igst_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.IgstAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_cgst_percent", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.CgstPercent ?? DBNull.Value;
            cmd.Parameters.Add("p_cgst_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.CgstAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_sgst_percent", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.SgstPercent ?? DBNull.Value;
            cmd.Parameters.Add("p_sgst_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.SgstAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_round_off", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.RoundOff ?? DBNull.Value;
            cmd.Parameters.Add("p_previous_outstanding", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.PreviousOutstanding ?? DBNull.Value;
            cmd.Parameters.Add("p_current_bill_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.CurrentBillAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_final_outstanding", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.FinalOutstanding ?? DBNull.Value;
            cmd.Parameters.Add("p_jurisdiction", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.Jurisdiction ?? DBNull.Value;

            cmd.Parameters.Add("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.FiscalYearId ?? DBNull.Value;
            cmd.Parameters.Add("p_branch_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.BranchId ?? DBNull.Value;

            cmd.Parameters.Add("p_created_by", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)createdBy ?? DBNull.Value;
            cmd.Parameters.Add("p_modified_by", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)modifiedBy ?? DBNull.Value;
            cmd.Parameters.Add("p_deleted_by", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.DeletedBy ?? DBNull.Value;
            cmd.Parameters.Add("p_delete_flag", NpgsqlTypes.NpgsqlDbType.Boolean).Value = entity.DeleteFlag ?? false;
            cmd.Parameters.Add("p_rate_diff", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.RateDiff ?? 0;
            // [MODIFIED 2026-05-06] Added missing parameters to match latest DB function fn_sale_pur
            cmd.Parameters.Add("p_invoice_total", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.InvoiceTotal ?? DBNull.Value;
            cmd.Parameters.Add("p_final_invoice_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.FinalInvoiceAmount ?? DBNull.Value;
            AddQuantityParameter(cmd, "p_total_quantity", entity.TotalQuantity);
            cmd.Parameters.Add("p_salesperson_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.SalespersonId ?? DBNull.Value;
        }

        private void AddSalePurTranParameters(NpgsqlCommand cmd, SalePurTran entity)
        {
            var currentUserId = _tenantProvider?.UserId;
            var createdBy = entity.CreatedBy ?? currentUserId;
            var modifiedBy = entity.ModifiedBy ?? currentUserId;
            cmd.Parameters.Add("p_sp_tran_unique_code", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.SpTranUniqueCode == null || entity.SpTranUniqueCode == 0 ? DBNull.Value : entity.SpTranUniqueCode;
            cmd.Parameters.Add("p_sp_unique_code", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.SpUniqueCode;
            cmd.Parameters.Add("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = entity.CompanyId;
            cmd.Parameters.Add("p_product_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.ProductId ?? DBNull.Value;
            cmd.Parameters.Add("p_product_details", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)entity.ProductDetails ?? DBNull.Value;
            cmd.Parameters.Add("p_batch_date", NpgsqlTypes.NpgsqlDbType.Date).Value = (object?)entity.BatchDate ?? DBNull.Value;
            cmd.Parameters.Add("p_batch_no", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.BatchNo ?? DBNull.Value;
            cmd.Parameters.Add("p_mrp", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.Mrp ?? DBNull.Value;
            cmd.Parameters.Add("p_sub_unit", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.SubUnit ?? DBNull.Value;
            AddQuantityParameter(cmd, "p_sub_unit_quantity", entity.SubUnitQuantity);
            cmd.Parameters.Add("p_unit", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.Unit ?? DBNull.Value;
            AddQuantityParameter(cmd, "p_quantity", entity.Quantity);
            cmd.Parameters.Add("p_rate", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.Rate;
            cmd.Parameters.Add("p_discount_per", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.DiscountPer;
            cmd.Parameters.Add("p_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.Amount;
            cmd.Parameters.Add("p_igst_percent", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.IgstPercent ?? DBNull.Value;
            cmd.Parameters.Add("p_igst_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.IgstAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_cgst_percent", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.CgstPercent ?? DBNull.Value;
            cmd.Parameters.Add("p_cgst_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.CgstAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_sgst_percent", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.SgstPercent ?? DBNull.Value;
            cmd.Parameters.Add("p_sgst_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.SgstAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_total_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.TotalAmount;
            cmd.Parameters.Add("p_crt", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.Crt ?? DBNull.Value;
            cmd.Parameters.Add("p_packing", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.Packing ?? DBNull.Value;
            cmd.Parameters.Add("p_hsn_code", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object?)entity.HsnCode ?? DBNull.Value;

            cmd.Parameters.Add("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.FiscalYearId ?? DBNull.Value;
            cmd.Parameters.Add("p_branch_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.BranchId ?? DBNull.Value;

            cmd.Parameters.Add("p_created_by", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)createdBy ?? DBNull.Value;
            cmd.Parameters.Add("p_modified_by", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)modifiedBy ?? DBNull.Value;
            cmd.Parameters.Add("p_deleted_by", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.DeletedBy ?? DBNull.Value;
            cmd.Parameters.Add("p_delete_flag", NpgsqlTypes.NpgsqlDbType.Boolean).Value = entity.DeleteFlag ?? false;
            cmd.Parameters.Add("p_rate_diff", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.RateDiff ?? 0;
            cmd.Parameters.Add("p_cogs", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.Cogs ?? 0;
            cmd.Parameters.Add("p_dealer_price", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.DealerPrice ?? 0;
            cmd.Parameters.Add("p_sp_comm_unit", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.SpCommUnit ?? 0;
        }

		/// <summary>
		/// Computes the net outstanding for a party from tab_jv (strictly BEFORE the given invoiceDate).
		/// Net = SUM(debit entries) - SUM(credit entries) for the party account.
		/// Used for the 'Previous Outstanding' shown on the invoice.
		/// </summary>
		public async Task<decimal> GetPartyOutstandingAsync(int companyId, int partyId)
		{
			// Called from the form when selecting a customer (no date = use today)
			return await GetPartyOutstandingBeforeDateAsync(companyId, partyId, DateTime.Today.AddDays(1));
		}

		public async Task<List<SoftBook.Domain.DTOs.PartyBulkSummaryDto>> GetPartyRateAdjustmentSummaryBulkAsync(int companyId, string partyType, DateTime asOfDate)
		{
			var result = await ExecuteReaderAsync(@"
                WITH PartyAccounts AS (
                    SELECT 
                        party_id,
                        COALESCE(accounts_receivable_account, party_id) AS ar_acc,
                        COALESCE(accounts_payable_account, party_id) AS ap_acc
                    FROM public.tab_party_master
                    WHERE (@p_company_id = 0 OR company_id = @p_company_id)
                      AND party_type = ANY(string_to_array(@p_party_types, ','))
                ),
                JV_Accounts AS (
                    SELECT ar_acc AS acc FROM PartyAccounts
                    UNION
                    SELECT ap_acc AS acc FROM PartyAccounts
                ),
                JV_Dr_Bal AS (
                    SELECT j.dr_account_code AS acc, SUM(j.amount) as dr_amt
                    FROM tab_jv j
                    WHERE j.jv_comp_code = @p_company_id
                      AND j.jv_date < @p_as_of_date
                      AND COALESCE(j.delete_flag, false) = false
                      AND j.dr_account_code IN (SELECT acc FROM JV_Accounts)
                    GROUP BY j.dr_account_code
                ),
                JV_Cr_Bal AS (
                    SELECT j.cr_account_code AS acc, SUM(j.amount) as cr_amt
                    FROM tab_jv j
                    WHERE j.jv_comp_code = @p_company_id
                      AND j.jv_date < @p_as_of_date
                      AND COALESCE(j.delete_flag, false) = false
                      AND j.cr_account_code IN (SELECT acc FROM JV_Accounts)
                    GROUP BY j.cr_account_code
                ),
                Sales_Bal AS (
                    SELECT s.party_id, SUM(s.grand_total) as total
                    FROM public.tab_sale_pur s
                    WHERE s.company_id = @p_company_id
                      AND LOWER(TRIM(s.inv_type)) = 'sales'
                      AND s.inv_date >= (@p_as_of_date::date - INTERVAL '7 day')
                      AND s.inv_date < @p_as_of_date::date
                      AND COALESCE(s.delete_flag, FALSE) = FALSE
                      AND (@p_fiscal_year_id IS NULL OR s.fiscal_year_id = @p_fiscal_year_id)
                    GROUP BY s.party_id
                )
                SELECT 
                    p.party_id AS PartyId,
                    p.display_name AS DisplayName,
                    p.party_type AS PartyType,
                    p.payment_terms AS PaymentTerms,
                    COALESCE(p.rate_diff, 0) AS StoredRateDiff,
                    COALESCE(sales.total, 0) AS Last7DaysSalesTotal,
                    (COALESCE(dr.dr_amt, 0) + COALESCE(dr2.dr_amt, 0) - 
                     COALESCE(cr.cr_amt, 0) - COALESCE(cr2.cr_amt, 0) + 
                     CASE 
                        WHEN UPPER(p.opening_balance_type) = 'DR' THEN  COALESCE(p.opening_balance, 0)::NUMERIC
                        WHEN UPPER(p.opening_balance_type) = 'CR' THEN -COALESCE(p.opening_balance, 0)::NUMERIC
                        ELSE 0 
                     END) AS TotalOutstanding
                FROM public.tab_party_master p
                JOIN PartyAccounts pa ON pa.party_id = p.party_id
                LEFT JOIN Sales_Bal sales ON sales.party_id = p.party_id
                LEFT JOIN JV_Dr_Bal dr ON dr.acc = pa.ar_acc
                LEFT JOIN JV_Dr_Bal dr2 ON dr2.acc = pa.ap_acc AND pa.ar_acc <> pa.ap_acc
                LEFT JOIN JV_Cr_Bal cr ON cr.acc = pa.ar_acc
                LEFT JOIN JV_Cr_Bal cr2 ON cr2.acc = pa.ap_acc AND pa.ar_acc <> pa.ap_acc
                WHERE (@p_company_id = 0 OR p.company_id = @p_company_id)
                  AND p.party_type = ANY(string_to_array(@p_party_types, ','));
            ", cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
				cmd.Parameters.AddWithValue("p_party_types", partyType ?? "Customer,Trader");
				cmd.Parameters.Add(new NpgsqlParameter("p_as_of_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = asOfDate.Date });
				cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
				{
					Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
				});
			}, reader => new SoftBook.Domain.DTOs.PartyBulkSummaryDto
			{
				PartyId = reader["PartyId"] != DBNull.Value ? Convert.ToInt32(reader["PartyId"]) : 0,
				DisplayName = reader["DisplayName"] != DBNull.Value ? Convert.ToString(reader["DisplayName"]) ?? string.Empty : string.Empty,
				PartyType = reader["PartyType"] != DBNull.Value ? Convert.ToString(reader["PartyType"]) ?? string.Empty : string.Empty,
				PaymentTerms = reader["PaymentTerms"] != DBNull.Value ? Convert.ToString(reader["PaymentTerms"]) : null,
				StoredRateDiff = reader["StoredRateDiff"] != DBNull.Value ? Convert.ToDecimal(reader["StoredRateDiff"]) : 0m,
				Last7DaysSalesTotal = reader["Last7DaysSalesTotal"] != DBNull.Value ? Convert.ToDecimal(reader["Last7DaysSalesTotal"]) : 0m,
				TotalOutstanding = reader["TotalOutstanding"] != DBNull.Value ? Convert.ToDecimal(reader["TotalOutstanding"]) : 0m
			}, CommandType.Text);

			return result.ToList();
		}

		public async Task<decimal> GetPartyOutstandingBeforeDateAsync(int companyId, int partyId, DateTime beforeDate)
		{
			var result = await ExecuteScalarAsync<object>(@"
                WITH party_info AS (
                    SELECT 
                        -- If accounts_receivable_account is NULL, fall back to party_id itself
                        COALESCE(accounts_receivable_account, @p_party_id) AS ar_account,
                        COALESCE(accounts_payable_account,   @p_party_id) AS ap_account,
                        CASE 
                            WHEN UPPER(opening_balance_type) = 'DR' THEN  COALESCE(opening_balance, 0)::NUMERIC
                            WHEN UPPER(opening_balance_type) = 'CR' THEN -COALESCE(opening_balance, 0)::NUMERIC
                            ELSE 0 
                        END AS opening_balance
                    FROM tab_party_master
                    WHERE party_id = @p_party_id
                    LIMIT 1
                )
                SELECT 
                    -- 1. Net JV balance strictly BEFORE this invoice date
                    COALESCE((
                        SELECT
                            SUM(CASE WHEN j.dr_account_code IN (p.ar_account, p.ap_account) THEN j.amount ELSE 0 END) -
                            SUM(CASE WHEN j.cr_account_code IN (p.ar_account, p.ap_account) THEN j.amount ELSE 0 END)
                        FROM tab_jv j
                        CROSS JOIN party_info p
                        WHERE j.jv_comp_code = @p_company_id
                          AND (j.dr_account_code IN (p.ar_account, p.ap_account)
                               OR j.cr_account_code IN (p.ar_account, p.ap_account))
                          AND j.jv_date::DATE < @p_before_date
                          AND (j.delete_flag = false OR j.delete_flag IS NULL)
                    ), 0) +
                    -- 2. Static opening balance from party creation
                    COALESCE((SELECT opening_balance FROM party_info), 0)",
			cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
				cmd.Parameters.AddWithValue("p_party_id", partyId);
				cmd.Parameters.Add(new NpgsqlParameter("p_before_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = beforeDate });
			}, CommandType.Text);

			if (result != null && result != DBNull.Value)
			{
				if (result is decimal d) return d;
				if (decimal.TryParse(result.ToString(), out decimal parsed)) return parsed;
			}
			return 0;
		}

		public async Task<decimal> GetTodaysReceiptAsync(int companyId, int partyId, DateTime date)
		{
			var result = await ExecuteScalarAsync<object>(@"
                WITH party_info AS (
                    SELECT 
                        COALESCE(accounts_receivable_account, @p_party_id) AS ar_account,
                        COALESCE(accounts_payable_account,   @p_party_id) AS ap_account
                    FROM tab_party_master
                    WHERE party_id = @p_party_id
                    LIMIT 1
                )
                SELECT COALESCE(
                    SUM(CASE WHEN j.cr_account_code IN (p.ar_account, p.ap_account) THEN j.amount ELSE 0 END)
                , 0)
                FROM tab_jv j
                CROSS JOIN party_info p
                WHERE j.jv_comp_code = @p_company_id
                  AND j.jv_date::DATE = @p_date
                  AND j.cr_account_code IN (p.ar_account, p.ap_account)
                  AND (j.delete_flag = false OR j.delete_flag IS NULL)",
			cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
				cmd.Parameters.AddWithValue("p_party_id", partyId);
				cmd.Parameters.Add(new NpgsqlParameter("p_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = date });
			}, CommandType.Text);

			if (result != null && result != DBNull.Value)
			{
				if (result is decimal d) return d;
				if (decimal.TryParse(result.ToString(), out decimal parsed)) return parsed;
			}
			return 0;
		}

		public async Task<decimal> GetTodaysInvoiceAmountAsync(int companyId, int partyId, DateTime date)
		{
			var result = await ExecuteScalarAsync<object>(@"
                SELECT COALESCE(SUM(grand_total), 0)
                FROM public.tab_sale_pur
                WHERE company_id = @p_company_id
                  AND party_id = @p_party_id
                  AND inv_type = 'Sales'
                  AND inv_date::DATE = @p_date
                  AND (delete_flag = false OR delete_flag IS NULL)",
			cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
				cmd.Parameters.AddWithValue("p_party_id", partyId);
				cmd.Parameters.Add(new NpgsqlParameter("p_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = date });
			}, CommandType.Text);

			if (result != null && result != DBNull.Value)
			{
				if (result is decimal d) return d;
				if (decimal.TryParse(result.ToString(), out decimal parsed)) return parsed;
			}
			return 0;
		}

		public async Task<decimal> GetPartyLast7DaysSalesTotalAsync(int companyId, int partyId)
		{
			return await GetPartyLast7DaysSalesTotalAsOfDateAsync(companyId, partyId, DateTime.Today);
		}

		public async Task<decimal> GetPartyLast7DaysSalesTotalAsOfDateAsync(int companyId, int partyId, DateTime asOfDate)
		{
			var result = await ExecuteScalarAsync<object>(@"
                SELECT COALESCE(SUM(grand_total), 0)
                FROM public.tab_sale_pur
                WHERE company_id = @p_company_id
                  AND party_id = @p_party_id
                  AND LOWER(TRIM(inv_type)) = 'sales'
                  AND inv_date::date BETWEEN (@p_as_of_date - INTERVAL '7 day')::date AND (@p_as_of_date - INTERVAL '1 day')::date
                  AND COALESCE(delete_flag, FALSE) = FALSE
                  AND (@p_fiscal_year_id IS NULL OR fiscal_year_id = @p_fiscal_year_id)",
			cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
				cmd.Parameters.AddWithValue("p_party_id", partyId);
				cmd.Parameters.Add(new NpgsqlParameter("p_as_of_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = asOfDate.Date });
				cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
				{
					Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
				});
			}, CommandType.Text);

			if (result != null && result != DBNull.Value)
			{
				if (result is decimal d) return d;
				if (decimal.TryParse(result.ToString(), out decimal parsed)) return parsed;
			}
			return 0;
		}

		/*
		Old KPI mismatch approach kept for reference only.
		This method fetched invoices first, and the service then calculated expected add price
		one invoice/party/date at a time. For year filters that created many database round trips.
		Use GetSalesInvoiceAddPriceMismatchesAsync instead; it does the full calculation in one SQL query.
		public async Task<List<SalesInvoiceAddPriceCheckDto>> GetSalesInvoicesForAddPriceCheckAsync(
			int companyId,
			DateTime fromDate,
			DateTime toDate,
			string? partyType = null)
		{
			return await ExecuteReaderAsync(@"
                WITH FilteredSales AS (
                    SELECT sp.sp_unique_code, sp.inv_no, sp.inv_date, sp.due_date, sp.party_id, sp.rate_diff, sp.company_id, sp.fiscal_year_id
                    FROM public.tab_sale_pur sp
                    WHERE sp.company_id = @p_company_id
                      AND LOWER(TRIM(sp.inv_type)) = 'sales'
                      AND sp.inv_date >= @p_from_date 
                      AND sp.inv_date < (@p_to_date::date + INTERVAL '1 day')
                      AND COALESCE(sp.delete_flag, FALSE) = FALSE
                      AND (@p_fiscal_year_id IS NULL OR sp.fiscal_year_id = @p_fiscal_year_id)
                )
                SELECT
                    sp.sp_unique_code AS SpUniqueCode,
                    COALESCE(sp.inv_no, '') AS InvNo,
                    sp.inv_date AS InvDate,
                    sp.due_date AS DueDate,
                    sp.party_id AS PartyId,
                    COALESCE(pm.display_name, '') AS DisplayName,
                    COALESCE(pm.party_type, '') AS PartyType,
                    COALESCE(line_adj.actual_add_price, sp.rate_diff, 0) AS ActualAddPrice
                FROM FilteredSales sp
                INNER JOIN public.tab_party_master pm
                    ON pm.party_id = sp.party_id
                   AND pm.company_id = sp.company_id
                LEFT JOIN LATERAL (
                    SELECT MAX(COALESCE(t.rate_diff, 0)) AS actual_add_price
                    FROM public.tab_sal_pur_tran t
                    WHERE t.sp_unique_code = sp.sp_unique_code
                      AND COALESCE(t.delete_flag, FALSE) = FALSE
                ) line_adj ON TRUE
                WHERE (@p_party_type IS NULL OR pm.party_type = ANY(string_to_array(@p_party_type, ',')))
                ORDER BY sp.inv_date DESC, sp.sp_unique_code DESC",
			cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
				cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate.Date });
				cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate.Date });
				cmd.Parameters.AddWithValue("p_party_type", (object?)partyType ?? DBNull.Value);
				cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
				{
					Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
				});
			},
			reader => new SalesInvoiceAddPriceCheckDto
			{
				SpUniqueCode = Convert.ToInt32(reader["SpUniqueCode"]),
				InvNo = reader["InvNo"]?.ToString() ?? string.Empty,
				InvDate = Convert.ToDateTime(reader["InvDate"]),
				DueDate = reader["DueDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["DueDate"]),
				PartyId = Convert.ToInt32(reader["PartyId"]),
				DisplayName = reader["DisplayName"]?.ToString() ?? string.Empty,
				PartyType = reader["PartyType"]?.ToString() ?? string.Empty,
				ActualAddPrice = reader["ActualAddPrice"] != DBNull.Value ? Convert.ToDecimal(reader["ActualAddPrice"]) : 0
			},
			CommandType.Text);
		}
		*/

		public async Task<List<PartyInvoiceAddPriceMismatchDto>> GetSalesInvoiceAddPriceMismatchesAsync(
			int companyId,
			DateTime fromDate,
			DateTime toDate,
			string? partyType = null)
		{
			return await ExecuteReaderAsync(@"
                WITH FilteredSales AS (
                    SELECT
                        sp.sp_unique_code,
                        sp.inv_no,
                        sp.inv_date::date AS inv_date,
                        sp.due_date,
                        sp.party_id,
                        sp.company_id,
                        COALESCE(sp.rate_diff, 0) AS header_rate_diff
                    FROM public.tab_sale_pur sp
                    WHERE sp.company_id = @p_company_id
                      AND LOWER(TRIM(sp.inv_type)) = 'sales'
                      AND sp.inv_date >= @p_from_date
                      AND sp.inv_date < (@p_to_date::date + INTERVAL '1 day')
                      AND COALESCE(sp.delete_flag, FALSE) = FALSE
                      AND (@p_fiscal_year_id IS NULL OR sp.fiscal_year_id = @p_fiscal_year_id)
                ),
                InvoiceParties AS (
                    SELECT
                        fs.*,
                        COALESCE(pm.display_name, '') AS display_name,
                        COALESCE(pm.party_type, '') AS party_type,
                        COALESCE(pm.rate_diff, 0) AS stored_rate_diff,
                        COALESCE(pm.accounts_receivable_account, fs.party_id) AS ar_acc,
                        COALESCE(pm.accounts_payable_account, fs.party_id) AS ap_acc,
                        CASE
                            WHEN UPPER(pm.opening_balance_type) = 'DR' THEN COALESCE(pm.opening_balance, 0)::NUMERIC
                            WHEN UPPER(pm.opening_balance_type) = 'CR' THEN -COALESCE(pm.opening_balance, 0)::NUMERIC
                            ELSE 0
                        END AS opening_balance
                    FROM FilteredSales fs
                    INNER JOIN public.tab_party_master pm
                        ON pm.party_id = fs.party_id
                       AND pm.company_id = fs.company_id
                    WHERE (@p_party_type IS NULL OR pm.party_type = ANY(string_to_array(@p_party_type, ',')))
                ),
                InvoiceKeys AS (
                    SELECT DISTINCT party_id, inv_date, ar_acc, ap_acc, opening_balance
                    FROM InvoiceParties
                ),
                ActualAddPrice AS (
                    SELECT
                        t.sp_unique_code,
                        MAX(COALESCE(t.rate_diff, 0)) AS actual_add_price
                    FROM public.tab_sal_pur_tran t
                    INNER JOIN InvoiceParties ip ON ip.sp_unique_code = t.sp_unique_code
                    WHERE COALESCE(t.delete_flag, FALSE) = FALSE
                    GROUP BY t.sp_unique_code
                ),
                JVBalance AS (
                    SELECT
                        k.party_id,
                        k.inv_date,
                        SUM(
                            CASE WHEN j.dr_account_code IN (k.ar_acc, k.ap_acc) THEN j.amount ELSE 0 END
                            - CASE WHEN j.cr_account_code IN (k.ar_acc, k.ap_acc) THEN j.amount ELSE 0 END
                        ) AS jv_balance
                    FROM InvoiceKeys k
                    INNER JOIN public.tab_jv j
                        ON j.jv_comp_code = @p_company_id
                       AND j.jv_date < k.inv_date
                       AND COALESCE(j.delete_flag, FALSE) = FALSE
                       AND (
                            j.dr_account_code IN (k.ar_acc, k.ap_acc)
                            OR j.cr_account_code IN (k.ar_acc, k.ap_acc)
                       )
                    GROUP BY k.party_id, k.inv_date
                ),
                Last7DaysSales AS (
                    SELECT
                        k.party_id,
                        k.inv_date,
                        SUM(s.grand_total) AS last_7_days_sales_total
                    FROM InvoiceKeys k
                    INNER JOIN public.tab_sale_pur s
                        ON s.company_id = @p_company_id
                       AND s.party_id = k.party_id
                       AND LOWER(TRIM(s.inv_type)) = 'sales'
                       AND s.inv_date >= (k.inv_date - INTERVAL '7 day')
                       AND s.inv_date < k.inv_date
                       AND COALESCE(s.delete_flag, FALSE) = FALSE
                       AND (@p_fiscal_year_id IS NULL OR s.fiscal_year_id = @p_fiscal_year_id)
                    GROUP BY k.party_id, k.inv_date
                ),
                Calculated AS (
                    SELECT
                        ip.*,
                        COALESCE(aa.actual_add_price, ip.header_rate_diff, 0) AS actual_add_price,
                        COALESCE(jv.jv_balance, 0) + ip.opening_balance AS total_outstanding,
                        COALESCE(s7.last_7_days_sales_total, 0) AS last_7_days_sales_total,
                        COALESCE(s7.last_7_days_sales_total, 0) / 7.0 AS avg_sale_per_day
                    FROM InvoiceParties ip
                    LEFT JOIN ActualAddPrice aa ON aa.sp_unique_code = ip.sp_unique_code
                    LEFT JOIN JVBalance jv ON jv.party_id = ip.party_id AND jv.inv_date = ip.inv_date
                    LEFT JOIN Last7DaysSales s7 ON s7.party_id = ip.party_id AND s7.inv_date = ip.inv_date
                ),
                WithDays AS (
                    SELECT
                        c.*,
                        CASE
                            WHEN c.avg_sale_per_day > 0
                                THEN GREATEST(c.total_outstanding / c.avg_sale_per_day, 0)
                            ELSE 0
                        END AS avg_outstanding_days
                    FROM Calculated c
                ),
                WithExpected AS (
                    SELECT
                        wd.*,
                        COALESCE(ap.add_price, 0) AS expected_add_price,
                        COALESCE(ap.from_due, 0) AS from_due,
                        COALESCE(ap.to_due, 0) AS to_due
                    FROM WithDays wd
                    LEFT JOIN LATERAL (
                        SELECT ap.add_price, ap.from_due, ap.to_due
                        FROM public.tab_add_price ap
                        WHERE wd.avg_outstanding_days >= ap.from_due
                          AND wd.avg_outstanding_days <= ap.to_due
                          AND (ap.company_id IS NULL OR ap.company_id = @p_company_id)
                        ORDER BY ap.rowid
                        LIMIT 1
                    ) ap ON TRUE
                )
                SELECT
                    sp_unique_code AS SpUniqueCode,
                    COALESCE(inv_no, '') AS InvNo,
                    inv_date AS InvDate,
                    due_date AS DueDate,
                    party_id AS PartyId,
                    display_name AS DisplayName,
                    party_type AS PartyType,
                    expected_add_price AS ExpectedAddPrice,
                    actual_add_price AS ActualAddPrice,
                    actual_add_price - expected_add_price AS Difference,
                    total_outstanding AS TotalOutstanding,
                    last_7_days_sales_total AS Last7DaysSalesTotal,
                    avg_sale_per_day AS AvgSalePerDay,
                    avg_outstanding_days AS AvgOutstandingDays,
                    from_due AS FromDue,
                    to_due AS ToDue
                FROM WithExpected
                WHERE expected_add_price <> actual_add_price
                ORDER BY inv_date DESC, display_name",
			cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
				cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate.Date });
				cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate.Date });
				cmd.Parameters.AddWithValue("p_party_type", (object?)partyType ?? DBNull.Value);
				cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
				{
					Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
				});
			},
			reader => new PartyInvoiceAddPriceMismatchDto
			{
				SpUniqueCode = Convert.ToInt32(reader["SpUniqueCode"]),
				InvNo = reader["InvNo"]?.ToString() ?? string.Empty,
				InvDate = Convert.ToDateTime(reader["InvDate"]),
				DueDate = reader["DueDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["DueDate"]),
				PartyId = Convert.ToInt32(reader["PartyId"]),
				DisplayName = reader["DisplayName"]?.ToString() ?? string.Empty,
				PartyType = reader["PartyType"]?.ToString() ?? string.Empty,
				ExpectedAddPrice = reader["ExpectedAddPrice"] != DBNull.Value ? Math.Round(Convert.ToDecimal(reader["ExpectedAddPrice"]), 2) : 0,
				ActualAddPrice = reader["ActualAddPrice"] != DBNull.Value ? Math.Round(Convert.ToDecimal(reader["ActualAddPrice"]), 2) : 0,
				Difference = reader["Difference"] != DBNull.Value ? Math.Round(Convert.ToDecimal(reader["Difference"]), 2) : 0,
				TotalOutstanding = reader["TotalOutstanding"] != DBNull.Value ? Math.Round(Convert.ToDecimal(reader["TotalOutstanding"]), 2) : 0,
				Last7DaysSalesTotal = reader["Last7DaysSalesTotal"] != DBNull.Value ? Math.Round(Convert.ToDecimal(reader["Last7DaysSalesTotal"]), 2) : 0,
				AvgSalePerDay = reader["AvgSalePerDay"] != DBNull.Value ? Math.Round(Convert.ToDecimal(reader["AvgSalePerDay"]), 2) : 0,
				AvgOutstandingDays = reader["AvgOutstandingDays"] != DBNull.Value ? Math.Round(Convert.ToDecimal(reader["AvgOutstandingDays"]), 2) : 0,
				FromDue = reader["FromDue"] != DBNull.Value ? Convert.ToDecimal(reader["FromDue"]) : 0,
				ToDue = reader["ToDue"] != DBNull.Value ? Convert.ToDecimal(reader["ToDue"]) : 0
			},
			CommandType.Text);
		}

		public async Task<decimal> GetAddPriceForDueDaysAsync(int companyId, decimal avgOutstandingDays)
		{
			var lookup = await GetAddPriceLookupForDueDaysAsync(companyId, avgOutstandingDays);
			return lookup.AddPrice;
		}

		public async Task<List<AddPriceLookupDto>> GetAllAddPriceLookupsAsync(int companyId)
		{
			var rows = await ExecuteReaderAsync(@"
                SELECT COALESCE(ap.add_price, 0) AS add_price,
                       COALESCE(ap.sale_per_comission, 0) AS sale_per_comission,
                       COALESCE(ap.from_due, 0) AS from_due,
                       COALESCE(ap.to_due, 0) AS to_due
                FROM public.tab_add_price ap
                WHERE (ap.company_id IS NULL OR ap.company_id = @p_company_id)
                ORDER BY ap.from_due",
			cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
			}, reader => new AddPriceLookupDto
			{
				AddPrice = reader["add_price"] != DBNull.Value ? Convert.ToDecimal(reader["add_price"]) : 0,
				SalePerComission = reader["sale_per_comission"] != DBNull.Value ? Convert.ToDecimal(reader["sale_per_comission"]) : 0,
				FromDue = reader["from_due"] != DBNull.Value ? Convert.ToDecimal(reader["from_due"]) : 0,
				ToDue = reader["to_due"] != DBNull.Value ? Convert.ToDecimal(reader["to_due"]) : 0
			}, CommandType.Text);

			return rows.ToList();
		}

		public async Task<AddPriceLookupDto> GetAddPriceLookupForDueDaysAsync(int companyId, decimal avgOutstandingDays)
		{
			var rows = await ExecuteReaderAsync(@"
                SELECT COALESCE(ap.add_price, 0) AS add_price,
                       COALESCE(ap.sale_per_comission, 0) AS sale_per_comission,
                       COALESCE(ap.from_due, 0) AS from_due,
                       COALESCE(ap.to_due, 0) AS to_due
                FROM public.tab_add_price ap
                WHERE @p_avg_os_days >= ap.from_due
                  AND @p_avg_os_days <= ap.to_due
                  AND (ap.company_id IS NULL OR ap.company_id = @p_company_id)
                ORDER BY ap.rowid
                LIMIT 1",
			cmd =>
			{
				cmd.Parameters.AddWithValue("p_company_id", companyId);
				cmd.Parameters.AddWithValue("p_avg_os_days", avgOutstandingDays);
			},
			reader => new AddPriceLookupDto
			{
				AddPrice = reader["add_price"] != DBNull.Value ? Convert.ToDecimal(reader["add_price"]) : 0,
				SalePerComission = reader["sale_per_comission"] != DBNull.Value ? Convert.ToDecimal(reader["sale_per_comission"]) : 0,
				FromDue = reader["from_due"] != DBNull.Value ? Convert.ToDecimal(reader["from_due"]) : 0,
				ToDue = reader["to_due"] != DBNull.Value ? Convert.ToDecimal(reader["to_due"]) : 0
			},
			CommandType.Text);

			return rows.Count > 0
				? rows[0]
				: new AddPriceLookupDto();
		}
	}
}
