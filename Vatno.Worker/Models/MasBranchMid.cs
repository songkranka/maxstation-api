#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBranchMid
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string MidNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
