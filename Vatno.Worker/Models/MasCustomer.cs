#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasCustomer
    {
        public string CustCode { get; set; }
        public string MapCustCode { get; set; }
        public string CustStatus { get; set; }
        public string CustPrefix { get; set; }
        public string CustName { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string ProvCode { get; set; }
        public string ProvName { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string CitizenId { get; set; }
        public string ContactName { get; set; }
        public string BillType { get; set; }
        public string DueType { get; set; }
        public string AccountId { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public decimal? CreditLimit { get; set; }
        public int? CreditTerm { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
