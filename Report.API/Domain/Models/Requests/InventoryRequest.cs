using Report.API.Domain.Enums;
using System;

namespace Report.API.Domain.Models.Requests
{
    public class InventoryRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public ReportType ReportType { get; set; }
        public string ProductIdStart { get; set; }
        public string ProductIdEnd { get; set; }
        public string ProductGroupIdStart { get; set; }
        public string ProductGroupIdEnd { get; set; }
    }
}
