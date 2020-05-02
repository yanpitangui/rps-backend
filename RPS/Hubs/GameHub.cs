using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPS.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private static readonly ConcurrentDictionary<GameUser, DateTime> gameQueue = new ConcurrentDictionary<GameUser, DateTime>();
        private static readonly ConcurrentBag<GameGroup> groups = new ConcurrentBag<GameGroup>();
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger;
        }
        public async Task EnterQueue()
        {
            GameUser user = new GameUser { ConnectionId = Context.ConnectionId, Nickname = Context.UserIdentifier, Id = new Guid(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value) };
            try
            {
                _logger.LogDebug("User {@User} entered queue to matchmaking.", user);
                if (!gameQueue.IsEmpty)
                {
                    if (!gameQueue.Any(x => x.Key.Id == user.Id))
                    {
                        var user2 = gameQueue.OrderBy(X => X.Value).First().Key;
                        if (user.Id != user2.Id)
                        {
                            _logger.LogDebug("User {@User} has found {@UserMatched} to make a match.", user, user2);
                            if (RemoveFromQueue(user2.Id)) {
                                var groupId = Guid.NewGuid().ToString();
                                await Groups.AddToGroupAsync(user.ConnectionId, groupId);
                                await Groups.AddToGroupAsync(user2.ConnectionId, groupId);

                                await Clients.Group(groupId).SendAsync("Matched", $"User #{user.Nickname} has been matched with user #{user2.Nickname}");
                                _logger.LogInformation("Group {@GroupId} created to match Users {@User} and {@UserMatched}.", groupId, user, user2);
                                groups.Add(new GameGroup { GroupId = groupId, Users = new System.Collections.Generic.List<GameUser> { user, user2 } });
                            } else
                            {
                                throw new Exception($"Something went wrong while trying to remove user {user2.ToString()} from queue.");
                            }
                            
                        }
                    }
                }
                else
                {
                    bool added = gameQueue.TryAdd(user, DateTime.Now);
                    if (!added)
                    {
                        _logger.LogDebug("Something went wrong while trying to add user {@User} to matchmaking.", user);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to add user {@User} to matchmaking.", user);
            }
            
        }

        public async Task LeaveQueue()
        {
            GameUser user = new GameUser { ConnectionId = Context.ConnectionId, Nickname = Context.UserIdentifier, Id = new Guid(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value) };
            RemoveFromQueue(user.Id);
            Context.Abort(); 
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            GameUser user = new GameUser { ConnectionId = Context.ConnectionId, Nickname = Context.UserIdentifier, Id = new Guid(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value) };
            try
            {
                bool removed = RemoveFromQueue(user.Id);
                if (removed)
                {
                    _logger.LogDebug("User {@User} has left the queue due to a disconnection", user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to remove disconnected user {@User} from queue.", user);
            }
            await base.OnDisconnectedAsync(exception);
        }

        #region helpers
        /// <summary>
        /// returns true if the user has been successfully deleted from the queue.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool RemoveFromQueue(Guid Id)
        {
            foreach (var item in gameQueue.Where(x => x.Key.Id == Id))
            {
                try
                {
                    gameQueue.TryRemove(item.Key, out var dateDisconnect);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while trying to remove user {@User} from queue.", item);
                    throw ex;
                }
            }
            return !gameQueue.Any(x => x.Key.Id == Id);
        }
        #endregion
    }
}
