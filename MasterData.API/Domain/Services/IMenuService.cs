using MasterData.API.Domain.Models.Responses;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IMenuService
    {
        Task<Menus> FindByBranchCodeAsync(string compCode, string brnCode);
        Task<AutPositionRole> GetPositionRole(string pStrEmpCode, string pStrRouteUrl);
    }
}
