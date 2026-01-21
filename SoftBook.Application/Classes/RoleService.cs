using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(Role entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(Role entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
