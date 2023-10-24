#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SysBranchConfigRole
    {
        public string PositionCode { get; set; }
        public int ItemNo { get; set; }
        public string ConfigId { get; set; }
        public string IsView { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
