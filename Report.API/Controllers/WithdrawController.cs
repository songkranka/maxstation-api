using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Requests.WithdrawRequest;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : BaseController
    {
        private readonly IWithdrawService _withdrawService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummarySaleController));
        public WithdrawController(IWithdrawService withdrawService, PTMaxstationContext context) : base(context)
        {
            _withdrawService = withdrawService;
        }




        [HttpPost("GetDocumentPDF")]
        public IActionResult GetDocumentPDF(GetDocumentRequest req)
        {
            try
            {
                var result = _withdrawService.GetDocumentPDF(req);
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
