using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services.Communication
{
    public class WithdrawResponse : BaseResponse<InvWithdrawHd>
    {
        public WithdrawResponse(InvWithdrawHd resource) : base(resource)
        {
        }

        public WithdrawResponse(string message) : base(message)
        {
        }
    }
}
