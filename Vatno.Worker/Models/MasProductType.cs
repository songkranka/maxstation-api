#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasProductType
    {
        public string DocTypeId { get; set; }
        public string GroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
