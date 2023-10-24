using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Responses;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IEmployeeAuthRepository
    {
        Task<AutEmployeeRole> FindByEmpCodeAsync(string empCode);
        Task<MasBranch> FindBranchByBrnCode(string compCode, string empCode);
        Task<List<MasBranch>> GetBranchByCompCode(string compCode);
        Task<List<MasBranch>> GetBranchByAuthCode(string compCode, int? authCode);

        Task AddAuthEmployeeRoleAsync(AutEmployeeRole authEmployeeRole);
        Task UpdateAuthEmployeeRoleAsync(AutEmployeeRole authEmployeeRole);
    }
}
