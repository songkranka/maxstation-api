using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.Request
{
    public class PCCARequest
    {
        public string CODCOMP_HEAD { get; set; }
        public string CENTER_CODE { get; set; }
        public string H_TS { get; set; }
    }
}
