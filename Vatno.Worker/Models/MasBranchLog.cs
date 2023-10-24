#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBranchLog
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public int SeqNo { get; set; }
        public string JsonData { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
