using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
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
