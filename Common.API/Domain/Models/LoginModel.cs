using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Domain.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CompCode { get; set; }
        public string IpAddress { get; set; }
    }
}
