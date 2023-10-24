using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.Unlock;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UnlockController : BaseController
    {
        private readonly IUnlockService _unlockService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(UnlockController));

        public UnlockController(
            IUnlockService unlockService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _unlockService = unlockService;
            _mapper = mapper;
        }

        [HttpPost("GetEmployeeBranchConfig")]
        public async Task<IActionResult> GetEmployeeBranchConfig([FromBody] EmployeeBranchConfigRequest req)
        {
            ResponseData<List<EmployBranchConfigResponse>> response = new ResponseData<List<EmployBranchConfigResponse>>();
            try
            {
                response.Data = await _unlockService.GetEmployeeBranchConfig(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetSysBranchConfig")]
        public async Task<IActionResult> GetSysBranchConfig([FromBody] SysBranchConfigRequest req)
        {
            ResponseData<SysBranchConfigResponse> response = new ResponseData<SysBranchConfigResponse>();
            try
            {
                response.Data = await _unlockService.GetSysBranchConfig(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("SaveUnlock")]
        public async Task<IActionResult> SaveUnlock([FromBody] SaveUnlockResource request)
        {
            ResponseData<SaveUnlockResource> response = new ResponseData<SaveUnlockResource>();

            try
            {
                var result = await _unlockService.SaveUnlockAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
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
