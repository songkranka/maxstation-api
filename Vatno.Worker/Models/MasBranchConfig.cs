#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBranchConfig
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string IsPos { get; set; }
        public string IsLockMeter { get; set; }
        public string IsLockSlip { get; set; }
        public string Trader { get; set; }
        public string TraderPosition { get; set; }
        public string ReportTaxType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
