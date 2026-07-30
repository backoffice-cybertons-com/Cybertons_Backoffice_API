using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoftBook.Application.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Application.Classes
{
    public class PartyRateAdjustmentService : IPartyRateAdjustmentService
    {
        private readonly ISalePurRepository _salePurRepository;
        private readonly IPartyRepository _partyRepository;

        public PartyRateAdjustmentService(ISalePurRepository salePurRepository, IPartyRepository partyRepository)
        {
            _salePurRepository = salePurRepository;
            _partyRepository = partyRepository;
        }

        public async Task<PartyRateAdjustmentResultDto> CalculateAsync(int companyId, int partyId)
        {
            return await CalculateAsOfDateAsync(companyId, partyId, DateTime.Today);
        }

        public async Task<PartyRateAdjustmentResultDto> CalculateAsOfDateAsync(int companyId, int partyId, DateTime asOfDate)
        {
            var party = await _partyRepository.GetByIdAsync(partyId);
            var storedRateDiff = party?.RateDiff ?? 0;

            var last7DaysSales = await _salePurRepository.GetPartyLast7DaysSalesTotalAsOfDateAsync(companyId, partyId, asOfDate);
            var outstanding = await _salePurRepository.GetPartyOutstandingBeforeDateAsync(companyId, partyId, asOfDate.Date);

            var avgSalePerDay = last7DaysSales / 7m;
            decimal avgOutstandingDays = 0;

            if (avgSalePerDay > 0)
            {
                avgOutstandingDays = outstanding / avgSalePerDay;
                if (avgOutstandingDays < 0)
                {
                    avgOutstandingDays = 0;
                }
            }

            var addPriceLookup = await _salePurRepository.GetAddPriceLookupForDueDaysAsync(companyId, avgOutstandingDays);
            var calculatedAddPrice = addPriceLookup.AddPrice;

            // When AutoPersistRateDiff is off, sales orders use the stored value; AddPrice is virtual only.
            var effectiveRateDiff = PartyRateAdjustmentOptions.AutoPersistRateDiff
                ? calculatedAddPrice
                : storedRateDiff;

            return new PartyRateAdjustmentResultDto
            {
                PartyId = partyId,
                Last7DaysSalesTotal = Math.Round(last7DaysSales, 2),
                AvgSalePerDay = Math.Round(avgSalePerDay, 2),
                TotalOutstanding = Math.Round(outstanding, 2),
                AvgOutstandingDays = Math.Round(avgOutstandingDays, 2),
                AddPrice = calculatedAddPrice,
                FromDue = addPriceLookup.FromDue,
                ToDue = addPriceLookup.ToDue,
                StoredRateDiff = storedRateDiff,
                RateDiff = effectiveRateDiff,
                Updated = false
            };
        }

        public async Task<List<PartyRateAdjustmentSummaryItemDto>> GetSummaryListAsync(int companyId,string? partyType = "Customer,Trader",bool onlyWithAddPrice = true)
        {
            var paymentTerms = await _partyRepository.GetPaymentTermLookupAsync(companyId);
            var addPriceLookups = await _salePurRepository.GetAllAddPriceLookupsAsync(companyId);
            var summary = new List<PartyRateAdjustmentSummaryItemDto>();

            var bulkData = await _salePurRepository.GetPartyRateAdjustmentSummaryBulkAsync(companyId, partyType ?? "Customer,Trader", DateTime.Today);

            foreach (var party in bulkData)
            {
                var last7DaysSales = party.Last7DaysSalesTotal;
                var outstanding = party.TotalOutstanding;
                var avgSalePerDay = last7DaysSales / 7m;
                decimal avgOutstandingDays = 0;

                if (avgSalePerDay > 0)
                {
                    avgOutstandingDays = outstanding / avgSalePerDay;
                    if (avgOutstandingDays < 0)
                    {
                        avgOutstandingDays = 0;
                    }
                }

                // In-memory add price lookup
                var addPriceLookup = addPriceLookups.FirstOrDefault(x => avgOutstandingDays >= x.FromDue && avgOutstandingDays <= x.ToDue) 
                                     ?? new SoftBook.Domain.DTOs.AddPriceLookupDto();

                paymentTerms.TryGetValue(party.PaymentTerms ?? string.Empty, out var paymentTerm);

                var item = new PartyRateAdjustmentSummaryItemDto
                {
                    PartyId = party.PartyId,
                    DisplayName = party.DisplayName,
                    PartyType = party.PartyType,
                    PaymentTermName = paymentTerm?.TermName,
                    PaymentTermDays = paymentTerm?.NumberOfDays,
                    Last7DaysSalesTotal = Math.Round(last7DaysSales, 2),
                    AvgSalePerDay = Math.Round(avgSalePerDay, 2),
                    TotalOutstanding = Math.Round(outstanding, 2),
                    AvgOutstandingDays = Math.Round(avgOutstandingDays, 2),
                    FromDue = addPriceLookup.FromDue,
                    ToDue = addPriceLookup.ToDue,
                    AddPrice = addPriceLookup.AddPrice,
                    StoredRateDiff = party.StoredRateDiff
                };

                if (!onlyWithAddPrice || item.AddPrice > 0 || item.StoredRateDiff > 0)
                {
                    summary.Add(item);
                }
            }

            return summary.OrderByDescending(x => x.TotalOutstanding).ToList();
        }


        public async Task<List<PartyRateAdjustmentChangeItemDto>> GetChangeListAsync(int companyId,string? partyType = "Customer,Trader",string? changeType = null)
        {
            var summary = await GetSummaryListAsync(companyId, partyType, onlyWithAddPrice: false);
            var changes = new List<PartyRateAdjustmentChangeItemDto>();

            foreach (var item in summary)
            {
                if (item.AddPrice == item.StoredRateDiff)
                {
                    continue;
                }

                var isUpgraded = item.AddPrice < item.StoredRateDiff;
                var status = isUpgraded ? "Upgraded" : "Downgraded";

                if (!string.IsNullOrWhiteSpace(changeType)
                    && !string.Equals(changeType, status, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                changes.Add(new PartyRateAdjustmentChangeItemDto
                {
                    PartyId = item.PartyId,
                    DisplayName = item.DisplayName,
                    PartyType = item.PartyType,
                    PaymentTermName = item.PaymentTermName,
                    PaymentTermDays = item.PaymentTermDays,
                    Last7DaysSalesTotal = item.Last7DaysSalesTotal,
                    AvgSalePerDay = item.AvgSalePerDay,
                    TotalOutstanding = item.TotalOutstanding,
                    AvgOutstandingDays = item.AvgOutstandingDays,
                    FromDue = item.FromDue,
                    ToDue = item.ToDue,
                    PreviousAddPrice = item.StoredRateDiff,
                    NewAddPrice = item.AddPrice,
                    ChangeAmount = Math.Abs(item.StoredRateDiff - item.AddPrice),
                    ChangeType = status
                });
            }

            return changes
                .OrderByDescending(x => x.ChangeAmount)
                .ThenBy(x => x.DisplayName)
                .ToList();
        }

        public async Task<List<PartyInvoiceAddPriceMismatchDto>> GetInvoiceMismatchListAsync(
            int companyId,
            DateTime fromDate,
            DateTime toDate,
            string? partyType = "Customer,Trader")
        {
            return await _salePurRepository.GetSalesInvoiceAddPriceMismatchesAsync(
                companyId, fromDate, toDate, partyType);
        }

        public async Task<PartyRateAdjustmentResultDto> CalculateAndUpdateAsync(int companyId, int partyId)
        {
            var result = await CalculateAsync(companyId, partyId);

            if (!PartyRateAdjustmentOptions.AutoPersistRateDiff)
            {
                // Virtual calculation only — do not write to tab_party_master.
                result.Updated = false;
                return result;
            }

            var updated = await _partyRepository.UpdateRateDiffAsync(companyId, partyId, result.AddPrice);
            result.Updated = updated;
            result.StoredRateDiff = result.AddPrice;
            result.RateDiff = result.AddPrice;
            return result;
        }
    }
}
