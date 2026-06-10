using SoftBook.Domain.DTOs;
using System;
using System.Threading.Tasks;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IDashboardRepository
    {
        Task<string> GetDashboardDataAsync(DashboardRequestDto request);
        Task<string> GetGoodsSummaryAsync(DashboardRequestDto request);
        Task<string> GetMastersCreatedAsync(int companyId, DateTime fromDate, DateTime toDate);
        Task<string> GetTransactionSummaryAsync(DashboardRequestDto request);
        Task<string> GetSalesProfitSummaryAsync(DashboardRequestDto request);
        Task<string> GetOutstandingNoInvoiceAsync(DashboardRequestDto request);
    }
}
