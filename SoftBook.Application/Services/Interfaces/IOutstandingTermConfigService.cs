using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IOutstandingTermConfigService
    {
        Task<long> AddAsync(CreateOutstandingTermConfigRequest request);
        Task UpdateAsync(UpdateOutstandingTermConfigRequest request);
        Task DeleteAsync(long configId);
        Task<List<OutstandingTermConfigModel>> ListAsync(long companyId);
    }
}

