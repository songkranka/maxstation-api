using DailyOperation.API.Domain.Models.Response;
using DailyOperation.API.Resources.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Services.Communication
{
    public class CashsaleResponse : BaseResponse<SaveCashSaleResource>
    {
        public CashsaleResponse(SaveCashSaleResource cashSale) : base(cashSale) { }

        public CashsaleResponse(string message) : base(message) { }
    }
}
