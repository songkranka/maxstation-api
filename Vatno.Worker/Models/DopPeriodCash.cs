#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPeriodCash
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public int PeriodNo { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public decimal? MeterAmt { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? SaleAmt { get; set; }
        public decimal? CreditAmt { get; set; }
        public decimal? CashAmt { get; set; }
        public decimal? CouponAmt { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? TotalAmt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
