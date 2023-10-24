using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Services;
using Transferdata.API.Resources;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : BaseController
    {

        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public InvoiceController(
            IInvoiceService creditsaleservice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _invoiceService = creditsaleservice;
            _mapper = mapper;
        }


        /// <summary>
        /// Saves a new cashsalehd.
        /// </summary>
        /// <param name="resource">cashsalehd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateInvoiceList")]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateInvoiceListAsync([FromBody] List<SalCreditsaleHd> invoiceList)
        {
            ResponseData<LogResource> response = new ResponseData<LogResource>();
            try
            {
                response.Data = await _invoiceService.SaveListAsync(invoiceList);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreateInvoiceListAsync Request: {JsonConvert.SerializeObject(invoiceList)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;               
                return BadRequest(response);
            }
        }

    }
}
