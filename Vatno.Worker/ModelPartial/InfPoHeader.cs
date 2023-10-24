using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class InfPoHeader
    {
        [NotMapped]
        public string SupplierName { get; set; }
    }
}
