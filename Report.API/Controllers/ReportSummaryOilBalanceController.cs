using AutoMapper;
using FastReport.Web;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using Report.API.Domain.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Services;
using Report.API.Domain.Services.Communication;
using Report.API.Resources;
using Report.API.Resources.TaxInvoice;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportSummaryOilBalanceController : BaseController
    {
        private readonly IReportSummaryOilBalanceService _ReportSummaryOilBalanceService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummaryOilBalanceController));


        public ReportSummaryOilBalanceController(
            IReportSummaryOilBalanceService ReportSummaryOilBalanceService,

            PTMaxstationContext context) : base(context)

        {
            _ReportSummaryOilBalanceService = ReportSummaryOilBalanceService;
        }

        [HttpPost("GetReportSummaryOilBalanceExcel")]
        public IActionResult GetReportSummaryOilBalanceExcel(ReportSummaryOilBalanceRequest req)
        {
			try
			{
                var result = _ReportSummaryOilBalanceService.GetReportSummaryOilBalanceExcel(req).ToArray();
                return File(result, "application/octet-stream");
			}
			catch (Exception ex)
			{
				log.Error("Error", ex);
				return BadRequest(ex.Message);
			}
        }

        [HttpPost("GetReportSummaryOilBalancePDF")]
        public IActionResult GetReportSummaryOilBalancePDF(ReportSummaryOilBalanceRequest req)
        {
            try
            {
                var result = _ReportSummaryOilBalanceService.GetReportSummaryOilBalancePDF(req);
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
