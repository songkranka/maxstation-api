using MaxStation.Entities.Models;
using System.Collections.Generic;

namespace DailyOperation.API.Domain.Models.Meter
{
    public class GetDocumentResponse
    {
        public int PeriodNo { get; set; }
        public string Post { get; set; }
        public string IsPos { get; set; }
        public Meter Meter { get; set; }
        public Tank Tank { get; set; }
        public Cash Cash { get; set; }
    }

    public class MeterResponse 
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class MasBranchMeterResponse
    {
        public string PeriodStart { get; set; }
        public string PeriodFinish { get; set; }
        public List<MasBranchDisp> MasBranchDispItems { get; set; }
        public List<MasBranchTank> MasBranchTankItems { get; set; }
        public List<DopPeriodGl> MasBranchCashDrItems { get; set; }
        public List<DopPeriodGl> MasBranchCashCrItems { get; set; }
    }

    public class SaleQtyAndSaleAmtPos 
    { 
        public int HostId { get; set; }
        public decimal MeterStart { get; set; }
        public decimal MeterFinish { get; set; }
        public decimal SaleQty { get; set; }
        public decimal SaleAmt { get; set; }
    }

    public class CreditPos
    {
        public int HostId { get; set; }
        public decimal CreditAmt { get; set; }
    }

    public class CashPos
    {
        public int HostId { get; set; }
        public decimal CashAmt { get; set; }
    }

    public class DiscPos
    {
        public int HostId { get; set; }
        public decimal DiscAmt { get; set; }
    }

    public class CouponPos
    {
        public int HostId { get; set; }
        public decimal CouponAmt { get; set; }
    }

    public class TestPos
    {
        public int HostId { get; set; }
        public decimal TestAmt { get; set; }
    }

    public class CardPos
    {
        public int HostId { get; set; }
        public decimal CardAmt { get; set; }
    }

    public class TestPos2
    {
        public int HoseId { get; set; }
        public string PluNumber { get; set; }
        public decimal? TestQty { get; set; }
        public decimal? TestAmt { get; set; }
    }

    public class ModelMaxOrderPos
    {
        public string SiteId { get; set; }
        public int ShiftNo { get; set; }
        public int MobCode { get; set; }
        public string MobInfo { get; set; }
        public decimal? Amount { get; set; }
        public string GlNo { get; set; }
    }

}
