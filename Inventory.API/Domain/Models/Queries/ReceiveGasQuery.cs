using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models.Queries
{
    public class ReceiveGasQuery
    {
        public InvReceiveProdHd Header { get; set; }
        public InvReceiveProdDt[] Details { get; set; }
    }

    public class ReceiveGasListQuery //: Query
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public string LocCode { get; set; }
        public string DocType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
    }

    public class PoHeaderListQuery// : InfPoHeader
    {
        public string CompCode { get; set; }
        public string KeyWord { get; set; }
        public DateTime? SystemDate { get; set; }
        public string BrnCode { get; set; }
    }

    public class PoItemListResult
    {
        public MasProduct[] ArrProduct { get; set; }
        public InfPoItem[] ArrPoItem { get; set; }
        public MasUnit[] ArrUnit { get; set; }
        public MasDensity Density { get; set; }
    }

    public class PoItemListParam
    {
        public string PoNumber { get; set; }
        public string CompCode { get; set; }
        public DateTime? SystemDate { get; set; }

    }


    public class ModelSupplierResult
    {
        public MasSupplier Supplier { get; set; }
        //public MasSupplierPay SupplierPay { get; set; }
        public MasMapping[] ArrayMapping { get; set; }
    }
}
