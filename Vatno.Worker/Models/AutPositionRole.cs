#nullable disable

namespace Vatno.Worker.Models
{
    public partial class AutPositionRole
    {
        public string PositionCode { get; set; }
        public string MenuId { get; set; }
        public string PostnameThai { get; set; }
        public string IsView { get; set; }
        public string IsCreate { get; set; }
        public string IsEdit { get; set; }
        public string IsCancel { get; set; }
        public string IsPrint { get; set; }
        public string JsonData { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
