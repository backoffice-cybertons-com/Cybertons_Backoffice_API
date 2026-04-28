using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.DTOs;
using Npgsql;

namespace SoftBook.Infrastructure.Repositories.Classes
{
  public class PartyRepository : BaseDAL, IPartyRepository
  {
    public PartyRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

    public async Task<List<Party>> GetAllAsync()
    {
      return await ExecuteReaderAsync("sp_party_get_all", cmd =>
      {
        cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
      }, reader => MapToParty(reader));
    }

    public async Task<Party?> GetByIdAsync(int id)
    {
      return await ExecuteSingleReaderAsync("sp_party_get_by_id",
          cmd =>
          {
            cmd.Parameters.AddWithValue("p_party_id", id);
            cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
          },
          reader => MapToParty(reader));
    }

    public async Task<DbResult> InsertAsync(Party party, int? userId = null)
    {
      try
      {
        var partyId = await ExecuteScalarAsync<int>("sp_party_create", cmd =>
        {
          cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
          cmd.Parameters.AddWithValue("p_party_type", party.PartyType);
          cmd.Parameters.AddWithValue("p_salutation", party.Salutation ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_first_name", party.FirstName ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_last_name", party.LastName ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_company_name", party.CompanyName ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_display_name", party.DisplayName);
          cmd.Parameters.AddWithValue("p_email", party.Email ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_phone_work", party.PhoneWork ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_phone_mobile", party.PhoneMobile ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_preferred_language", party.PreferredLanguage ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_gst_treatment", party.GSTTreatment ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_place_of_supply", party.PlaceOfSupply ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_pan", party.PAN ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_gst_no", party.GSTNO ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_msme_registered", party.MSMERegistered ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_tax_preference", party.TaxPreference ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_tds", party.TDS ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_currency_id", party.Currency ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_accounts_receivable_account", party.AccountsReceivableAccount ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_accounts_payable_account", party.AccountsPayableAccount ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_opening_balance", party.OpeningBalance ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_opening_balance_type", party.OpeningBalanceType ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_payment_terms", party.PaymentTerms ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_enable_portal", party.EnablePortal ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_website_url", party.WebsiteURL ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_department", party.Department ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_remarks", party.Remarks ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_party_profile", party.PartyProfile ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_rate_diff", party.RateDiff ?? 0);
          cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
        });

        if (partyId != 0) party.PartyId = Convert.ToInt32(partyId);
        return DbResult.Ok(party);
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    public async Task<DbResult> UpdateAsync(Party party, int? userId = null)
    {
      try
      {
        await ExecuteSingleReaderAsync("sp_party_update", cmd =>
        {
          cmd.Parameters.AddWithValue("p_party_id", party.PartyId);
          cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
          cmd.Parameters.AddWithValue("p_party_type", party.PartyType);
          cmd.Parameters.AddWithValue("p_salutation", party.Salutation ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_first_name", party.FirstName ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_last_name", party.LastName ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_company_name", party.CompanyName ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_display_name", party.DisplayName);
          cmd.Parameters.AddWithValue("p_email", party.Email ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_phone_work", party.PhoneWork ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_phone_mobile", party.PhoneMobile ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_preferred_language", party.PreferredLanguage ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_gst_treatment", party.GSTTreatment ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_place_of_supply", party.PlaceOfSupply ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_pan", party.PAN ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_gst_no", party.GSTNO ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_msme_registered", party.MSMERegistered ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_tax_preference", party.TaxPreference ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_tds", party.TDS ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_currency_id", party.Currency ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_accounts_receivable_account", party.AccountsReceivableAccount ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_accounts_payable_account", party.AccountsPayableAccount ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_opening_balance", party.OpeningBalance ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_opening_balance_type", party.OpeningBalanceType ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_payment_terms", party.PaymentTerms ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_enable_portal", party.EnablePortal ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_website_url", party.WebsiteURL ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_department", party.Department ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_remarks", party.Remarks ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_party_profile", party.PartyProfile ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_rate_diff", party.RateDiff ?? 0);
          cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
        }, reader => reader[0]);
        return DbResult.Ok(party);
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    public async Task<DbResult> DeleteAsync(int id, int? userId = null)
    {
      try
      {
        await ExecuteSingleReaderAsync("sp_party_delete", cmd =>
        {
          cmd.Parameters.AddWithValue("p_party_id", id);
          cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
          cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
        }, reader => reader[0]);
        return DbResult.Ok();
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    public async Task<List<Party>> GetByPartyTypeAsync(string partyType)
    {
      return await ExecuteReaderAsync("sp_party_get_by_type", cmd =>
      {
        cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
        cmd.Parameters.AddWithValue("p_party_type", partyType);
      }, reader => MapToParty(reader));
    }

    private Party MapToParty(Npgsql.NpgsqlDataReader reader)
    {
       return new Party
       {
          PartyId = SafeGetInt(reader, "party_id"),
          PartyType = SafeGetString(reader, "party_type"),
          Salutation = SafeGetString(reader, "salutation"),
          FirstName = SafeGetString(reader, "first_name"),
          LastName = SafeGetString(reader, "last_name"),
          CompanyName = SafeGetString(reader, "company_name"),
          DisplayName = SafeGetString(reader, "display_name"),
          Email = SafeGetString(reader, "email"),
          PhoneWork = SafeGetString(reader, "phone_work"),
          PhoneMobile = SafeGetString(reader, "phone_mobile"),
          PreferredLanguage = SafeGetString(reader, "preferred_language"),
          GSTTreatment = SafeGetString(reader, "gst_treatment"),
          PlaceOfSupply = SafeGetString(reader, "place_of_supply"),
          PAN = SafeGetString(reader, "pan"),
          GSTNO = SafeGetString(reader, "gst_no"),
          MSMERegistered = SafeGetBool(reader, "msme_registered"),
          TaxPreference = SafeGetString(reader, "tax_preference"),
          TDS = SafeGetString(reader, "tds"),
          Currency = SafeGetInt(reader, "currency_id"), 
          AccountsReceivableAccount = SafeGetInt(reader, "accounts_receivable_account"),
          AccountsPayableAccount = SafeGetInt(reader, "accounts_payable_account"),
          OpeningBalance = reader["opening_balance"] != DBNull.Value ? Convert.ToDecimal(reader["opening_balance"]) : null,
          OpeningBalanceType = SafeGetString(reader, "opening_balance_type"),
          PaymentTerms = SafeGetString(reader, "payment_terms"),
          EnablePortal = SafeGetBool(reader, "enable_portal"),
          WebsiteURL = SafeGetString(reader, "website_url"),
          Department = SafeGetString(reader, "department"),
          Remarks = SafeGetString(reader, "remarks"),
          PartyProfile = SafeGetString(reader, "party_profile"),
          RateDiff = reader["rate_diff"] != DBNull.Value ? Convert.ToDecimal(reader["rate_diff"]) : null,
          CreatedOn = SafeGetDateTime(reader, "created_on")
       };
    }
  }
}
