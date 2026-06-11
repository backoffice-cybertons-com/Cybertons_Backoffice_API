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
            const string sql = @"
                SELECT json_build_object(
                    'goods_summary', (
                        SELECT json_build_object(
                            'total_purchase_qty', COALESCE(SUM(
                                CASE WHEN LOWER(TRIM(h.inv_type)) = 'purchase' THEN COALESCE(t.quantity, 0) ELSE 0 END
                            ), 0),
                            'total_sales_qty', COALESCE(SUM(
                                CASE WHEN LOWER(TRIM(h.inv_type)) = 'sales' THEN COALESCE(t.quantity, 0) ELSE 0 END
                            ), 0)
                        )
                        FROM public.tab_sal_pur_tran t
                        INNER JOIN public.tab_sale_pur h
                            ON t.sp_unique_code = h.sp_unique_code
                        WHERE h.company_id = @p_company_id
                          AND COALESCE(h.delete_flag, FALSE) = FALSE
                          AND COALESCE(t.delete_flag, FALSE) = FALSE
                          AND h.inv_date::date BETWEEN @p_from_date AND @p_to_date
                          AND (@p_fiscal_year_id IS NULL OR h.fiscal_year_id = @p_fiscal_year_id)
                    ),
                    'item_wise_goods', COALESCE((
                        SELECT json_agg(row_to_json(sub) ORDER BY sub.purchase_qty + sub.sales_qty DESC, sub.product_name)
                        FROM (
                            SELECT
                                COALESCE(t.product_id, 0) AS product_id,
                                COALESCE(
                                    NULLIF(TRIM(p.product_name), ''),
                                    NULLIF(TRIM(t.product_details), ''),
                                    'Unknown Item'
                                ) AS product_name,
                                COALESCE(SUM(
                                    CASE WHEN LOWER(TRIM(h.inv_type)) = 'purchase' THEN COALESCE(t.quantity, 0) ELSE 0 END
                                ), 0) AS purchase_qty,
                                COALESCE(SUM(
                                    CASE WHEN LOWER(TRIM(h.inv_type)) = 'sales' THEN COALESCE(t.quantity, 0) ELSE 0 END
                                ), 0) AS sales_qty
                            FROM public.tab_sal_pur_tran t
                            INNER JOIN public.tab_sale_pur h
                                ON t.sp_unique_code = h.sp_unique_code
                            LEFT JOIN public.tab_product_master p
                                ON t.product_id = p.product_id
                               AND p.company_id = h.company_id
                            WHERE h.company_id = @p_company_id
                              AND COALESCE(h.delete_flag, FALSE) = FALSE
                              AND COALESCE(t.delete_flag, FALSE) = FALSE
                              AND h.inv_date::date BETWEEN @p_from_date AND @p_to_date
                              AND (@p_fiscal_year_id IS NULL OR h.fiscal_year_id = @p_fiscal_year_id)
                            GROUP BY t.product_id, p.product_name, t.product_details
                            HAVING COALESCE(SUM(COALESCE(t.quantity, 0)), 0) > 0
                        ) sub
                    ), '[]'::json)
                )";

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
            const string sql = @"
                SELECT json_build_object(
                    'product_count', COALESCE((
                        SELECT COUNT(*)
                        FROM public.tab_product_master p
                        WHERE p.company_id = @p_company_id
                          AND p.created_on IS NOT NULL
                          AND p.created_on::date BETWEEN @p_from_date AND @p_to_date
                    ), 0),
                    'party_count', COALESCE((
                        SELECT COUNT(*)
                        FROM public.tab_party_master pm
                        WHERE pm.company_id = @p_company_id
                          AND pm.created_on IS NOT NULL
                          AND pm.created_on::date BETWEEN @p_from_date AND @p_to_date
                    ), 0),
                    'products', COALESCE((
                        SELECT json_agg(row_to_json(sub) ORDER BY sub.created_on DESC)
                        FROM (
                            SELECT
                                p.product_id,
                                COALESCE(NULLIF(TRIM(p.product_name), ''), 'Unnamed Product') AS product_name,
                                COALESCE(NULLIF(TRIM(p.product_type), ''), '—') AS product_type,
                                COALESCE(p.selling_price, 0) AS selling_price,
                                COALESCE(p.cost_price, 0) AS cost_price,
                                p.created_on
                            FROM public.tab_product_master p
                            WHERE p.company_id = @p_company_id
                              AND p.created_on IS NOT NULL
                              AND p.created_on::date BETWEEN @p_from_date AND @p_to_date
                            ORDER BY p.created_on DESC
                        ) sub
                    ), '[]'::json),
                    'parties', COALESCE((
                        SELECT json_agg(row_to_json(sub) ORDER BY sub.created_on DESC)
                        FROM (
                            SELECT
                                pm.party_id,
                                COALESCE(NULLIF(TRIM(pm.display_name), ''), 'Unnamed Party') AS display_name,
                                COALESCE(NULLIF(TRIM(pm.party_type), ''), '—') AS party_type,
                                COALESCE(NULLIF(TRIM(pm.phone_mobile), ''), NULLIF(TRIM(pm.phone_work), ''), '—') AS phone,
                                COALESCE(NULLIF(TRIM(pm.email), ''), '—') AS email,
                                pm.created_on
                            FROM public.tab_party_master pm
                            WHERE pm.company_id = @p_company_id
                              AND pm.created_on IS NOT NULL
                              AND pm.created_on::date BETWEEN @p_from_date AND @p_to_date
                            ORDER BY pm.created_on DESC
                        ) sub
                    ), '[]'::json)
                )";

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
            const string sql = @"
                WITH                 purchase_entries AS (
                    SELECT h.sp_unique_code::text AS entry_key
                    FROM public.tab_sale_pur h
                    WHERE h.company_id = @p_company_id
                      AND COALESCE(h.delete_flag, FALSE) = FALSE
                      AND LOWER(TRIM(h.inv_type)) = 'purchase'
                      AND h.inv_date::date BETWEEN @p_from_date AND @p_to_date
                      AND (@p_fiscal_year_id IS NULL OR h.fiscal_year_id = @p_fiscal_year_id)
                ),
                sales_entries AS (
                    SELECT h.sp_unique_code::text AS entry_key
                    FROM public.tab_sale_pur h
                    WHERE h.company_id = @p_company_id
                      AND COALESCE(h.delete_flag, FALSE) = FALSE
                      AND LOWER(TRIM(h.inv_type)) = 'sales'
                      AND h.inv_date::date BETWEEN @p_from_date AND @p_to_date
                      AND (@p_fiscal_year_id IS NULL OR h.fiscal_year_id = @p_fiscal_year_id)
                ),
                income_entries AS (
                    SELECT j.g_code AS entry_key
                    FROM public.tab_jv j
                    WHERE j.jv_comp_code = @p_company_id
                      AND COALESCE(j.delete_flag, FALSE) = FALSE
                      AND LOWER(TRIM(j.v_type)) = 'income'
                      AND j.jv_date::date BETWEEN @p_from_date AND @p_to_date
                      AND NULLIF(TRIM(j.g_code), '') IS NOT NULL
                    GROUP BY j.g_code
                ),
                expense_entries AS (
                    SELECT j.g_code AS entry_key
                    FROM public.tab_jv j
                    WHERE j.jv_comp_code = @p_company_id
                      AND COALESCE(j.delete_flag, FALSE) = FALSE
                      AND LOWER(TRIM(j.v_type)) = 'expense'
                      AND j.jv_date::date BETWEEN @p_from_date AND @p_to_date
                      AND NULLIF(TRIM(j.g_code), '') IS NOT NULL
                    GROUP BY j.g_code
                ),
                all_jv_entries AS (
                    SELECT entry_key FROM purchase_entries
                    UNION ALL
                    SELECT entry_key FROM sales_entries
                    UNION ALL
                    SELECT entry_key FROM income_entries
                    UNION ALL
                    SELECT entry_key FROM expense_entries
                )
                SELECT json_build_object(
                    'total_purchase', (SELECT COUNT(*) FROM purchase_entries),
                    'total_sales', (SELECT COUNT(*) FROM sales_entries),
                    'total_income', (SELECT COUNT(*) FROM income_entries),
                    'total_expense', (SELECT COUNT(*) FROM expense_entries),
                    'total_jv', (SELECT COUNT(*) FROM all_jv_entries)
                )";

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
            const string sql = @"
                SELECT json_build_object(
                    'total_sale_at_sale_price', COALESCE(SUM(
                        COALESCE(t.quantity, 0) * COALESCE(t.rate, 0)
                    ), 0),
                    'total_sale_at_cog', COALESCE(SUM(
                        COALESCE(t.quantity, 0) * COALESCE(t.cogs, 0)
                    ), 0),
                    'gross_profit', COALESCE(SUM(
                        COALESCE(t.quantity, 0) * (COALESCE(t.rate, 0) - COALESCE(t.cogs, 0))
                    ), 0)
                )
                FROM public.tab_sal_pur_tran t
                INNER JOIN public.tab_sale_pur h
                    ON t.sp_unique_code = h.sp_unique_code
                WHERE h.company_id = @p_company_id
                  AND COALESCE(h.delete_flag, FALSE) = FALSE
                  AND COALESCE(t.delete_flag, FALSE) = FALSE
                  AND LOWER(TRIM(h.inv_type)) = 'sales'
                  AND h.inv_date::date BETWEEN @p_from_date AND @p_to_date
                  AND (@p_fiscal_year_id IS NULL OR h.fiscal_year_id = @p_fiscal_year_id)";

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
            const string sql = @"
                WITH party_accounts AS (
                    SELECT
                        pm.party_id,
                        COALESCE(NULLIF(TRIM(pm.display_name), ''), NULLIF(TRIM(pm.company_name), ''), 'Unknown Party') AS party_name,
                        COALESCE(NULLIF(TRIM(pm.phone_mobile), ''), NULLIF(TRIM(pm.phone_work), ''), '—') AS phone,
                        COALESCE(pm.accounts_receivable_account, pm.party_id) AS ar_account,
                        COALESCE(pm.accounts_payable_account, pm.party_id) AS ap_account,
                        CASE
                            WHEN UPPER(COALESCE(pm.opening_balance_type, '')) = 'DR' THEN COALESCE(pm.opening_balance, 0)
                            WHEN UPPER(COALESCE(pm.opening_balance_type, '')) = 'CR' THEN -COALESCE(pm.opening_balance, 0)
                            ELSE 0
                        END AS opening_balance
                    FROM public.tab_party_master pm
                    WHERE pm.company_id = @p_company_id
                      AND pm.party_type = ANY(ARRAY['Customer'])
                ),
                party_outstanding AS (
                    SELECT
                        pa.party_id,
                        pa.party_name,
                        pa.phone,
                        pa.opening_balance + COALESCE((
                            SELECT
                                SUM(CASE WHEN j.dr_account_code IN (pa.ar_account, pa.ap_account) THEN j.amount ELSE 0 END) -
                                SUM(CASE WHEN j.cr_account_code IN (pa.ar_account, pa.ap_account) THEN j.amount ELSE 0 END)
                            FROM public.tab_jv j
                            WHERE j.jv_comp_code = @p_company_id
                              AND (
                                    j.dr_account_code IN (pa.ar_account, pa.ap_account)
                                 OR j.cr_account_code IN (pa.ar_account, pa.ap_account)
                              )
                              AND j.jv_date::date < @p_as_of_date
                              AND COALESCE(j.delete_flag, FALSE) = FALSE
                        ), 0) AS outstanding
                    FROM party_accounts pa
                ),
                invoiced_in_period AS (
                    SELECT DISTINCT h.party_id
                    FROM public.tab_sale_pur h
                    WHERE h.company_id = @p_company_id
                      AND COALESCE(h.delete_flag, FALSE) = FALSE
                      AND LOWER(TRIM(h.inv_type)) = 'sales'
                      AND h.party_id IS NOT NULL
                      AND h.inv_date::date BETWEEN @p_from_date AND @p_to_date
                      AND (@p_fiscal_year_id IS NULL OR h.fiscal_year_id = @p_fiscal_year_id)
                ),
                filtered_parties AS (
                    SELECT
                        po.party_id,
                        po.party_name,
                        po.phone,
                        ROUND(po.outstanding::numeric, 2) AS outstanding
                    FROM party_outstanding po
                    WHERE ROUND(po.outstanding::numeric, 2) > 0
                      AND po.party_id NOT IN (SELECT party_id FROM invoiced_in_period)
                )
                SELECT json_build_object(
                    'party_count', (SELECT COUNT(*) FROM filtered_parties),
                    'total_outstanding', COALESCE((SELECT SUM(outstanding) FROM filtered_parties), 0),
                    'parties', COALESCE((
                        SELECT json_agg(row_to_json(sub) ORDER BY ABS(sub.outstanding) DESC, sub.party_name)
                        FROM (
                            SELECT
                                party_id,
                                party_name,
                                phone,
                                outstanding
                            FROM filtered_parties
                        ) sub
                    ), '[]'::json)
                )";

            var asOfDate = request.ToDate?.Date.AddDays(1) ?? DashboardDateHelper.GetBusinessToday().AddDays(1);

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
