using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Resources.Position;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IPositionService
    {
        Task<ModelGetPositionListResult> GetPositionList(ModelGetPositionListParam param);
        Task<ModelPosition> GetPosition(string pStrPosCode);
        Task<ModelPosition> InsertPosition(ModelPosition pInput);
        Task<SaveUnlock> InsertUnlock(SaveUnlock param);
        Task<ModelPosition> UpdatePosition(ModelPosition pInput);
        Task<SaveUnlock> UpdateUnlock(SaveUnlock pInput);
        Task<SysMenu[]> GetSysMenuList();
        Task<bool> ChangeStatus(MasPosition param);
        Task<List<BranchConfig>> GetBranchConfig(string pStrPosCode);
        Task<List<BranchConfig>> GetBranchConfigDesc();
    }
}
