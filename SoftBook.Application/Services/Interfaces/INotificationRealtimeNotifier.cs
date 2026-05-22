using System.Threading.Tasks;

namespace SoftBook.Application.Services.Interfaces
{
    public interface INotificationRealtimeNotifier
    {
        Task NotifyUserAsync(long userId);
    }
}
