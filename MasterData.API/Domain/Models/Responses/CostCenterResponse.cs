using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class CostCenterResponse : BaseResponse<MasCostCenter>
    {
        public CostCenterResponse(MasCostCenter costCenterRequest) : base(costCenterRequest) { }

        public CostCenterResponse(string message) : base(message) { }
    }
}
