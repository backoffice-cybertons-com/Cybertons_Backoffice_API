using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;

        public CompanyService(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Company?> GetCompanyByIdAsync(int companyId)
        {
            return await _repository.GetCompanyByIdAsync(companyId);
        }
        public async Task<List<Company>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
