using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IMasBranchConfigRepository
    {
        Task<MasBranchConfig> GetMasBranchConfig(string compCode, string brnCode);
        Task AddMasBranchConfigAsync(MasBranchConfig masBranchConfig);
        Task UpdateMasBranchConfigAsync(MasBranchConfig masBranchConfig);
        Task DeleteMasBranchConfigAsync(string compCode, string brnCode);
    }
}
