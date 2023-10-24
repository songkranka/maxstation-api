using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
{
    public partial class MasEmployee
    {
        public string EmpName { get { return ( $"{this.PrefixThai} {this.PersonFnameThai} {this.PersonLnameThai}").Trim(); } }
    }
}
