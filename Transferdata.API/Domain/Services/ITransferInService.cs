using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Services
{

    public interface ITransferInService
    {
        Task<List<InvTraninHd>> ListTransferInAsync(TransferInQuery query);
    }
}
