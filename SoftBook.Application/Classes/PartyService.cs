using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.ViewModels;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Classes
{
    public class PartyService : IPartyService
    {
        private readonly IPartyRepository _partyRepository;
        private readonly IPartyAddressRepository _addressRepository;
        private readonly IPartyBankDetailsRepository _bankRepository;
        private readonly IPartyContactPersonRepository _contactRepository;
        private readonly IPartyReportingTagRepository _tagRepository;

        public PartyService(
            IPartyRepository partyRepository,
            IPartyAddressRepository addressRepository,
            IPartyBankDetailsRepository bankRepository,
            IPartyContactPersonRepository contactRepository,
            IPartyReportingTagRepository tagRepository)
        {
            _partyRepository = partyRepository;
            _addressRepository = addressRepository;
            _bankRepository = bankRepository;
            _contactRepository = contactRepository;
            _tagRepository = tagRepository;
        }

        public async Task<List<Party>> GetAllAsync() => await _partyRepository.GetAllAsync();
        public async Task<Party?> GetByIdAsync(int id) => await _partyRepository.GetByIdAsync(id);
        public async Task<DbResult> InsertAsync(Party entity, int? userId = null) => await _partyRepository.InsertAsync(entity, userId);
        public async Task<DbResult> UpdateAsync(Party entity, int? userId = null) => await _partyRepository.UpdateAsync(entity, userId);
        public async Task<DbResult> DeleteAsync(int id, int? userId = null)
        {
            try
            {
                // Cascade Delete - Manual
                // 1. Delete Addresses
                var addresses = await _addressRepository.GetByPartyIdAsync(id);
                foreach (var item in addresses) await _addressRepository.DeleteAsync(item.AddressId);

                // 2. Delete Bank Details
                var banks = await _bankRepository.GetByPartyIdAsync(id);
                foreach (var item in banks) await _bankRepository.DeleteAsync(item.BankDetailId);

                // 3. Delete Contact Persons
                var contacts = await _contactRepository.GetByPartyIdAsync(id);
                foreach (var item in contacts) await _contactRepository.DeleteAsync(item.ContactPersonId);

                // 4. Delete Reporting Tags
                var tags = await _tagRepository.GetByPartyIdAsync(id);
                foreach (var item in tags) await _tagRepository.DeleteAsync(item.PartyReportingTagId);

                // 5. Delete Party
                return await _partyRepository.DeleteAsync(id, userId);
            }
            catch (Exception ex)
            {
                // Fallback for non-postgres exceptions or issues during manual cascade
                return DbResult.Failure("Error during delete: " + ex.Message);
            }
        }
        public async Task<List<Party>> GetByPartyTypeAsync(string partyType) => await _partyRepository.GetByPartyTypeAsync(partyType);

        public async Task<DbResult> InsertCompositeAsync(PartyViewModel viewModel, int? userId = null)
        {
            // 1. Insert Party
            var result = await _partyRepository.InsertAsync(viewModel.Party, userId);
            if (!result.Success) return result;

            int partyId = viewModel.Party.PartyId;

            // 2. Insert Children
            if (viewModel.Addresses != null)
            {
                foreach (var item in viewModel.Addresses)
                {
                    if (!string.IsNullOrWhiteSpace(item.Street1) || !string.IsNullOrWhiteSpace(item.City))
                    {
                        item.PartyId = partyId;
                        await _addressRepository.InsertAsync(item);
                    }
                }
            }

            if (viewModel.BankDetails != null)
            {
                foreach (var item in viewModel.BankDetails)
                {
                    if (!string.IsNullOrWhiteSpace(item.AccountNumber))
                    {
                        item.PartyId = partyId;
                        await _bankRepository.InsertAsync(item);
                    }
                }
            }

            if (viewModel.ContactPersons != null)
            {
                foreach (var item in viewModel.ContactPersons)
                {
                    if (!string.IsNullOrWhiteSpace(item.FirstName))
                    {
                        item.PartyId = partyId;
                        await _contactRepository.InsertAsync(item);
                    }
                }
            }

            if (viewModel.ReportingTags != null)
            {
                foreach (var item in viewModel.ReportingTags)
                {
                    if (!string.IsNullOrWhiteSpace(item.TagValue))
                    {
                        item.PartyId = partyId;
                        await _tagRepository.InsertAsync(item);
                    }
                }
            }
            
            return DbResult.Ok(viewModel.Party);
        }

        public async Task<DbResult> UpdateCompositeAsync(PartyViewModel viewModel, int? userId = null)
        {
            int partyId = viewModel.Party.PartyId;
            
            // 1. Update Party
            var result = await _partyRepository.UpdateAsync(viewModel.Party);
            if (!result.Success) return result;

            // 2. Update Addresses (Delete Existing & Re-insert)
            var addresses = await _addressRepository.GetByPartyIdAsync(partyId);
            foreach (var addr in addresses) await _addressRepository.DeleteAsync(addr.AddressId);
            
            if (viewModel.Addresses != null)
            {
                foreach (var item in viewModel.Addresses)
                {
                    if (!string.IsNullOrWhiteSpace(item.Street1) || !string.IsNullOrWhiteSpace(item.City))
                    {
                        item.PartyId = partyId;
                        await _addressRepository.InsertAsync(item);
                    }
                }
            }

            // 3. Update Bank Details
            var banks = await _bankRepository.GetByPartyIdAsync(partyId);
            foreach (var b in banks) await _bankRepository.DeleteAsync(b.BankDetailId);

            if (viewModel.BankDetails != null)
            {
                foreach (var item in viewModel.BankDetails)
                {
                    if (!string.IsNullOrWhiteSpace(item.AccountNumber))
                    {
                         item.PartyId = partyId;
                         await _bankRepository.InsertAsync(item);
                    }
                }
            }

            // 4. Update Contact Persons
            var contacts = await _contactRepository.GetByPartyIdAsync(partyId);
            foreach (var c in contacts) await _contactRepository.DeleteAsync(c.ContactPersonId);

            if (viewModel.ContactPersons != null)
            {
                foreach (var item in viewModel.ContactPersons)
                {
                    if (!string.IsNullOrWhiteSpace(item.FirstName))
                    {
                        item.PartyId = partyId;
                        await _contactRepository.InsertAsync(item);
                    }
                 }
            }

            // 5. Update Reporting Tags
            var tags = await _tagRepository.GetByPartyIdAsync(partyId);
            foreach (var t in tags) await _tagRepository.DeleteAsync(t.PartyReportingTagId);

            if (viewModel.ReportingTags != null)
            {
                foreach (var item in viewModel.ReportingTags)
                {
                    if (!string.IsNullOrWhiteSpace(item.TagValue))
                    {
                        item.PartyId = partyId;
                        await _tagRepository.InsertAsync(item);
                    }
                }
            }

            return DbResult.Ok(viewModel.Party);
        }

        public async Task<PartyViewModel?> GetCompositeAsync(int id)
        {
            var party = await _partyRepository.GetByIdAsync(id);
            if (party == null) return null;

            return new PartyViewModel
            {
                Party = party,
                Addresses = await _addressRepository.GetByPartyIdAsync(id),
                BankDetails = await _bankRepository.GetByPartyIdAsync(id),
                ContactPersons = await _contactRepository.GetByPartyIdAsync(id),
                ReportingTags = await _tagRepository.GetByPartyIdAsync(id),
                // We return empty list for PartyTypes as it keys off Master data which we don't return here
                // UI should load Master data for dropdowns separately via dictionary endpoint if needed
                PartyTypes = new List<PartyTypeMaster>() 
            };
        }
    }
}
