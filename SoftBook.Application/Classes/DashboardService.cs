using SoftBook.Application.Interfaces;
using SoftBook.Domain.Helpers;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SoftBook.Application.Classes
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IPartyRateAdjustmentService _partyRateAdjustmentService;

        public DashboardService(
            IDashboardRepository dashboardRepository,
            IReportRepository reportRepository,
            IPartyRateAdjustmentService partyRateAdjustmentService)
        {
            _dashboardRepository = dashboardRepository;
            _reportRepository = reportRepository;
            _partyRateAdjustmentService = partyRateAdjustmentService;
        }

        public async Task<string> GetDashboardDataAsync(DashboardRequestDto request)
        {
            ApplyDashboardDateRange(request);

            // Apply defaults if still null
            request.ZoneType ??= "ALL";
            request.TopLimit ??= 10;
            request.Granularity ??= "daily";
            request.InvStatusPartyType ??= "Customer";

            // If fromDate/toDate are still null, default to THIS_MONTH
            if (request.FromDate == null || request.ToDate == null)
            {
                var today = DashboardDateHelper.GetBusinessToday();
                request.FromDate = new DateTime(today.Year, today.Month, 1);
                request.ToDate = today;
            }

            var dashboardJson = await _dashboardRepository.GetDashboardDataAsync(request);

            try
            {
                var goodsJson = await _dashboardRepository.GetGoodsSummaryAsync(request);
                dashboardJson = MergeDashboardJson(dashboardJson, goodsJson);
            }
            catch
            {
                // Keep base dashboard data if goods summary fails.
            }

            try
            {
                var transactionJson = await _dashboardRepository.GetTransactionSummaryAsync(request);
                dashboardJson = MergeTransactionSummary(dashboardJson, transactionJson);
            }
            catch
            {
                // Keep dashboard data if transaction summary query fails.
            }

            try
            {
                var salesProfitJson = await _dashboardRepository.GetSalesProfitSummaryAsync(request);
                dashboardJson = MergeSalesProfitSummary(dashboardJson, salesProfitJson);
            }
            catch
            {
                // Keep dashboard data if sales profit summary query fails.
            }

            try
            {
                var outstandingNoInvoiceJson = await _dashboardRepository.GetOutstandingNoInvoiceAsync(request);
                dashboardJson = MergeOutstandingNoInvoice(dashboardJson, outstandingNoInvoiceJson);
            }
            catch
            {
                // Keep dashboard data if outstanding-without-invoice query fails.
            }

            try
            {
                var belowPriceRows = await _reportRepository.GetBelowPriceSalesReportAsync(
                    request.CompanyId,
                    request.FromDate!.Value,
                    request.ToDate!.Value,
                    "all");
                dashboardJson = MergeBelowPriceSales(dashboardJson, belowPriceRows);
            }
            catch
            {
                // Keep dashboard data if below-price sales query fails.
            }

            try
            {
                var mastersJson = await _dashboardRepository.GetMastersCreatedAsync(
                    request.CompanyId,
                    request.FromDate!.Value.Date,
                    request.ToDate!.Value.Date);
                dashboardJson = MergeNewMasters(dashboardJson, mastersJson);
            }
            catch
            {
                // Keep dashboard data if new masters query fails.
            }

            if (DashboardFeatureFlags.ShowPartyAddPriceCards)
            {
                try
                {
                    var summary = await _partyRateAdjustmentService.GetSummaryListAsync(
                        request.CompanyId, "Customer,Trader", onlyWithAddPrice: false);
                    var mismatches = await _partyRateAdjustmentService.GetInvoiceMismatchListAsync(
                        request.CompanyId, request.FromDate!.Value, request.ToDate!.Value, "Customer,Trader");
                    var changes = BuildPartyAddPriceChanges(summary);
                    dashboardJson = MergePartyAddPrice(dashboardJson, summary, mismatches, changes);
                }
                catch
                {
                    // Keep dashboard data if party add price queries fail.
                }
            }

            return dashboardJson;
        }

        private static List<PartyRateAdjustmentChangeItemDto> BuildPartyAddPriceChanges(
            IEnumerable<PartyRateAdjustmentSummaryItemDto> summary)
        {
            return summary
                .Where(x => x.AddPrice != x.StoredRateDiff)
                .Select(x =>
                {
                    var isUpgraded = x.AddPrice < x.StoredRateDiff;
                    return new PartyRateAdjustmentChangeItemDto
                    {
                        PartyId = x.PartyId,
                        DisplayName = x.DisplayName,
                        PartyType = x.PartyType,
                        PaymentTermName = x.PaymentTermName,
                        PaymentTermDays = x.PaymentTermDays,
                        Last7DaysSalesTotal = x.Last7DaysSalesTotal,
                        AvgSalePerDay = x.AvgSalePerDay,
                        TotalOutstanding = x.TotalOutstanding,
                        AvgOutstandingDays = x.AvgOutstandingDays,
                        FromDue = x.FromDue,
                        ToDue = x.ToDue,
                        PreviousAddPrice = x.StoredRateDiff,
                        NewAddPrice = x.AddPrice,
                        ChangeAmount = Math.Abs(x.StoredRateDiff - x.AddPrice),
                        ChangeType = isUpgraded ? "Upgraded" : "Downgraded"
                    };
                })
                .OrderByDescending(x => x.ChangeAmount)
                .ThenBy(x => x.DisplayName)
                .ToList();
        }

        private static void ApplyDashboardDateRange(DashboardRequestDto request)
        {
            if (request.FromDate.HasValue && request.ToDate.HasValue)
            {
                request.FromDate = request.FromDate.Value.Date;
                request.ToDate = request.ToDate.Value.Date;

                if (request.Granularity == null)
                {
                    request.Granularity = InferGranularity(
                        request.FromDate.Value,
                        request.ToDate.Value,
                        request.FilterType);
                }

                return;
            }

            if (string.IsNullOrEmpty(request.FilterType))
            {
                return;
            }

            var today = DashboardDateHelper.GetBusinessToday();
            switch (request.FilterType.ToUpper())
            {
                case "YESTERDAY":
                    var yesterday = today.AddDays(-1);
                    request.FromDate = yesterday;
                    request.ToDate = yesterday;
                    request.Granularity ??= "daily";
                    break;

                case "TODAY":
                    request.FromDate = today;
                    request.ToDate = today;
                    request.Granularity ??= "daily";
                    break;

                case "THIS_WEEK":
                    var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
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

        private static string InferGranularity(DateTime fromDate, DateTime toDate, string? filterType)
        {
            if (string.Equals(filterType, "THIS_YEAR", StringComparison.OrdinalIgnoreCase))
            {
                return "monthly";
            }

            var span = (toDate - fromDate).TotalDays;
            if (span <= 31)
            {
                return "daily";
            }

            if (span <= 366)
            {
                return "monthly";
            }

            return "yearly";
        }

        private static string MergeDashboardJson(string dashboardJson, string goodsJson)
        {
            var dashboard = JsonNode.Parse(string.IsNullOrWhiteSpace(dashboardJson) ? "{}" : dashboardJson) as JsonObject
                ?? new JsonObject();
            var goods = JsonNode.Parse(string.IsNullOrWhiteSpace(goodsJson) ? "{}" : goodsJson) as JsonObject;

            if (goods?["goods_summary"] != null)
            {
                dashboard["goods_summary"] = goods["goods_summary"]!.DeepClone();
            }

            if (goods?["item_wise_goods"] != null)
            {
                dashboard["item_wise_goods"] = goods["item_wise_goods"]!.DeepClone();
            }

            return dashboard.ToJsonString();
        }

        private static string MergeBelowPriceSales(string dashboardJson, List<BelowPriceSalesReportDTO> rows)
        {
            var dashboard = JsonNode.Parse(string.IsNullOrWhiteSpace(dashboardJson) ? "{}" : dashboardJson) as JsonObject
                ?? new JsonObject();

            var items = new JsonArray();
            foreach (var row in rows)
            {
                items.Add(new JsonObject
                {
                    ["sp_unique_code"] = row.SpUniqueCode,
                    ["inv_date"] = row.InvDate.ToString("yyyy-MM-dd"),
                    ["inv_no"] = row.InvNo,
                    ["party_name"] = row.PartyName,
                    ["product_name"] = row.ProductName,
                    ["qty"] = row.Qty,
                    ["invoice_rate"] = row.InvoiceRate,
                    ["cost_price"] = row.CostPrice,
                    ["selling_price"] = row.SellingPrice,
                    ["loss_vs_cost"] = row.LossVsCost,
                    ["diff_vs_selling_price"] = row.DiffVsSellingPrice,
                    ["is_below_cost"] = row.IsBelowCost,
                    ["is_below_selling_price"] = row.IsBelowSellingPrice,
                    ["alert_type"] = row.AlertType
                });
            }

            dashboard["below_price_sales"] = new JsonObject
            {
                ["below_cost_count"] = rows.Count(r => r.IsBelowCost),
                ["below_sale_price_count"] = rows.Count(r => r.IsBelowSellingPrice),
                ["total_count"] = rows.Count,
                ["items"] = items
            };

            return dashboard.ToJsonString();
        }

        private static string MergeTransactionSummary(string dashboardJson, string transactionJson)
        {
            var dashboard = JsonNode.Parse(string.IsNullOrWhiteSpace(dashboardJson) ? "{}" : dashboardJson) as JsonObject
                ?? new JsonObject();
            var summary = JsonNode.Parse(string.IsNullOrWhiteSpace(transactionJson) ? "{}" : transactionJson);

            if (summary != null)
            {
                dashboard["transaction_summary"] = summary.DeepClone();
            }

            return dashboard.ToJsonString();
        }

        private static string MergeSalesProfitSummary(string dashboardJson, string salesProfitJson)
        {
            var dashboard = JsonNode.Parse(string.IsNullOrWhiteSpace(dashboardJson) ? "{}" : dashboardJson) as JsonObject
                ?? new JsonObject();
            var summary = JsonNode.Parse(string.IsNullOrWhiteSpace(salesProfitJson) ? "{}" : salesProfitJson);

            if (summary != null)
            {
                dashboard["sales_profit_summary"] = summary.DeepClone();
            }

            return dashboard.ToJsonString();
        }

        private static string MergeOutstandingNoInvoice(string dashboardJson, string outstandingJson)
        {
            var dashboard = JsonNode.Parse(string.IsNullOrWhiteSpace(dashboardJson) ? "{}" : dashboardJson) as JsonObject
                ?? new JsonObject();
            var data = JsonNode.Parse(string.IsNullOrWhiteSpace(outstandingJson) ? "{}" : outstandingJson);

            if (data != null)
            {
                dashboard["outstanding_no_invoice"] = data.DeepClone();
            }

            return dashboard.ToJsonString();
        }

        private static string MergeNewMasters(string dashboardJson, string mastersJson)
        {
            var dashboard = JsonNode.Parse(string.IsNullOrWhiteSpace(dashboardJson) ? "{}" : dashboardJson) as JsonObject
                ?? new JsonObject();
            var masters = JsonNode.Parse(string.IsNullOrWhiteSpace(mastersJson) ? "{}" : mastersJson);

            if (masters != null)
            {
                dashboard["new_masters_today"] = masters.DeepClone();
            }

            return dashboard.ToJsonString();
        }

        private static string MergePartyAddPrice(
            string dashboardJson,
            List<PartyRateAdjustmentSummaryItemDto> summary,
            List<PartyInvoiceAddPriceMismatchDto> mismatches,
            List<PartyRateAdjustmentChangeItemDto> changes)
        {
            const int dashboardLimit = 10;
            var dashboard = JsonNode.Parse(string.IsNullOrWhiteSpace(dashboardJson) ? "{}" : dashboardJson) as JsonObject
                ?? new JsonObject();

            var summaryItems = new JsonArray();
            foreach (var row in summary.Where(x => x.AddPrice > 0).Take(dashboardLimit))
            {
                summaryItems.Add(new JsonObject
                {
                    ["party_id"] = row.PartyId,
                    ["display_name"] = row.DisplayName,
                    ["party_type"] = row.PartyType,
                    ["total_outstanding"] = row.TotalOutstanding,
                    ["last_7_days_sales_total"] = row.Last7DaysSalesTotal,
                    ["avg_sale_per_day"] = row.AvgSalePerDay,
                    ["avg_outstanding_days"] = row.AvgOutstandingDays,
                    ["from_due"] = row.FromDue,
                    ["to_due"] = row.ToDue,
                    ["add_price"] = row.AddPrice,
                    ["stored_rate_diff"] = row.StoredRateDiff
                });
            }

            var mismatchItems = new JsonArray();
            foreach (var row in mismatches.Take(dashboardLimit))
            {
                mismatchItems.Add(new JsonObject
                {
                    ["sp_unique_code"] = row.SpUniqueCode,
                    ["inv_no"] = row.InvNo,
                    ["inv_date"] = row.InvDate.ToString("yyyy-MM-dd"),
                    ["party_id"] = row.PartyId,
                    ["display_name"] = row.DisplayName,
                    ["expected_add_price"] = row.ExpectedAddPrice,
                    ["actual_add_price"] = row.ActualAddPrice,
                    ["difference"] = row.Difference,
                    ["avg_outstanding_days"] = row.AvgOutstandingDays
                });
            }

            var changeItems = new JsonArray();
            foreach (var row in changes.Take(dashboardLimit))
            {
                changeItems.Add(new JsonObject
                {
                    ["party_id"] = row.PartyId,
                    ["display_name"] = row.DisplayName,
                    ["party_type"] = row.PartyType,
                    ["total_outstanding"] = row.TotalOutstanding,
                    ["avg_outstanding_days"] = row.AvgOutstandingDays,
                    ["previous_add_price"] = row.PreviousAddPrice,
                    ["new_add_price"] = row.NewAddPrice,
                    ["change_amount"] = row.ChangeAmount,
                    ["change_type"] = row.ChangeType
                });
            }

            dashboard["party_add_price"] = new JsonObject
            {
                ["summary"] = new JsonObject
                {
                    ["total_count"] = summary.Count,
                    ["with_add_price_count"] = summary.Count(x => x.AddPrice > 0),
                    ["items"] = summaryItems
                },
                ["invoice_mismatches"] = new JsonObject
                {
                    ["total_count"] = mismatches.Count,
                    ["items"] = mismatchItems
                },
                ["changes"] = new JsonObject
                {
                    ["total_count"] = changes.Count,
                    ["upgraded_count"] = changes.Count(x => x.ChangeType == "Upgraded"),
                    ["downgraded_count"] = changes.Count(x => x.ChangeType == "Downgraded"),
                    ["items"] = changeItems
                }
            };

            return dashboard.ToJsonString();
        }
    }
}
