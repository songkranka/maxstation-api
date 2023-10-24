using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
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

    public class ModelProduct
    {
        public MasProduct Product { get; set; }
        public MasProductUnit[] ArrProductUnit { get; set; }
    }
}
