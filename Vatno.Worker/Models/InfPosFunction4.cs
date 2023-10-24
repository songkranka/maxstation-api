#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InfPosFunction4
    {
        public string SiteId { get; set; }
        public DateTime BusinessDate { get; set; }
        public string ShiftNo { get; set; }
        public string JournalId { get; set; }
        public DateTime? JournalDate { get; set; }
        public string JournalStatus { get; set; }
        public int? PosId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public int? ShiftId { get; set; }
        public int? TransNo { get; set; }
        public string Billno { get; set; }
        public string Taxinvno { get; set; }
        public string MaxCardNumber { get; set; }
        public decimal? TotalGoodsamt { get; set; }
        public decimal? TotalDiscamt { get; set; }
        public decimal? TotalTaxamt { get; set; }
        public decimal? TotalPaidAmt { get; set; }
        public string CustomerId { get; set; }
        public string LicNo { get; set; }
        public string UserCardNo { get; set; }
        public string Miles { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
