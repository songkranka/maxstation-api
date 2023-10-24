using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using MasterData.API.Resources;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Resources.Employee;
using Microsoft.AspNetCore.Authorization;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(EmployeeController));
        public EmployeeController(
            IEmployeeService employeeService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _employeeService = employeeService;
            _mapper = mapper;

        }

        [HttpPost("List")]
        [ProducesResponseType(typeof(QueryResultResource<MasEmployee>), 200)]
        public async Task<QueryResult<MasEmployee>> ListAsync([FromBody] EmployeeQueryResource query)
        {
            var employeeQuery = _mapper.Map<EmployeeQueryResource, EmployeeQuery>(query);
            var resource = await _employeeService.ListAsync(employeeQuery);
            return resource;
        }

        [HttpPost("ListAllWithoutPage")]
        [ProducesResponseType(typeof(QueryResultResource<MasEmployee>), 200)]
        public async Task<QueryResult<MasEmployee>> ListAllWithoutPage([FromBody] EmployeeQueryResource query)
        {
            var employeeQuery = _mapper.Map<EmployeeQueryResource, EmployeeQuery>(query);
            var resource = await _employeeService.ListAllWitnoutPageAsync(employeeQuery);
            return resource;
        }

        [HttpPost("ListAllByBranch")]
        [ProducesResponseType(typeof(QueryResultResource<MasEmployee>), 200)]
        public async Task<QueryResult<MasEmployee>> ListAllByBranch([FromBody] EmployeeQueryByBranch query)
        {
            var resource = await _employeeService.ListAllByBranch(query);
            return resource;
        }

        [HttpGet("FindById/{id}")]
        public async Task<IActionResult> DetailListAsync(string id)
        {
            ResponseData<MasEmployee> response = new ResponseData<MasEmployee>();

            try
            {
                response.Data = await _employeeService.FindByIdAsync(id);
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
