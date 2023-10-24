#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPosLog
    {
        public int LogNo { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime? DocDate { get; set; }
        public int? PayGroupId { get; set; }
        public int? Period { get; set; }
        public int? ItemCount { get; set; }
        public string JsonData { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
