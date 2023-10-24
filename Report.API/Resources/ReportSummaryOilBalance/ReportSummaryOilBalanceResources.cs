using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Resources.ReportSummaryOilBalance
{
    public class TankModel
    {
        public string TankId { get; set; }
        public string PdName { get; set; }
        public decimal BeforeQty { get; set; }
        public decimal ReceiveQty { get; set; }
        public decimal TransferQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal RemainQty { get; set; }
        public decimal RealQty { get; set; }
        public decimal DiffQty { get; set; }
    }


    public class ResponseTankModelForPDF
    {
        public string brnCode { get; set; }
        public string brnName { get; set; }
        public string compName { get; set; }
        public string compImage { get; set; }
        public string docDate { get; set; }
        public List<HdTankModelForPDF> tankData { get; set; }

       // public List<HdTankModelForPDF> tankSum { get; set; }
    }

    public class HdTankModelForPDF
    {
        public int period { get; set; }
        public List<DtTankModelForPDF> dtItems { get; set; }
    }

    public class DtTankModelForPDF
    {
        public string tankId { get; set; }
        public string pdName { get; set; }
        public decimal beforeQty { get; set; }
        public decimal receiveQty { get; set; }
        public decimal transferQty { get; set; }
        public decimal issueQty { get; set; }
        public decimal remainQty { get; set; }
        public decimal realQty { get; set; }
        public decimal diffQty { get; set; }
    }

    public class SummaryTank
    {
        public string tankId { get; set; }
        public List<DopPeriodTank> DopPeriodTank { get; set; }
        public string pdName { get; set; }
        public decimal? beforeQty { get; set; }
    }

}
