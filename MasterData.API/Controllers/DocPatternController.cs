using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.DocPattern.Request;
using MasterData.API.Domain.Models.DocPattern.Response;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocPatternController : BaseController
    {
        private readonly IDocpatternService _docPatternService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(DocPatternController));
        public DocPatternController(
            IDocpatternService docPatternService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _docPatternService = docPatternService;
            _mapper = mapper;
        }

        [HttpPost("GetDocPattern")]
        public async Task<IActionResult> GetDocPatternAsync([FromBody] DocPatternRequest request)
        {
            ResponseData<DocPatternResponse> response = new ResponseData<DocPatternResponse>();
            
            try
            {
                var result = await _docPatternService.GetDocPatternAsync(request);

                if (!result.Success)
                {
                    response.Data = null;
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = result.Message;
                    return Ok(JsonConvert.SerializeObject(response));
                }

                response.Data = result;
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
