using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.Response
{
    public class POSPeriodCountResponse
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        public PeriodCountResponse Data { get; set; }
        public class PeriodCountResponse
        {
            public int CountPeriod { get; set; }
        }
    }
}
