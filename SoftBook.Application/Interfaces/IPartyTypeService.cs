using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Interfaces
{
    public interface IPartyTypeService
    {
        Task<IEnumerable<dynamic>> GetAllActiveAsync(int companyId);
    }
}
