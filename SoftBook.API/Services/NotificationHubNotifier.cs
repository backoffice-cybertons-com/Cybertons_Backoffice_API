using Microsoft.AspNetCore.SignalR;
using SoftBook.API.Hubs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Services
{
    public class NotificationHubNotifier : INotificationRealtimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyUserAsync(long userId)
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("ReceiveNotification");
        }
    }
}
