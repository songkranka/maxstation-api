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
    public class ReportStockController : BaseController
    {
        private readonly IReportStockService _reportStockService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummaryOilBalanceController));

        public ReportStockController(
            IReportStockService reportStockService,
            PTMaxstationContext context) : base(context)

        {
            _reportStockService = reportStockService;
        }

        [HttpPost("GetReportStockExcel")]
        public IActionResult GetReportStockExcel(ReportStockRequest req)
        {
            try
            {

                var result = _reportStockService.GetReportStockExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetReportStockPDF")]
        public async Task<IActionResult> GetReportStockPDF(ReportStockRequest req)
        {
            try
            {
                var result = await _reportStockService.GetReportStockPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("GetReportStockTest")]
        //public IActionResult GetReportStockTest(ReportStockRequest req)
        //{
        //    try
        //    {
        //        var result = _reportStockService.GetReportStockTest(req);
        //        return Ok(JsonConvert.SerializeObject(result));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error", ex);
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("GetMonthlyPDF")]
        public IActionResult GetMonthlyPDF(ReportStockRequest req)
        {
            try
            {
                var result = _reportStockService.GetMonthlyPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetMonthlyExcel")]
        public IActionResult GetMonthlyExcel(ReportStockRequest req)
        {
            try
            {

                var result = _reportStockService.GetMonthlyExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("GetReportStockTest")]
        //public IActionResult GetReportStockTest(ReportStockRequest req)
        //{
        //    try
        //    {
        //        var result = _reportStockService.GetReportStockTest(req);
        //        return Ok(JsonConvert.SerializeObject(result));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error", ex);
        //        return BadRequest(ex.Message);
        //    }
        //}

    }
}
