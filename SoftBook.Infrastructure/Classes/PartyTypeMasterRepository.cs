using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class PartyTypeMasterRepository : BaseDAL, IPartyTypeMasterRepository
    {
        public PartyTypeMasterRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<PartyTypeMaster>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_PartyTypeMaster",
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new PartyTypeMaster
                {
                    PartyTypeId = SafeGetInt(reader, "PartyTypeId"),
                    PartyTypeCode = SafeGetString(reader, "PartyTypeCode"),
                    PartyTypeName = SafeGetString(reader, "PartyTypeName"),
                    Description = SafeGetString(reader, "Description"),
                    IsCustomer = SafeGetBool(reader, "IsCustomer") ?? false,
                    IsVendor = SafeGetBool(reader, "IsVendor") ?? false,
                    IsActive = SafeGetBool(reader, "IsActive") ?? false,
                    SortOrder = SafeGetInt(reader, "SortOrder"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<PartyTypeMaster?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_PartyTypeMaster",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@PartyTypeId", id);
                },
                reader => new PartyTypeMaster
                {
                    PartyTypeId = SafeGetInt(reader, "PartyTypeId"),
                    PartyTypeCode = SafeGetString(reader, "PartyTypeCode"),
                    PartyTypeName = SafeGetString(reader, "PartyTypeName"),
                    Description = SafeGetString(reader, "Description"),
                    IsCustomer = SafeGetBool(reader, "IsCustomer") ?? false,
                    IsVendor = SafeGetBool(reader, "IsVendor") ?? false,
                    IsActive = SafeGetBool(reader, "IsActive") ?? false,
                    SortOrder = SafeGetInt(reader, "SortOrder"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<List<Party>> GetPartiesByTypeAsync(string partyType)
        {
            return await ExecuteReaderAsync("sp_PartyTypeMaster",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_TYPE");
                    cmd.Parameters.AddWithValue("@PartyType", partyType);
                },
                reader => new Party
                {
                    PartyId = SafeGetInt(reader, "PartyId"),
                    PartyType = SafeGetString(reader, "PartyType"),
                    PartyTypeId = SafeGetInt(reader, "PartyTypeId") == 0 ? (int?)null : SafeGetInt(reader, "PartyTypeId"),
                    Salutation = SafeGetString(reader, "Salutation"),
                    FirstName = SafeGetString(reader, "FirstName"),
                    LastName = SafeGetString(reader, "LastName"),
                    CompanyName = SafeGetString(reader, "CompanyName"),
                    DisplayName = SafeGetString(reader, "DisplayName"),
                    Email = SafeGetString(reader, "Email"),
                    PhoneWork = SafeGetString(reader, "PhoneWork"),
                    PhoneMobile = SafeGetString(reader, "PhoneMobile"),
                    PreferredLanguage = SafeGetString(reader, "PreferredLanguage"),
                    GSTTreatment = SafeGetString(reader, "GSTTreatment"),
                    PlaceOfSupply = SafeGetString(reader, "PlaceOfSupply"),
                    PAN = SafeGetString(reader, "PAN"),
                    MSMERegistered = SafeGetBool(reader, "MSMERegistered"),
                    TaxPreference = SafeGetString(reader, "TaxPreference"),
                    TDS = SafeGetString(reader, "TDS"),
                    AccountsReceivableAccount = SafeGetInt(reader, "AccountsReceivableAccount"),
                    AccountsPayableAccount = SafeGetInt(reader, "AccountsPayableAccount"),
                    OpeningBalance = reader.IsDBNull(reader.GetOrdinal("OpeningBalance")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("OpeningBalance")),
                    OpeningBalanceType = SafeGetString(reader, "OpeningBalanceType"),
                    PaymentTerms = SafeGetString(reader, "PaymentTerms"),
                    EnablePortal = SafeGetBool(reader, "EnablePortal"),
                    WebsiteURL = SafeGetString(reader, "WebsiteURL"),
                    Department = SafeGetString(reader, "Department"),
                    Remarks = SafeGetString(reader, "Remarks"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task InsertAsync(PartyTypeMaster partyTypeMaster)
        {
            await ExecuteNonQueryAsync("sp_PartyTypeMaster", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@PartyTypeCode", partyTypeMaster.PartyTypeCode);
                cmd.Parameters.AddWithValue("@PartyTypeName", partyTypeMaster.PartyTypeName);
                cmd.Parameters.AddWithValue("@Description", partyTypeMaster.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsCustomer", partyTypeMaster.IsCustomer);
                cmd.Parameters.AddWithValue("@IsVendor", partyTypeMaster.IsVendor);
                cmd.Parameters.AddWithValue("@IsActive", partyTypeMaster.IsActive);
                cmd.Parameters.AddWithValue("@SortOrder", partyTypeMaster.SortOrder);
            });
        }

        public async Task UpdateAsync(PartyTypeMaster partyTypeMaster)
        {
            await ExecuteNonQueryAsync("sp_PartyTypeMaster", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@PartyTypeId", partyTypeMaster.PartyTypeId);
                cmd.Parameters.AddWithValue("@PartyTypeCode", partyTypeMaster.PartyTypeCode);
                cmd.Parameters.AddWithValue("@PartyTypeName", partyTypeMaster.PartyTypeName);
                cmd.Parameters.AddWithValue("@Description", partyTypeMaster.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsCustomer", partyTypeMaster.IsCustomer);
                cmd.Parameters.AddWithValue("@IsVendor", partyTypeMaster.IsVendor);
                cmd.Parameters.AddWithValue("@IsActive", partyTypeMaster.IsActive);
                cmd.Parameters.AddWithValue("@SortOrder", partyTypeMaster.SortOrder);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_PartyTypeMaster", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@PartyTypeId", id);
            });
        }
    }
}
