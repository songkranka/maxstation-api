using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class InvAdjustHd
    {
        public InvAdjustHd()
        {
            InvAdjustDt = new HashSet<InvAdjustDt>();
        }

        [NotMapped]
        public ICollection<InvAdjustDt> InvAdjustDt { get; set; }


    }
}
