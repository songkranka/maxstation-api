using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services.Communication
{
    public class CashsaleHdResponse : BaseResponse<SalCashsaleHd>
    {
        public CashsaleHdResponse(SalCashsaleHd product) : base(product) { }

        public CashsaleHdResponse(string message) : base(message) { }
    }
}
