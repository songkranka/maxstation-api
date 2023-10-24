using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Meter;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Repositories;
using DailyOperation.API.Domain.Services;
using DailyOperation.API.Domain.Services.Communication;
using DailyOperation.API.Resources.POS;
using Inventory.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Services
{
    public class MeterService : IMeterService
    {
        private readonly IMeterRepository _meterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private PTMaxstationContext _context;
        public MeterService(
            IMeterRepository meterRepository,
            IUnitOfWork unitOfWork , 
            PTMaxstationContext pContext)
        {
            _meterRepository = meterRepository;
            _unitOfWork = unitOfWork;
            _context = pContext;
        }

        public Task<MeterResponse> DeleteDocument(DeleteDocumentRequest req)
        {
            return _meterRepository.DeleteDocument(req);
        }

        public Task<GetResponse> GetDocument(GetDocumentRequest req)
        {
            return _meterRepository.GetDocument(req);
        }

        public Task<MeterResponse> SaveDocument(SaveDocumentRequest req)
        {             
            return _meterRepository.SaveDocument(req);
        }

        public Task<MeterResponse> SubmitDocument(SaveDocumentRequest req)
        {
            var result = _meterRepository.SubmitDocument(req);
            _meterRepository.SaveLog(req);           
            return result;
        }

        public Task<QueryResult<MasBranchMeterResponse>> GetPosMeter(BranchMeterRequest req)
        {
            return _meterRepository.GetPosMeter(req);
        }

        public QueryResult<MasBranchCalibrate> GetMasBranchCalibrate(MasBranchCalibrateRequest req)
        {
            return _meterRepository.GetMasBranchCalibrate(req);
        }

        public async Task<MasReason[]> GetHoldReason()
        {
            var qryReason = _context.MasReasons.Where(x => x.ReasonGroup == "Tank").AsNoTracking();
            //qryReason.ToQueryString();
            var result = await qryReason.ToArrayAsync();
            return result;
        }

        public async Task<MeterResponse> ValidatePOS(string pStrBrnCode, int pIntPeriodNo, DateTime pDatDocDate)
        {
            return await _meterRepository.ValidatePOS(pStrBrnCode, pIntPeriodNo, pDatDocDate);
        }
    }
}
