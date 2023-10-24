#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SysApproveConfig
    {
        public string DocType { get; set; }
        public string DocName { get; set; }
        public int? StepCount { get; set; }
        public string Route { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
