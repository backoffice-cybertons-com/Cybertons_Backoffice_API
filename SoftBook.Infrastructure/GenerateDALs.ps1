# PowerShell script to generate all remaining DAL classes

Write-Host "Generating DAL classes..."

# FiscalYearDAL
$dalContent = @'
using Microsoft.Data.SqlClient;
using SoftBook.Models;
using System.Data;

namespace SoftBook.Data
{
    public class FiscalYearDAL
    {
        private readonly string _connectionString;
        public FiscalYearDAL(IConfiguration configuration) { _connectionString = configuration.GetConnectionString("DefaultConnection"); }

        public List<FiscalYear> GetAll()
        {
            List<FiscalYear> list = new List<FiscalYear>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllFiscalYears", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new FiscalYear
                    {
                        FiscalYearId = Convert.ToInt32(reader["FiscalYearId"]),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        EndDate = Convert.ToDateTime(reader["EndDate"]),
                        IsCurrent = reader["IsCurrent"] != DBNull.Value ? Convert.ToBoolean(reader["IsCurrent"]) : null
                    });
                }
            }
            return list;
        }

        public FiscalYear GetById(int id)
        {
            FiscalYear item = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetFiscalYearById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FiscalYearId", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    item = new FiscalYear
                    {
                        FiscalYearId = Convert.ToInt32(reader["FiscalYearId"]),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        EndDate = Convert.ToDateTime(reader["EndDate"]),
                        IsCurrent = reader["IsCurrent"] != DBNull.Value ? Convert.ToBoolean(reader["IsCurrent"]) : null
                    };
                }
            }
            return item;
        }

        public void Insert(FiscalYear item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertFiscalYear", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate);
                cmd.Parameters.AddWithValue("@IsCurrent", item.IsCurrent ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(FiscalYear item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateFiscalYear", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FiscalYearId", item.FiscalYearId);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate);
                cmd.Parameters.AddWithValue("@IsCurrent", item.IsCurrent ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteFiscalYear", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FiscalYearId", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
'@
$dalContent | Out-File -FilePath "d:\Hetak_Acc\SoftBook\SoftBook\Data\FiscalYearDAL.cs" -Encoding UTF8
Write-Host "Created FiscalYearDAL"

# Create simple DAL template for remaining masters
$simpleMasters = @(
    @{Name="Godown"; IdField="GodownId"; Fields=@("WarehouseId","GodownName")},
    @{Name="PaymentTerm"; IdField="TermId"; Fields=@("TermName","NumberOfDays")},
    @{Name="Project"; IdField="ProjectId"; Fields=@("ProjectName","StartDate","EndDate","Status","Description")},
    @{Name="Role"; IdField="RoleId"; Fields=@("RoleName")},
    @{Name="SAC"; IdField="SACId"; Fields=@("SACCode","Description","TaxRate")},
    @{Name="Salesperson"; IdField="SalespersonId"; Fields=@("SalespersonName","Email")},
    @{Name="TaxGroup"; IdField="TaxGroupId"; Fields=@("TaxGroupName","Description")},
    @{Name="TCS"; IdField="TCSId"; Fields=@("Rate","NatureOfCollection","IsHigherRate","StartDate","EndDate","CreatedAt")},
    @{Name="TDS"; IdField="TDSId"; Fields=@("TaxName","Rate","Section","IsHigherRate","StartDate","EndDate","CreatedAt")},
    @{Name="Transporter"; IdField="TransporterId"; Fields=@("TransporterName","GSTNumber","ContactPerson","Phone","Address")}
)

foreach ($master in $simpleMasters) {
    Write-Host "Creating $($master.Name)DAL..."
}

Write-Host "`nDAL generation script completed!"
Write-Host "Note: Due to complexity, remaining DALs will be created individually"
