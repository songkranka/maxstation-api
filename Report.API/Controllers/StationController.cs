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
    public class StationController : BaseController
    {

        private readonly IStationService _stationService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReceivePayController));

        public StationController(IStationService stationService, PTMaxstationContext context) : base(context)
        {
            _stationService = stationService;
        }

        // รายงานตรวจสอบรวม
        [HttpPost("GetST310Excel")]
        public IActionResult GetST310Excel(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST310Excel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("GetST312PDF")]
        public IActionResult GetST312PDF(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST312PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        // รายงานการขายประจำวัน
        [HttpPost("GetST313PDF")]
        public IActionResult GetST313PDF(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST313PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }


        // รายงานยอดขาดเกินน้ำมันใสที่สถานีบริการ
        [HttpPost("GetST313Excel")]
        public IActionResult GetST313Excel(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST313Excel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        // รายงานยอดขาดเกินน้ำมันใสที่สถานีบริการ
        [HttpPost("GetST315PDF")]
        public IActionResult GetST315PDF(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST315PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        // รายงานยอดขาดเกินน้ำมันใสที่สถานีบริการ
        [HttpPost("GetST315Excel")]
        public IActionResult GetST315Excel(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST315Excel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        // รายงานสรุปยอดขาดเกินน้ำมันใสที่สถานีบริการ
        [HttpPost("GetST316PDF")]
        public IActionResult GetST316PDF(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST316PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetST316Excel")]
        public IActionResult GetST316Excel(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST316Excel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }

        }

        // รายงานยอดขายแยกตามกะ
        [HttpPost("GetST317PDF")]
        public IActionResult GetST317PDF(StationRequest req)
        {
            try
            {
                var result = _stationService.GetST317PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetST317Excel")]
        public IActionResult GetST317Excel(StationRequest req)
        {           
            try
            {
                var result = _stationService.GetST317Excel(req).ToArray();
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
