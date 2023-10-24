#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasSapPlant
    {
        public string SrcName { get; set; }
        public string LegCode { get; set; }
        public string Cca { get; set; }
        public string Description { get; set; }
        public string Kokrs { get; set; }
        public string Pca { get; set; }
        public string BusinessArea { get; set; }
        public string TextCsksGsber { get; set; }
        public string Plant { get; set; }
        public string PlantName { get; set; }
        public string BusinessPlace { get; set; }
        public string IsDeleted { get; set; }
        public string PccaCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
