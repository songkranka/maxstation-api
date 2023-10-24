using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Models
{
    public class CashSale : SalCashsaleHd
    {
        public List<SalCashsaleDt> SalCashsaleDt { get; set; }
    }

    public class QuotationDetail : SalQuotationDt
    {
        public MasProductPrice MasProductPrice { get; set; }
    }
}
