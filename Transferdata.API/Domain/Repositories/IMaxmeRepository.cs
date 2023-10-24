using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Models.Response;

namespace Transferdata.API.Domain.Repositories
{
    public interface IMaxmeRepository
    {
        Task<MaxmeResponse> GetMaxPosSumAsync(MaxmeQuery query);
    }

}
