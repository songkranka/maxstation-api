#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopFormula
    {
        public int FmNo { get; set; }
        public string FmStatus { get; set; }
        public string FmName { get; set; }
        public string Remark { get; set; }
        public string SourceName { get; set; }
        public string SourceType { get; set; }
        public string SourceKey { get; set; }
        public string SourceValue { get; set; }
        public string DestinationType { get; set; }
        public string DestinationValue { get; set; }
        public string UnitName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
