﻿using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SalCndnDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public string UnitId { get; set; }
        public string UnitBarcode { get; set; }
        public string UnitName { get; set; }
        public decimal? BeforePrice { get; set; }
        public decimal? BeforeQty { get; set; }
        public decimal? BeforeAmt { get; set; }
        public decimal? BeforeAmtCur { get; set; }
        public decimal? AfterPrice { get; set; }
        public decimal? AfterQty { get; set; }
        public decimal? AfterAmt { get; set; }
        public decimal? AfterAmtCur { get; set; }
        public decimal? AdjustQty { get; set; }
        public decimal? AdjustAmt { get; set; }
        public decimal? AdjustAmtCur { get; set; }
        public string VatType { get; set; }
        public int? VatRate { get; set; }
        public decimal? VatAmt { get; set; }
        public decimal? VatAmtCur { get; set; }
        public decimal? TaxBaseAmt { get; set; }
        public decimal? TaxBaseAmtCur { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalAmtCur { get; set; }
    }
}
