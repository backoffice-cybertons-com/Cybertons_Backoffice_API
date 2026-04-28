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
        public async Task<IActionResult> GetLedgerReport([FromQuery] int accountCode, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _reportService.GetLedgerReportAsync(accountCode, fromDate, toDate);
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
        public async Task<IActionResult> GetTrialBalance(int companyId, DateTime date)
        {
            var result = await _reportService.GetTrialBalance(companyId, date);
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

    }
}

