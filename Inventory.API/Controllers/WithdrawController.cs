using AutoMapper;
using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services;
using Inventory.API.Resources;
using Inventory.API.Resources.Withdraw;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : BaseController
    {
        private readonly IWithdrawService _withdrawService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(RequestController));

        private const string _appJson = "application/json";
        public WithdrawController(IWithdrawService withdrawService,IMapper mapper,PTMaxstationContext context) : base(context)
        {
            _withdrawService = withdrawService;
            _mapper = mapper;
        }



        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpGet("GetWithdraw/{guid}/{compcode}/{brnCode}/{locCode}")]
        public async Task<IActionResult> GetWithdraw(Guid guid, string compcode, string brnCode, string locCode)
        {
            ResponseData<InvWithdrawHd> response = new ResponseData<InvWithdrawHd>();

            try
            {
                //var cashSaleQuery = _mapper.Map<WithdrawQueryResource, WithdrawQuery>(query);
                response.Data = await _withdrawService.FindByIdAsync(guid, compcode, brnCode, locCode);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("GetWithdraw", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Lists all existing withdrawhd.
        /// </summary>
        /// <returns>List of withdrawhd.</returns>
        //[HttpPost("GetWithdrawList")]
        //[ProducesResponseType(typeof(QueryResultResource<WithdrawResource>), 200)]
        //public async Task<QueryResultResource<WithdrawResource>> GetWithdrawListAsync([FromBody] WithdrawQueryResource query)
        //{

        //



        //    var withdrawQuery = _mapper.Map<WithdrawQueryResource, WithdrawQuery>(query);

        //    var queryResult = await _withdrawService.ListAsync(withdrawQuery);
        //    var response = _mapper.Map<QueryResult<InvWithdrawHd>, QueryResultResource<WithdrawResource>>(queryResult);
        //    return response;

        //}


        [HttpPost("GetWithdrawList")]
        [ProducesResponseType(typeof(QueryResultResource<WithdrawResource>), 200)]
        public async Task<QueryResultResource<WithdrawResource>> GetWithdrawListAsync([FromBody] WithdrawQueryResource query)
        {
            QueryResultResource<WithdrawResource> response = new QueryResultResource<WithdrawResource>();
            try
            {
                //var test = $"FromDate = {query.FromDate}; ToDate = {query.ToDate}";
                //throw new Exception(test);

                var withdrawQuery = _mapper.Map<WithdrawQueryResource, WithdrawQuery>(query);

                var queryResult = await _withdrawService.ListAsync(withdrawQuery);
                response = _mapper.Map<QueryResult<InvWithdrawHd>, QueryResultResource<WithdrawResource>>(queryResult);
                response.IsSuccess = true;
                response.Message = "";
                return response;
            }
            catch (Exception ex)
            {
                _log.Error("GetWithdrawList", ex);
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }

        }


        /// <summary>
        /// Saves a new cashsalehd.
        /// </summary>
        /// <param name="resource">cashsalehd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateWithdraw")]
        [ProducesResponseType(typeof(WithdrawResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateWithdrawAsync([FromBody] InvWithdrawHd model)
        {
            ResponseData<WithdrawResource> response = new ResponseData<WithdrawResource>();
            try
            {
                //var test = $"DocDate = {model.DocDate}; CreatedDate = {model.CreatedDate}";
                //throw new Exception(test);

               // var cashSale = _mapper.Map<InvWithdrawHd, CashSale>(resource);
                var result = await _withdrawService.CreateAsync(model);

                if (!result.Success)
                {
                    _log.Error("CreateWithdraw : " + result.Message);
                    return BadRequest(new ErrorResource(result.Message));
                }

                var withdrawResource = _mapper.Map<InvWithdrawHd, WithdrawResource>(result.Resource);
                response.Data = withdrawResource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //_log.Error("CreateWithdraw", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



        /// <summary>
        /// Updates an existing cashsalehd according to an identifier.
        /// </summary>
        /// <param name="id">Cashsalehd identifier.</param>
        /// <param name="resource">Updated cashsalehd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPut("UpdateWithdraw/{guid}")]
        [ProducesResponseType(typeof(WithdrawResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> UpdateWithdrawAsync(Guid guid, [FromBody] InvWithdrawHd resource)
        {
            var result = await _withdrawService.UpdateAsync(guid, resource);
            if (!result.Success)
            {
                // _log.Error("UpdateWithdraw : " + result.Message);
                return BadRequest(new ErrorResource(result.Message));
            }

            var cashSaleHdResource = _mapper.Map<InvWithdrawHd, WithdrawResource>(result.Resource);
            return Ok(cashSaleHdResource);
        }

        [HttpPost("CancelWithdraw/{guid}")]
        [ProducesResponseType(typeof(WithdrawResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CancelWithdraw(Guid guid, [FromBody] InvWithdrawHd resource)
        {
            var result = await _withdrawService.CancelAsync(guid, resource);
            if (!result.Success)
            {
                // _log.Error("UpdateWithdraw : " + result.Message);
                return BadRequest(new ErrorResource(result.Message));
            }

            var cashSaleHdResource = _mapper.Map<InvWithdrawHd, WithdrawResource>(result.Resource);
            return Ok(cashSaleHdResource);
        }

        [HttpGet("GetReasons")]
        public async Task<IActionResult> GetReasons()
        {
            try
            {
                MasReason[] arrReason = null;
                arrReason = await _withdrawService.GetReasons();
                string strJson = null;
                strJson = JsonConvert.SerializeObject(arrReason);
                ContentResult result = null;
                result = Content(strJson, _appJson);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error("GetReasons", ex);
                string strErrorMessage = getErrorMessage(ex);
                return BadRequest(strErrorMessage);
            }
        }

        [HttpGet("GetReasonGroups/{pStrReasonId}")]
        public async Task<IActionResult> GetReasonGroups(string pStrReasonId)
        {
            try
            {
                MasReasonGroup[] arrReasonGroup = null;
                arrReasonGroup = await _withdrawService.GetReasonGroups(pStrReasonId);
                string strJson = null;
                strJson = JsonConvert.SerializeObject(arrReasonGroup);
                ContentResult result = null;
                result = Content(strJson, _appJson);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error("GetProducts", ex);
                string strErrorMessage = getErrorMessage(ex);
                return BadRequest(strErrorMessage);
            }
        }

        private string getErrorMessage(Exception pException)
        {
            if (pException == null)
            {
                return string.Empty;
            }
            string strStack = pException.StackTrace;
            while (pException.InnerException != null) pException = pException.InnerException;
            return pException.Message + Environment.NewLine + strStack;
        }
    }
}
