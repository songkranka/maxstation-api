#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InfPoConfirmation
    {
        public string PoNumber { get; set; }
        public string PoItem { get; set; }
        public string DelivNumb { get; set; }
        public int? DelivItem { get; set; }
        public string IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string InfStatus { get; set; }
        public string ErrorMsg { get; set; }
    }
}
