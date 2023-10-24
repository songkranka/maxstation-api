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
    public class SummaryController : BaseController
    {
        private readonly ISummaryService _summaryService;        
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public SummaryController(
            ISummaryService summaryservice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _summaryService = summaryservice;
            _mapper = mapper;
        }



        ///// <summary>
        ///// 1. Lists of Summary ส่วนลด กลุ่มน้ำมันใส และ LPG เฉพาะการขายสด 
        ///// </summary>
        ///// <returns>List of cashSaleHd.</returns>
        [HttpPost("CashSaleOilDisc")]
        [ProducesResponseType(typeof(List<SummaryQuery>), 200)]
        public async Task<IActionResult> CashSaleOilDiscAsync([FromBody] SummaryQuery query)
        {
            ResponseData<List<CashsaleDisc>> response = new ResponseData<List<CashsaleDisc>>();
            try
            {
                response.Data = await _summaryService.ListCashsaleSummaryDiscAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CashSaleOilDiscAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        ///// <summary>
        ///// 2. Lists of Summary ส่วนลด กลุ่มน้ำมันเครื่อง ทั้งการขายสดและขายเชื่อ > comp, brn, date, ส่วนลด
        ///// </summary>
        ///// <returns>List of sale.</returns>
        [HttpPost("SaleEngineOilDisc")]
        [ProducesResponseType(typeof(List<SummaryQuery>), 200)]
        public async Task<IActionResult> SaleEngineOilDiscAsync([FromBody] SummaryQuery query)
        {
            ResponseData<List<SaleEngineOil>> response = new ResponseData<List<SaleEngineOil>>();
            try
            {
                response.Data = await _summaryService.ListSaleEngineOilSummaryDiscAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"SaleEngineOilDiscAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        ///// <summary>
        ///// 3. Lists of  Summary จำนวนเงินขายเชื่อ กลุ่มน้ำมันใส และ LPG > comp, brn, date, amount ยังไม่หักส่วนลด 
        ///// </summary>
        ///// <returns>List of sale.</returns>
        [HttpPost("CreditSaleOilAmount")]
        [ProducesResponseType(typeof(List<SummaryQuery>), 200)]
        public async Task<IActionResult> CreditSaleOilAmountAsync([FromBody] SummaryQuery query)
        {
            ResponseData<List<CreditsaleAmount>> response = new ResponseData<List<CreditsaleAmount>>();
            try
            {
                response.Data = await _summaryService.ListCreditsaleOilAmountAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreditSaleOilAmountAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        ///// <summary>
        ///// 4. Lists of  Summary จำนวนเงิน(ขายสด - ส่วนลด) กลุ่มน้ำมันเครื่อง และสินค้าอื่น (ไม่เอากลุ่มน้ำมันใส และ LPG) > comp, brn, date, net_amount
        ///// </summary>
        ///// <returns>List of sale.</returns>
        [HttpPost("CashSaleNonOilAmount")]
        [ProducesResponseType(typeof(List<SummaryQuery>), 200)]
        public async Task<IActionResult> CashSaleNonOilAmountAsync([FromBody] SummaryQuery query)
        {
            ResponseData<List<SaleNonOil>> response = new ResponseData<List<SaleNonOil>>();
            try
            {
                response.Data = await _summaryService.ListCashsaleNonOilAmountAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CashSaleNonOilAmountAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



        ///// <summary>
        ///// 5. Summary จำนวนเงิน(ขายเชื่อ - ส่วนลด) ทุกรายการ ยกเว้น material: 08898 > comp, brn, date, customer, net_amount 
        ///// </summary>
        ///// <returns>List of sale.</returns>
        [HttpPost("CreditSaleAmount")]
        [ProducesResponseType(typeof(List<SummaryQuery>), 200)]
        public async Task<IActionResult> CreditSaleAmountAsync([FromBody] SummaryQuery query)
        {
            ResponseData<List<CreditsaleAmount>> response = new ResponseData<List<CreditsaleAmount>>();
            try
            {
                response.Data = await _summaryService.ListCreditsaleAmountAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreditSaleAmountAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



    }
}
