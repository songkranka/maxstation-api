using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.CreditNote
{
    public class CreditNoteQueryResource : QueryResource
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

    public class CreditNoteQueryResource2
    {
        public SalCndnHd CreditNoteHeader { get; set; }
        public SalCndnDt[] ArrCreditNoteDetail { get; set; }
        public FinBalance FinBalance { get; set; }
    }

    public class SearchTaxInvoiceParam
    {
        public string CompCode { get; set; }
        public string CustCode { get; set; }
        public string DocNo { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
    }

    public class SearchTaxInvoiceResult
    {
        public SalTaxinvoiceHd[] ArrTaxInvoice { get; set; }
        public int TotalItem { get; set; }
    }
}
