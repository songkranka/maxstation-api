#nullable disable

namespace Vatno.Worker.Models
{
    public partial class FinBalance
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string CustCode { get; set; }
        public string Currency { get; set; }
        public decimal? NetAmt { get; set; }
        public decimal? NetAmtCur { get; set; }
        public decimal? BalanceAmt { get; set; }
        public decimal? BalanceAmtCur { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
