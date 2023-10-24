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
using Transferdata.API.Domain.Queries;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Pos;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosController : BaseController
    {
        private readonly IPosService _posService;
        private static readonly ILog log = LogManager.GetLogger(typeof(PosController));

        public PosController(IPosService posService, PTMaxstationContext context) : base(context)
        {
            _posService = posService;
        }

        [HttpPost("TransferPOS")]
        [ProducesResponseType(typeof(TranferPosResponse), 200)]
        public async Task<IActionResult> CashList([FromBody] TranferPosQueryResource query)
        {
            ResponseData<TranferPosResponse> response = new ResponseData<TranferPosResponse>();

            try
            {
                response.Data = await _posService.GetDepositAmt(query);
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
