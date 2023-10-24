using AutoMapper;
using log4net;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.CostCenter;
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
using MasterData.API.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CostCenterController : BaseController
    {
        private readonly ICostCenterService _costCenterService;

        private static readonly ILog _log = LogManager.GetLogger(typeof(CompanyCarController));
        private readonly IMapper _mapper;
        public CostCenterController(
            ICostCenterService costCenterService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _costCenterService = costCenterService;
            _mapper = mapper;
        }

        [HttpPost("List")]
        [ProducesResponseType(typeof(QueryResultResource<MasCostCenter>), 200)]
        public async Task<QueryResult<MasCostCenter>> ListAsync([FromBody] CosCenterQueryResource query)
        {
            var costCenterQuery = _mapper.Map<CosCenterQueryResource, CosCenterQuery>(query);
            var resource = await _costCenterService.ListAsync(costCenterQuery);
            return resource;
        }

        [HttpGet("GetCostCenterByGuid/{guid}")]
        public async Task<IActionResult> GetCostCenterAsync(Guid guid)
        {
            ResponseData<MasCostCenter> response = new ResponseData<MasCostCenter>();
            try
            {
                response.Data = await _costCenterService.GetCostCenterByGuid(guid);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("Method GetCostCenterAsync Error", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("SaveCostCenter")]
        public async Task<IActionResult> SaveCostCenterAsync([FromBody] MasCostCenterRequest request)
        {
            var response = new ResponseData<MasCostCenter>();
            var host = HttpContext.Request.Host.Value;
            var path = HttpContext.Request.Path;
            var method = HttpContext.Request.Method;

            try
            {
                var result = await _costCenterService.SaveCostCenterAsync(request, host, path, method);

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

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] MasCostCenterRequest request)
        {
            var response = new ResponseData<MasCostCenter>();
            var host = HttpContext.Request.Host.Value;
            var path = HttpContext.Request.Path;
            var method = HttpContext.Request.Method;
            try
            {
                var result = await _costCenterService.UpdateCostCenterAsync(request, host, path, method);

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
