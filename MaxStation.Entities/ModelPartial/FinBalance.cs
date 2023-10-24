using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
{
    public partial class FinBalance
    {
        [NotMapped]
        public string BlNo { get; set; }
    }
}
