using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services;
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
    public class MappingController : BaseController
    {
        private readonly IMasMappingService _masMappingService;

        private static readonly ILog _log = LogManager.GetLogger(typeof(CompanyCarController));
        private readonly IMapper _mapper;
        public MappingController(
            IMasMappingService masMappingService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _masMappingService = masMappingService;
            _mapper = mapper;
        }

        [HttpPost("GetMasMapping")]
        public async Task<IActionResult> GetMasMapping([FromBody] MasMappingRequest masMappingRequest)
        {
            ResponseData<List<MasMapping>> response = new ResponseData<List<MasMapping>>();

            try
            {
                response.Data = await _masMappingService.GetMasMapping(masMappingRequest.MapValue);
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
