using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sal.API.Controllers;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Models.Request;
using Sale.API.Domain.Services;
using Sale.API.Resources;
using Sale.API.Resources.CashTax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CashTaxController : BaseController
    {
        private readonly ICashTaxService _cashTaxService;
        private readonly IMapper _mapper;

        public CashTaxController(
            ICashTaxService cashTaxService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _cashTaxService = cashTaxService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lists all existing cashTaxhd.
        /// </summary>
        /// <returns>List of cashTaxHd.</returns>
        [HttpPost("GetCashTaxHdList")]
        [ProducesResponseType(typeof(QueryResultResource<CashTaxHdResource>), 200)]
        public async Task<QueryResultResource<CashTaxHdResource>> CashTaxHdListAsync([FromBody] CashTaxHdQueryResource query)
        {
            var cashTaxQuery = _mapper.Map<CashTaxHdQueryResource, CashTaxHdQuery>(query);

            var queryResult = await _cashTaxService.ListAsync(cashTaxQuery);
            var resource = _mapper.Map<QueryResult<SalTaxinvoiceHd>, QueryResultResource<CashTaxHdResource>>(queryResult);
            return resource;
        }

        /// <summary>
        /// Lists all existing cashtaxhd.
        /// </summary>
        /// <returns>List of cashtaxHd.</returns>
        [HttpPost("CashTax")]
        public async Task<IActionResult> CashTax([FromBody] CashTaxHdQueryResource query)
        {
            ResponseData<CashTax> response = new ResponseData<CashTax>();

            try
            {
                var cashTaxQuery = _mapper.Map<CashTaxHdQueryResource, CashTaxHdQuery>(query);
                response.Data = await _cashTaxService.FindByIdAsync(cashTaxQuery);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Saves a new cashtaxhd.
        /// </summary>
        /// <param name="resource">cashtax data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPost("CreateCashTax")]
        [ProducesResponseType(typeof(CashTaxHdResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PostAsync([FromBody] SaveCashTaxResource resource)
        {
            ResponseData<CashTaxHdResource> response = new ResponseData<CashTaxHdResource>();
            try
            {
                var cashTax = _mapper.Map<SaveCashTaxResource, CashTax>(resource);
                var result = await _cashTaxService.SaveAsync(cashTax);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                var cashTaxHdResource = _mapper.Map<SalTaxinvoiceHd, CashTaxHdResource>(result.Resource);
                response.Data = cashTaxHdResource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch(Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Updates an existing cashtaxhd according to an identifier.
        /// </summary>
        /// <param name="id">Cashtaxhd identifier.</param>
        /// <param name="resource">Updated cashtaxhd data.</param>
        /// <returns>Response for the request.</returns>
        [HttpPut("UpdateCashTax/{guid}")]
        [ProducesResponseType(typeof(CashTaxHdResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PutAsync(Guid guid, [FromBody] SaveCashTaxResource resource)
        {
            ResponseData<CashTaxHdResource> response = new ResponseData<CashTaxHdResource>();
            try
            {
                var cashTax = _mapper.Map<SaveCashTaxResource, CashTax>(resource);
                var result = await _cashTaxService.UpdateAsync(guid, cashTax);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                var cashTaxHdResource = _mapper.Map<SalTaxinvoiceHd, CashTaxHdResource>(result.Resource);
                response.Data = cashTaxHdResource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            

        }
        [HttpPost("CancelAndReplace")]
        public async Task<IActionResult> CancelAndReplace([FromBody] CashTaxCancelAndReplaceRequest pInput)
        {
            try
            {
                string result = string.Empty;
                await _cashTaxService.CancelAndReplace(pInput);
                result = JsonConvert.SerializeObject(pInput);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return BadRequest(strMessage);
            }
        }

        [HttpPost("GetFinBalanceByCashTax")]
        public async Task<IActionResult> GetFinBalanceByCashTax(FinBalanceByCasTaxRequest pCashTax)
        {
            try
            {
                FinBalance fin = await _cashTaxService.GetFinBalanceByCashTax(pCashTax);
                string result = JsonConvert.SerializeObject(fin);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return BadRequest(strMessage);
            }
        }

        [HttpPost("GetCustomerByCustCode")]
        public async Task<IActionResult> GetCustomerByCustCode(CustomerByCustCodeRequset request)
        {
            try
            {
                var masCustomer = await _cashTaxService.GetCustomerByCustCode(request);
                string result = JsonConvert.SerializeObject(masCustomer);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                return BadRequest(strMessage);
            }
        }
    }
}
