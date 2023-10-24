using AutoMapper;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MaxmeController : Controller
    {
        private readonly IMaxmeService maxmeService;
        private readonly IMapper mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public MaxmeController(IMaxmeService _maxmeService, IMapper _mapper)
        {
            maxmeService = _maxmeService;
            mapper = _mapper;
        }


        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("GetMaxPosSum")]
        [ProducesResponseType(typeof(List<MaxmeQuery>), 200)]
        public async Task<IActionResult> GetMaxPosSum([FromBody] MaxmeQuery query)
        {
            MaxmeResponse response = new MaxmeResponse();
            try
            {
                response = await maxmeService.GetMaxPosSum(query);
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                response.ResCode = StatusCodes.Status400BadRequest.ToString();
                response.Status = ex.Message;
                return BadRequest(response);
            }
        }



    }
}
