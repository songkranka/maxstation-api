using Report.API.Domain.Enums;
using System;

namespace Report.API.Domain.Models.Requests
{
    public class SalesRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public ReportType ReportType { get; set; }
        public string CustCodeFrom { get; set; }
        public string CustCodeTo { get; set; }
        public string PdIdFrom { get; set; }
        public string PdIdTo { get; set; }
        public EDocType DocType { get; set; }

        public enum EDocType
        {
            CashAndCredit = 0,
            Cash = 1,
            Credit = 2,
        }
    }

}
