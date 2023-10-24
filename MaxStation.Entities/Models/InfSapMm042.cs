using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapMm042
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
        public string GrRcpt { get; set; }
        public string MoveMat { get; set; }
        public string MovePlant { get; set; }
        public string MoveStloc { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TfiCompCode { get; set; }
        public string TfiBrnCode { get; set; }
        public string TfiLocCode { get; set; }
        public string TfiDocNo { get; set; }
        public DateTime TfiDocDate { get; set; }
        public string TfiPdId { get; set; }
        public string TfiPdName { get; set; }
    }
}
