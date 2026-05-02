using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
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

        public async Task<DbResult> InsertAsync(SalePur entity, int? userId = null)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_sale_pur", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "INSERT");
                    AddSalePurParameters(cmd, entity);
                    cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
                }, MapSalePur);
                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> UpdateAsync(SalePur entity, int? userId = null)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_sale_pur", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "UPDATE");
                    AddSalePurParameters(cmd, entity);
                    cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
                }, MapSalePur);
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

        public async Task<DbResult> InsertLineItemAsync(SalePurTran entity)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_sal_pur_tran", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "INSERT");
                    AddSalePurTranParameters(cmd, entity);
                }, MapSalePurTran);
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
                Cogs = reader.IsDBNull(reader.GetOrdinal("cogs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cogs"))
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
            cmd.Parameters.Add("p_invoice_total", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.InvoiceTotal ?? DBNull.Value;
            cmd.Parameters.Add("p_final_invoice_amount", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.FinalInvoiceAmount ?? DBNull.Value;
            cmd.Parameters.Add("p_total_quantity", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.TotalQuantity;
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
            cmd.Parameters.Add("p_sub_unit_quantity", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)entity.SubUnitQuantity ?? DBNull.Value;
            cmd.Parameters.Add("p_unit", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)entity.Unit ?? DBNull.Value;
            cmd.Parameters.Add("p_quantity", NpgsqlTypes.NpgsqlDbType.Numeric).Value = entity.Quantity;
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

	}
}
