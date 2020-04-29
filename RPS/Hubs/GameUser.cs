using System;

namespace RPS.Hubs
{
    public class GameUser
    {
        public string Nickname { get; set; }
        public string ConnectionId { get; set; }

        public Guid Id { get; set; }
    }
}
