using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
{
    public partial class MasCustomer
    {
        [NotMapped]
        public string CustAddr1 { get { return  (this.Address + " " + this.SubDistrict??"" ).Trim(); } }

        [NotMapped]
        public string CustAddr2 { get { return (this.District + " " + this.ProvName + " " + this.Postcode).Trim();  } }
        
    }
}
