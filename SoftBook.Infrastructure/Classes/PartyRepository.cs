using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
  public class PartyRepository : BaseDAL, IPartyRepository
  {
    public PartyRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

    public async Task<List<Party>> GetAllAsync()
    {
      return await ExecuteReaderAsync("sp_Party_Master", cmd =>
      {
        cmd.Parameters.AddWithValue("@Action", "SELECT");
      }, reader => new Party
                {
                    PartyId = SafeGetInt(reader, "PartyId"),
                    PartyType = SafeGetString(reader, "PartyType"),
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
                    GSTNO = SafeGetString(reader, "GSTNO"),
                    MSMERegistered = SafeGetBool(reader, "MSMERegistered"),
                    TaxPreference = SafeGetString(reader, "TaxPreference"),
                    TDS = SafeGetString(reader, "TDS"),
                    Currency = SafeGetInt(reader, "Currency", SafeGetInt(reader, "CurrencyId")),
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

    public async Task<Party?> GetByIdAsync(int id)
    {
      return await ExecuteSingleReaderAsync("sp_Party_Master",
          cmd =>
          {
            cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
            cmd.Parameters.AddWithValue("@PartyId", id);
          },
          reader => new Party
                {
                    PartyId = SafeGetInt(reader, "PartyId"),
                    PartyType = SafeGetString(reader, "PartyType"),
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
                    GSTNO = SafeGetString(reader, "GSTNO"),
                    MSMERegistered = SafeGetBool(reader, "MSMERegistered"),
                    TaxPreference = SafeGetString(reader, "TaxPreference"),
                    TDS = SafeGetString(reader, "TDS"),
                    Currency = SafeGetInt(reader, "Currency", SafeGetInt(reader, "CurrencyId")),
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

    public async Task InsertAsync(Party party)
    {
      var partyId = await ExecuteSingleReaderAsync("sp_Party_Master", cmd =>
      {
        cmd.Parameters.AddWithValue("@Action", "INSERT");
        cmd.Parameters.AddWithValue("@PartyType", party.PartyType);
        cmd.Parameters.AddWithValue("@Salutation", party.Salutation ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@FirstName", party.FirstName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@LastName", party.LastName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@CompanyName", party.CompanyName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@DisplayName", party.DisplayName);
        cmd.Parameters.AddWithValue("@Email", party.Email ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PhoneWork", party.PhoneWork ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PhoneMobile", party.PhoneMobile ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PreferredLanguage", party.PreferredLanguage ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@GSTTreatment", party.GSTTreatment ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PlaceOfSupply", party.PlaceOfSupply ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PAN", party.PAN ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@GSTNO", party.GSTNO ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@MSMERegistered", party.MSMERegistered ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@TaxPreference", party.TaxPreference ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@TDS", party.TDS ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Currency", party.Currency ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@AccountsReceivableAccount", party.AccountsReceivableAccount ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@AccountsPayableAccount", party.AccountsPayableAccount ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@OpeningBalance", party.OpeningBalance ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@OpeningBalanceType", party.OpeningBalanceType ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PaymentTerms", party.PaymentTerms ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@EnablePortal", party.EnablePortal ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@WebsiteURL", party.WebsiteURL ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Department", party.Department ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Remarks", party.Remarks ?? (object)DBNull.Value);
      }, reader => Convert.ToInt32(reader["PartyId"]));

      party.PartyId = partyId;
    }

    public async Task UpdateAsync(Party party)
    {
      await ExecuteNonQueryAsync("sp_Party_Master", cmd =>
      {
        cmd.Parameters.AddWithValue("@Action", "UPDATE");
        cmd.Parameters.AddWithValue("@PartyId", party.PartyId);
        cmd.Parameters.AddWithValue("@PartyType", party.PartyType);
        cmd.Parameters.AddWithValue("@Salutation", party.Salutation ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@FirstName", party.FirstName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@LastName", party.LastName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@CompanyName", party.CompanyName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@DisplayName", party.DisplayName);
        cmd.Parameters.AddWithValue("@Email", party.Email ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PhoneWork", party.PhoneWork ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PhoneMobile", party.PhoneMobile ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PreferredLanguage", party.PreferredLanguage ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@GSTTreatment", party.GSTTreatment ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PlaceOfSupply", party.PlaceOfSupply ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PAN", party.PAN ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@GSTNO", party.GSTNO ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@MSMERegistered", party.MSMERegistered ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@TaxPreference", party.TaxPreference ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@TDS", party.TDS ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Currency", party.Currency ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@AccountsReceivableAccount", party.AccountsReceivableAccount ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@AccountsPayableAccount", party.AccountsPayableAccount ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@OpeningBalance", party.OpeningBalance ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@OpeningBalanceType", party.OpeningBalanceType ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PaymentTerms", party.PaymentTerms ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@EnablePortal", party.EnablePortal ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@WebsiteURL", party.WebsiteURL ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Department", party.Department ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Remarks", party.Remarks ?? (object)DBNull.Value);
      });
    }

    public async Task DeleteAsync(int id)
    {
      await ExecuteNonQueryAsync("sp_Party_Master", cmd =>
      {
        cmd.Parameters.AddWithValue("@Action", "DELETE");
        cmd.Parameters.AddWithValue("@PartyId", id);
      });
    }

    public async Task<List<Party>> GetByPartyTypeAsync(string partyType)
    {
      return await ExecuteReaderAsync("sp_Party_Master", cmd =>
      {
        cmd.Parameters.AddWithValue("@Action", "SELECT_BY_TYPE");
        cmd.Parameters.AddWithValue("@PartyType", partyType);
      }, reader => new Party
                {
                    PartyId = SafeGetInt(reader, "PartyId"),
                    PartyType = SafeGetString(reader, "PartyType"),
                    Salutation = SafeGetString(reader, "Salutation"),
                    FirstName = SafeGetString(reader, "FirstName"),
                    LastName = SafeGetString(reader, "LastName"),
                    CompanyName = SafeGetString(reader, "CompanyName"),
                    DisplayName = SafeGetString(reader, "DisplayName"),
                    Email = SafeGetString(reader, "Email"),
                    PhoneWork = SafeGetString(reader, "PhoneWork"),
                    PhoneMobile = SafeGetString(reader, "PhoneMobile"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
    }
  }
}
