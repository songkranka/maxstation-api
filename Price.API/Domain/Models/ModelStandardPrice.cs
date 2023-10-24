using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price.API.Domain.Models
{
    public class ModelStandardPrice
    {
        public OilStandardPriceHd Header { get; set; }
        public OilStandardPriceDt[] ArrayDetail { get; set; }
    }
    public class ModelStandardPriceParam
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }

    }
    public class ModelStandardPriceResult
    {
        public OilStandardPriceHd[] ArrayHeader { get; set; }
        public MasEmployee[] ArrayEmployee { get; set; }
        public int TotalItems { get; set; }
    }
    public class ModelStandardPriceProduct
    {
        public MasProduct[] ArrayProduct { get; set; }
        public MasProductPrice[] ArrayProductPrice { get; set; }
        public OilStandardPriceDt[] ArrayStandardPriceDetail { get; set; }
    }
    public class ModelGetArrayStandardPriceDetailParam
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }

    }
}
