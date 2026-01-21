using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PartyContactPersonService : IPartyContactPersonService
    {
        private readonly IPartyContactPersonRepository _repository;

        public PartyContactPersonService(IPartyContactPersonRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PartyContactPerson>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PartyContactPerson?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<PartyContactPerson>> GetByPartyIdAsync(int partyId)
        {
            return await _repository.GetByPartyIdAsync(partyId);
        }

        public async Task InsertAsync(PartyContactPerson entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(PartyContactPerson entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}







