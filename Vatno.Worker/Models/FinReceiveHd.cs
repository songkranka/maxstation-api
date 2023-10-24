#nullable disable

namespace Vatno.Worker.Models
{
    public partial class FinReceiveHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public string ReceiveTypeId { get; set; }
        public string ReceiveType { get; set; }
        public string BillNo { get; set; }
        public string CustCode { get; set; }
        public string CustName { get; set; }
        public string CustAddr1 { get; set; }
        public string CustAddr2 { get; set; }
        public string PayTypeId { get; set; }
        public string PayType { get; set; }
        public DateTime? PayDate { get; set; }
        public string BankNo { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string PayNo { get; set; }
        public string Remark { get; set; }
        public decimal? SubAmt { get; set; }
        public decimal? SubAmtCur { get; set; }
        public decimal? FeeAmt { get; set; }
        public decimal? FeeAmtCur { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? DiscAmtCur { get; set; }
        public decimal? WhtAmt { get; set; }
        public decimal? WhtAmtCur { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalAmtCur { get; set; }
        public string VatType { get; set; }
        public int? VatRate { get; set; }
        public decimal? VatAmt { get; set; }
        public decimal? VatAmtCur { get; set; }
        public decimal? NetAmt { get; set; }
        public decimal? NetAmtCur { get; set; }
        public string Post { get; set; }
        public int? RunNumber { get; set; }
        public string DocPattern { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string EtlLotNo { get; set; }
    }
}
