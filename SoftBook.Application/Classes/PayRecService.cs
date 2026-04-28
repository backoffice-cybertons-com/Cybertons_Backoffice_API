using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PayRecService : IPayRecService
    {
        private readonly IPayRecRepository _payRecRepository;
        private readonly IJournalService _journalService;
        private readonly IJournalRepository _journalRepository;

        public PayRecService(IPayRecRepository payRecRepository, IJournalService journalService, IJournalRepository journalRepository)
        {
            _payRecRepository = payRecRepository;
            _journalService = journalService;
            _journalRepository = journalRepository;
        }

        public async Task<IEnumerable<dynamic>> GetPaymentListAsync(long companyId, int pageNo, int pageSize, string? vType, int? partyId)
        {
            return await _payRecRepository.GetPaymentListAsync(companyId, pageNo, pageSize, vType, partyId);
        }

        public async Task<IEnumerable<dynamic>> GetPendingInvoicesAsync(string trnType, int partyId)
        {
            return await _payRecRepository.GetPendingInvoicesAsync(trnType, partyId);
        }

        public async Task<List<PayRec>> GetByIdAsync(long companyId, long code)
        {
            return await _payRecRepository.GetByIdAsync(companyId, code);
        }

        public async Task<DbResult> SaveCompositeAsync(IEnumerable<PayRec> entities, int? userId = null)
        {
            if (entities == null || !entities.Any())
                return DbResult.Failure("No data to save.");

            var primary = entities.First();
            bool isNew = string.IsNullOrEmpty(primary.GCode);
            string gCode = isNew ? "PR-" + DateTime.Now.Ticks : primary.GCode;
            
            // Use the connection string from the provider if available
            // BaseDAL has GetConnectionString() but it's protected.
            // We can use NpgsqlConnection with the repo's connection if we expose it,
            // but for simplicity in this architecture, we'll open a connection here.
            
            // In a real app, we'd use a UnitOfWork. 
            // Here, we'll manually manage the transaction to ensure atomicity.
            
            using var connection = new Npgsql.NpgsqlConnection(_payRecRepository is SoftBook.Infrastructure.Classes.PayRecRepository repo ? repo.GetConnectionStringForService() : "");
            // Wait, I need to expose GetConnectionString or use a shared context. 
            // I'll check if I can add a helper to PayRecRepository.
            
            // Let's assume I can get the connection string.
            // I'll add a temporary public method to PayRecRepository to get the connection string.
            
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 1. Soft Delete existing records if updating
                if (!isNew)
                {
                    await _payRecRepository.DeleteByGCodeAsync(gCode, primary.CreatedBy ?? "System", transaction, userId);
                }

                // 2. Generate and Save Journal Vouchers
                // Note: GenerateJournalFromPayRecAsync now takes IEnumerable and calls SaveJournalGroupAsync
                // We need to pass the transaction to it.
                // I'll update IJournalService to accept transaction too.
                
                foreach (var entity in entities)
                {
                    entity.GCode = gCode;
                    entity.Date = primary.Date;
                    entity.VchType = primary.VchType;
                    entity.AccountId = primary.AccountId;
                    entity.PartyId = primary.PartyId;
                    entity.CreatedBy = primary.CreatedBy;
                    entity.CompanyId = primary.CompanyId;
                    entity.YearCode = primary.YearCode;
                    entity.BranchCode = primary.BranchCode;
                }

                // Since IJournalService doesn't take transaction yet, I'll update it.
                // Or I can call SaveJournalGroupAsync directly from here if I have the entries.
                // But GenerateJournalFromPayRecAsync is where the logic is.
                
                var journalResult = await _journalService.GenerateJournalFromPayRecAsync(entities, transaction);
                // Wait, GenerateJournalFromPayRecAsync creates its own transaction in SaveJournalGroupAsync 
                // UNLESS I pass the transaction. I already added IDbTransaction to SaveJournalGroupAsync.
                // I should also add it to IJournalService.GenerateJournalFromPayRecAsync.
                
                if (!journalResult.Success)
                {
                    await transaction.RollbackAsync();
                    return journalResult;
                }

                // 3. Insert fresh PayRec records
                foreach (var entity in entities)
                {
                    // Update entity with JvCode if needed? 
                    // Actually, GCode is the link.
                    var insResult = await _payRecRepository.InsertAsync(entity, transaction, userId);
                    if (!insResult.Success)
                    {
                        await transaction.RollbackAsync();
                        return insResult;
                    }
                }

                await transaction.CommitAsync();
                return DbResult.Ok(gCode);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return DbResult.Failure($"Failed to save Payment/Receipt: {ex.Message}");
            }
        }

        public async Task<DbResult> DeleteAsync(long companyId, long code, string deletedBy, int? userId = null)
        {
            var allocations = await _payRecRepository.GetByIdAsync(companyId, code);
            var primaryAllocation = allocations?.FirstOrDefault();
            if (primaryAllocation != null && !string.IsNullOrEmpty(primaryAllocation.GCode))
            {
                await _journalService.DeleteJournalByGCodeAsync(primaryAllocation.GCode);
            }
            return await _payRecRepository.DeleteAsync(companyId, code, deletedBy, userId);
        }
    }
}
