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

            // Calculate overall header RateDiff based on lines
            viewModel.Header.RateDiff = viewModel.Lines.Sum(l => l.RateDiff ?? 0);

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

            if (!isNew)
            {
                // ✅ Save old sp_unique_code for JV cleanup BEFORE re-insert changes it
                var oldSpUniqueCode = viewModel.Header.SpUniqueCode;

                // ✅ AUDIT TRAIL: UpdateAsync now soft-deletes the old header + its lines,
                // then inserts the edited data as a NEW record, returning the fresh sp_unique_code.
                result = await _salePurRepository.UpdateAsync(viewModel.Header, userId);
                if (!result.Success) return result;

                if (result.Data is SalePur updatedHeader)
                {
                    // Use the NEW sp_unique_code for all line insertions
                    viewModel.Header.SpUniqueCode = updatedHeader.SpUniqueCode;
                }
                else
                {
                    return DbResult.Failure("Failed to retrieve the new record ID after re-insert on edit.");
                }

                // ✅ Delete OLD journal entries by the old GCode before generating new ones
                await _journalService.DeleteJournalByGCodeAsync(oldSpUniqueCode.ToString());

                // Insert all lines fresh against the new sp_unique_code
                // (Old lines were already soft-deleted by the stored proc)
                foreach (var line in viewModel.Lines)
                {
                    line.SpTranUniqueCode = 0; // force INSERT — old PKs are now invalidated
                    line.SpUniqueCode = viewModel.Header.SpUniqueCode;
                    line.CompanyId = viewModel.Header.CompanyId;
                    line.FiscalYearId = fiscalYearId;
                    line.BranchId = branchId;
                    var insResult = await _salePurRepository.InsertLineItemAsync(line);
                    if (!insResult.Success) return insResult;
                }
            }
            else
            {
                result = await _salePurRepository.InsertAsync(viewModel.Header, userId);
                if (!result.Success) return result;
                
                if (result.Data is SalePur insertedHeader)
                {
                    viewModel.Header.SpUniqueCode = insertedHeader.SpUniqueCode;
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
                    var insResult = await _salePurRepository.InsertLineItemAsync(line);
                    if (!insResult.Success) return insResult;
                }
            }

            var journalResult = await _journalService.GenerateJournalFromSaleAsync(viewModel.Header, viewModel.Lines);
            if (!journalResult.Success)
            {
                return DbResult.Failure($"Accounting Entry Error: {journalResult.Error}");
            }

            return DbResult.Ok(viewModel);
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
