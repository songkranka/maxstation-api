using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasProduct
    {
        public string PdId { get; set; }
        public string MapPdId { get; set; }
        public string PdName { get; set; }
        public string PdStatus { get; set; }
        public string PdDesc { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string GroupId { get; set; }
        public string SubGroupId { get; set; }
        public string VatType { get; set; }
        public int? VatRate { get; set; }
        public string PdType { get; set; }
        public string AcctCode { get; set; }
        public string PdImage { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
