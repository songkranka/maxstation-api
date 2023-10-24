#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasOrganize
    {
        public string OrgCodedev { get; set; }
        public string OrgComp { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string OrgShopid { get; set; }
        public string MpPlant { get; set; }
        public string StatPosMart { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
