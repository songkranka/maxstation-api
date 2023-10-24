using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : BaseController
    {
        public DemoController(PTMaxstationContext context) : base(context)
        {
        }

        [HttpGet("Sql")]
        public IActionResult Sql()
        {
            var value = this.context.MasUnits.FirstOrDefault(x => x.UnitId == "042").UpdatedBy;
            return Ok($"Test Sql :" + value);
        }


        [HttpGet("Hello")]
        public IActionResult Hello()
        {            
            return Ok($"Hello PostDay");
        }



    }
}
