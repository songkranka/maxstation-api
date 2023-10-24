using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class SalBillingDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string TxNo { get; set; }
        public DateTime? TxDate { get; set; }
        public string TxType { get; set; }
        public string TxBrnCode { get; set; }
        public decimal? TxAmt { get; set; }
        public decimal? TxAmtCur { get; set; }
    }
}
