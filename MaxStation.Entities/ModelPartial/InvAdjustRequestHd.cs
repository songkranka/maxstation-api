using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
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
