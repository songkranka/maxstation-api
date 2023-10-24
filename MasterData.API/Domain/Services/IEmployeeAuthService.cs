using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IEmployeeAuthService
    {
        Task<AutEmployeeRole> FindByEmpCodeAsync(string empCode);
        Task<EmployeeAuthResponse> SaveEmployeeAuthAsync(SaveEmployeeAuthRequest request);
        Task<MasOrganize[]> GetArrOrganize(string pStrEmpCode);

        Task<List<AuthBranch>> GetAuthBranch(string compCode,string empCode);
    }
}
