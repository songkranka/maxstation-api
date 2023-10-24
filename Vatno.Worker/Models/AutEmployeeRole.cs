#nullable disable

namespace Vatno.Worker.Models
{
    public partial class AutEmployeeRole
    {
        public string EmpCode { get; set; }
        public int? AuthCode { get; set; }
        public string PositionCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
