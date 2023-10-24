using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.CashSale
{
    public class CashSaleHdResource
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public decimal? VatAmtCur { get; set; }
        public string DocStatus { get; set; }
        public string Guid { get; set; }
        public decimal? NetAmt { get; set; }
        public string RefNo { get; set; }
    }
}
