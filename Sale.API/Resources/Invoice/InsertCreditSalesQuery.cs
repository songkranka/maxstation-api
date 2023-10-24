using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.Invoice
{
    public class InsertCreditSalesQuery : QueryResource
    {
        public MaxStation.Entities.Models.SalCreditsaleHd CreditSaleHeader { get; set; }
        public MaxStation.Entities.Models.SalCreditsaleDt[] ArrCreditSaleDetail { get; set; }


    }
}
