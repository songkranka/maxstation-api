using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class SalCndnHd
    {
        [NotMapped]
        public string CustPrefix { get; set; }
    }
}
