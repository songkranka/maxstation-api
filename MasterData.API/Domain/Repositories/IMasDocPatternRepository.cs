using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IMasDocPatternRepository
    {
        Task<List<MasDocPatternDt>> GetMasDocPatternDts(string docType);
    }
}
