#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InvAuditDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public string UnitId { get; set; }
        public string UnitBarcode { get; set; }
        public string UnitName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? BalanceQty { get; set; }
        public decimal? ItemQty { get; set; }
        public decimal? DiffQty { get; set; }
        public decimal? AdjustQty { get; set; }
        public decimal? NoadjQty { get; set; }
    }
}
