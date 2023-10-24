#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasSupplierProduct
    {
        public string CompCode { get; set; }
        public string SupCode { get; set; }
        public string UnitBarcode { get; set; }
        public int? UnitPack { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitCost { get; set; }
    }
}
