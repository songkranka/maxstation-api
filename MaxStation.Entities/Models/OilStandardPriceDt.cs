using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class OilStandardPriceDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocNo { get; set; }
        public string PdId { get; set; }
        public string UnitBarcode { get; set; }
        public decimal? BeforePrice { get; set; }
        public decimal? AdjustPrice { get; set; }
        public decimal? CurrentPrice { get; set; }
    }
}
