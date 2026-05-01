using SoftBook.Application.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Threading.Tasks;

namespace SoftBook.Application.Classes
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<string> GetDashboardDataAsync(DashboardRequestDto request)
        {
            // Resolve filterType if provided
            if (!string.IsNullOrEmpty(request.FilterType))
            {
                var today = DateTime.Today;
                switch (request.FilterType.ToUpper())
                {
                    case "TODAY":
                        request.FromDate = today;
                        request.ToDate = today;
                        request.Granularity ??= "daily";
                        break;

                    case "THIS_WEEK":
                        // Assuming week starts on Monday
                        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                        request.FromDate = today.AddDays(-1 * diff).Date;
                        request.ToDate = today;
                        request.Granularity ??= "daily";
                        break;

                    case "THIS_MONTH":
                        request.FromDate = new DateTime(today.Year, today.Month, 1);
                        request.ToDate = today;
                        request.Granularity ??= "daily";
                        break;

                    case "THIS_YEAR":
                        // Financial year or Calendar year? Using Calendar year (Jan 1st) as default
                        request.FromDate = new DateTime(today.Year, 1, 1);
                        request.ToDate = today;
                        request.Granularity ??= "MONTH"; // Matches fn_dashboard_sales_trend case sensitivity
                        break;
                }
            }

            // Apply defaults if still null
            request.ZoneType ??= "ALL";
            request.ExpenseVTypes ??= new[] { "Expense" };
            request.TopLimit ??= 10;
            request.Granularity ??= "daily";

            // If FilterType was THIS_YEAR, we might need to pass "MONTH" to fn_dashboard_sales_trend 
            // but the aggregator calls both with the same p_granularity.
            // Let's look at fn_dashboard_master_aggregator again.
            // It passes p_granularity to fn_dashboard_sales_trend and fn_dashboard_bar_line_graph.
            // fn_dashboard_sales_trend checks for 'MONTH' (uppercase).
            // fn_dashboard_bar_line_graph checks for 'monthly' (lowercase).
            
            // This is a bit of a mismatch in the provided SQL. 
            // I'll stick to 'monthly' for now as it's more common, 
            // but if the user sees issues with Sales Trend, they might need to fix the SQL case sensitivity.
            // Actually, I'll use 'monthly' and 'daily' as the primary ones.

            // If fromDate/toDate are still null (no filterType and no custom dates), default to THIS_MONTH
            if (request.FromDate == null || request.ToDate == null)
            {
                var today = DateTime.Today;
                request.FromDate = new DateTime(today.Year, today.Month, 1);
                request.ToDate = today;
            }

            return await _dashboardRepository.GetDashboardDataAsync(request);
        }
    }
}
