using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class PaymentTermRepository : BaseDAL, IPaymentTermRepository
    {
        public PaymentTermRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<PaymentTerm>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_PaymentTerm_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new PaymentTerm
            {
                TermId = Convert.ToInt32(reader["TermId"]),
                TermName = reader["TermName"]?.ToString(),
                NumberOfDays = reader["NumberOfDays"] != DBNull.Value ? Convert.ToInt32(reader["NumberOfDays"]) : null
            });
        }

        public async Task<PaymentTerm?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_PaymentTerm_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@TermId", id);
                },
                reader => new PaymentTerm
                {
                    TermId = Convert.ToInt32(reader["TermId"]),
                    TermName = reader["TermName"]?.ToString(),
                    NumberOfDays = reader["NumberOfDays"] != DBNull.Value ? Convert.ToInt32(reader["NumberOfDays"]) : null
                });
        }

        public async Task InsertAsync(PaymentTerm item)
        {
            await ExecuteNonQueryAsync("sp_PaymentTerm_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@TermName", item.TermName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NumberOfDays", item.NumberOfDays ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(PaymentTerm item)
        {
            await ExecuteNonQueryAsync("sp_PaymentTerm_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@TermId", item.TermId);
                cmd.Parameters.AddWithValue("@TermName", item.TermName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NumberOfDays", item.NumberOfDays ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_PaymentTerm_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@TermId", id);
            });
        }
    }
}
