using SoftBook.Infrastructure.Interfaces;
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
            return await ExecuteReaderAsync("sp_PartyAddress_Master",
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new PartyAddress
                {
                    AddressId = SafeGetInt(reader, "AddressId"),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    AddressType = SafeGetString(reader, "AddressType", SafeGetString(reader, "AddressTypeId")),
                    Attention = SafeGetString(reader, "Attention"),
                    Country = SafeGetString(reader, "Country"),
                    Street1 = SafeGetString(reader, "Street1", SafeGetString(reader, "AddressLine1")),
                    Street2 = SafeGetString(reader, "Street2", SafeGetString(reader, "AddressLine2")),
                    City = SafeGetString(reader, "City"),
                    State = SafeGetString(reader, "State"),
                    PinCode = SafeGetString(reader, "PinCode"),
                    Phone = SafeGetString(reader, "Phone"),
                    FaxNumber = SafeGetString(reader, "FaxNumber", SafeGetString(reader, "Fax")),
                    IsDefault = SafeGetBool(reader, "IsDefault"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<PartyAddress?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_PartyAddress_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@AddressId", id);
                },
                reader => new PartyAddress
                {
                    AddressId = SafeGetInt(reader, "AddressId"),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    AddressType = SafeGetString(reader, "AddressType", SafeGetString(reader, "AddressTypeId")),
                    Attention = SafeGetString(reader, "Attention"),
                    Country = SafeGetString(reader, "Country"),
                    Street1 = SafeGetString(reader, "Street1", SafeGetString(reader, "AddressLine1")),
                    Street2 = SafeGetString(reader, "Street2", SafeGetString(reader, "AddressLine2")),
                    City = SafeGetString(reader, "City"),
                    State = SafeGetString(reader, "State"),
                    PinCode = SafeGetString(reader, "PinCode"),
                    Phone = SafeGetString(reader, "Phone"),
                    FaxNumber = SafeGetString(reader, "FaxNumber", SafeGetString(reader, "Fax")),
                    IsDefault = SafeGetBool(reader, "IsDefault"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<List<PartyAddress>> GetByPartyIdAsync(int partyId)
        {
            return await ExecuteReaderAsync("sp_PartyAddress_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_PARTY_ID");
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                },
                reader => new PartyAddress
                {
                    AddressId = SafeGetInt(reader, "AddressId"),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    AddressType = SafeGetString(reader, "AddressType", SafeGetString(reader, "AddressTypeId")),
                    Attention = SafeGetString(reader, "Attention"),
                    Country = SafeGetString(reader, "Country"),
                    Street1 = SafeGetString(reader, "Street1", SafeGetString(reader, "AddressLine1")),
                    Street2 = SafeGetString(reader, "Street2", SafeGetString(reader, "AddressLine2")),
                    City = SafeGetString(reader, "City"),
                    State = SafeGetString(reader, "State"),
                    PinCode = SafeGetString(reader, "PinCode"),
                    Phone = SafeGetString(reader, "Phone"),
                    FaxNumber = SafeGetString(reader, "FaxNumber", SafeGetString(reader, "Fax")),
                    IsDefault = SafeGetBool(reader, "IsDefault"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task InsertAsync(PartyAddress address)
        {
            await ExecuteNonQueryAsync("sp_PartyAddress_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@PartyId", address.PartyId);
                cmd.Parameters.AddWithValue("@AddressType", address.AddressType);
                cmd.Parameters.AddWithValue("@Attention", address.Attention ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", address.Country ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Street1", address.Street1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Street2", address.Street2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", address.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", address.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PinCode", address.PinCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", address.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FaxNumber", address.FaxNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsDefault", address.IsDefault ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(PartyAddress address)
        {
            await ExecuteNonQueryAsync("sp_PartyAddress_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@AddressId", address.AddressId);
                cmd.Parameters.AddWithValue("@PartyId", address.PartyId);
                cmd.Parameters.AddWithValue("@AddressType", address.AddressType);
                cmd.Parameters.AddWithValue("@Attention", address.Attention ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", address.Country ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Street1", address.Street1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Street2", address.Street2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", address.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", address.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PinCode", address.PinCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", address.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FaxNumber", address.FaxNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsDefault", address.IsDefault ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_PartyAddress_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@AddressId", id);
            });
        }
    }
}
