    //using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SoftBook.Application.Services.Interfaces;
//using SoftBook.Domain.RepositoryInterfaces;
//using System.Collections.Generic;

//namespace SoftBook.API.Controllers
//{
//    // [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ReportController : ControllerBase
//    {
//        private readonly IReportService _reportService;
//        private readonly ITenantProvider _tenantProvider;

//        public ReportController(IReportService reportService, ITenantProvider tenantProvider)
//        {
//            _reportService = reportService;
//            _tenantProvider = tenantProvider;
//        }

//        [HttpGet("ledger")]
//        public async Task<IActionResult> GetLedgerReport([FromQuery] int accountCode, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
//        {
//            try
//            {
//                var report = await _reportService.GetLedgerReportAsync(accountCode, fromDate, toDate);
//                return Ok(report);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }

//        //sale-pur-summary InvWise
//        [HttpGet("sale-pur-invwise")]
//        public async Task<IActionResult> GetSalePurInvWise(
//            [FromQuery] int companyId,
//            [FromQuery] DateTime? fromDate,
//            [FromQuery] DateTime? toDate,
//            [FromQuery] string? productName,
//            [FromQuery] string? customerName,
//            [FromQuery] string? invoiceNo,
//            [FromQuery] string? invType)
//        {
//            try
//            {
//                var data = await _reportService.GetSalePurInvWiseAsync(
//                    companyId,
//                    fromDate,
//                    toDate,
//                    productName,
//                    customerName,
//                    invoiceNo,
//                    invType
//                );

//                return Ok(data);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }

//        //sale-pur-summary itemwise
//        [HttpGet("sale-purchase-register-itemwise")]
//        public async Task<IActionResult> GetSalePurchaseRegisterItemwise(
//            [FromQuery] int companyId,
//            [FromQuery] DateTime? fromDate,
//            [FromQuery] DateTime? toDate,
//            [FromQuery] string? productName,
//            [FromQuery] string? customerName,
//            [FromQuery] string? invoiceNo,
//            [FromQuery] string? invType)
//        {
//            try
//            {
//                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterItemwise. CompanyId: {companyId}, From: {fromDate}, To: {toDate}, Customer: {customerName}");

//                if (companyId == 0)
//                {
//                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
//                    return BadRequest("Company context not found.");
//                }

//                var report = await _reportService.GetSalePurchaseRegisterItemwiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//                return Ok(report);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }


//        //sale-pur-summary DateWise
//        [HttpGet("sale-purchase-register-datewise")]
//        public async Task<IActionResult> GetSalePurchaseRegisterDatewise(
//            [FromQuery] int companyId,
//            [FromQuery] DateTime? fromDate,
//            [FromQuery] DateTime? toDate,
//            [FromQuery] string? productName,
//            [FromQuery] string? customerName,
//            [FromQuery] string? invoiceNo,
//            [FromQuery] string? invType)
//        {
//            try
//            {
//                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterDatewise. CompanyId: {companyId}, From: {fromDate}, To: {toDate}, Customer: {customerName}");

//                if (companyId == 0)
//                {
//                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
//                    return BadRequest("Company context not found.");
//                }

//                var report = await _reportService.GetSalePurchaseRegisterDatewiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//                return Ok(report);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }



//        //	//sale-pur-summary PartyWise
//        //sale-pur-summary PartyWise
//        [HttpGet("sale-purchase-register-partywise")]
//        public async Task<IActionResult> GetSalePurchaseRegisterPartywise(
//            [FromQuery] int companyId,
//            [FromQuery] DateTime? fromDate,
//            [FromQuery] DateTime? toDate,
//            [FromQuery] string? productName,
//            [FromQuery] string? customerName,
//            [FromQuery] string? invoiceNo,
//            [FromQuery] string? invType)
//        {
//            try
//            {
//                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterPartywise. CompanyId: {companyId}, From: {fromDate}, To: {toDate}, Customer: {customerName}, Product: {productName}");

//                if (companyId == 0)
//                {
//                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
//                    return BadRequest("Company context not found.");
//                }

//                var report = await _reportService.GetSalePurchaseRegisterPartywiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//                return Ok(report);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }
//    }
//}


using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using ClosedXML.Excel;

namespace SoftBook.API.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ITenantProvider _tenantProvider;

        public ReportController(IReportService reportService, ITenantProvider tenantProvider)
        {
            _reportService = reportService;
            _tenantProvider = tenantProvider;
        }

        [HttpGet("ledger")]
        public async Task<IActionResult> GetLedgerReport([FromQuery] int companyId, [FromQuery] int accountCode, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _reportService.GetLedgerReportAsync(companyId, accountCode, fromDate, toDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //sale-pur-summary InvWise
        [HttpGet("sale-pur-invwise")]
        public async Task<IActionResult> GetSalePurInvWise(
            [FromQuery] int companyId,
            [FromQuery] string? invType,
            [FromQuery] string? where)
        {
            try
            {
                var data = await _reportService.GetSalePurInvWiseAsync(
                    companyId,
                    invType,
                    where
                );

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }




        //sale-pur-summary TransactionWise
        [HttpGet("sale-purchase-register-transactionWise")]
        public async Task<IActionResult> GetSalePurchaseRegisterTransactionWise(
            [FromQuery] int companyId,
            [FromQuery] string? invType,
            [FromQuery] string? where)
        {
            try
            {
                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterTransactionWise. CompanyId: {companyId}, Type: {invType}, Where: {where}");

                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetSalePurchaseRegisterTransactionWise(companyId, invType, where);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        //sale-pur-summary itemwise
        [HttpGet("sale-purchase-register-itemwise")]
        public async Task<IActionResult> GetSalePurchaseRegisterItemwise(
            [FromQuery] int companyId,
            [FromQuery] string? invType,
            [FromQuery] string? where)
        {
            try
            {
                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterItemwise. CompanyId: {companyId}, Type: {invType}, Where: {where}");

                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetSalePurchaseRegisterItemwiseAsync(companyId, invType, where);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        //sale-pur-summary DateWise
        [HttpGet("sale-purchase-register-datewise")]
        public async Task<IActionResult> GetSalePurchaseRegisterDatewise(
            [FromQuery] int companyId,
            [FromQuery] string? invType,
            [FromQuery] string? where)
        {
            try
            {
                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterDatewise. CompanyId: {companyId}, Type: {invType}, Where: {where}");

                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetSalePurchaseRegisterDatewiseAsync(companyId, invType, where);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        //sale-pur-summary PartyWise
        [HttpGet("sale-purchase-register-partywise")]
        public async Task<IActionResult> GetSalePurchaseRegisterPartywise(
            [FromQuery] int companyId,
            [FromQuery] string? invType,
            [FromQuery] string? where)
        {
            try
            {
                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterPartywise. CompanyId: {companyId}, Type: {invType}, Where: {where}");

                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetSalePurchaseRegisterPartywiseAsync(companyId, invType, where);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        //sale-pur-summary MonthWise
        [HttpGet("sale-purchase-register-monthwise")]
        public async Task<IActionResult> GetSalePurchaseRegisterMonthwise(
            [FromQuery] int companyId,
            [FromQuery] string? invType,
            [FromQuery] string? where)
        {
            try
            {
                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterMonthwise. CompanyId: {companyId}, Type: {invType}, Where: {where}");

                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetSalePurchaseRegisterMonthwiseAsync(companyId, invType, where);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("trial-balance")]
        public async Task<IActionResult> GetTrialBalance([FromQuery] int companyId, [FromQuery] DateTime date)
        {
            var result = await _reportService.GetTrialBalance(companyId, date);
            return Ok(result);
        }

        [HttpGet("trial-balance-groupwise")]
        public async Task<IActionResult> GetTrialBalanceGroupWise([FromQuery] int companyId, [FromQuery] DateTime date)
        {
            var result = await _reportService.GetTrialBalanceGroupWise(companyId, date);
            return Ok(result);
        }

        [HttpGet("balance-sheet")]
        public async Task<IActionResult> GetBalanceSheet([FromQuery] int companyId, [FromQuery] DateTime date)
        {
            if (companyId == 0)
            {
                return BadRequest("Company ID is required.");
            }

            var result = await _reportService.GetBalanceSheetReportAsync(companyId, date);
            return Ok(result);
        }

        [HttpGet("profit-loss")]
        public async Task<IActionResult> GetProfitLoss([FromQuery] int companyId, [FromQuery] DateTime date)
        {
            if (companyId == 0)
            {
                return BadRequest("Company ID is required.");
            }

            var result = await _reportService.GetProfitLossReportAsync(companyId, date);
            return Ok(result);
        }




        //by diya  4/24
        //payeble receivable 
        [HttpGet("payeble-receivable")]
        public async Task<IActionResult> GetPayebleReceivable(
            [FromQuery] int companyId,
            [FromQuery] int? partyId,
            [FromQuery] string? pAsOfDate,
            [FromQuery] string partyType)
        {
            try
            {

                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetPayebleReceivableAsync(companyId, partyId, pAsOfDate, partyType);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }




        ///reports/sale-purchase-datewise-comparison
        [HttpGet("sale-purchase-datewise-comparison")]
        public async Task<IActionResult> GetSalePurchaseRegisterDatewiseComparison(
            [FromQuery] int companyId,
            [FromQuery] string? where,
              [FromQuery] string compareMode)
        {
            try
            {
                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterDatewiseComparison. CompanyId: {companyId},  Where: {where}, compareMode:{compareMode}");

                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetSalePurchaseRegisterDatewiseComparisonAsync(companyId, where, compareMode);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //by Diya 04/27
        ///reports/gstr-3b
        [HttpGet("GSTR-3B-Report")]
        public async Task<IActionResult> GetGstr3b(
            [FromQuery] int companyId,
            [FromQuery] string? where,
            [FromQuery] string Type)
        {
            try
            {
                Console.WriteLine($"[ReportController] Fetching Gstr3b. CompanyId: {companyId},  Where: {where}, Type:{Type}");
                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetGSTR3bAsync(companyId, where, Type);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Ledger Summary Datewise - Hetak Modi 02-05-2026
        [HttpGet("ledger-summary-datewise")]
        public async Task<IActionResult> GetLedgerSummaryDatewise(
            [FromQuery] int companyId,
            [FromQuery] int accountCode,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                if (accountCode == 0)
                    return BadRequest("accountCode is required.");

                var result = await _reportService.GetLedgerSummaryDatewiseAsync(companyId, accountCode, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportController] ledger-summary-datewise error: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("gstr1-export")]
        public async Task<IActionResult> ExportGstr1Excel(
            [FromQuery] int companyId,
            [FromQuery] DateOnly fromDate,
            [FromQuery] DateOnly toDate)
        {
            var file = await _reportService
               .GenerateGstr1ExcelAsync(
                  companyId,
                  fromDate,
                  toDate);

            return File(
               file,
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "GSTR1.xlsx"
            );
        }

        [HttpGet("gst-report-ca-export")]
        public async Task<IActionResult> ExportGstReportCaExcel(
            [FromQuery] int companyId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            if (companyId == 0)
            {
                return BadRequest("Company context not found.");
            }

            var file = await _reportService.GenerateGstCaReportExcelAsync(companyId, fromDate, toDate);

            return File(
               file,
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               $"GST_REPORT_CA_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx"
            );
        }

        ///reports/outstanding-comparison - Diya 05-05-2026
        [HttpGet("outstanding-comparison")]
        public async Task<IActionResult> outstandingcomparison(
            [FromQuery] int companyId,
            [FromQuery] DateOnly Date,
            [FromQuery] string? where,
            [FromQuery] string mode)
        {
            try
            {
                Console.WriteLine($"[ReportController] Fetching SalePurchaseRegisterDatewiseComparison. CompanyId: {companyId},  Where: {where}, Date:{Date}");

                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetOutstandingComparisonAsync(companyId, Date,where,mode);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Created by Hetak on 08052026
        [HttpGet("stock-summary")]
        public async Task<IActionResult> GetStockSummaryReport([FromQuery] int companyId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var result = await _reportService.GetStockSummaryReportAsync(companyId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Developed by Hetak on 11-05-2026
        [HttpGet("stock-ledger-datewise")]
        public async Task<IActionResult> GetStockLedgerDatewise([FromQuery] int companyId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] int? productId = null)
        {
            try
            {
                var result = await _reportService.GetStockLedgerDatewiseAsync(companyId, fromDate, toDate, productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("below-price-sales")]
        public async Task<IActionResult> GetBelowPriceSalesReport(
            [FromQuery] int companyId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] string? filterType)
        {
            try
            {
                if (companyId == 0)
                {
                    return BadRequest("Company context not found.");
                }

                var result = await _reportService.GetBelowPriceSalesReportAsync(companyId, fromDate, toDate, filterType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpGet("daily-outstanding-report")]
        public async Task<IActionResult> GetDailyOutstandingReport(
    [FromQuery] int companyId,
    [FromQuery] string? fromDate,
    [FromQuery] string? toDate,
    [FromQuery] int? partyId)
        {
            try
            {
                if (companyId == 0)
                {
                    Console.WriteLine("[ReportController] Warning: CompanyId is 0. Returning BadRequest.");
                    return BadRequest("Company context not found.");
                }

                var report = await _reportService.GetDailyOutstandingReportAsync(
                    companyId,
                    fromDate,
                    toDate,
                    partyId
                );

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("calendar-summary")]
        public async Task<IActionResult> GetCalendarSummary(
            [FromQuery(Name = "company_id")] long companyId,
            [FromQuery(Name = "from_date")] string? fromDate,
            [FromQuery(Name = "to_date")] string? toDate,
            [FromQuery(Name = "party_id")] int? partyId)
        {
            try
            {
                if (companyId <= 0)
                {
                    return BadRequest(new { message = "company_id is required and must be greater than 0." });
                }

                if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
                {
                    return BadRequest(new { message = "from_date and to_date are required (yyyy-MM-dd)." });
                }

                if (!DateTime.TryParse(fromDate, out var parsedFrom))
                {
                    return BadRequest(new { message = "Invalid from_date. Use yyyy-MM-dd format." });
                }

                if (!DateTime.TryParse(toDate, out var parsedTo))
                {
                    return BadRequest(new { message = "Invalid to_date. Use yyyy-MM-dd format." });
                }

                parsedFrom = parsedFrom.Date;
                parsedTo = parsedTo.Date;

                if (parsedFrom > parsedTo)
                {
                    return BadRequest(new { message = "from_date cannot be after to_date." });
                }

                var dayCount = (parsedTo - parsedFrom).Days + 1;
                if (dayCount > 93)
                {
                    return BadRequest(new { message = "Date range cannot exceed 93 days for calendar summary." });
                }

                var report = await _reportService.GetCalendarSummaryReportAsync(companyId, parsedFrom, parsedTo, partyId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Code developed by Hetak Modi 290526
        [HttpGet("day-book")]
        public async Task<IActionResult> GetDayBook([FromQuery] int companyId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                if (companyId == 0) return BadRequest("Company ID is required.");
                var result = await _reportService.GetDayBookReportAsync(companyId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Code developed by Hetak Modi 290526
        [HttpGet("cash-book")]
        public async Task<IActionResult> GetCashBook([FromQuery] int companyId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                if (companyId == 0) return BadRequest("Company ID is required.");
                var result = await _reportService.GetCashBookReportAsync(companyId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Code developed by Hetak Modi 290526
        [HttpGet("bank-book")]
        public async Task<IActionResult> GetBankBook([FromQuery] int companyId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                if (companyId == 0) return BadRequest("Company ID is required.");
                var result = await _reportService.GetBankBookReportAsync(companyId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Customer Yesterday OS / Yesterday Order / Today Payment (Income Direct JV).
        /// Today Order is returned computed for UI; Excel export sets it as a formula.
        /// </summary>
        [HttpGet("customer-order-payment")]
        public async Task<IActionResult> GetCustomerOrderPaymentReport(
            [FromQuery] int companyId,
            [FromQuery] DateTime? reportDate,
            [FromQuery] int? partyId)
        {
            try
            {
                if (companyId == 0)
                {
                    return BadRequest("Company context not found.");
                }

                var date = (reportDate ?? DateTime.Today).Date;
                var result = await _reportService.GetCustomerOrderPaymentReportAsync(companyId, date, partyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Monthly Commission Report: date-wise for a party, or summary per party for all parties.
        /// Columns: A-O as per screenshot.
        /// </summary>
        [HttpGet("monthly-commission")]
        public async Task<IActionResult> GetMonthlyCommissionReport(
            [FromQuery] int companyId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? partyId,
            [FromQuery] string groupBy = "date")
        {
            try
            {
                if (companyId == 0)
                    return BadRequest("Company context not found.");

                var from = (fromDate ?? DateTime.Today.AddDays(-30)).Date;
                var to = (toDate ?? DateTime.Today).Date;
                var result = await _reportService.GetMonthlyCommissionReportAsync(companyId, from, to, partyId, groupBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
