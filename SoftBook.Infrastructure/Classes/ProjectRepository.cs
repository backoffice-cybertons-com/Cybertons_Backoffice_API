using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class ProjectRepository : BaseDAL, IProjectRepository
    {
        public ProjectRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Project>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Project_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Project
            {
                ProjectId = Convert.ToInt32(reader["ProjectId"]),
                ProjectName = reader["ProjectName"]?.ToString() ?? string.Empty,
                StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null,
                Status = reader["Status"]?.ToString(),
                Description = reader["Description"]?.ToString()
            });
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Project_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@ProjectId", id);
                },
                reader => new Project
                {
                    ProjectId = Convert.ToInt32(reader["ProjectId"]),
                    ProjectName = reader["ProjectName"]?.ToString() ?? string.Empty,
                    StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                    EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null,
                    Status = reader["Status"]?.ToString(),
                    Description = reader["Description"]?.ToString()
                });
        }

        public async Task InsertAsync(Project item)
        {
            await ExecuteNonQueryAsync("sp_Project_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@ProjectName", item.ProjectName);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", item.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Project item)
        {
            await ExecuteNonQueryAsync("sp_Project_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@ProjectId", item.ProjectId);
                cmd.Parameters.AddWithValue("@ProjectName", item.ProjectName);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", item.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Project_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ProjectId", id);
            });
        }
    }
}
