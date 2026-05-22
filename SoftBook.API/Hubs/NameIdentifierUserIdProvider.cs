using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SoftBook.API.Hubs
{
    public class NameIdentifierUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? connection.User?.FindFirst("UserId")?.Value
                ?? connection.User?.FindFirst("user_id")?.Value
                ?? connection.User?.FindFirst("sub")?.Value
                ?? connection.User?.FindFirst("nameid")?.Value;
        }
    }
}
