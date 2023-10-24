using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{

    public partial class InvTranoutHd
    {
        public InvTranoutHd()
        {
            InvTranoutDt = new HashSet<InvTranoutDt>();
        }
        [NotMapped]
        public string BrnName { get; set; }
        [NotMapped]
        public ICollection<InvTranoutDt> InvTranoutDt { get; set; }

    }
}
