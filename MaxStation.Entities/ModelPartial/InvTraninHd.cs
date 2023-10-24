using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaxStation.Entities.Models
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
