using MaxStation.Entities.Models;
using System.Collections.Generic;

namespace Report.API.Domain.Models.Response
{
    public partial class WithdrawResponse
    {
        public class WithdrawHd
        {
            public string compCode { get; set; }
            public string compName { get; set; }
            public string compNameEn { get; set; }
            public string compAddress { get; set; }
            public string compPhone { get; set; }
            public string compFax { get; set; }
            public string compImage { get; set; }
            public string registerId { get; set; }
            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }

            public string docNo { get; set; }
            public string docDate { get; set; }
            public string empCode { get; set; }
            public string empName { get; set; }
            public string useBrnCode { get; set; }
            public string useBrnName { get; set; }
            public string licensePlate { get; set; }
            public string reasonDesc { get; set; }
            public string remark { get; set; }
            public decimal totalQty { get; set; }
            public int itemCount { get; set; }

            public List<WithdrawDt> items { get; set; }

        }

        public class WithdrawDt
        {
            public int seqNo { get; set; }
            public string pdId { get; set; }
            public string pdName { get; set; }
            public string unitName { get; set; }
            public decimal itemQty { get; set; }
        }





    }
}
