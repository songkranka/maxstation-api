using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{

    public class ReceivePayResponse
    {
        //public List<finReceivePayHd> finReceivePayHd { get; set; }
        public string compName { get; set; }
        public string compAddress { get; set; }
        public string compPhone { get; set; }
        public string compFax { get; set; }
        public string compImage { get; set; }
        public string headerName { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string docNo { get; set; }
        public string docDate { get; set; }
        public string custCode { get; set; }
        public string custName { get; set; }
        public string custAddr1 { get; set; }
        public string custAddr2 { get; set; }
        public string citizenId { get; set; }
        public string receiveTypeId { get; set; }
        public string receiveType { get; set; }
        public string payTypeId { get; set; }
        public string payType { get; set; }
        public string payDate { get; set; }
        public string bankNo { get; set; }
        public string bankName { get; set; }
        public string accountNo { get; set; }
        public string payNo { get; set; }
        public string remark { get; set; }
        public decimal totalAmt { get; set; }
        public decimal vatAmt { get; set; }
        public decimal netAmt { get; set; }
        public string netAmtText { get; set; }        

        public List<finReceivePayDt> finReceivePayDt { get; set; }


    }


    public class finReceivePayDt
    {
        public string pdId { get; set; }
        public string pdName { get; set; }
        public decimal itemAmt { get; set; }
    }



}
