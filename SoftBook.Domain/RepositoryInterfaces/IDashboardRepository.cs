using SoftBook.Domain.DTOs;
using System.Threading.Tasks;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IDashboardRepository
    {
        Task<string> GetDashboardDataAsync(DashboardRequestDto request);
    }
}
