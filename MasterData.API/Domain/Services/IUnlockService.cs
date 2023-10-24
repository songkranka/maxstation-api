using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services.Communication;
using MasterData.API.Resources.Unlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IUnlockService
    {
        Task<List<EmployBranchConfigResponse>> GetEmployeeBranchConfig(EmployeeBranchConfigRequest req);
        Task<SysBranchConfigResponse> GetSysBranchConfig(SysBranchConfigRequest req);
        Task<UnlockResponse> SaveUnlockAsync(SaveUnlockResource request);
    }
}
