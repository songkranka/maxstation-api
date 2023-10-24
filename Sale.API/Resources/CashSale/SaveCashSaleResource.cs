using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Resources.CashSale
{
    public class SaveCashSaleResource : SalCashsaleHd
    {
        
    }

    public class CashSaleResource2
    {
        public SalCashsaleHd CashSaleHeader { get; set; }
        public SalCashsaleDt[] ArrCashSaleDetail { get; set; }
        public SalQuotationHd QuotationHeader { get; set; }
        public SalQuotationDt[] ArrQuotationDetail { get; set; }
    }
}
