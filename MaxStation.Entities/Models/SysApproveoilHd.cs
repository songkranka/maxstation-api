using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SysApproveoilHd
    {
        public string CompCode { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string ApproveStatus { get; set; }
        public string ApproveBy { get; set; }
        public string ApproveName { get; set; }
        public string Remark { get; set; }
        public int? RunNumber { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
