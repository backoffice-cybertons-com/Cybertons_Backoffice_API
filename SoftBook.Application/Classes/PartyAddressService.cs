using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PartyAddressService : IPartyAddressService
    {
        private readonly IPartyAddressRepository _repository;

        public PartyAddressService(IPartyAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PartyAddress>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PartyAddress?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<PartyAddress>> GetByPartyIdAsync(int partyId)
        {
            return await _repository.GetByPartyIdAsync(partyId);
        }

        public async Task InsertAsync(PartyAddress entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(PartyAddress entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}







