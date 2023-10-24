#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasPosition
    {
        public string PositionCode { get; set; }
        public string PositionStatus { get; set; }
        public string PositionName { get; set; }
        public string PositionDesc { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
