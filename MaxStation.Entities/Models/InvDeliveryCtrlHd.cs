using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InvDeliveryCtrlHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public string ReceiveNo { get; set; }
        public DateTime? RealDate { get; set; }
        public string WhId { get; set; }
        public string WhName { get; set; }
        public string LicensePlate { get; set; }
        public string CarNo { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string CtrlCorrect { get; set; }
        public string CtrlCorrectReasonId { get; set; }
        public string CtrlCorrectReasonDesc { get; set; }
        public string CtrlCorrectOther { get; set; }
        public string CtrlOntime { get; set; }
        public int? CtrlOntimeLate { get; set; }
        public string CtrlDoc { get; set; }
        public string CtrlDocDesc { get; set; }
        public string CtrlSeal { get; set; }
        public int? CtrlSealStart { get; set; }
        public int? CtrlSealFinish { get; set; }
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
