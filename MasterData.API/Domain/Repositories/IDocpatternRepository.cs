using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface IDocpatternRepository
    {
        Task<List<MasDocPatternDt>> FindDocPatternDtByDocTypeAsync(string docType);
    }
}
