using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Resources.Quotation
{
    public class QuotationDtResource
    {
        public string UnitBarcode { get; set; }
        public decimal? ItemQty { get; set; }
        public decimal? StockQty { get; set; }
        public decimal? ItemQtyBefore { get; set; }

    }
}
