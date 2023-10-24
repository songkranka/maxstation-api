using AutoMapper;
using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Resources;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Inventory.API.Domain.Services;
using Inventory.API.Resources.Adjust;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdjustController : BaseController
    {
        private readonly IAdjustService _AdjustService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(RequestController));
        private const string _appJson = "application/json";

        public AdjustController(IAdjustService AdjustService, IMapper mapper, PTMaxstationContext context) : base(context)
        {
            _AdjustService = AdjustService;
            _mapper = mapper;
        }


        [HttpPost("GetAdjustHDList")]
        [ProducesResponseType(typeof(QueryResultResource<AdjustResource>), 200)]
        public async Task<QueryResultResource<AdjustResource>> GetAdjustHDList([FromBody] AdjustQueryResource req)
        {
            QueryResultResource<AdjustResource> response = new QueryResultResource<AdjustResource>();
            try
            {
                var queryResult = await _AdjustService.GetAdjustHDList(req);

                response = _mapper.Map<QueryResult<InvAdjustHd>, QueryResultResource<AdjustResource>>(queryResult);
                response.IsSuccess = true;
                response.Message = "";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        [HttpGet("GetAdjust/{guid}")]
        public async Task<IActionResult> GetAdjust(Guid guid)
        {
            ResponseData<InvAdjustHd> response = new ResponseData<InvAdjustHd>();

            try
            {
                response.Data = await _AdjustService.FindByIdAsync(guid);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";                
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("GetAdjust", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("UpdateAdjust/{guid}")]
        [ProducesResponseType(typeof(AdjustResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> UpdateAdjust(Guid guid, [FromBody] InvAdjustHd AdjustHd)
        {
            var result = await _AdjustService.UpdateAsync(guid, AdjustHd);
            if (!result.Success)
            {
                _log.Error("UpdateAdjust : " + result.Message);
                return BadRequest(new ErrorResource(result.Message));
            }

            var cashSaleHdResource = _mapper.Map<InvAdjustHd, AdjustResource>(result.Resource);
            return Ok(cashSaleHdResource);
        }

        [HttpPost("CreateAdjust")]
        [ProducesResponseType(typeof(AdjustResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> CreateAdjustAsync([FromBody] InvAdjustHd model)
        {
            ResponseData<AdjustResource> response = new ResponseData<AdjustResource>();
            try
            {
                var result = await _AdjustService.CreateAsync(model);

                if (!result.Success)
                {
                    _log.Error("CreateAdjust : " + result.Message);
                    return BadRequest(new ErrorResource(result.Message));
                }

                var AdjustResource = _mapper.Map<InvAdjustHd, AdjustResource>(result.Resource);
                response.Data = AdjustResource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("CreateAdjust", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("GetReasonAdjusts")]
        public async Task<IActionResult> GetReasonAdjusts()
        {
            try
            {
                MasReason[] arrReason = null;
                arrReason = await _AdjustService.GetReasonAdjusts();
                string strJson = null;
                strJson = JsonConvert.SerializeObject(arrReason);
                ContentResult result = null;
                result = Content(strJson, _appJson);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error("GetReasons", ex);
                string strErrorMessage = getErrorMessage(ex);
                return BadRequest(strErrorMessage);
            }
        }

        private string getErrorMessage(Exception pException)
        {
            if (pException == null)
            {
                return string.Empty;
            }
            string strStack = pException.StackTrace;
            while (pException.InnerException != null) pException = pException.InnerException;
            return pException.Message + Environment.NewLine + strStack;
        }
    }
}
