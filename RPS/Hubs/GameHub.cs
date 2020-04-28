using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace RPS.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private static readonly ConcurrentQueue<GameUser> gameQueue = new ConcurrentQueue<GameUser>();
        private static readonly ConcurrentBag<GameGroup> groups = new ConcurrentBag<GameGroup>();
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger;
        }
        public async Task EnterQueue()
        {
            var user = new GameUser { ConnectionId = Context.ConnectionId, Nickname = Context.UserIdentifier };
            _logger.LogInformation("User {@User} entered queue to matchmaking.", user);
            if (!gameQueue.IsEmpty)
            {
                if (!gameQueue.Any(x => x == user))
                {
                    gameQueue.TryDequeue(out var user2);
                    if (user != user2)
                    {
                        _logger.LogInformation("User {@User} has found {@UserMatched} to make a match.", user, user2);
                        var groupId = Guid.NewGuid().ToString();
                        await Groups.AddToGroupAsync(user.ConnectionId, groupId);
                        await Groups.AddToGroupAsync(user2.ConnectionId, groupId);
                        await Clients.Group(groupId).SendAsync("Matched", $"User #{user.Nickname} has been matched with user #{user2.Nickname}");
                        _logger.LogInformation("Group {@GroupId} created to match Users {@User} and {@UserMatched}.", groupId, user, user2);
                        _logger.LogInformation("Groups {@Groups}", groups);
                        //groups.Add(new GameGroup { GroupId = groupId, Users = { user, user2 } });
                    }
                }
            }
            else
            {
                gameQueue.Enqueue(user);
            }
        }
    }
}
