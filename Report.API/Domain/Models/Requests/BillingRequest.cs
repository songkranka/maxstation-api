using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Requests
{
    public class BillingRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocNo { get; set; }
        public string EmpCode { get; set; }
        public object DateFrom { get; internal set; }
    }
}
