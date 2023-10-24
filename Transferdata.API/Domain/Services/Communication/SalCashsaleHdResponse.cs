using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Domain.Services.Communication
{
    public class SalCashsaleHdResponse : BaseResponse<SalCashsaleHd>
    {
        public SalCashsaleHdResponse(SalCashsaleHd product) : base(product) { }

        public SalCashsaleHdResponse(string message) : base(message) { }
    }
}
