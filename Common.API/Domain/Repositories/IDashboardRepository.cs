using Common.API.Domain.Models;
using Common.API.Resource;
using Common.API.Resources;
using MaxStation.Entities.Models;
using System.Threading.Tasks;

namespace Common.API.Domain.Repositories
{
    public interface IDashboardRepository
    {
        Task<QueryResultResource<InvRequestHd>> GetRequestList(RequestGetRequestList req);
        Task<QueryResultResource<InvTranoutHd>> GetTransferOutList(RequestGetRequestList req);
        Task<QueryObjectResource<ProductDisplayResponse>> GetProductDisplay(RequestGetRequestList req);

    }
}
