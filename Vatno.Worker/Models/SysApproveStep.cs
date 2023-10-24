#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SysApproveStep
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public int StepNo { get; set; }
        public string DocType { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string ApprCode { get; set; }
        public string ApprStatus { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
