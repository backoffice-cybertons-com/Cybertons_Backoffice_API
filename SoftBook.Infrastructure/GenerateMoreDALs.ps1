# PowerShell script to create all remaining DAL classes in one go

$dalTemplates = @{
    "ProjectDAL" = @'
using Microsoft.Data.SqlClient;
using SoftBook.Models;
using System.Data;

namespace SoftBook.Data
{
    public class ProjectDAL
    {
        private readonly string _connectionString;
        public ProjectDAL(IConfiguration configuration) { _connectionString = configuration.GetConnectionString("DefaultConnection"); }

        public List<Project> GetAll()
        {
            List<Project> list = new List<Project>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllProjects", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) list.Add(new Project { ProjectId = Convert.ToInt32(reader["ProjectId"]), ProjectName = reader["ProjectName"]?.ToString() ?? string.Empty, StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null, EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null, Status = reader["Status"]?.ToString(), Description = reader["Description"]?.ToString() });
            }
            return list;
        }

        public Project GetById(int id)
        {
            Project item = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetProjectById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectId", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) item = new Project { ProjectId = Convert.ToInt32(reader["ProjectId"]), ProjectName = reader["ProjectName"]?.ToString() ?? string.Empty, StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null, EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null, Status = reader["Status"]?.ToString(), Description = reader["Description"]?.ToString() };
            }
            return item;
        }

        public void Insert(Project item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertProject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectName", item.ProjectName);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", item.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Project item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateProject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectId", item.ProjectId);
                cmd.Parameters.AddWithValue("@ProjectName", item.ProjectName);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", item.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteProject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectId", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
'@

    "RoleDAL" = @'
using Microsoft.Data.SqlClient;
using SoftBook.Models;
using System.Data;

namespace SoftBook.Data
{
    public class RoleDAL
    {
        private readonly string _connectionString;
        public RoleDAL(IConfiguration configuration) { _connectionString = configuration.GetConnectionString("DefaultConnection"); }

        public List<Role> GetAll()
        {
            List<Role> list = new List<Role>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllRoles", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) list.Add(new Role { RoleId = Convert.ToInt32(reader["RoleId"]), RoleName = reader["RoleName"]?.ToString() });
            }
            return list;
        }

        public Role GetById(int id)
        {
            Role item = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetRoleById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleId", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) item = new Role { RoleId = Convert.ToInt32(reader["RoleId"]), RoleName = reader["RoleName"]?.ToString() };
            }
            return item;
        }

        public void Insert(Role item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertRole", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleName", item.RoleName ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Role item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateRole", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleId", item.RoleId);
                cmd.Parameters.AddWithValue("@RoleName", item.RoleName ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteRole", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleId", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
'@
}

foreach ($dalName in $dalTemplates.Keys) {
    $dalTemplates[$dalName] | Out-File -FilePath "d:\Hetak_Acc\SoftBook\SoftBook\Data\$dalName.cs" -Encoding UTF8
    Write-Host "Created $dalName.cs"
}

Write-Host "`nDAL classes created successfully!"
