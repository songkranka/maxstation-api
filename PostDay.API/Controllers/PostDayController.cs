using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using MaxStation.Utility.Caches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nancy.Helpers;
using Newtonsoft.Json;
using PostDay.API.Domain.Models;
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Services;
using PostDay.API.Resources;
using PostDay.API.Resources.PostDay;
using PostDay.API.Services;
using System;
using System.Threading.Tasks;

namespace PostDay.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostDayController : BaseController
    {
        private readonly IPostDayService _postdayService;
        private readonly ICloseDayService _closedayService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(PostDayController));
        private readonly ICommonCacheHelper _cacheHelper;

        public PostDayController(IPostDayService postdayService, ICloseDayService closedayService, IMapper mapper, ICommonCacheHelper cacheHelper, PTMaxstationContext context) : base(context)
        {
            _postdayService = postdayService;
            _closedayService = closedayService;
            _mapper = mapper;
            _cacheHelper = cacheHelper;
        }

        [HttpPost("SaveDocument")]
        [ProducesResponseType(typeof(SaveDocumentResponse), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> SaveDocument([FromBody] SaveDocumentRequest req)
        {
            ResponseData<SaveDocumentRequest> response = new ResponseData<SaveDocumentRequest>();

            try
            {
                var result = await _closedayService.SaveDocument(req);

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
                log.Error("SaveDocument", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("CreateTaxInvoice")]
        [ProducesResponseType(typeof(SaveDocumentResponse), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateTaxInvoice([FromBody] PostDayResource req)
        {
            SaveDocumentResponse response = new SaveDocumentResponse();
            try
            {
                response = await _postdayService.CreateTaxInvoice(req);
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreateTaxInvoice Request: {JsonConvert.SerializeObject(req)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Status = "Fail";
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }


        [HttpPost("GetDocument"), AllowAnonymous]
        [ProducesResponseType(typeof(DopPeriodMeter), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> GetDocument([FromBody] GetDocumentRequest req)
        {
            ResponseData<GetDocumentResponse> response = new ResponseData<GetDocumentResponse>();
            try
            {
                var cacheKey = _cacheHelper.GetRequestName($"GetPostDay_{req.CompCode}_{req.BrnCode}_{req.DocDate:ddMMyyyy}");
                response.Data = _cacheHelper.GetAsync<GetDocumentResponse>(cacheKey).Result;

                if (response.Data == null)
                {
                    var result = await _closedayService.GetDocument(req);
                    _cacheHelper.Create(cacheKey, result.Resource);
                    response.Data = result.Resource;
                }

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("GetDocument", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("AddStock")]
        public async Task<IActionResult> AddStock(AddStockParam req)
        {
            return await DefaultService.DoActionAsync("AddStock", async () => await _postdayService.AddStock(req), log);
        }

        [HttpGet("TestSelectDate")]
        public async Task<IActionResult> TestSelectDate()
        {
            var result = await _postdayService.TestSelectDate();
            return Ok(result);
        }

        [HttpGet("TestSelectDate2")]
        public async Task<IActionResult> TestSelectDate2()
        {
            var result = await _postdayService.TestSelectDate2();
            return Ok(result);
        }

        [HttpPost("AddStockMonthly")]
        public async Task<IActionResult> AddStockMonthly(AddStockMonthlyParam req)
        {
            return await DefaultService.DoActionAsync("AddStockMonthly", async () => await _postdayService.AddStockMonthly(req), log);
        }

        [HttpPost("GetDopValidData")]
        public async Task<IActionResult> GetDopValidData(GetDopValidDataParam req)
        {
            return await DefaultService.DoActionAsync("GetDopValidData", async () => await _postdayService.GetDopValidData(req), log);
        }
    }
}