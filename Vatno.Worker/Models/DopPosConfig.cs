#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPosConfig
    {
        public string DocType { get; set; }
        public string DocDesc { get; set; }
        public string DocStatus { get; set; }
        public string PdId { get; set; }
        public string ApiUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
