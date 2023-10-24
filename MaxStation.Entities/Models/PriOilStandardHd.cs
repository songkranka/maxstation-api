using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class PriOilStandardHd
    {
        public string CompCode { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string DocStatus { get; set; }
        public string DocTypeId { get; set; }
        public string DocTypeDesc { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Remark { get; set; }
        public string ApproveStatus { get; set; }
        public DateTime? ApproveDate { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
