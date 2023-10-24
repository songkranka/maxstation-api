using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InvAdjustHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string ReasonId { get; set; }
        public string ReasonDesc { get; set; }
        public string Remark { get; set; }
        public string RefNo { get; set; }
        public string BrnCodeFrom { get; set; }
        public string BrnNameFrom { get; set; }
        public string Post { get; set; }
        public int? RunNumber { get; set; }
        public string DocPattern { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
