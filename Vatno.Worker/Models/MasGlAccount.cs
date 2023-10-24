#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasGlAccount
    {
        public string CompCode { get; set; }
        public string AcctCode { get; set; }
        public string AcctStatus { get; set; }
        public string AcctName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
