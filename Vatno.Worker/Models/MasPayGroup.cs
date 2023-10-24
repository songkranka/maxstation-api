#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasPayGroup
    {
        public int PayGroupId { get; set; }
        public string PayGroupName { get; set; }
        public string PayGroupRemark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
