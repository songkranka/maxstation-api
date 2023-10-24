using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.Response
{
    public class PostCreditResponse
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        public CreditSummaryResponse Data { get; set; }
        public class CreditSummaryResponse
        {
            public decimal Amount { get; set; }
        }
    }
}
