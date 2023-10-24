using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Resources.Unlock;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IUnlockRepository
    {
        Task<List<EmployBranchConfigResponse>> GetEmployeeBranchConfig(EmployeeBranchConfigRequest req);
        Task<SysBranchConfigResponse> GetSysBranchConfig(SysBranchConfigRequest req);
        Task<List<SysBranchConfig>> GetSysBranchConfigListAsync(string compCode, string brnCode, string docDate);
        Task<MasBranchConfig> GetIsLockMasBranchConfig(string compCode, string brnCode);
        Task AddUnlockAsync(SaveUnlockResource request);
    }
}
