using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IPartyTypeRepository
    {
        Task<IEnumerable<dynamic>> GetAllActiveAsync(int companyId);
    }
}
