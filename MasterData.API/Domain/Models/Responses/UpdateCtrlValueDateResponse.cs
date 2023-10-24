using MasterData.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class UpdateCtrlValueDateResponse : BaseResponse<MasControl>
    {
        public UpdateCtrlValueDateResponse(MasControl masControlRespose) : base(masControlRespose) { }

        public UpdateCtrlValueDateResponse(string message) : base(message) { }
    }
}
