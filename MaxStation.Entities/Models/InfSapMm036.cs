using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapMm036
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
        public string PoNumber { get; set; }
        public string PoItem { get; set; }
        public string NoMoreGr { get; set; }
        public string GrRcpt { get; set; }
        public string MvtInd { get; set; }
        public string DelNumToSearch { get; set; }
        public int? DelivItemToSearch { get; set; }
        public string DelivNum { get; set; }
        public string DelivItem { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RcCompCode { get; set; }
        public string RcBrnCode { get; set; }
        public string RcDocNo { get; set; }
        public DateTime RcDocDate { get; set; }
        public int? RcSeqNo { get; set; }
        public string RcPdId { get; set; }
        public string RcPdName { get; set; }
    }
}
