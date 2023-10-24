using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class BillingResponse
    {
        public string docNo { get; set; }
        public string docDate { get; set; }
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compAddress { get; set; }
        public string compPhone { get; set; }
        public string compFax { get; set; }
        public string compRegisterId { get; set; }
        public string compImage { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }
        public string empName { get; set; }
        public string custCode { get; set; }
        public string citizenId { get; set; }
        public string custName { get; set; }
        public string custAddress1 { get; set; }
        public string custAddress2 { get; set; }
        public decimal creditLimit { get; set; }
        public int creditTerm { get; set; }
        public string dueType { get; set; }
        public string dueDate { get; set; }
        public int itemCount { get; set; }
        public string remark { get; set; }
        public string currency { get; set; }
        public decimal curRate { get; set; }
        public decimal totalAmt { get; set; }
        public decimal totalAmtCur { get; set; }
        public int countBilling { get; set; }
        public decimal sumBilling { get; set; }
        public string firstDate { get; set; }
        public string lastDate { get; set; }
        public List<BillingItem> items { get; set; }

        public class BillingItem
        {
            public string docNo { get; set; }
            public int seqNo { get; set; }
            public string txNo { get; set; }
            public string txDate { get; set; }
            public string txType { get; set; }
            public string txBrnCode { get; set; }
            public decimal txAmt { get; set; }
            public decimal txAmtCur { get; set; }
        }
    }
}
