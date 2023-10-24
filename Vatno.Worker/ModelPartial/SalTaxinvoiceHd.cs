using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
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
