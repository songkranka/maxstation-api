using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
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
    public class OtherController : BaseController
    {
        private readonly IOtherService _otherService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        public OtherController(
            IOtherService otherService,
            PTMaxstationContext context) : base(context)

        {
            _otherService = otherService;
        }

        [HttpPost("GetPattern")]
        public IActionResult GetPattern([FromBody] OtherRequest req)
        {
            ResponseData<RespDocType> response = new ResponseData<RespDocType>();
            try
            {
                response.Data = _otherService.GetPattern(req);
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


        [HttpPost("GetMasMappingList")]
        public IActionResult GetMasMappingList([FromBody] OtherRequest req)
        {
            ResponseData<List<MasMapping>> response = new ResponseData<List<MasMapping>>();
            try
            {
                response.Data = _otherService.GetMasMappingList(req);
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
    }
}
