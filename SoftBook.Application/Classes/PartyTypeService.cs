using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Application.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Application.Classes
{
    public class PartyTypeService : IPartyTypeService
    {
        private readonly IPartyTypeRepository _partyTypeRepository;

        public PartyTypeService(IPartyTypeRepository partyTypeRepository)
        {
            _partyTypeRepository = partyTypeRepository;
        }

        public async Task<IEnumerable<dynamic>> GetAllActiveAsync(int companyId)
        {
            return await _partyTypeRepository.GetAllActiveAsync(companyId);
        }
    }
}
