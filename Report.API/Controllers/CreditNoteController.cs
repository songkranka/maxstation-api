using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models.Requests;
using Report.API.Resources;
using System.Threading.Tasks;
using System;
using MaxStation.Entities.Models;
using AutoMapper;
using Report.API.Domain.Services;

namespace Report.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CreditNoteController : BaseController
    {
        private readonly ICreditNoteService _Service;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TaxInvoiceController));

        public CreditNoteController(
            ICreditNoteService service,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _Service = service;
            _mapper = mapper;
        }


        [HttpPost("PrintPdf")]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PrintPdf(CreditNoteRequest request)
        {
            try
            {
                var response = await _Service.GetDocumentAsync(request);
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

    }
}
