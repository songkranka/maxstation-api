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
    public class MeterController : BaseController
    {

        private readonly IMeterService _periodService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public MeterController(
            IMeterService meterservice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _periodService = meterservice;
            _mapper = mapper;
        }



        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("GetPeriod")]
        [ProducesResponseType(typeof(List<PeriodQuery>), 200)]
        public async Task<IActionResult> GetPeriodAsync([FromBody] PeriodQuery query)
        {
            ResponseData<DopPeriod>response = new ResponseData<DopPeriod>();
            try
            {
                response.Data = await _periodService.GetPeriodAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"GetPeriodAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }


    }
}
