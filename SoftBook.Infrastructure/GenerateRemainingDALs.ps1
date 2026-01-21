# PowerShell script to generate remaining DAL classes using BaseDAL pattern

Write-Host "Generating remaining refactored DAL classes..."

# Party DAL
$partyDAL = @'
using Microsoft.Data.SqlClient;
using SoftBook.Models;

namespace SoftBook.Data
{
    public class PartyDAL : BaseDAL
    {
        public PartyDAL(IConfiguration configuration) : base(configuration) { }

        public List<Party> GetAll()
        {
            return ExecuteReader("sp_GetAllParties", null, reader => new Party
            {
                PartyId = Convert.ToInt32(reader["PartyId"]),
                PartyName = reader["PartyName"]?.ToString(),
                PartyType = reader["PartyType"]?.ToString(),
                GSTNumber = reader["GSTNumber"]?.ToString(),
                PANNumber = reader["PANNumber"]?.ToString(),
                ContactPerson = reader["ContactPerson"]?.ToString(),
                Phone = reader["Phone"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                Address = reader["Address"]?.ToString(),
                City = reader["City"]?.ToString(),
                State = reader["State"]?.ToString(),
                PinCode = reader["PinCode"]?.ToString(),
                OpeningBalance = reader["OpeningBalance"] != DBNull.Value ? Convert.ToDecimal(reader["OpeningBalance"]) : null,
                CreditLimit = reader["CreditLimit"] != DBNull.Value ? Convert.ToDecimal(reader["CreditLimit"]) : null,
                CreditDays = reader["CreditDays"] != DBNull.Value ? Convert.ToInt32(reader["CreditDays"]) : null
            });
        }

        public Party GetById(int id)
        {
            return ExecuteSingleReader("sp_GetPartyById",
                cmd => cmd.Parameters.AddWithValue("@PartyId", id),
                reader => new Party
                {
                    PartyId = Convert.ToInt32(reader["PartyId"]),
                    PartyName = reader["PartyName"]?.ToString(),
                    PartyType = reader["PartyType"]?.ToString(),
                    GSTNumber = reader["GSTNumber"]?.ToString(),
                    PANNumber = reader["PANNumber"]?.ToString(),
                    ContactPerson = reader["ContactPerson"]?.ToString(),
                    Phone = reader["Phone"]?.ToString(),
                    Email = reader["Email"]?.ToString(),
                    Address = reader["Address"]?.ToString(),
                    City = reader["City"]?.ToString(),
                    State = reader["State"]?.ToString(),
                    PinCode = reader["PinCode"]?.ToString(),
                    OpeningBalance = reader["OpeningBalance"] != DBNull.Value ? Convert.ToDecimal(reader["OpeningBalance"]) : null,
                    CreditLimit = reader["CreditLimit"] != DBNull.Value ? Convert.ToDecimal(reader["CreditLimit"]) : null,
                    CreditDays = reader["CreditDays"] != DBNull.Value ? Convert.ToInt32(reader["CreditDays"]) : null
                });
        }

        public void Insert(Party party)
        {
            ExecuteNonQuery("sp_InsertParty", cmd =>
            {
                cmd.Parameters.AddWithValue("@PartyName", party.PartyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PartyType", party.PartyType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GSTNumber", party.GSTNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PANNumber", party.PANNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactPerson", party.ContactPerson ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", party.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", party.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", party.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", party.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", party.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PinCode", party.PinCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OpeningBalance", party.OpeningBalance ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreditLimit", party.CreditLimit ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreditDays", party.CreditDays ?? (object)DBNull.Value);
            });
        }

        public void Update(Party party)
        {
            ExecuteNonQuery("sp_UpdateParty", cmd =>
            {
                cmd.Parameters.AddWithValue("@PartyId", party.PartyId);
                cmd.Parameters.AddWithValue("@PartyName", party.PartyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PartyType", party.PartyType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GSTNumber", party.GSTNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PANNumber", party.PANNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactPerson", party.ContactPerson ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", party.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", party.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", party.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", party.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", party.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PinCode", party.PinCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OpeningBalance", party.OpeningBalance ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreditLimit", party.CreditLimit ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreditDays", party.CreditDays ?? (object)DBNull.Value);
            });
        }

        public void Delete(int id)
        {
            ExecuteNonQuery("sp_DeleteParty", cmd =>
            {
                cmd.Parameters.AddWithValue("@PartyId", id);
            });
        }
    }
}
'@

$partyDAL | Out-File -FilePath "d:\Hetak_Acc\SoftBook\SoftBook\Data\PartyDAL.cs" -Encoding UTF8
Write-Host "Created PartyDAL.cs"

Write-Host "`nPartyDAL created successfully!"
'@

$scriptContent | Out-File -FilePath "d:\Hetak_Acc\SoftBook\SoftBook\Data\GenerateRemainingDALs.ps1" -Encoding UTF8
Write-Host "Script created"
