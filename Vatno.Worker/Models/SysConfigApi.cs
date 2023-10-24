#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SysConfigApi
    {
        public string SystemId { get; set; }
        public string ApiId { get; set; }
        public string ApiDesc { get; set; }
        public string ApiUrl { get; set; }
        public string Method { get; set; }
        public string Topic { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
