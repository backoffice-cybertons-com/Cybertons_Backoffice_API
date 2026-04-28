using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Company?> GetCompanyByIdAsync(int companyId);
        Task<List<Company>> GetAllAsync();
    }
}
