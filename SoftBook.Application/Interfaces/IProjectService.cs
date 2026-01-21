using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task InsertAsync(Project entity);
        Task UpdateAsync(Project entity);
        Task DeleteAsync(int id);
    }
}
