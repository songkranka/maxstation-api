#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InvReceiveProdHd
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public string PoTypeId { get; set; }
        public string PoNo { get; set; }
        public DateTime? PoDate { get; set; }
        public string SupCode { get; set; }
        public string SupName { get; set; }
        public string InvNo { get; set; }
        public DateTime? InvDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string VatType { get; set; }
        public string InvAddrId { get; set; }
        public string InvAddress { get; set; }
        public string PayAddrId { get; set; }
        public string PayAddress { get; set; }
        public string Remark { get; set; }
        public int? ItemCount { get; set; }
        public string Currency { get; set; }
        public decimal? CurRate { get; set; }
        public decimal? SubAmt { get; set; }
        public decimal? SubAmtCur { get; set; }
        public string DiscRate { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? DiscAmtCur { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TotalAmtCur { get; set; }
        public decimal? TaxBaseAmt { get; set; }
        public decimal? TaxBaseAmtCur { get; set; }
        public int? VatRate { get; set; }
        public decimal? VatAmt { get; set; }
        public decimal? VatAmtCur { get; set; }
        public decimal? ShippingAmt { get; set; }
        public decimal? ShippingAmtCur { get; set; }
        public decimal? EtaxAmt { get; set; }
        public decimal? EtaxAmtCur { get; set; }
        public decimal? NetAmt { get; set; }
        public decimal? NetAmtCur { get; set; }
        public string Source { get; set; }
        public string Post { get; set; }
        public int? RunNumber { get; set; }
        public string DocPattern { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string DeliveryNo { get; set; }
    }
}
