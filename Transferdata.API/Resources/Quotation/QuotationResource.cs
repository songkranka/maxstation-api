using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Resources.Quotation
{
    public class QuotationResource
    {
        public string BrnCode { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string MaxCardId { get; set; }
        public string RefNo { get; set; }
        public List<QuotationDtResource> QuotationDt { get; set; }
    }
}
