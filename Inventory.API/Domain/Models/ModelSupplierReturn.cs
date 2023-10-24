using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models
{
        public class ModelGetArrayPoHeaderParam
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public string LocCode { get; set; }
        }

        public class ModelGetArrayPoItemResult
        {
            public InfPoItem[] ArrPoItem { get; set; }
            public MasProduct[] ArrayProduct { get; set; }
            public MasUnit[] ArrayUnit { get; set; }
        }

        public class ModelSupplierReturn
        {
            public InvReturnSupHd Header { get; set; }
            public InvReturnSupDt[] ArrayDetail { get; set; }
        }
        public class ModelSupplierReturnParam
        {
            public string BrnCode { get; set; }
            public string CompCode { get; set; }
            public string LocCode { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int ItemsPerPage { get; set; }

        }
        public class ModelSupplierReturnResult
    {
            public InvReturnSupHd[] ArraySupplierReturnHeader { get; set; }
            public MasEmployee[] ArrayEmployee { get; set; }
            public int TotalItems { get; set; }
        }
    
}
