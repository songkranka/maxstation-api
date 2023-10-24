#nullable disable

namespace Vatno.Worker.Models
{
    public partial class EtlLotHd
    {
        public string LotNo { get; set; }
        public string LotSource { get; set; }
        public decimal? LotTotal { get; set; }
        public string LotStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
