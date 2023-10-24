using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SysApproveDt
    {
        public string CompCode { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string RefNo { get; set; }
        public string RefTypeDesc { get; set; }
        public DateTime? RefDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
