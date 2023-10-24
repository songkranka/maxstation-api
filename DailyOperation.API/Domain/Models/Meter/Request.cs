using MaxStation.Entities.Models;
using System.Collections.Generic;

namespace DailyOperation.API.Domain.Models.Meter
{
    public class SaveDocumentRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocDate { get; set; }
        public string User { get; set; }
        public int PeriodNo { get; set; }
        public string IsPos { get; set; }
        public Meter Meter { get; set; }
        public Tank Tank { get; set; }
        public Cash Cash { get; set; }
    }

    public class GetDocumentRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocDate { get; set; }
        public int PeriodNo { get; set; }

    }

    public class DeleteDocumentRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocDate { get; set; }
        public int PeriodNo { get; set; }
    }

    public class Meter 
    {
        public string PeriodStart { get; set; }
        public string PeriodFinish { get; set; }
        public decimal SumMeterSaleQty { get; set; }
        public decimal SumMeterTotalQty { get; set; }
        public List<string> Employee { get; set; }
        public List<MasBranchDisp> Items { get; set; }
    }

    public class Tank 
    {
        public List<MasBranchTank> TankItems { get; set; }
        public List<DopPeriodTankSum> SumTankItems { get; set; }
    }

    public class Cash
    { 
        public List<DopPeriodCash> CashItems { get; set; }
        public DopPeriodCashSum SumCashItems { get; set; }
        public List<DopPeriodGl> DrItems { get; set; }
        public List<DopPeriodGl> CrItems { get; set; }
    }

    public class BranchMeterRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public int PeriodNo { get; set; }
        public string PeriodStart { get; set; }
        public string DocDate { get; set; }
    }

    public class MasBranchCalibrateRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
    }
}
