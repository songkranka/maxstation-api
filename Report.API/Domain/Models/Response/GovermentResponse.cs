using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Models.Response
{
    public class GovermentResponse
    {
 
    }


    public class Gov01Response
    {
        public string compCode { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }
        public string compName { get; set; }
        public List<Gov01Head> docDates { get; set; }
        public List<Gov01Item> meters { get; set; }
        public List<Gov01Summary> diffs { get; set; }
        public List<Gov01Summary> summary { get; set; }
    }

    public class Gov01Head
    {
        public string docDate { get; set; }
    }

    public class Gov01Item
    {
        public string docDate { get; set; }
        public string dispId { get; set; }
        public decimal meterStart { get; set; }
        public decimal meterFinish { get; set; }

        public decimal itemQty1 { get; set; }
        public decimal itemAmt1 { get; set; }

        public decimal itemQty2 { get; set; }
        public decimal itemAmt2 { get; set; }

        public decimal itemQty3 { get; set; }
        public decimal itemAmt3 { get; set; }

        public decimal itemQty4 { get; set; }
        public decimal itemAmt4 { get; set; }

        public decimal itemQty5 { get; set; }
        public decimal itemAmt5 { get; set; }

        public decimal itemQty6 { get; set; }
        public decimal itemAmt6 { get; set; }

        public decimal itemQty7 { get; set; }
        public decimal itemAmt7 { get; set; }

        public decimal itemQty8 { get; set; }
        public decimal itemAmt8 { get; set; }

    }

    public class Gov01Summary
    {
        public string docDate { get; set; }
        public int seqNo { get; set; }
        public string detail { get; set; }

        public decimal itemQty1 { get; set; }
        public decimal itemAmt1 { get; set; }

        public decimal itemQty2 { get; set; }
        public decimal itemAmt2 { get; set; }

        public decimal itemQty3 { get; set; }
        public decimal itemAmt3 { get; set; }

        public decimal itemQty4 { get; set; }
        public decimal itemAmt4 { get; set; }

        public decimal itemQty5 { get; set; }
        public decimal itemAmt5 { get; set; }

        public decimal itemQty6 { get; set; }
        public decimal itemAmt6 { get; set; }

        public decimal itemQty7 { get; set; }
        public decimal itemAmt7 { get; set; }

        public decimal itemQty8 { get; set; }
        public decimal itemAmt8 { get; set; }

    }


    public class Gov03Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string registerId { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }       
        public string monthName { get; set; }
        public string year { get; set; }

        public List<Gov03Tank> tanks { get; set; }
        public List<Gov03Summary> summaries { get; set; }
    }

    public class Gov03Tank
    {
        public string tankId { get; set; }
        public string pdId { get; set; }
        public decimal itemQty1 { get; set; }
        public decimal itemQty2 { get; set; }
        public decimal itemQty3 { get; set; }
        public decimal itemQty4 { get; set; }
        public decimal itemQty5 { get; set; }
        public decimal itemQty6 { get; set; }
        public decimal itemQty7 { get; set; }
        public decimal itemQty8 { get; set; }
    }

    public class Gov03Summary
    {
        public string desc { get; set; }        
        public decimal itemQty1 { get; set; }
        public decimal itemQty2 { get; set; }
        public decimal itemQty3 { get; set; }
        public decimal itemQty4 { get; set; }
        public decimal itemQty5 { get; set; }
        public decimal itemQty6 { get; set; }
        public decimal itemQty7 { get; set; }
        public decimal itemQty8 { get; set; }
    }

    public class Gov05Response
    {
        public string compCode { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }
        public string compName { get; set; }

        public string month { get; set; }
        public string year { get; set; }
        public int seqNo { get; set; }
        public string supCode { get; set; }
        public string supName { get; set; }
        public string supAddress { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public decimal itemQty { get; set; }
    }


    public class Gov06Response
    {
        public string compCode { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }
        public string phone { get; set; }
        public string compName { get; set; }
        public string registerId { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public decimal vatTotal { get; set; }
        public string vatTotalText { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public decimal meterAmt { get; set; }
        public decimal vatRate { get; set; }
        public decimal vatAmt { get; set; }
    }


    public class Gov07Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string registerId { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }
        public string docDate { get; set; }
        public string invType { get; set; }        

        public string desc { get; set; }
        public decimal taxbaseAmt { get; set; }
        public decimal vatAmt { get; set; }
        public decimal totalAmt { get; set; }
    }

    public class Gov08Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }
        
        public string month { get; set; }
        public string year { get; set; }
        
        public int seqNo { get; set; }
        public string docDate { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public string docNo { get; set; }
       // public string invNo { get; set; }
        public decimal balanceQty { get; set; }
        public decimal receiveQty { get; set; }
        public decimal saleQty { get; set; }
        public decimal adjustQty { get; set; }
        public decimal remainQty { get; set; }

        public decimal sumYearReceiveQty { get; set; }
        public decimal sumYearSaleQty { get; set; }
        public decimal subYearAdjustQty { get; set; }

    }



    public class Gov09Response
    {
        public string compCode { get; set; }
        public string compName { get; set; }
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }

        public string month { get; set; }
        public string year { get; set; }

        public int seqNo { get; set; }
        public string description { get; set; }
        public decimal itemQty1 { get; set; }
        public decimal itemQty2 { get; set; }
        public decimal itemQty3 { get; set; }
        public decimal itemQty4 { get; set; }
        public decimal itemQty5 { get; set; }
        public decimal itemQty6 { get; set; }
        public decimal itemQty7 { get; set; }
        public decimal itemQty8 { get; set; }
    }



    //public class Gov11Response
    //{
    //    public List<Gov11Item> gov11s { get; set; }
    //}


    public class Gov11Response
    {
        public string month { get; set; }
        public string year { get; set; }
        public string compCode { get; set; }
        public string compName { get; set; }
        public string compAddress { get; set; }
        public string registerId { get; set; }

        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string brnAddress { get; set; }
        public string branchNo { get; set; }
        public string postCode { get; set; }
        public string phone { get; set; }
        public string trader { get; set; }
        public string traderPosition { get; set; }

        public decimal totalAmt { get; set; }
        public string totalAmtText { get; set; }

        public int seqNo { get; set; }
        public string pdId { get; set; }
        public string pdName { get; set; }
        public decimal itemQty { get; set; }
        public decimal vatRate { get; set; }
        public decimal subAmt { get; set; }

        //public List<Gov11Item> items { get; set; }
    }

    //public class Gov11Item
    //{


    //}

}
