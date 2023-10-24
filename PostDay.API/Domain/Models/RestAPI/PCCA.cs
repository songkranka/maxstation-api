using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.RestAPI
{
    public class PCCA
    {
        public string CODCOMP_HEAD { get; set; }
        public string CENTER_CODE { get; set; }
        public string ORG_SHOPID { get; set; }
        public DateTime H_DATE { get; set; }
        public DateTime H_TS { get; set; }
        public decimal AMOUNT { get; set; }
    }
}
