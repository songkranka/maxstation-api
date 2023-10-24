using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.ReceiveOil;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class ReceiveOilService : IReceiveOilService
    {
        private readonly IReceiveOilRepository _receiveOilRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReceiveOilService(
            IReceiveOilRepository receiveOilRepository,
            IUnitOfWork unitOfWork)
        {
            _receiveOilRepository = receiveOilRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueryResult<InvReceiveProdHd>> ListAsync(ReceiveOilHdQuery query)
        {
            return await _receiveOilRepository.ReceiveOilHdListAsync(query);
        }

        public async Task<ReceiveOil> FindByIdAsync(Guid guid)
        {
            return await _receiveOilRepository.FindByIdAsync(guid);
        }

        public async Task<ReceiveOilResponse> SaveAsync(SaveReceiveOilResource saveReceiveOil)
        {
            try
            {
                if(saveReceiveOil.InvReceiveProdHd.DocStatus == "New")
                {
                    var checkReceiveStatus = await _receiveOilRepository.CheckReceiveStatus(saveReceiveOil);

                    if (checkReceiveStatus)
                    {
                        return new ReceiveOilResponse($"ใบสั่งซื้อเลขที่ {saveReceiveOil.InvReceiveProdHd.PoNo} ได้ทำรับแล้ว กรุณาตรวจสอบ");
                    }
                }

                await _receiveOilRepository.AddReceiveOil(saveReceiveOil);
                await _unitOfWork.CompleteAsync();

                return new ReceiveOilResponse(saveReceiveOil);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ReceiveOilResponse($"An error occurred when saving the receive oil: {ex.Message}");
            }
        }

        public async Task<ReceiveOilResponse> UpdateStatusAsync(SaveReceiveOilResource saveReceiveOil)
        {
            try
            {
                await _receiveOilRepository.UpdateStatus(saveReceiveOil);
                await _unitOfWork.CompleteAsync();

                return new ReceiveOilResponse(saveReceiveOil);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new ReceiveOilResponse($"An error occurred when saving the receive oil: {ex.Message}");
            }
        }
    }
}
