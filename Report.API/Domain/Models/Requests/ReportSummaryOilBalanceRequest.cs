using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Requests
{
    public class ReportSummaryOilBalanceRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocDate { get; set; }
    }
}
