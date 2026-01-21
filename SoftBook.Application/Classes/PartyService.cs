using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

using SoftBook.Domain.ViewModels;

namespace SoftBook.Application.Services.Classes
{
    public class PartyService : IPartyService
    {
        private readonly IPartyRepository _partyRepository;
        private readonly IPartyAddressRepository _addressRepository;
        private readonly IPartyBankDetailsRepository _bankRepository;
        private readonly IPartyContactPersonRepository _contactRepository;
        private readonly IPartyReportingTagRepository _tagRepository;
        private readonly IPartyTypeMasterRepository _typeRepository;

        public PartyService(
            IPartyRepository partyRepository,
            IPartyAddressRepository addressRepository,
            IPartyBankDetailsRepository bankRepository,
            IPartyContactPersonRepository contactRepository,
            IPartyReportingTagRepository tagRepository,
            IPartyTypeMasterRepository typeRepository)
        {
            _partyRepository = partyRepository;
            _addressRepository = addressRepository;
            _bankRepository = bankRepository;
            _contactRepository = contactRepository;
            _tagRepository = tagRepository;
            _typeRepository = typeRepository;
        }

        public async Task<List<Party>> GetAllAsync() => await _partyRepository.GetAllAsync();
        public async Task<Party?> GetByIdAsync(int id) => await _partyRepository.GetByIdAsync(id);
        public async Task InsertAsync(Party entity) => await _partyRepository.InsertAsync(entity);
        public async Task UpdateAsync(Party entity) => await _partyRepository.UpdateAsync(entity);
        public async Task DeleteAsync(int id)
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

            // 5. Delete Party Types (Mapping - Removed)

            // 6. Delete Party
            await _partyRepository.DeleteAsync(id);
        }
        public async Task<List<Party>> GetByPartyTypeAsync(string partyType) => await _partyRepository.GetByPartyTypeAsync(partyType);

        public async Task InsertCompositeAsync(PartyViewModel viewModel)
        {
            // 1. Insert Party
            await _partyRepository.InsertAsync(viewModel.Party);
            int partyId = viewModel.Party.PartyId; // Assuming InsertAsync populates ID via output param or EF tracking

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
            
            // Party Type mapping (Removed incorrect logic)
        }

        public async Task UpdateCompositeAsync(PartyViewModel viewModel)
        {
            int partyId = viewModel.Party.PartyId;
            
            // 1. Update Party
            await _partyRepository.UpdateAsync(viewModel.Party);

            // 2. Update Addresses (Delete Existing & Re-insert)
            // Note: This matches the previous logic from UI Controller
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

            // 6. Update Party Type (Removed logic)
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
