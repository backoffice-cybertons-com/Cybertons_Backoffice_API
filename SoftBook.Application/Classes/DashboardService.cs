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
                        // Week starts on Monday
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

                    case "PREV_MONTH":
                        var prevMonthFirst = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
                        request.FromDate = prevMonthFirst;
                        request.ToDate = prevMonthFirst.AddMonths(1).AddDays(-1);
                        request.Granularity ??= "daily";
                        break;

                    case "THIS_YEAR":
                        request.FromDate = new DateTime(today.Year, 1, 1);
                        request.ToDate = today;
                        request.Granularity ??= "monthly";
                        break;
                }
            }

            // For CUSTOM filter (no filterType, both dates provided), auto-detect granularity
            if (string.IsNullOrEmpty(request.FilterType) && request.FromDate.HasValue && request.ToDate.HasValue)
            {
                if (request.Granularity == null)
                {
                    var span = (request.ToDate.Value - request.FromDate.Value).TotalDays;
                    if (span <= 31)
                        request.Granularity = "daily";
                    else if (span <= 366)
                        request.Granularity = "monthly";
                    else
                        request.Granularity = "yearly";
                }
            }

            // Apply defaults if still null
            request.ZoneType ??= "ALL";
            request.ExpenseVTypes ??= new[] { "Expense" };
            request.TopLimit ??= 10;
            request.Granularity ??= "daily";

            // If fromDate/toDate are still null, default to THIS_MONTH
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
