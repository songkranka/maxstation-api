using AutoMapper;
using Sale.API.Services;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sal.API.Controllers;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Services;
using Sale.API.Domain.Services.Communication;
using Sale.API.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Sale.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CreditSaleController : BaseController
    {
        private readonly ICreditSaleService _cresitSaleService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(QuotationController));
        private readonly IServiceScopeFactory serviceScopeFactory;
        public CreditSaleController(
            ICreditSaleService cresitSaleService,
            IMapper mapper,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory) : base(context)
        {
            _cresitSaleService = cresitSaleService;
            _mapper = mapper;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        [HttpPost("CreateCreditSale")]
        public async Task<IActionResult> CreateCreditSale([FromBody] SalCreditsaleHd req)
        {
            //Convert Date to TimeZone +7 Fixed ไว้ก่อน
            req.DocDate = ((DateTime)req.DocDate).AddHours(7);
            //req.CreatedDate = ((DateTime)req.CreatedDate).AddHours(7);
            req.UpdatedDate = ((DateTime)req.UpdatedDate).AddHours(7);

            ResponseData<SalCreditsaleHd> response = new ResponseData<SalCreditsaleHd>();
            ResponseService<SalCreditsaleHd> resp = await _cresitSaleService.SaveAsync(req, serviceScopeFactory);
            if (resp.IsSuccess)
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = resp.Message;
                response.Data = resp.Data;                
                return Ok(JsonConvert.SerializeObject(response));
            }
            else {
                log.Error($"CreateCreditSale Request: {JsonConvert.SerializeObject(req)};Error: {resp.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = resp.Message;                
                return BadRequest(response);
            }
        }

        [HttpPost("CreateCreditSaleList")]
        public async Task<IActionResult> CreateCreditSaleList([FromBody] List<SalCreditsaleHd> req)
        {
            ResponseData<List<SalCreditsaleHd>> response = new ResponseData<List<SalCreditsaleHd>>();
            ResponseService<List<SalCreditsaleHd>> resp = await _cresitSaleService.SaveListAsync(req, serviceScopeFactory);
            if (resp.IsSuccess)
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = resp.Message;
                response.Data = resp.Data;                
                return Ok(JsonConvert.SerializeObject(response));
            }
            else
            {
                log.Error($"CreateCreditSale Request: {JsonConvert.SerializeObject(req)};Error: {resp.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = resp.Message;                
                return BadRequest(response);
            }
        }

        [HttpPost("GetCreditSale")]
        public async Task<IActionResult> GetCreditSale([FromBody] RequestData req)
        {
            ResponseData<SalCreditsaleHd> response = new ResponseData<SalCreditsaleHd>();
            try
            {
                response.Data = await _cresitSaleService.FindByIdAsync(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"GetCreditSale Request: {JsonConvert.SerializeObject(req)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;                
                return BadRequest(response);
            }
        }

        [HttpPost("GetCreditSaleList")]
        public async Task<IActionResult> GetCreditSaleList([FromBody] RequestData req)
        {
            //Convert Date to TimeZone +7 Fixed ไว้ก่อน
            //req.DocDate = ((DateTime)req.DocDate).AddHours(7);
            //if (req.FromDate != null)
            //{
            //    req.FromDate = ((DateTime)req.FromDate).AddHours(7);
            //}
            //if (req.ToDate != null)
            //{
            //    req.ToDate = ((DateTime)req.ToDate).AddHours(7);
            //}

            ResponseData<List<SalCreditsaleHd>> response = new ResponseData<List<SalCreditsaleHd>>();
            try
            {
                QueryResult<SalCreditsaleHd> resp = await _cresitSaleService.ListAsync(req);
                response.Data = resp.Items;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                response.TotalItems = resp.TotalItems;                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"GetCreditSaleList Request: {JsonConvert.SerializeObject(req)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                response.TotalItems = 0;                
                return BadRequest(response);
            }
        }

        [HttpPost("GetProductListWithOutMaterialCode")]
        public IActionResult GetProductListWithOutMaterialCode(ProductRequest req)
        {
            return DefaultService.DoAction("GetProductListWithOutMaterialCode", () => _cresitSaleService.GetProductListWithOutMaterialCode(req),log);
        }

        //============================== HttpPut ==============================
        [HttpPut("UpdateCreditSale")]
        public async Task<IActionResult> UpdateCreditSale([FromBody] SalCreditsaleHd obj)
        {
            //Convert Date to TimeZone +7 Fixed ไว้ก่อน
            obj.DocDate = ((DateTime)obj.DocDate).AddHours(7);
            obj.CreatedDate = ((DateTime)obj.CreatedDate).AddHours(7);
            obj.UpdatedDate = ((DateTime)obj.UpdatedDate).AddHours(7);

            ResponseData<SalCreditsaleHd> response = new ResponseData<SalCreditsaleHd>();
            ResponseService<SalCreditsaleHd> resp = await _cresitSaleService.UpdateAsync(obj, serviceScopeFactory);
            if (resp.IsSuccess)
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = resp.Message;                
                return Ok(JsonConvert.SerializeObject(response));
            }
            else
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = resp.Message;
                log.Error(resp.Message);
                return BadRequest(response);
            }
        }
        [HttpGet("GetCustomerCar")]
        public async Task<IActionResult> GetCustomerCar(string pStrCusCode)
        {
            var result = new ApiResponse<List<MasCustomerCar>>();
            try
            {
                List<MasCustomerCar> listCar = await _cresitSaleService.GetCustomerCar(pStrCusCode);
                result.SetResult(listCar);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }
            //List<MasCustomerCar>
            return Ok( result);
        }
        [HttpGet("GetCompCar/{pStrCustcode}")]
        public async Task<IActionResult> GetCompCar(string pStrCustcode)
        {
            return await DefaultService.DoActionAsync(
                "GetCompCar", 
                async () => await _cresitSaleService.GetCompCar(pStrCustcode), 
                log
            );
        }
    }
}
