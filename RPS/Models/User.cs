﻿namespace RPS.Models
{
    public class User
    {
        public System.Guid? Id { get; set; }
        public string Email { get; set; }

        public int TotalMatches { get; set; }

        public int Wins { get; set; }

        public string Nickname { get; set; }
    }

    public class UserView
    {
        public string tokenId { get; set; }
    }
}
