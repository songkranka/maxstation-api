using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class FinBalance
    {
        [NotMapped]
        public string BlNo { get; set; }
    }
}
