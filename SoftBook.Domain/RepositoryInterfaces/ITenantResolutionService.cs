using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface ITenantResolutionService
    {
        Task<string?> GetConnectionStringAsync(int tenantId);
    }
}
