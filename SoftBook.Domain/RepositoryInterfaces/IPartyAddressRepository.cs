using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IPartyAddressRepository : IRepository<PartyAddress>
    {
        Task<List<PartyAddress>> GetByPartyIdAsync(int partyId);
    }
}







