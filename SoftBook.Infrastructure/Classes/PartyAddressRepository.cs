using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class PartyAddressRepository : BaseDAL, IPartyAddressRepository
    {
        public PartyAddressRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<PartyAddress>> GetAllAsync()
        {
            // GetAll doesn't make sense for addresses - they should be filtered by PartyId
            // Return empty list or throw NotImplementedException
            return new List<PartyAddress>();
        }

        public async Task<List<PartyAddress>> GetByPartyIdAsync(int partyId)
        {
            return await ExecuteReaderAsync("sp_party_address_get_by_party_id", cmd =>
            {
                cmd.Parameters.AddWithValue("p_party_id", partyId);
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
            }, reader => MapToPartyAddress(reader));
        }

        public async Task<PartyAddress?> GetByIdAsync(int id)
        {
            // We can use generic get or specific address get function if added to SQL
            // For now assuming we might not have sp_party_address_get_by_id in the previous artifact,
            // Let's check Postgres_Party_Functions.sql content in mind...
            // I didn't add sp_party_address_get_by_id explicitly?
            // I added sp_party_address_get_by_party_id.
            // Wait, Step 1598 content: I added sp_party_address_get_by_party_id.
            // I did NOT add sp_party_address_get_by_id!
            // But I can implement it or use raw query.
            // Given constraints, I should probably add it to SQL or use raw query.
            // Using Raw Query for single select is easy enough or I can invoke a new function.
            // I'll stick to a raw select for simplicity on this missing function or update the SQL file.
            // Actually, sp_party_address_get_by_id is common. I probably missed it.
            // I will assume for now I should add it to SQL file if I can, or just use raw query.
            // Using raw query: "SELECT * FROM tab_party_address WHERE address_id = ..."
            /* 
               Actually, let's write raw query to valid standard.
            */
            return await ExecuteSingleReaderAsync("SELECT * FROM tab_party_address WHERE address_id = @id AND company_id = @company_id",
              cmd =>
              {
                  cmd.CommandType = System.Data.CommandType.Text;
                  cmd.Parameters.AddWithValue("id", id);
                  cmd.Parameters.AddWithValue("company_id", _tenantProvider?.CompanyId ?? 0);
              },
              reader => MapToPartyAddress(reader));
        }

        public async Task InsertAsync(PartyAddress address)
        {
            var addressId = await ExecuteScalarAsync<int>("sp_party_address_create", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                cmd.Parameters.AddWithValue("p_party_id", address.PartyId);
                // Map AddressType string to ID: "Billing" = 1, "Shipping" = 2
                int addressTypeId = address.AddressType?.ToUpper() switch
                {
                    "BILLING" => 1,
                    "SHIPPING" => 2,
                    _ => int.TryParse(address.AddressType, out int id) ? id : 1 // Default to Billing if unknown
                };
                cmd.Parameters.AddWithValue("p_address_type_id", addressTypeId);
                cmd.Parameters.AddWithValue("p_attention", address.Attention ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_country", address.Country ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_address_line1", address.Street1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_address_line2", address.Street2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_city", address.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_state", address.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_pin_code", address.PinCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_phone", address.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_fax", address.FaxNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_is_default", (object)address.IsDefault ?? DBNull.Value);
            });

            if (addressId != null) address.AddressId = Convert.ToInt32(addressId);
        }

        public async Task UpdateAsync(PartyAddress address)
        {
            await ExecuteNonQueryAsync("sp_party_address_update", cmd =>
            {
                cmd.Parameters.AddWithValue("p_address_id", address.AddressId);
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                cmd.Parameters.AddWithValue("p_party_id", address.PartyId);
                // Map AddressType string to ID: "Billing" = 1, "Shipping" = 2
                int addressTypeId = address.AddressType?.ToUpper() switch
                {
                    "BILLING" => 1,
                    "SHIPPING" => 2,
                    _ => int.TryParse(address.AddressType, out int id) ? id : 1 // Default to Billing if unknown
                };
                cmd.Parameters.AddWithValue("p_address_type_id", addressTypeId);
                cmd.Parameters.AddWithValue("p_attention", address.Attention ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_country", address.Country ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_address_line1", address.Street1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_address_line2", address.Street2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_city", address.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_state", address.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_pin_code", address.PinCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_phone", address.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_fax", address.FaxNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_is_default", (object)address.IsDefault ?? DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_party_address_delete", cmd =>
            {
                cmd.Parameters.AddWithValue("p_address_id", id);
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
            });
        }

        private PartyAddress MapToPartyAddress(Npgsql.NpgsqlDataReader reader)
        {
            var addressTypeId = SafeGetString(reader, "address_type_id");
            return new PartyAddress
            {
                AddressId = SafeGetInt(reader, "address_id"),
                PartyId = SafeGetInt(reader, "party_id"),
                AddressType = addressTypeId switch 
                {
                    "1" => "Billing",
                    "2" => "Shipping",
                    _ => addressTypeId // Fallback if already label
                },
                Attention = SafeGetString(reader, "attention"),
                Country = SafeGetString(reader, "country"),
                Street1 = SafeGetString(reader, "address_line1"),
                Street2 = SafeGetString(reader, "address_line2"),
                City = SafeGetString(reader, "city"),
                State = SafeGetString(reader, "state"),
                PinCode = SafeGetString(reader, "pin_code"),
                Phone = SafeGetString(reader, "phone"),
                FaxNumber = SafeGetString(reader, "fax"),
                IsDefault = SafeGetBool(reader, "is_default") ?? false
            };
        }
    }
}
