using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models.Request
{
    public class CreditSummaryRequest
    {
        public string BrnCode { get; set; }
        public DateTime FromDate { get; set; }
    }
}
