using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Services
{

    public interface ITransferOutService
    {
        Task<List<InvTranoutHd>> ListTransferOutAsync(TransferOutQuery query);
    }
}
