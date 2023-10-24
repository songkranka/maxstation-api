using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IUnitService
    {
        Task<QueryResult<MasUnit>> List(UnitQuery pQuery);
        Task<MasUnit> GetMasUnitAsync(string unitId);
        Task<SaveUnitResponse> SaveUnitAsync(SaveUnitRequest request);
        Task<SaveUnitResponse> UpdateStatusAsync(SaveUnitRequest request);
    }
}
