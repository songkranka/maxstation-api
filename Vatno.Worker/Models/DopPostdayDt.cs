#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPostdayDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime DocDate { get; set; }
        public string DocType { get; set; }
        public int SeqNo { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public int? DocNo { get; set; }
        public int? DocStart { get; set; }
        public int? DocFinish { get; set; }
        public int? Total { get; set; }
        public decimal? Amount { get; set; }
    }
}
