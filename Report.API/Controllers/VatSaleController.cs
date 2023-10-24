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
    public class VatSaleController : BaseController
    {

        private readonly IVatSaleService _vatsaleService;
        private static readonly ILog log = LogManager.GetLogger(typeof(VatSaleController));

        public VatSaleController(IVatSaleService vatsaleService, PTMaxstationContext context) : base(context)
        {
            _vatsaleService = vatsaleService;
        }


        [HttpGet("Test/{hello}")]
        public IActionResult GetReceivePayPDF(string hello)
        {
            try
            {
                var result = hello;
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetVatSalePDF")]
        public IActionResult GetReceivePayPDF(VatSaleRequest req)
        {
            try
            {
                var result = _vatsaleService.GetVatSalePDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
