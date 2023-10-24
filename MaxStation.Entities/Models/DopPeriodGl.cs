using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class DopPeriodGl
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string GlType { get; set; }
        public string GlNo { get; set; }
        public int? GlSeqNo { get; set; }
        public string GlAccount { get; set; }
        public string GlStatus { get; set; }
        public string GlDesc { get; set; }
        public string GlLock { get; set; }
        public string GlSlip { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
