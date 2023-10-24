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
    public class WithdrawController : BaseController
    {
        private readonly IWithdrawService _withdrawService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public WithdrawController(
            IWithdrawService withdrawService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _withdrawService = withdrawService;
            _mapper = mapper;
        }



        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListWithdraw")]
        [ProducesResponseType(typeof(List<WithdrawQuery>), 200)]
        public async Task<IActionResult> ListTransferOutAsync([FromBody] WithdrawQuery query)
        {
            ResponseData<List<InvWithdrawHd>> response = new ResponseData<List<InvWithdrawHd>>();
            try
            {
                response.Data = await _withdrawService.ListWithdrawAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListTransferOutAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

    }
}
