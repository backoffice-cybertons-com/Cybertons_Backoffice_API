using SoftBook.Domain.DTOs;
using System.Threading.Tasks;

namespace SoftBook.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<string> GetDashboardDataAsync(DashboardRequestDto request);
    }
}
