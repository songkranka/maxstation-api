using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.Invoice
{
    public class InvoiceQueryResource : QueryResource
    {
        public string COMP_CODE { get; set; }
        public string BRN_CODE { get; set; }
        public string KeyWord { get; set; }
        public string LOC_CODE { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetInvoiceQueryResource : QueryResource
    {
        public string LocCode { get; set; }
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public string DocNo { get; set; }
    }
}
