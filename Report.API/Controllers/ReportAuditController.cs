using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class ReportAuditController : BaseController
    {
        private readonly IReportAuditService _reportAuditService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummaryOilBalanceController));

        public ReportAuditController(
            IReportAuditService reportAuditService,
            PTMaxstationContext context) : base(context)

        {
            _reportAuditService = reportAuditService;
        }

        [HttpPost("AuditDiffPDF")]
        public IActionResult GetAuditDiffPDF(ReportAuditRequest req)
        {
            try
            {
                var result = _reportAuditService.GetAuditDiffPDF(req.CompCode, req.BrnCode, req.DocNo);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AuditDetailPDF")]
        public IActionResult GetAuditDetailPDF(ReportAuditRequest req)
        {
            try
            {
                var result = _reportAuditService.GetAuditDetailPDF(req.CompCode, req.BrnCode, req.DocNo);
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
