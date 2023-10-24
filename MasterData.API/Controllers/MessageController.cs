using log4net;
using MasterData.API.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : Controller
    {

        MessageService _svMessage = null;
        private ILog _log = LogManager.GetLogger(typeof(MessageController));
        public MessageController(PTMaxstationContext pContext)
        {
            _svMessage = new MessageService(pContext);
        }

        [HttpGet("GetArrSysMessage")]
        public async Task<IActionResult> GetArrSysMessage()
        {
            return await DefaultService.DoActionAsync("GetArrSysMessage", async () => await _svMessage.GetArrSysMessage(), _log);

        }
    }
}
