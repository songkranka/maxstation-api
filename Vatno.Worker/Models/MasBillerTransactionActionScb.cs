#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBillerTransactionActionScb
    {
        public int Id { get; set; }
        public string Billerid { get; set; }
        public string Transref { get; set; }
        public string Action { get; set; }
        public string Subname { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTime? Createdate { get; set; }
        public string Createby { get; set; }
    }
}
