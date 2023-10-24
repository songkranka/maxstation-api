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
using Transferdata.API.Domain.Models.Response;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources.PostDay;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostDayController : BaseController
    {
        private readonly IPostDayService _postdayService;
        private static readonly ILog log = LogManager.GetLogger(typeof(PosController));

        public PostDayController(IPostDayService postdayService, PTMaxstationContext context) : base(context)
        {
            _postdayService = postdayService;
        }

        [HttpPost("GetDeposit")]
        [ProducesResponseType(typeof(DepositResponse), 200)]
        public async Task<IActionResult> GetDeposit([FromBody] PostDayQueryResource query)
        {
            ResponseData<DepositResponse> response = new ResponseData<DepositResponse>();

            try
            {
                response.Data = await _postdayService.GetDepositAmt(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CashList : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            //return null;
        }

    }
}
