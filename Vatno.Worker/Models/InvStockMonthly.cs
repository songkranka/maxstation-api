#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InvStockMonthly
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public int YearNo { get; set; }
        public int MonthNo { get; set; }
        public string PdId { get; set; }
        public string UnitId { get; set; }
        public string UnitBarcode { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
