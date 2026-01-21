using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class AccountRepository : BaseDAL, IAccountRepository
    {
        public AccountRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<AccountHead>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Account_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new AccountHead
            {
                AccountId = Convert.ToInt32(reader["AccountId"]),
                AccountType = reader["AccountType"]?.ToString(),
                AccountName = reader["AccountName"]?.ToString(),
                IsSubAccount = reader["IsSubAccount"] != DBNull.Value ? Convert.ToBoolean(reader["IsSubAccount"]) : null,
                ParentAccountId = reader["ParentAccountId"] != DBNull.Value ? Convert.ToInt32(reader["ParentAccountId"]) : null,
                AccountCode = reader["AccountCode"]?.ToString(),
                Description = reader["Description"]?.ToString()
            });
        }

        public async Task<AccountHead?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Account_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@AccountId", id);
                },
                reader => new AccountHead
                {
                    AccountId = Convert.ToInt32(reader["AccountId"]),
                    AccountType = reader["AccountType"]?.ToString(),
                    AccountName = reader["AccountName"]?.ToString(),
                    IsSubAccount = reader["IsSubAccount"] != DBNull.Value ? Convert.ToBoolean(reader["IsSubAccount"]) : null,
                    ParentAccountId = reader["ParentAccountId"] != DBNull.Value ? Convert.ToInt32(reader["ParentAccountId"]) : null,
                    AccountCode = reader["AccountCode"]?.ToString(),
                    Description = reader["Description"]?.ToString()
                });
        }

        public async Task InsertAsync(AccountHead account)
        {
            await ExecuteNonQueryAsync("sp_Account_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@AccountType", account.AccountType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountName", account.AccountName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsSubAccount", account.IsSubAccount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentAccountId", account.ParentAccountId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountCode", account.AccountCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", account.Description ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(AccountHead account)
        {
            await ExecuteNonQueryAsync("sp_Account_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@AccountId", account.AccountId);
                cmd.Parameters.AddWithValue("@AccountType", account.AccountType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountName", account.AccountName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsSubAccount", account.IsSubAccount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentAccountId", account.ParentAccountId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountCode", account.AccountCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", account.Description ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Account_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@AccountId", id);
            });
        }
    }
}
