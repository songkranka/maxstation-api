using Inventory.API.Domain.Models;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Repositories
{
    public interface ITransferInRepository
    {
        public Task<ModelResponseData<List<ModelTransferInHeader>>> SearchTranIn(SearchTranInQueryResource param);
        public Task<List<InvTraninDt>> GetListTransferInDetail(ModelTransferInHeader pHeader);
        public Task<List<InvTranoutHd>> GetTransOutHdList(GetTransOutHdListQueryResource param);
        public Task<List<InvTranoutDt>> GetTransOutDtList(ModelTransferOutHeader param);
        public Task<ModelTransferInHeader> InsertTransferIn(ModelTransferInHeader param);
        public void UpdateTransferIn(ModelTransferInHeader param);
        Task<bool> CheckTranferByRefNo(ModelTransferInHeader param);
        Task<InvTraninHd> GetTranferInHdByGuid(Guid guid);
    }
}
