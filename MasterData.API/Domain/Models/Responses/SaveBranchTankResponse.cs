using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    
    public class SaveBranchTankResponse : BaseResponse<BranchTankResponse>
    {
        public SaveBranchTankResponse(BranchTankResponse branchTankResponse) : base(branchTankResponse) { }

        public SaveBranchTankResponse(string message) : base(message) { }
    }
}
