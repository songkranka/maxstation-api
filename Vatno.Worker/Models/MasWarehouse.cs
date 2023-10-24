#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasWarehouse
    {
        public string CompCode { get; set; }
        public string WhCode { get; set; }
        public string MapWhCode { get; set; }
        public string WhName { get; set; }
        public string WhStatus { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
