using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sal.API.Controllers;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Services;
using Sale.API.Resources;
using Sale.API.Resources.Quotation;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sale.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : BaseController
    {
        private const string _strAppJson = "application/json";
        private readonly IQuotationService _quotationService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(QuotationController));
        public QuotationController(
            IQuotationService quotationService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _quotationService = quotationService;
            _mapper = mapper;
        }

        //============================== HttpPost ==============================
        [HttpPost("CreateQuotation")]
        public async Task< IActionResult> CreateQuotation([FromBody] SalQuotationHd obj)
        {
            ResponseData<SalQuotationHd> response = new ResponseData<SalQuotationHd>();
            try
            {                
                response.Data = await _quotationService.CreateQuotation(obj);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เพิ่มข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }
        [HttpGet("GetMaxCardProfile")]
        public IActionResult GetMaxCardProfile(string MaxCardID)
        {
            try
            {
                string strServiceResult = _quotationService.GetMaxCardProfile(MaxCardID);
                return Content(strServiceResult, "application/json");
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                string strStackTrace = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex.Message + Environment.NewLine + strStackTrace);
            }
        }
        
        [HttpPost("GetQuotation")]
        public async Task<IActionResult> GetQuotation([FromBody] RequestData req)
        {
            //Convert Date to TimeZone +7 Fixed ไว้ก่อน
            //req.DocDate = ((DateTime)req.DocDate).AddHours(7);

            ResponseData<SalQuotationHd> response = new ResponseData<SalQuotationHd>();
            try
            {
                response.Data = await _quotationService.GetQuotation(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }


        [HttpPost("GetQuotationList")]
        [ProducesResponseType(typeof(QueryResultResource<QuotationResource>), 200)]
        public async Task<QueryResult<QuotationResource>> GetQuotationListAsync([FromBody] QuotationHdQueryResource query)
        {
            QueryResult<QuotationResource> response = new QueryResult<QuotationResource>();
            try
            {
                var qtQuery = _mapper.Map<QuotationHdQueryResource, QuotationHdQuery>(query);

                var queryResult = await _quotationService.SearchList(qtQuery);

                response = queryResult; //  _mapper.Map<QueryResult<SalQuotationHd>, QueryResult<QuotationResource>>(queryResult);
                response.IsSuccess = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "";
                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }

        }


        //[HttpPost("GetQuotationList")]
        //[ProducesResponseType(typeof(QueryResultResource<QuotationResource>), 200)]
        //public async Task<IActionResult> GetQuotationList([FromBody] QuotationHdQueryResource req)
        //{

        //    QueryResult<SalQuotationHd> response = new QueryResult<SalQuotationHd>();
        //    try
        //    {
        //        var quotationQuery = _mapper.Map<QuotationHdQueryResource, QuotationHdQuery>(req);
        //        if (quotationQuery.FromDate != null)
        //        {
        //            quotationQuery.FromDate = ((DateTime)quotationQuery.FromDate).AddHours(7);
        //        }

        //        if (quotationQuery.ToDate != null)
        //        {
        //            quotationQuery.ToDate = ((DateTime)quotationQuery.ToDate).AddHours(7);
        //        }

        //        var queryResult = await _quotationService.GetQuotationHDList(quotationQuery);


        //        response = queryResult;  // _mapper.Map<QueryResult<SalQuotationHd>, QueryResultResource<QuotationResource>>(queryResult);
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.IsSuccess = true;
        //        response.Message = "เรียกข้อมูลสำเร็จ";
        //        //response.TotalItems = response.Data.TotalItems;        
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = StatusCodes.Status400BadRequest;
        //        response.Message = ex.Message;
        //        response.IsSuccess = false;
        //        log.Error("Error", ex);
        //        return BadRequest(response);
        //    }
        //}

        [HttpPost("GetQuotationHdRemainList")]
        public async Task<IActionResult> GetQuotationHdRemainList([FromBody] RequestData req)
        {
            //Convert Date to TimeZone +7 Fixed ไว้ก่อน
            //req.DocDate = ((DateTime)req.DocDate).AddHours(7);

            ResponseData<List<SalQuotationHd>> response = new ResponseData<List<SalQuotationHd>>();
            try
            {
                response.Data = await _quotationService.GetQuotationHdRemainList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        //Task<SysApproveStep[]> GetApproveStep(SalQuotationHd param)
        [HttpPost("GetApproveStep")]
        public async Task<IActionResult> GetApproveStep(SalQuotationHd param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName:   "GetApproveStep",
                pFunc:              async () => await _quotationService.GetApproveStep(param),
                pLog:               log
            );
        }

        //============================== HttpPut ==============================
        [HttpPut("UpdateQuotation")]
        public async Task<IActionResult> UpdateQuotation([FromBody] SalQuotationHd obj)
        {
            ResponseData<SalQuotationHd> response = new ResponseData<SalQuotationHd>();

            //Convert Date to TimeZone +7 Fixed ไว้ก่อน
            //obj.DocDate = ((DateTime)obj.DocDate).AddHours(7);
            //obj.StartDate = ((DateTime)obj.StartDate).AddHours(7);
            //obj.FinishDate = ((DateTime)obj.FinishDate).AddHours(7);
            //obj.CreatedDate = ((DateTime)obj.CreatedDate).AddHours(7);
            //obj.UpdatedDate = ((DateTime)obj.UpdatedDate).AddHours(7);

            try
            {
                response.Data = await _quotationService.UpdateQuotation(obj);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เพิ่มข้อมูลสำเร็จ";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }
        [HttpGet("GetArrayPayType")]
        public async Task<IActionResult> GetArrayPayType()
        {
            try
            {
                MasPayType[] arrPayType = null;
                arrPayType = await _quotationService.GetArrayPayType();
                string strJson = JsonConvert.SerializeObject(arrPayType);
                log.Error("GetArrayPayType Complete");
                return Content(strJson, _strAppJson);
            }
            catch (Exception ex)
            {
                string strError = string.Empty;
                strError = getErrorMessage(ex);
                log.Error("GetArrayPayType", ex);
                return BadRequest(strError);
            }
        }

        [HttpGet("GetArrayEmployee")]
        public async Task<IActionResult> GetArrayEmployee()
        {
            try
            {
                MasEmployee[] arrEmployee = null;
                arrEmployee = await _quotationService.GetArrayEmployee();
                string strJson = JsonConvert.SerializeObject(arrEmployee);                
                return Content(strJson, _strAppJson);
            }
            catch (Exception ex)
            {
                string strError = string.Empty;
                strError = getErrorMessage(ex);
                log.Error("GetArrayEmployee", ex);
                return BadRequest(strError);
            }
        }
        //public async Task<MasEmployee> GetEmployee(string pStrEmpCode)
        [HttpGet("GetEmployee/{pStrEmpCode}")]
        public async Task<IActionResult> GetEmployee(string pStrEmpCode)
        {
            return await doAction("GetEmployee", async () => await _quotationService.GetEmployee(pStrEmpCode));
        }
        #region - Private Function - 
        private async Task<IActionResult> doAction<T>(string pStrFunctionName, Func<Task<T>> pFunc)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                T result;
                result = await pFunc();
                sw.Stop();                
                return jsonResult(result);
            }
            catch (Exception ex)
            {
                log.Error(pStrFunctionName, ex);
                return exeptionResult(ex);
            }
        }
        private ContentResult jsonResult(object pInput)
        {
            string strJson = string.Empty;

            strJson = JsonConvert.SerializeObject(pInput);
            ContentResult result = null;
            result = Content(strJson, _strAppJson);
            return result;
        }
        private string getErrorMessage(Exception pException)
        {
            if (pException == null)
            {
                return string.Empty;
            }
            string result = string.Empty;
            result = pException.StackTrace;
            while (pException.InnerException != null)
            {
                pException = pException.InnerException;
            }
            result = pException.Message + Environment.NewLine + result;
            return result;
        }
        private BadRequestObjectResult exeptionResult(Exception pException)
        {
            string strErrorMessage = string.Empty;
            strErrorMessage = getErrorMessage(pException);
            return BadRequest(strErrorMessage);
        }
        #endregion
        [HttpGet("GetApproverStep/{pIntStepQty}/{pStrEmpCode}")]
        public async Task<IActionResult> GetApproverStep(int pIntStepQty ,  string pStrEmpCode)
        {
            DefaultService.GetApproverStepParam param = new DefaultService.GetApproverStepParam()
            {
                empidRequest = pStrEmpCode,
                stepQty = pIntStepQty
            };
            return await doAction("" , async()=> await DefaultService.GetApproverStep(param));
        }
    }
}
