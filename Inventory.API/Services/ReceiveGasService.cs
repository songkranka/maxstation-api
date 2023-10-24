using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class ReceiveGasService : IReceiveGasService
    {
        IUnitOfWork _unitOfWork = null;
        IReceiveGasRepository _repo = null;
        public ReceiveGasService(IUnitOfWork pUnitOfWork , IReceiveGasRepository pRepo)
        {
            _unitOfWork = pUnitOfWork;
            _repo = pRepo;
        }
        public async Task<InfPoHeader[]> GetPoHeaderList(PoHeaderListQuery pQuery)
        {
            return await _repo.GetPoHeaderList(pQuery);
        }
        public async Task<PoItemListResult> GetPoItemList(PoItemListParam param)
        {
            return await _repo.GetPoItemList(param);
        }
        public async Task<ReceiveGasQuery> GetReceive(string pStrGuid)
        {
            return await _repo.GetReceive(pStrGuid);
        }

        public async Task<QueryResult<InvReceiveProdHd>> GetReceiveList(ReceiveGasListQuery pQuery)
        {
            return await _repo.GetReceiveList(pQuery);
        }

        public async Task<ReceiveGasQuery> SaveReceive(ReceiveGasQuery pQuery)
        {
            await _repo.SaveReceive(pQuery);
            await _unitOfWork.CompleteAsync();
            return pQuery;
        }

        public async Task UpdateStatus(InvReceiveProdHd pInput)
        {
            await _repo.UpdateStatus(pInput);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<MasSupplier[]> GetArraySupplier()
        {
            return await _repo.GetArraySupplier();
        }

        public async Task<bool> CheckReceiveStatus(ReceiveGasQuery saveReceiveGas)
        {
            return await _repo.CheckReceiveStatus(saveReceiveGas);
        }
    }
}
