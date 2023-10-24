using System.Collections.Generic;

namespace Report.API.Domain.Models.Response
{
    public class TaxInvoiceResponse
    {
        public string docType { get; set; }
        public string docNo { get; set; }
        public string docDate { get; set; }
        public string compCode { get; set; }
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
        public string sapCustCode { get; set; }
        public string ref2 { get; set; }
        public string empName { get; set; }

        public string custCode { get; set; }
        public string custName { get; set; }
        public string custAddress1 { get; set; }
        public string custAddress2 { get; set; }
        public string custCitizenId { get; set; }
        public string refDocNo { get; set; }
        public decimal taxBaseAmt { get; set; }
        public decimal taxBaseAmtHd { get; set; }
        public decimal vatAmt { get; set; }
        public decimal subAmt { get; set; }
        public decimal netAmt { get; set; }
        public string netAmtText { get; set; }
        public string licensePlate { get; set; }        
        public List<TaxInvoiceItem> items { get; set; }

    }

    public class TaxInvoiceItem
    {
        public int seqNo { get; set; }
        public string licensePlate { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string unitName { get; set; }
        public decimal itemQty { get; set; }
        public decimal unitPrice { get; set; }
        public decimal discAmt { get; set; }
        public decimal subAmt { get; set; }
    }



}
