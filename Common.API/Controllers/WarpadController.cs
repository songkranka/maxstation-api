using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Common.API.Domain.Services;
using Common.API.Controllers;
using Common.API.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Common.API.Services;

namespace Common.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarpadController : BaseController
    {
        private readonly IWarpadService _warpadService;
        private static readonly ILog log = LogManager.GetLogger(typeof(WarpadController));
        public WarpadController(
            IWarpadService warpadService,
            PTMaxstationContext context) : base(context)
        {
            _warpadService = warpadService;
        }

        //============================== HttpPost ==============================
        [HttpPost("GetWarpadTaskList")]
        public async Task<ResponseWarpadTaskList> GetWarpadTaskList(RequestWarpadTaskList req)
        {
            ResponseWarpadTaskList response = new ResponseWarpadTaskList();
            try
            {
                response = await _warpadService.GetWarpadTaskList(req);
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return response;
            }
        }
        [HttpGet("GetToDoTask/{pStrEmpCode}")]
        public async Task<IActionResult> GetToDoTask(string pStrEmpCode)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName: "GetToDoTask",
                pFunc: async()=> await _warpadService.GetToDoTask(pStrEmpCode),
                pLog: log
            );
        }
    }
}
