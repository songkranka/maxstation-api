using AutoMapper;
using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources;
using Inventory.API.Resources.ReceiveOil;
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
    public class ReceiveOilController : BaseController
    {
        private readonly IReceiveOilService _receiveOilService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ReceiveGasController));
        public ReceiveOilController(
            IReceiveOilService receiveOilService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _receiveOilService = receiveOilService;
            _mapper = mapper;

        }

        /// <summary>
        /// Lists all existing receiveOil.
        /// </summary>
        /// <returns>List of receiveOil.</returns>
        [HttpPost("List")]
        [ProducesResponseType(typeof(QueryResult<InvReceiveProdHd>), 200)]
        public async Task<IActionResult> ListAsync([FromBody] ReceiveOilHdQueryResource query)
        {
            var receiveOilHdQuery = _mapper.Map<ReceiveOilHdQueryResource, ReceiveOilHdQuery>(query);
            var result = await _receiveOilService.ListAsync(receiveOilHdQuery);
            return Ok(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// existing receiveoil.
        /// </summary>
        /// <returns>receiveoil.</returns>
        [HttpGet("FindById/{guid}")]
        public async Task<IActionResult> FindByIdAsync(Guid guid)
        {
            ResponseData<ReceiveOil> response = new ResponseData<ReceiveOil>();

            try
            {
                response.Data = await _receiveOilService.FindByIdAsync(guid);
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

        /// <summary>
        /// Saves a new ReceiveOil.
        /// </summary>
        /// <param name="resource">ReceiveOil data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ReceiveOilResponse), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PostAsync([FromBody] SaveReceiveOilResource resource)
        {
            ResponseData<ReceiveOilResponse> response = new ResponseData<ReceiveOilResponse>();
            try
            {
                var result = await _receiveOilService.SaveAsync(resource);

                if (!result.Success)
                {
                    response.Data = null;
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = result.Message;
                    return Ok(JsonConvert.SerializeObject(response));
                }

                response.Data = result;
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

        [HttpPost("UpdateStatus")]
        [ProducesResponseType(typeof(ReceiveOilResponse), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> UpdateStatusAsync([FromBody] SaveReceiveOilResource resource)
        {
            ResponseData<ReceiveOilResponse> response = new ResponseData<ReceiveOilResponse>();
            try
            {
                var result = await _receiveOilService.UpdateStatusAsync(resource);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result;
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
