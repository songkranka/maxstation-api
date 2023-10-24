using MasterData.API.Domain.Repositories;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class ExcelReaderService
    {
        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWork = null;
        public ExcelReaderService(PTMaxstationContext pContext , IUnitOfWork pUnitOfWork)
        {
            _context = pContext;
            _unitOfWork = pUnitOfWork;
        }
        public async Task< bool> AddMasBranchCalibrate(MasBranchCalibrate  param)
        {
            if(param == null)
            {
                return false;
            }
            var qryCalibrate = _context.MasBranchCalibrates.Where(
                x => x.CompCode == param.CompCode
                && x.BrnCode == param.BrnCode
                && x.TankId == param.TankId
                && x.SeqNo == param.SeqNo
            ).AsNoTracking();
            if (await qryCalibrate.AnyAsync())
            {
                _context.Update(param);
            }
            else
            {
                await _context.AddAsync(param);
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
