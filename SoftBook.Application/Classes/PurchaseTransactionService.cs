using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PurchaseTransactionService : IPurchaseTransactionService
    {
        private readonly ISalePurRepository _salePurRepository;
        private readonly IJournalService _journalService;

        public PurchaseTransactionService(ISalePurRepository salePurRepository, IJournalService journalService)
        {
            _salePurRepository = salePurRepository;
            _journalService = journalService;
        }

        public async Task<List<SalePur>> GetAllAsync(int companyId)
        {
            var all = await _salePurRepository.GetAllAsync(companyId);
            // Filter to only Purchase type records
            return all.Where(x => string.Equals(x.InvType, "Purchase", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<PurchaseTransactionViewModel?> GetByIdAsync(int companyId, int spUniqueCode)
        {
            var header = await _salePurRepository.GetByIdAsync(companyId, spUniqueCode);
            if (header == null) return null;

            var lines = await _salePurRepository.GetLineItemsAsync(spUniqueCode);

            return new PurchaseTransactionViewModel
            {
                Header = header,
                Lines = lines
            };
        }

        public async Task<DbResult> SaveCompositeAsync(PurchaseTransactionViewModel viewModel)
        {
            DbResult result;
            bool isNew = viewModel.Header.SpUniqueCode <= 0;
            var fiscalYearId = viewModel.Header.FiscalYearId;
            var branchId = viewModel.Header.BranchId;

            // Enforce Purchase identity and category similar to Sales
            viewModel.Header.InvType = "Purchase";
            viewModel.Header.InvCategory = "Purchase Order";

            if (!isNew)
            {
                // ✅ Save old sp_unique_code for JV cleanup BEFORE re-insert changes it
                var oldSpUniqueCode = viewModel.Header.SpUniqueCode;

                // ✅ AUDIT TRAIL: UpdateAsync now soft-deletes the old header + its lines,
                // then inserts the edited data as a NEW record, returning the fresh sp_unique_code.
                result = await _salePurRepository.UpdateAsync(viewModel.Header);
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
                result = await _salePurRepository.InsertAsync(viewModel.Header);
                if (!result.Success) return result;
                
                if (result.Data is SalePur insertedHeader)
                {
                    viewModel.Header.SpUniqueCode = insertedHeader.SpUniqueCode;
                    foreach(var line in viewModel.Lines)
                    {
                        line.SpUniqueCode = insertedHeader.SpUniqueCode;
                        line.CompanyId = viewModel.Header.CompanyId;
                        line.FiscalYearId = fiscalYearId;
                        line.BranchId = branchId;
                    }
                }
                else
                {
                    return DbResult.Failure("Failed to retrieve the new record ID after insert.");
                }

                // For NEW records, just insert all lines
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

            // Generate Accounting Entries in tab_jv (tab_jv handles its own versioning — no change needed)
            var journalResult = await _journalService.GenerateJournalFromPurchaseAsync(viewModel.Header, viewModel.Lines);
            if (!journalResult.Success)
            {
                return DbResult.Failure($"Accounting Entry Error: {journalResult.Error}");
            }

            return DbResult.Ok(viewModel);
        }


        public async Task<DbResult> DeleteAsync(int companyId, int spUniqueCode)
        {
            await _journalService.DeleteJournalByGCodeAsync(spUniqueCode.ToString());
            return await _salePurRepository.DeleteAsync(companyId, spUniqueCode);
        }
    }
}
