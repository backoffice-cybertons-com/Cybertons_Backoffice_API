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
    public class PartyBankDetailsRepository : BaseDAL, IPartyBankDetailsRepository
    {
        public PartyBankDetailsRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<PartyBankDetails>> GetAllAsync()
        {
             // Not commonly used for banks, but let's implement if needed or throw not implemented. 
             // Original implementation had it. I'll use raw query for safety as I didn't add sp_party_bank_details_get_all
             return await ExecuteReaderAsync("SELECT * FROM tab_party_bank_details WHERE company_id = @company_id", 
                cmd => cmd.Parameters.AddWithValue("company_id", _tenantProvider?.CompanyId ?? 0),
                reader => MapToPartyBankDetails(reader));
        }

        public async Task<PartyBankDetails?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("SELECT * FROM tab_party_bank_details WHERE bank_id = @id AND company_id = @company_id",
                cmd =>
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("company_id", _tenantProvider?.CompanyId ?? 0);
                },
                reader => MapToPartyBankDetails(reader));
        }

        public async Task<List<PartyBankDetails>> GetByPartyIdAsync(int partyId)
        {
            return await ExecuteReaderAsync("sp_party_bank_details_get_by_party_id",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_party_id", partyId);
                    cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                },
                reader => MapToPartyBankDetails(reader));
        }

        public async Task InsertAsync(PartyBankDetails bankDetails)
        {
            var bankId = await ExecuteScalarAsync<int>("sp_party_bank_details_create", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                cmd.Parameters.AddWithValue("p_party_id", bankDetails.PartyId);
                cmd.Parameters.AddWithValue("p_account_holder_name", bankDetails.AccountHolderName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_bank_name", bankDetails.BankName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_account_number", bankDetails.AccountNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ifsc", bankDetails.IFSC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_branch", bankDetails.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_micr", bankDetails.MICR ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_is_default", (object)bankDetails.IsDefault ?? DBNull.Value);
            });

            if (bankId != null) bankDetails.BankDetailId = Convert.ToInt32(bankId);
        }

        public async Task UpdateAsync(PartyBankDetails bankDetails)
        {
            await ExecuteNonQueryAsync("sp_party_bank_details_update", cmd =>
            {
                cmd.Parameters.AddWithValue("p_bank_id", bankDetails.BankDetailId);
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                cmd.Parameters.AddWithValue("p_party_id", bankDetails.PartyId);
                cmd.Parameters.AddWithValue("p_account_holder_name", bankDetails.AccountHolderName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_bank_name", bankDetails.BankName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_account_number", bankDetails.AccountNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_ifsc", bankDetails.IFSC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_branch", bankDetails.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_micr", bankDetails.MICR ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_is_default", (object)bankDetails.IsDefault ?? DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_party_bank_details_delete", cmd =>
            {
                cmd.Parameters.AddWithValue("p_bank_id", id);
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
            });
        }

        private PartyBankDetails MapToPartyBankDetails(Npgsql.NpgsqlDataReader reader)
        {
            return new PartyBankDetails
            {
                BankDetailId = SafeGetInt(reader, "bank_id"),
                PartyId = SafeGetInt(reader, "party_id"),
                AccountHolderName = SafeGetString(reader, "account_holder_name"),
                BankName = SafeGetString(reader, "bank_name"),
                AccountNumber = SafeGetString(reader, "account_number"),
                IFSC = SafeGetString(reader, "ifsc"),
                Branch = SafeGetString(reader, "branch"),
                MICR = SafeGetString(reader, "micr"),
                IsDefault = SafeGetBool(reader, "is_default"),
                CreatedOn = SafeGetDateTime(reader, "created_on")
            };
        }
    }
}
