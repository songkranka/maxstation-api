using Common.API.Domain.Models;
using Common.API.Domain.Services;
using Common.API.Services;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Common.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : BaseController
    {
        private readonly ICommonService _commonService;
        private static readonly ILog log = LogManager.GetLogger(typeof(CommonController));
        public CommonController(
            ICommonService commonService,
            PTMaxstationContext context) : base(context)
        {
            _commonService = commonService;
        }

        //============================== HttpPost ==============================
        //[HttpPost("UpdateCloseDay")]
        //public async Task<IActionResult> UpdateCloseDay([FromBody] RequestData req)
        //{
        //    ResponseData<string> response = new ResponseData<string>();
        //    try
        //    {
        //        response.Data = await _commonService.UpdateCloseDay(req);
        //        log.Info("Success");
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error", ex);
        //        return BadRequest(response);
        //    }
        //}

        [HttpPost("GetNoti")]
        public async Task<IActionResult> GetNoti(GetNotiParam param)
        {
            return await DefaultService.DoActionAsync("GetNoti", async () => await _commonService.GetNoti(param), log);
        }

        [HttpPost("UpdateNoti")]
        public async Task<IActionResult> UpdateNoti(SysNotification param)
        {
            return await DefaultService.DoActionAsync("UpdateNoti", async () => await _commonService.UpdateNoti(param), log);
        }

        //[HttpPost("TestLog")]
        //public IActionResult TestLog()
        //{
        //    try
        //    {
        //        throw new Exception($"TestLog: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        //        //return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error", ex);
        //        return BadRequest(ex.Message);
        //    }
        //}

    }
}
