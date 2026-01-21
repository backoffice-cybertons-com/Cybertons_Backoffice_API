using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class PaymentModeRepository : BaseDAL, IPaymentModeRepository
    {
        public PaymentModeRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<PaymentMode>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_PaymentMode_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new PaymentMode
            {
                PaymentModeId = Convert.ToInt32(reader["PaymentModeId"]),
                ModeName = reader["ModeName"]?.ToString(),
                IsDefault = Convert.ToBoolean(reader["IsDefault"])
            });
        }

        public async Task<PaymentMode?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_PaymentMode_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@PaymentModeId", id);
                },
                reader => new PaymentMode
                {
                    PaymentModeId = Convert.ToInt32(reader["PaymentModeId"]),
                    ModeName = reader["ModeName"]?.ToString(),
                    IsDefault = Convert.ToBoolean(reader["IsDefault"])
                });
        }

        public async Task InsertAsync(PaymentMode paymentMode)
        {
            await ExecuteNonQueryAsync("sp_PaymentMode_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@ModeName", paymentMode.ModeName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsDefault", paymentMode.IsDefault);
            });
        }

        public async Task UpdateAsync(PaymentMode paymentMode)
        {
            await ExecuteNonQueryAsync("sp_PaymentMode_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@PaymentModeId", paymentMode.PaymentModeId);
                cmd.Parameters.AddWithValue("@ModeName", paymentMode.ModeName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsDefault", paymentMode.IsDefault);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_PaymentMode_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@PaymentModeId", id);
            });
        }
    }
}
