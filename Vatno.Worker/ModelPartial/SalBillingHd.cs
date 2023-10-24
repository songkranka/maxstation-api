using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class SalBillingHd
    {
        [NotMapped]
        public string CustPrefix { get; set; }
    }
}
