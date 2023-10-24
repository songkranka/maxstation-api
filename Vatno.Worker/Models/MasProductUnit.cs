#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasProductUnit
    {
        public string PdId { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitBarcode { get; set; }
        public int? UnitRatio { get; set; }
        public int? UnitStock { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
