using MasterData.API.Domain.Models.DocPattern.Request;
using MasterData.API.Domain.Models.DocPattern.Response;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IDocpatternService
    {
        Task<DocPatternResponse> GetDocPatternAsync(DocPatternRequest docPatternRequest);
    }
}
