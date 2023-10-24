using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class InfSapMm052
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
        public string Costcenter { get; set; }
        public string Orderid { get; set; }
        public string GlAccount { get; set; }
        public string Zcustomer { get; set; }
        public string Zcoscenter { get; set; }
        public DateTime CreatedDate { get; set; }
        public string GiCompCode { get; set; }
        public string GiBrnCode { get; set; }
        public string GiLocCode { get; set; }
        public string GiDocNo { get; set; }
        public DateTime GiDocDate { get; set; }
        public string GiPdId { get; set; }
        public string GiPdName { get; set; }
    }
}
