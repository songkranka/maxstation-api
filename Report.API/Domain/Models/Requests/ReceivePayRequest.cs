namespace Report.API.Domain.Models.Requests
{
    public class ReceivePayRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string DocNo { get; set; }
    }

}
