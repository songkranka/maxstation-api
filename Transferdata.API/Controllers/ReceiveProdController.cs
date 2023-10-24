using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiveProdController : BaseController
    {
        private readonly IReceiveProdService _receiveProdService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public ReceiveProdController(
            IReceiveProdService receiveprodservice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _receiveProdService = receiveprodservice;
            _mapper = mapper;
        }




        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListReceiveProd")]
        [ProducesResponseType(typeof(List<ReceiveProdQuery>), 200)]
        public async Task<IActionResult> ListReceiveProdAsync([FromBody] ReceiveProdQuery query)
        {
            ResponseData<List<InvReceiveProdHd>> response = new ResponseData<List<InvReceiveProdHd>>();
            try
            {
                response.Data = await _receiveProdService.ListReceiveProdAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListReceiveProdAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }


        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListReceiveOil")]
        [ProducesResponseType(typeof(List<ReceiveProdQuery>), 200)]
        public async Task<IActionResult> ListReceiveOilAsync([FromBody] ReceiveProdQuery query)
        {
            ResponseData<List<InvReceiveProdHd>> response = new ResponseData<List<InvReceiveProdHd>>();
            try
            {
                response.Data = await _receiveProdService.ListReceiveOilAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListReceiveOilAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListReceiveGas")]
        [ProducesResponseType(typeof(List<ReceiveProdQuery>), 200)]
        public async Task<IActionResult> ListReceiveGasAsync([FromBody] ReceiveProdQuery query)
        {
            ResponseData<List<InvReceiveProdHd>> response = new ResponseData<List<InvReceiveProdHd>>();
            try
            {
                response.Data = await _receiveProdService.ListReceiveGasAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListReceiveGasAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }






    }
}
