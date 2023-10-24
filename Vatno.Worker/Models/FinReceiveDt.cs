#nullable disable

namespace Vatno.Worker.Models
{
    public partial class FinReceiveDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public int SeqNo { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public string AccountNo { get; set; }
        public string Remark { get; set; }
        public decimal? ItemAmt { get; set; }
        public decimal? ItemAmtCur { get; set; }
        public string VatType { get; set; }
        public int? VatRate { get; set; }
        public decimal? VatAmt { get; set; }
        public decimal? VatAmtCur { get; set; }
    }
}
