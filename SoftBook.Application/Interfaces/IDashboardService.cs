using SoftBook.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<string> GetDashboardDataAsync(DashboardRequestDto request);
        Task<List<CashBankPartyDetailDto>> GetCashBankPartyDetailsAsync(int companyId, DateTime fromDate, DateTime toDate, string accountGroup);
    }
}
