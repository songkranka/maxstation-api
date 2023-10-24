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
    public class SalesController : BaseController
    {
        private readonly ISalesService service;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReceivePayController));

        public SalesController(ISalesService salesService, PTMaxstationContext context) : base(context)
        {
            service = salesService;
        }


        [HttpPost("GetSale02PDF")]
        public async Task<IActionResult> GetSale02PDF(SalesRequest req)
        {
            try
            {
                var result = await service.GetSale02PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetSale02Excel")]
        public async Task<IActionResult> GetSale02Excel(SalesRequest req)
        {
            try
            {
                var result = await service.GetSale02ExcelAsync(req);

                string reportType = string.Empty;

                switch (req.ReportType)
                {
                    case Domain.Enums.ReportType.Summary:
                        reportType = Domain.Enums.ReportType.Summary.ToString();
                        break;
                    case Domain.Enums.ReportType.Detail:
                        reportType = Domain.Enums.ReportType.Detail.ToString();
                        break;
                }

                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"salereport02-{reportType}-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetSale03PDF")]
        public IActionResult GetSale03PDF(SalesRequest req)
        {
            try
            {
                var result = service.GetSale03PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetSale03Excel")]
        public async Task<IActionResult> GetSale03Excel(SalesRequest req)
        {
            try
            {
                var result = await service.GetSale03ExcelAsync(req);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"salereport03-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetSale04PDF")]
        public IActionResult GetSale04PDF(SalesRequest req)
        {
            try
            {
                var result = service.GetSale04PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetSale04Excel")]
        public async Task<IActionResult> GetSale04Excel(SalesRequest req)
        {
            try
            {
                var result = await service.GetSale04ExcelAsync(req);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"salereport04-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetSale06PDF")]
        public IActionResult GetSale06PDF(SalesRequest req)
        {
            try
            {
                var result = service.GetSale06PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetSale06Excel")]
        [Produces("application/json")]
        public async Task<IActionResult> GetSale06Excel(SalesRequest req)
        {
            try
            {
                var result = await service.GetSale06ExcelAsync(req);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"salereport06-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}