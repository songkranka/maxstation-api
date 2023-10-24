#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasSapCustomer
    {
        public string SrcName { get; set; }
        public string LegCode { get; set; }
        public string SapCode { get; set; }
        public string SapAltCode { get; set; }
        public string CustomerName { get; set; }
        public string BillingCust { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
