using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace RPS.Hubs
{
    //[Authorize]
    public class GameHub : Hub
    {
        private static readonly ConcurrentQueue<string> gameQueue = new ConcurrentQueue<string>();
        private static readonly ConcurrentBag<object> groups = new ConcurrentBag<object>();
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger;
        }
        public async Task EnterQueue()
        {
            var userId = Context.UserIdentifier;//Context.ConnectionId;
            _logger.LogInformation("User {@UserId} entered queue to matchmaking.", userId);
            if (!gameQueue.IsEmpty)
            {
                if (!gameQueue.Any(x => x == userId))
                {
                    gameQueue.TryDequeue(out var user2);
                    if (userId != user2)
                    {
                        _logger.LogInformation("User {@UserId} has found {@UserMatched} to make a match.", userId, user2);
                        var groupId = new Guid().ToString();
                        await Groups.AddToGroupAsync(userId, groupId);
                        await Groups.AddToGroupAsync(user2, groupId);
                        await Clients.Group(groupId).SendAsync("Matched", $"User #{userId} has been matched with user #{user2}");
                        groups.Add(new GameGroup { GroupId = groupId, Users = { userId, user2 } });
                        _logger.LogInformation("Group {@GroupId} created do match Users {@UserId} and {@UserMatched}.", groupId, userId, user2);

                    }
                }
            }
            else
            {
                gameQueue.Enqueue(userId);
            }
        }        
    }
}
