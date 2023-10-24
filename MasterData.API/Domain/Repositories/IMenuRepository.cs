using MasterData.API.Domain.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IMenuRepository
    {
        Task<Menus> FindByBranchCodeAsync(string compCode, string brnCode);
    }
}
