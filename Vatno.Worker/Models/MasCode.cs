#nullable disable

namespace Vatno.Worker.Models
{
    public partial class MasCode
    {
        public Guid CodeId { get; set; }
        public string Code { get; set; }
        public string CodeDesc { get; set; }
        public string CodeType { get; set; }
    }
}
