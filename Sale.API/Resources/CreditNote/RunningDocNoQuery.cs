using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.CreditNote
{
    public class RunningDocNoQuery : QueryResource
    {
        public string compCode { get; set; }
        public string brnCode { get; set; }
        public string locCode { get; set; }
    }
}
