using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IMasterRepository
    {
        Task<IEnumerable<dynamic>> GetAllAsync(string masterName, int? filterId = null);
        Task<dynamic?> GetByIdAsync(string masterName, int id);
        Task<DbResult> InsertAsync(string masterName, string jsonData);
        Task<DbResult> UpdateAsync(string masterName, int id, string jsonData);
        Task<DbResult> DeleteAsync(string masterName, int id);
    }
}
