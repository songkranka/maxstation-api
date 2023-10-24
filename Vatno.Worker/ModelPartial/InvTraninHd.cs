using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class InvTraninHd
    {
        public InvTraninHd()
        {
            InvTraninDt = new HashSet<InvTraninDt>();
        }

        [NotMapped]
        public ICollection<InvTraninDt> InvTraninDt { get; set; }
    }
}
