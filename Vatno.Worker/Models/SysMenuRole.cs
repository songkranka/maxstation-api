#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SysMenuRole
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string MenuId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
