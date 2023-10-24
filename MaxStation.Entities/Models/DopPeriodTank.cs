using System;
using System.Collections.Generic;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class DopPeriodTank
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public DateTime DocDate { get; set; }
        public int PeriodNo { get; set; }
        public string TankId { get; set; }
        public string PeriodStatus { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public decimal? Unitprice { get; set; }
        public decimal? BeforeQty { get; set; }
        public decimal? ReceiveQty { get; set; }
        public decimal? TransferQty { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal? RemainQty { get; set; }
        public decimal? WithdrawQty { get; set; }
        public decimal? SaleQty { get; set; }
        public decimal? Height { get; set; }
        public decimal? RealQty { get; set; }
        public decimal? DiffQty { get; set; }
        public decimal? WaterHeight { get; set; }
        public decimal? WaterQty { get; set; }
        public string Hold { get; set; }
        public string HoldReasonId { get; set; }
        public string HoldReasonDesc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
