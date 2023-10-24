#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBranchPeriod
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public int PeriodNo { get; set; }
        public string TimeStart { get; set; }
        public string TimeFinish { get; set; }
    }
}
