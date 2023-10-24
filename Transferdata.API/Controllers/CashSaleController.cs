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
using Transferdata.API.Resources.CashSale;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashSaleController : BaseController
    {
        private readonly ICashSaleService _cashsaleService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public CashSaleController(
            ICashSaleService cashsaleservice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _cashsaleService = cashsaleservice;
            _mapper = mapper;
        }



        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListCashSale")]
        [ProducesResponseType(typeof(List<CashsaleQuery>), 200)]
        public async Task<IActionResult> ListCashSaleAsync([FromBody] CashsaleQuery query)
        {
            ResponseData<List<SalCashsaleHd>> response = new ResponseData<List<SalCashsaleHd>>();
            try
            {
                response.Data = await _cashsaleService.ListCashSaleAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListCashSaleAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
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
        [HttpPost("CreateCashSaleList")]        
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateCashSaleListAsync([FromBody] List<SalCashsaleHd> cashSaleList)
        {
            ResponseData<LogResource> response = new ResponseData<LogResource>();
            try
            {
                response.Data = await _cashsaleService.SaveListAsync(cashSaleList);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreateCashSaleListAsync Request: {JsonConvert.SerializeObject(cashSaleList)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;                
                return BadRequest(response);
            }
        }



    }
}
