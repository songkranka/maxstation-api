using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Services;
using Report.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Requests.MeterRequest;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MeterController : BaseController
    {
        private readonly IMeterService _meterService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummarySaleController));
        public MeterController(IMeterService meterService,PTMaxstationContext context) : base(context)
        {
            _meterService = meterService;
        }

        // รายงานการเปลี่ยนแปลงตัวเลขมิเตอร์
        [HttpPost("GetMeterRepairPDF")]
        public IActionResult GetMeterRepairPDF(MeterTestResquest req)
        {
            try
            {
                var result = _meterService.GetMeterRepairPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }        
        [HttpPost("GetMeterRepairExcel")]
        public IActionResult GetMeterRepairExcel(MeterTestResquest req)
        {
            try
            {
                var result = _meterService.GetMeterRepairExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }


        // รายงานยอดขาดเกินน้ำมันใสที่สถานีบริการ
        [HttpPost("GetMeterTestPDF")]
        public IActionResult GetMeterTestPDF(MeterTestResquest req)
        {
            try
            {
                var result = _meterService.GetMeterTestPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("GetMeterTestExcel")]
        public IActionResult GetMeterTestExcel(MeterTestResquest req)
        {
            try
            {
                var result = _meterService.GetMeterTestExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

    }
}
