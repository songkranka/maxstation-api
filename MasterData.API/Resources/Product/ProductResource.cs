using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Resources.Product
{
    public class ProductResource
    {
        public string PdId { get; set; }
        public string PdName { get; set; }
        public string PdStatus { get; set; }
        public string PdDesc { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string GroupId { get; set; }
        public string VatType { get; set; }
        public int? VatRate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string UnitBarcode { get; set; }
    }
}
