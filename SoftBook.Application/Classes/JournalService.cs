using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System.Text.Json;

namespace SoftBook.Application.Services.Classes
{
    public class JournalService : IJournalService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly IMasterRepository _masterRepository;

        public JournalService(IJournalRepository journalRepository, IMasterRepository masterRepository)
        {
            _journalRepository = journalRepository;
            _masterRepository = masterRepository;
        }

        //public async Task<DbResult> GenerateJournalFromSaleAsync(SalePur header, IEnumerable<SalePurTran> lines)
        //{
        //    try
        //    {
        //        Console.WriteLine($"[JournalService] Generating journal for Sale: {header.InvNo}, Company: {header.CompanyId}");
        //        Console.WriteLine($"[JournalService] GenerateJournalFromSaleAsync: InvNo={header.InvNo}, CompanyId={header.CompanyId}, TaxableAmount={header.TaxableAmount}");
        //        var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, header.CompanyId);
        //        var settings = settingsList.FirstOrDefault(); 

        //        if (settings == null) 
        //        {
        //            Console.WriteLine($"[JournalService] ERROR: Ledger settings not found for CompanyId={header.CompanyId}");
        //            return DbResult.Failure("Ledger settings not configured.");
        //        }

        //        var settingsDict = (IDictionary<string, object>)settings;
        //        var salesAccountId = MapLongValue(settingsDict, "SalesRevenueAccountId");
        //        var cgstOutputAccountId = MapLongValue(settingsDict, "CgstOutputAccountId");
        //        var sgstOutputAccountId = MapLongValue(settingsDict, "SgstOutputAccountId");
        //        var igstOutputAccountId = MapLongValue(settingsDict, "IgstOutputAccountId");
        //        var roundOffAccountId = MapLongValue(settingsDict, "RoundOffAccountId");

        //        if (salesAccountId == null)
        //        {
        //            Console.WriteLine($"[JournalService] ERROR: Sales Revenue Account not mapped for Company: {header.CompanyId}. Check Ledger Settings.");
        //            return DbResult.Failure("Sales Revenue Account not mapped.");
        //        }

        //        Console.WriteLine($"[JournalService] Mapping Success: Sales:{salesAccountId}, CGST:{cgstOutputAccountId}, SGST:{sgstOutputAccountId}, IGST:{igstOutputAccountId}, RoundOff:{roundOffAccountId}");

        //        var entries = new List<JournalVoucher>();

        //        // Entry 1: DR Party, CR Sales
        //        if (header.TaxableAmount > 0)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = header.PartyId,
        //                CrAccountCode = salesAccountId.Value,
        //                Amount = header.TaxableAmount.Value,
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"Sales Taxable Value - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // CGST
        //        if (header.CgstAmount > 0 && cgstOutputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = header.PartyId,
        //                CrAccountCode = cgstOutputAccountId.Value,
        //                Amount = header.CgstAmount.Value,
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"CGST Output - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // SGST
        //        if (header.SgstAmount > 0 && sgstOutputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = header.PartyId,
        //                CrAccountCode = sgstOutputAccountId.Value,
        //                Amount = header.SgstAmount.Value,
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"SGST Output - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // IGST
        //        if (header.IgstAmount > 0 && igstOutputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = header.PartyId,
        //                CrAccountCode = igstOutputAccountId.Value,
        //                Amount = header.IgstAmount.Value,
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"IGST Output - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // Round Off
        //        if (header.RoundOff != 0 && roundOffAccountId != null)
        //        {
        //            bool isDrParty = header.RoundOff > 0;
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = isDrParty ? header.PartyId : roundOffAccountId.Value,
        //                CrAccountCode = isDrParty ? roundOffAccountId.Value : header.PartyId,
        //                Amount = Math.Abs(header.RoundOff.Value),
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"Round Off - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // Assign Group Serial Numbers
        //        for (int i = 0; i < entries.Count; i++)
        //        {
        //            entries[i].GroupJvSrNo = i + 1;
        //        }

        //        // Atomic Save: Delete by GCode and Insert new entries in one transaction
        //        return await _journalRepository.SaveJournalGroupAsync(entries, header.SpUniqueCode.ToString(), null, null);
        //    }
        //}

        //public async Task<DbResult> GenerateJournalFromSaleAsync(SalePur header, IEnumerable<SalePurTran> lines)
        //{
        //    try
        //    {
        //        Console.WriteLine($"[JournalService] Generating journal for Sale: {header.InvNo}, Company: {header.CompanyId}");
        //        Console.WriteLine($"[JournalService] GenerateJournalFromSaleAsync: InvNo={header.InvNo}, CompanyId={header.CompanyId}, TaxableAmount={header.TaxableAmount}");
        //        var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, header.CompanyId);
        //        var settings = settingsList.FirstOrDefault(); 

        //        if (settings == null) 
        //        {
        //            Console.WriteLine($"[JournalService] ERROR: Ledger settings not found for CompanyId={header.CompanyId}");
        //            return DbResult.Failure("Ledger settings not configured.");
        //        }

        //        var settingsDict = (IDictionary<string, object>)settings;
        //        var salesAccountId = MapLongValue(settingsDict, "SalesRevenueAccountId");
        //        var cgstOutputAccountId = MapLongValue(settingsDict, "CgstOutputAccountId");
        //        var sgstOutputAccountId = MapLongValue(settingsDict, "SgstOutputAccountId");
        //        var igstOutputAccountId = MapLongValue(settingsDict, "IgstOutputAccountId");
        //        var roundOffAccountId = MapLongValue(settingsDict, "RoundOffAccountId");

        //        if (salesAccountId == null)
        //        {
        //            Console.WriteLine($"[JournalService] ERROR: Sales Revenue Account not mapped for Company: {header.CompanyId}. Check Ledger Settings.");
        //            return DbResult.Failure("Sales Revenue Account not mapped.");
        //        }

        //        Console.WriteLine($"[JournalService] Mapping Success: Sales:{salesAccountId}, CGST:{cgstOutputAccountId}, SGST:{sgstOutputAccountId}, IGST:{igstOutputAccountId}, RoundOff:{roundOffAccountId}");

        //        var entries = new List<JournalVoucher>();

        //        // Entry 1: DR Party, CR Sales
        //        if (header.TaxableAmount > 0)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = header.PartyId,
        //                CrAccountCode = salesAccountId.Value,
        //                Amount = header.TaxableAmount.Value,
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"Sales Taxable Value - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // CGST
        //        if (header.CgstAmount > 0 && cgstOutputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = header.PartyId,
        //                CrAccountCode = cgstOutputAccountId.Value,
        //                Amount = header.CgstAmount.Value,
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"CGST Output - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // SGST
        //        if (header.SgstAmount > 0 && sgstOutputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = header.PartyId,
        //                CrAccountCode = sgstOutputAccountId.Value,
        //                Amount = header.SgstAmount.Value,
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"SGST Output - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // IGST
        //        if (header.IgstAmount > 0 && igstOutputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = header.PartyId,
        //                CrAccountCode = igstOutputAccountId.Value,
        //                Amount = header.IgstAmount.Value,
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"IGST Output - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // Round Off
        //        if (header.RoundOff != 0 && roundOffAccountId != null)
        //        {
        //            bool isDrParty = header.RoundOff > 0;
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = isDrParty ? header.PartyId : roundOffAccountId.Value,
        //                CrAccountCode = isDrParty ? roundOffAccountId.Value : header.PartyId,
        //                Amount = Math.Abs(header.RoundOff.Value),
        //                VType = "Sales",
        //                VCode = header.SpUniqueCode,
        //                Description = $"Round Off - Inv {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // Assign Group Serial Numbers
        //        for (int i = 0; i < entries.Count; i++)
        //        {
        //            entries[i].GroupJvSrNo = i + 1;
        //        }

        //        // Atomic Save: Delete by GCode and Insert new entries in one transaction
        //        return await _journalRepository.SaveJournalGroupAsync(entries, header.SpUniqueCode.ToString(), null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return DbResult.Failure(ex.Message);
        //    }
        //}

        //public async Task<DbResult> GenerateJournalFromSaleAsync(SalePur header, IEnumerable<SalePurTran> lines)
        //{
        //    try
        //    {
        //        var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, header.CompanyId);
        //        var settings = settingsList.FirstOrDefault();
        //        if (settings == null) return DbResult.Failure("Ledger settings not configured.");

        //        var settingsDict = (IDictionary<string, object>)settings;
        //        var salesAccountId = MapLongValue(settingsDict, "SalesRevenueAccountId");
        //        var cgstOutputAccountId = MapLongValue(settingsDict, "CgstOutputAccountId");
        //        var sgstOutputAccountId = MapLongValue(settingsDict, "SgstOutputAccountId");
        //        var igstOutputAccountId = MapLongValue(settingsDict, "IgstOutputAccountId");
        //        var roundOffAccountId = MapLongValue(settingsDict, "RoundOffAccountId");
        //        var discountAccountId = MapLongValue(settingsDict, "DiscountAllowedAccountId"); // Need this for sales discount

        //        if (salesAccountId == null) return DbResult.Failure("Sales Revenue Account not mapped.");

        //        var entries = new List<JournalVoucher>();

        //        // Double-Entry Balanced Entries for Sales

        //        // 1. Taxable Amount (Dr: Party, Cr: Sales)
        //        if (header.TaxableAmount > 0)
        //        {
        //            entries.Add(CreateJVEntry(header, header.PartyId, salesAccountId.Value, header.TaxableAmount.Value, "Sales", $"Sales Taxable Value - Inv {header.InvNo}"));
        //        }

        //        // 2. CGST (Dr: Party, Cr: CGST)
        //        if (header.CgstAmount > 0 && cgstOutputAccountId != null)
        //        {
        //            entries.Add(CreateJVEntry(header, header.PartyId, cgstOutputAccountId.Value, header.CgstAmount.Value, "Sales", $"CGST Output - Inv {header.InvNo}"));
        //        }

        //        // 3. SGST (Dr: Party, Cr: SGST)
        //        if (header.SgstAmount > 0 && sgstOutputAccountId != null)
        //        {
        //            entries.Add(CreateJVEntry(header, header.PartyId, sgstOutputAccountId.Value, header.SgstAmount.Value, "Sales", $"SGST Output - Inv {header.InvNo}"));
        //        }

        //        // 4. IGST (Dr: Party, Cr: IGST)
        //        if (header.IgstAmount > 0 && igstOutputAccountId != null)
        //        {
        //            entries.Add(CreateJVEntry(header, header.PartyId, igstOutputAccountId.Value, header.IgstAmount.Value, "Sales", $"IGST Output - Inv {header.InvNo}"));
        //        }

        //        // 5. Discount (Dr: Discount Allowed, Cr: Party)
        //        if (header.TotDiscount != 0 && discountAccountId != null)
        //        {
        //            entries.Add(CreateJVEntry(header, discountAccountId.Value, header.PartyId, Math.Abs(header.TotDiscount), "Sales", $"Discount - Inv {header.InvNo}"));
        //        }

        //        // 6. Round Off
        //        if (header.RoundOff != 0 && roundOffAccountId != null)
        //        {
        //            // For Sales: If RoundOff is positive (we added to invoice), it's income -> Dr Party, Cr: RoundOffAccount
        //            // If RoundOff is negative (we reduced from invoice), it's expense -> Dr: RoundOffAccount, Cr: Party
        //            bool isPositiveRoundOff = header.RoundOff > 0;
        //            long drAcc = isPositiveRoundOff ? header.PartyId : roundOffAccountId.Value;
        //            long crAcc = isPositiveRoundOff ? roundOffAccountId.Value : header.PartyId;

        //            entries.Add(CreateJVEntry(header, drAcc, crAcc, Math.Abs(header.RoundOff.Value), "Sales", $"Round Off - Inv {header.InvNo}"));
        //        }

        //        // Assign Group Serial Numbers
        //        for (int i = 0; i < entries.Count; i++) entries[i].GroupJvSrNo = i + 1;

        //        return await _journalRepository.SaveJournalGroupAsync(entries, header.SpUniqueCode.ToString(), null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return DbResult.Failure(ex.Message);
        //    }
        //}


        //public async Task<DbResult> GenerateJournalFromPurchaseAsync(SalePur header, IEnumerable<SalePurTran> lines)
        //{
        //    try
        //    {
        //        Console.WriteLine($"[JournalService] Generating journal for Purchase: {header.InvNo}, Company: {header.CompanyId}");
        //        var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, header.CompanyId);
        //        var settings = settingsList.FirstOrDefault();
        //        if (settings == null) return DbResult.Failure("Ledger settings not configured.");

        //        var settingsDict = (IDictionary<string, object>)settings;
        //        var purchaseAccountId = MapLongValue(settingsDict, "PurchaseExpenseAccountId");
        //        var cgstInputAccountId = MapLongValue(settingsDict, "CgstInputAccountId");
        //        var sgstInputAccountId = MapLongValue(settingsDict, "SgstInputAccountId");
        //        var igstInputAccountId = MapLongValue(settingsDict, "IgstInputAccountId");
        //        var roundOffAccountId = MapLongValue(settingsDict, "RoundOffAccountId");

        //        if (purchaseAccountId == null)
        //        {
        //            Console.WriteLine($"[JournalService] ERROR: Purchase Expense Account not mapped for Company: {header.CompanyId}. Check Ledger Settings.");
        //            return DbResult.Failure("Purchase Expense Account not mapped.");
        //        }

        //        Console.WriteLine($"[JournalService] Mapping Success: Purchase:{purchaseAccountId}, CGST:{cgstInputAccountId}, SGST:{sgstInputAccountId}, IGST:{igstInputAccountId}, RoundOff:{roundOffAccountId}");

        //        var entries = new List<JournalVoucher>();

        //        // Entry 1: DR Purchase, CR Party
        //        if (header.TaxableAmount > 0)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = purchaseAccountId.Value,
        //                CrAccountCode = header.PartyId,
        //                Amount = header.TaxableAmount.Value,
        //                VType = "Purchase",
        //                VCode = header.SpUniqueCode,
        //                Description = $"Purchase Taxable Value - Bill {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // Taxation
        //        if (header.CgstAmount > 0 && cgstInputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = cgstInputAccountId.Value,
        //                CrAccountCode = header.PartyId,
        //                Amount = header.CgstAmount.Value,
        //                VType = "Purchase",
        //                VCode = header.SpUniqueCode,
        //                Description = $"CGST Input - Bill {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        if (header.SgstAmount > 0 && sgstInputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = sgstInputAccountId.Value,
        //                CrAccountCode = header.PartyId,
        //                Amount = header.SgstAmount.Value,
        //                VType = "Purchase",
        //                VCode = header.SpUniqueCode,
        //                Description = $"SGST Input - Bill {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        if (header.IgstAmount > 0 && igstInputAccountId != null)
        //        {
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = igstInputAccountId.Value,
        //                CrAccountCode = header.PartyId,
        //                Amount = header.IgstAmount.Value,
        //                VType = "Purchase",
        //                VCode = header.SpUniqueCode,
        //                Description = $"IGST Input - Bill {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // Round Off
        //        if (header.RoundOff != 0 && roundOffAccountId != null)
        //        {
        //            bool isDrParty = header.RoundOff < 0; 
        //            entries.Add(new JournalVoucher
        //            {
        //                JvDate = header.InvDate,
        //                DrAccountCode = isDrParty ? header.PartyId : roundOffAccountId.Value,
        //                CrAccountCode = isDrParty ? roundOffAccountId.Value : header.PartyId,
        //                Amount = Math.Abs(header.RoundOff.Value),
        //                VType = "Purchase",
        //                VCode = header.SpUniqueCode,
        //                Description = $"Round Off - Bill {header.InvNo}",
        //                JvCompCode = header.CompanyId,
        //                YearCode = header.FiscalYearId ?? 0,
        //                BranchCode = header.BranchId ?? 0,
        //                CreatedBy = header.CreatedBy?.ToString(),
        //                GCode = header.SpUniqueCode.ToString()
        //            });
        //        }

        //        // Assign Group Serial Numbers
        //        for (int i = 0; i < entries.Count; i++)
        //        {
        //            entries[i].GroupJvSrNo = i + 1;
        //        }

        //        // Atomic Save
        //        return await _journalRepository.SaveJournalGroupAsync(entries, header.SpUniqueCode.ToString(), null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return DbResult.Failure(ex.Message);
        //    }
        //}

        //public async Task<DbResult> GenerateJournalFromPurchaseAsync(SalePur header, IEnumerable<SalePurTran> lines)
        //{
        //    try
        //    {
        //        var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, header.CompanyId);
        //        var settings = settingsList.FirstOrDefault();
        //        if (settings == null) return DbResult.Failure("Ledger settings not configured.");

        //        var settingsDict = (IDictionary<string, object>)settings;
        //        var purchaseAccountId = MapLongValue(settingsDict, "PurchaseExpenseAccountId");
        //        var cgstInputAccountId = MapLongValue(settingsDict, "CgstInputAccountId");
        //        var sgstInputAccountId = MapLongValue(settingsDict, "SgstInputAccountId");
        //        var igstInputAccountId = MapLongValue(settingsDict, "IgstInputAccountId");
        //        var roundOffAccountId = MapLongValue(settingsDict, "RoundOffAccountId");
        //        var discountAccountId = MapLongValue(settingsDict, "DiscountReceivedAccountId"); // Need this for purchase discount

        //        if (purchaseAccountId == null) return DbResult.Failure("Purchase Expense Account not mapped.");

        //        var entries = new List<JournalVoucher>();

        //        // Double-Entry Balanced Entries for Purchase

        //        // 1. Taxable Amount (Dr: Purchase, Cr: Party)
        //        if (header.TaxableAmount > 0)
        //        {
        //            entries.Add(CreateJVEntry(header, purchaseAccountId.Value, header.PartyId, header.TaxableAmount.Value, "Purchase", $"Purchase Taxable Value - Bill {header.InvNo}"));
        //        }

        //        // 2. CGST (Dr: CGST, Cr: Party)
        //        if (header.CgstAmount > 0 && cgstInputAccountId != null)
        //        {
        //            entries.Add(CreateJVEntry(header, cgstInputAccountId.Value, header.PartyId, header.CgstAmount.Value, "Purchase", $"CGST Input - Bill {header.InvNo}"));
        //        }

        //        // 3. SGST (Dr: SGST, Cr: Party)
        //        if (header.SgstAmount > 0 && sgstInputAccountId != null)
        //        {
        //            entries.Add(CreateJVEntry(header, sgstInputAccountId.Value, header.PartyId, header.SgstAmount.Value, "Purchase", $"SGST Input - Bill {header.InvNo}"));
        //        }

        //        // 4. IGST (Dr: IGST, Cr: Party)
        //        if (header.IgstAmount > 0 && igstInputAccountId != null)
        //        {
        //            entries.Add(CreateJVEntry(header, igstInputAccountId.Value, header.PartyId, header.IgstAmount.Value, "Purchase", $"IGST Input - Bill {header.InvNo}"));
        //        }

        //        // 5. Discount (Dr: Party, Cr: Discount Received)
        //        if (header.TotDiscount != 0 && discountAccountId != null)
        //        {
        //            entries.Add(CreateJVEntry(header, header.PartyId, discountAccountId.Value, Math.Abs(header.TotDiscount), "Purchase", $"Discount - Bill {header.InvNo}"));
        //        }

        //        // 6. Round Off
        //        if (header.RoundOff != 0 && roundOffAccountId != null)
        //        {
        //            // For Purchase: If RoundOff > 0 (invoice increased), we owe more (Cr Party), so Dr RoundOff Expense
        //            // If RoundOff < 0 (invoice decreased), we owe less (Dr Party), so Cr RoundOff Income
        //            bool isPositiveRoundOff = header.RoundOff > 0;
        //            long drAcc = isPositiveRoundOff ? roundOffAccountId.Value : header.PartyId;
        //            long crAcc = isPositiveRoundOff ? header.PartyId : roundOffAccountId.Value;

        //            entries.Add(CreateJVEntry(header, drAcc, crAcc, Math.Abs(header.RoundOff.Value), "Purchase", $"Round Off - Bill {header.InvNo}"));
        //        }

        //        for (int i = 0; i < entries.Count; i++) entries[i].GroupJvSrNo = i + 1;

        //        return await _journalRepository.SaveJournalGroupAsync(entries, header.SpUniqueCode.ToString(), null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return DbResult.Failure(ex.Message);
        //    }
        //}//after adding invoice column


        // changes made by Hetak dt 20052026 - optional transaction keeps sale header, lines, and JV atomic.
        public async Task<DbResult> GenerateJournalFromSaleAsync(SalePur header, IEnumerable<SalePurTran> lines, IDbTransaction? transaction = null)
        {
            try
            {
                var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, header.CompanyId);
                var settings = settingsList.FirstOrDefault();
                if (settings == null) return DbResult.Failure("Ledger settings not configured.");

                var settingsDict = (IDictionary<string, object>)settings;
                var salesAccountId = MapLongValue(settingsDict, "SalesRevenueAccountId") ?? 0;
                var cgstOutputAccountId = MapLongValue(settingsDict, "CgstOutputAccountId") ?? 0;
                var sgstOutputAccountId = MapLongValue(settingsDict, "SgstOutputAccountId") ?? 0;
                var igstOutputAccountId = MapLongValue(settingsDict, "IgstOutputAccountId") ?? 0;
                var roundOffAccountId = MapLongValue(settingsDict, "RoundOffAccountId") ?? 0;
                var discountAccountId = MapLongValue(settingsDict, "DiscountAccountId") ?? 0;

                if (salesAccountId == 0) return DbResult.Failure("Sales Revenue Account not mapped in Ledger Settings.");
                if (header.TotDiscount != 0 && discountAccountId == 0) return DbResult.Failure("Discount Account not mapped in Ledger Settings. Please set it in Masters -> Ledger Settings.");
                if ((header.RoundOff ?? 0) != 0 && roundOffAccountId == 0) return DbResult.Failure("Round Off Account not mapped in Ledger Settings. Please set it in Masters -> Ledger Settings.");

                var entries = new List<JournalVoucher>();

                // --- 7-Line Breakdown Logic using Account 0 as Contra ---

                // 1. Final Invoice Amount (Dr: Party, Cr: 0)
                entries.Add(CreateJVEntry(header, header.PartyId, 0, header.GrandTotal ?? 0, "Sales", $"Final Invoice Amount - Inv {header.InvNo}"));

                // 2. Taxable Amount (Dr: 0, Cr: Sales)
                entries.Add(CreateJVEntry(header, 0, salesAccountId, header.TaxableAmount ?? 0, "Sales", $"Sales Taxable Value - Inv {header.InvNo}"));

                // 3. CGST (Dr: 0, Cr: CGST)
                entries.Add(CreateJVEntry(header, 0, cgstOutputAccountId, header.CgstAmount ?? 0, "Sales", $"CGST Output - Inv {header.InvNo}"));

                // 4. SGST (Dr: 0, Cr: SGST)
                entries.Add(CreateJVEntry(header, 0, sgstOutputAccountId, header.SgstAmount ?? 0, "Sales", $"SGST Output - Inv {header.InvNo}"));

                // 5. IGST (Dr: 0, Cr: IGST)
                entries.Add(CreateJVEntry(header, 0, igstOutputAccountId, header.IgstAmount ?? 0, "Sales", $"IGST Output - Inv {header.InvNo}"));

                // 6. Discount (Sales)
                // Positive Discount = Expense (Dr: Discount, Cr: 0)
                // Negative Discount = Surcharge/Income (Dr: 0, Cr: Discount)
                if (header.TotDiscount != 0)
                {
                    long drAcc_D = header.TotDiscount > 0 ? discountAccountId : 0;
                    long crAcc_D = header.TotDiscount > 0 ? 0 : discountAccountId;
                    entries.Add(CreateJVEntry(header, drAcc_D, crAcc_D, Math.Abs(header.TotDiscount), "Sales", $"Discount - Inv {header.InvNo}"));
                }
                else
                {
                    entries.Add(CreateJVEntry(header, discountAccountId, 0, 0, "Sales", $"Discount - Inv {header.InvNo}"));
                }

                // 7. Round Off (Dr: 0/RoundOff, Cr: RoundOff/0)
                decimal roundOff = header.RoundOff ?? 0;
                long drAcc_R = roundOff > 0 ? 0 : (roundOffAccountId != 0 ? roundOffAccountId : 0);
                long crAcc_R = roundOff > 0 ? (roundOffAccountId != 0 ? roundOffAccountId : 0) : 0;
                entries.Add(CreateJVEntry(header, drAcc_R, crAcc_R, Math.Abs(roundOff), "Sales", $"Round Off - Inv {header.InvNo}"));

                for (int i = 0; i < entries.Count; i++) entries[i].GroupJvSrNo = i + 1;
                return await _journalRepository.SaveJournalGroupAsync(entries, header.SpUniqueCode.ToString(), transaction, null);
            }
            catch (Exception ex) { return DbResult.Failure(ex.Message); }
        }

        // changes made by Hetak dt 20052026 - optional transaction keeps purchase header, lines, and JV atomic.
        public async Task<DbResult> GenerateJournalFromPurchaseAsync(SalePur header, IEnumerable<SalePurTran> lines, IDbTransaction? transaction = null)
        {
            try
            {
                var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, header.CompanyId);
                var settings = settingsList.FirstOrDefault();
                if (settings == null) return DbResult.Failure("Ledger settings not configured.");

                var settingsDict = (IDictionary<string, object>)settings;
                var purchaseAccountId = MapLongValue(settingsDict, "PurchaseExpenseAccountId") ?? 0;
                var cgstInputAccountId = MapLongValue(settingsDict, "CgstInputAccountId") ?? 0;
                var sgstInputAccountId = MapLongValue(settingsDict, "SgstInputAccountId") ?? 0;
                var igstInputAccountId = MapLongValue(settingsDict, "IgstInputAccountId") ?? 0;
                var roundOffAccountId = MapLongValue(settingsDict, "RoundOffAccountId") ?? 0;
                var discountAccountId = MapLongValue(settingsDict, "DiscountAccountId") ?? 0;

                Console.WriteLine($"[JournalService] Purchase Mapping: Purchase={purchaseAccountId}, CGST={cgstInputAccountId}, SGST={sgstInputAccountId}, IGST={igstInputAccountId}, RoundOff={roundOffAccountId}, Discount={discountAccountId}");

                if (purchaseAccountId == 0) return DbResult.Failure("Purchase Expense Account not mapped in Ledger Settings.");
                if (header.TotDiscount != 0 && discountAccountId == 0) return DbResult.Failure("Discount Account not mapped in Ledger Settings. Please set it in Masters -> Ledger Settings.");
                if ((header.RoundOff ?? 0) != 0 && roundOffAccountId == 0) return DbResult.Failure("Round Off Account not mapped in Ledger Settings. Please set it in Masters -> Ledger Settings.");

                var entries = new List<JournalVoucher>();

                // --- 7-Line Breakdown Logic using Account 0 as Contra ---

                // 1. Final Bill Amount (Dr: 0, Cr: Party)
                entries.Add(CreateJVEntry(header, 0, header.PartyId, header.GrandTotal ?? 0, "Purchase", $"Final Bill Amount - Bill {header.InvNo}"));

                // 2. Taxable Amount (Dr: Purchase, Cr: 0)
                entries.Add(CreateJVEntry(header, purchaseAccountId, 0, header.TaxableAmount ?? 0, "Purchase", $"Purchase Taxable Value - Bill {header.InvNo}"));

                // 3. CGST (Dr: CGST, Cr: 0)
                entries.Add(CreateJVEntry(header, cgstInputAccountId, 0, header.CgstAmount ?? 0, "Purchase", $"CGST Input - Bill {header.InvNo}"));

                // 4. SGST (Dr: SGST, Cr: 0)
                entries.Add(CreateJVEntry(header, sgstInputAccountId, 0, header.SgstAmount ?? 0, "Purchase", $"SGST Input - Bill {header.InvNo}"));

                // 5. IGST (Dr: IGST, Cr: 0)
                entries.Add(CreateJVEntry(header, igstInputAccountId, 0, header.IgstAmount ?? 0, "Purchase", $"IGST Input - Bill {header.InvNo}"));

                // 6. Discount (Purchase)
                // Positive Discount = Income (Dr: 0, Cr: Discount)
                // Negative Discount = Surcharge/Expense (Dr: Discount, Cr: 0)
                if (header.TotDiscount != 0)
                {
                    long drAcc_D = header.TotDiscount > 0 ? 0 : discountAccountId;
                    long crAcc_D = header.TotDiscount > 0 ? discountAccountId : 0;
                    entries.Add(CreateJVEntry(header, drAcc_D, crAcc_D, Math.Abs(header.TotDiscount), "Purchase", $"Discount - Bill {header.InvNo}"));
                }
                else
                {
                    entries.Add(CreateJVEntry(header, 0, discountAccountId, 0, "Purchase", $"Discount - Bill {header.InvNo}"));
                }

                // 7. Round Off (Dr: RoundOff/0, Cr: 0/RoundOff)
                decimal roundOff = header.RoundOff ?? 0;
                long drAcc_R = roundOff > 0 ? (roundOffAccountId != 0 ? roundOffAccountId : 0) : 0;
                long crAcc_R = roundOff > 0 ? 0 : (roundOffAccountId != 0 ? roundOffAccountId : 0);
                entries.Add(CreateJVEntry(header, drAcc_R, crAcc_R, Math.Abs(roundOff), "Purchase", $"Round Off - Bill {header.InvNo}"));

                for (int i = 0; i < entries.Count; i++) entries[i].GroupJvSrNo = i + 1;
                return await _journalRepository.SaveJournalGroupAsync(entries, header.SpUniqueCode.ToString(), transaction, null);
            }
            catch (Exception ex) { return DbResult.Failure(ex.Message); }
        }






        public async Task<DbResult> GenerateJournalFromPayRecAsync(IEnumerable<PayRec> allocations, IDbTransaction? transaction = null)
        {
            try
            {
                if (allocations == null || !allocations.Any())
                    return DbResult.Failure("No allocations provided for JV generation.");

                var first = allocations.First();
                Console.WriteLine($"[JournalService] Generating detailed journal for {first.VchType}: {first.GCode}, Company: {first.CompanyId}");
                
                // Fetch ledger settings for this specific company
                var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, (int?)first.CompanyId);
                var settings = settingsList.FirstOrDefault();
                if (settings == null) return DbResult.Failure("Ledger settings not configured.");  //ledger setting null check set on 04/04/26
                var settingsDict = settings as IDictionary<string, object>;

                var entries = new List<JournalVoucher>();
                string vType = first.VchType ?? "RECEIPT";
                decimal totalAmount = allocations.Sum(a => a.Amount ?? 0);
                decimal totalDiscount = allocations.Sum(a => a.DiscountAmount);

                // 1. Party & Bank Entries (Individual for each allocation)
                foreach (var alloc in allocations)
                {
                    decimal amount = alloc.Amount ?? 0;
                    if (amount > 0)
                    {
                        entries.Add(new JournalVoucher
                        {
                            JvDate = alloc.Date ?? DateTime.Now,
                            // Balanced Entry: DR Bank/Cash, CR Party (if Receipt)
                            // Balanced Entry: DR Party, CR Bank/Cash (if Payment)
                            DrAccountCode = vType == "RECEIPT" ? (alloc.AccountId ?? 0) : (alloc.PartyId ?? 0),
                            CrAccountCode = vType == "RECEIPT" ? (alloc.PartyId ?? 0) : (alloc.AccountId ?? 0),
                            Amount = amount,
                            VType = vType == "RECEIPT" ? "Receipt" : "Payment",
                            VCode = alloc.Code,
                            Description = !string.IsNullOrEmpty(alloc.Disc) ? alloc.Disc : $"{vType} Allocation - {alloc.GCode}",
                            JvCompCode = (int)(alloc.CompanyId ?? 0),
                            YearCode = alloc.YearCode ?? 0,
                            BranchCode = alloc.BranchCode ?? 0,
                            CreatedBy = alloc.CreatedBy,
                            GCode = alloc.GCode
                        });
                    }

                    // 2. Discount Entries (Individual for each allocation)
                    if (alloc.DiscountAmount > 0 && settingsDict != null)
                    {
                        var discountKey = vType == "RECEIPT" ? "DiscountAllowedAccountId" : "DiscountReceivedAccountId";
                        var discountAccountId = MapLongValue(settingsDict, discountKey);
                        
                        if (discountAccountId != null)
                        {
                            entries.Add(new JournalVoucher
                            {
                                JvDate = alloc.Date ?? DateTime.Now,
                                // If RECEIPT: DR Discount, CR Party.
                                // If PAYMENT: DR Party, CR Discount.
                                DrAccountCode = vType == "RECEIPT" ? discountAccountId.Value : (alloc.PartyId ?? 0),
                                CrAccountCode = vType == "RECEIPT" ? (alloc.PartyId ?? 0) : discountAccountId.Value,
                                Amount = alloc.DiscountAmount,
                                VType = vType == "RECEIPT" ? "Receipt" : "Payment",
                                VCode = alloc.Code,
                                Description = $"Discount on {vType} - {alloc.GCode}",
                                JvCompCode = (int)(alloc.CompanyId ?? 0),
                                YearCode = alloc.YearCode ?? 0,
                                BranchCode = alloc.BranchCode ?? 0,
                                CreatedBy = alloc.CreatedBy,
                                GCode = alloc.GCode
                            });
                        }
                    }
                }
                for (int i = 0; i < entries.Count; i++)
                {
                    entries[i].GroupJvSrNo = i + 1;
                }

                // Atomic Save using GCode to soft-delete existing ones
                return await _journalRepository.SaveJournalGroupAsync(entries, first.GCode, transaction, null);
            }
            catch (Exception ex)
            {
                return DbResult.Failure(ex.Message);
            }
        }

        public async Task<DbResult> DeleteJournalByGCodeAsync(string gCode, int? userId = null)
        {
            try
            {
                // We reuse the atomic save with an empty list to perform a clean delete by GCode
                return await _journalRepository.SaveJournalGroupAsync(new List<JournalVoucher>(), gCode, null, userId);
            }
            catch (Exception ex)
            {
                return DbResult.Failure(ex.Message);
            }
        }

        public async Task<DbResult> SaveDirectJournalAsync(IEnumerable<JournalVoucher> entries, int? userId = null)
        {
            try
            {
                if (entries == null || !entries.Any())
                    return DbResult.Failure("No journal entries provided for direct saving.");

                var first = entries.First();
                string gCode = first.GCode;

                for (int i = 0; i < entries.Count(); i++)
                {
                    entries.ElementAt(i).GroupJvSrNo = i + 1;
                }

                // If updating (GCode exists), Soft Delete the old ones linked to this GCode
                return await _journalRepository.SaveJournalGroupAsync(entries, string.IsNullOrEmpty(gCode) ? null : gCode, null, userId);
            }
            catch (Exception ex)
            {
                return DbResult.Failure(ex.Message);
            }
        }

        public async Task<IEnumerable<JournalVoucher>> GetDirectJournalsAsync(long companyId, string vType)
        {
            return await _journalRepository.GetDirectJournalsAsync(companyId, vType);
        }

        private long? MapLongValue(IDictionary<string, object> dict, string key)
        {
            if (!dict.ContainsKey(key) || dict[key] == null) 
            {
                Console.WriteLine($"[JournalService] Warning: Mapping key '{key}' not found in settings dictionary.");
                return null;
            }
            
            var val = dict[key];
            
            // Handle JsonElement (from System.Text.Json)
            if (val is JsonElement jsonEl)
            {
                if (jsonEl.ValueKind == JsonValueKind.Number) return jsonEl.GetInt64();
                if (jsonEl.ValueKind == JsonValueKind.String)
                {
                    if (long.TryParse(jsonEl.GetString(), out long result)) return result;
                }
                Console.WriteLine($"[JournalService] Warning: Mapping key '{key}' is not a valid number. Value: {jsonEl.GetRawText()}");
                return null;
            }

            // Handle direct values
            try
            {
                if (val is string str && long.TryParse(str, out long parsedStr)) return parsedStr;
                return Convert.ToInt64(val);
            }
            catch
            {
                Console.WriteLine($"[JournalService] Warning: Mapping key '{key}' could not be converted to long. Value Type: {val?.GetType().Name}");
                return null;
            }
        }
        public async Task<object?> GetLedgerSettingsAsync(int? companyId = null)
        {
            var settingsList = await _masterRepository.GetAllAsync("ledgersetting", null, companyId);
            return settingsList.FirstOrDefault();
        }

        private JournalVoucher CreateJVEntry(SalePur header, long drCode, long crCode, decimal amount, string vType, string desc)
        {
            return new JournalVoucher
            {
                JvDate = header.InvDate,
                DrAccountCode = drCode,
                CrAccountCode = crCode,
                Amount = amount,
                VType = vType,
                VCode = header.SpUniqueCode,
                Description = desc,
                JvCompCode = header.CompanyId,
                YearCode = header.FiscalYearId ?? 0,
                BranchCode = header.BranchId ?? 0,
                CreatedBy = header.CreatedBy?.ToString(),
                GCode = header.SpUniqueCode.ToString()
            };
        }

    }
}
