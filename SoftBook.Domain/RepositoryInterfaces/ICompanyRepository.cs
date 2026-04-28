using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompanyByIdAsync(int companyId);
        Task<List<Company>> GetAllAsync();
    }
}
