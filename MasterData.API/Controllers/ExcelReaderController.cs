using log4net;
using MasterData.API.Domain.Repositories;
using MasterData.API.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelReaderController : Controller
    {
        private ExcelReaderService _svExcelReader = null;
        private ILog _log = LogManager.GetLogger(typeof(ExcelReaderController));
        public ExcelReaderController(PTMaxstationContext pContext , IUnitOfWork pUnitOfWork)
        {
            _svExcelReader = new ExcelReaderService(pContext, pUnitOfWork);
        }
        [HttpPost("AddMasBranchCalibrate")]
        public async Task<IActionResult> AddMasBranchCalibrate(MasBranchCalibrate param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetArraySysApprove",
                pFunc: async () => await _svExcelReader.AddMasBranchCalibrate(param),
                pLog: _log
            );
        }
    }
}
