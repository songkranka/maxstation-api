using Azure.Core;
using log4net;
using MasterData.API.Domain.Models;
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
    public class ApproveController : Controller
    {
        private ApproveService _svApprove;
        private ILog _log = LogManager.GetLogger(typeof(ApproveController));
        public ApproveController(PTMaxstationContext pContext, IUnitOfWork pUnitOfWork)
        {
            _svApprove = new ApproveService(pContext, pUnitOfWork);
        }

        [HttpGet("GetPendingApprove/{pStrEmpCode}")]
        public async Task<IActionResult> GetPendingApprove(string pStrEmpCode)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetPendingApprove",
                pFunc: async () => await _svApprove.GetPendingApprove(pStrEmpCode),
                pLog: _log
            );
        }
        [HttpGet("GetArraySysApprove/{pStrEmpCode}")]
        public async Task<IActionResult> GetArraySysApprove(string pStrEmpCode)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetArraySysApprove",
                pFunc: async () => await _svApprove.GetArraySysApprove(pStrEmpCode),
                pLog: _log
            );
        }

        [HttpPost("GetApprove")]
        public async Task<IActionResult> GetApprove(ModelApproveParam param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetApprove",
                pFunc: async () => await _svApprove.GetApprove(param),
                pLog: _log
            );
        }
        [HttpGet("GetApproveByGuid/{strGuid}")]
        public async Task<IActionResult> GetApproveByGuid(string strGuid)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetApproveByGuid",
                pFunc: async () => await _svApprove.GetApproveByGuid(strGuid),
                pLog: _log
            );
        }
        [HttpPost("SaveApprove")]
        public async Task<IActionResult> SaveApprove(ModelApprove param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "SaveApprove",
                pFunc: async () => await _svApprove.SaveApprove(param),
                pLog: _log
            );
        }
        [HttpGet("GetArrayConfig")]
        public async Task<IActionResult> GetArrayConfig()
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetArrayConfig",
                pFunc: async () => await _svApprove.GetArrayConfig(),
                pLog: _log
            );
        }
        [HttpGet("GetIp")]
        public string GetIp()
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
           
            var result = remoteIpAddress.ToString();
            return result;
        }
        [HttpPost("ValidateApproveDocument")]
        public async Task<IActionResult> ValidateApproveDocument(ModelApproveParam param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "ValidateApproveDocument",
                pFunc: async () => await _svApprove.ValidateApproveDocument(param),
                pLog: _log
            );
        }
        [HttpPost("SearchApprove")]
        public async Task<IActionResult> SearchApprove(ModelSearchApproveParam param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "SearchApprove",
                pFunc: async () => await _svApprove.SearchApprove(param),
                pLog: _log
            );
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(_svApprove != null)
            {
                _svApprove.Dispose();
                _svApprove = null;
                GC.Collect();
            }
        }
    }
}
