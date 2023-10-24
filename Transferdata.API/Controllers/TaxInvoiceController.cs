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
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxInvoiceController : BaseController
    {

        private readonly ITaxInvoiceService _taxinvoiceService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public TaxInvoiceController(
            ITaxInvoiceService taxinvoiceservice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _taxinvoiceService = taxinvoiceservice;
            _mapper = mapper;
        }




        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListTaxInvoice")]
        [ProducesResponseType(typeof(List<TaxInvoiceQuery>), 200)]
        public async Task<IActionResult> ListTaxInvoiceAsync([FromBody] TaxInvoiceQuery query)
        {
            ResponseData<List<SalTaxinvoiceHd>> response = new ResponseData<List<SalTaxinvoiceHd>>();
            try
            {
                response.Data = await _taxinvoiceService.ListTaxInvoiceAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListTaxInvoiceAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



    }
}
