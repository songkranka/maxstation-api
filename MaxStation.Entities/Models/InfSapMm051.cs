using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapMm051
    {
        public string SrcName { get; set; }
        public string PstngDate { get; set; }
        public string DocDate { get; set; }
        public string RefDocNo { get; set; }
        public string BillOfLading { get; set; }
        public string HeaderTxt { get; set; }
        public string VerGrGiSlip { get; set; }
        public string VerGrGiSlipx { get; set; }
        public string GmCode { get; set; }
        public string Zitem { get; set; }
        public string Material { get; set; }
        public string Plant { get; set; }
        public string StgeLoc { get; set; }
        public string MoveType { get; set; }
        public decimal? EntryQnt { get; set; }
        public string EntryUom { get; set; }
        public string ItemText { get; set; }
        public string GrRcpt { get; set; }
        public string UnloadPt { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AjCompCode { get; set; }
        public string AjBrnCode { get; set; }
        public string AjLocCode { get; set; }
        public string AjDocNo { get; set; }
        public DateTime AjDocDate { get; set; }
        public string AjPdId { get; set; }
        public string AjPdName { get; set; }
    }
}
