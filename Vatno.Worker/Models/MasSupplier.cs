#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasSupplier
    {
        public string SupCode { get; set; }
        public string MapSupCode { get; set; }
        public string SupPrefix { get; set; }
        public string SupName { get; set; }
        public string SupStatus { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string TaxId { get; set; }
        public string VatType { get; set; }
        public int? CreditTerm { get; set; }
        public string Remark { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
