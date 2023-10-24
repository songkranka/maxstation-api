using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class MasGl
    {
        public string GlNo { get; set; }
        public int? SeqNo { get; set; }
        public string GlType { get; set; }
        public string GlAccount { get; set; }
        public string GlStatus { get; set; }
        public string GlDesc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
