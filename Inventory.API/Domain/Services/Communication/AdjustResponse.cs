using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services.Communication
{
    public class AdjustResponse : BaseResponse<InvAdjustHd>
    {
        public AdjustResponse(InvAdjustHd resource) : base(resource)
        {
        }

        public AdjustResponse(string message) : base(message)
        {
        }
    }
}
