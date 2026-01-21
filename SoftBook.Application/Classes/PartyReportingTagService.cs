using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PartyReportingTagService : IPartyReportingTagService
    {
        private readonly IPartyReportingTagRepository _repository;

        public PartyReportingTagService(IPartyReportingTagRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PartyReportingTag>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PartyReportingTag?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<PartyReportingTag>> GetByPartyIdAsync(int partyId)
        {
            return await _repository.GetByPartyIdAsync(partyId);
        }

        public async Task InsertAsync(PartyReportingTag entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(PartyReportingTag entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}







