using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models.Response
{
    public class CashSale
    {
        public SalCashsaleHd CashSaleHeader { get; set; }
        public List<SalCashsaleDt> CashSaleDetails { get; set; }
    }
}
