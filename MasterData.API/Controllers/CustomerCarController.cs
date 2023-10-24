using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.CustomerCar;
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
    public class CustomerCarController : BaseController
    {
        private readonly ICustomerCarService _customerCarService;
        private static readonly ILog _log = LogManager.GetLogger(typeof(CompanyCarController));
        private readonly IMapper _mapper;
        public CustomerCarController(
            ICustomerCarService customerCarService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _customerCarService = customerCarService;
            _mapper = mapper;
        }

        [HttpPost("FindByCustCode")]
        [ProducesResponseType(typeof(QueryResultResource<MasCustomerCar>), 200)]
        public async Task<QueryResult<MasCustomerCar>> FindByCustCodeAsync([FromBody] CustomerCarQueryResource query)
        {
            var customerCarQuery = _mapper.Map<CustomerCarQueryResource, Domain.Models.Queries.CustomerCarQuery>(query);
            var resource = await _customerCarService.FindByCustCodeAsync(customerCarQuery);
            return resource;
        }

        [HttpGet("GetAllByCompany")]
        public async Task<IActionResult> GetAllByCompany()
        {
            ResponseData<List<CustomerCompanyCar>> response = new ResponseData<List<CustomerCompanyCar>>();

            try
            {
                var customerCars = await _customerCarService.GetAllByCompany();
                var resource = _mapper.Map<List<MasCustomerCar>, List<CustomerCompanyCar>>(customerCars);
                response.Data = resource;
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
    }
}
