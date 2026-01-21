using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class PartyContactPersonRepository : BaseDAL, IPartyContactPersonRepository
    {
        public PartyContactPersonRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<PartyContactPerson>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_PartyContactPerson_Master",
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new PartyContactPerson
                {
                    ContactPersonId = SafeGetInt(reader, "ContactPersonId"),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    Salutation = SafeGetString(reader, "Salutation"),
                    FirstName = SafeGetString(reader, "FirstName"),
                    LastName = SafeGetString(reader, "LastName"),
                    EmailAddress = SafeGetString(reader, "EmailAddress", SafeGetString(reader, "Email")),
                    WorkPhone = SafeGetString(reader, "WorkPhone"),
                    Mobile = SafeGetString(reader, "Mobile"),
                    IsPrimary = SafeGetBool(reader, "IsPrimary"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<PartyContactPerson?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_PartyContactPerson_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@ContactPersonId", id);
                },
                reader => new PartyContactPerson
                {
                    ContactPersonId = SafeGetInt(reader, "ContactPersonId"),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    Salutation = SafeGetString(reader, "Salutation"),
                    FirstName = SafeGetString(reader, "FirstName"),
                    LastName = SafeGetString(reader, "LastName"),
                    EmailAddress = SafeGetString(reader, "EmailAddress", SafeGetString(reader, "Email")),
                    WorkPhone = SafeGetString(reader, "WorkPhone"),
                    Mobile = SafeGetString(reader, "Mobile"),
                    IsPrimary = SafeGetBool(reader, "IsPrimary"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task<List<PartyContactPerson>> GetByPartyIdAsync(int partyId)
        {
            return await ExecuteReaderAsync("sp_PartyContactPerson_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_PARTY_ID");
                    cmd.Parameters.AddWithValue("@PartyId", partyId);
                },
                reader => new PartyContactPerson
                {
                    ContactPersonId = SafeGetInt(reader, "ContactPersonId"),
                    PartyId = SafeGetInt(reader, "PartyId"),
                    Salutation = SafeGetString(reader, "Salutation"),
                    FirstName = SafeGetString(reader, "FirstName"),
                    LastName = SafeGetString(reader, "LastName"),
                    EmailAddress = SafeGetString(reader, "EmailAddress", SafeGetString(reader, "Email")),
                    WorkPhone = SafeGetString(reader, "WorkPhone"),
                    Mobile = SafeGetString(reader, "Mobile"),
                    IsPrimary = SafeGetBool(reader, "IsPrimary"),
                    CreatedOn = SafeGetDateTime(reader, "CreatedOn")
                });
        }

        public async Task InsertAsync(PartyContactPerson contactPerson)
        {
            await ExecuteNonQueryAsync("sp_PartyContactPerson_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@PartyId", contactPerson.PartyId);
                cmd.Parameters.AddWithValue("@Salutation", contactPerson.Salutation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FirstName", contactPerson.FirstName);
                cmd.Parameters.AddWithValue("@LastName", contactPerson.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EmailAddress", contactPerson.EmailAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@WorkPhone", contactPerson.WorkPhone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Mobile", contactPerson.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPrimary", contactPerson.IsPrimary ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(PartyContactPerson contactPerson)
        {
            await ExecuteNonQueryAsync("sp_PartyContactPerson_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@ContactPersonId", contactPerson.ContactPersonId);
                cmd.Parameters.AddWithValue("@PartyId", contactPerson.PartyId);
                cmd.Parameters.AddWithValue("@Salutation", contactPerson.Salutation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FirstName", contactPerson.FirstName);
                cmd.Parameters.AddWithValue("@LastName", contactPerson.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EmailAddress", contactPerson.EmailAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@WorkPhone", contactPerson.WorkPhone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Mobile", contactPerson.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPrimary", contactPerson.IsPrimary ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_PartyContactPerson_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ContactPersonId", id);
            });
        }
    }
}
