using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.Request
{
    public class POSPeriodCountRequest
    {
        public string BrnCode { get; set; }
        public DateTime FromDate { get; set; }
    }
}
