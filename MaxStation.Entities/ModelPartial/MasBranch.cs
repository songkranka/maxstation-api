using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
{
    public partial class MasBranch
    {
        [NotMapped]
        public string FullAddress { get { return (this.Address + " " +  (this.SubDistrict ?? "") +" " + (this.District??"")+" "+(this.Postcode??"") ).Trim(); } }

        [NotMapped]
        public string CompanyName { get; set; }
    }
}
