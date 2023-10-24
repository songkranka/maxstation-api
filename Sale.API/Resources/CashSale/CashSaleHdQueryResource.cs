using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.CashSale
{
    public class CashSaleHdQueryResource : QueryResource
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public string Guid { get; set; }
        public DateTime SysDate { get; set; }
    }
}
