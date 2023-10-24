﻿#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InvTranoutHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public string RefNo { get; set; }
        public string BrnCodeTo { get; set; }
        public string BrnNameTo { get; set; }
        public string Remark { get; set; }
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
