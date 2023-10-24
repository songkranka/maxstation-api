using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services.Communication
{
    public class AdjustRequestResponse : BaseResponse<InvAdjustRequestHd>
    {
        public AdjustRequestResponse(InvAdjustRequestHd resource) : base(resource)
        {
        }

        public AdjustRequestResponse(string message) : base(message)
        {
        }
    }
}
