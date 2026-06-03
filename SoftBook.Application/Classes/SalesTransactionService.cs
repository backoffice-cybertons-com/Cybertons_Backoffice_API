using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Application.Services.Classes
{
    public class SalesTransactionService : ISalesTransactionService
    {
        private readonly ISalePurRepository _salePurRepository;
        private readonly IJournalService _journalService;
        private readonly IProductRepository _productRepository;

        public SalesTransactionService(ISalePurRepository salePurRepository, IJournalService journalService, IProductRepository productRepository)
        {
            _salePurRepository = salePurRepository;
            _journalService = journalService;
            _productRepository = productRepository;
        }

        public async Task<List<SalePur>> GetAllAsync(int companyId)
        {
            var allRecords = await _salePurRepository.GetAllAsync(companyId);
            // Filter strictly for Sales documents (Sales Order, Sales Invoice, etc.)
            return allRecords.Where(r => r.InvType == "Sales").ToList();
        }

		//public async Task<SalesTransactionViewModel?> GetByIdAsync(int companyId, int spUniqueCode)
		//{
		//    var header = await _salePurRepository.GetByIdAsync(companyId, spUniqueCode);
		//    if (header == null || header.InvType != "Sales") return null;

		//    var lines = await _salePurRepository.GetLineItemsAsync(spUniqueCode);

		//    return new SalesTransactionViewModel
		//    {
		//        Header = header,
		//        Lines = lines
		//    };
		//}

		public async Task<SalesTransactionViewModel?> GetByIdAsync(int companyId, int spUniqueCode)
		{
			var header = await _salePurRepository.GetByIdAsync(companyId, spUniqueCode);
			if (header == null || header.InvType != "Sales") return null;

			var lines = await _salePurRepository.GetLineItemsAsync(spUniqueCode);

			// Compute Previous Outstanding LIVE from tab_jv:
			// Net balance for this party strictly BEFORE the invoice date.
			// This is safe regardless of which module (PayRec / Income / JV) was used for payments.
			header.PreviousOutstanding = await _salePurRepository.GetPartyOutstandingBeforeDateAsync(
				companyId, header.PartyId, header.InvDate.Date);

			// Compute Today's Receipt from tab_jv on the same invoice date
			header.TodaysReceipt = await _salePurRepository.GetTodaysReceiptAsync(
				companyId, header.PartyId, header.InvDate.Date);

			// Compute Today's Invoice total for this customer on this date
			header.TodaysInvoice = await _salePurRepository.GetTodaysInvoiceAmountAsync(
				companyId, header.PartyId, header.InvDate.Date);

			return new SalesTransactionViewModel
			{
				Header = header,
				Lines = lines
			};
		}


		public async Task<DbResult> SaveCompositeAsync(SalesTransactionViewModel viewModel, int? userId = null)
        {
            DbResult result;
            bool isNew = viewModel.Header.SpUniqueCode <= 0;
            var fiscalYearId = viewModel.Header.FiscalYearId;
            var branchId = viewModel.Header.BranchId;

            // Enforce Sales identity regardless of what frontend sends
            viewModel.Header.InvType = "Sales";
            viewModel.Header.InvCategory = "Sales Order";
            viewModel.Header.IgstPercent = 0;
            viewModel.Header.CgstPercent = 0;
            viewModel.Header.SgstPercent = 0;

            // Double Validation: Recalculate overall header totals based on lines
            viewModel.Header.RateDiff = viewModel.Lines.Sum(l => l.RateDiff ?? 0);
            viewModel.Header.TotalQuantity = viewModel.Lines.Sum(l => l.Quantity);
            viewModel.Header.InvoiceTotal = viewModel.Lines.Sum(l => l.Amount);
            viewModel.Header.FinalInvoiceAmount = viewModel.Header.GrandTotal;

            // Snapshot Cost Price for each line item from the Product Master
            foreach (var line in viewModel.Lines)
            {
                if (line.ProductId.HasValue)
                {
                    var product = await _productRepository.GetByIdAsync(viewModel.Header.CompanyId, line.ProductId.Value);
                    if (product != null)
                    {
                        line.Cogs = product.CostPrice;
                    }
                }
            }

            // changes made by Hetak dt 20052026 - Sales header, lines, and JV now save in one transaction.
            return await _salePurRepository.ExecuteInTransactionAsync(async transaction =>
            {
                if (!isNew)
                {
                // [MODIFIED 2026-05-06] Stable ID Implementation:
                // We no longer soft-delete the header because changing sp_unique_code breaks payment links in tab_jv.
                // We now:
                // 1. Soft-delete ONLY the line items (tab_sal_pur_tran).
                // 2. Perform a direct UPDATE on the header row in tab_sale_pur.
                // 3. Re-insert fresh line items and journal entries linked to the SAME sp_unique_code.

                var currentSpUniqueCode = viewModel.Header.SpUniqueCode;

                /* OLD LOGIC (SOFT DELETE HEADER + INSERT NEW):
                var oldSpUniqueCode = viewModel.Header.SpUniqueCode;
                result = await _salePurRepository.UpdateAsync(viewModel.Header, userId);
                if (!result.Success) return result;
                if (result.Data is SalePur updatedHeader) {
                    viewModel.Header.SpUniqueCode = updatedHeader.SpUniqueCode;
                } else {
                    return DbResult.Failure("Failed to retrieve the new record ID after re-insert on edit.");
                }
                */

                // 1. Soft-delete OLD line items first (tab_sal_pur_tran)
                // This ensures we can insert new lines without having duplicates under the same header ID.
                var deleteLinesResult = await _salePurRepository.DeleteLineItemsByParentAsync(currentSpUniqueCode, userId, transaction);
                if (!deleteLinesResult.Success) return deleteLinesResult;

                // 2. Perform Direct Update on the Header (tab_sale_pur)
                // This preserves sp_unique_code and updates modified_on/modified_by.
                result = await _salePurRepository.UpdateAsync(viewModel.Header, userId, transaction);
                if (!result.Success) return result;

                // 3. Insert New Line Items using the same ID
                foreach (var line in viewModel.Lines)
                {
                    line.SpTranUniqueCode = 0; // force INSERT — old line PKs are invalidated
                    line.SpUniqueCode = currentSpUniqueCode; // Use the SAME stable ID
                    line.CompanyId = viewModel.Header.CompanyId;
                    line.FiscalYearId = fiscalYearId;
                    line.BranchId = branchId;
                    var insResult = await _salePurRepository.InsertLineItemAsync(line, transaction);
                    if (!insResult.Success) return insResult;
                }
                }
                else
                {
                result = await _salePurRepository.InsertWithNextInvoiceNumberAsync(viewModel.Header, userId, transaction);
                if (!result.Success) return result;
                
                if (result.Data is SalePur insertedHeader)
                {
                    viewModel.Header.SpUniqueCode = insertedHeader.SpUniqueCode;
                    viewModel.Header.InvNo = insertedHeader.InvNo;
                    viewModel.Header.InvoiceSeries = insertedHeader.InvoiceSeries;
                    viewModel.Header.InvoiceSeriesIncrementNumber = insertedHeader.InvoiceSeriesIncrementNumber;
                    foreach(var line in viewModel.Lines)
                    {
                        line.SpUniqueCode = insertedHeader.SpUniqueCode;
                        line.CompanyId = viewModel.Header.CompanyId;
                        // Ensure line rows also carry the same fiscal year/branch
                        line.FiscalYearId = fiscalYearId;
                        line.BranchId = branchId;
                    }
                }
                else
                {
                    return DbResult.Failure("Failed to retrieve the new record ID after insert.");
                }

                foreach (var line in viewModel.Lines)
                {
                    line.SpUniqueCode = viewModel.Header.SpUniqueCode;
                    line.CompanyId = viewModel.Header.CompanyId;
                    line.FiscalYearId = fiscalYearId;
                    line.BranchId = branchId;
                    var insResult = await _salePurRepository.InsertLineItemAsync(line, transaction);
                    if (!insResult.Success) return insResult;
                }
            }

                var journalResult = await _journalService.GenerateJournalFromSaleAsync(viewModel.Header, viewModel.Lines, transaction);
                if (!journalResult.Success)
                {
                    return DbResult.Failure($"Accounting Entry Error: {journalResult.Error}");
                }

                return DbResult.Ok(viewModel);
            });
        }


        public async Task<DbResult> DeleteAsync(int companyId, int spUniqueCode, int? userId = null)
        {
            var header = await _salePurRepository.GetByIdAsync(companyId, spUniqueCode);
            if (header != null && header.InvType != "Sales") 
            {
                return DbResult.Failure("Cannot delete non-sales transaction using this service.");
            }
            await _journalService.DeleteJournalByGCodeAsync(spUniqueCode.ToString());
            return await _salePurRepository.DeleteAsync(companyId, spUniqueCode, userId);
        }

        public async Task<decimal> GetPartyOutstandingAsync(int companyId, int partyId)
        {
            return await _salePurRepository.GetPartyOutstandingAsync(companyId, partyId);
        }
    }
}
