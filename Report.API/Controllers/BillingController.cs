using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Services;
using Report.API.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : BaseController
    {
        private readonly IBillingService _billingService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TaxInvoiceController));

        public BillingController(
            IBillingService billingService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _billingService = billingService;
            _mapper = mapper;
        }

        [HttpPost("PrintPdf")]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PrintPdf(BillingRequest req)
        {
            try
            {
                var  response = await _billingService.GetBilling(req);
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ExportExcel")]
        public async Task<IActionResult> ExportExcel(BillingRequest req)
        {
            try
            {
                var result = await _billingService.ExportExcelAsync(req);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"debtor02-{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
