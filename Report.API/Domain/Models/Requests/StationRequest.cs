using System;

namespace Report.API.Domain.Models.Requests
{
    public class StationRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string Period { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
