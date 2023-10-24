#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SysBranchConfig
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public int SeqNo { get; set; }
        public int ItemNo { get; set; }
        public string ConfigId { get; set; }
        public string IsLock { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
