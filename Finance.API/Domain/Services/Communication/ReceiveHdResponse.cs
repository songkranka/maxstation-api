using Finance.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services.Communication
{
    public class ReceiveHdResponse : BaseResponse<FinReceiveHd>
    {
        public ReceiveHdResponse(FinReceiveHd product) : base(product) { }

        public ReceiveHdResponse(string message) : base(message) { }
    }
}
