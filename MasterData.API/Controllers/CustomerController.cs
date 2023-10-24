using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.Customer;
using MasterData.API.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(CustomerController));

        public CustomerController(
            ICustomerService customerService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        [HttpPost("GetCustomerList")]
        public IActionResult GetCustomerList([FromBody] CustomerRequest req)
        {
            ResponseData<List<MasCustomer>> response = new ResponseData<List<MasCustomer>>();
            try
            {
                response.Data = _customerService.GetCustomerList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }


        [HttpPost("GetCustomers")]
        public async Task<QueryResult<MasCustomer>> GetCustomers([FromBody] CustomerQuery query)
        {
            return await _customerService.ListAsync(query);
            //return await DefaultService.DoActionAsync("GetCustomers", async () => await _customerService.ListAsync(query), log);
        }
        /*
        [HttpPost("GetCustomers")]
        [ProducesResponseType(typeof(QueryResultResource<MasCustomer>), 200)]
        public async Task<QueryResult<MasCustomer>> GetCustomers([FromBody] CustomerQueryResource query)
        {
            var customerQuery = _mapper.Map<CustomerQueryResource, CustomerQuery>(query);
            var resource = await _customerService.ListAsync(customerQuery);
            //var resource = _mapper.Map<QueryResult<MasCustomer>, QueryResultResource<CustomerResource>>(queryResult);
            return resource;
        }
        */
        [HttpPost("GetTaxInfo")]
        [ProducesResponseType(typeof(QueryResultResource<MasCustomer>), 200)]
        public async Task<ResponseData<Revenue>> GetTaxInfo([FromBody] TaxQueryResource query)
        {
            ResponseData<Revenue> response = new ResponseData<Revenue>();
            try
            {
                response.Data = await  _customerService.GetTaxInfoAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                //log.Error("Error", ex);
                return response;
               // return BadRequest(response);
            }

        }

        [HttpGet("GetCustomer/{pStrGuid}")]
        public async Task<IActionResult> GetCustomer(string pStrGuid)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetCustomer", 
                pFunc: async () => await _customerService.GetCustomer(pStrGuid),
                pLog: log
            );
        }

        [HttpGet("CheckDuplicateCustCode/{pStrCuscode}")]
        public async Task<IActionResult> CheckDuplicateCustCode(string pStrCuscode)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "CheckDuplicateCustCode",
                pFunc: async () => await _customerService.CheckDuplicateCustCode(pStrCuscode),
                pLog: log
            );
        }

        [HttpPost("InsertCustomer")]
        public async Task<IActionResult> InsertCustomer([FromBody]ModelCustomer param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "InsertCustomer",
                pFunc: async () => await _customerService.InsertCustomer(param),
                pLog: log
            );
        }

        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] ModelCustomer param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "UpdateCustomer",
                pFunc: async () => await _customerService.UpdateCustomer(param),
                pLog: log
            );
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] MasCustomer param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "UpdateStatus",
                pFunc: async () => await _customerService.UpdateStatus(param),
                pLog: log
            );
        }

        [HttpPost("GetCustomerList2")]
        public async Task<IActionResult> GetCustomerList2([FromBody] ModelGetCustomerLisParam param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetCustomerList2",
                pFunc: async () => await _customerService.GetCustomerList2(param),
                pLog: log
            );
        }

        [HttpPost("FindAll")]
        [ProducesResponseType(typeof(QueryResultResource<MasCustomer>), 200)]
        public async Task<QueryResult<MasCustomer>> FindAllAsync([FromBody] CustomerQueryResource query)
        {
            var customerQuery = _mapper.Map<CustomerQueryResource, CustomerQuery>(query);
            var resource = await _customerService.FindAllAsync(customerQuery);
            return resource;
        }
    }
}
