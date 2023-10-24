using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using Inventory.API.Domain.Services;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Services
{

    public class TransOutService : ITransferOutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private ITransferOutRepository _repository;
        private PTMaxstationContext _context;
        public TransOutService(IUnitOfWork pUnitOfWork , ITransferOutRepository pRepository , PTMaxstationContext pContext)
        {
            _unitOfWork = pUnitOfWork;
            _repository = pRepository;
            _context = pContext;

        }

        public async Task<List<InvRequestHd>> GetRequestHdList(GetRequestHdListQueryResource param)
        {
            return await _repository.GetRequestHdList(param);
        }

        public async Task<ResponseData<List<ModelTransferOutHeader>>> GetTransferOutList(TransferOutQueryResource param)
        {
            return await _repository.GetTransferOutList(param);
        }

        public async Task<List<InvRequestDt>> GetRequestDtList(GetRequestDtListQueryResource param)
        {
            return await _repository.GetRequestDtList(param);
        }

        public async Task<Guid> InsertTransferOut(ModelTransferOutHeader param)
        {
            try
            {
                var tranferInIsValid =  _repository.GetRequest(param);

                if (tranferInIsValid != null && tranferInIsValid.DocStatus == "Reference")
                {

                    throw new Exception($@"ใบร้องขอเลขที่ {param.RefNo} ถูกทำโอนจ่ายแล้ว {Environment.NewLine} กรุณาตรวจสอบอีกครั้ง");
                }

                var guid = await _repository.InsertTransferOut(param);

                await _unitOfWork.CompleteAsync();
                return guid;
            }
            catch (Exception ex)
            {                
                throw new Exception($"{ex.Message}");
            }
            
        }

        public async Task UpdateTransferOut(ModelTransferOutHeader param)
        {
            await _repository.UpdateTransferOut(param);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<ModelStockRealTime[]> CheckStockRealTime(ModelCheckStockRealtimeParam param)
        {
            if(param == null)
            {
                return null;
            }
            string strSpCheckStock = @"exec sp_check_stock_realtime @p0,@p1,@p2,@p3";
            var arrParam = new SqlParameter[]
            {
                new SqlParameter("@p0", param.CompCode),
                new SqlParameter("@p1",param.BrnCode),
                new SqlParameter("@p2",param.DocDate),
                new SqlParameter("@p3",param.Json)
            };
            ModelStockRealTime[] result = null;
            using (var dt = await DefaultService.GetDataTable(_context, strSpCheckStock, arrParam))
            {
                if (dt != null)
                {
                    result = DefaultService.GetEntityFromDataTable<ModelStockRealTime[]>(dt);
                }
            }
            return result;
        }
    }
}
