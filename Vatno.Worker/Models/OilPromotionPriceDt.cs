#nullable disable

namespace Vatno.Worker.Models
{
    public partial class OilPromotionPriceDt
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string DocNo { get; set; }
        public string PdId { get; set; }
        public string UnitBarcode { get; set; }
        public decimal? AdjustPrice { get; set; }
    }
}
