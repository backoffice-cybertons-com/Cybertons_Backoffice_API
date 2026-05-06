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

            // Double Validation: Recalculate overall header totals based on lines
            viewModel.Header.TotalQuantity = viewModel.Lines.Sum(l => l.Quantity);
            viewModel.Header.InvoiceTotal = viewModel.Lines.Sum(l => l.Amount);
            viewModel.Header.FinalInvoiceAmount = viewModel.Header.GrandTotal;

            if (!isNew)
            {
                // [MODIFIED 2026-05-06] Stable ID Implementation:
                // We no longer soft-delete the header because changing sp_unique_code breaks payment links in tab_jv.
                // We now:
                // 1. Soft-delete ONLY the line items (tab_sal_pur_tran) and journal entries (tab_jv).
                // 2. Perform a direct UPDATE on the header row in tab_sale_pur.
                // 3. Re-insert fresh line items and journal entries linked to the SAME sp_unique_code.

                var currentSpUniqueCode = viewModel.Header.SpUniqueCode;

                /* OLD LOGIC (SOFT DELETE HEADER + INSERT NEW):
                var oldSpUniqueCode = viewModel.Header.SpUniqueCode;
                result = await _salePurRepository.UpdateAsync(viewModel.Header);
                if (!result.Success) return result;
                if (result.Data is SalePur updatedHeader) {
                    viewModel.Header.SpUniqueCode = updatedHeader.SpUniqueCode;
                } else {
                    return DbResult.Failure("Failed to retrieve the new record ID after re-insert on edit.");
                }
                */

                // 1. Soft-delete OLD line items first (tab_sal_pur_tran)
                // This ensures we can insert new lines without having duplicates under the same header ID.
                var deleteLinesResult = await _salePurRepository.DeleteLineItemsByParentAsync(currentSpUniqueCode);
                if (!deleteLinesResult.Success) return deleteLinesResult;

                // 2. Perform Direct Update on the Header (tab_sale_pur)
                // This preserves sp_unique_code and updates modified_on/modified_by.
                result = await _salePurRepository.UpdateAsync(viewModel.Header);
                if (!result.Success) return result;

                // 3. Delete OLD journal entries linked to this ID (tab_jv)
                // We wipe the old accounting lines and GenerateJournalFromPurchaseAsync will create fresh ones.
                await _journalService.DeleteJournalByGCodeAsync(currentSpUniqueCode.ToString());

                // 4. Insert New Line Items using the same ID
                foreach (var line in viewModel.Lines)
                {
                    line.SpTranUniqueCode = 0; // force INSERT — old line PKs are invalidated
                    line.SpUniqueCode = currentSpUniqueCode; // Use the SAME stable ID
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
