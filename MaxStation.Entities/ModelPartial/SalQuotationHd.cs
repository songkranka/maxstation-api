using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaxStation.Entities.Models
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
