using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class InvAdjustRequestHd
    {
        public InvAdjustRequestHd()
        {
            InvAdjustRequestDt = new HashSet<InvAdjustRequestDt>();
        }

        [NotMapped]
        public ICollection<InvAdjustRequestDt> InvAdjustRequestDt { get; set; }


    }
}
