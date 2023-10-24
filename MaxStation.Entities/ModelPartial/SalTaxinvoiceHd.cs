using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
{
    public partial class SalTaxinvoiceHd
    {
        //public string NetAmtBathText { get; set; }
        public SalTaxinvoiceHd()
        {
            SalTaxinvoiceDt = new HashSet<SalTaxinvoiceDt>();
        }

        [NotMapped]
        public ICollection<SalTaxinvoiceDt> SalTaxinvoiceDt { get; set; }



    }
}
