#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPeriodLog
    {
        public int LogNo { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime? DocDate { get; set; }
        public int? PeriodNo { get; set; }
        public string JsonData { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
