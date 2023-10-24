using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class MasBranchDisp
    {
        [NotMapped]
        public decimal MeterStart { get; set; }
        [NotMapped]
        public decimal MeterFinish { get; set; }
        [NotMapped]
        public decimal SaleQty { get; set; }
        [NotMapped]
        public decimal SaleAmt { get; set; }
        [NotMapped]
        public decimal CashAmt { get; set; }
        [NotMapped]
        public decimal CreditAmt { get; set; }
        [NotMapped]
        public decimal DiscAmt { get; set; }
        [NotMapped]
        public decimal CouponAmt { get; set; }
        [NotMapped]
        public decimal RepairStart { get; set; }
        [NotMapped]
        public decimal? RepairFinish { get; set; }
        [NotMapped]
        public decimal RepairQty { get; set; }
        [NotMapped]
        public decimal TestStart { get; set; }
        [NotMapped]
        public decimal TestQty { get; set; }
        [NotMapped]
        public decimal CardAmt { get; set; }
        [NotMapped]
        public string Remark { get; set; }
        [NotMapped]
        public string PeriodStatus { get; set; }
        [NotMapped]
        public string Post { get; set; }
        [NotMapped]
        public decimal UnitPrice { get; set; }
        [NotMapped]
        public decimal TotalQty { get; set; }

    }
}
