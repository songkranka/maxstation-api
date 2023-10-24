using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price.API.Domain.Models
{
    public class ModelNonOilPrice
    {
        public PriNonoilDt[] ArrDetail { get; set; }
        public PriNonoilHd Header { get; set; }
    }

    public class ModelNonOilPriceParam
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }

    }

    public class ModelNonOilPriceResult
    {
        public PriNonoilHd[] ArrayHeader { get; set; }
        public MasEmployee[] ArrayEmployee { get; set; }
        public int TotalItems { get; set; }
    }
}
