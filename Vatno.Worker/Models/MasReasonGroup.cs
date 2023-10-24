#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasReasonGroup
    {
        public string ReasonGroup { get; set; }
        public string ReasonId { get; set; }
        public string GroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
