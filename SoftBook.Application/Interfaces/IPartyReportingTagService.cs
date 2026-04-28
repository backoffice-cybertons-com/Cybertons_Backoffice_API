using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPartyReportingTagService
    {
        Task<List<PartyReportingTag>> GetAllAsync();
        Task<PartyReportingTag?> GetByIdAsync(int id);
        Task<List<PartyReportingTag>> GetByPartyIdAsync(int partyId);
        Task InsertAsync(PartyReportingTag entity);
        Task UpdateAsync(PartyReportingTag entity);
        Task DeleteAsync(int id);
    }
}







