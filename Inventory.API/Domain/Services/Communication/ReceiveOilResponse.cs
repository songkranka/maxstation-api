using Inventory.API.Domain.Models;
using Inventory.API.Resources.ReceiveOil;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services.Communication
{
    public class ReceiveOilResponse : BaseResponse<SaveReceiveOilResource>
    {
        public ReceiveOilResponse(SaveReceiveOilResource receiveOil) : base(receiveOil) { }

        public ReceiveOilResponse(string message) : base(message) { }
    }
}
