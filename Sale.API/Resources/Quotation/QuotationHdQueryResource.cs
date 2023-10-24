using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.Quotation
{
    public class QuotationHdQueryResource : QueryResource
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string Keyword { get; set; }
        public string Guid { get; set; }
        public string PDListID { get; set; }
        public string DocStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
