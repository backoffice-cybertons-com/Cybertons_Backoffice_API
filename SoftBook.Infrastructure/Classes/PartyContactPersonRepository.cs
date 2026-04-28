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
    public class PartyContactPersonRepository : BaseDAL, IPartyContactPersonRepository
    {
        public PartyContactPersonRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<PartyContactPerson>> GetAllAsync()
        {
            return await ExecuteReaderAsync("SELECT * FROM tab_party_contact_persons WHERE company_id = @company_id",
                cmd => cmd.Parameters.AddWithValue("company_id", _tenantProvider?.CompanyId ?? 0),
                reader => MapToPartyContactPerson(reader));
        }

        public async Task<PartyContactPerson?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("SELECT * FROM tab_party_contact_persons WHERE contact_person_id = @id AND company_id = @company_id",
                cmd =>
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("company_id", _tenantProvider?.CompanyId ?? 0);
                },
                reader => MapToPartyContactPerson(reader));
        }

        public async Task<List<PartyContactPerson>> GetByPartyIdAsync(int partyId)
        {
            return await ExecuteReaderAsync("sp_party_contact_person_get_by_party_id",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_party_id", partyId);
                    cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                },
                reader => MapToPartyContactPerson(reader));
        }

        public async Task InsertAsync(PartyContactPerson contactPerson)
        {
            var contactId = await ExecuteScalarAsync<int>("sp_party_contact_person_create", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                cmd.Parameters.AddWithValue("p_party_id", contactPerson.PartyId);
                cmd.Parameters.AddWithValue("p_salutation", contactPerson.Salutation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_first_name", contactPerson.FirstName);
                cmd.Parameters.AddWithValue("p_last_name", contactPerson.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_email", contactPerson.EmailAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_work_phone", contactPerson.WorkPhone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_mobile", contactPerson.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_is_primary", contactPerson.IsPrimary ?? (object)DBNull.Value);
            });

            if (contactId != null) contactPerson.ContactPersonId = Convert.ToInt32(contactId);
        }

        public async Task UpdateAsync(PartyContactPerson contactPerson)
        {
            await ExecuteNonQueryAsync("sp_party_contact_person_update", cmd =>
            {
                cmd.Parameters.AddWithValue("p_contact_person_id", contactPerson.ContactPersonId);
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
                cmd.Parameters.AddWithValue("p_party_id", contactPerson.PartyId);
                cmd.Parameters.AddWithValue("p_salutation", contactPerson.Salutation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_first_name", contactPerson.FirstName);
                cmd.Parameters.AddWithValue("p_last_name", contactPerson.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_email", contactPerson.EmailAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_work_phone", contactPerson.WorkPhone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_mobile", contactPerson.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_is_primary", contactPerson.IsPrimary ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_party_contact_person_delete", cmd =>
            {
                cmd.Parameters.AddWithValue("p_contact_person_id", id);
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider?.CompanyId ?? 0);
            });
        }

        private PartyContactPerson MapToPartyContactPerson(Npgsql.NpgsqlDataReader reader)
        {
            return new PartyContactPerson
            {
                ContactPersonId = SafeGetInt(reader, "contact_person_id"),
                PartyId = SafeGetInt(reader, "party_id"),
                Salutation = SafeGetString(reader, "salutation"),
                FirstName = SafeGetString(reader, "first_name"),
                LastName = SafeGetString(reader, "last_name"),
                EmailAddress = SafeGetString(reader, "email"), // assuming 'email' in DB, mapped to EmailAddress in model
                WorkPhone = SafeGetString(reader, "work_phone"),
                Mobile = SafeGetString(reader, "mobile"),
                IsPrimary = SafeGetBool(reader, "is_primary"),
                CreatedOn = SafeGetDateTime(reader, "created_on")
            };
        }
    }
}
