using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class TransporterRepository : BaseDAL, ITransporterRepository
    {
        public TransporterRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Transporter>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Transporter_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Transporter
            {
                TransporterId = Convert.ToInt32(reader["TransporterId"]),
                TransporterName = reader["TransporterName"]?.ToString(),
                GSTNumber = reader["GSTNumber"]?.ToString(),
                ContactPerson = reader["ContactPerson"]?.ToString(),
                Phone = reader["Phone"]?.ToString(),
                Address = reader["Address"]?.ToString()
            });
        }

        public async Task<Transporter?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Transporter_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@TransporterId", id);
                },
                reader => new Transporter
                {
                    TransporterId = Convert.ToInt32(reader["TransporterId"]),
                    TransporterName = reader["TransporterName"]?.ToString(),
                    GSTNumber = reader["GSTNumber"]?.ToString(),
                    ContactPerson = reader["ContactPerson"]?.ToString(),
                    Phone = reader["Phone"]?.ToString(),
                    Address = reader["Address"]?.ToString()
                });
        }

        public async Task InsertAsync(Transporter item)
        {
            await ExecuteNonQueryAsync("sp_Transporter_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@TransporterName", item.TransporterName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GSTNumber", item.GSTNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactPerson", item.ContactPerson ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", item.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", item.Address ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Transporter item)
        {
            await ExecuteNonQueryAsync("sp_Transporter_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@TransporterId", item.TransporterId);
                cmd.Parameters.AddWithValue("@TransporterName", item.TransporterName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GSTNumber", item.GSTNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactPerson", item.ContactPerson ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", item.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", item.Address ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Transporter_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@TransporterId", id);
            });
        }
    }
}
