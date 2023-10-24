#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPostdaySum
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime DocDate { get; set; }
        public int SeqNo { get; set; }
        public int? FmNo { get; set; }
        public string Remark { get; set; }
        public decimal? SumHead { get; set; }
        public decimal? SumDetail { get; set; }
        public string UnitName { get; set; }
    }
}
