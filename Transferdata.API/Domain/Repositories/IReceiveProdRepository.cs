using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Repositories
{

    public interface IReceiveProdRepository
    {
        Task<List<InvReceiveProdHd>> ListReceiveProdAsync(ReceiveProdQuery query);
        Task<List<InvReceiveProdHd>> ListReceiveOilAsync(ReceiveProdQuery query);
        Task<List<InvReceiveProdHd>> ListReceiveGasAsync(ReceiveProdQuery query);
    }
}
