using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPartyService
    {
        Task<List<Party>> GetAllAsync();
        Task<Party?> GetByIdAsync(int id);
        Task<List<Party>> GetByPartyTypeAsync(string partyType);
        Task<DbResult> InsertAsync(Party entity, int? userId = null);
        Task<DbResult> UpdateAsync(Party entity, int? userId = null);
        Task<DbResult> DeleteAsync(int id, int? userId = null);
        Task<DbResult> InsertCompositeAsync(PartyViewModel viewModel, int? userId = null);
        Task<DbResult> UpdateCompositeAsync(PartyViewModel viewModel, int? userId = null);
        Task<PartyViewModel?> GetCompositeAsync(int id);
    }
}
