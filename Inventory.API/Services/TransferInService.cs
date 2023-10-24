using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class TransferInService : ITransferInService
    {
        private ITransferInRepository _repo = null;
        private IUnitOfWork _unitOfWork = null;
        public TransferInService(ITransferInRepository pRepo , IUnitOfWork pUnitOfWork)
        {
            _repo = pRepo;
            _unitOfWork = pUnitOfWork;
        }

        public async Task<ModelResponseData<List<ModelTransferInHeader>>>
        SearchTranIn(SearchTranInQueryResource param)
        {
            return await _repo.SearchTranIn(param);
        }
        public async Task<List<InvTraninDt>> GetListTransferInDetail(ModelTransferInHeader pHeader)
        {
            return await _repo.GetListTransferInDetail(pHeader);
        }

        public async Task<List<InvTranoutDt>> GetTransOutDtList(ModelTransferOutHeader param)
        {
            return await _repo.GetTransOutDtList(param);
        }

        public async Task<List<InvTranoutHd>> GetTransOutHdList(GetTransOutHdListQueryResource param)
        {
            return await _repo.GetTransOutHdList(param);
        }

        public async Task<ModelTransferInHeader> InsertTransferIn(ModelTransferInHeader param)
        {
            await _repo.InsertTransferIn(param);
            await _unitOfWork.CompleteAsync();
            return param;
        }

        public async Task  UpdateTransferIn(ModelTransferInHeader param)
        {
            _repo.UpdateTransferIn(param);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> CheckTranferByRefNo(ModelTransferInHeader param)
        {
            return await _repo.CheckTranferByRefNo(param);
        }

        public async Task<InvTraninHd> GetTranferInHdByGuid(Guid guid)
        {
            return await _repo.GetTranferInHdByGuid(guid);
        }
    }
}
