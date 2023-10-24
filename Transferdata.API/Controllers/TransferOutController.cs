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
    public class TransferOutController : BaseController
    {
        private readonly ITransferOutService _transoutService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public TransferOutController(
            ITransferOutService transoutService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _transoutService = transoutService;
            _mapper = mapper;
        }



        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListTransferOut")]
        [ProducesResponseType(typeof(List<TransferOutQuery>), 200)]
        public async Task<IActionResult> ListTransferOutAsync([FromBody] TransferOutQuery query)
        {
            ResponseData<List<InvTranoutHd>> response = new ResponseData<List<InvTranoutHd>>();
            try
            {
                response.Data = await _transoutService.ListTransferOutAsync(query);
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
