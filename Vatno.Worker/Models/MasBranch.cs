#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBranch
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string MapBrnCode { get; set; }
        public string LocCode { get; set; }
        public string BrnName { get; set; }
        public string BrnStatus { get; set; }
        public string BranchNo { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string ProvCode { get; set; }
        public string Province { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public int? PosCount { get; set; }
        public DateTime? CloseDate { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
