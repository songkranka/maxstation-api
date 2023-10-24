#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasCompanyMapping
    {
        public string CompanyCode { get; set; }
        public string ComLegCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
