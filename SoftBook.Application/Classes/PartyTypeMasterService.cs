using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PartyTypeMasterService : IPartyTypeMasterService
    {
        private readonly IPartyTypeMasterRepository _repository;

        public PartyTypeMasterService(IPartyTypeMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PartyTypeMaster>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PartyTypeMaster?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Party>> GetPartiesByTypeAsync(string partyType)
        {
            return await _repository.GetPartiesByTypeAsync(partyType);
        }

        public async Task InsertAsync(PartyTypeMaster entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(PartyTypeMaster entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}




