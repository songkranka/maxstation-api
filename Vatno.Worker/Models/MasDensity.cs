#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasDensity
    {
        public string CompCode { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? DensityBase { get; set; }
        public string DensityDesc { get; set; }
        public string CalculateType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
