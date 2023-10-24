using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class DopPeriodGl
    {
        [NotMapped]
        public decimal GlAmt { get; set; }
    }
}
