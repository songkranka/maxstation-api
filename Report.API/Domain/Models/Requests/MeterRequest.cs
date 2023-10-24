using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Requests
{
    public partial class MeterRequest
    {
        public class MeterTestResquest
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }
    }
}
