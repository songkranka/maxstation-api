using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Models.Response;

namespace Transferdata.API.Domain.Services
{
    public interface IMaxmeService
    {
        Task<MaxmeResponse> GetMaxPosSum(MaxmeQuery query);
    }
}
