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
    public class CustomerController : BaseController
    {

        private readonly ICustomerService service;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummarySaleController));
        public CustomerController(ICustomerService customerService, PTMaxstationContext context) : base(context)
        {
            service = customerService;
        }

        [HttpPost("GetDebtor02PDF")]
        public IActionResult GetDebtor02PDF(CustomerRequest req)
        {
            try
            {
                var result = service.GetDebtor02PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDebtor02Excel")]
        public async Task<IActionResult> GetDebtor02Excel(CustomerRequest req)
        {
            try
            {
                var result = await service.GetDebtor02ExcelAsync(req);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"debtor02-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ExportExcel")]
        public async Task<IActionResult> ExportExcel(ExportExcelRequest req)
        {
            try
            {
                var result = await service.ExportExcelAsync(req);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"customer-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
