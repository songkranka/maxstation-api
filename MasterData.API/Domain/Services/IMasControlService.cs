using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IMasControlService
    {
        MasControl GetMasControl(MasControlRequest req);
        Task<UpdateCtrlValueDateResponse> UpdateCtrlValueAsync(UpdateCtrlValueDateRequest request);
    }
}
