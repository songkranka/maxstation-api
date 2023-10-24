#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPostdayValidate
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime DocDate { get; set; }
        public int SeqNo { get; set; }
        public string ValidRemark { get; set; }
        public string ValidResult { get; set; }
    }
}
