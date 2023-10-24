using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface ICostCenterRepository
    {
        List<MasCostCenter> GetCostCenterList(CostCenterRequest req);
        Task<QueryResult<MasCostCenter>> ListAsync(CosCenterQuery query);
        Task AddCostCenterAsync(MasCostCenter costCenter);
        Task<MasCostCenter> GetCostCenterByGuid(Guid guid);
        Task SaveLogError(LogError logError);
        Task UpdateCostCenterAsync(MasCostCenter costCenter);
        Task<MasCostCenter> CheckMasCostCenter(MasCostCenter request);
        Task<MasCostCenter> GetMasConstCenterFromBranCodeAndCompCode(string CompCode, string BrnCode);
    }
}