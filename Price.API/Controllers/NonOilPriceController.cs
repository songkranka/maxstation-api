using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Price.API.Domain.Models;
using Price.API.Domain.Repositories;
using Price.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NonOilPriceController : Controller
    {
        private NonOilPriceService _svNonOil = null;
        private ILog _log = LogManager.GetLogger(typeof(NonOilPriceController));
        public NonOilPriceController(PTMaxstationContext pContext, IUnitOfWork pUnitOfWork)
        {
            _svNonOil = new NonOilPriceService(pContext, pUnitOfWork);
        }

        [HttpGet("GetNonOilProduct")]
        public async Task<IActionResult> GetNonOilProduct()
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetNonOilProduct",
                pFunc: async()=> await _svNonOil.GetNonOilProduct(),
                pLog: _log
            );
        }

        [HttpPost("GetNonOilPriceDetail")]
        public async Task<IActionResult> GetNonOilPriceDetail(PriNonoilDt param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetNonOilPriceDetail",
                pFunc: async () => await _svNonOil.GetNonOilPriceDetail(param),
                pLog: _log
            );
        }
        [HttpPost("SaveNonOil")]
        public async Task<IActionResult> SaveNonOil(ModelNonOilPrice param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "SaveNonOil",
                pFunc: async () => await _svNonOil.SaveNonOil(param),
                pLog: _log
            );
        }
        [HttpGet("GetNonOilPrice/{pStrGuid}")]
        public async Task<IActionResult> GetNonOilPrice(string pStrGuid)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetNonOilPrice",
                pFunc: async () => await _svNonOil.GetNonOilPrice(pStrGuid),
                pLog: _log
            );
        }

        [HttpPost("GetArrayHeader")]
        public async Task<IActionResult> GetArrayHeader(ModelNonOilPriceParam param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetArrayHeader",
                pFunc: async () => await _svNonOil.GetArrayHeader(param),
                pLog: _log
            );
        }
        [HttpPost("CancelNonOil")]
        public async Task<IActionResult> CancelNonOil(PriNonoilHd param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "CancelNonOil",
                pFunc: async () => await _svNonOil.CancelNonOil(param),
                pLog: _log
            );
        }

        [HttpGet("GetUnApproveDocument/{pStrCompCode}")]
        public async Task<IActionResult> GetUnApproveDocument(string pStrCompCode)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetUnApproveDocument",
                pFunc: async () => await _svNonOil.GetUnApproveDocument(pStrCompCode),
                pLog: _log
            );
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(_svNonOil != null)
            {
                _svNonOil.Dispose();
                _svNonOil = null;
                GC.Collect();
            }
        }
    }
}
