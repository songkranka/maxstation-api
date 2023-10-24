using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Domain.Services.Communication
{
    public class SalCreditsaleHdResponse : BaseResponse<SalCreditsaleHd>
    {
        public SalCreditsaleHdResponse(SalCreditsaleHd product) : base(product) { }

        public SalCreditsaleHdResponse(string message) : base(message) { }
    }
}
