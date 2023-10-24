#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBillerTransactionScb
    {
        public int Id { get; set; }
        public string Billerid { get; set; }
        public string Amount { get; set; }
        public string Transref { get; set; }
        public string Transtime { get; set; }
        public string Ref3 { get; set; }
        public string Countrycode { get; set; }
        public string Sendingbank { get; set; }
        public string Receivingbank { get; set; }
        public string Transdate { get; set; }
        public string Ref2 { get; set; }
        public string Ref1 { get; set; }
        public DateTime? Createdate { get; set; }
        public string Createby { get; set; }
    }
}
