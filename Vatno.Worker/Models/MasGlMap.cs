#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasGlMap
    {
        public string GlNo { get; set; }
        public int MopCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
