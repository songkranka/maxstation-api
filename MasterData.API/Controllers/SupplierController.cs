using log4net;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services;
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
    public class SupplierController : Controller
    {
        ISupplierService _svSupplier = null;
        private static readonly ILog _log = LogManager.GetLogger(typeof(SupplierController));
        public SupplierController(ISupplierService pSvSupplier)
        {
            _svSupplier = pSvSupplier;            
        }
        //Task<ModelGetSupplierListResult> GetSupplierList(ModelGetSupplierListParam param);
        [HttpPost("GetSupplierList")]
        public async Task<IActionResult> GetSupplierList(ModelGetSupplierListParam param)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "GetSupplierList", 
                pFunc: async()=> await _svSupplier.GetSupplierList(param) , 
                pLog: _log
            );
            return result;
        }
        //Task<ModelSupplier> GetSupplier(string pStrGuid);
        [HttpGet("GetSupplier/{pStrGuid}")]
        public async Task<IActionResult> GetSupplier(string pStrGuid)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "GetSupplier",
                pFunc: async () => await _svSupplier.GetSupplier(pStrGuid),
                pLog: _log
            );
            return result;
        }
        //Task<bool> CheckDuplicateCode(string pStrSupCode);
        [HttpGet("CheckDuplicateCode/{pStrSupCode}")]
        public async Task<IActionResult> CheckDuplicateCode(string pStrSupCode)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "CheckDuplicateCode",
                pFunc: async () => await _svSupplier.CheckDuplicateCode(pStrSupCode),
                pLog: _log
            );
            return result;
        }
        //Task<MasSupplier> UpdateStatus(MasSupplier param);
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(MasSupplier param)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "UpdateStatus",
                pFunc: async () => await _svSupplier.UpdateStatus(param),
                pLog: _log
            );
            return result;
        }
        //Task<ModelSupplier> InsertSupplier(ModelSupplier param);
        [HttpPost("InsertSupplier")]
        public async Task<IActionResult> InsertSupplier(ModelSupplier param)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "InsertSupplier",
                pFunc: async () => await _svSupplier.InsertSupplier(param),
                pLog: _log
            );
            return result;
        }
        //Task<ModelSupplier> UpdateSupplier(ModelSupplier param);
        [HttpPost("UpdateSupplier")]
        public async Task<IActionResult> UpdateSupplier(ModelSupplier param)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "UpdateSupplier",
                pFunc: async () => await _svSupplier.UpdateSupplier(param),
                pLog: _log
            );
            return result;
        }


    }
}
