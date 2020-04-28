using System.Collections.Generic;

namespace RPS.Hubs
{
    internal class GameGroup
    {
        public string GroupId { get; set; }

        public List<string> Users { get; set; }

    }
}
