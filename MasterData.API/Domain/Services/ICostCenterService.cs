using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MasterData.API.Domain.Models.Responses;

namespace MasterData.API.Domain.Services
{
    public interface ICostCenterService
    {
        List<MasCostCenter> GetCostCenterList(CostCenterRequest req);
        Task<QueryResult<MasCostCenter>> ListAsync(CosCenterQuery query);
        Task<SaveCostCenterResponse> SaveCostCenterAsync(MasCostCenterRequest request, string host, string path, string method);
        Task<MasCostCenter> GetCostCenterByGuid(Guid guid);
        Task SaveLogError(string logStatus, Exception exception, string jsonData, MasCostCenter request);
        //Task SaveLogError(string logStatus, string exception, string jsonData, MasCostCenter request);
        Task<CostCenterResponse> UpdateCostCenterAsync(MasCostCenterRequest request, string host, string path, string method);
    }
}
