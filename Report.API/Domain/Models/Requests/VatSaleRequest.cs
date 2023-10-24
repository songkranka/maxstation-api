namespace Report.API.Domain.Models.Requests
{
    public class VatSaleRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

}
