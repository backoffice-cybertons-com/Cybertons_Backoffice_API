using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.DTOs;
using Npgsql;
using System.Data;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class CompanyProfileRepository : BaseDAL, ICompanyProfileRepository
    {
        public CompanyProfileRepository(IConfiguration configuration, ITenantProvider tenantProvider) 
            : base(configuration, tenantProvider)
        {
        }

        public async Task<CompanyProfile?> GetByCompanyIdAsync(int companyId)
        {
            return await ExecuteSingleReaderAsync("public.fn_company_profile", cmd =>
            {
                cmd.Parameters.AddWithValue("p_action", "GETBYID");
                cmd.Parameters.AddWithValue("p_company_id", companyId);
            }, MapCompanyProfile);
        }

        public async Task<DbResult> UpsertAsync(CompanyProfile profile)
        {
            try
            {
                var result = await ExecuteSingleReaderAsync("public.fn_company_profile", cmd =>
                {
                    Console.WriteLine($"[DEBUG] Upserting Company Profile: ID={profile.CompanyId}, BankName='{profile.BankName}', Branch='{profile.BankBranch}', Acc='{profile.AccountNumber}', IFSC='{profile.IfscCode}'");
                    
                    cmd.Parameters.AddWithValue("p_action", "UPSERT");
                    cmd.Parameters.AddWithValue("p_company_id", profile.CompanyId);
                    cmd.Parameters.AddWithValue("p_company_name", (object?)profile.CompanyName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_company_address", (object?)profile.CompanyAddress ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_mobile", (object?)profile.Mobile ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_phone", (object?)profile.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_email", (object?)profile.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_gstin", (object?)profile.Gstin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_pan", (object?)profile.Pan ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_state", (object?)profile.State ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_bank_name", (object?)profile.BankName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_bank_branch", (object?)profile.BankBranch ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_account_number", (object?)profile.AccountNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_ifsc_code", (object?)profile.IfscCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_logo_path", (object?)profile.LogoPath ?? DBNull.Value);
                }, MapCompanyProfile);
                return DbResult.Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private CompanyProfile MapCompanyProfile(NpgsqlDataReader reader)
        {
            return new CompanyProfile
            {
                CompanyId = SafeGetInt(reader, "company_id"),
                CompanyName = SafeGetString(reader, "company_name"),
                CompanyAddress = SafeGetString(reader, "company_address"),
                Mobile = SafeGetString(reader, "mobile"),
                Phone = SafeGetString(reader, "phone"),
                Email = SafeGetString(reader, "email"),
                Gstin = SafeGetString(reader, "gstin"),
                Pan = SafeGetString(reader, "pan"),
                State = SafeGetString(reader, "state"),
                BankName = SafeGetString(reader, "bank_name"),
                BankBranch = SafeGetString(reader, "bank_branch"),
                AccountNumber = SafeGetString(reader, "account_number"),
                IfscCode = SafeGetString(reader, "ifsc_code"),
                LogoPath = SafeGetString(reader, "logo_path"),
                CreatedAt = SafeGetDateTime(reader, "created_at"),
                UpdatedAt = SafeGetDateTime(reader, "updated_at")
            };
        }
    }
}
