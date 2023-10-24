using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Services;
using MasterData.API.Resources.Company;
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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _companyService;

        private static readonly ILog _log = LogManager.GetLogger(typeof(CompanyCarController));
        private readonly IMapper _mapper;
        public CompanyController(
            ICompanyService companyService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _companyService = companyService;
            _mapper = mapper;
        }

        [HttpPost("GetAll")]
        //[Authorize]
        public async Task<IActionResult> GetAll()
        {
            ResponseData<List<MasCompany>> response = new ResponseData<List<MasCompany>>();

            try
            {
                response.Data = await _companyService.GetAll();
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

        [HttpPost("FindCustomerCompanyById")]
        public async Task<IActionResult> FindCustomerCompanyById([FromBody] CustomerCompanyQuery query)
        {
            ResponseData<MasCompany> response = new ResponseData<MasCompany>();

            try
            {
                response.Data = await _companyService.FindCustomerCompanyById(query);
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
