#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SysMenu
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuStatus { get; set; }
        public string Parent { get; set; }
        public string Child { get; set; }
        public string Route { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
