using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
{
    public partial class InvWithdrawHd
    {
        public InvWithdrawHd()
        {
            InvWithdrawDt = new HashSet<InvWithdrawDt>();
        }

        [NotMapped]
        public ICollection<InvWithdrawDt> InvWithdrawDt { get; set; }


    }
}
