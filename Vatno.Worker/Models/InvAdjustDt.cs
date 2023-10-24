﻿#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InvAdjustDt
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
        public decimal? RefQty { get; set; }
        public decimal? ItemQty { get; set; }
        public decimal? StockQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitCost { get; set; }
    }
}
