using System;

namespace PostDay.API.Domain.Models.Request
{
    public class POSPeriodWaterRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime FromDate { get; set; }
    }
}
