using AutoMapper;
using Finance.API.Domain.Models;
using Finance.API.Domain.Models.Queries;
using Finance.API.Domain.Services;
using Finance.API.Resources;
using Finance.API.Resources.Recive;
using Finance.API.Services;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sale.API.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiveController : BaseController
    {
        private const string _appJson = "application/json";

        private readonly IReceiveService _receiveService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReceiveController));

        public ReceiveController(IReceiveService receiveService,IMapper mapper) : base()
        {
            _receiveService = receiveService;
            _mapper = mapper;
        }


        [HttpPost("GetReceiveHdList")]
        [ProducesResponseType(typeof(QueryResult<FinReceiveHd>), 200)]
        public async Task<IActionResult> ReceiveHdListAsync([FromBody] ReceiveHdQueryResource query)
        {
            try
            {
                var receiveQuery = _mapper.Map<ReceiveHdQueryResource, ReceiveHdQuery>(query);
                var queryResult = await _receiveService.ListAsync(receiveQuery);
                return Ok(queryResult);
            }
            catch (Exception ex)
            {
                string strErrorMessage = string.Empty;
                strErrorMessage = _receiveService.GetErrorMessage(ex);
                return BadRequest(strErrorMessage);
            }
        }


        /// <summary>
        /// Lists all existing receivehd.
        /// </summary>
        /// <returns>List of receivehd.</returns>
        [HttpPost("Receive")]
        public async Task<IActionResult> Receive([FromBody] ReceiveHdQueryResource query)
        {
            ResponseData<FinReceiveHd> response = new ResponseData<FinReceiveHd>();

            try
            {
                var receiveQuery = _mapper.Map<ReceiveHdQueryResource, ReceiveHdQuery>(query);
                response.Data = await _receiveService.FindByIdAsync(receiveQuery);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                log.Info(query);
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error(response.Message);
                return BadRequest(response);
            }
        }

        [HttpPost("CreateReceive")]
        [ProducesResponseType(typeof(ReceiveHdResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PostAsync([FromBody] FinReceiveHd obj)
        {

            ResponseData<FinReceiveHd> response = new ResponseData<FinReceiveHd>();
            try
            {
                if(obj == null)
                {
                    return BadRequest("Input is Null");
                }
                if ("รับชำระ".Equals(obj.ReceiveType) && (obj.FinReceivePay == null || !obj.FinReceivePay.Any()))
                {
                    return BadRequest("กรุณาเลือกรายการตัดหนี้");
                }
                
                obj.CreatedDate = DateTime.Now;
                var result = await _receiveService.SaveAsync(obj);
                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error(response.Message);
                return BadRequest(response);
            }
        }

        [HttpPut("UpdateReceive")]
        [ProducesResponseType(typeof(ReceiveHdResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PutAsync([FromBody] FinReceiveHd obj)
        {
            ResponseData<FinReceiveHd> response = new ResponseData<FinReceiveHd>();
            try
            {
                if (obj == null)
                {
                    return BadRequest("Input is Null");
                }
                if ( "รับชำระ".Equals( obj.ReceiveType) && ( obj.FinReceivePay == null || !obj.FinReceivePay.Any()))
                {
                    return BadRequest("กรุณาเลือกรายการตัดหนี้");
                }

                obj.UpdatedDate = DateTime.Now;
                var result = await _receiveService.UpdateAsync(obj);
                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error(response.Message);
                return BadRequest(response);
            }
        }

        [HttpPost("GetRemainFinBalanceList")]
        public async Task<IActionResult> GetRemainFinBalanceList([FromBody] ReceiveQuery query)
        {
            ResponseData<List<FinReceivePay>> response = new ResponseData<List<FinReceivePay>>();
            try
            {
                response.Data = await _receiveService.GetRemainFinBalanceList(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                log.Info(query);
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error(response.Message);
                return BadRequest(response);
            }
        }

        [HttpPost("GetFinReceivePays")]
        public async Task<IActionResult> GetFinReceivePays(FinReceiveHd pInput)
        {
            try
            {
                FinReceivePay[] arrFinReceivePays = null;
                arrFinReceivePays = await _receiveService.GetFinReceivePays(pInput);
                string strJson = string.Empty;
                strJson = JsonConvert.SerializeObject(arrFinReceivePays);
                log.Info("GetFinReceivePays");
                return Content(strJson, _appJson);
            }
            catch (Exception ex)
            {
                string strError = _receiveService.GetErrorMessage(ex);
                log.Error("GetFinReceivePays", ex);
                return BadRequest(strError);
            }
        }

        [HttpGet("GetMasMapping")]
        public async Task<IActionResult> GetMasMapping()
        {
            return await DefaultService.DoActionAsync("GetMasMapping", async () => await _receiveService.GetMasMapping(), log);
        }

        [HttpGet("SumReceivePay/{pStrComCode}/{pStrBrnCode}/{pDatDocDate}")]
        public async Task<IActionResult> SumReceivePay(string pStrComCode, string pStrBrnCode, DateTime pDatDocDate)
        {
            ModelSumRecivePayResult2 result = new ModelSumRecivePayResult2();
            result.ItemCount = 0;
            result.Status = "200";
            result.Message = string.Empty;
            try
            {
                result.Data = await _receiveService.SumReceivePay(pStrComCode, pStrBrnCode, pDatDocDate);
                if(result.Data != null)
                {
                    result.ItemCount = result.Data.Length;
                }
                
            }
            catch (Exception ex)
            {
                result.Status = "400";
                result.Message = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            string strJson = JsonConvert.SerializeObject(result);
            ContentResult result2 = null;
            result2 = new ContentResult()
            {
                Content = strJson,
                ContentType = "application/json"
            };
            return result2;
            //return await DefaultService.DoActionAsync(
            //    "SumReceivePay", 
            //    async () => await _receiveService.SumReceivePay(pStrComCode, pStrBrnCode, pDatDocDate), 
            //    log
            //);
        }
    }
}
