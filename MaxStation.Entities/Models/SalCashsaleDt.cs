using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SalCashsaleDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public bool? IsFree { get; set; }
        public string UnitId { get; set; }
        public string UnitBarcode { get; set; }
        public string UnitName { get; set; }
        public decimal? ItemQty { get; set; }
        public decimal? StockQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitPriceCur { get; set; }
        public decimal? RefPrice { get; set; }
        public decimal? RefPriceCur { get; set; }
        public decimal? SumItemAmt { get; set; }
        public decimal? SumItemAmtCur { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? DiscAmtCur { get; set; }
        public decimal? DiscHdAmt { get; set; }
        public decimal? DiscHdAmtCur { get; set; }
        public decimal? SubAmt { get; set; }
        public decimal? SubAmtCur { get; set; }
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
