using System;

namespace RPS.Models
{
    public class JWTToken
    {
        public string Token { get; set; }
        public DateTime ExpDate { get; set; }

    }
}
