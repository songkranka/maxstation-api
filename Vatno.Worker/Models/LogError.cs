#nullable disable

namespace Vatno.Worker.Models
{
    public partial class LogError
    {
        public int LogNo { get; set; }
        public string LogStatus { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string JsonData { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Host { get; set; }
        public string StackTrace { get; set; }
    }
}
