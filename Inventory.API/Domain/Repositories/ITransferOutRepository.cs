using Inventory.API.Domain.Models;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Repositories
{
    public interface ITransferOutRepository
    {
        Task<ResponseData<List<ModelTransferOutHeader>>> GetTransferOutList(TransferOutQueryResource param);
        Task<List<InvRequestDt>> GetRequestDtList(GetRequestDtListQueryResource param);
        Task<List<InvRequestHd>> GetRequestHdList(GetRequestHdListQueryResource param);
        Task<Guid> InsertTransferOut(ModelTransferOutHeader param);
        Task UpdateTransferOut(ModelTransferOutHeader param);
        InvRequestHd GetRequest(ModelTransferOutHeader param);
    }
}
