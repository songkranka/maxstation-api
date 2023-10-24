#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPeriodMeter
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public int PeriodNo { get; set; }
        public string DispId { get; set; }
        public string DispStatus { get; set; }
        public string PeriodStatus { get; set; }
        public string TankId { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public decimal? Unitprice { get; set; }
        public decimal? MeterMax { get; set; }
        public decimal? MeterStart { get; set; }
        public decimal? MeterFinish { get; set; }
        public decimal? SaleQty { get; set; }
        public decimal? SaleAmt { get; set; }
        public decimal? RepairStart { get; set; }
        public decimal? RepairFinish { get; set; }
        public decimal? RepairQty { get; set; }
        public decimal? RepairAmt { get; set; }
        public decimal? TestStart { get; set; }
        public decimal? TestFinish { get; set; }
        public decimal? TestQty { get; set; }
        public decimal? TestAmt { get; set; }
        public decimal? TotalQty { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
