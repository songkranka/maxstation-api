using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Domain.Service
{
    public interface IAuthService
    {
        Task<MasEmployee> FindMasEmployeeByEmpCodeAsync(string empCode);
        Task<AutEmployeeRole> FindAuthEmpRoleByEmpCode(string empCode);
        Task<MasBranch> FindBranchByBrnCode(string compCode, string empCode);
        Task<List<MasBranch>> GetBranchByCompCode(string compCode);   
        Task<List<MasBranch>> GetBranchByAuthCode(string compCode, int? authCode);
        Task<List<MasBranch>> GetAutBranchRole(string username, string compCode);
        Task<List<AutPositionRole>> GetAutPositionRole(string positionCode);
        Task<MasPosition> GetMasPosition(string positionCode);
        bool LDapAuth(string username, string password);
        Task InsertLogLogin(LogLogin pLogin);
    }
}
