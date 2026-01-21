using SoftBook.Infrastructure.Interfaces;
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
            return await ExecuteReaderAsync("sp_PartyBankDetails_Master",
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new PartyBankDetails
                {
                    BankDetailId = SafeGetInt(reader, "BankDetailId", SafeGetInt(reader, "BankId")),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    AccountHolderName = SafeGetString(reader, "AccountHolderName"),
                    BankName = SafeGetString(reader, "BankName"),
                    AccountNumber = SafeGetString(reader, "AccountNumber"),
                    IFSC = SafeGetString(reader, "IFSC"),
                    Branch = SafeGetString(reader, "Branch"),
                    MICR = SafeGetString(reader, "MICR"),
                    IsDefault = SafeGetBool(reader, "IsDefault"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<PartyBankDetails?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_PartyBankDetails_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@BankDetailId", id);
                },
                reader => new PartyBankDetails
                {
                    BankDetailId = SafeGetInt(reader, "BankDetailId", SafeGetInt(reader, "BankId")),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    AccountHolderName = SafeGetString(reader, "AccountHolderName"),
                    BankName = SafeGetString(reader, "BankName"),
                    AccountNumber = SafeGetString(reader, "AccountNumber"),
                    IFSC = SafeGetString(reader, "IFSC"),
                    Branch = SafeGetString(reader, "Branch"),
                    MICR = SafeGetString(reader, "MICR"),
                    IsDefault = SafeGetBool(reader, "IsDefault"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<List<PartyBankDetails>> GetByPartyIdAsync(int partyId)
        {
            return await ExecuteReaderAsync("sp_PartyBankDetails_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_PARTY_ID");
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                },
                reader => new PartyBankDetails
                {
                    BankDetailId = SafeGetInt(reader, "BankDetailId", SafeGetInt(reader, "BankId")),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    AccountHolderName = SafeGetString(reader, "AccountHolderName"),
                    BankName = SafeGetString(reader, "BankName"),
                    AccountNumber = SafeGetString(reader, "AccountNumber"),
                    IFSC = SafeGetString(reader, "IFSC"),
                    Branch = SafeGetString(reader, "Branch"),
                    MICR = SafeGetString(reader, "MICR"),
                    IsDefault = SafeGetBool(reader, "IsDefault"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task InsertAsync(PartyBankDetails bankDetails)
        {
            await ExecuteNonQueryAsync("sp_PartyBankDetails_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@PartyId", bankDetails.PartyId);
                cmd.Parameters.AddWithValue("@AccountHolderName", bankDetails.AccountHolderName);
                cmd.Parameters.AddWithValue("@BankName", bankDetails.BankName);
                cmd.Parameters.AddWithValue("@AccountNumber", bankDetails.AccountNumber);
                cmd.Parameters.AddWithValue("@IFSC", bankDetails.IFSC);
                cmd.Parameters.AddWithValue("@Branch", bankDetails.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MICR", bankDetails.MICR ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsDefault", bankDetails.IsDefault ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(PartyBankDetails bankDetails)
        {
            await ExecuteNonQueryAsync("sp_PartyBankDetails_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@BankDetailId", bankDetails.BankDetailId);
                cmd.Parameters.AddWithValue("@PartyId", bankDetails.PartyId);
                cmd.Parameters.AddWithValue("@AccountHolderName", bankDetails.AccountHolderName);
                cmd.Parameters.AddWithValue("@BankName", bankDetails.BankName);
                cmd.Parameters.AddWithValue("@AccountNumber", bankDetails.AccountNumber);
                cmd.Parameters.AddWithValue("@IFSC", bankDetails.IFSC);
                cmd.Parameters.AddWithValue("@Branch", bankDetails.Branch ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MICR", bankDetails.MICR ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsDefault", bankDetails.IsDefault ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_PartyBankDetails_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@BankDetailId", id);
            });
        }
    }
}
