#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InfPosFunction5
    {
        public string JournalId { get; set; }
        public int Runno { get; set; }
        public string ItemType { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PluNumber { get; set; }
        public string DiscGroup { get; set; }
        public string ProductCodesap { get; set; }
        public decimal? SellQty { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? GoodsAmt { get; set; }
        public string TaxRate { get; set; }
        public decimal? TaxAmt { get; set; }
        public int? HoseId { get; set; }
        public int? PumpId { get; set; }
        public int? DeliveryId { get; set; }
        public int? TankId { get; set; }
        public int? DeliveryType { get; set; }
        public decimal? DiscAmt { get; set; }
        public int? ShiftId { get; set; }
        public int? PosId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string ProductSubType { get; set; }
    }
}
