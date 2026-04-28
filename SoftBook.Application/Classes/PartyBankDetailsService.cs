using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PartyBankDetailsService : IPartyBankDetailsService
    {
        private readonly IPartyBankDetailsRepository _repository;

        public PartyBankDetailsService(IPartyBankDetailsRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PartyBankDetails>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PartyBankDetails?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<PartyBankDetails>> GetByPartyIdAsync(int partyId)
        {
            return await _repository.GetByPartyIdAsync(partyId);
        }

        public async Task InsertAsync(PartyBankDetails entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(PartyBankDetails entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}







