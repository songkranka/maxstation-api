using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class FinReceivePay
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string ItemType { get; set; }
        public string BillBrnCode { get; set; }
        public string BillNo { get; set; }
        public string TxBrnCode { get; set; }
        public string TxNo { get; set; }
        public DateTime? TxDate { get; set; }
        public decimal? TxAmt { get; set; }
        public decimal? TxBalance { get; set; }
        public decimal? PayAmt { get; set; }
        public decimal? RemainAmt { get; set; }
    }
}
