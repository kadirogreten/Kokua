using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokuaApi.Models
{
    public class LockoutUserViewModel
    {
        public string Username { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
