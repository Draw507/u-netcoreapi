using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguridad.Models
{
    public class Usertoken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
