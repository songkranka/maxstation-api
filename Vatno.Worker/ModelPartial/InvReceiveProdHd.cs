using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{

    public partial class InvReceiveProdHd
    {
        public InvReceiveProdHd()
        {
            InvReceiveProdDt = new HashSet<InvReceiveProdDt>();
        }

        [NotMapped]
        public ICollection<InvReceiveProdDt> InvReceiveProdDt { get; set; }

    }
}
