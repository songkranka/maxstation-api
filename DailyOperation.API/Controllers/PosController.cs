using AutoMapper;
using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Models.Request;
using DailyOperation.API.Domain.Models.Response;
using DailyOperation.API.Domain.Services;
using DailyOperation.API.Resources;
using DailyOperation.API.Resources.POS;
using DailyOperation.API.Services;
using log4net;
using MaxStation.Entities.Models;
using MaxStation.Utility.Caches;
using Microsoft.AspNetCore.Authorization;
//using MaxStation.Utility.Caches;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PosController : BaseController
    {
        private readonly IPosService _posService;
        private readonly IMapper _mapper;
        private readonly ICommonCacheHelper _cacheHelper;
        private static readonly ILog log = LogManager.GetLogger(typeof(PosController));

        public PosController(
            IPosService posService,
            IMapper mapper,
            ICommonCacheHelper cacheHelper,
            PTMaxstationContext context) : base(context)
        {
            _posService = posService;
            _mapper = mapper;
            _cacheHelper = cacheHelper;
        }


        /// <summary>
        /// Lists all existing cash.
        /// </summary>
        /// <returns>List of cash.</returns>
        //[HttpGet("GetConn")]
        //[ProducesResponseType(typeof(QueryResultResource<MasControl>), 200)]
        //public ActionResult<string> GetConn()
        //{
        //    return _posService.GetConn();
        //}



        /// <summary>
        /// Lists all existing cash.
        /// </summary>
        /// <returns>List of cash.</returns>
        [HttpPost("GetCashList")]
        [ProducesResponseType(typeof(QueryResultResource<POSCash>), 200)]
        public IActionResult CashList([FromBody] CashQueryResource query)
        {
            ResponseData<QueryResult<POSCash>> response = new ResponseData<QueryResult<POSCash>>();

            try
            {
                var cacheKey = _cacheHelper.GetRequestName($"GetCashList_{query.CompCode}_{query.BrnCode}_{query.FromDate:ddMMyyyy}");
                response.Data = _cacheHelper.GetAsync<QueryResult<POSCash>>(cacheKey).Result;

                if (response.Data == null)
                {
                    var result = _posService.CashList(query);
                    _cacheHelper.Create(cacheKey, result);
                    response.Data = result;
                }

                //response.Data = _posService.CashList(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"GetCashList : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            //return null;
        }

        /// <summary>
        /// Lists all existing credit.
        /// </summary>
        /// <returns>List of credit.</returns>
        [HttpPost("GetCreditList")]
        [ProducesResponseType(typeof(QueryResultResource<POSCredit>), 200)]
        public IActionResult CreditList([FromBody] CreditQueryResource query)
        {
            ResponseData<QueryResult<POSCredit>> response = new ResponseData<QueryResult<POSCredit>>();
            try
            {
                var cacheKey = _cacheHelper.GetRequestName($"GetCreditList_{query.CompCode}_{query.BrnCode}_{query.FromDate:ddMMyyyy}");
                response.Data = _cacheHelper.GetAsync<QueryResult<POSCredit>>(cacheKey).Result;

                if (response.Data == null)
                {
                    var result = _posService.CreditList(query);
                    _cacheHelper.Create(cacheKey, result);
                    response.Data = result;
                }

                //response.Data = _posService.CreditList(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"GetCreditList : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }

        }

        /// <summary>
        /// Lists all existing credit.
        /// </summary>
        /// <returns>List of credit.</returns>
        [HttpPost("GetWithdrawList")]
        [ProducesResponseType(typeof(QueryResultResource<POSWithdraw>), 200)]
        public IActionResult WithdrawList([FromBody] WithdrawQueryResource query)
        {
            ResponseData<QueryResult<POSWithdraw>> response = new ResponseData<QueryResult<POSWithdraw>>();
            try
            {
                var cacheKey = _cacheHelper.GetRequestName($"GetWithdrawList_{query.CompCode}_{query.BrnCode}_{query.FromDate:ddMMyyyy}");
                response.Data = _cacheHelper.GetAsync<QueryResult<POSWithdraw>>(cacheKey).Result;

                if (response.Data == null)
                {
                    var result = _posService.WithdrawList(query);
                    _cacheHelper.Create(cacheKey, result);
                    response.Data = result;
                }

                //response.Data = _posService.WithdrawList(query);
                response.TotalItems = response.Data.Items.Count();
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"GetWithdrawList : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Lists all existing receive.
        /// </summary>
        /// <returns>List of rceive.</returns>
        [HttpPost("GetReceiveList")]
        [ProducesResponseType(typeof(QueryResultResource<POSReceive>), 200)]
        public IActionResult ReceiveList([FromBody] ReceiveQueryResource query)
        {
            ResponseData<QueryResult<POSReceive>> response = new ResponseData<QueryResult<POSReceive>>();

            try
            {
                var cacheKey = _cacheHelper.GetRequestName($"GetReceiveList_{query.CompCode}_{query.BrnCode}_{query.FromDate:ddMMyyyy}");
                response.Data = _cacheHelper.GetAsync<QueryResult<POSReceive>>(cacheKey).Result;

                if (response.Data == null)
                {
                    var result = _posService.ReceiveList(query);
                    _cacheHelper.Create(cacheKey, result);

                    response.Data = result;
                }

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"GetReceiveList : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            //return null;
        }

        [HttpPost("SaveCashSale")]
        public async Task<IActionResult> SaveCashSale([FromBody] SaveCashSaleResource request)
        {
            ResponseData<SaveCashSaleResource> response = new ResponseData<SaveCashSaleResource>();
            try
            {
                var result = await _posService.SaveCashSaleAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"SaveCashSale : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("SaveCreditSale")]
        public async Task<IActionResult> SaveCreditSale([FromBody] SaveCreditSaleResource request)
        {
            ResponseData<SaveCreditSaleResource> response = new ResponseData<SaveCreditSaleResource>();
            try
            {
                var result = await _posService.SaveCreditSaleAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"SaveCreditSale : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("SaveWithdraw")]
        public async Task<IActionResult> SaveWithdraw([FromBody] SaveWithdrawResource request)
        {
            ResponseData<SaveWithdrawResource> response = new ResponseData<SaveWithdrawResource>();
            try
            {
                var result = await _posService.SaveWithdrawAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"SaveWithdraw : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("SaveReceive")]
        public async Task<IActionResult> SaveReceive([FromBody] SaveReceiveResource request)
        {
            ResponseData<SaveReceiveResource> response = new ResponseData<SaveReceiveResource>();
            try
            {
                var result = await _posService.SaveReceiveAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"SaveReceive : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("GetStatus")]
        public IActionResult GetWithdrawStatus([FromBody] WithdrawStatusRequest req)
        {
            ResponseData<DopPosConfig> response = new ResponseData<DopPosConfig>();
            try
            {
                response.Data = _posService.GetWithdrawStatus(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"GetStatus : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("GetCreditSummaryByBranch")]
        public IActionResult GetCreditSummaryByBranch([FromBody] CreditSummaryRequest req)
        {
            ResponseData<CreditSummaryResponse> response = new ResponseData<CreditSummaryResponse>();
            try
            {
                response.Data = _posService.GetCreditSummaryByBranch(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"GetCreditSummaryByBranch : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("GetDopPosConfig")]
        public async Task< IActionResult> GetDopPosConfig([FromBody] GetDopPosConfigParam param)
        {
            return await DefaultService.DoActionAsync("GetDopPosConfig", async () => await _posService.GetDopPosConfig(param), log);
        }

        [HttpPost("GetPeriodCount")]
        public async Task<IActionResult> GetPeriodCount([FromBody] PeriodCountRequest req)
        {
            ResponseData<PeriodCountResponse> response = new ResponseData<PeriodCountResponse>();
            try
            {
                var cacheKey = _cacheHelper.GetRequestName($"GetPeriodCount_{req.BrnCode}_{req.FromDate:ddMMyyyy}");
                response.Data = await _cacheHelper.GetAsync<PeriodCountResponse>(cacheKey);

                if (response.Data == null)
                {
                    var result = await _posService.GetPeriodCount(req);
                    await _cacheHelper.CreateAsync(cacheKey, result);
                    response.Data = result;
                }

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"GetPeriodCount : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



        [HttpPost("CheckPeriodWater")]
        public async Task<IActionResult> CheckPeriodWater([FromBody] PeriodCountRequest req)
        {
            CheckPeriodWaterResponse response = new CheckPeriodWaterResponse();
            try
            {
                var cacheKey = _cacheHelper.GetRequestName($"GetPeriodWater_{req.BrnCode}_{req.FromDate:ddMMyyyy}");
                response = await _cacheHelper.GetAsync<CheckPeriodWaterResponse>(cacheKey);
               
                if (response == null)
                {
                    var result = await _posService.CheckPeriodWater(req);
                    await _cacheHelper.CreateAsync(cacheKey, result);
                    response = result;
                }
                return Ok(JsonConvert.SerializeObject(response.result));
            }
            catch (Exception ex)
            {
                return BadRequest(response.result);
            }
        }

        //[HttpPost("SaveWithdraw")]
        //public async Task<IActionResult> SaveWithdraw2([FromBody] SaveWithdrawResource request)
        //{
        //    ResponseData<SaveWithdrawResource> response = new ResponseData<SaveWithdrawResource>();
        //    try
        //    {
        //        var result = await _posService.SaveWithdrawAsync(request);

        //        if (!result.Success)
        //        {
        //            return BadRequest(new ErrorResource(result.Message));
        //        }

        //        response.Data = result.Resource;
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.Message = "บันทึกสำเร็จ";
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
