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
            return await ExecuteReaderAsync("sp_PartyReportingTag_Master",
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new PartyReportingTag
                {
                    PartyReportingTagId = SafeGetInt(reader, "PartyReportingTagId", SafeGetInt(reader, "TagId")),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    ReportingTagId = SafeGetInt(reader, "ReportingTagId", SafeGetInt(reader, "TagId")),
                    TagValue = SafeGetString(reader, "TagValue", SafeGetString(reader, "TagName")),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<PartyReportingTag?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_PartyReportingTag_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@PartyReportingTagId", id);
                },
                reader => new PartyReportingTag
                {
                    PartyReportingTagId = SafeGetInt(reader, "PartyReportingTagId", SafeGetInt(reader, "TagId")),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    ReportingTagId = SafeGetInt(reader, "ReportingTagId", SafeGetInt(reader, "TagId")),
                    TagValue = SafeGetString(reader, "TagValue", SafeGetString(reader, "TagName")),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<List<PartyReportingTag>> GetByPartyIdAsync(int partyId)
        {
            return await ExecuteReaderAsync("sp_PartyReportingTag_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_PARTY_ID");
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                },
                reader => new PartyReportingTag
                {
                    PartyReportingTagId = SafeGetInt(reader, "PartyReportingTagId", SafeGetInt(reader, "TagId")),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    ReportingTagId = SafeGetInt(reader, "ReportingTagId", SafeGetInt(reader, "TagId")),
                    TagValue = SafeGetString(reader, "TagValue", SafeGetString(reader, "TagName")),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task InsertAsync(PartyReportingTag reportingTag)
        {
            await ExecuteNonQueryAsync("sp_PartyReportingTag_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@PartyId", reportingTag.PartyId);
                cmd.Parameters.AddWithValue("@ReportingTagId", reportingTag.ReportingTagId);
                cmd.Parameters.AddWithValue("@TagValue", reportingTag.TagValue ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(PartyReportingTag reportingTag)
        {
            await ExecuteNonQueryAsync("sp_PartyReportingTag_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@PartyReportingTagId", reportingTag.PartyReportingTagId);
                cmd.Parameters.AddWithValue("@PartyId", reportingTag.PartyId);
                cmd.Parameters.AddWithValue("@ReportingTagId", reportingTag.ReportingTagId);
                cmd.Parameters.AddWithValue("@TagValue", reportingTag.TagValue ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_PartyReportingTag_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@PartyReportingTagId", id);
            });
        }
    }
}
