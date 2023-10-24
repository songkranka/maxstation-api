using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class CustomerResponse
    {
    }
    public class Debtor02Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string docType { get; set; }
        public string custCode { get; set; }
        public string custName { get; set; }
        public string docDate { get; set; }
        public string docNo { get; set; }
        public decimal netAmt { get; set; }
        public decimal balanceAmt { get; set; }
        //public decimal crdrAmt { get; set; }
        //public decimal remainAmt { get; set; }
        //public decimal totalRemainAmt { get; set; }

    }


}
