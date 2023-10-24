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
    public class ReportSummarySaleController : BaseController
    {
        private readonly IReportSummarySaleService _ReportSummarySaleService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummarySaleController));


        public ReportSummarySaleController(
            IReportSummarySaleService ReportSummarySaleService,

            PTMaxstationContext context) : base(context)

        {
            _ReportSummarySaleService = ReportSummarySaleService;
        }

        [HttpPost("GetReportSummarySaleExcel")]
        public IActionResult GetReportSummarySaleExcel(ReportSummarySaleRequest req)
        {
			try
			{
                var result = _ReportSummarySaleService.GetReportSummarySaleExcel(req).ToArray();
                return File(result, "application/octet-stream");
			}
			catch (Exception ex)
			{
				log.Error("Error", ex);
				return BadRequest(ex.Message);
			}
        }

        [HttpPost("GetReportSummarySalePDF")]
        public IActionResult GetReportSummarySalePDF(ReportSummarySaleRequest req)
        {
            try
            {
                var result = _ReportSummarySaleService.GetReportSummarySalePDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetPeriod")]
        public IActionResult GetPeriod(ReportSummarySaleRequest req)
        {
            try
            {
                var result = _ReportSummarySaleService.GetPeriod(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
