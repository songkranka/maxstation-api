using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Services;
using System;
using System.Threading.Tasks;

namespace Report.API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : BaseController
    {
        private readonly IFinanceService service;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummarySaleController));
        public FinanceController(IFinanceService financeService, PTMaxstationContext context) : base(context)
        {
            service = financeService;
        }


        [HttpPost("GetFin03PDF")]
        public IActionResult GetFin03PDF(FinanceRequest req)
        {
            try
            {
                var result = service.GetFin03PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetFin03Excel")]
        public async Task<IActionResult> GetFin03ExcelAsync(FinanceRequest req)
        {
            try
            {
                var result = await service.GetFin03ExcelAsync(req);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Finaance03-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetFin08PDF")]
        public IActionResult GetFin08PDF(FinanceRequest req)
        {
            try
            {
                var result = service.GetFin08PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetFin08Excel")]
        public async Task<IActionResult> GetFin08Excel(FinanceRequest req)
        {
            try
            {
                var result = await service.GetFin08ExcelAsync(req);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Finaance08-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

    }
}
