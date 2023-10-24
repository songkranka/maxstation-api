using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Requests
{
    public class ReportStockRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }        
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string ProductIdStart { get; set; }
        public string ProductIdEnd { get; set; }
        public string ProductGroupIdStart { get; set; }
        public string ProductGroupIdEnd { get; set; }
    }
}
