using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.EmployeeAuth;
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

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAuthController : BaseController
    {
        private readonly IEmployeeAuthService _employeeAuthCenterService;

        private static readonly ILog _log = LogManager.GetLogger(typeof(CompanyCarController));
        private readonly IMapper _mapper;
        public EmployeeAuthController(
            IEmployeeAuthService employeeAuthCenterService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _employeeAuthCenterService = employeeAuthCenterService;
            _mapper = mapper;
        }

        [HttpGet("FindByEmpCode/{empCode}")]
        public async Task<IActionResult> FindByEmpCodeAsync(string empCode)
        {
            ResponseData<AutEmployeeRole> response = new ResponseData<AutEmployeeRole>();

            try
            {
                response.Data = await _employeeAuthCenterService.FindByEmpCodeAsync(empCode);
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

        [HttpPost("SaveEmployeeAuth")]
        public async Task<IActionResult> SaveEmployeeAuth([FromBody] SaveEmployeeAuthRequest request)
        {
            ResponseData<SaveEmployeeAuthRequest> response = new ResponseData<SaveEmployeeAuthRequest>();

            try
            {
                var result = await _employeeAuthCenterService.SaveEmployeeAuthAsync(request);

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

        [HttpGet("GetArrOrganize/{pStrEmpCode}")]
        public async Task<IActionResult> GetArrOrganize(string pStrEmpCode)
        {
            IActionResult result;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "GetArrOrganize", 
                pFunc: async()=> await _employeeAuthCenterService.GetArrOrganize(pStrEmpCode), 
                pLog: _log
            );
            return result;
        }

        [HttpGet("GetAuthBranch/{compCode}/{empCode}")]
        public async Task<IActionResult> GetAuthBranch(string compCode,string empCode)
        {
            IActionResult result;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "GetAuthBranch",
                pFunc: async () =>  await _employeeAuthCenterService.GetAuthBranch(compCode,empCode),
                pLog: _log
            );
            return result;
        }

    }
}
