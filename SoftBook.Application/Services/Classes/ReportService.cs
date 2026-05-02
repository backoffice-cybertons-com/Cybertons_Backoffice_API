//using System;
//using System.Threading.Tasks;
//using SoftBook.Application.Services.Interfaces;
//using SoftBook.Domain.DTOs;
//using SoftBook.Domain.RepositoryInterfaces;

//namespace SoftBook.Application.Services.Classes
//{
//    public class ReportService : IReportService
//    {
//        private readonly IReportRepository _reportRepository;

//        public ReportService(IReportRepository reportRepository)
//        {
//            _reportRepository = reportRepository;
//        }

//        public async Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate)
//        {
//            return await _reportRepository.GetLedgerReportAsync(accountCode, fromDate, toDate);
//        }

//        public async Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType)
//        {
//            return await _reportRepository.GetSalePurchaseRegisterItemwiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//        }
//        public async Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType)
//        {
//            return await _reportRepository.GetSalePurchaseRegisterDatewiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//        }
//        public async Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType)
//        {
//            return await _reportRepository.GetSalePurchaseRegisterPartywiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//        }

//        public async Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType)
//        {
//            return await _reportRepository.GetSalePurInvWiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//        }
//    }
//}


using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.VisualBasic;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoftBook.Application.Services.Classes
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate)
        {
            return await _reportRepository.GetLedgerReportAsync(accountCode, fromDate, toDate);
        }

        public async Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurchaseRegisterItemwiseAsync(companyId, invType, where);
        }
        public async Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurchaseRegisterDatewiseAsync(companyId, invType, where);
        }
        public async Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurchaseRegisterPartywiseAsync(companyId, invType, where);
        }
        public async Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurInvWiseAsync(companyId, invType, where);
        }

        public async Task<IEnumerable<SalePurchaseRegisterMonthwiseDTO>> GetSalePurchaseRegisterMonthwiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurchaseRegisterMonthwiseAsync(companyId, invType, where);
        }

        //----------------------trial balance code - 230426 hetak modi--------------------------------------------------------------------------------------------------
        public async Task<List<TrialBalanceDTO>> GetTrialBalance(int companyId, DateTime date)
        {
            return await _reportRepository.GetTrialBalance(companyId, date);
        }



        //by diya 4/24
        //payeble receivable
        public async Task<IEnumerable<PayebleReceivableDTO>> GetPayebleReceivableAsync(int companyId, int? partyId, string? pAsOfDate, string partyType)
        {
            return await _reportRepository.GetPayebleReceivableAsync(companyId, partyId, pAsOfDate, partyType);
        }

        public async Task<IEnumerable<SalePurchaseRegisterDatewiseComparisonDTO>> GetSalePurchaseRegisterDatewiseComparisonAsync(int companyId, string? where, string compareMode)
        {
            return await _reportRepository.GetSalePurchaseRegisterDatewiseComparisonAsync(companyId, where, compareMode);
        }


        //by diya 4/27
        //GSTR-3b
        public async Task<IEnumerable<GetGSTR3bDTO>> GetGSTR3bAsync(int companyId, string? where, string Type)
        {
            return await _reportRepository.GetGSTR3bAsync(companyId, where, Type);
        }

        //by diya 4/27
        //GSTR-3b
        public async Task<IEnumerable<GetGSTR3bDTO>> GetGSTR3bAsync(int companyId, string? where, string Type)
        {
            return await _reportRepository.GetGSTR3bAsync(companyId, where, Type);
        }

        public async Task<byte[]> GenerateGstr1ExcelAsync(
    int companyId,
    DateOnly fromDate,
    DateOnly toDate)
        {
            var rows = await _reportRepository
                .GetGstr1ExcelExportAsync(companyId, fromDate, toDate);

            using var wb = new XLWorkbook();

            // ════════════════════════════════════════════════════════════════
            // COLORS  (matched exactly from reference file)
            // ════════════════════════════════════════════════════════════════
            var colorTitleBg = XLColor.FromArgb(0, 112, 192);     // 🔵 DARK BLUE
            var colorRevBg = XLColor.FromArgb(180, 198, 231);   // 🔷 LIGHT BLUE
            var colorOrigBg = XLColor.FromArgb(247, 202, 172);   // 🟧 ORIGINAL (keep or change)
            var colorHeaderBg = XLColor.FromArgb(247, 202, 172);   // 🟧 NORMAL HEADER

            // ════════════════════════════════════════════════════════════════
            // HEADERS  (row 4 column labels per sheet)
            // ════════════════════════════════════════════════════════════════
            var headers = new Dictionary<string, string[]>
            {
                ["b2b,sez,de"] = new[] { "GSTIN/UIN of Recipient", "Receiver Name", "Invoice Number", "Invoice date", "Invoice Value", "Place Of Supply", "Reverse Charge", "Applicable % of Tax Rate", "Invoice Type", "E-Commerce GSTIN", "Rate", "Taxable Value", "Cess Amount" },
                ["b2ba"] = new[] { "GSTIN/UIN of Recipient", "Receiver Name", "Original Invoice Number", "Original Invoice date", "Revised Invoice Number", "Revised Invoice date", "Invoice Value", "Place Of Supply", "Reverse Charge", "Applicable % of Tax Rate", "Invoice Type", "E-Commerce GSTIN", "Rate", "Taxable Value", "Cess Amount" },
                ["b2cl"] = new[] { "Invoice Number", "Invoice date", "Invoice Value", "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "E-Commerce GSTIN" },
                ["b2cla"] = new[] { "Original Invoice Number", "Original Invoice date", "Original Place Of Supply", "Revised Invoice Number", "Revised Invoice date", "Invoice Value", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "E-Commerce GSTIN" },
                ["b2cs"] = new[] { "Type", "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "E-Commerce GSTIN" },
                ["b2csa"] = new[] { "Financial Year", "Original Month", "Place Of Supply", "Type", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount", "E-Commerce GSTIN" },
                ["cdnr"] = new[] { "GSTIN/UIN of Recipient", "Receiver Name", "Note Number", "Note Date", "Note Type", "Place Of Supply", "Reverse Charge", "Note Supply Type", "Note Value", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount" },
                ["cdnra"] = new[] { "GSTIN/UIN of Recipient", "Receiver Name", "Original Note Number", "Original Note Date", "Revised Note Number", "Revised Note Date", "Note Type", "Place Of Supply", "Reverse Charge", "Note Supply Type", "Note Value", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount" },
                ["cdnur"] = new[] { "UR Type", "Note Number", "Note Date", "Note Type", "Place Of Supply", "Note Value", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount" },
                ["cdnura"] = new[] { "UR Type", "Original Note Number", "Original Note Date", "Revised Note Number", "Revised Note Date", "Note Type", "Place Of Supply", "Note Value", "Applicable % of Tax Rate", "Rate", "Taxable Value", "Cess Amount" },
                ["exp"] = new[] { "Export Type", "Invoice Number", "Invoice date", "Invoice Value", "Port Code", "Shipping Bill Number", "Shipping Bill Date", "Rate", "Taxable Value", "Cess Amount" },
                ["expa"] = new[] { "Export Type", "Original Invoice Number", "Original Invoice date", "Revised Invoice Number", "Revised Invoice date", "Invoice Value", "Port Code", "Shipping Bill Number", "Shipping Bill Date", "Rate", "Taxable Value", "Cess Amount" },
                ["at"] = new[] { "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Gross Advance Received", "Cess Amount" },
                ["ata"] = new[] { "Financial Year", "Original Month", "Original Place Of Supply", "Applicable % of Tax Rate", "Rate", "Gross Advance Received", "Cess Amount" },
                ["atadj"] = new[] { "Place Of Supply", "Applicable % of Tax Rate", "Rate", "Gross Advance Adjusted", "Cess Amount" },
                ["atadja"] = new[] { "Financial Year", "Original Month", "Original Place Of Supply", "Applicable % of Tax Rate", "Rate", "Gross Advance Adjusted", "Cess Amount" },
                ["exemp"] = new[] { "Description", "Nil Rated Supplies", "Exempted(other than nil rated/non GST supply)", "Non-GST Supplies" },
                ["hsn(b2b)"] = new[] { "HSN", "Description", "UQC", "Total Quantity", "Total Value", "Rate", "Taxable Value", "Integrated Tax Amount", "Central Tax Amount", "State/UT Tax Amount", "Cess Amount" },
                ["hsn(b2c)"] = new[] { "HSN", "Description", "UQC", "Total Quantity", "Total Value", "Rate", "Taxable Value", "Integrated Tax Amount", "Central Tax Amount", "State/UT Tax Amount", "Cess Amount" },
                ["docs"] = new[] { "Nature of Document", "Sr. No. From", "Sr. No. To", "Total Number", "Cancelled" },
                ["eco"] = new[] { "Nature of Supply", "GSTIN of E-Commerce Operator", "E-Commerce Operator Name", "Net value of supplies", "Integrated tax", "Central tax", "State/UT tax", "Cess" },
                ["ecoa"] = new[] { "Nature of Supply", "Financial Year", "Original Month/Quarter", "Original GSTIN of E-Commerce Operator", "Revised GSTIN of E-Commerce Operator", "E-Commerce Operator Name", "Revised Net value of supplies", "Integrated tax", "Central tax", "State/UT tax", "Cess" },
                ["ecob2b"] = new[] { "Supplier GSTIN/UIN", "Supplier Name", "Recipient GSTIN/UIN", "Recipient Name", "Document Number", "Document Date", "Value of supplies made", "Place Of Supply", "Document type", "Rate", "Taxable Value", "Cess Amount" },
                ["ecourp2b"] = new[] { "Recipient GSTIN/UIN", "Recipient Name", "Document Number", "Document Date", "Value of supplies made", "Place Of Supply", "Document type", "Rate", "Taxable Value", "Cess Amount" },
                ["ecob2c"] = new[] { "Supplier GSTIN/UIN", "Supplier Name", "Place Of Supply", "Taxable Value", "Rate", "Cess Amount" },
                ["ecourp2c"] = new[] { "Place Of Supply", "Taxable Value", "Rate", "Cess Amount" },
                ["ecoab2b"] = new[] { "Supplier GSTIN/UIN", "Supplier Name", "Recipient GSTIN/UIN", "Recipient Name", "Original Document Number", "Original Document Date", "Revised Document Number", "Revised Document Date", "Value of supplies made", "Place Of Supply", "Document type", "Rate", "Taxable Value", "Cess Amount" },
                ["ecoab2c"] = new[] { "Financial Year", "Original Month", "Supplier GSTIN/UIN", "Supplier Name", "Place Of Supply", "Rate", "Taxable Value", "Cess Amount" },
                ["ecoaurp2b"] = new[] { "Recipient GSTIN/UIN", "Recipient Name", "Original Document Number", "Original Document Date", "Revised Document Number", "Revised Document Date", "Value of supplies made", "Document type", "Place Of Supply", "Rate", "Taxable Value", "Cess Amount" },
                ["ecoaurp2c"] = new[] { "Financial Year", "Original Month", "Place Of Supply", "Rate", "Taxable Value", "Cess Amount" },
            };

            // ════════════════════════════════════════════════════════════════
            // ROW 1 TITLE + SECTION LAYOUT
            // MainTitle  → always in col A (blue bg, white font)
            // Sections   → (label, startCol, endCol) in same row 1, B onward
            //              "Original details" = origBg, "Revised details" = revBg
            // ════════════════════════════════════════════════════════════════
            var sheetLayout = new Dictionary<string, (string Title, List<(string Label, int From, int To)> Sections)>
            {
                ["b2b,sez,de"] = ("Summary For B2B, SEZ, DE (4A, 4B, 6B, 6C)", new()),
                
                ["b2ba"] = ("Summary For B2BA", new()
                            {
                                ("Original details", 1, 4),               // orange bg, no text, cols 1-4
                                ("Revised Details", 5, 15) // blue bg, merged, cols 5-15
                            }),


                ["b2cl"] = ("Summary For B2CL(5)", new()),

                ["b2cla"] = ("Summary For B2CLA", new() { ("Orignal details", 2, 4), ("Revised Details", 5, 11) }),

                ["b2cs"] = ("Summary For B2CS(7)", new()),
                ["b2csa"] = ("Summary For B2CSA", new() { ("Original details", 2, 2), ("Revised details", 3, 9) }),
                ["cdnr"] = ("Summary For CDNR(9B)", new()),

                ["cdnra"] = ("Summary For CDNRA", new() { ("Original details",2,4), ("Revised details", 5, 15) }),

                ["cdnur"] = ("Summary For CDNUR(9B)", new()),
                ["cdnura"] = ("Summary For CDNURA", new() { ("Original details", 2, 3), ("Revised details", 4, 12) }),
                ["exp"] = ("Summary For EXP(6)", new()),
                ["expa"] = ("Summary For EXPA", new() { ("Original details", 2, 3), ("Revised details", 4, 12) }),
                ["at"] = ("Summary For Advance Received (11B)", new()),
                ["ata"] = ("Summary For Amended Tax Liability (Advance Received)", new() { ("Original details", 2, 3), ("Revised details", 4, 7) }),
                ["atadj"] = ("Summary For Advance Adjusted (11B)", new()),
                ["atadja"] = ("Summary For Amendment Of Adjustment Advances", new() { ("Original details", 2, 3), ("Revised details", 4, 7) }),
                ["exemp"] = ("Summary For Nil rated, exempted and non GST outward supplies (8)", new()),
                ["hsn(b2b)"] = ("Summary For HSN(12)", new()),
                ["hsn(b2c)"] = ("Summary For HSN(12)", new()),
                ["docs"] = ("Summary of documents issued during the tax period (13)", new()),
                ["eco"] = ("Summary For Supplies through ECO-14", new()),
                ["ecoa"] = ("Summary For Amended Supplies through ECO-14A", new() { ("Original details", 2, 3), ("Revised details", 4, 11) }),
                ["ecob2b"] = ("Summary For Supplies U/s 9(5)-15-B2B", new()),
                ["ecourp2b"] = ("Summary For Supplies U/s 9(5)-15-URP2B", new()),
                ["ecob2c"] = ("Summary For Supplies U/s 9(5)-15-B2C", new()),
                ["ecourp2c"] = ("Summary For Supplies U/s 9(5)-15-URP2C", new()),
                ["ecoab2b"] = ("Summary For Supplies U/s 9(5) - 15A-B2B", new() { ("Original details", 2, 6), ("Revised details", 7, 14) }),
                ["ecoab2c"] = ("Summary For Supplies U/s 9(5)-15A-B2C", new() { ("Original details", 2, 4), ("Revised details", 5, 8) }),
                ["ecoaurp2b"] = ("Summary For Supplies U/s 9(5)-15A-URP2B", new() { ("Original details", 2, 4), ("Revised details", 5, 12) }),
                ["ecoaurp2c"] = ("Summary For Supplies U/s 9(5)-15A-URP2C", new() { ("Original details", 2, 2), ("Revised details", 3, 6) }),
            };

            // ════════════════════════════════════════════════════════════════
            // ROW 2 SUMMARY LABELS  (exact text from reference, col → label)
            // These are static labels; row 3 will hold the calculated totals.
            // ════════════════════════════════════════════════════════════════
            var summaryLabels = new Dictionary<string, Dictionary<int, string>>
            {
                ["b2b,sez,de"] = new() { [1] = "No. of Recipients", [3] = "No. of Invoices", [5] = "Total Invoice Value", [12] = "Total Taxable Value", [13] = "Total Cess" },
                ["b2ba"] = new() { [1] = "No. of Recipients", [3] = "No. of Invoices", [7] = "Total Invoice Value", [14] = "Total Taxable Value", [15] = "Total Cess" },
                ["b2cl"] = new() { [1] = "No. of Invoices" },
                ["b2cla"] = new() { [1] = "No. of Invoices", [6] = "Total Inv Value", [9] = "Total Taxable Value", [10] = "Total Cess" },
                ["b2cs"] = new() { [5] = "Total Taxable Value", [6] = "Total Cess" },
                ["b2csa"] = new() { [7] = "Total Taxable Value", [8] = "Total Cess" },
                ["cdnr"] = new() { [1] = "No. of Recipients", [3] = "No. of Notes", [9] = "Total Note Value", [12] = "Total Taxable Value", [13] = "Total Cess" },
                ["cdnra"] = new() { [1] = "No. of Recipients", [3] = "No. of Notes/Vouchers", [11] = "Total Note Value", [14] = "Total Taxable Value", [15] = "Total Cess" },
                ["cdnur"] = new() { [2] = "No. of Notes/Vouchers", [6] = "Total Note Value", [9] = "Total Taxable Value", [10] = "Total Cess" },
                ["cdnura"] = new() { [2] = "No. of Notes/Vouchers", [8] = "Total Note Value", [11] = "Total Taxable Value", [12] = "Total Cess" },
                ["exp"] = new() { [2] = "No. of Invoices", [4] = "Total Invoice Value", [6] = "No. of Shipping Bill", [10] = "Total Taxable Value" },
                ["expa"] = new() { [2] = "No. of Invoices", [6] = "Total Invoice Value", [8] = "No. of Shipping Bill", [11] = "Total Taxable Value", [12] = "Total Cess" },
                ["at"] = new() { [4] = "Total Advance Received", [5] = "Total Cess" },
                ["ata"] = new() { [6] = "Total Advance Received", [7] = "Total Cess" },
                ["atadj"] = new() { [4] = "Total Advance Adjusted", [5] = "Total Cess" },
                ["atadja"] = new() { [6] = "Total Advance Adjusted", [7] = "Total Cess" },
                ["exemp"] = new() { [2] = "Total Nil Rated Supplies", [3] = "Total Exempted Supplies", [4] = "Total Non-GST Supplies" },
                ["hsn(b2b)"] = new() { [1] = "No. of HSN", [5] = "Total Value", [7] = "Total Taxable Value", [8] = "Total Integrated Tax", [9] = "Total Central Tax", [10] = "Total State/UT Tax", [11] = "Total Cess" },
                ["hsn(b2c)"] = new() { [1] = "No. of HSN", [5] = "Total Value", [7] = "Total Taxable Value", [8] = "Total Integrated Tax", [9] = "Total Central Tax", [10] = "Total State/UT Tax", [11] = "Total Cess" },
                ["docs"] = new() { [4] = "Total Number", [5] = "Total Cancelled" },
                ["eco"] = new() { [2] = "No. of E-Commerce Operator", [4] = "Total Net Value of Supplies", [5] = "Total Integrated Tax", [6] = "Total Central Tax", [7] = "Total State/UT Tax", [8] = "Total Cess" },
                ["ecoa"] = new() { [4] = "No. of E-Commerce Operator", [7] = "Total Net Value of Supplies", [8] = "Total Integrated Tax", [9] = "Total Central Tax", [10] = "Total State/UT Tax", [11] = "Total Cess" },
                ["ecob2b"] = new() { [1] = "No. of Supplier", [3] = "No. of Recipients", [5] = "No. of Documents", [7] = "Total value of supplies made", [11] = "Total Taxable Value", [12] = "Total Cess" },
                ["ecourp2b"] = new() { [1] = "No. of Recipients", [3] = "No. of Documents", [5] = "Total value of supplies made", [9] = "Total Taxable Value", [10] = "Total Cess" },
                ["ecob2c"] = new() { [1] = "No. of Supplier", [4] = "Total Taxable Value", [6] = "Total Cess" },
                ["ecourp2c"] = new() { [2] = "Total Taxable Value", [4] = "Total Cess" },
                ["ecoab2b"] = new() { [1] = "No. of Supplier", [3] = "No. of Recipients", [5] = "No. of Documents", [9] = "Total value of supplies made", [13] = "Total Taxable Value", [14] = "Total Cess" },
                ["ecoab2c"] = new() { [3] = "No. of Supplier", [7] = "Total Taxable Value", [8] = "Total Cess" },
                ["ecoaurp2b"] = new() { [1] = "No. of Recipients", [3] = "No. of Documents", [7] = "Total value of supplies made", [11] = "Total Taxable Value", [12] = "Total Cess" },
                ["ecoaurp2c"] = new() { [5] = "Total Taxable Value", [6] = "Total Cess" },
            };

            // ════════════════════════════════════════════════════════════════
            // ROW 3 SUMMED COLUMNS  (which cols get a numeric total in row 3)
            // Only numeric/amount columns — count cols (No. of X) stay blank in row 3.
            // ════════════════════════════════════════════════════════════════
            var sumCols = new Dictionary<string, List<int>>
            {
                ["b2b,sez,de"] = new() { 5, 12, 13 },
                ["b2ba"] = new() { 7, 14, 15 },
                ["b2cl"] = new() { 7, 8 },
                ["b2cla"] = new() { 6, 9, 10 },
                ["b2cs"] = new() { 5, 6 },
                ["b2csa"] = new() { 7, 8 },
                ["cdnr"] = new() { 9, 12, 13 },
                ["cdnra"] = new() { 11, 14, 15 },
                ["cdnur"] = new() { 6, 9, 10 },
                ["cdnura"] = new() { 8, 11, 12 },
                ["exp"] = new() { 4, 10 },
                ["expa"] = new() { 6, 11, 12 },
                ["at"] = new() { 4, 5 },
                ["ata"] = new() { 6, 7 },
                ["atadj"] = new() { 4, 5 },
                ["atadja"] = new() { 6, 7 },
                ["exemp"] = new() { 2, 3, 4 },
                ["hsn(b2b)"] = new() { 5, 7, 8, 9, 10, 11 },
                ["hsn(b2c)"] = new() { 5, 7, 8, 9, 10, 11 },
                ["docs"] = new() { 4, 5 },
                ["eco"] = new() { 4, 5, 6, 7, 8 },
                ["ecoa"] = new() { 7, 8, 9, 10, 11 },
                ["ecob2b"] = new() { 7, 11, 12 },
                ["ecourp2b"] = new() { 5, 9, 10 },
                ["ecob2c"] = new() { 4, 6 },
                ["ecourp2c"] = new() { 2, 4 },
                ["ecoab2b"] = new() { 9, 13, 14 },
                ["ecoab2c"] = new() { 7, 8 },
                ["ecoaurp2b"] = new() { 7, 11, 12 },
                ["ecoaurp2c"] = new() { 5, 6 },
            };

            // ════════════════════════════════════════════════════════════════
            // HELPER: read dynamic column value
            // ════════════════════════════════════════════════════════════════
            string GetCol(dynamic r, int col) => col switch
            {
                1 => r.Col1,
                2 => r.Col2,
                3 => r.Col3,
                4 => r.Col4,
                5 => r.Col5,
                6 => r.Col6,
                7 => r.Col7,
                8 => r.Col8,
                9 => r.Col9,
                10 => r.Col10,
                11 => r.Col11,
                12 => r.Col12,
                13 => r.Col13,
                14 => r.Col14,
                15 => r.Col15,
                _ => null
            };

            // ════════════════════════════════════════════════════════════════
            // SHEET ORDER
            // ════════════════════════════════════════════════════════════════
            var allSheets = new[]
            {
        "b2b,sez,de","b2ba","b2cl","b2cla","b2cs","b2csa",
        "cdnr","cdnra","cdnur","cdnura",
        "exp","expa",
        "at","ata","atadj","atadja",
        "exemp",
        "hsn(b2b)","hsn(b2c)",
        "docs",
        "eco","ecoa",
        "ecob2b","ecourp2b","ecob2c","ecourp2c",
        "ecoab2b","ecoab2c","ecoaurp2b","ecoaurp2c"
    };
            foreach (var sheet in allSheets)
            {
                var ws = wb.Worksheets.Add(sheet);
                var grp = rows.Where(x => x.Sheet_Name == sheet).ToList();

                if (!headers.ContainsKey(sheet)) continue;

                var hdrs = headers[sheet];
                var layout = sheetLayout[sheet];
                int nCols = hdrs.Length;

                // ── ROW 1: Main title in col A ───────────────────────────
                var titleCell = ws.Cell(1, 1);
                titleCell.Value = layout.Title;
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Font.FontColor = XLColor.White;
                titleCell.Style.Fill.BackgroundColor = colorTitleBg;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Row(1).Height = 25;

                // ── ROW 1: "Original details" / "Revised details" sections ─
                foreach (var (label, from, to) in layout.Sections)
                {
                    int safeFrom = Math.Max(2, from);
                    int safeTo = Math.Min(nCols, to);
                    if (safeFrom > safeTo) continue;

                    bool isOriginal = label.StartsWith("Original", StringComparison.OrdinalIgnoreCase);
                    var sectionBg = isOriginal ? colorOrigBg : colorRevBg;

                    if (safeTo > safeFrom)
                        ws.Range(1, safeFrom, 1, safeTo).Merge();

                    var sc = ws.Cell(1, safeFrom);
                    sc.Value = label;
                    sc.Style.Font.Bold = true;
                    sc.Style.Font.FontColor = XLColor.Black;
                    sc.Style.Fill.BackgroundColor = sectionBg;
                    sc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    sc.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    for (int c = safeFrom + 1; c <= safeTo; c++)
                        ws.Cell(1, c).Style.Fill.BackgroundColor = sectionBg;
                }

                // ── ROW 2: Summary labels (blue bg, white text) ──────────
                if (summaryLabels.TryGetValue(sheet, out var row2Labels))
                {
                    var row2Range = ws.Range(2, 1, 2, nCols);
                    row2Range.Style.Fill.BackgroundColor = colorTitleBg;
                    row2Range.Style.Font.Bold = true;
                    row2Range.Style.Font.FontColor = XLColor.White;
                    row2Range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    row2Range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    row2Range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    row2Range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    foreach (var (col, labelText) in row2Labels)
                        ws.Cell(2, col).Value = labelText;
                }

                // ── ROW 3: Numeric totals + "No. of" counts ──────────────
                if (sumCols.TryGetValue(sheet, out var sumColList))
                {
                    var noOfCols = summaryLabels.TryGetValue(sheet, out var labelsForCheck)
                        ? labelsForCheck
                            .Where(kvp => kvp.Value.StartsWith("No. of", StringComparison.OrdinalIgnoreCase))
                            .Select(kvp => kvp.Key)
                            .ToHashSet()
                        : new HashSet<int>();

                    foreach (int col in sumColList)
                    {
                        var cell = ws.Cell(3, col);
                        cell.Style.Font.Bold = true;

                        if (noOfCols.Contains(col))
                        {
                            int count = grp
                                .Select(x => GetCol(x, col))
                                .Where(v => !string.IsNullOrWhiteSpace(v))
                                .Distinct()
                                .Count();

                            cell.Value = count;
                            cell.Style.NumberFormat.Format = "0";
                        }
                        else
                        {
                            decimal total = grp.Sum(x =>
                                decimal.TryParse(GetCol(x, col), out var d) ? d : 0m);

                            cell.Value = total;
                            cell.Style.NumberFormat.Format = "0.00";
                        }
                    }
                }

                // ── ROW 3: "No. of" columns NOT in sumCols ───────────────
                if (summaryLabels.TryGetValue(sheet, out var remainingLabels))
                {
                    var alreadyHandled = sumCols.TryGetValue(sheet, out var handledCols)
                        ? handledCols.ToHashSet()
                        : new HashSet<int>();

                    foreach (var kvp in remainingLabels)
                    {
                        if (kvp.Value.StartsWith("No. of", StringComparison.OrdinalIgnoreCase)
                            && !alreadyHandled.Contains(kvp.Key))
                        {
                            int col = kvp.Key;
                            int count = grp
                                .Select(x => GetCol(x, col))
                                .Where(v => !string.IsNullOrWhiteSpace(v))
                                .Distinct()
                                .Count();

                            var cell = ws.Cell(3, col);
                            cell.Value = count;
                            cell.Style.Font.Bold = true;
                            cell.Style.NumberFormat.Format = "0";
                        }
                    }
                }

                // ── ROW 4: Column headers (orange bg, black text) ─────────
                for (int i = 0; i < nCols; i++)
                    ws.Cell(4, i + 1).Value = hdrs[i];

                var headerRange = ws.Range(4, 1, 4, nCols);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.FontColor = XLColor.Black;
                headerRange.Style.Fill.BackgroundColor = colorHeaderBg;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Alignment.WrapText = true;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Revised-column header background for sheets that have one
                if (layout.Sections.Any(s => s.Label.StartsWith("Revised", StringComparison.OrdinalIgnoreCase)))
                {
                    var revSection = layout.Sections.First(s => s.Label.StartsWith("Revised", StringComparison.OrdinalIgnoreCase));
                    for (int c = revSection.From; c <= Math.Min(revSection.To, nCols); c++)
                        ws.Cell(4, c).Style.Fill.BackgroundColor = colorRevBg;
                }

                // ── ROW 4 height + data rows ──────────────────────────────
                ws.Row(4).Height = 30;

                int dataRow = 6;
                foreach (var r in grp)
                {
                    for (int c = 1; c <= nCols && c <= 15; c++)
                    {
                        string val = GetCol(r, c);
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (decimal.TryParse(val, out var num))
                                ws.Cell(dataRow, c).Value = num;
                            else
                                ws.Cell(dataRow, c).Value = val;
                        }
                    }
                    dataRow++;
                }

                // Ensure row 3 is not empty when no data
                if (!grp.Any())
                    ws.Cell(3, 1).Value = " ";

                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(4);
            }
            //foreach (var sheet in allSheets)
            //{
            //    var ws = wb.Worksheets.Add(sheet);
            //    var grp = rows.Where(x => x.Sheet_Name == sheet).ToList();

            //    if (!headers.ContainsKey(sheet)) continue;

            //    var hdrs = headers[sheet];
            //    var layout = sheetLayout[sheet];
            //    int nCols = hdrs.Length;

            //    // ── ROW 1: Main title in col A ───────────────────────────
            //    // ── ROW 1: ONLY FIRST CELL (A1) ───────────────────────────
            //    var titleCell = ws.Cell(1, 1);
            //    titleCell.Value = layout.Title;
            //    titleCell.Style.Font.Bold = true;
            //    titleCell.Style.Font.FontColor = XLColor.White;
            //    titleCell.Style.Fill.BackgroundColor = colorTitleBg;
            //    titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            //    titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //    ws.Row(1).Height = 25;  

            //    // Fill remaining row-1 cells with blue bg (no text) as base

            //    // ── ROW 1: "Original details" / "Revised details" sections ─
            //    foreach (var (label, from, to) in layout.Sections)
            //    {
            //        int safeFrom = Math.Max(2, from);
            //        int safeTo = Math.Min(nCols, to);
            //        if (safeFrom > safeTo) continue;

            //        bool isOriginal = label.StartsWith("Original", StringComparison.OrdinalIgnoreCase);
            //        var sectionBg = isOriginal ? colorOrigBg : colorRevBg;

            //        if (safeTo > safeFrom)
            //            ws.Range(1, safeFrom, 1, safeTo).Merge();

            //        var sc = ws.Cell(1, safeFrom);
            //        sc.Value = label;
            //        sc.Style.Font.Bold = true;
            //        sc.Style.Font.FontColor = XLColor.Black;
            //        sc.Style.Fill.BackgroundColor = sectionBg;
            //        sc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //        sc.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //        // Apply bg to all cells in the section range
            //        for (int c = safeFrom + 1; c <= safeTo; c++)
            //            ws.Cell(1, c).Style.Fill.BackgroundColor = sectionBg;
            //    }

            //    // ── ROW 2: Summary labels (blue bg, white text) ──────────
            //    if (summaryLabels.TryGetValue(sheet, out var labels))
            //    {
            //        // Apply blue bg to entire row 2
            //        var row2Range = ws.Range(2, 1, 2, nCols);
            //        row2Range.Style.Fill.BackgroundColor = colorTitleBg;
            //        row2Range.Style.Font.Bold = true;
            //        row2Range.Style.Font.FontColor = XLColor.White;
            //        row2Range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //        row2Range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            //        row2Range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //        row2Range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            //        // Write the label text into the correct columns
            //        foreach (var (col, labelText) in labels)
            //            ws.Cell(2, col).Value = labelText;
            //    }

            //    // ── ROW 3: Numeric totals ─────────────────────────────────
            //    if (sumCols.TryGetValue(sheet, out var cols))
            //    {
            //        foreach (int col in cols)
            //        {
            //            decimal total = grp.Sum(x =>
            //                decimal.TryParse(GetCol(x, col), out var d) ? d : 0m);

            //            var cell = ws.Cell(3, col);
            //            cell.Value = total;
            //            cell.Style.Font.Bold = true;
            //            cell.Style.NumberFormat.Format = "0.00";
            //        }
            //    }// ── ROW 3: Numeric totals ─────────────────────────────────
            //    if (sumCols.TryGetValue(sheet, out var cols))
            //    {
            //        foreach (int col in cols)
            //        {
            //            decimal total = grp.Sum(x =>
            //                decimal.TryParse(GetCol(x, col), out var d) ? d : 0m);

            //            var cell = ws.Cell(3, col);
            //            cell.Value = total;
            //            cell.Style.Font.Bold = true;
            //            cell.Style.NumberFormat.Format = "0.00";
            //        }
            //    }
            //    // ── ROW 4: Column headers (orange bg, black text) ─────────
            //    for (int i = 0; i < nCols; i++)
            //        ws.Cell(4, i + 1).Value = hdrs[i];

            //    var headerRange = ws.Range(4, 1, 4, nCols);
            //    headerRange.Style.Font.Bold = true;
            //    headerRange.Style.Font.FontColor = XLColor.Black;
            //    headerRange.Style.Fill.BackgroundColor = colorHeaderBg;
            //    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //    headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            //    headerRange.Style.Alignment.WrapText = true;
            //    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //    headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            //    // Revised-column header background (#93ADDD) for sheets that have one
            //    if (layout.Sections.Any(s => s.Label.StartsWith("Revised", StringComparison.OrdinalIgnoreCase)))
            //    {
            //        var revSection = layout.Sections.First(s => s.Label.StartsWith("Revised", StringComparison.OrdinalIgnoreCase));
            //        for (int c = revSection.From; c <= Math.Min(revSection.To, nCols); c++)
            //            ws.Cell(4, c).Style.Fill.BackgroundColor = colorRevBg;
            //    }

            //    // ── ROW 5: empty spacer (row 6 onward = data) ────────────
            //    ws.Row(4).Height = 30;

            //    // ── ROWS 6+: Data ─────────────────────────────────────────
            //    int dataRow = 6;
            //    foreach (var r in grp)
            //    {
            //        for (int c = 1; c <= nCols && c <= 15; c++)
            //        {
            //            string val = GetCol(r, c);
            //            if (!string.IsNullOrEmpty(val))
            //            {
            //                if (decimal.TryParse(val, out var num))
            //                    ws.Cell(dataRow, c).Value = num;
            //                else
            //                    ws.Cell(dataRow, c).Value = val;
            //            }
            //        }
            //        dataRow++;
            //    }

            //    // Ensure row 3 is not empty when no data
            //    if (!grp.Any())
            //        ws.Cell(3, 1).Value = " ";

            //    ws.Columns().AdjustToContents();
            //    ws.SheetView.FreezeRows(4);
            //}

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        
    }
}

