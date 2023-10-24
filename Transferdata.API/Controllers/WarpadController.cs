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
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarpadController : BaseController
    {
        private readonly IWarpadService _warpadService;        
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public WarpadController(IWarpadService warpadService,PTMaxstationContext context) : base(context)
        {
            _warpadService = warpadService;            
        }



        /// <summary>
        /// Update Post Days
        /// </summary>
        /// <param name="resource">SalCashsaleHd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CloseDay")]        
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CloseDayAsync([FromBody] TransferDataResource query)
        {
            ResponseData<WarpadCloseDay> response = new ResponseData<WarpadCloseDay>();
            try
            {
               // await _warpadService.SendCloseDay(query);
                response.Data = await _warpadService.SendCloseDay(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CloseDayAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }


    }
}
