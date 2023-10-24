using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class InvWithdrawHd
    {
        public InvWithdrawHd()
        {
            InvWithdrawDt = new HashSet<Models.InvWithdrawDt>();
        }

        [NotMapped]
        public ICollection<Models.InvWithdrawDt> InvWithdrawDt { get; set; }


    }
}
