using System;

namespace Report.API.Domain.Models.Requests
{
    public class FinanceRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string CustCodeFrom { get; set; }
        public string CustCodeTo { get; set; }
    }
}
