using System.Collections.Generic;

namespace RPS.Hubs
{
    internal class GameGroup
    {
        public string GroupId { get; set; }

        public List<GameUser> Users { get; set; } = new List<GameUser>();

    }
}
