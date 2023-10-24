﻿using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapMm041
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
        public string MoveValType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TfoCompCode { get; set; }
        public string TfoBrnCode { get; set; }
        public string TfoLocCode { get; set; }
        public string TfoDocNo { get; set; }
        public DateTime TfoDocDate { get; set; }
        public string TfoBrnCodeTo { get; set; }
        public string TfoPdId { get; set; }
        public string TfoPdName { get; set; }
    }
}
