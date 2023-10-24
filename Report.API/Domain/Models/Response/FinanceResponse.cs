namespace Report.API.Domain.Models.Response
{
    public class FinanceResponse
    {
    }

    public class Fin03Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string docDate { get; set; }
        public string custCode { get; set; }
        public string custName { get; set; }
        public string billNo { get; set; }
        public string receiveType { get; set; }
        public string payType { get; set; }
        public string bankNo { get; set; }
        public string bankName { get; set; }
        public string accountNo { get; set; }
        public string payNo { get; set; }
        public string payDate { get; set; }
        public string remark { get; set; }
        public decimal netAmt { get; set; }
        public decimal whtAmt { get; set; }
        public decimal vatAmt { get; set; }
        public decimal subAmt { get; set; }
    }
    public class Fin08Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string docDate { get; set; }
        public string docNo { get; set; }
        public string docStatus { get; set; }
        public string custCode { get; set; }
        public string custName { get; set; }
        public string pdId { get; set; }
        public decimal vatAmt { get; set; }
        public decimal taxBaseAmt { get; set; }
        public decimal subAmt { get; set; }
        //public decimal rentAmt { get; set; }
        //public decimal waterBaseAmt { get; set; }
        //public decimal waterSubAmt { get; set; }
        //public decimal electricBaseAmt { get; set; }
        //public decimal electricSubAmt { get; set; }



    }
    public class Fin09Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }

    }


}
