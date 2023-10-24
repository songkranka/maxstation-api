using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IUnitRepository
    {
        Task<QueryResult<MasUnit>> ListAsync(UnitQuery pQuery);
        Task<MasUnit> GetMasUnitAsync(string unitId);
        Task AddUnitAsync(MasUnit masUnit);
        Task UpdateUnitAsync(MasUnit masUnit);
        Task UpdateStatusAsync(MasUnit masUnit);
    }
}
