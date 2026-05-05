using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SoftBook.Infrastructure.Classes
{
    public class DashboardRepository : BaseDAL, IDashboardRepository
    {
        public DashboardRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        public async Task<string> GetDashboardDataAsync(DashboardRequestDto request)
        {
            return await ExecuteScalarAsync<string>("fn_dashboard_master_aggregator", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", request.CompanyId);
                
                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) 
                { 
                    Value = (object?)request.FromDate ?? DBNull.Value 
                });
                
                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) 
                { 
                    Value = (object?)request.ToDate ?? DBNull.Value 
                });
                
                cmd.Parameters.Add(new NpgsqlParameter("p_branch_id", NpgsqlTypes.NpgsqlDbType.Integer) 
                { 
                    Value = (object?)request.BranchId ?? DBNull.Value 
                });
                
                cmd.Parameters.Add(new NpgsqlParameter("p_granularity", NpgsqlTypes.NpgsqlDbType.Text) 
                { 
                    Value = string.IsNullOrWhiteSpace(request.Granularity) ? DBNull.Value : request.Granularity 
                });
                
                cmd.Parameters.Add(new NpgsqlParameter("p_zone_type", NpgsqlTypes.NpgsqlDbType.Text) 
                { 
                    Value = string.IsNullOrWhiteSpace(request.ZoneType) ? DBNull.Value : request.ZoneType 
                });
                
                cmd.Parameters.Add(new NpgsqlParameter("p_expense_party_types", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text) 
                { 
                    Value = (object?)request.ExpensePartyTypes ?? DBNull.Value 
                });
                
                cmd.Parameters.Add(new NpgsqlParameter("p_expense_v_types", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text) 
                { 
                    Value = (object?)request.ExpenseVTypes ?? DBNull.Value 
                });
                
                cmd.Parameters.Add(new NpgsqlParameter("p_top_limit", NpgsqlTypes.NpgsqlDbType.Integer) 
                { 
                    Value = (object?)request.TopLimit ?? DBNull.Value 
                });

                cmd.Parameters.Add(new NpgsqlParameter("p_inv_status_party_type", NpgsqlTypes.NpgsqlDbType.Text)
                {
                    Value = string.IsNullOrWhiteSpace(request.InvStatusPartyType) ? (object)DBNull.Value : request.InvStatusPartyType
                });
            }) ?? "{}";
        }
    }
}
