using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Classes
{
    public class JournalRepository : BaseDAL, IJournalRepository
    {
        public JournalRepository(IConfiguration configuration, ITenantProvider tenantProvider) 
            : base(configuration, tenantProvider)
        {
        }

        public async Task<DbResult> InsertAsync(JournalVoucher entity, IDbTransaction? transaction = null, int? userId = null)
        {
            try
            {
                const string query = "SELECT public.fn_journal_voucher(" +
                                    "p_action := @p_action, p_jv_date := @p_jv_date, p_due_date := @p_due_date, " +
                                    "p_dr_account := @p_dr_account, p_cr_account := @p_cr_account, p_amount := @p_amount, " +
                                    "p_v_type := @p_v_type, p_v_code := @p_v_code, p_description := @p_description, " +
                                    "p_adjust_v_type := @p_adjust_v_type, p_jv_comp_code := @p_jv_comp_code, " +
                                    "p_g_code := @p_g_code, p_cheque_no := @p_cheque_no, p_user_code := @p_user_code, " +
                                    "p_group_jv_sr_no := @p_group_jv_sr_no, p_is_outstanding := @p_is_outstanding, " +
                                    "p_year_code := @p_year_code, p_branch_code := @p_branch_code, " +
                                    "p_created_by := @p_created_by, p_delete_flag := @p_delete_flag, p_user_id := @p_user_id)";

                var conn = (transaction?.Connection as NpgsqlConnection) ?? new NpgsqlConnection(GetConnectionString());
                if (transaction == null) await conn.OpenAsync();

                await using var command = new NpgsqlCommand(query, conn, transaction as NpgsqlTransaction)
                {
                    CommandType = CommandType.Text
                };
                
                // Use explicit parameter types to prevent PostgreSQL signature mismatch or type detection errors
                command.Parameters.Add(new NpgsqlParameter("p_action", NpgsqlTypes.NpgsqlDbType.Text) { Value = "INSERT" });
                command.Parameters.Add(new NpgsqlParameter("p_jv_date", NpgsqlTypes.NpgsqlDbType.TimestampTz) { Value = entity.JvDate.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(entity.JvDate, DateTimeKind.Utc) : entity.JvDate.ToUniversalTime() });
                command.Parameters.Add(new NpgsqlParameter("p_due_date", NpgsqlTypes.NpgsqlDbType.TimestampTz) { Value = entity.DueDate.HasValue ? (entity.DueDate.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(entity.DueDate.Value, DateTimeKind.Utc) : entity.DueDate.Value.ToUniversalTime()) : DBNull.Value });
                command.Parameters.Add(new NpgsqlParameter("p_dr_account", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = entity.DrAccountCode });
                command.Parameters.Add(new NpgsqlParameter("p_cr_account", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = entity.CrAccountCode });
                command.Parameters.Add(new NpgsqlParameter("p_amount", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = entity.Amount });
                command.Parameters.Add(new NpgsqlParameter("p_v_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = entity.VType ?? (object)DBNull.Value });
                //command.Parameters.Add(new NpgsqlParameter("p_v_code", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = entity.VCode });
                command.Parameters.Add(new NpgsqlParameter("p_v_code", NpgsqlTypes.NpgsqlDbType.Bigint)
                {
                    Value = (object?)entity.VCode ?? DBNull.Value
                });
                command.Parameters.Add(new NpgsqlParameter("p_description", NpgsqlTypes.NpgsqlDbType.Text) { Value = entity.Description ?? (object)DBNull.Value });
                command.Parameters.Add(new NpgsqlParameter("p_adjust_v_type", NpgsqlTypes.NpgsqlDbType.Text) { Value = entity.AdjustVType ?? (object)DBNull.Value });
                command.Parameters.Add(new NpgsqlParameter("p_jv_comp_code", NpgsqlTypes.NpgsqlDbType.Integer) { Value = entity.JvCompCode });
                command.Parameters.Add(new NpgsqlParameter("p_g_code", NpgsqlTypes.NpgsqlDbType.Text) { Value = entity.GCode ?? (object)DBNull.Value });
                command.Parameters.Add(new NpgsqlParameter("p_cheque_no", NpgsqlTypes.NpgsqlDbType.Text) { Value = entity.ChequeNo ?? (object)DBNull.Value });
                command.Parameters.Add(new NpgsqlParameter("p_user_code", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object?)entity.UserCode ?? DBNull.Value });
                command.Parameters.Add(new NpgsqlParameter("p_group_jv_sr_no", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object?)entity.GroupJvSrNo ?? DBNull.Value });
                command.Parameters.Add(new NpgsqlParameter("p_is_outstanding", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = entity.IsOutstanding });
                command.Parameters.Add(new NpgsqlParameter("p_year_code", NpgsqlTypes.NpgsqlDbType.Integer) { Value = entity.YearCode });
                command.Parameters.Add(new NpgsqlParameter("p_branch_code", NpgsqlTypes.NpgsqlDbType.Integer) { Value = entity.BranchCode });
                command.Parameters.Add(new NpgsqlParameter("p_created_by", NpgsqlTypes.NpgsqlDbType.Text) { Value = entity.CreatedBy ?? (object)DBNull.Value });
                command.Parameters.Add(new NpgsqlParameter("p_delete_flag", NpgsqlTypes.NpgsqlDbType.Boolean) { Value = entity.DeleteFlag });
                command.Parameters.Add(new NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value });

                await command.ExecuteNonQueryAsync();
                if (transaction == null) await conn.CloseAsync();

                Console.WriteLine($"[JournalRepository] SUCCESS: Inserted JV entry: Type={entity.VType}, Amount={entity.Amount}, GCode={entity.GCode}");
                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<DbResult> DeleteAsync(long jvCode)
        {
            try
            {
                await ExecuteNonQueryAsync("UPDATE tab_jv SET delete_flag = true, deleted_on = CURRENT_TIMESTAMP, deleted_by = @deleted_by WHERE jv_code = @jv_code", cmd =>
                {
                    cmd.Parameters.AddWithValue("jv_code", jvCode);
                    cmd.Parameters.AddWithValue("deleted_by", _tenantProvider?.UserId ?? (object)DBNull.Value);
                }, CommandType.Text);
                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<List<JournalVoucher>> GetByVoucherAsync(string vType, long vCode)
        {
            return await ExecuteReaderAsync("SELECT * FROM tab_jv WHERE v_type = @v_type AND v_code = @v_code AND delete_flag = false", cmd =>
            {
                cmd.Parameters.AddWithValue("v_type", vType);
                cmd.Parameters.AddWithValue("v_code", vCode);
            }, MapJournalVoucher, CommandType.Text);
        }

        public async Task<List<JournalVoucher>> GetByGCodeAsync(string gCode)
        {
            return await ExecuteReaderAsync("SELECT * FROM tab_jv WHERE g_code = @g_code AND delete_flag = false", cmd =>
            {
                cmd.Parameters.AddWithValue("g_code", gCode);
            }, MapJournalVoucher, CommandType.Text);
        }

        public async Task<IEnumerable<JournalVoucher>> GetDirectJournalsAsync(long companyId, string vType)
        {
            return await ExecuteRawQueryAsync(
                "SELECT base.*, " +
                "       COALESCE(dr_am.account_name, dr_pm.display_name, 'Unknown') as dr_account_name, " +
                "       COALESCE(cr_am.account_name, cr_pm.display_name, 'Unknown') as cr_account_name " +
                "FROM ( " +
                "    SELECT j1.g_code, MAX(j1.jv_date) as jv_date, MAX(j1.description) as description, " +
                "           SUM(j1.amount) as amount, MAX(j1.dr_account_code) as dr_account_code, " +
                "           MAX(j1.cr_account_code) as cr_account_code " +
                "    FROM tab_jv j1 " +
                "    WHERE j1.jv_comp_code = @companyId " +
                "    AND (@vType = 'ALL' OR j1.v_type = @vType) " +
                "    AND j1.delete_flag = false " +
                "    GROUP BY j1.g_code " +
                ") base " +
                "LEFT JOIN tab_account_master dr_am ON dr_am.account_id = base.dr_account_code " +
                "LEFT JOIN tab_party_master dr_pm ON dr_pm.party_id = base.dr_account_code " +
                "LEFT JOIN tab_account_master cr_am ON cr_am.account_id = base.cr_account_code " +
                "LEFT JOIN tab_party_master cr_pm ON cr_pm.party_id = base.cr_account_code " +
                "ORDER BY base.jv_date DESC",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("companyId", (int)companyId);
                    cmd.Parameters.AddWithValue("vType", vType);
                },
                reader => new JournalVoucher
                {
                    GCode = reader["g_code"]?.ToString(),
                    JvDate = Convert.ToDateTime(reader["jv_date"]),
                    Description = reader["description"]?.ToString(),
                    Amount = Convert.ToDecimal(reader["amount"]),
                    DrAccountCode = reader["dr_account_code"] != DBNull.Value ? Convert.ToInt64(reader["dr_account_code"]) : 0,
                    CrAccountCode = reader["cr_account_code"] != DBNull.Value ? Convert.ToInt64(reader["cr_account_code"]) : 0,
                    DrAccountName = reader["dr_account_name"]?.ToString(),
                    CrAccountName = reader["cr_account_name"]?.ToString(),
                    VType = vType
                });
        }

        public async Task<IEnumerable<JournalVoucher>> GetJournalsByDateAndAccountAsync(long companyId, DateTime jvDate, long accountCode)
        {
            return await ExecuteRawQueryAsync(
                "SELECT * FROM public.fn_get_journals_by_date_account(" +
                "p_company_id := @companyId, p_jv_date := @jvDate, p_account_code := @accountCode)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("companyId", (int)companyId);
                    cmd.Parameters.Add(new NpgsqlParameter("jvDate", NpgsqlTypes.NpgsqlDbType.Date)
                    {
                        Value = jvDate.Date
                    });
                    cmd.Parameters.AddWithValue("accountCode", accountCode);
                },
                reader => new JournalVoucher
                {
                    JvCode = reader["jv_code"] != DBNull.Value ? Convert.ToInt64(reader["jv_code"]) : 0,
                    GCode = reader["g_code"]?.ToString(),
                    JvDate = Convert.ToDateTime(reader["jv_date"]),
                    VType = reader["v_type"]?.ToString(),
                    Description = reader["description"]?.ToString(),
                    Amount = Convert.ToDecimal(reader["amount"]),
                    DrAccountCode = reader["dr_account_code"] != DBNull.Value ? Convert.ToInt64(reader["dr_account_code"]) : 0,
                    CrAccountCode = reader["cr_account_code"] != DBNull.Value ? Convert.ToInt64(reader["cr_account_code"]) : 0,
                    DrAccountName = reader["dr_account_name"]?.ToString(),
                    CrAccountName = reader["cr_account_name"]?.ToString()
                });
        }

        public async Task<DbResult> SaveJournalGroupAsync(IEnumerable<JournalVoucher> entries, string? gCodeToDelete = null, IDbTransaction? transaction = null, int? userId = null)
        {
            NpgsqlConnection? connection = (transaction?.Connection as NpgsqlConnection);
            bool isNewConnection = connection == null;
            if (isNewConnection) connection = new NpgsqlConnection(GetConnectionString());
            
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();
            
            NpgsqlTransaction? npgsqlTransaction = (transaction as NpgsqlTransaction);
            bool isNewTransaction = npgsqlTransaction == null;
            if (isNewTransaction) npgsqlTransaction = await connection.BeginTransactionAsync();

            try
            {
                // 1. Delete existing entries if GCode is provided (Soft Delete via Procedure)
                if (!string.IsNullOrEmpty(gCodeToDelete))
                {
                    const string delQuery = "SELECT public.fn_journal_voucher(p_action := 'DELETE', p_g_code := @g_code, p_jv_comp_code := @company_id, p_user_id := @user_id)";
                    await using var delCmd = new NpgsqlCommand(delQuery, connection, npgsqlTransaction);
                    delCmd.Parameters.AddWithValue("g_code", gCodeToDelete);
                    delCmd.Parameters.AddWithValue("company_id", _tenantProvider.CompanyId ?? 0);
                    delCmd.Parameters.AddWithValue("user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
                    await delCmd.ExecuteNonQueryAsync();
                }

                // 2. Insert new entries
                foreach (var entry in entries)
                {
                    var insertResult = await InsertAsync(entry, npgsqlTransaction, userId ?? _tenantProvider?.UserId);
                    if (!insertResult.Success)
                    {
                        if (isNewTransaction) await npgsqlTransaction.RollbackAsync();
                        return insertResult;
                    }
                }

                if (isNewTransaction) await npgsqlTransaction.CommitAsync();
                
                var gCode = entries.FirstOrDefault()?.GCode;
                if (!string.IsNullOrEmpty(gCode))
                {
                    var savedEntries = await GetByGCodeAsync(gCode);
                    return DbResult.Ok(savedEntries);
                }

                return DbResult.Ok();
            }
            catch (Exception ex)
            {
                if (isNewTransaction) await npgsqlTransaction.RollbackAsync();
                return HandleException(ex);
            }
            finally
            {
                if (isNewConnection) await connection.CloseAsync();
            }
        }

        private JournalVoucher MapJournalVoucher(NpgsqlDataReader reader)
        {
            return new JournalVoucher
            {
                JvCode = Convert.ToInt64(reader["jv_code"]),
                JvDate = Convert.ToDateTime(reader["jv_date"]),
                DueDate = reader["due_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["due_date"]),
                DrAccountCode = Convert.ToInt64(reader["dr_account_code"]),
                CrAccountCode = Convert.ToInt64(reader["cr_account_code"]),
                Amount = Convert.ToDecimal(reader["amount"]),
                VType = reader["v_type"]?.ToString(),
                VCode = reader["v_code"] == DBNull.Value ? (long?)null : Convert.ToInt64(reader["v_code"]),
                Description = reader["description"]?.ToString(),
                AdjustVType = reader["adjust_v_type"]?.ToString(),
                JvCompCode = Convert.ToInt32(reader["jv_comp_code"]),
                GCode = reader["g_code"]?.ToString(),
                ChequeNo = reader["cheque_no"]?.ToString(),
                UserCode = reader["user_code"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["user_code"]),
                GroupJvSrNo = reader["group_jv_sr_no"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["group_jv_sr_no"]),
                IsOutstanding = Convert.ToBoolean(reader["is_outstanding"]),
                YearCode = Convert.ToInt32(reader["year_code"]),
                BranchCode = Convert.ToInt32(reader["branch_code"]),
                DeleteFlag = Convert.ToBoolean(reader["delete_flag"]),
                CreatedBy = reader["created_by"]?.ToString(),
                CreatedOn = reader["created_on"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["created_on"]),
                ModifiedBy = reader["modify_by"]?.ToString(),
                ModifiedOn = reader["modify_on"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["modify_on"]),
                DeletedBy = reader["deleted_by"]?.ToString(),
                DeletedOn = reader["deleted_on"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["deleted_on"])
            };
        }
    }
}
