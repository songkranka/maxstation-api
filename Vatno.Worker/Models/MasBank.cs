#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBank
    {
        public string CompCode { get; set; }
        public string BankCode { get; set; }
        public string AccountNo { get; set; }
        public string BankStatus { get; set; }
        public string BankName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
