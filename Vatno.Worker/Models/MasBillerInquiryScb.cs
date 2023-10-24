#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasBillerInquiryScb
    {
        public int Id { get; set; }
        public string Billerid { get; set; }
        public string Eventcode { get; set; }
        public string Transactiontype { get; set; }
        public string Reverseflag { get; set; }
        public string Payeeproxyid { get; set; }
        public string Payeeproxytype { get; set; }
        public string Payeeaccountnumber { get; set; }
        public string Payeename { get; set; }
        public string Payerproxyid { get; set; }
        public string Payerproxytype { get; set; }
        public string Payeraccountnumber { get; set; }
        public string Payername { get; set; }
        public string Sendingbankcode { get; set; }
        public string Receivingbankcode { get; set; }
        public string Amount { get; set; }
        public string Transactionid { get; set; }
        public string Fasteasyslipnumber { get; set; }
        public string Transactiondateandtime { get; set; }
        public string Billpaymentref1 { get; set; }
        public string Billpaymentref2 { get; set; }
        public string Billpaymentref3 { get; set; }
        public string Currencycode { get; set; }
        public string Equivalentamount { get; set; }
        public string Equivalentcurrencycode { get; set; }
        public string Exchangerate { get; set; }
        public string Channelcode { get; set; }
        public string Partnertransactionid { get; set; }
        public string Tepacode { get; set; }
        public DateTime? Createdate { get; set; }
        public string Createby { get; set; }
    }
}
