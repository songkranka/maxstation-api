using Inventory.API.Domain.Models;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface ITransferInService
    {
        public Task<ModelResponseData<List<ModelTransferInHeader>>>
        SearchTranIn(SearchTranInQueryResource param);
        public Task<List<InvTraninDt>> GetListTransferInDetail(ModelTransferInHeader pHeader);
        public Task<List<InvTranoutDt>> GetTransOutDtList(ModelTransferOutHeader param);
        public Task<List<InvTranoutHd>> GetTransOutHdList(GetTransOutHdListQueryResource param);
        public Task<ModelTransferInHeader> InsertTransferIn(ModelTransferInHeader param);
        public Task UpdateTransferIn(ModelTransferInHeader param);
        Task<bool> CheckTranferByRefNo(ModelTransferInHeader param);
        Task<InvTraninHd> GetTranferInHdByGuid(Guid guid);
    }
}
