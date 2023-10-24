using DailyOperation.API.Resources.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Services.Communication
{
    public class CreditsaleResponse : BaseResponse<SaveCreditSaleResource>
    {
        public CreditsaleResponse(SaveCreditSaleResource creditSale) : base(creditSale) { }

        public CreditsaleResponse(string message) : base(message) { }
    }
}
