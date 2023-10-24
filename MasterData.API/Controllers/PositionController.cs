using log4net;
using MasterData.API.Controllers;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.Position;
using MasterData.API.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : BaseController
    {
        private IPositionService _svPosition;
        private readonly IMasPositionService _masPositionService;
        private static readonly ILog _log = LogManager.GetLogger(typeof(PositionController));
        public PositionController(PTMaxstationContext pContext, IPositionService pServicePos, IMasPositionService masPositionService) : base(pContext)
        {
            _svPosition = pServicePos;
            _masPositionService = masPositionService;
        }

        [HttpPost("GetPositionList")]
        public async Task<IActionResult> GetPositionList([FromBody] ModelGetPositionListParam param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetPositionList",
                pFunc: async () => await _svPosition.GetPositionList(param),
                pLog: _log
            );
        }
        [HttpGet("GetPosition/{pStrPosCode}")]
        public async Task<IActionResult> GetPosition(string pStrPosCode)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetPosition",
                pFunc: async () => await _svPosition.GetPosition(pStrPosCode),
                pLog: _log
            );
        }

        [HttpGet("GetSysMenuList")]
        public async Task<IActionResult> GetSysMenuList()
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetSysMenuList",
                pFunc: async () => await _svPosition.GetSysMenuList(),
                pLog: _log
            );
        }

        [HttpPost("InsertUnlock")]
        public async Task<IActionResult> InsertUnlock([FromBody] SaveUnlock param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "InsertUnlock",
                pFunc: async () => await _svPosition.InsertUnlock(param),
                pLog: _log
            );
        }

        [HttpPost("InsertPosition")]
        public async Task<IActionResult> InsertPosition([FromBody] ModelPosition param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "InsertPosition",
                pFunc: async () => await _svPosition.InsertPosition(param),
                pLog: _log
            );
        }

        [HttpPost("UpdatePosition")]
        public async Task<IActionResult> UpdatePosition([FromBody] ModelPosition param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "UpdatePosition",
                pFunc: async () => await _svPosition.UpdatePosition(param),
                pLog: _log
            );
        }

        [HttpPost("UpdateUnlock")]
        public async Task<IActionResult> UpdateUnlock([FromBody] SaveUnlock param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "UpdateUnlock",
                pFunc: async () => await _svPosition.UpdateUnlock(param),
                pLog: _log
            );
        }

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromBody] MasPosition param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "ChangeStatus",
                pFunc: async () => await _svPosition.ChangeStatus(param),
                pLog: _log
            );
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            ResponseData<List<MasPosition>> response = new ResponseData<List<MasPosition>>();

            try
            {
                response.Data = await _masPositionService.GetAll();
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetBranchConfig/{pStrPosCode}")]
        public async Task<IActionResult> GetBranchConfig(string pStrPosCode)
        {
            ResponseData<List<BranchConfig>> response = new ResponseData<List<BranchConfig>>();

            try
            {
                response.Data = await _svPosition.GetBranchConfig(pStrPosCode);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetBranchConfigDesc")]
        public async Task<IActionResult> GetBranchConfigDesc()
        {
            ResponseData<List<BranchConfig>> response = new ResponseData<List<BranchConfig>>();

            try
            {
                response.Data = await _svPosition.GetBranchConfigDesc();
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
