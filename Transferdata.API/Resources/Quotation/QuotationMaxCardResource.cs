using System;

namespace Transferdata.API.Resources.Quotation
{
    public class QuotationMaxCardResource
    {
        public string BrnCode { get; set; }
        public string DocNo { get; set; }
        public  DateTime? DocDate { get; set; }
        public string MaxCardId { get; set; }
        //public string CustCode { get; set; }
        public string CustName { get; set; }
        public int ItemCount { get; set; }
        //public decimal? NetAmt { get; set; }

    }
}
