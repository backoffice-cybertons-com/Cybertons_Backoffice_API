using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IOutstandingTermConfigRepository
    {
        Task<long> AddAsync(CreateOutstandingTermConfigRequest request, long companyId);
        Task<bool> UpdateAsync(UpdateOutstandingTermConfigRequest request, long companyId);
        Task<bool> SoftDeleteAsync(long configId);
        Task<List<OutstandingTermConfigModel>> GetActiveByCompanyAsync(long companyId);
        Task<OutstandingTermConfigModel?> GetByIdAsync(long configId);
    }
}

