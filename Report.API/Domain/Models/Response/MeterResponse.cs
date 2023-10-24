using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public partial class MeterResponse
    {
        public class MeterRepairResponse
        {
            public string compCode { get; set; }
            public string compName { get; set; }
            public string compImage { get; set; }
            public string registerId { get; set; }
            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }

            public string docDate { get; set; }
            public int periodNo { get; set; }
            public string timeStart { get; set; }
            public string timeFinish { get; set; }
            public string dispId { get; set; }
            public string pdName { get; set; }
            public decimal repairStart { get; set; }
            public decimal repairFinish { get; set; }
            public decimal repairQty { get; set; }
            public decimal repairAmt { get; set; }
            public string remark { get; set; }


        }

        public class MeterTestResponse
        {
            public string compCode { get; set; }
            public string compName { get; set; }
            public string compImage { get; set; }
            public string registerId { get; set; }
            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }

            public string docDate { get; set; }
            public int periodNo { get; set; }
            public string timeStart { get; set; }
            public string timeFinish { get; set; }
            public string dispId { get; set; }
            public string pdName { get; set; }
            public decimal meterStart { get; set; }
            public decimal meterFinish { get; set; }
            public decimal testQty { get; set; }
            public decimal testAmt { get; set; }
            public decimal totalQty { get; set; }
            public decimal totalAmt { get; set; }
            public string remark { get; set; }

           // public List<MeterTestItem> items { get; set; }

        }

            public class MeterTestItem
        {
            public int periodNo { get; set; }
            public string timeStart { get; set; }
            public string timeFinish { get; set; }
            public string dispId { get; set; }
            public string pdName { get; set; }
            public decimal meterStart { get; set; }
            public decimal meterFinish { get; set; }
            public decimal testQty { get; set; }
            public decimal testAmt { get; set; }
            public decimal totalQty { get; set; }
            public decimal totalAmt { get; set; }
            public string remark { get; set; }
        }

    }
}
