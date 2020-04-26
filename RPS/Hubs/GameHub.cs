using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RPS.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
    }
}
