using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class PostDayController : BaseController
    {

        private readonly IPostDayService _service;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummarySaleController));
        public PostDayController(IPostDayService service, PTMaxstationContext context) : base(context)
        {
            _service = service;
        }


        // รายงานตรวจสอบรวม
        [HttpPost("GetWorkDateExcel")]
        public IActionResult GetWorkDateExcel(PostDayRequest req)
        {
            try
            {
                var result = _service.GetWorkDateExcel(req).ToArray();
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
