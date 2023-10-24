using AutoMapper;
using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Meter;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Services;
using DailyOperation.API.Resources;
using DailyOperation.API.Resources.POS;
using MaxStation.Entities.Models;
using MaxStation.Utility.Caches;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;

namespace DailyOperation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadTestController : BaseController
    {
        private readonly IPosService _posService;
        private readonly IMapper _mapper;
        private readonly ICommonCacheHelper _cacheHelper;

        public LoadTestController(
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
        [HttpPost("PostLog")]
        [ProducesResponseType(typeof(QueryResultResource<POSCash>), 200)]
        public IActionResult PostLog([FromBody] BranchMeterRequest req)
        {
            ResponseData<QueryResult<POSCash>> response = new ResponseData<QueryResult<POSCash>>();

            try
            {
                throw new Exception("hello world");
                //return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                //log.Error($"CashList : {ex.StackTrace}");
                Console.WriteLine($"[{HttpContext.Request.Path.Value}] [{req.BrnCode}] : {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            //return null;
        }


        /// <summary>
        /// Lists all existing cash.
        /// </summary>
        /// <returns>List of cash.</returns>
        //[HttpPost("GetCashListWithCache")]
        //[ProducesResponseType(typeof(QueryResultResource<POSCash>), 200)]
        //public IActionResult CashListWithCache([FromBody] CashQueryResource query)
        //{
        //    ResponseData<QueryResult<POSCash>> response = new ResponseData<QueryResult<POSCash>>();

        //    try
        //    {
        //        var cacheKey = _cacheHelper.GetRequestName($"GetCashList_{query.CompCode}_{query.BrnCode}_{query.FromDate:ddMMyyyy}");
        //        response.Data = _cacheHelper.GetAsync<QueryResult<POSCash>>(cacheKey).Result;

        //        if (response.Data == null)
        //        {
        //            var result = _posService.CashList(query);
        //            _cacheHelper.Create(cacheKey, result);
        //            response.Data = result;
        //        }

        //        //response.Data = _posService.CashList(query);
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.Message = "Success";
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        //log.Error($"CashList : {ex.StackTrace}");
        //        response.StatusCode = StatusCodes.Status400BadRequest;
        //        response.Message = ex.Message;
        //        return BadRequest(response);
        //    }

        //    //return null;
        //}




    }
}
