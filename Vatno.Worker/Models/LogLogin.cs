#nullable disable

namespace Vatno.Worker.Models
{
    public partial class LogLogin
    {
        public int LogNo { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string EmpCode { get; set; }
        public string IpAddress { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
