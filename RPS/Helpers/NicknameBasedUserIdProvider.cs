using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RPS.Helpers
{
    public class NicknameBasedUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.GivenName)?.Value;
        }
    }

}
