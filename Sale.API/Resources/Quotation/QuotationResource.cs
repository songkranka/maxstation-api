using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.Quotation
{
    public class QuotationResource
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string DocStatus { get; set; }
        public string CustCode { get; set; }
        public string CustName { get; set; }
        public Guid Guid { get; set; }
        public decimal? NetAmt { get; set; }
        public decimal? NetAmtCur { get; set; }
    }
}
