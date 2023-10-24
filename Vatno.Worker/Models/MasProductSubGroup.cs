#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasProductSubGroup
    {
        public string SubGroupId { get; set; }
        public string SubGroupName { get; set; }
        public string SubGroupStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
