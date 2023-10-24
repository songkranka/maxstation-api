#nullable disable

namespace Vatno.Worker.Models
{
    public partial class SalBillingHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public string CustCode { get; set; }
        public string CitizenId { get; set; }
        public string CustName { get; set; }
        public string CustAddr1 { get; set; }
        public string CustAddr2 { get; set; }
        public decimal? CreditLimit { get; set; }
        public int? CreditTerm { get; set; }
        public string DueType { get; set; }
        public DateTime? DueDate { get; set; }
        public int? ItemCount { get; set; }
        public string Remark { get; set; }
        public string Currency { get; set; }
        public decimal? CurRate { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalAmtCur { get; set; }
        public string Post { get; set; }
        public int? RunNumber { get; set; }
        public string DocPattern { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
