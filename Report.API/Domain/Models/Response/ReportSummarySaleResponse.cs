using System;
using System.Collections.Generic;

namespace Report.API.Domain.Models.Response
{
    public class GetPeriodResponse
    {
        public int PeriodNo { get; set; }
        public string PeriodName { get; set; }
    }

    public class DispOfSummerySale 
    { 
        public string DispName { get; set; }
        public decimal MeterStart { get; set; }
        public decimal MeterFinish { get; set; }
        public decimal TotalQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmt { get; set; }
    }

    public class DispOfSummerySaleForPDF
    {
        public string dispName { get; set; }
        public decimal meterStart { get; set; }
        public decimal meterFinish { get; set; }
        public decimal totalQty { get; set; }
        public decimal unitPrice { get; set; }
        public decimal totalAmt { get; set; }
    }

    public class BodyRightSummarySale
    {
        public List<string> Body { get; set; }
    }

    public class FinalSummarySale
    {
        public string Header { get; set; }
        public List<SumGl> Body { get; set; }
        public FooterSumGl Footer { get; set; }
    }

    public class SumGl 
    {
        public string GlType { get; set; }
        public string GlDesc { get; set; }
        public decimal GlAmt { get; set; }
    }

    public class SumGlForPDF
    {
        public string glType { get; set; }
        public string glDesc { get; set; }
        public decimal glAmt { get; set; }
    }

    public class FooterSumGl
    {
        public string Remark { get; set; }
        public decimal SumGlAmt { get; set; }
    }

    public class FinalSummary
    {
        public string Remark { get; set; }
        public decimal Amt { get; set; }
    }

    public class FinalSummaryForPDF
    {
        public string remark { get; set; }
        public decimal amt { get; set; }
    }

    public class SaleAmtByDisp 
    {
        public int HostId { get; set; }
        public decimal SaleAmt { get; set; }
    }

    public class ResponseSummarySaleForPDF 
    { 
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string docDate { get; set; }        
        public string compName { get; set; }
        public string compImage { get; set; }
        public string empName { get; set; }
        public decimal totalQty { get; set; }
        public decimal totalAmt { get; set; }
        public decimal totalCashAmt { get; set; }
        public decimal totalDiffAmt { get; set; }
        public decimal totalDepositAmt { get; set; }

        public List<HeaderSummarySaleForPdf> headerSummarySale { get; set; }
        public List<DispOfSummerySaleForPDF> bodyLeftItems { get; set; }
        public List<BodyRightItemForPDF> bodyRightItems { get; set; }
        public List<SumGlForPDF> crItems { get; set; }
        public List<SumGlForPDF> drItems { get; set; }
        public List<FinalSummaryForPDF> finalSummary { get; set; }        
    }

    public class HeaderSummarySaleForPdf
    {
        public string brnCode { get; set; }
    }

    public class BodyRightItemForPDF
    {
        public string label { get; set; }
        public decimal dieselB7 { get; set; } //000001
        public decimal benzine { get; set; }  //000002
        public decimal benzine91 { get; set; }//000004
        public decimal gasohol95 { get; set; }//000005
        public decimal gasohol91 { get; set; }//000006
        public decimal gasoholE20 { get; set; }//000010
        public decimal gasLPG { get; set; }     //000011
        public decimal dieselB20 { get; set; }  //000073
        public decimal diesel { get; set; }     //000074
        public decimal sumAllProduct { get; set; }
    }
}
