#nullable disable

namespace Vatno.Worker.Models
{
    public partial class InvStockDaily
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public DateTime StockDate { get; set; }
        public string PdId { get; set; }
        public string UnitId { get; set; }
        public string UnitBarcode { get; set; }
        public decimal? Balance { get; set; }
        public decimal? ReceiveIn { get; set; }
        public decimal? TransferIn { get; set; }
        public decimal? TransferOut { get; set; }
        public decimal? SaleOut { get; set; }
        public decimal? FreeOut { get; set; }
        public decimal? ReturnOut { get; set; }
        public decimal? WithdrawOut { get; set; }
        public decimal? Adjust { get; set; }
        public decimal? Audit { get; set; }
        public decimal? Remain { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
