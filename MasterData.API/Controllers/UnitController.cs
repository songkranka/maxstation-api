using AutoMapper;
using log4net;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MasterData.API.Domain.Models.Queries;
using System.Threading.Tasks;
using MasterData.API.Resources;
using MasterData.API.Resources.Unit;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using MasterData.API.Domain.Models;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class UnitController : BaseController
    {
        private readonly IUnitService _unitService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        private readonly IMapper _mapper;

        public UnitController(
            IUnitService unitService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _unitService = unitService;
            _mapper = mapper;
        }

        [HttpPost("List")]
        [ProducesResponseType(typeof(QueryResultResource<MasUnit>), 200)]
        public async Task<QueryResult<MasUnit>> List([FromBody] UnitQueryResource pQuery)
        {
            var unitQuery = _mapper.Map<UnitQueryResource, UnitQuery>(pQuery);
            var resource = await _unitService.List(unitQuery);
            return resource;
        }

        [HttpGet("GetMasUnitById/{unitId}")]
        public async Task<IActionResult> GetMasUnitAsync(string unitId)
        {
            ResponseData<MasUnit> response = new ResponseData<MasUnit>();
            
            try
            {
                response.Data = await _unitService.GetMasUnitAsync(unitId);
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

        [HttpPost("SaveUnit")]
        public async Task<IActionResult> SaveBranchTank([FromBody] SaveUnitRequest request)
        {
            ResponseData<MasUnit> response = new ResponseData<MasUnit>();

            try
            {
                var result = await _unitService.SaveUnitAsync(request);

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
        public async Task<IActionResult> UpdateStatus([FromBody] SaveUnitRequest request)
        {
            ResponseData<MasUnit> response = new ResponseData<MasUnit>();

            try
            {
                var result = await _unitService.UpdateStatusAsync(request);

                if (!result.Success)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = result.Message;
                    return Ok(JsonConvert.SerializeObject(response));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
