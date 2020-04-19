using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPS.Models
{
    public class JWTConfig
    {
        public string JwtEmailEncryption { get; set; }
        public string JwtSecret { get; set; }

        public string GoogleClientId { get; set; }

    }
}
