//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using Npgsql;
//using SoftBook.Domain.Data;
//using SoftBook.Domain.DTOs;
//using SoftBook.Domain.RepositoryInterfaces;

//namespace SoftBook.Infrastructure.Classes
//{
//    public class ReportRepository : BaseDAL, IReportRepository
//    {
//        public ReportRepository(IConfiguration configuration, ITenantProvider tenantProvider) 
//            : base(configuration, tenantProvider)
//        {
//        }

//        public async Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate)
//        {
//            // Calling the V2 function which resolves the ambiguity issue
//            var entries = await ExecuteReaderAsync("SELECT * FROM fn_ledger_report_v2(@p_account_code, @p_from_date, @p_to_date)", cmd =>
//            {
//                cmd.Parameters.AddWithValue("p_account_code", accountCode);

//                // Explicitly send as DATE to match SQL function signature (INT, DATE, DATE)
//                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
//                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
//            }, reader =>
//            {
//                // Helper to safely get values that might be returned in different formats (TEXT, NUMERIC, etc)
//                long? GetLong(string name)
//                {
//                    int ordinal = reader.GetOrdinal(name);
//                    if (reader.IsDBNull(ordinal)) return null;
//                    var val = reader.GetValue(ordinal);
//                    if (val is long l) return l;
//                    if (val is int i) return i;
//                    if (val is decimal d) return (long)d;
//                    if (long.TryParse(val.ToString(), out long parsed)) return parsed;
//                    return null;
//                }

//                return new LedgerEntryDTO
//                {
//                    // Map result columns with "res_" prefix to avoid ambiguity in Postgres
//                    JvDate = reader.GetDateTime(reader.GetOrdinal("res_jv_date")),
//                    VType = reader.IsDBNull(reader.GetOrdinal("res_v_type")) ? null : reader.GetString(reader.GetOrdinal("res_v_type")),
//                    VCode = GetLong("res_v_code"),
//                    Description = reader.IsDBNull(reader.GetOrdinal("res_description")) ? null : reader.GetString(reader.GetOrdinal("res_description")),
//                    Debit = reader.IsDBNull(reader.GetOrdinal("res_debit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("res_debit")),
//                    Credit = reader.IsDBNull(reader.GetOrdinal("res_credit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("res_credit")),
//                    Balance = reader.IsDBNull(reader.GetOrdinal("res_balance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("res_balance")),
//                    Id = GetLong("res_id")
//                };
//            }, CommandType.Text);

//            var report = new LedgerReportDTO
//            {
//                AccountCode = accountCode,
//                Entries = entries
//            };

//            if (entries.Count > 0)
//            {
//                // The first row is guaranteed to be "Opening Balance" by our SQL function wrapper
//                report.OpeningBalance = entries.First().Balance;

//                // The last row has the final running balance
//                report.ClosingBalance = entries.Last().Balance;
//            }

//            return report;
//        }

//        public async Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(
//    int companyId,
//    DateTime? fromDate,
//    DateTime? toDate,
//    string? productName,
//    string? customerName,
//    string? invoiceNo,
//    string? invType)
//        {
//            return await ExecuteReaderAsync(
//                "SELECT * FROM fn_sale_purchase_register_itemwise(@p_company_id, @p_from_date, @p_to_date, @p_product_name, @p_customer_name, @p_invoice_no, @p_inv_type)",
//                cmd =>
//                {
//                    object Normalize(string? val)
//                    {
//                        return string.IsNullOrWhiteSpace(val) ? DBNull.Value : val.Trim();
//                    }

//                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
//                    {
//                        Value = companyId
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date)
//                    {
//                        Value = (object?)fromDate ?? DBNull.Value
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date)
//                    {
//                        Value = (object?)toDate ?? DBNull.Value
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_product_name", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(productName)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_customer_name", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(customerName)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_invoice_no", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(invoiceNo)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(invType)
//                    });
//                },
//                reader =>
//                {
//                    return new SalePurchaseRegisterItemwiseDTO
//                    {
//                        InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
//                        InvNo = reader.IsDBNull(reader.GetOrdinal("inv_no")) ? null : reader.GetString(reader.GetOrdinal("inv_no")),
//                        InvType = reader.IsDBNull(reader.GetOrdinal("inv_type")) ? null : reader.GetString(reader.GetOrdinal("inv_type")),
//                        CustomerName = reader.IsDBNull(reader.GetOrdinal("customer_name")) ? null : reader.GetString(reader.GetOrdinal("customer_name")),
//                        ItemName = reader.IsDBNull(reader.GetOrdinal("item_name")) ? null : reader.GetString(reader.GetOrdinal("item_name")),
//                        Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),
//                        AvgPrice = reader.IsDBNull(reader.GetOrdinal("avg_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avg_price")),
//                        TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
//                        Igst = reader.IsDBNull(reader.GetOrdinal("igst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst")),
//                        Cgst = reader.IsDBNull(reader.GetOrdinal("cgst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst")),
//                        Sgst = reader.IsDBNull(reader.GetOrdinal("sgst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst")),
//                        TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
//                        ExpDiscount = reader.IsDBNull(reader.GetOrdinal("exp_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("exp_discount")),
//                        TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
//                        RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
//                        FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
//                        AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_price"))
//                    };
//                },
//                CommandType.Text
//            );
//        }
//        public async Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(
//            int companyId,
//            DateTime? fromDate,
//            DateTime? toDate,
//            string? productName,
//            string? customerName,
//            string? invoiceNo,
//            string? invType)
//        {
//            return await ExecuteReaderAsync(
//                "SELECT * FROM fn_sale_purchase_register_datewise(@p_company_id, @p_from_date, @p_to_date, @p_product_name, @p_customer_name, @p_invoice_no, @p_inv_type)",
//                cmd =>
//                {
//                    object Normalize(string? val)
//                    {
//                        return string.IsNullOrWhiteSpace(val) ? DBNull.Value : val.Trim();
//                    }

//                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
//                    {
//                        Value = companyId
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date)
//                    {
//                        Value = (object?)fromDate ?? DBNull.Value
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date)
//                    {
//                        Value = (object?)toDate ?? DBNull.Value
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_product_name", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(productName)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_customer_name", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(customerName)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_invoice_no", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(invoiceNo)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(invType)
//                    });
//                },
//                reader =>
//                {
//                    return new SalePurchaseRegisterDatewiseDTO
//                    {
//                        InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
//                        InvType = reader.IsDBNull(reader.GetOrdinal("inv_type")) ? null : reader.GetString(reader.GetOrdinal("inv_type")),
//                        TotalQty = reader.IsDBNull(reader.GetOrdinal("total_qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_qty")),
//                        AvgPrice = reader.IsDBNull(reader.GetOrdinal("avg_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avg_price")),
//                        TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
//                        Igst = reader.IsDBNull(reader.GetOrdinal("igst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst")),
//                        Cgst = reader.IsDBNull(reader.GetOrdinal("cgst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst")),
//                        Sgst = reader.IsDBNull(reader.GetOrdinal("sgst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst")),
//                        TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
//                        TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
//                        ExpDiscount = reader.IsDBNull(reader.GetOrdinal("exp_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("exp_discount")),
//                        RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
//                        FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
//                        AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_price")),
//                        AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
//                    };
//                },
//                CommandType.Text
//            );
//        }
//        public async Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(
//            int companyId,
//            DateTime? fromDate,
//            DateTime? toDate,
//            string? productName,
//            string? customerName,
//            string? invoiceNo,
//            string? invType)
//        {
//            return await ExecuteReaderAsync(
//                "SELECT * FROM fn_sale_purchase_register_partywise(@p_company_id, @p_from_date, @p_to_date, @p_product_name, @p_customer_name, @p_invoice_no, @p_inv_type)",
//                cmd =>
//                {
//                    object Normalize(string? val)
//                    {
//                        return string.IsNullOrWhiteSpace(val) ? DBNull.Value : val.Trim();
//                    }

//                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
//                    {
//                        Value = companyId
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date)
//                    {
//                        Value = (object?)fromDate ?? DBNull.Value
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date)
//                    {
//                        Value = (object?)toDate ?? DBNull.Value
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_product_name", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(productName)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_customer_name", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(customerName)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_invoice_no", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(invoiceNo)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(invType)
//                    });
//                },
//                reader =>
//                {
//                    return new SalePurchaseRegisterPartywiseDTO
//                    {
//                        CustomerName = reader.IsDBNull(reader.GetOrdinal("customer_name")) ? null : reader.GetString(reader.GetOrdinal("customer_name")),
//                        InvType = reader.IsDBNull(reader.GetOrdinal("inv_type")) ? null : reader.GetString(reader.GetOrdinal("inv_type")),
//                        TotalQty = reader.IsDBNull(reader.GetOrdinal("total_qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_qty")),
//                        AvgPrice = reader.IsDBNull(reader.GetOrdinal("avg_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avg_price")),
//                        TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
//                        Igst = reader.IsDBNull(reader.GetOrdinal("igst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst")),
//                        Cgst = reader.IsDBNull(reader.GetOrdinal("cgst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst")),
//                        Sgst = reader.IsDBNull(reader.GetOrdinal("sgst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst")),
//                        TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
//                        TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
//                        ExpDiscount = reader.IsDBNull(reader.GetOrdinal("exp_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("exp_discount")),
//                        RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
//                        FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
//                        AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_price")),
//                        AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
//                    };
//                },
//                CommandType.Text
//            );
//        }

//        public async Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(
//            int companyId,
//            DateTime? fromDate,
//            DateTime? toDate,
//            string? productName,
//            string? customerName,
//            string? invoiceNo,
//            string? invType)
//        {
//            Console.WriteLine($"API PARAMS => companyId={companyId}, invType={invType}");

//            var result = await ExecuteReaderAsync(
//                "SELECT * FROM fn_sale_purchase_register_invwise(@p_company_id, @p_from_date, @p_to_date, @p_product_name, @p_customer_name, @p_invoice_no, @p_inv_type)",
//                cmd =>
//                {
//                    object Normalize(string? val)
//                    {
//                        return string.IsNullOrWhiteSpace(val) ? DBNull.Value : val.Trim();
//                    }

//                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
//                    {
//                        Value = companyId
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date)
//                    {
//                        Value = (object?)fromDate ?? DBNull.Value
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date)
//                    {
//                        Value = (object?)toDate ?? DBNull.Value
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_product_name", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(productName)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_customer_name", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(customerName)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_invoice_no", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(invoiceNo)
//                    });

//                    cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text)
//                    {
//                        Value = Normalize(invType)
//                    });
//                },
//                reader =>
//                {
//                    return new InvWiseDTO
//                    {
//                        InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
//                        InvNo = reader["inv_no"]?.ToString(),
//                        InvType = reader["inv_type"]?.ToString(),
//                        SpUniqueCode = Convert.ToInt32(reader["sp_unique_code"]),
//                        CustomerName = reader["customer_name"]?.ToString(),

//                        Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),

//                        TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
//                        IGST = reader.IsDBNull(reader.GetOrdinal("igst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst")),
//                        CGST = reader.IsDBNull(reader.GetOrdinal("cgst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst")),
//                        SGST = reader.IsDBNull(reader.GetOrdinal("sgst")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst")),
//                        TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
//                        AvgPrice = reader.IsDBNull(reader.GetOrdinal("AvgPrice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("AvgPrice")),
//                        TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
//                        ExpDiscount = reader.IsDBNull(reader.GetOrdinal("exp_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("exp_discount")),
//                        RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
//                        FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
//                        AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 1 : reader.GetDecimal(reader.GetOrdinal("add_price")),
//                        AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
//                    };
//                },
//                CommandType.Text
//            );

//            Console.WriteLine($"ROWS FETCHED => {result?.Count}");
//            return result;
//        }


//    }
//}


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SoftBook.Infrastructure.Classes
{
    public class ReportRepository : BaseDAL, IReportRepository
    {
        public ReportRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        public async Task<LedgerReportDTO> GetLedgerReportAsync(int companyId, int accountCode, DateTime fromDate, DateTime toDate)
        {
            // Calling the V2 function which resolves the ambiguity issue
            var entries = await ExecuteReaderAsync("SELECT * FROM fn_ledger_report_v2(@p_company_id, @p_account_code, @p_from_date, @p_to_date)", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.AddWithValue("p_account_code", accountCode);

                // Explicitly send as DATE to match SQL function signature (INT, DATE, DATE)
                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
            }, reader =>
            {
                // Helper to safely get values that might be returned in different formats (TEXT, NUMERIC, etc)
                long? GetLong(string name)
                {
                    int ordinal = reader.GetOrdinal(name);
                    if (reader.IsDBNull(ordinal)) return null;
                    var val = reader.GetValue(ordinal);
                    if (val is long l) return l;
                    if (val is int i) return i;
                    if (val is decimal d) return (long)d;
                    if (long.TryParse(val.ToString(), out long parsed)) return parsed;
                    return null;
                }

                return new LedgerEntryDTO
                {
                    // Map result columns with "res_" prefix to avoid ambiguity in Postgres
                    JvDate = reader.GetDateTime(reader.GetOrdinal("res_jv_date")),
                    VType = reader.IsDBNull(reader.GetOrdinal("res_v_type")) ? null : reader.GetString(reader.GetOrdinal("res_v_type")),
                    VCode = GetLong("res_v_code"),
                    Description = reader.IsDBNull(reader.GetOrdinal("res_description")) ? null : reader.GetString(reader.GetOrdinal("res_description")),
                    Debit = reader.IsDBNull(reader.GetOrdinal("res_debit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("res_debit")),
                    Credit = reader.IsDBNull(reader.GetOrdinal("res_credit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("res_credit")),
                    Balance = reader.IsDBNull(reader.GetOrdinal("res_balance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("res_balance")),
                    Id = GetLong("res_id")
                };
            }, CommandType.Text);

            var report = new LedgerReportDTO
            {
                AccountCode = accountCode,
                Entries = entries
            };

            if (entries.Count > 0)
            {
                // The first row is guaranteed to be "Opening Balance" by our SQL function wrapper
                report.OpeningBalance = entries.First().Balance;

                // The last row has the final running balance
                report.ClosingBalance = entries.Last().Balance;
            }

            return report;
        }

        public async Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(
            int companyId,
            string? invType,
            string? where)
        {
            try
            {
                return await ExecuteReaderAsync(
                    "SELECT * FROM fn_sale_purchase_register_report(@p_company_id, @p_inv_type, @p_where_clause)",
                    cmd =>
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                        {
                            Value = companyId
                        });

                        string finalWhere = string.IsNullOrWhiteSpace(invType) ? (where ?? "1=1") : $"tsp.inv_type = '{invType}'" + (string.IsNullOrWhiteSpace(where) ? "" : $" AND ({where})");

                        cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = "ITEM" });
                        cmd.Parameters.Add(new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text) { Value = finalWhere });
                    },
                    reader =>
                    {
                        return new SalePurchaseRegisterItemwiseDTO
                        {
                            // InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
                            InvNo = reader.IsDBNull(reader.GetOrdinal("inv_no")) ? null : reader.GetString(reader.GetOrdinal("inv_no")),
                            InvType = reader.IsDBNull(reader.GetOrdinal("inv_type")) ? null : reader.GetString(reader.GetOrdinal("inv_type")),
                            PartyName = reader.IsDBNull(reader.GetOrdinal("party_name")) ? null : reader.GetString(reader.GetOrdinal("party_name")),
                            ProductName = reader.IsDBNull(reader.GetOrdinal("product_name")) ? null : reader.GetString(reader.GetOrdinal("product_name")),
                            Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),
                            cogs = reader.IsDBNull(reader.GetOrdinal("cogs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cogs")),
                            AvgPrice = reader.IsDBNull(reader.GetOrdinal("avgprice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avgprice")),
                            TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
                            IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
                            CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
                            SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
                            TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
                            TotDiscount = reader.IsDBNull(reader.GetOrdinal("tot_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tot_discount")),
                            TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
                            RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
                            FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
                            AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_price")),
                            AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
                        };
                    },
                    CommandType.Text
                );
            }
            catch (Exception ex)
            {
                // 🔥 Log full context (VERY IMPORTANT)
                Console.WriteLine("Error in GetSalePurchaseRegisterItemwiseAsync:");
                Console.WriteLine($"CompanyId: {companyId}");
                Console.WriteLine($"InvType: {invType}");
                Console.WriteLine($"Where: {where}");
                Console.WriteLine($"Error: {ex.Message}");

                throw; // rethrow so caller knows
            }
        }
        public async Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(
            int companyId,
            string? invType,
            string? where)
        {
            return await ExecuteReaderAsync(
                "SELECT * FROM fn_sale_purchase_register_report(@p_company_id, @p_inv_type, @p_where_clause)",
                cmd =>
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                    {
                        Value = companyId
                    });

                    string finalWhere = string.IsNullOrWhiteSpace(invType) ? (where ?? "1=1") : $"tsp.inv_type = '{invType}'" + (string.IsNullOrWhiteSpace(where) ? "" : $" AND ({where})");

                    cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = "DATE" });
                    cmd.Parameters.Add(new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text) { Value = finalWhere });
                },
                reader =>
                {
                    return new SalePurchaseRegisterDatewiseDTO
                    {
                        InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
                        InvType = reader.IsDBNull(reader.GetOrdinal("inv_type")) ? null : reader.GetString(reader.GetOrdinal("inv_type")),
                        Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),
                        AvgPrice = reader.IsDBNull(reader.GetOrdinal("avgprice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avgprice")),
                        TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
                        IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
                        CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
                        SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
                        TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
                        TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
                        RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
                        FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
                        AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_price")),
                        AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount")),
                        TotDiscount = reader.IsDBNull(reader.GetOrdinal("tot_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tot_discount"))
                    };
                },
                CommandType.Text
            );
        }
        public async Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(
            int companyId,
            string? invType,
            string? where)
        {
            return await ExecuteReaderAsync(
                "SELECT * FROM fn_sale_purchase_register_report(@p_company_id, @p_inv_type, @p_where_clause)",
                cmd =>
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                    {
                        Value = companyId
                    });

                    string finalWhere = string.IsNullOrWhiteSpace(invType) ? (where ?? "1=1") : $"tsp.inv_type = '{invType}'" + (string.IsNullOrWhiteSpace(where) ? "" : $" AND ({where})");

                    cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = "PARTY" });
                    cmd.Parameters.Add(new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text) { Value = finalWhere });
                },
                reader =>
                {
                    return new SalePurchaseRegisterPartywiseDTO
                    {
                        PartyName = reader.IsDBNull(reader.GetOrdinal("party_name")) ? null : reader.GetString(reader.GetOrdinal("party_name")),
                        InvType = reader.IsDBNull(reader.GetOrdinal("inv_type")) ? null : reader.GetString(reader.GetOrdinal("inv_type")),
                        Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),
                        AvgPrice = reader.IsDBNull(reader.GetOrdinal("avgprice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avgprice")),
                        TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
                        IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
                        CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
                        SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
                        TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
                        TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
                        TotDiscount = reader.IsDBNull(reader.GetOrdinal("tot_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tot_discount")),
                        RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
                        FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
                        AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_price")),
                        AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
                    };
                },
                CommandType.Text
            );
        }

        //public async Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(
        //    int companyId,
        //    string? invType,
        //    string? where)
        //{
        //    Console.WriteLine($"API PARAMS => companyId={companyId}, invType={invType}, where={where}");
        //    try
        //    {
        //        var result = await ExecuteReaderAsync(
        //            "SELECT * FROM fn_sale_purchase_register_report(@p_company_id, @p_inv_type, @p_where_clause)",
        //            cmd =>
        //            {
        //                cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
        //                {
        //                    Value = companyId
        //                });

        //                string finalWhere = string.IsNullOrWhiteSpace(invType) ? (where ?? "1=1") : $"tsp.inv_type = '{invType}'" + (string.IsNullOrWhiteSpace(where) ? "" : $" AND ({where})");

        //                cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = "INV" });
        //                cmd.Parameters.Add(new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text) { Value = finalWhere });
        //            },
        //            reader =>
        //            {
        //                return new InvWiseDTO
        //                {
        //                    InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
        //                    InvNo = reader["inv_no"]?.ToString(),
        //                    InvType = reader["inv_type"]?.ToString(),
        //                    SpUniqueCode = Convert.ToInt32(reader["sp_unique_code"]),
        //                    PartyName = reader.IsDBNull(reader.GetOrdinal("party_name")) ? null : reader.GetString(reader.GetOrdinal("party_name")),

        //                    Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),

        //                    TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
        //                    IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
        //                    CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
        //                    SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
        //                    TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
        //                    AvgPrice = reader.IsDBNull(reader.GetOrdinal("avgprice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avgprice")),
        //                    TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
        //                    TotDiscount = reader.IsDBNull(reader.GetOrdinal("tot_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tot_discount")),
        //                    RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
        //                    FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
        //                    AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 1 : reader.GetDecimal(reader.GetOrdinal("add_price")),
        //                    AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
        //                };
        //            };
        //            }
        //}

        public async Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(
      int companyId,
      string? invType,
      string? where)
        {
            Console.WriteLine($"API PARAMS => companyId={companyId}, invType={invType}, where={where}");
            try
            {
                var result = await ExecuteReaderAsync(
                    "SELECT * FROM fn_sale_purchase_register_report(@p_company_id, @p_inv_type, @p_where_clause)",
                    cmd =>
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                        {
                            Value = companyId
                        });

                        string finalWhere = string.IsNullOrWhiteSpace(invType) ? (where ?? "1=1") : $"tsp.inv_type = '{invType}'" + (string.IsNullOrWhiteSpace(where) ? "" : $" AND ({where})");

                        cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = "INV" });
                        cmd.Parameters.Add(new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text) { Value = finalWhere });
                    },
                    reader =>
                    {
                        return new InvWiseDTO
                        {
                            InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
                            InvNo = reader["inv_no"]?.ToString(),
                            InvType = reader["inv_type"]?.ToString(),
                            SpUniqueCode = Convert.ToInt32(reader["sp_unique_code"]),
                            PartyName = reader.IsDBNull(reader.GetOrdinal("party_name")) ? null : reader.GetString(reader.GetOrdinal("party_name")),

                            Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),

                            TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
                            IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
                            CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
                            SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
                            TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
                            AvgPrice = reader.IsDBNull(reader.GetOrdinal("avgprice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avgprice")),
                            TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
                            TotDiscount = reader.IsDBNull(reader.GetOrdinal("tot_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tot_discount")),
                            RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
                            FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
                            AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 1 : reader.GetDecimal(reader.GetOrdinal("add_price")),
                            AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
                        };
                    },
                    CommandType.Text
                );

                Console.WriteLine($"ROWS FETCHED => {result?.Count}");
                return result;
            }
            catch (Exception e)
            {
                // Log the exception and return an empty list so the method always returns a value
                Console.WriteLine($"ERROR in GetSalePurInvWiseAsync: {e}");
                return new List<InvWiseDTO>();
            }
        }




        public async Task<IEnumerable<SalePurchaseRegisterMonthwiseDTO>> GetSalePurchaseRegisterMonthwiseAsync(
           int companyId,
           string? invType,
           string? where)
        {
            try
            {
                return await ExecuteReaderAsync(
                    "SELECT * FROM fn_sale_purchase_register_report(@p_company_id, @p_inv_type, @p_where_clause)",
                    cmd =>
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                        {
                            Value = companyId
                        });

                        string finalWhere = string.IsNullOrWhiteSpace(invType) ? (where ?? "1=1") : $"tsp.inv_type = '{invType}'" + (string.IsNullOrWhiteSpace(where) ? "" : $" AND ({where})");

                        cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = "MONTH" });
                        cmd.Parameters.Add(new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text) { Value = finalWhere });
                    },
                    reader =>
                    {
                        return new SalePurchaseRegisterMonthwiseDTO
                        {
                            InvNo = reader["inv_no"]?.ToString(),
                            // InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
                            //PartyName = reader.IsDBNull(reader.GetOrdinal("party_name")) ? null : reader.GetString(reader.GetOrdinal("party_name")),
                            InvType = reader.IsDBNull(reader.GetOrdinal("inv_type")) ? null : reader.GetString(reader.GetOrdinal("inv_type")),
                            Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),
                            AvgPrice = reader.IsDBNull(reader.GetOrdinal("avgprice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avgprice")),
                            TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
                            IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
                            CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
                            SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
                            TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
                            TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
                            TotDiscount = reader.IsDBNull(reader.GetOrdinal("tot_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tot_discount")),
                            RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
                            FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
                            AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_price")),
                            AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
                        };
                    },
                    CommandType.Text
                );
            }
            catch (Exception ex)
            {
                // 🔥 Log full context (VERY IMPORTANT)
                Console.WriteLine("Error in GetSalePurchaseRegisterItemwiseAsync:");
                Console.WriteLine($"CompanyId: {companyId}");
                Console.WriteLine($"InvType: {invType}");
                Console.WriteLine($"Where: {where}");
                Console.WriteLine($"Error: {ex.Message}");

                throw; // rethrow so caller knows
            }
        }



        //Trial Balance Report Repository code writtine by Hetak Modi 23-04-2026
        //==============================================================================================================================================
       public async Task<List<TrialBalanceDTO>> GetTrialBalance(int companyId, DateTime date)
{
    return await ExecuteReaderAsync(
        "SELECT * FROM fn_trial_balance(@p_company_id, @p_date)",
        cmd =>
        {
            cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = companyId
            });

            cmd.Parameters.Add(new NpgsqlParameter("p_date", NpgsqlTypes.NpgsqlDbType.Date)
            {
                Value = date
            });
        },
        reader =>
        {
            return new TrialBalanceDTO
            {
                AccountName = reader.IsDBNull(reader.GetOrdinal("account_name"))? null: reader.GetString(reader.GetOrdinal("account_name")),

                Debit = reader.IsDBNull(reader.GetOrdinal("debit"))? 0: reader.GetDecimal(reader.GetOrdinal("debit")),

                Credit = reader.IsDBNull(reader.GetOrdinal("credit"))? 0: reader.GetDecimal(reader.GetOrdinal("credit"))
            };
        },
        CommandType.Text
    );
}

        public async Task<List<BalanceSheetReportDTO>> GetBalanceSheetReportAsync(int companyId, DateTime asOfDate)
        {
            const string sql = "SELECT * FROM public.fn_get_balance_sheet(@p_company_id, @p_as_of_date);";

            return await ExecuteReaderAsync(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                    {
                        Value = companyId
                    });
                    cmd.Parameters.Add(new NpgsqlParameter("p_as_of_date", NpgsqlTypes.NpgsqlDbType.Date)
                    {
                        Value = asOfDate
                    });
                },
                reader =>
                {
                    string? GetNullableString(string name)
                    {
                        var ordinal = reader.GetOrdinal(name);
                        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
                    }

                    long? GetNullableLong(string name)
                    {
                        var ordinal = reader.GetOrdinal(name);
                        if (reader.IsDBNull(ordinal)) return null;
                        var value = reader.GetValue(ordinal);
                        return value is long l ? l : Convert.ToInt64(value);
                    }

                    decimal GetDecimal(string name)
                    {
                        var ordinal = reader.GetOrdinal(name);
                        return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
                    }

                    return new BalanceSheetReportDTO
                    {
                        ReportSide = GetNullableString("report_side"),
                        RowType = GetNullableString("row_type"),
                        GroupName = GetNullableString("group_name"),
                        LedgerId = GetNullableLong("ledger_id"),
                        LedgerName = GetNullableString("ledger_name"),
                        Amount = GetDecimal("amount")
                    };
                },
                CommandType.Text
            );
        }

        public async Task<List<BalanceSheetReportDTO>> GetProfitLossReportAsync(int companyId, DateTime asOfDate)
        {
            const string sql = "SELECT * FROM public.fn_get_profit_loss(@p_company_id, @p_as_of_date);";

            return await ExecuteReaderAsync(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                    {
                        Value = companyId
                    });
                    cmd.Parameters.Add(new NpgsqlParameter("p_as_of_date", NpgsqlTypes.NpgsqlDbType.Date)
                    {
                        Value = asOfDate
                    });
                },
                reader =>
                {
                    string? GetNullableString(string name)
                    {
                        var ordinal = reader.GetOrdinal(name);
                        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
                    }

                    long? GetNullableLong(string name)
                    {
                        var ordinal = reader.GetOrdinal(name);
                        if (reader.IsDBNull(ordinal)) return null;
                        var value = reader.GetValue(ordinal);
                        return value is long l ? l : Convert.ToInt64(value);
                    }

                    decimal GetDecimal(string name)
                    {
                        var ordinal = reader.GetOrdinal(name);
                        return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
                    }

                    return new BalanceSheetReportDTO
                    {
                        ReportSide = GetNullableString("report_side"),
                        RowType = GetNullableString("row_type"),
                        GroupName = GetNullableString("group_name"),
                        LedgerId = GetNullableLong("ledger_id"),
                        LedgerName = GetNullableString("ledger_name"),
                        Amount = GetDecimal("amount")
                    };
                },
                CommandType.Text
            );
        }


        //by diya 4/24
        public async Task<IEnumerable<PayebleReceivableDTO>> GetPayebleReceivableAsync(
            int companyId,
            int? partyId,
            string asOfDate,
            string partyType)
                    {
                        try
                        {
                            return await ExecuteReaderAsync(
                                "SELECT * FROM fn_payeble_receivable_credit(@p_company_id,@p_party_id,@p_as_of_date,@p_party_type)",

                                cmd =>
                                {
                                    cmd.Parameters.Add(
                                        new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                                        {
                                            Value = companyId
                                        });

                                    cmd.Parameters.Add(
                                      new NpgsqlParameter("p_party_id", NpgsqlTypes.NpgsqlDbType.Integer)
                                      {
                                          Value = (object?)partyId ?? DBNull.Value
                                      });

                                    cmd.Parameters.Add(
                                        new NpgsqlParameter("p_as_of_date", NpgsqlTypes.NpgsqlDbType.Date)
                                        {
                                            Value = !string.IsNullOrWhiteSpace(asOfDate)
                                                        ? DateTime.Parse(asOfDate).Date
                                                        : DateTime.Today
                                        });
                                    cmd.Parameters.Add(
                                         new NpgsqlParameter("p_party_type", NpgsqlTypes.NpgsqlDbType.Text)
                                         {
                                             Value = !string.IsNullOrWhiteSpace(partyType)
                                                 ? partyType
                                                 : "Customer"
                                         });

                                },

                                reader =>
                                {
                                    return new PayebleReceivableDTO
                                    {
                                        PartyId = reader.IsDBNull(reader.GetOrdinal("party_id"))
                                            ? 0
                                            : reader.GetInt32(reader.GetOrdinal("party_id")),

                                        DisplayName = reader["display_name"]?.ToString(),

                                        PartyType = reader["party_type"]?.ToString(),

                                        Contact = reader["contact"]?.ToString(),

                                        OpeningBalanceType = reader["opening_balance_type"]?.ToString(),

                                        OpeningBalance = reader.IsDBNull(reader.GetOrdinal("opening_balance"))
                                            ? 0
                                            : reader.GetDecimal(reader.GetOrdinal("opening_balance")),

                                        InvAmt = reader.IsDBNull(reader.GetOrdinal("inv_amt"))
                                                        ? 0
                                                        : reader.GetDecimal(reader.GetOrdinal("inv_amt")),

                                        PaidAmt = reader.IsDBNull(reader.GetOrdinal("paid_amt"))
                                            ? 0
                                            : reader.GetDecimal(reader.GetOrdinal("paid_amt")),

                                        Outstanding = reader.IsDBNull(reader.GetOrdinal("outstanding"))
                                            ? 0
                                            : reader.GetDecimal(reader.GetOrdinal("outstanding")),

                                        CreditDebit = reader["credit_debit"]?.ToString(),

                                        Terms = reader.IsDBNull(reader.GetOrdinal("terms"))
                                            ? 0
                                            : reader.GetDecimal(reader.GetOrdinal("terms")),

                                        Weekly = reader.IsDBNull(reader.GetOrdinal("weekly"))
                                            ? 0
                                            : reader.GetDecimal(reader.GetOrdinal("weekly")),

                                        Monthly = reader.IsDBNull(reader.GetOrdinal("monthly"))
                                            ? 0
                                            : reader.GetDecimal(reader.GetOrdinal("monthly")),

                                        MonthlyTerm = reader.IsDBNull(reader.GetOrdinal("monthly_term"))
                                            ? 0
                                            : reader.GetDecimal(reader.GetOrdinal("monthly_term"))
                                    };
                                },

                                CommandType.Text
                            );
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in GetPayebleReceivableAsync:");
                            Console.WriteLine($"CompanyId: {companyId}");
                            Console.WriteLine($"PartyId: {partyId}");
                            Console.WriteLine($"AsOfDate: {asOfDate}");
                            Console.WriteLine($"Error: {ex.Message}");

                            throw;
                        }
                    }


        public async Task<IEnumerable<SalePurchaseRegisterDatewiseComparisonDTO>> GetSalePurchaseRegisterDatewiseComparisonAsync(
             int companyId,
             string? where,
             string compareMode)
                {
                    try
                    {
                        return await ExecuteReaderAsync(
                            "SELECT * FROM fn_sale_purchase_comparative_datewise(@p_company_id,@p_where_clause,@p_compare_mode)",
                            cmd =>
                            {
                                cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                                {
                                    Value = companyId
                                });

                                cmd.Parameters.Add(
                                    new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text)
                                    {
                                        Value = where ?? ""
                                    });

                                cmd.Parameters.Add(
                                    new NpgsqlParameter("p_compare_mode", NpgsqlTypes.NpgsqlDbType.Text)
                                    {
                                        Value = compareMode ?? ""
                                    });
                            },
                            reader =>
                            {
                                return new SalePurchaseRegisterDatewiseComparisonDTO
                                {
                                    PInvDate = reader.IsDBNull(reader.GetOrdinal("p_inv_date"))
                                                ? (DateTime?)null
                                                : reader.GetDateTime(reader.GetOrdinal("p_inv_date")),
                                    PurQty = reader.IsDBNull(reader.GetOrdinal("pur_qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("pur_qty")),
                                    PurAvgPrice = reader.IsDBNull(reader.GetOrdinal("pur_avg_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("pur_avg_price")),
                                    PurAmount = reader.IsDBNull(reader.GetOrdinal("pur_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("pur_amount")),
                                    PurTax = reader.IsDBNull(reader.GetOrdinal("pur_tax")) ? 0 : reader.GetDecimal(reader.GetOrdinal("pur_tax")),
                                    PurExpDisRound = reader.IsDBNull(reader.GetOrdinal("pur_exp_dis_round")) ? 0 : reader.GetDecimal(reader.GetOrdinal("pur_exp_dis_round")),
                                    PurFinal = reader.IsDBNull(reader.GetOrdinal("pur_final")) ? 0 : reader.GetDecimal(reader.GetOrdinal("pur_final")),

                                    SInvDate = reader.IsDBNull(reader.GetOrdinal("s_inv_date"))
                                                ? (DateTime?)null
                                                : reader.GetDateTime(reader.GetOrdinal("s_inv_date")),
                                    SaleQty = reader.IsDBNull(reader.GetOrdinal("sale_qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sale_qty")),
                                    SaleAvgPrice = reader.IsDBNull(reader.GetOrdinal("sale_avg_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sale_avg_price")),
                                    SaleAmount = reader.IsDBNull(reader.GetOrdinal("sale_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sale_amount")),
                                    SaleTax = reader.IsDBNull(reader.GetOrdinal("sale_tax")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sale_tax")),
                                    SaleExpDisRound = reader.IsDBNull(reader.GetOrdinal("sale_exp_dis_round")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sale_exp_dis_round")),
                                    SaleFinal = reader.IsDBNull(reader.GetOrdinal("sale_final")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sale_final")),

                                    PL = reader.IsDBNull(reader.GetOrdinal("pl")) ? 0 : reader.GetDecimal(reader.GetOrdinal("pl")),


                                };
                            },
                            CommandType.Text
                        );
                    }
                    catch (Exception ex)
                    {
                        // 🔥 Log full context (VERY IMPORTANT)
                        Console.WriteLine("Error in GetSalePurchaseRegisterItemwiseAsync:");
                        Console.WriteLine($"CompanyId: {companyId}");

                        Console.WriteLine($"Where: {where}");
                        Console.WriteLine($"Error: {ex.Message}");

                        throw; // rethrow so caller knows
                    }
                }


        //by Diya
        //gst rpt
        public async Task<IEnumerable<GetGSTR3bDTO>> GetGSTR3bAsync(
          int companyId,
          string where,
          string reportPart)
            {
                try
                {
                    return await ExecuteReaderAsync(
                        "SELECT * FROM fn_gst_report(@p_company_id,@p_where_clause,@p_report_part)",

                        cmd =>
                        {
                            cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                            {
                                Value = companyId
                            });

                            cmd.Parameters.Add(new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text)
                            {
                                Value = where ?? ""
                            });

                            cmd.Parameters.Add(new NpgsqlParameter("p_report_part", NpgsqlTypes.NpgsqlDbType.Text)
                            {
                                Value = reportPart ?? "3.1"
                            });
                        },

                        reader =>
                        {
                            return new GetGSTR3bDTO
                            {
                                Section =
                                    reader.IsDBNull(reader.GetOrdinal("section"))
                                    ? ""
                                    : reader.GetString(
                                        reader.GetOrdinal("section")),

                                TaxableAmt =
                                    reader.IsDBNull(reader.GetOrdinal("taxable_amt"))
                                    ? 0
                                    : reader.GetDecimal(
                                        reader.GetOrdinal("taxable_amt")),

                                IgstAmt =
                                    reader.IsDBNull(reader.GetOrdinal("igst_amt"))
                                    ? 0
                                    : reader.GetDecimal(
                                        reader.GetOrdinal("igst_amt")),

                                CgstAmt =
                                    reader.IsDBNull(reader.GetOrdinal("cgst_amt"))
                                    ? 0
                                    : reader.GetDecimal(
                                        reader.GetOrdinal("cgst_amt")),

                                SgstAmt =
                                    reader.IsDBNull(reader.GetOrdinal("sgst_amt"))
                                    ? 0
                                    : reader.GetDecimal(
                                        reader.GetOrdinal("sgst_amt")),

                                Cess =
                                    reader.IsDBNull(reader.GetOrdinal("cess"))
                                    ? 0
                                    : reader.GetDecimal(
                                        reader.GetOrdinal("cess")),

                                InterStateSupplies =
                                    reader.IsDBNull(reader.GetOrdinal("inter_state_supplies"))
                                    ? 0
                                    : reader.GetDecimal(
                                        reader.GetOrdinal("inter_state_supplies")),

                                IntraStateSupplies =
                                    reader.IsDBNull(reader.GetOrdinal("intra_state_supplies"))
                                    ? 0
                                    : reader.GetDecimal(
                                        reader.GetOrdinal("intra_state_supplies"))
                            };
                        },

                        CommandType.Text
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetGSTR3bAsync:");
                    Console.WriteLine($"CompanyId: {companyId}");
                    Console.WriteLine($"Where: {where}");
                    Console.WriteLine($"ReportPart: {reportPart}");
                    Console.WriteLine($"Error: {ex.Message}");

                    throw;
                }
            }


        public async Task<List<GstrExcelRow>> GetGstr1ExcelExportAsync(
       int companyId,
       DateOnly fromDate,
       DateOnly toDate)
        {
            try
            {
                return (await ExecuteReaderAsync(
                                      @"SELECT * FROM public.fn_gstr1_excel_export(
 @CompanyId,
 @FromDate::date,
 @ToDate::date
)",

                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("@CompanyId", companyId);
                        cmd.Parameters.AddWithValue(  "@FromDate", fromDate.ToDateTime(TimeOnly.MinValue) );

                        cmd.Parameters.AddWithValue( "@ToDate",toDate.ToDateTime(TimeOnly.MinValue));
                    },

                    reader => new GstrExcelRow
                    {
                        Sheet_Name = reader["sheet_name"]?.ToString(),

                        Col1 = reader["col1"]?.ToString(),
                        Col2 = reader["col2"]?.ToString(),
                        Col3 = reader["col3"]?.ToString(),
                        Col4 = reader["col4"]?.ToString(),
                        Col5 = reader["col5"]?.ToString(),
                        Col6 = reader["col6"]?.ToString(),
                        Col7 = reader["col7"]?.ToString(),
                        Col8 = reader["col8"]?.ToString(),
                        Col9 = reader["col9"]?.ToString(),
                        Col10 = reader["col10"]?.ToString(),
                        Col11 = reader["col11"]?.ToString(),
                        Col12 = reader["col12"]?.ToString(),
                        Col13 = reader["col13"]?.ToString(),
                        Col14 = reader["col14"]?.ToString(),
                        Col15 = reader["col15"]?.ToString(),
                        Col16 = reader["col16"]?.ToString(),
                        Col17 = reader["col17"]?.ToString(),
                        Col18 = reader["col18"]?.ToString(),
                        Col19 = reader["col19"]?.ToString(),
                        Col20 = reader["col20"]?.ToString(),
                        Col21 = reader["col21"]?.ToString(),
                        Col22 = reader["col22"]?.ToString(),
                        Col23 = reader["col23"]?.ToString(),
                        Col24 = reader["col24"]?.ToString(),
                        Col25 = reader["col25"]?.ToString(),
                        Col26 = reader["col26"]?.ToString(),
                        Col27 = reader["col27"]?.ToString(),
                        Col28 = reader["col28"]?.ToString()
                    },

                    CommandType.Text

                )).ToList();
            }
            catch
            {
                throw;
            }
        }

        // Ledger Summary Datewise - Hetak Modi 02-05-2026
        public async Task<IEnumerable<LedgerSummaryDatewiseDTO>> GetLedgerSummaryDatewiseAsync(
            int companyId,
            int accountCode,
            DateTime fromDate,
            DateTime toDate)
        {
            try
            {
                return await ExecuteReaderAsync(
                    "SELECT * FROM fn_ledger_summary_datewise(@p_company_id, @p_account_code, @p_from_date, @p_to_date)",
                    cmd =>
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = companyId });
                        cmd.Parameters.Add(new NpgsqlParameter("p_account_code", NpgsqlTypes.NpgsqlDbType.Integer) { Value = accountCode });
                        cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
                        cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
                    },
                    reader =>
                    {
                        int dateOrd = reader.GetOrdinal("res_date");
                        int particularOrd = reader.GetOrdinal("res_particular");
                        int debitOrd = reader.GetOrdinal("res_debit");
                        int creditOrd = reader.GetOrdinal("res_credit");
                        int balanceOrd = reader.GetOrdinal("res_balance");

                        return new LedgerSummaryDatewiseDTO
                        {
                            ResDate = reader.IsDBNull(dateOrd) ? (DateTime?)null : reader.GetDateTime(dateOrd),
                            ResParticular = reader.IsDBNull(particularOrd) ? null : reader.GetString(particularOrd),
                            ResDebit = reader.IsDBNull(debitOrd) ? 0 : reader.GetDecimal(debitOrd),
                            ResCredit = reader.IsDBNull(creditOrd) ? 0 : reader.GetDecimal(creditOrd),
                            ResBalance = reader.IsDBNull(balanceOrd) ? (decimal?)null : reader.GetDecimal(balanceOrd),
                        };
                    },
                    CommandType.Text
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetLedgerSummaryDatewiseAsync:");
                Console.WriteLine($"AccountCode: {accountCode}, From: {fromDate:yyyy-MM-dd}, To: {toDate:yyyy-MM-dd}");
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        //outstanding-comparison - Diya 04-05-2026
        public async Task<IEnumerable<OutstandingComparisonDTO>> GetOutstandingComparisonAsync(
             int companyId,
             DateOnly date,
             string? where,
             string mode)
        {
            try
            {
                var safeWhere = string.IsNullOrWhiteSpace(where) ? "" : where.Trim();

                return await ExecuteReaderAsync(
                    "SELECT * FROM fn_outstanding_comparision(@p_comp_id, @p_date, @p_where,@P_mode)",
                    cmd =>
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("p_comp_id", NpgsqlTypes.NpgsqlDbType.Integer)
                        {
                            Value = companyId
                        });

                        cmd.Parameters.Add(new NpgsqlParameter("p_date", NpgsqlTypes.NpgsqlDbType.Date)
                        {
                            Value = date.ToDateTime(TimeOnly.MinValue)
                        });

                        cmd.Parameters.Add(new NpgsqlParameter("p_where", NpgsqlTypes.NpgsqlDbType.Text)
                        {
                            Value = safeWhere

                        });
                        cmd.Parameters.Add(new NpgsqlParameter("p_mode", NpgsqlTypes.NpgsqlDbType.Text)
                        {
                            Value = mode

                        });
                    },
                    reader =>
                    {
                        return new OutstandingComparisonDTO
                        {
                            //PartyId = reader.GetInt32(reader.GetOrdinal("party_id")),
                            //CustomerName = reader.GetString(reader.GetOrdinal("customer_name")),
                            PartyId = reader.IsDBNull(reader.GetOrdinal("party_id"))
                                    ? 0   // or null if nullable
                                    : reader.GetInt32(reader.GetOrdinal("party_id")),

                            CustomerName = reader.IsDBNull(reader.GetOrdinal("customer_name"))
                                    ? ""
                                    : reader.GetString(reader.GetOrdinal("customer_name")),

                            YQty = reader.IsDBNull(reader.GetOrdinal("y_qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("y_qty")),
                            YAmount = reader.IsDBNull(reader.GetOrdinal("y_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("y_amount")),
                            YOs = reader.IsDBNull(reader.GetOrdinal("y_os")) ? 0 : reader.GetDecimal(reader.GetOrdinal("y_os")),

                            TQty = reader.IsDBNull(reader.GetOrdinal("t_qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("t_qty")),
                            TAmount = reader.IsDBNull(reader.GetOrdinal("t_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("t_amount")),
                            Receipt = reader.IsDBNull(reader.GetOrdinal("receipt")) ? 0 : reader.GetDecimal(reader.GetOrdinal("receipt")),

                            TOs = reader.IsDBNull(reader.GetOrdinal("t_os")) ? 0 : reader.GetDecimal(reader.GetOrdinal("t_os")),
                            Diff = reader.IsDBNull(reader.GetOrdinal("diff")) ? 0 : reader.GetDecimal(reader.GetOrdinal("diff"))
                        };
                    },
                    CommandType.Text
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetOutstandingComparisonAsync:");
                Console.WriteLine($"CompanyId: {companyId}");
                Console.WriteLine($"Date: {date}");
                Console.WriteLine($"Where: {where}");
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        // Created by Hetak on 08052026
        public async Task<List<StockSummaryReportDTO>> GetStockSummaryReportAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            return await ExecuteReaderAsync("public.fn_stock_summary_report", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
            }, reader => new StockSummaryReportDTO
            {
                ProductId = SafeGetInt(reader, "product_id"),
                ProductName = SafeGetString(reader, "product_name"),
                UnitName = SafeGetString(reader, "unit_name"),
                OpeningQty = reader.IsDBNull(reader.GetOrdinal("opening_qty")) ? 0 : Convert.ToDecimal(reader["opening_qty"]),
                InwardQty = reader.IsDBNull(reader.GetOrdinal("inward_qty")) ? 0 : Convert.ToDecimal(reader["inward_qty"]),
                CostPrice = reader.IsDBNull(reader.GetOrdinal("cost_price")) ? 0 : Convert.ToDecimal(reader["cost_price"]),
                InwardAmount = reader.IsDBNull(reader.GetOrdinal("inward_amount")) ? 0 : Convert.ToDecimal(reader["inward_amount"]),
                OutwardQty = reader.IsDBNull(reader.GetOrdinal("outward_qty")) ? 0 : Convert.ToDecimal(reader["outward_qty"]),
                SellingPrice = reader.IsDBNull(reader.GetOrdinal("selling_price")) ? 0 : Convert.ToDecimal(reader["selling_price"]),
                OutwardAmount = reader.IsDBNull(reader.GetOrdinal("outward_amount")) ? 0 : Convert.ToDecimal(reader["outward_amount"]),
                ClosingQty = reader.IsDBNull(reader.GetOrdinal("closing_qty")) ? 0 : Convert.ToDecimal(reader["closing_qty"]),
                ClosingAmount = reader.IsDBNull(reader.GetOrdinal("closing_amount")) ? 0 : Convert.ToDecimal(reader["closing_amount"])
            });
        }

        public async Task<IEnumerable<SalePurchaseRegisterTransactionwiseDTO>> GetSalePurchaseRegisterTransactionWise(
         int companyId,
         string? invType,
         string? where)
        {
            return await ExecuteReaderAsync(
                "SELECT * FROM fn_sale_purchase_register_report(@p_company_id, @p_inv_type, @p_where_clause)",
                cmd =>
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                    {
                        Value = companyId
                    });

                    string finalWhere = string.IsNullOrWhiteSpace(invType) ? (where ?? "1=1") : $"tsp.inv_type = '{invType}'" + (string.IsNullOrWhiteSpace(where) ? "" : $" AND ({where})");

                    cmd.Parameters.Add(new NpgsqlParameter("p_inv_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = "TRANSACTION" });
                    cmd.Parameters.Add(new NpgsqlParameter("p_where_clause", NpgsqlTypes.NpgsqlDbType.Text) { Value = finalWhere });
                },
                reader =>
                {
                    return new SalePurchaseRegisterTransactionwiseDTO
                    {
                        InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
                        InvNo = reader["inv_no"]?.ToString(),
                        ProductName = reader.IsDBNull(reader.GetOrdinal("product_name")) ? null : reader.GetString(reader.GetOrdinal("product_name")),
                        PartyName = reader.IsDBNull(reader.GetOrdinal("party_name")) ? null : reader.GetString(reader.GetOrdinal("party_name")),
                        InvType = reader.IsDBNull(reader.GetOrdinal("inv_type")) ? null : reader.GetString(reader.GetOrdinal("inv_type")),
                        Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("qty")),
                        cogs = reader.IsDBNull(reader.GetOrdinal("cogs")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cogs")),
                        AvgPrice = reader.IsDBNull(reader.GetOrdinal("avgprice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avgprice")),
                        TaxableAmount = reader.IsDBNull(reader.GetOrdinal("taxable_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("taxable_amount")),
                        IgstAmount = reader.IsDBNull(reader.GetOrdinal("igst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("igst_amount")),
                        CgstAmount = reader.IsDBNull(reader.GetOrdinal("cgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("cgst_amount")),
                        SgstAmount = reader.IsDBNull(reader.GetOrdinal("sgst_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("sgst_amount")),
                        TaxAmount = reader.IsDBNull(reader.GetOrdinal("tax_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tax_amount")),
                        TotalAmount = reader.IsDBNull(reader.GetOrdinal("total_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("total_amount")),
                        TotDiscount = reader.IsDBNull(reader.GetOrdinal("tot_discount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("tot_discount")),
                        RoundOff = reader.IsDBNull(reader.GetOrdinal("round_off")) ? 0 : reader.GetDecimal(reader.GetOrdinal("round_off")),
                        FinalAmount = reader.IsDBNull(reader.GetOrdinal("final_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("final_amount")),
                        AddPrice = reader.IsDBNull(reader.GetOrdinal("add_price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_price")),
                        AddAmount = reader.IsDBNull(reader.GetOrdinal("add_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("add_amount"))
                    };
                },
                CommandType.Text
            );
        }

        // Developed by Hetak on 11-05-2026
        public async Task<IEnumerable<StockLedgerDatewiseDTO>> GetStockLedgerDatewiseAsync(int companyId, DateTime fromDate, DateTime toDate, int? productId = null)
        {
            try
            {
                return await ExecuteReaderAsync(
                    "SELECT * FROM fn_stock_ledger_datewise_report(@p_company_id, @p_from_date, @p_to_date, @p_product_id)",
                    cmd =>
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = companyId });
                        cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
                        cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
                        cmd.Parameters.Add(new NpgsqlParameter("p_product_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object?)productId ?? DBNull.Value });
                    },
                    reader =>
                    {
                        return new StockLedgerDatewiseDTO
                        {
                            SrNo        = reader.IsDBNull(reader.GetOrdinal("res_sr_no"))        ? 0    : reader.GetInt64(reader.GetOrdinal("res_sr_no")),
                            TranDate    = reader.IsDBNull(reader.GetOrdinal("res_tran_date"))    ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("res_tran_date")),
                            Description = reader.IsDBNull(reader.GetOrdinal("res_description")) ? null : reader.GetString(reader.GetOrdinal("res_description")),
                            InwardQty   = reader.IsDBNull(reader.GetOrdinal("res_inward_qty"))  ? 0    : reader.GetDecimal(reader.GetOrdinal("res_inward_qty")),
                            InwardAmount = reader.IsDBNull(reader.GetOrdinal("res_inward_amount"))? 0  : reader.GetDecimal(reader.GetOrdinal("res_inward_amount")),
                            OutwardQty  = reader.IsDBNull(reader.GetOrdinal("res_outward_qty")) ? 0    : reader.GetDecimal(reader.GetOrdinal("res_outward_qty")),
                            OutwardAmount = reader.IsDBNull(reader.GetOrdinal("res_outward_amount"))? 0 : reader.GetDecimal(reader.GetOrdinal("res_outward_amount")),
                            BalanceQty  = reader.IsDBNull(reader.GetOrdinal("res_balance_qty")) ? 0    : reader.GetDecimal(reader.GetOrdinal("res_balance_qty")),
                            BalanceAmount = reader.IsDBNull(reader.GetOrdinal("res_balance_amount"))? 0 : reader.GetDecimal(reader.GetOrdinal("res_balance_amount"))
                        };
                    },
                    CommandType.Text
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetStockLedgerDatewiseAsync:");
                Console.WriteLine($"CompanyId: {companyId}, From: {fromDate:yyyy-MM-dd}, To: {toDate:yyyy-MM-dd}");
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        public async Task<IEnumerable<DailyOutstandingDto>> GetDailyOutstandingReportAsync(
    int companyId,
    string fromDate,
    string toDate,
    int? partyId)
        {
            try
            {
                return await ExecuteReaderAsync(
                    "SELECT * FROM fn_daily_outstanding_report(@p_company_id,@p_from_date,@p_to_date,@p_party_id)",

                    cmd =>
                    {
                        cmd.Parameters.Add(
                            new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer)
                            {
                                Value = companyId
                            });


                        cmd.Parameters.Add(
                            new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date)
                            {
                                Value = !string.IsNullOrWhiteSpace(fromDate)
                                            ? DateTime.Parse(fromDate).Date
                                            : DateTime.Today.Date
                            });

                        cmd.Parameters.Add(
                            new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date)
                            {
                                Value = !string.IsNullOrWhiteSpace(toDate)
                                            ? DateTime.Parse(toDate).Date
                                            : DateTime.Today.Date
                            });
                        cmd.Parameters.Add(
                            new NpgsqlParameter("p_party_id", NpgsqlTypes.NpgsqlDbType.Integer)
                            {
                                Value = (object?)partyId ?? DBNull.Value
                            });
                    },

                    reader =>
                    {
                        return new DailyOutstandingDto
                        {
                            PartyId = reader.IsDBNull(reader.GetOrdinal("party_id"))
                                ? 0
                                : Convert.ToInt32(reader["party_id"]),

                            PartyName = reader["display_name"]?.ToString(),

                            ReportDate = reader.IsDBNull(reader.GetOrdinal("report_date"))
                                ? DateTime.MinValue
                                : Convert.ToDateTime(reader["report_date"]),

                            Outstanding = reader.IsDBNull(reader.GetOrdinal("outstanding"))
                                ? 0
                                : Convert.ToDecimal(reader["outstanding"])
                        };
                    },

                    CommandType.Text
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetDailyOutstandingReportAsync:");
                Console.WriteLine($"CompanyId: {companyId}");
                Console.WriteLine($"PartyId: {partyId}");
                Console.WriteLine($"FromDate: {fromDate}");
                Console.WriteLine($"ToDate: {toDate}");
                Console.WriteLine($"Error: {ex.Message}");

                throw;
            }
        }

        public async Task<IEnumerable<CalendarSummaryDto>> GetCalendarSummaryReportAsync(
            long companyId,
            DateTime fromDate,
            DateTime toDate,
            int? partyId)
        {
            try
            {
                var rows = await ExecuteReaderAsync(
                    @"SELECT *
                      FROM fn_calendar_summary_report(@p_company_id, @p_from_date, @p_to_date, @p_party_id)
                      ORDER BY report_date",
                    cmd =>
                    {
                        cmd.Parameters.Add(
                            new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Bigint)
                            {
                                Value = companyId
                            });

                        cmd.Parameters.Add(
                            new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date)
                            {
                                Value = fromDate.Date
                            });

                        cmd.Parameters.Add(
                            new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date)
                            {
                                Value = toDate.Date
                            });
                        cmd.Parameters.Add(
                            new NpgsqlParameter("p_party_id", NpgsqlTypes.NpgsqlDbType.Integer)
                            {
                                Value = (object?)partyId ?? DBNull.Value
                            });
                    },
                    reader => new CalendarSummaryDto
                    {
                        ReportDate = SafeGetDateTime(reader, "report_date") ?? DateTime.MinValue,
                        TotalSale = SafeGetDecimal(reader, "total_sale"),
                        TotalReceipt = SafeGetDecimal(reader, "total_receipt"),
                        TotalOs = SafeGetDecimal(reader, "total_os"),
                        TotalAdj = SafeGetDecimal(reader, "total_adjustment"),
                        AvgTerms = SafeGetDecimal(reader, "avg_terms"),

                        BgColor = SafeGetString(reader, "bg_color"),
                    },
                    CommandType.Text);

                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetCalendarSummaryReportAsync:");
                Console.WriteLine($"CompanyId: {companyId}");
                Console.WriteLine($"FromDate: {fromDate:yyyy-MM-dd}");
                Console.WriteLine($"ToDate: {toDate:yyyy-MM-dd}");
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private static decimal SafeGetDecimal(NpgsqlDataReader reader, string columnName, decimal defaultValue = 0m)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal))
                {
                    return defaultValue;
                }

                return Convert.ToDecimal(reader.GetValue(ordinal));
            }
            catch (IndexOutOfRangeException)
            {
                return defaultValue;
            }
        }

        // Code developed by Hetak Modi 290526
        public async Task<List<DayBookReportDTO>> GetDayBookReportAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            var entries = await ExecuteReaderAsync("SELECT * FROM fn_get_day_book(@p_company_id, @p_from_date, @p_to_date)", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
            }, reader =>
            {
                return new DayBookReportDTO
                {
                    JvDate = reader.GetDateTime(reader.GetOrdinal("jv_date")),
                    JvCode = reader.GetInt64(reader.GetOrdinal("jv_code")),
                    VType = reader.IsDBNull(reader.GetOrdinal("v_type")) ? null : reader.GetString(reader.GetOrdinal("v_type")),
                    DrAccountName = reader.IsDBNull(reader.GetOrdinal("dr_account_name")) ? null : reader.GetString(reader.GetOrdinal("dr_account_name")),
                    CrAccountName = reader.IsDBNull(reader.GetOrdinal("cr_account_name")) ? null : reader.GetString(reader.GetOrdinal("cr_account_name")),
                    Amount = reader.IsDBNull(reader.GetOrdinal("amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"))
                };
            }, CommandType.Text);

            return entries.ToList();
        }

        // Code developed by Hetak Modi 290526
        public async Task<List<CashBankBookReportDTO>> GetCashBookReportAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            var entries = await ExecuteReaderAsync("SELECT * FROM fn_get_cash_book(@p_company_id, @p_from_date, @p_to_date)", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
            }, reader =>
            {
                return new CashBankBookReportDTO
                {
                    TransactionDate = reader.GetDateTime(reader.GetOrdinal("transaction_date")),
                    VoucherNo = reader.IsDBNull(reader.GetOrdinal("voucher_no")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("voucher_no")),
                    VoucherType = reader.IsDBNull(reader.GetOrdinal("voucher_type")) ? null : reader.GetString(reader.GetOrdinal("voucher_type")),
                    Particulars = reader.IsDBNull(reader.GetOrdinal("particulars")) ? null : reader.GetString(reader.GetOrdinal("particulars")),
                    ReceiptAmount = reader.IsDBNull(reader.GetOrdinal("receipt_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("receipt_amount")),
                    PaymentAmount = reader.IsDBNull(reader.GetOrdinal("payment_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("payment_amount")),
                    Balance = reader.IsDBNull(reader.GetOrdinal("balance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("balance"))
                };
            }, CommandType.Text);

            return entries.ToList();
        }

        // Code developed by Hetak Modi 290526
        public async Task<List<CashBankBookReportDTO>> GetBankBookReportAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            var entries = await ExecuteReaderAsync("SELECT * FROM fn_get_bank_book(@p_company_id, @p_from_date, @p_to_date)", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
            }, reader =>
            {
                return new CashBankBookReportDTO
                {
                    TransactionDate = reader.GetDateTime(reader.GetOrdinal("transaction_date")),
                    VoucherNo = reader.IsDBNull(reader.GetOrdinal("voucher_no")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("voucher_no")),
                    VoucherType = reader.IsDBNull(reader.GetOrdinal("voucher_type")) ? null : reader.GetString(reader.GetOrdinal("voucher_type")),
                    Particulars = reader.IsDBNull(reader.GetOrdinal("particulars")) ? null : reader.GetString(reader.GetOrdinal("particulars")),
                    ReceiptAmount = reader.IsDBNull(reader.GetOrdinal("receipt_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("receipt_amount")),
                    PaymentAmount = reader.IsDBNull(reader.GetOrdinal("payment_amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("payment_amount")),
                    Balance = reader.IsDBNull(reader.GetOrdinal("balance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("balance"))
                };
            }, CommandType.Text);

            return entries.ToList();
        }

        public async Task<List<BelowPriceSalesReportDTO>> GetBelowPriceSalesReportAsync(
            int companyId,
            DateTime fromDate,
            DateTime toDate,
            string? filterType)
        {
            const string sql = @"
                SELECT
                    h.sp_unique_code,
                    h.inv_date,
                    h.inv_no,
                    COALESCE(pm.display_name, '') AS party_name,
                    COALESCE(NULLIF(TRIM(p.product_name), ''), NULLIF(TRIM(t.product_details), ''), 'Unknown Item') AS product_name,
                    COALESCE(t.quantity, 0) AS qty,
                    COALESCE(t.rate, 0) AS invoice_rate,
                    COALESCE(p.cost_price, 0) AS cost_price,
                    COALESCE(p.selling_price, 0) AS selling_price,
                    (COALESCE(p.cost_price, 0) - COALESCE(t.rate, 0)) AS loss_vs_cost,
                    (COALESCE(p.selling_price, 0) - COALESCE(t.rate, 0)) AS diff_vs_selling_price,
                    (COALESCE(p.cost_price, 0) > 0 AND COALESCE(t.rate, 0) < COALESCE(p.cost_price, 0)) AS is_below_cost,
                    (COALESCE(p.selling_price, 0) > 0 AND COALESCE(t.rate, 0) < COALESCE(p.selling_price, 0)) AS is_below_selling_price,
                    CASE
                        WHEN COALESCE(p.cost_price, 0) > 0 AND COALESCE(t.rate, 0) < COALESCE(p.cost_price, 0)
                         AND COALESCE(p.selling_price, 0) > 0 AND COALESCE(t.rate, 0) < COALESCE(p.selling_price, 0)
                            THEN 'Below Cost & Sale Price'
                        WHEN COALESCE(p.cost_price, 0) > 0 AND COALESCE(t.rate, 0) < COALESCE(p.cost_price, 0)
                            THEN 'Below Cost Price'
                        WHEN COALESCE(p.selling_price, 0) > 0 AND COALESCE(t.rate, 0) < COALESCE(p.selling_price, 0)
                            THEN 'Below Sale Price'
                        ELSE 'Price Alert'
                    END AS alert_type
                FROM public.tab_sal_pur_tran t
                INNER JOIN public.tab_sale_pur h
                    ON t.sp_unique_code = h.sp_unique_code
                LEFT JOIN public.tab_product_master p
                    ON t.product_id = p.product_id
                   AND p.company_id = h.company_id
                LEFT JOIN public.tab_party_master pm
                    ON h.party_id = pm.party_id
                   AND pm.company_id = h.company_id
                WHERE h.company_id = @p_company_id
                  AND COALESCE(h.delete_flag, FALSE) = FALSE
                  AND COALESCE(t.delete_flag, FALSE) = FALSE
                  AND LOWER(TRIM(h.inv_type)) = 'sales'
                  AND h.inv_date::date BETWEEN @p_from_date AND @p_to_date
                  AND (@p_fiscal_year_id IS NULL OR h.fiscal_year_id = @p_fiscal_year_id)
                  AND (
                        (@p_filter_type = 'below_cost'
                            AND COALESCE(p.cost_price, 0) > 0
                            AND COALESCE(t.rate, 0) < COALESCE(p.cost_price, 0))
                     OR (@p_filter_type = 'below_sale_price'
                            AND COALESCE(p.selling_price, 0) > 0
                            AND COALESCE(t.rate, 0) < COALESCE(p.selling_price, 0))
                     OR (@p_filter_type NOT IN ('below_cost', 'below_sale_price')
                            AND (
                                (COALESCE(p.cost_price, 0) > 0 AND COALESCE(t.rate, 0) < COALESCE(p.cost_price, 0))
                                OR (COALESCE(p.selling_price, 0) > 0 AND COALESCE(t.rate, 0) < COALESCE(p.selling_price, 0))
                            ))
                  )
                ORDER BY h.inv_date DESC, h.inv_no, product_name";

            var rows = await ExecuteReaderAsync(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate });
                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate });
                cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
                });
                cmd.Parameters.Add(new NpgsqlParameter("p_filter_type", NpgsqlTypes.NpgsqlDbType.Text)
                {
                    Value = string.IsNullOrWhiteSpace(filterType) ? "all" : filterType.Trim().ToLower()
                });
            }, reader => new BelowPriceSalesReportDTO
            {
                SpUniqueCode = SafeGetInt(reader, "sp_unique_code"),
                InvDate = reader.GetDateTime(reader.GetOrdinal("inv_date")),
                InvNo = SafeGetString(reader, "inv_no"),
                PartyName = SafeGetString(reader, "party_name"),
                ProductName = SafeGetString(reader, "product_name"),
                Qty = reader.IsDBNull(reader.GetOrdinal("qty")) ? 0 : Convert.ToDecimal(reader["qty"]),
                InvoiceRate = reader.IsDBNull(reader.GetOrdinal("invoice_rate")) ? 0 : Convert.ToDecimal(reader["invoice_rate"]),
                CostPrice = reader.IsDBNull(reader.GetOrdinal("cost_price")) ? 0 : Convert.ToDecimal(reader["cost_price"]),
                SellingPrice = reader.IsDBNull(reader.GetOrdinal("selling_price")) ? 0 : Convert.ToDecimal(reader["selling_price"]),
                LossVsCost = reader.IsDBNull(reader.GetOrdinal("loss_vs_cost")) ? 0 : Convert.ToDecimal(reader["loss_vs_cost"]),
                DiffVsSellingPrice = reader.IsDBNull(reader.GetOrdinal("diff_vs_selling_price")) ? 0 : Convert.ToDecimal(reader["diff_vs_selling_price"]),
                IsBelowCost = !reader.IsDBNull(reader.GetOrdinal("is_below_cost")) && reader.GetBoolean(reader.GetOrdinal("is_below_cost")),
                IsBelowSellingPrice = !reader.IsDBNull(reader.GetOrdinal("is_below_selling_price")) && reader.GetBoolean(reader.GetOrdinal("is_below_selling_price")),
                AlertType = SafeGetString(reader, "alert_type")
            }, CommandType.Text);

            return rows;
        }

    }
}
