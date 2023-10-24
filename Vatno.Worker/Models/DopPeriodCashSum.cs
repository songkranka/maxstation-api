#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPeriodCashSum
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public int PeriodNo { get; set; }
        public decimal? SumTotalAmt { get; set; }
        public decimal? SumCrAmt { get; set; }
        public decimal? SumDrAmt { get; set; }
        public decimal? SumSlipAmt { get; set; }
        public decimal? CashAmt { get; set; }
        public decimal? RealAmt { get; set; }
        public decimal? DepositAmt { get; set; }
        public decimal? DiffAmt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
