using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SoftBook.Infrastructure.Classes
{
    public class PayRecRepository : BaseDAL, IPayRecRepository
    {
        public PayRecRepository(IConfiguration configuration, ITenantProvider tenantProvider) 
            : base(configuration, tenantProvider) { }

        // ------------------------------------------------------------------
        // GETALL – uses PostgreSQL named-arg syntax so order doesn't matter
        // ------------------------------------------------------------------
        public async Task<List<PayRec>> GetAllAsync(long companyId, string? vchType = null)
        {
            return await ExecuteRawQueryAsync(
                "SELECT * FROM public.fn_pay_rec(p_action => @p_action, p_company_id => @p_company_id, p_vch_type => @p_vch_type)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "GETALL");
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                    cmd.Parameters.AddWithValue("p_vch_type", (object?)vchType ?? DBNull.Value);
                },
                reader => MapToPayRec(reader));
        }

        // ------------------------------------------------------------------
        // GETBYID (which actually fetches by JV Code to return all allocations)
        // ------------------------------------------------------------------
        public async Task<List<PayRec>> GetByIdAsync(long companyId, long code)
        {
            var list = await ExecuteRawQueryAsync(
                "SELECT * FROM public.fn_pay_rec(p_action => @p_action, p_company_id => @p_company_id, p_code => @p_code)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "GETBYJV");
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                    cmd.Parameters.AddWithValue("p_code", code); // code passed here is the jv_code
                },
                reader => MapToPayRec(reader));

            return list;
        }
        // ------------------------------------------------------------------
        // INSERT  (supports optional shared transaction)
        // ------------------------------------------------------------------
        public async Task<DbResult> InsertAsync(PayRec entity, IDbTransaction? transaction = null, int? userId = null)
        {
            try
            {
                const string sql =
                    "SELECT * FROM public.fn_pay_rec(" +
                    "p_action => @p_action, " +
                    "p_company_id => @p_company_id, " +
                    "p_date => @p_date, " +
                    "p_inv_code => @p_inv_code, " +
                    "p_jv_code => @p_jv_code, " +
                    "p_amount => @p_amount, " +
                    "p_disc => @p_disc, " +
                    "p_party_id => @p_party_id, " +
                    "p_account_id => @p_account_id, " +
                    "p_discount_amount => @p_discount_amount, " +
                    "p_vch_type => @p_vch_type, " +
                    "p_g_code => @p_g_code, " +
                    "p_year_code => @p_year_code, " +
                    "p_branch_code => @p_branch_code, " +
                    "p_user_code => @p_user_code, " +
                    "p_created_by => @p_created_by, " +
                    "p_user_id => @p_user_id)";

                NpgsqlConnection conn;
                NpgsqlTransaction? npgTx = transaction as NpgsqlTransaction;
                bool ownConn = transaction == null;

                if (ownConn)
                {
                    conn = new NpgsqlConnection(GetConnectionString());
                    await conn.OpenAsync();
                }
                else
                {
                    conn = (NpgsqlConnection)transaction!.Connection!;
                }

                await using var command = new NpgsqlCommand(sql, conn, npgTx) { CommandTimeout = 60 };
                AddInsertParams(command, entity, userId);

                PayRec? result = null;
                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync()) result = MapToPayRec(reader);

                if (ownConn) await conn.CloseAsync();

                if (result != null) return DbResult.Ok(result);
                return DbResult.Failure("Failed to insert Payment/Receipt record.");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private static void AddInsertParams(NpgsqlCommand cmd, PayRec entity, int? userId = null)
        {
            cmd.Parameters.AddWithValue("p_action", "INSERT");
            cmd.Parameters.AddWithValue("p_user_id", userId.HasValue ? (object)userId.Value : DBNull.Value);

            // p_company_id (bigint)
            cmd.Parameters.AddWithValue("p_company_id", entity.CompanyId.HasValue ? (object)entity.CompanyId.Value : DBNull.Value);

            // p_date – must be NpgsqlDbType.Date, NOT timestamp
            var dateParam = new NpgsqlParameter("p_date", NpgsqlTypes.NpgsqlDbType.Date)
            {
                Value = entity.Date.HasValue ? (object)entity.Date.Value.Date : DBNull.Value
            };
            cmd.Parameters.Add(dateParam);

            // nullable bigint params – must be explicitly typed so NULL isn't sent as 'unknown'
            var invCodeParam = new NpgsqlParameter("p_inv_code", NpgsqlTypes.NpgsqlDbType.Bigint)
            {
                Value = entity.InvCode.HasValue ? (object)entity.InvCode.Value : DBNull.Value
            };
            cmd.Parameters.Add(invCodeParam);

            var jvCodeParam = new NpgsqlParameter("p_jv_code", NpgsqlTypes.NpgsqlDbType.Bigint)
            {
                Value = entity.JvCode.HasValue ? (object)entity.JvCode.Value : DBNull.Value
            };
            cmd.Parameters.Add(jvCodeParam);

            cmd.Parameters.AddWithValue("p_amount", entity.Amount.HasValue ? (object)entity.Amount.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("p_disc", (object?)entity.Disc ?? DBNull.Value);

            var partyIdParam = new NpgsqlParameter("p_party_id", NpgsqlTypes.NpgsqlDbType.Bigint)
            {
                Value = entity.PartyId.HasValue ? (object)entity.PartyId.Value : DBNull.Value
            };
            cmd.Parameters.Add(partyIdParam);

            var accountIdParam = new NpgsqlParameter("p_account_id", NpgsqlTypes.NpgsqlDbType.Bigint)
            {
                Value = entity.AccountId.HasValue ? (object)entity.AccountId.Value : DBNull.Value
            };
            cmd.Parameters.Add(accountIdParam);

            cmd.Parameters.AddWithValue("p_discount_amount", entity.DiscountAmount);
            cmd.Parameters.AddWithValue("p_vch_type", (object?)entity.VchType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("p_g_code", (object?)entity.GCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("p_year_code", (object?)entity.YearCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("p_branch_code", (object?)entity.BranchCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("p_user_code", (object?)entity.UserCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("p_created_by", (object?)entity.CreatedBy ?? DBNull.Value);
        }

        // ------------------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------------------
        public async Task<DbResult> UpdateAsync(PayRec entity, int? userId = null)
        {
            try
            {
                const string sql =
                    "SELECT * FROM public.fn_pay_rec(" +
                    "p_action => @p_action, " +
                    "p_code => @p_code, " +
                    "p_company_id => @p_company_id, " +
                    "p_date => @p_date, " +
                    "p_inv_code => @p_inv_code, " +
                    "p_jv_code => @p_jv_code, " +
                    "p_amount => @p_amount, " +
                    "p_disc => @p_disc, " +
                    "p_party_id => @p_party_id, " +
                    "p_account_id => @p_account_id, " +
                    "p_discount_amount => @p_discount_amount, " +
                    "p_vch_type => @p_vch_type, " +
                    "p_g_code => @p_g_code, " +
                    "p_year_code => @p_year_code, " +
                    "p_branch_code => @p_branch_code, " +
                    "p_user_code => @p_user_code, " +
                    "p_user_id => @p_user_id)";

                var list = await ExecuteRawQueryAsync(sql, cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "UPDATE");
                    cmd.Parameters.AddWithValue("p_code", entity.Code);
                    cmd.Parameters.AddWithValue("p_company_id", entity.CompanyId.HasValue ? (object)entity.CompanyId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("p_user_id", userId.HasValue ? (object)userId.Value : DBNull.Value);

                    var dp = new NpgsqlParameter("p_date", NpgsqlTypes.NpgsqlDbType.Date)
                    {
                        Value = entity.Date.HasValue ? (object)entity.Date.Value.Date : DBNull.Value
                    };
                    cmd.Parameters.Add(dp);

                    cmd.Parameters.Add(new NpgsqlParameter("p_inv_code", NpgsqlTypes.NpgsqlDbType.Bigint)
                    {
                        Value = entity.InvCode.HasValue ? (object)entity.InvCode.Value : DBNull.Value
                    });
                    cmd.Parameters.Add(new NpgsqlParameter("p_jv_code", NpgsqlTypes.NpgsqlDbType.Bigint)
                    {
                        Value = entity.JvCode.HasValue ? (object)entity.JvCode.Value : DBNull.Value
                    });
                    cmd.Parameters.AddWithValue("p_amount", entity.Amount.HasValue ? (object)entity.Amount.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("p_disc", (object?)entity.Disc ?? DBNull.Value);
                    cmd.Parameters.Add(new NpgsqlParameter("p_party_id", NpgsqlTypes.NpgsqlDbType.Bigint)
                    {
                        Value = entity.PartyId.HasValue ? (object)entity.PartyId.Value : DBNull.Value
                    });
                    cmd.Parameters.Add(new NpgsqlParameter("p_account_id", NpgsqlTypes.NpgsqlDbType.Bigint)
                    {
                        Value = entity.AccountId.HasValue ? (object)entity.AccountId.Value : DBNull.Value
                    });
                    cmd.Parameters.AddWithValue("p_discount_amount", entity.DiscountAmount);
                    cmd.Parameters.AddWithValue("p_vch_type", (object?)entity.VchType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_g_code", (object?)entity.GCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_year_code", (object?)entity.YearCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_branch_code", (object?)entity.BranchCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_user_code", (object?)entity.UserCode ?? DBNull.Value);
                }, reader => MapToPayRec(reader));

                if (list.Count > 0) return DbResult.Ok(list[0]);
                return DbResult.Failure("Failed to update Payment/Receipt record.");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        // ------------------------------------------------------------------
        // DELETE (single by code)
        // ------------------------------------------------------------------
        public async Task<DbResult> DeleteAsync(long companyId, long code, string deletedBy, int? userId = null)
        {
            try
            {
                await ExecuteRawQueryAsync(
                    "SELECT * FROM public.fn_pay_rec(p_action => @p_action, p_company_id => @p_company_id, p_code => @p_code, p_deleted_by => @p_deleted_by, p_user_id => @p_user_id)",
                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("p_action", "DELETE");
                        cmd.Parameters.AddWithValue("p_company_id", companyId);
                        cmd.Parameters.AddWithValue("p_code", code);
                        cmd.Parameters.AddWithValue("p_deleted_by", deletedBy);
                        cmd.Parameters.AddWithValue("p_user_id", userId.HasValue ? (object)userId.Value : DBNull.Value);
                    },
                    reader => (object)reader[0]);

                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // ------------------------------------------------------------------
        // DELETE by GCode (soft delete – direct SQL, shares transaction)
        // ------------------------------------------------------------------
        public async Task<DbResult> DeleteByGCodeAsync(string gCode, string deletedBy, IDbTransaction? transaction = null, int? userId = null)
        {
            try
            {
                // To enforce permissions correctly for Payments/Receipts, we need to know the voucher type.
                // We'll fetch the records for this GCode first.
                var list = await ExecuteRawQueryAsync(
                    "SELECT * FROM tab_pay_rec WHERE g_code = @g_code AND company_id = @company_id AND delete_flag = FALSE",
                    cmd => {
                        cmd.Parameters.AddWithValue("@g_code", gCode);
                        cmd.Parameters.AddWithValue("@company_id", _tenantProvider?.CompanyId ?? 0);
                    }, reader => MapToPayRec(reader));

                foreach (var item in list)
                {
                    // Pass the vch_type via something if needed, but fn_pay_rec handles it if we call DELETE with correct code.
                    // Actually fn_pay_rec DELETE doesn't check vch_type in my rewrite unless we pass it.
                    // Wait, I updated fn_pay_rec to check vch_type. So we MUST pass p_vch_type to DELETE too.
                    
                    await ExecuteRawQueryAsync(
                        "SELECT * FROM public.fn_pay_rec(p_action => @p_action, p_company_id => @p_company_id, p_code => @p_code, p_deleted_by => @p_deleted_by, p_user_id => @p_user_id, p_vch_type => @p_vch_type)",
                        cmd =>
                        {
                            cmd.Parameters.AddWithValue("p_action", "DELETE");
                            cmd.Parameters.AddWithValue("p_company_id", item.CompanyId ?? 0);
                            cmd.Parameters.AddWithValue("p_code", item.Code);
                            cmd.Parameters.AddWithValue("p_deleted_by", deletedBy);
                            cmd.Parameters.AddWithValue("p_user_id", userId.HasValue ? (object)userId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("p_vch_type", (object?)item.VchType ?? DBNull.Value);
                        },
                        reader => (object)reader[0]);
                }

                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // ------------------------------------------------------------------
        // Pending invoices & payment list (unchanged helpers)
        // ------------------------------------------------------------------
        public async Task<IEnumerable<dynamic>> GetPendingInvoicesAsync(string trnType, int partyId)
        {
            return await ExecuteRawQueryAsync(
                "SELECT * FROM fn_get_pending_invoices(p_trn_type => @p_trn_type, p_party_id => @p_party_id)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_trn_type", trnType);
                    cmd.Parameters.AddWithValue("p_party_id", partyId);
                },
                reader => (dynamic)new
                {
                    inv_code         = SafeGetLong(reader, "inv_code"),
                    inv_no           = SafeGetString(reader, "inv_no"),
                    inv_type         = SafeGetString(reader, "inv_type"),
                    inv_date         = SafeGetDateTime(reader, "inv_date"),
                    due_date         = SafeGetDateTime(reader, "due_date"),
                    party_id         = SafeGetLong(reader, "party_id"),
                    bill_id          = SafeGetInt(reader, "bill_id"),
                    tot_sku_in_invoice = SafeGetInt(reader, "tot_sku_in_invoice"),
                    total_amount     = SafeGetDecimal(reader, "total_amount"),
                    paid_amount      = SafeGetDecimal(reader, "paid_amount"),
                    pending_amount   = SafeGetDecimal(reader, "pending_amount")
                });
        }

        public async Task<IEnumerable<dynamic>> GetPaymentListAsync(long companyId, int pageNo, int pageSize, string? vType, int? partyId)
        {
            return await ExecuteRawQueryAsync(
                "SELECT * FROM fn_payment_list(p_company_id => @p_company_id, p_page_no => @p_page_no, p_page_size => @p_page_size, p_v_type => @p_v_type, p_party_id => @p_party_id)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_company_id", (int)companyId);
                    cmd.Parameters.AddWithValue("p_page_no", pageNo);
                    cmd.Parameters.AddWithValue("p_page_size", pageSize);
                    cmd.Parameters.AddWithValue("p_v_type", (object?)vType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_party_id", (object?)partyId ?? DBNull.Value);
                },
                reader => (dynamic)new
                {
                    sr_no          = SafeGetLong(reader, "sr_no"),
                    jv_code        = SafeGetLong(reader, "jv_code"),
                    inv_no         = SafeGetString(reader, "inv_no"),
                    bill_no        = SafeGetInt(reader, "bill_no"),
                    jv_date        = SafeGetDateTime(reader, "jv_date"),
                    due_date       = SafeGetDateTime(reader, "due_date"),
                    v_type         = SafeGetString(reader, "v_type"),
                    main_account_id = SafeGetLong(reader, "main_account"),
                    trn_account_id  = SafeGetLong(reader, "trn_account"),
                    description    = SafeGetString(reader, "description"),
                    party_name     = SafeGetString(reader, "party_name"),
                    account_name   = SafeGetString(reader, "account_name"),
                    amount         = SafeGetDecimal(reader, "amount"),
                    sp_amount      = SafeGetDecimal(reader, "sp_amount")
                });
        }

        // ------------------------------------------------------------------
        // Helper: expose connection string to service layer
        // ------------------------------------------------------------------
        public string GetConnectionStringForService() => GetConnectionString();

        // ------------------------------------------------------------------
        // Mapping
        // ------------------------------------------------------------------
        private PayRec MapToPayRec(NpgsqlDataReader reader)
        {
            return new PayRec
            {
                Code           = SafeGetLong(reader, "code"),
                Date           = SafeGetDateTime(reader, "date"),
                InvCode        = reader["inv_code"] != DBNull.Value ? SafeGetLong(reader, "inv_code") : null,
                JvCode         = reader["jv_code"]  != DBNull.Value ? SafeGetLong(reader, "jv_code")  : null,
                Amount         = reader["amount"]   != DBNull.Value ? Convert.ToDecimal(reader["amount"]) : null,
                Disc           = SafeGetString(reader, "disc"),
                CreatedBy      = SafeGetString(reader, "created_by"),
                CreatedOn      = SafeGetDateTime(reader, "created_on"),
                UserCode       = SafeGetInt(reader, "user_code"),
                PostCheque     = reader["post_cheque"] != DBNull.Value ? SafeGetLong(reader, "post_cheque") : null,
                CompanyId      = reader["company_id"]  != DBNull.Value ? SafeGetLong(reader, "company_id")  : null,
                FullHalf       = SafeGetString(reader, "full_half"),
                YearCode       = SafeGetInt(reader, "year_code"),
                BranchCode     = SafeGetInt(reader, "branch_code"),
                VchType        = SafeGetString(reader, "vch_type"),
                DeletedBy      = SafeGetString(reader, "deleted_by"),
                DeletedOn      = SafeGetDateTime(reader, "deleted_on"),
                DeleteFlag     = SafeGetBool(reader, "delete_flag") ?? false,
                PartyId        = reader["party_id"]   != DBNull.Value ? SafeGetLong(reader, "party_id")   : null,
                AccountId      = reader["account_id"] != DBNull.Value ? SafeGetLong(reader, "account_id") : null,
                DiscountAmount = reader["discount_amount"] != DBNull.Value ? Convert.ToDecimal(reader["discount_amount"]) : 0,
                GCode          = SafeGetString(reader, "g_code")
            };
        }

        private long SafeGetLong(NpgsqlDataReader reader, string col, long def = 0)
        {
            try
            {
                int o = reader.GetOrdinal(col);
                return reader.IsDBNull(o) ? def : Convert.ToInt64(reader.GetValue(o));
            }
            catch (IndexOutOfRangeException) { return def; }
        }

        private decimal SafeGetDecimal(NpgsqlDataReader reader, string col, decimal def = 0)
        {
            try
            {
                int o = reader.GetOrdinal(col);
                return reader.IsDBNull(o) ? def : Convert.ToDecimal(reader.GetValue(o));
            }
            catch (IndexOutOfRangeException) { return def; }
        }
    }
}
