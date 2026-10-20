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
          AddPartyCreateParameters(cmd, party, userId);
        });

        if (partyId != 0) party.PartyId = Convert.ToInt32(partyId);
        await PersistPartyLimitFieldsAsync(party);
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
        await ExecuteNonQueryAsync("sp_party_update", cmd =>
        {
          AddPartyUpdateParameters(cmd, party, userId);
        });

        await PersistPartyLimitFieldsAsync(party);
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

    public async Task<Dictionary<string, PaymentTermLookupDto>> GetPaymentTermLookupAsync(int companyId)
    {
      var rows = await ExecuteReaderAsync(@"
          SELECT term_id::TEXT AS term_id,
                 term_name,
                 number_of_days
          FROM public.tab_payment_term_master
          WHERE company_id = @p_company_id",
        cmd => cmd.Parameters.AddWithValue("p_company_id", companyId),
        reader => new
        {
          TermId = SafeGetString(reader, "term_id"),
          TermName = SafeGetString(reader, "term_name"),
          NumberOfDays = reader["number_of_days"] != DBNull.Value ? Convert.ToInt32(reader["number_of_days"]) : (int?)null
        },
        CommandType.Text);

      var lookup = new Dictionary<string, PaymentTermLookupDto>(StringComparer.OrdinalIgnoreCase);
      foreach (var row in rows)
      {
        var item = new PaymentTermLookupDto
        {
          TermName = row.TermName,
          NumberOfDays = row.NumberOfDays
        };

        if (!string.IsNullOrWhiteSpace(row.TermId))
        {
          lookup[row.TermId] = item;
        }

        if (!string.IsNullOrWhiteSpace(row.TermName) && !lookup.ContainsKey(row.TermName))
        {
          lookup[row.TermName] = item;
        }
      }

      return lookup;
    }

    public async Task<bool> UpdateRateDiffAsync(int companyId, int partyId, decimal rateDiff)
    {
      var updatedPartyId = await ExecuteScalarAsync<int?>(@"
          UPDATE public.tab_party_master
          SET rate_diff = @p_rate_diff,
              modified_on = CURRENT_TIMESTAMP
          WHERE party_id = @p_party_id
            AND company_id = @p_company_id
          RETURNING party_id",
        cmd =>
        {
          cmd.Parameters.AddWithValue("p_company_id", companyId);
          cmd.Parameters.AddWithValue("p_party_id", partyId);
          cmd.Parameters.AddWithValue("p_rate_diff", rateDiff);
        },
        CommandType.Text);

      return updatedPartyId.HasValue && updatedPartyId.Value > 0;
    }

    private void AddPartyCreateParameters(NpgsqlCommand cmd, Party party, int? userId)
    {
      cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
      AddPartySharedParameters(cmd, party, userId);
    }

    private void AddPartyUpdateParameters(NpgsqlCommand cmd, Party party, int? userId)
    {
      cmd.Parameters.AddWithValue("p_party_id", party.PartyId);
      cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
      AddPartySharedParameters(cmd, party, userId);
    }

    // Parameter order must match sp_party_create / sp_party_update signatures exactly.
    private void AddPartySharedParameters(NpgsqlCommand cmd, Party party, int? userId)
    {
      cmd.Parameters.AddWithValue("p_party_type", party.PartyType);
      cmd.Parameters.AddWithValue("p_report_party_type", party.ReportPartyType ?? party.PartyType);
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
      cmd.Parameters.AddWithValue("p_kam_mobile", party.KamMobile ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_recovery_mobile", party.RecoveryMobile ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_sumul_incharge", party.SumulIncharge ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_retailer_code", party.RetailerCode ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_sumul_depo_code", party.SumulDepoCode ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_pan_doc_path", party.PanDocPath ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_aadhaar_doc_path", party.AadhaarDocPath ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_gst_cert_path", party.GstCertPath ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_user_id", (object?)userId ?? (object?)_tenantProvider?.UserId ?? DBNull.Value);
      cmd.Parameters.AddWithValue("p_salesperson_id", party.SalespersonId ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_salesperson_mobile", party.SalespersonMobile ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_kam", party.Kam ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_recovery_agent", party.RecoveryAgent ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_transporter_id", party.TransporterId ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_transporter_mobile", party.TransporterMobile ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_zone_id", party.ZoneId ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_sumul_incharge_mobile", party.SumulInchargeMobile ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_daily_sale_limit", party.DailySaleLimit ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_credit_limit", party.CreditLimit ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("p_is_active", party.IsActive);
    }

    private async Task PersistPartyLimitFieldsAsync(Party party)
    {
      if (party.PartyId <= 0) return;

      await ExecuteNonQueryAsync(@"
          UPDATE public.tab_party_master
          SET daily_sale_limit = @p_daily_sale_limit,
              credit_limit = @p_credit_limit,
              is_active = @p_is_active,
              modified_on = CURRENT_TIMESTAMP
          WHERE party_id = @p_party_id
            AND company_id = @p_company_id",
        cmd =>
        {
          cmd.Parameters.AddWithValue("p_party_id", party.PartyId);
          cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
          cmd.Parameters.AddWithValue("p_daily_sale_limit", party.DailySaleLimit ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_credit_limit", party.CreditLimit ?? (object)DBNull.Value);
          cmd.Parameters.AddWithValue("p_is_active", party.IsActive);
        },
        CommandType.Text);
    }

    private Party MapToParty(Npgsql.NpgsqlDataReader reader)
    {
       return new Party
       {
          PartyId = SafeGetInt(reader, "party_id"),
          PartyType = SafeGetString(reader, "party_type"),
          ReportPartyType = SafeGetString(reader, "report_party_type"),
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
          DailySaleLimit = SafeGetNullableDecimal(reader, "daily_sale_limit"),
          CreditLimit = SafeGetNullableDecimal(reader, "credit_limit"),
          IsActive = SafeGetBool(reader, "is_active", true) ?? true,
          KamMobile = SafeGetString(reader, "kam_mobile"),
          RecoveryMobile = SafeGetString(reader, "recovery_mobile"),
          SumulIncharge = SafeGetString(reader, "sumul_incharge"),
          SumulInchargeMobile = SafeGetString(reader, "sumul_incharge_mobile"),
          RetailerCode = SafeGetString(reader, "retailer_code"),
          SumulDepoCode = SafeGetString(reader, "sumul_depo_code"),
          PanDocPath = SafeGetString(reader, "pan_doc_path"),
          AadhaarDocPath = SafeGetString(reader, "aadhaar_doc_path"),
          GstCertPath = SafeGetString(reader, "gst_cert_path"),
          SalespersonId = SafeGetInt(reader, "salesperson_id"),
          SalespersonMobile = SafeGetString(reader, "salesperson_mobile"),
          Kam = SafeGetString(reader, "kam"),
          RecoveryAgent = SafeGetString(reader, "recovery_agent"),
          TransporterId = SafeGetInt(reader, "transporter_id"),
          TransporterMobile = SafeGetString(reader, "transporter_mobile"),
          ZoneId = SafeGetInt(reader, "zone_id"),
          CreatedOn = SafeGetDateTime(reader, "created_on"),
          ModifiedOn = SafeGetDateTime(reader, "modified_on")
       };
    }
  }
}
