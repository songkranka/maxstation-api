using System.Collections.Generic;

namespace Report.API.Domain.Models.Response
{
    public class CreditNoteResponse
    {

        public class CreditNoteHd
        {
            public string compName { get; set; }
            public string compAddress { get; set; }
            public string compPhone { get; set; }
            public string compFax { get; set; }
            public string compRegisterId { get; set; }
            public string compImage { get; set; }

            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }
            public string empCode { get; set; }
            public string empName { get; set; }

            public string docNo { get; set; }
            public string docDate { get; set; }
            public string txNo { get; set; }
            public string reasonDesc { get; set; }
            public string custCode { get; set; }
            public string custName { get; set; }
            public string custAddr1 { get; set; }
            public string custAddr2 { get; set; }
            public string citizenId { get; set; }
            public decimal? subAmt { get; set; }
            public decimal? vatAmt { get; set; }
            public decimal? netAmt { get; set; }
            public string netAmtText { get; set; }
            public string docType { get; set; }
            public string taxInvoiceDocDate { get; set; }

            public List<CreditNoteDt> items = new List<CreditNoteDt>();

        }

        public class CreditNoteDt {
            public int seqNo { get; set; }
            public string pdId { get; set; }
            public string pdName { get; set; }
            public string unitName { get; set; }
            public decimal? beforePrice { get; set; }
            public decimal? afterPrice { get; set; }
            public decimal? beforeQty { get; set; }
            public decimal? afterQty { get; set; }
            public decimal? beforeAmt { get; set; }
            public decimal? afterAmt { get; set; }
        }

    }


}
