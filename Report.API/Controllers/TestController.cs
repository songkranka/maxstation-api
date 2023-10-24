using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class TestController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TaxInvoiceController));
        private readonly ITaxInvoiceService _taxInvoiceService;

        public TestController(
            ITaxInvoiceService taxInvoiceService,
            PTMaxstationContext context) : base(context)
        {
            _taxInvoiceService = taxInvoiceService;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        //[HttpGet("Print")]
        //[ProducesResponseType(typeof(ErrorResource), 400)]
        //public async Task<IActionResult> Print()
        //{
        //    try
        //    {
        //        var response = await _taxInvoiceService.Demo();
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error", ex);
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
