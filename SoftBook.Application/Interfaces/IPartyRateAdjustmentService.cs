using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Interfaces
{
    public interface IPartyRateAdjustmentService
    {
        Task<PartyRateAdjustmentResultDto> CalculateAndUpdateAsync(int companyId, int partyId);
        Task<PartyRateAdjustmentResultDto> CalculateAsync(int companyId, int partyId);
        Task<List<PartyRateAdjustmentSummaryItemDto>> GetSummaryListAsync(int companyId, string? partyType = "Customer,Trader", bool onlyWithAddPrice = true);
        Task<PartyRateAdjustmentResultDto> CalculateAsOfDateAsync(int companyId, int partyId, DateTime asOfDate);
        Task<List<PartyInvoiceAddPriceMismatchDto>> GetInvoiceMismatchListAsync(int companyId, DateTime fromDate, DateTime toDate, string? partyType = "Customer,Trader");
        Task<List<PartyRateAdjustmentChangeItemDto>> GetChangeListAsync(int companyId, string? partyType = "Customer,Trader", string? changeType = null);
    }
}
