using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPS.Models
{
    public class JWTToken
    {
        public string Token { get; set; }
        public DateTime ExpDate { get; set; }

    }
}
