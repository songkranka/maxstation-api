using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReceivePayController : BaseController
    {
        private readonly IReceivePayService _receivePayService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReceivePayController));

        public ReceivePayController(IReceivePayService receivePayService,PTMaxstationContext context) : base(context)
        {
            _receivePayService = receivePayService;
        }


        [HttpPost("GetReceivePayPDF")]
        public async Task<IActionResult> GetReceivePayPDF(ReceivePayRequest req)
        {
            try
            {
                var result = await _receivePayService.GetReceivePayPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
