using System;

namespace Report.API.Domain.Models.Requests
{
    public partial class WithdrawRequest
    {
        public class GetDocumentRequest
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public string docNo { get; set; }
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }
    }
}
