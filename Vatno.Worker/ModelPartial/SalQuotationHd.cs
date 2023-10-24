using System.ComponentModel.DataAnnotations.Schema;

namespace Vatno.Worker.Models
{
    public partial class SalQuotationHd
    {
        public SalQuotationHd()
        {
            SalQuotationDt = new HashSet<SalQuotationDt>();
        }

        [NotMapped]
        public decimal? SumStockRemain { get; set; }

        [NotMapped]
        public string CustPrefix { get; set; }

        [NotMapped]
        public ICollection<SalQuotationDt> SalQuotationDt { get; set; }
    }
}
