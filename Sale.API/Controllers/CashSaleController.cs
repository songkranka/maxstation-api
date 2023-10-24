using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sal.API.Controllers;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Resources;
using System.Threading.Tasks;
using Sale.API.Domain.Services;
using MaxStation.Entities.Models;
using Sale.API.Resources.CashSale;
using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using log4net;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Sale.API.Services;

namespace Sale.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CashSaleController : BaseController
    {
        private readonly ICashSaleService _cashSaleService;
        private readonly IMapper _mapper;
        private const string _appJson = "application/json";
        private static readonly ILog log = LogManager.GetLogger(typeof(CashSaleController));
        public CashSaleController(
            ICashSaleService cashSaleService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _cashSaleService = cashSaleService;
            _mapper = mapper;
        }


        [HttpPost("GetCashSaleHdList")]
        [ProducesResponseType(typeof(QueryResult<SalCashsaleHd>), 200)]
        public async Task<IActionResult> CashSaleHdListAsync([FromBody] CashSaleHdQueryResource query)
        {
            return await DefaultService.DoActionAsync("GetCashSaleHdList", async () =>
           {
               
               var cashSaleQuery = _mapper.Map<CashSaleHdQueryResource, CashSaleHdQuery>(query);
               var queryResult = await _cashSaleService.ListAsync(cashSaleQuery);
               //var resource = _mapper.Map<QueryResult<SalCashsaleHd>, QueryResultResource<CashSaleHdResource>>(queryResult);
               return queryResult;
           }, log);
        }

        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("GetCashSaleHdListOld")]
        [ProducesResponseType(typeof(QueryResult<SalCashsaleHd>), 200)]
        public async Task<QueryResult<SalCashsaleHd>> CashSaleHdListAsyncOld([FromBody] CashSaleHdQueryResource query)
        {
            try
            {                
                var cashSaleQuery = _mapper.Map<CashSaleHdQueryResource, CashSaleHdQuery>(query);

                var queryResult = await _cashSaleService.ListAsync(cashSaleQuery);
                //var resource = _mapper.Map<QueryResult<SalCashsaleHd>, QueryResultResource<CashSaleHdResource>>(queryResult);
                return queryResult;
            }catch(Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"CashSaleHdListAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                return null;
            }

        }

        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("GetCashSaleActive")]
        [ProducesResponseType(typeof(QueryResultResource<CashSaleHdResource>), 200)]
        public async Task<QueryResultResource<CashSaleHdResource>> GetCashSaleActive([FromBody] CashSaleHdQueryResource query)
        {
            try
            {
                var cashSaleQuery = _mapper.Map<CashSaleHdQueryResource, CashSaleHdQuery>(query);

                var queryResult = await _cashSaleService.ListActiveAsync(cashSaleQuery);
                var resource = _mapper.Map<QueryResult<SalCashsaleHd>, QueryResultResource<CashSaleHdResource>>(queryResult);
                return resource;
            }catch(Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"CashSaleHdListAsync Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                return null;
            }
           
        }

        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("CashSale")]
        public async Task<IActionResult> CashSale([FromBody] CashSaleHdQueryResource query)
        {
            ResponseData<CashSale> response = new ResponseData<CashSale>();

            try
            {
                var cashSaleQuery = _mapper.Map<CashSaleHdQueryResource, CashSaleHdQuery>(query);
                response.Data = await _cashSaleService.FindByIdAsync(cashSaleQuery);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"CashSale Request: {JsonConvert.SerializeObject(query)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Saves a new cashsalehd.
        /// </summary>
        /// <param name="resource">cashsalehd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateCashSale")]
        [ProducesResponseType(typeof(CashSaleHdResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PostAsync([FromBody] SaveCashSaleResource resource)
        {
            ResponseData<CashSaleHdResource> response = new ResponseData<CashSaleHdResource>();
            try
            {
                var cashSale = _mapper.Map<SaveCashSaleResource, CashSale>(resource);
                var result = await _cashSaleService.SaveAsync(cashSale);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                var cashSaleHdResource = _mapper.Map<SalCashsaleHd, CashSaleHdResource>(result.Resource);
                response.Data = cashSaleHdResource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"PostAsync Request: {JsonConvert.SerializeObject(resource)};Error: {ex.Message}");
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
        [HttpPut("UpdateCashSale/{guid}")]
        [ProducesResponseType(typeof(CashSaleHdResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PutAsync(Guid guid, [FromBody] SaveCashSaleResource resource)
        {
            ResponseData<CashSaleHdResource> response = new ResponseData<CashSaleHdResource>();

            try
            {
                var cashSale = _mapper.Map<SaveCashSaleResource, CashSale>(resource);
                var result = await _cashSaleService.UpdateAsync(guid, cashSale);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                var cashSaleHdResource = _mapper.Map<SalCashsaleHd, CashSaleHdResource>(result.Resource);
                response.Data = cashSaleHdResource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"PutAsync Request: {JsonConvert.SerializeObject(resource)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Saves a new cashsalehd.
        /// </summary>
        /// <param name="resource">cashsalehd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateCashSaleList")]
        [ProducesResponseType(typeof(CashSaleHdResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PostListAsync([FromBody] List<SalCashsaleHd> cashSaleList)
        {
            ResponseData<CashSaleHdResource> response = new ResponseData<CashSaleHdResource>();
            try
            {
                var result = await _cashSaleService.SaveListAsync(cashSaleList);
                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"PostListAsync Request: {JsonConvert.SerializeObject(cashSaleList)};Error: {ex.Message}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error(response.Message);
                return BadRequest(response);
            }
        }

        [HttpGet("GetQuotation")]
        public async Task<IActionResult> GetQuotation()
        {
            try
            {
                List<SalQuotationHd> result = await _cashSaleService.GetQuotation();
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"GetQuotation; Error: {ex.Message}");
                string strStacktrack = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex.Message + Environment.NewLine + strStacktrack );
            }
        }

        [HttpPost("GetQuotationListByCashSale")]
        public async Task<IActionResult> GetQuotationListByCashSale(SalCashsaleHd pCashSale)
        {
            try
            {
                List<SalQuotationHd> quotationHeader = await _cashSaleService.GetQuotationListByCashSale(pCashSale);
                string result = JsonConvert.SerializeObject(quotationHeader);
                return Content(result, _appJson);
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"GetQuotationListByCashSale Request: {JsonConvert.SerializeObject(pCashSale)};Error: {ex.Message}");
                string strStacktrack = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex.Message + Environment.NewLine + strStacktrack);
            }
        }

        [HttpPost("GetQuotationDetail")]
        public async Task<IActionResult> GetQuotationDetail(SalQuotationHd pQuotationHeader)
        {
            try
            {
                QuotationDetail[] arrQuotationDetail = await _cashSaleService.GetQuotationDetail(pQuotationHeader);
                string result = JsonConvert.SerializeObject(arrQuotationDetail);
                return Content(result , _appJson);
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"GetQuotationDetail Request: {JsonConvert.SerializeObject(pQuotationHeader)};Error: {ex.Message}");
                string strStacktrack = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex.Message + Environment.NewLine + strStacktrack);
            }
        }

        [HttpPost("SaveCashSale2")]
        public async Task<IActionResult> SaveCashSale2(CashSaleResource2 pInput)
        {
            return await DefaultService.DoActionAsync("SaveCashSale2", async () =>
           {               
               await _cashSaleService.SaveCashSale2(pInput);
               return pInput;
           }, log);            
        }

        [HttpPost("SaveCashSale2Old")]
        public async Task<IActionResult> SaveCashSale2Old(CashSaleResource2 pInput)
        {
            try
            {
                await _cashSaleService.SaveCashSale2(pInput);
                string result = JsonConvert.SerializeObject(pInput);
                return Content(result, _appJson);
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"SaveCashSale2 Request: {JsonConvert.SerializeObject(pInput)};Error: {ex.Message}");
                string strStacktrack = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex.Message + Environment.NewLine + strStacktrack);
            }
        }
        [HttpGet("GetCashSale2")]
        public async Task<IActionResult> GetCashSale2(string pStrGuid)
        {
            try
            {
                CashSaleResource2 cashSaleResource = await _cashSaleService.GetCashSale2(pStrGuid);
                string result = JsonConvert.SerializeObject(cashSaleResource);
                return Content(result, _appJson);
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                log.Error($"GetCashSale2 Request: {JsonConvert.SerializeObject(pStrGuid)};Error: {ex.Message}");
                string strStacktrack = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex.Message + Environment.NewLine + strStacktrack);
            }          
        }



    }
}
