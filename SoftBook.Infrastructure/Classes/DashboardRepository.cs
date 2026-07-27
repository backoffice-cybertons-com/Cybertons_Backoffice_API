using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Helpers;
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

        public async Task<string> GetGoodsSummaryAsync(DashboardRequestDto request)
        {
            const string sql = "SELECT public.fn_get_goods_summary(@p_company_id, @p_from_date, @p_to_date, @p_fiscal_year_id);";

            return await ExecuteScalarAsync<string>(sql, cmd =>
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

                cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
                });
            }, CommandType.Text)
                ?? "{\"goods_summary\":{\"total_purchase_qty\":0,\"total_sales_qty\":0},\"item_wise_goods\":[]}";
        }

        public async Task<string> GetMastersCreatedAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            const string sql = "SELECT public.fn_get_masters_created(@p_company_id, @p_from_date, @p_to_date);";

            return await ExecuteScalarAsync<string>(sql, cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", companyId);
                cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date)
                {
                    Value = fromDate.Date
                });
                cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date)
                {
                    Value = toDate.Date
                });
            }, CommandType.Text)
                ?? "{\"product_count\":0,\"party_count\":0,\"products\":[],\"parties\":[]}";
        }

        public async Task<string> GetTransactionSummaryAsync(DashboardRequestDto request)
        {
            const string sql = "SELECT public.fn_get_transaction_summary(@p_company_id, @p_from_date, @p_to_date, @p_fiscal_year_id);";

            return await ExecuteScalarAsync<string>(sql, cmd =>
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
                cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
                });
            }, CommandType.Text)
                ?? "{\"total_purchase\":0,\"total_sales\":0,\"total_income\":0,\"total_expense\":0,\"total_jv\":0}";
        }

        public async Task<string> GetSalesProfitSummaryAsync(DashboardRequestDto request)
        {
            const string sql = "SELECT public.fn_get_sales_profit_summary(@p_company_id, @p_from_date, @p_to_date, @p_fiscal_year_id);";

            return await ExecuteScalarAsync<string>(sql, cmd =>
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
                cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
                });
            }, CommandType.Text)
                ?? "{\"total_sale_at_sale_price\":0,\"total_sale_at_cog\":0,\"gross_profit\":0}";
        }

        public async Task<string> GetOutstandingNoInvoiceAsync(DashboardRequestDto request)
        {
            var asOfDate = request.ToDate?.Date.AddDays(1) ?? DashboardDateHelper.GetBusinessToday().AddDays(1);

            const string sql = "SELECT public.fn_get_outstanding_no_invoice(@p_company_id, @p_from_date, @p_to_date, @p_as_of_date, @p_fiscal_year_id);";

            return await ExecuteScalarAsync<string>(sql, cmd =>
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
                cmd.Parameters.Add(new NpgsqlParameter("p_as_of_date", NpgsqlTypes.NpgsqlDbType.Date)
                {
                    Value = asOfDate
                });
                cmd.Parameters.Add(new NpgsqlParameter("p_fiscal_year_id", NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Value = (object?)_tenantProvider?.FiscalYearId ?? DBNull.Value
                });
            }, CommandType.Text)
                ?? "{\"party_count\":0,\"total_outstanding\":0,\"parties\":[]}";
        }
    }
}
