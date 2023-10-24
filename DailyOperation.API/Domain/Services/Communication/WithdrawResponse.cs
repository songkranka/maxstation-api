using DailyOperation.API.Resources.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Services.Communication
{
    public class WithdrawResponse : BaseResponse<SaveWithdrawResource>
    {
        public WithdrawResponse(SaveWithdrawResource withdraw) : base(withdraw) { }

        public WithdrawResponse(string message) : base(message) { }
    }
}
