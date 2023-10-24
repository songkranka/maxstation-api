#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SalCashsaleLog
    {
        public int LogNo { get; set; }
        public string LogStatus { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string JsonData { get; set; }
        public string MessageLog { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
