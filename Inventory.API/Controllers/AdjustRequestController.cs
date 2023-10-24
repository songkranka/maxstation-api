using AutoMapper;
using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services;
using Inventory.API.Resources;
using Inventory.API.Resources.AdjustRequest;
using log4net;
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
    public class AdjustRequestController : BaseController
    {
        private readonly IAdjustRequestService _AdjustRequestService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(RequestController));

        public AdjustRequestController(IAdjustRequestService AdjustRequestService, IMapper mapper, PTMaxstationContext context) : base(context)
        {
            _AdjustRequestService = AdjustRequestService;
            _mapper = mapper;
        }

        [HttpPost("GetAdjustRequestHDList")]
        [ProducesResponseType(typeof(QueryResultResource<AdjustRequestResource>), 200)]
        public async Task<QueryResultResource<AdjustRequestResource>> GetAdjustRequestHDList([FromBody] AdjustRequestQueryResource req)
        {
            QueryResultResource<AdjustRequestResource> response = new QueryResultResource<AdjustRequestResource>();
            try
            {
                var queryResult = await _AdjustRequestService.GetAdjustRequestHDList(req);

                response = _mapper.Map<QueryResult<InvAdjustRequestHd>, QueryResultResource<AdjustRequestResource>>(queryResult);
                response.IsSuccess = true;
                response.Message = "";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        [HttpPost("GetAdjustRequestDTList")]
        [ProducesResponseType(typeof(QueryResultResource<InvAdjustRequestDt>), 200)]
        public async Task<QueryResultResource<InvAdjustRequestDt>> GetAdjustRequestDTList([FromBody] AdjustRequestDtQueryResource req)
        {
            QueryResultResource<InvAdjustRequestDt> response = new QueryResultResource<InvAdjustRequestDt>();
            try
            {
                var queryResult = await _AdjustRequestService.GetDetailAsync(req.CompCode, req.BrnCode, req.LocCode, req.DocNo);

                response.Items = queryResult;
                response.IsSuccess = true;
                response.Message = "";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        [HttpGet("GetAdjustRequest/{guid}")]
        public async Task<IActionResult> GetAdjustRequest(Guid guid)
        {
            ResponseData<InvAdjustRequestHd> response = new ResponseData<InvAdjustRequestHd>();

            try
            {
                response.Data = await _AdjustRequestService.FindByIdAsync(guid);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("GetAdjustRequest", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("UpdateAdjustRequest/{guid}")]
        [ProducesResponseType(typeof(AdjustRequestResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> UpdateAdjustRequest(Guid guid, [FromBody] InvAdjustRequestHd AdjustRequestHd)
        {
            var result = await _AdjustRequestService.UpdateAsync(guid, AdjustRequestHd);
            if (!result.Success)
            {
                _log.Error("UpdateAdjustRequest : " + result.Message);
                return BadRequest(new ErrorResource(result.Message));
            }

            var cashSaleHdResource = _mapper.Map<InvAdjustRequestHd, AdjustRequestResource>(result.Resource);
            return Ok(cashSaleHdResource);
        }

        [HttpPost("CreateAdjustRequest")]
        [ProducesResponseType(typeof(AdjustRequestResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateAdjustRequestAsync([FromBody] InvAdjustRequestHd model)
        {
            ResponseData<AdjustRequestResource> response = new ResponseData<AdjustRequestResource>();
            try
            {
                var result = await _AdjustRequestService.CreateAsync(model);

                if (!result.Success)
                {
                    _log.Error("CreateAdjustRequest : " + result.Message);
                    return BadRequest(new ErrorResource(result.Message));
                }

                var AdjustRequestResource = _mapper.Map<InvAdjustRequestHd, AdjustRequestResource>(result.Resource);
                response.Data = AdjustRequestResource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("CreateAdjustRequest", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
