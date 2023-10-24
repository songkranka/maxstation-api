using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class StationResponse
    {

        public class ST312Response
        {
            public string compCode { get; set; }
            public string compName { get; set; }
            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }
            public string docDate { get; set; }
            public List<ST312Detail> empList { get; set; }

        }

        public class ST312Detail
        {
            public string empCode { get; set; }
            public string empName { get; set; }
            public string periodNo { get; set; }
            public decimal diffAmt { get; set; }
            public decimal discAmt { get; set; }
        }


        public class ST313Response
        {
            public string compCode { get; set; }
            public string compName { get; set; }
            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }

            public string docDate { get; set; }

            public decimal saleQty { get; set; }
            public decimal saleAmt { get; set; }
            public decimal creditAmt { get; set; }
            public decimal cashAmt { get; set; }
            public decimal cashReceiveAmt { get; set; }
            public decimal cashPaymentAmt { get; set; }
            public decimal diffAmt { get; set; }
            public decimal totalAmt { get; set; }

        }


        public class ST315Response
        {
            public string compCode { get; set; }
            public string compName { get; set; }
            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }

            public string docDate { get; set; }
            public string pdId { get; set; }
            public string pdName { get; set; }
            public decimal p01 { get; set; }
            public decimal p02 { get; set; }
            public decimal p03 { get; set; }
            public decimal p04 { get; set; }
            public decimal p05 { get; set; }
            public decimal p06 { get; set; }
            public decimal p07 { get; set; }
            public decimal p08 { get; set; }
            public decimal p09 { get; set; }
            public decimal sumOil { get; set; }
            public decimal d01 { get; set; }
            public decimal d02 { get; set; }
            public decimal d03 { get; set; }
            public decimal d04 { get; set; }
            public decimal d05 { get; set; }
            public decimal d06 { get; set; }
            public decimal d07 { get; set; }
            public decimal d08 { get; set; }
            public decimal d09 { get; set; }
            public decimal sumSale { get; set; }
            public decimal percent { get; set; }
        }


        public class ST316Response
        {
            public string compCode { get; set; }
            public string compName { get; set; }
            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }

            public string docDate { get; set; }
            public string pdId { get; set; }
            public string pdName { get; set; }
            public decimal p01 { get; set; }
            public decimal p02 { get; set; }
            public decimal p03 { get; set; }
            public decimal p04 { get; set; }
            public decimal p05 { get; set; }
            public decimal p06 { get; set; }
            public decimal p07 { get; set; }
            public decimal p08 { get; set; }
            public decimal p09 { get; set; }
            public decimal sumOil { get; set; }
            public decimal d01 { get; set; }
            public decimal d02 { get; set; }
            public decimal d03 { get; set; }
            public decimal d04 { get; set; }
            public decimal d05 { get; set; }
            public decimal d06 { get; set; }
            public decimal d07 { get; set; }
            public decimal d08 { get; set; }
            public decimal d09 { get; set; }
            public decimal sumSale { get; set; }
            public decimal percent { get; set; }
        }


        public class ST317Response
        {
            public string compCode { get; set; }
            public string compName { get; set; }
            public string brnCode { get; set; }
            public string brnName { get; set; }
            public string brnAddress { get; set; }
            public string branchNo { get; set; }

            public string docDate { get; set; }
            public int periodNo { get; set; }
            public string pdId { get; set; }
            public string pdName { get; set; }
            public decimal unitPrice { get; set; }
            public decimal saleAmt { get; set; }


        }


    }
}
