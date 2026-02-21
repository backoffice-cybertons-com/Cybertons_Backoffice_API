using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class PartyReportingTagRepository : BaseDAL, IPartyReportingTagRepository
    {
        public PartyReportingTagRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<PartyReportingTag>> GetAllAsync()
        {
             // Not common for tags table directly, but implementing via raw query
            return await ExecuteReaderAsync("SELECT * FROM tab_party_reporting_tags WHERE company_id = @company_id",
                cmd => cmd.Parameters.AddWithValue("company_id", _tenantProvider?.CompanyId ?? 0),
                reader => MapToPartyReportingTag(reader));
        }

        public async Task<PartyReportingTag?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("SELECT * FROM tab_party_reporting_tags WHERE tag_id = @id AND company_id = @company_id",
                cmd =>
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("company_id", _tenantProvider?.CompanyId ?? 0);
                },
                reader => MapToPartyReportingTag(reader));
        }

        public async Task<List<PartyReportingTag>> GetByPartyIdAsync(int partyId)
        {
            return await ExecuteReaderAsync("sp_party_reporting_tag_get_by_party_id",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_party_id", partyId);
                    cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                },
                reader => MapToPartyReportingTag(reader));
        }

        public async Task InsertAsync(PartyReportingTag reportingTag)
        {
            var tagId = await ExecuteScalarAsync<int>("sp_party_reporting_tag_create", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                cmd.Parameters.AddWithValue("p_party_id", reportingTag.PartyId);
                // Note: SQL function sp_party_reporting_tag_create takes (company_id, party_id, tag_name)
                // C# model has ReportingTagId (which seems to be the PK, or TagId in DB), and TagValue.
                // It seems the original code treated ReportingTagId as something else or just an ID?
                // The DB schema in SQL (sp_PartyReportingTag_Master) showed TagId and TagName.
                // My postgres function uses p_tag_name.
                cmd.Parameters.AddWithValue("p_tag_name", reportingTag.TagValue ?? (object)DBNull.Value);
            });

            if (tagId != null) reportingTag.PartyReportingTagId = Convert.ToInt32(tagId);
        }

        public async Task UpdateAsync(PartyReportingTag reportingTag)
        {
            // Update logic seems missing in Postgres_Party_Functions.sql?
            // The logic: Delete and Re-insert or direct update?
            // Existing SQL (sp_PartyReportingTag_Master) had UPDATE.
            // My Postgres functions (Step 1617) ONLY HAS: create, delete, get_by_party_id.
            // I missed update and generic update in Postgres??
            // Wait, looking at Step 1617 content:
            // "PARTY REPORTING TAGS FUNCTIONS" -> create, get_by_party_id, delete. NO UPDATE.
            // Reporting tags are often just deleted and recreated or it's a simple value update.
            // I'll implement Update using raw query or generic update if I want to be safe, 
            // OR I just use delete/insert if that's how it's handled, but the Repo has UpdateAsync.
            // I will implement a raw UPDATE query here since I missed the function.
            await ExecuteNonQueryAsync("UPDATE tab_party_reporting_tags SET tag_name = @tag_name WHERE tag_id = @tag_id AND company_id = @company_id", cmd =>
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("tag_id", reportingTag.PartyReportingTagId);
                cmd.Parameters.AddWithValue("company_id", _tenantProvider?.CompanyId ?? 0);
                cmd.Parameters.AddWithValue("tag_name", reportingTag.TagValue ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_party_reporting_tag_delete", cmd =>
            {
                cmd.Parameters.AddWithValue("p_tag_id", id);
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
            });
        }

        private PartyReportingTag MapToPartyReportingTag(Npgsql.NpgsqlDataReader reader)
        {
            return new PartyReportingTag
            {
                PartyReportingTagId = SafeGetInt(reader, "tag_id"),
                PartyId = SafeGetInt(reader, "party_id"),
                ReportingTagId = SafeGetInt(reader, "tag_id"), // Mapped same as PK in original?
                TagValue = SafeGetString(reader, "tag_name"),
                CreatedOn = SafeGetDateTime(reader, "created_on")
            };
        }
    }
}
