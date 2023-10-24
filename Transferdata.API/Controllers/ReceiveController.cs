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
    public class ReceiveController : BaseController
    {
        private readonly IReceiveService _receiveService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReceiveController));

        public ReceiveController(
            IReceiveService receiveervice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _receiveService = receiveervice;
            _mapper = mapper;
        }




        /// <summary>
        /// Lists all existing receive.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListReceive")]
        [ProducesResponseType(typeof(List<ReceiveQuery>), 200)]
        public async Task<IActionResult> ListCashSaleAsync([FromBody] ReceiveQuery query)
        {
            ResponseData<List<FinReceiveHd>> response = new ResponseData<List<FinReceiveHd>>();
            try
            {
                response.Data = await _receiveService.ListReceiveAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListCashSaleAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



    }
}
