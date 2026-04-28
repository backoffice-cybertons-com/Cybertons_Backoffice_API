using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IMasterRepository
    {
        Task<IEnumerable<dynamic>> GetAllAsync(string masterName, int? filterId = null, int? companyId = null);
        Task<dynamic?> GetByIdAsync(string masterName, int id, int? companyId = null);
        Task<DbResult> InsertAsync(string masterName, string jsonData, int? userId = null);
        Task<DbResult> UpdateAsync(string masterName, int id, string jsonData, int? userId = null);
        Task<DbResult> DeleteAsync(string masterName, int id, int? userId = null);
    }
}
