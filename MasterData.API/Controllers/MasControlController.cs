using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
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
    public class MasControlController : BaseController
    {
        private readonly IMasControlService _masControlService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        public MasControlController(
            IMasControlService masControlService,
            PTMaxstationContext context) : base(context)

        {
            _masControlService = masControlService;
        }

        [HttpPost("GetMasControl")]
        public IActionResult GetMasControl([FromBody] MasControlRequest req)
        {
            ResponseData<MasControl> response = new ResponseData<MasControl>();
            try
            {
                response.Data = _masControlService.GetMasControl(req);
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

        [HttpPost("UpdateCtrlValue")]
        public async Task<IActionResult> UpdateCtrlValueAsync([FromBody] UpdateCtrlValueDateRequest request)
        {
            ResponseData<MasControl> response = new ResponseData<MasControl>();

            try
            {
                var result = await _masControlService.UpdateCtrlValueAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new ErrorResource(result.Message));
                }

                response.Data = result.Resource;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
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
