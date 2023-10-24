using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class SaveBranchResponse : BaseResponse<MasBranch>
    {
        public SaveBranchResponse(MasBranch branchRequest) : base(branchRequest) { }

        public SaveBranchResponse(string message) : base(message) { }
    }
}
