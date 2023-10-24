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
using Transferdata.API.Resources;
using Transferdata.API.Resources.Quotation;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditSaleController : BaseController
    {

        private readonly ICreditSaleService _creditsaleService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public CreditSaleController(
            ICreditSaleService creditsaleservice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _creditsaleService = creditsaleservice;
            _mapper = mapper;
        }



        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListCreditSale")]
        [ProducesResponseType(typeof(List<CreditsaleQuery>), 200)]
        public async Task<IActionResult> ListCreditSaleAsync([FromBody] CreditsaleQuery query)
        {
            ResponseData<List<SalCreditsaleHd>> response = new ResponseData<List<SalCreditsaleHd>>();
            try
            {
                response.Data = await _creditsaleService.ListCrditSaleAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListCreditSaleAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



        /// <summary>
        /// Saves a new cashsalehd.
        /// </summary>
        /// <param name="resource">cashsalehd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateCreditSaleList")]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateCreditSaleListAsync([FromBody] List<SalCreditsaleHd> creditsaleList)
        {
            ResponseData<LogResource> response = new ResponseData<LogResource>();
            try
            {
                response.Data = await _creditsaleService.SaveListAsync(creditsaleList);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreateCreditSaleListAsync Request: {JsonConvert.SerializeObject(creditsaleList)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;                
                return BadRequest(response);
            }
        }





    }
}
