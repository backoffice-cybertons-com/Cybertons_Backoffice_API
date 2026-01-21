using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class BankRepository : BaseDAL, IBankRepository
    {
        public BankRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Bank>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Bank_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Bank
            {
                BankMasterId = Convert.ToInt32(reader["BankMasterId"]),
                BankName = reader["BankName"]?.ToString() ?? string.Empty,
                Branch = reader["Branch"]?.ToString(),
                AccountNumber = reader["AccountNumber"]?.ToString(),
                IFSC = reader["IFSC"]?.ToString(),
                MICR = reader["MICR"]?.ToString()
            });
        }

        public async Task<Bank?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Bank_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@BankMasterId", id);
                },
                reader => new Bank
                {
                    BankMasterId = Convert.ToInt32(reader["BankMasterId"]),
                    BankName = reader["BankName"]?.ToString() ?? string.Empty,
                    Branch = reader["Branch"]?.ToString(),
                    AccountNumber = reader["AccountNumber"]?.ToString(),
                    IFSC = reader["IFSC"]?.ToString(),
                    MICR = reader["MICR"]?.ToString()
                });
        }

        public async Task InsertAsync(Bank bank)
        {
            await ExecuteNonQueryAsync("sp_Bank_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@BankName", bank.BankName);
                cmd.Parameters.AddWithValue("@Branch", bank.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IFSC", bank.IFSC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MICR", bank.MICR ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Bank bank)
        {
            await ExecuteNonQueryAsync("sp_Bank_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@BankMasterId", bank.BankMasterId);
                cmd.Parameters.AddWithValue("@BankName", bank.BankName);
                cmd.Parameters.AddWithValue("@Branch", bank.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountNumber", bank.AccountNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IFSC", bank.IFSC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MICR", bank.MICR ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Bank_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@BankMasterId", id);
            });
        }
    }
}
