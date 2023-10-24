using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Repositories
{

    public interface ITransferOutRepository
    {
        Task<List<InvTranoutHd>> ListTransferOutAsync(TransferOutQuery query);
    }
}
