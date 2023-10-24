using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
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
