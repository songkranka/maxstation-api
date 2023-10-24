using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaxStation.Entities.Models
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
