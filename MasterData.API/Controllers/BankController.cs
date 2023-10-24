using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers.Bank
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : BaseController
    {
        readonly IBankService _bankService;
        private static readonly ILog log = LogManager.GetLogger(typeof(BankController));

        public BankController(
            IBankService bankService, 
            PTMaxstationContext context) : base(context)
        {
            _bankService = bankService;
        }

        /// <summary>
        /// Lists all reason.
        /// </summary>
        /// <returns>List of reason.</returns>
        [HttpPost("GetBankList")]
        public async Task<IActionResult> GetBankListAsync([FromBody] BankQuery req)
        {
            ResponseData<List<MasBank>> response = new ResponseData<List<MasBank>>();
            try
            {
                var result = await _bankService.ListAsync(req);
                response.Data = result.Items;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }
    }
}
