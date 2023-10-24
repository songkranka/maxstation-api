#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasDocPatternDt
    {
        public string DocId { get; set; }
        public int SeqNo { get; set; }
        public Guid? ItemId { get; set; }
        public string DocCode { get; set; }
        public string DocValue { get; set; }
    }
}
