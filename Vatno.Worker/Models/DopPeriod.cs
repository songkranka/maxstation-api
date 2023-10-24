#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPeriod
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public int PeriodNo { get; set; }
        public string TimeStart { get; set; }
        public string TimeFinish { get; set; }
        public decimal? SumMeterSaleQty { get; set; }
        public decimal? SumMeterTotalQty { get; set; }
        public string IsPos { get; set; }
        public string Post { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string EtlLotNo { get; set; }
    }
}
