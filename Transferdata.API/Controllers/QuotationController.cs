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
using Transferdata.API.Resources.Quotation;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : BaseController
    {

        private readonly IQuotationService _quotationService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public QuotationController(
            IQuotationService quotationservice,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _quotationService = quotationservice;
            _mapper = mapper;
        }



        /// <summary>
        /// Saves a new SalCashsaleHd.
        /// </summary>
        /// <param name="resource">SalCashsaleHd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateSale")]
        [ProducesResponseType(typeof(QuotationResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateSaleAsync([FromBody] QuotationResource resource)
        {
            ResponseData<LogResource> response = new ResponseData<LogResource>();
            try
            {                                   
                response.Data = await _quotationService.CreateRemainQuotation(resource);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreateSaleAsync Request: {JsonConvert.SerializeObject(resource)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;                
                return BadRequest(response);
            }
        }


        /// <summary>
        /// Saves a new SalCashsaleHd.
        /// </summary>
        /// <param name="resource">SalCashsaleHd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("EditSale")]
        [ProducesResponseType(typeof(QuotationResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> EditSaleAsync([FromBody] QuotationResource resource)
        {
            ResponseData<SalQuotationHd> response = new ResponseData<SalQuotationHd>();
            try
            {
                var result = await _quotationService.UpdateRemainQuotation(resource);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"EditSaleAsync Request: {JsonConvert.SerializeObject(resource)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;                
                return BadRequest(response);
            }
        }



        /// <summary>
        /// Saves a new SalCashsaleHd.
        /// </summary>
        /// <param name="resource">SalCashsaleHd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CancelSale")]
        [ProducesResponseType(typeof(QuotationResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CancelSaleAsync([FromBody] QuotationResource resource)
        {
            ResponseData<LogResource> response = new ResponseData<LogResource>();
            try
            {                
                response.Data = await _quotationService.CancelRemainQuotation(resource);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CancelSaleAsync Request: {JsonConvert.SerializeObject(resource)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;                
                return BadRequest(response);
            }
        }


        /************ Get **************/
        /// <summary>
        /// Get Quotation by doc_no.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("GetQuotation")]
        [ProducesResponseType(typeof(List<QuotationResource>), 200)]
        public async Task<IActionResult> GetQuotationAsync([FromBody] QuotationResource query)
        {
            ResponseData<SalQuotationHd> response = new ResponseData<SalQuotationHd>();
            try
            {
                if (query.DocNo == "")
                {
                    throw new Exception("ต้องระบุเลขที่ใบเสนอราคา");
                }

                response.Data = await _quotationService.GetQuotationAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"GetQuotationAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }


        /// <summary>
        /// Get Quotation by max card id.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("ListByMaxCard")]
        [ProducesResponseType(typeof(List<QuotationResource>), 200)]
        public async Task<IActionResult> ListByMaxCardAsync([FromBody] QuotationResource query)
        {
            ResponseData<List<QuotationMaxCardResource>> response = new ResponseData<List<QuotationMaxCardResource>>();
            try
            {
                if(query.MaxCardId == "")
                {
                    throw new Exception("ต้องระบุรหัส Max Card");
                }

                response.Data = await _quotationService.ListByMaxCardAsync(query); ;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"ListByMaxCardAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



    }
}
