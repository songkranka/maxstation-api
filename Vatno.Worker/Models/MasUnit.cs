#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasUnit
    {
        public string UnitId { get; set; }
        public string MapUnitId { get; set; }
        public string UnitStatus { get; set; }
        public string UnitName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
