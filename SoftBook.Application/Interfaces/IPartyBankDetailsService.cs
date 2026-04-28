using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPartyBankDetailsService
    {
        Task<List<PartyBankDetails>> GetAllAsync();
        Task<PartyBankDetails?> GetByIdAsync(int id);
        Task<List<PartyBankDetails>> GetByPartyIdAsync(int partyId);
        Task InsertAsync(PartyBankDetails entity);
        Task UpdateAsync(PartyBankDetails entity);
        Task DeleteAsync(int id);
    }
}







