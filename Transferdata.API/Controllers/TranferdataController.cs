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
using Transferdata.API.Domain.Services.Communication;
using Transferdata.API.Resources;
using Transferdata.API.Resources.Transferdata;

namespace Transferdata.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranferdataController : BaseController
    {
        private readonly ITransferdataService _transferdataService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(TranferdataController));

        public TranferdataController(
            ITransferdataService transferdataService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _transferdataService = transferdataService;
            _mapper = mapper;
        }



        /// <summary>
        /// Update Post Days
        /// </summary>
        /// <param name="resource">SalCashsaleHd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("UpdateCloseDay")]
        [ProducesResponseType(typeof(SalCashsaleHdResponse), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> UpdateCloseDayAsync([FromBody] TransferDataResource query)
        {            
            ResponseData<LogResource> response = new ResponseData<LogResource>();
            try
            {
                await _transferdataService.UpdateCloseDayAsync(query);
                //response.Data = await _transferdataService.UpdateCloseDayAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"UpdateCloseDayAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }


        /// <summary>
        /// Update Post Days
        /// </summary>
        /// <param name="resource">SalCashsaleHd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateTaxInvoice")]
        [ProducesResponseType(typeof(SalCashsaleHdResponse), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateTaxInvoiceAsync([FromBody] TransferDataResource query)
        {
            ResponseData<string> response = new ResponseData<string>();
            try
            {
                await _transferdataService.CraateTaxInvoiceAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreateTaxInvoiceAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }


        /************ Create **************/
        /// <summary>
        /// Saves a new SalCreditsaleHd.
        /// </summary>
        /// <param name="resource">SalCreditSaleHd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateCreditSale")]
        [ProducesResponseType(typeof(SalCashsaleHdResponse), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PostCreditAsync([FromBody] IEnumerable<SalCreditsaleHd> resource)
        {
            var result = await _transferdataService.SaveCreditSaleAsync(resource);

            if (!result.Success)
            {
                log.Error($"PostCreditAsync Request: {JsonConvert.SerializeObject(resource)};Error: {result.Message}");
                return BadRequest(new ErrorResource(result.Message));
            }            
            return Ok();
        }

        /// <summary>
        /// Saves a new SalCashsaleHd.
        /// </summary>
        /// <param name="resource">SalCashsaleHd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateCashSale")]
        [ProducesResponseType(typeof(SalCashsaleHdResponse), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PostCashAsync([FromBody] IEnumerable<SalCashsaleHd> resource)
        {
            var result = await _transferdataService.SaveCashSaleAsync(resource);

            if (!result.Success)
            {
                log.Error($"PostCashAsync Request: {JsonConvert.SerializeObject(resource)};Error: {result.Message}");
                return BadRequest(new ErrorResource(result.Message));
            }            
            return Ok();
        }

        /************ Get **************/
        /// <summary>
        /// Lists all  cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        //[HttpPost("GetCashSale")]
        //[ProducesResponseType(typeof(List<TransferDataResource>), 200)]
        //public async Task<IActionResult> CashSaleListAsync([FromBody] TransferDataResource query)
        //{
        //    ResponseData<List<SalCashsaleHd>> response = new ResponseData<List<SalCashsaleHd>>();
        //    try
        //    {
        //        response.Data = await _transferdataService.ListCashSaleAsync(query);
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.Message = "Success";
        //        log.Info(query);
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = StatusCodes.Status400BadRequest;
        //        response.Message = ex.Message;
        //        return BadRequest(response);
        //    }
        //}



        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        //[HttpPost("GetCreditSale")]
        //[ProducesResponseType(typeof(List<TransferDataResource>), 200)]
        //public async Task<IActionResult> CreditSaleListAsync([FromBody] TransferDataResource query)
        //{
        //    ResponseData<List<CreditSale>> response = new ResponseData<List<CreditSale>>();
        //    try
        //    {
        //        response.Data = await _transferdataService.ListCreditSaleAsync(query);
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.Message = "Success";
        //        log.Info(query);
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = StatusCodes.Status400BadRequest;
        //        response.Message = ex.Message;
        //        return BadRequest(response);
        //    }
        //}





    }
}
