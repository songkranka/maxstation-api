using Common.API.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Domain.Models
{
    public class RequestGetRequestList : QueryResource
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocDate { get; set; }
    }

    public class ProductDisplayResponse 
    { 
        public DateTime LastEffectiveDate { get; set; }
        public List<ProductDisplayItem> Items { get; set; }
    }

    public class ProductDisplayItem
    {
        public string PdId { get; set; }
        public string PdName { get; set; }
        public string PdImage { get; set; }
        public decimal Unitprice { get; set; }
    }

    public class OilPrice
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string PdId { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
