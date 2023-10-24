#nullable disable

namespace Vatno.Worker.Models
{
    public partial class DopPostdayHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime DocDate { get; set; }
        public string Remark { get; set; }
        public decimal? CashAmt { get; set; }
        public decimal? DiffAmt { get; set; }
        public decimal? DepositAmt { get; set; }
        public decimal? ChequeAmt { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string EtlLotNo { get; set; }
    }
}
