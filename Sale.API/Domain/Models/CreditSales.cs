using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Models
{
    public class CreditSales
    {
    }

    public class Products : MasProduct
    {
        public string UnitBarcode { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class ProductRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string PdGroupID { get; set; }
        public string DocNo { get; set; }
        public string DocType { get; set; }
        public string DocumentTypeID { get; set; }
        public DateTime? DocDate { get; set; }
        public string Keyword { get; set; }
        public string PDListID { get; set; }
        public string PDBarcodeList { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
