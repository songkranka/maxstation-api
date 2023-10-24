#nullable disable

namespace Vatno.Worker.Models
{
    public partial class LogVatnoMaxme
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
