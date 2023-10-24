using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.CompanyCar;
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
    public class CompanyCarController : BaseController
    {
        private readonly ICompanyCarService _companyCarService;

        private static readonly ILog _log = LogManager.GetLogger(typeof(CompanyCarController));
        private readonly IMapper _mapper;
        public CompanyCarController(
            ICompanyCarService companyCarService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _companyCarService = companyCarService;
            _mapper = mapper;
        }

        //[HttpPost("List")]
        //public async Task<IActionResult> ListAsync(CustomerCarQuery query)
        //{
        //    ResponseData<List<MasCompanyCar>> response = new ResponseData<List<MasCompanyCar>>();

        //    try
        //    {
        //        response.Data = await _companyCarService.ListAsync(query);
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.Message = "Success";
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = StatusCodes.Status400BadRequest;
        //        response.Message = ex.Message;
        //        return BadRequest(response);
        //    }
        //}

        [HttpPost("List")]
        [ProducesResponseType(typeof(QueryResultResource<MasCompanyCar>), 200)]
        public async Task<QueryResult<MasCompanyCar>> ListAsync([FromBody] CompanyCarQueryReqource query)
        {
            var customerQuery = _mapper.Map<CompanyCarQueryReqource, CompanyCarQuery>(query);
            var resource = await _companyCarService.ListAsync(customerQuery);
            return resource;
        }
    }
}
