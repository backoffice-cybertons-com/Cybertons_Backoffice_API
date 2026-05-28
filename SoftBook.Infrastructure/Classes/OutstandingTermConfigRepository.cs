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
    public class OutstandingTermConfigRepository : BaseDAL, IOutstandingTermConfigRepository
    {
        public OutstandingTermConfigRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        private static OutstandingTermConfigModel Map(NpgsqlDataReader reader)
        {
            return new OutstandingTermConfigModel
            {
                ConfigId   = Convert.ToInt64(reader["config_id"]),
                CompanyId  = Convert.ToInt64(reader["company_id"]),
                MinDays    = Convert.ToInt32(reader["min_days"]),
                MaxDays    = Convert.ToInt32(reader["max_days"]),
                BgColor    = reader["bg_color"]   == DBNull.Value ? null : reader["bg_color"]?.ToString(),
                IsActive   = reader["is_active"]  != DBNull.Value && Convert.ToBoolean(reader["is_active"]),
                CreatedAt  = reader["created_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["created_at"])
            };
        }

        public async Task<long> AddAsync(CreateOutstandingTermConfigRequest request, long companyId)
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await EnsureNoOverlapAsync(
                    connection,
                    transaction,
                    companyId,
                    request.MinDays!.Value,
                    request.MaxDays!.Value,
                    excludeConfigId: null);

                const string insertSql = @"
                    INSERT INTO tab_outstanding_term_config
                        (company_id, min_days, max_days, bg_color, is_active)
                    VALUES
                        (@CompanyId, @MinDays, @MaxDays, @BgColor, @IsActive)
                    RETURNING config_id;";

                await using var cmd = new NpgsqlCommand(insertSql, connection, transaction)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@CompanyId", companyId);
                cmd.Parameters.AddWithValue("@MinDays",   request.MinDays!.Value);
                cmd.Parameters.AddWithValue("@MaxDays",   request.MaxDays!.Value);
                cmd.Parameters.AddWithValue("@BgColor",
                    (object?)request.BgColor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive",  request.IsActive);

                var result = await cmd.ExecuteScalarAsync();
                var configId = Convert.ToInt64(result);

                await transaction.CommitAsync();
                return configId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateOutstandingTermConfigRequest request, long companyId)
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Ensure the record exists and lock it for update.
                const string existsSql = @"
                    SELECT 1
                    FROM tab_outstanding_term_config
                    WHERE config_id = @ConfigId  
                    FOR UPDATE;";

                await using (var existsCmd = new NpgsqlCommand(existsSql, connection, transaction))
                {
                    existsCmd.Parameters.AddWithValue("@ConfigId", request.ConfigId);
                    var exists = await existsCmd.ExecuteScalarAsync();
                    if (exists == null)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                await EnsureNoOverlapAsync(
                    connection,
                    transaction,
                    companyId,
                    request.MinDays!.Value,
                    request.MaxDays!.Value,
                    excludeConfigId: request.ConfigId);

                const string updateSql = @"
                    UPDATE tab_outstanding_term_config
                    SET
                        company_id = @CompanyId,
                        min_days   = @MinDays,
                        max_days   = @MaxDays,
                        bg_color = @BgColor,
                        is_active  = @IsActive
                    WHERE config_id = @ConfigId
                    RETURNING config_id;";

                await using var cmd = new NpgsqlCommand(updateSql, connection, transaction)
                {
                    CommandType = CommandType.Text
                };

                cmd.Parameters.AddWithValue("@ConfigId",  request.ConfigId);
                cmd.Parameters.AddWithValue("@CompanyId", companyId);
                cmd.Parameters.AddWithValue("@MinDays",   request.MinDays!.Value);
                cmd.Parameters.AddWithValue("@MaxDays",   request.MaxDays!.Value);
                cmd.Parameters.AddWithValue("@BgColor",   (object?)request.BgColor   ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive",  request.IsActive);

                var updatedId = await cmd.ExecuteScalarAsync();
                var ok = updatedId != null && updatedId != DBNull.Value && Convert.ToInt64(updatedId) > 0;

                await transaction.CommitAsync();
                return ok;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> SoftDeleteAsync(long configId)
        {
            const string sql = @"
                UPDATE tab_outstanding_term_config
                SET is_active = false
                WHERE config_id = @ConfigId
                RETURNING config_id;";

            var deletedId = await ExecuteScalarAsync<long>(
                sql,
                cmd => cmd.Parameters.AddWithValue("@ConfigId", configId),
                CommandType.Text);

            return deletedId > 0;
        }

        public Task<OutstandingTermConfigModel?> GetByIdAsync(long configId)
        {
            const string sql = @"
                SELECT config_id, company_id, min_days, max_days, bg_color, is_active, created_at
                FROM tab_outstanding_term_config
                WHERE config_id = @ConfigId
                LIMIT 1;";

            return ExecuteSingleReaderAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("@ConfigId", configId),
                Map,
                CommandType.Text);
        }

        public async Task<List<OutstandingTermConfigModel>> GetActiveByCompanyAsync(long companyId)
        {
            const string sql = @"
                SELECT config_id, company_id, min_days, max_days, bg_color, is_active, created_at
                FROM tab_outstanding_term_config
                WHERE company_id = @CompanyId
                  AND is_active = true
                ORDER BY min_days ASC;";

            return await ExecuteReaderAsync(
                sql,
                cmd => cmd.Parameters.AddWithValue("@CompanyId", companyId),
                Map,
                CommandType.Text);
        }

        private static async Task EnsureNoOverlapAsync(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
            long companyId,
            int minDays,
            int maxDays,
            long? excludeConfigId)
        {
            const string overlapSql = @"
                SELECT 1
FROM tab_outstanding_term_config
WHERE company_id = @CompanyId

  AND COALESCE(is_active, TRUE) = TRUE

 
  AND (
        @ExcludeConfigId IS NULL
        OR config_id <> @ExcludeConfigId
      )

  AND (
        @MinDays <= max_days
        AND @MaxDays >= min_days
      )

LIMIT 1
FOR UPDATE;";

            await using var cmd = new NpgsqlCommand(overlapSql, connection, transaction)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            cmd.Parameters.Add(new NpgsqlParameter("@ExcludeConfigId", NpgsqlTypes.NpgsqlDbType.Bigint) { Value = (object?)excludeConfigId ?? DBNull.Value });
            cmd.Parameters.AddWithValue("@MinDays", minDays);
            cmd.Parameters.AddWithValue("@MaxDays", maxDays);

            var exists = await cmd.ExecuteScalarAsync();
            if (exists != null)
            {
                throw new ArgumentException("Overlapping range exists for this company. Please choose a non-overlapping min_days/max_days.");
            }
        }
    }
}

