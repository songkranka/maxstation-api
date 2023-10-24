using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class InvWithdrawDt
    {
        [NotMapped]
        public string GroupId { get; set; }
    }
}
