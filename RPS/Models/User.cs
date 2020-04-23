using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPS.Models
{
    public class User
    {
        public System.Guid Id { get; set; }
        public string Email { get; set; }

        public int TotalMatches { get; set; } = 0;

        public string Nickname { get; set; }
    }

    public class UserView
    {
        public string tokenId { get; set; }
    }
}
