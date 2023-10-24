using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Services;
using System;
using System.Threading.Tasks;

namespace Report.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GovermentController : BaseController
    {
        private readonly IGovermentService _govermentService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReceivePayController));
       
        public GovermentController(IGovermentService govermentService, PTMaxstationContext context) : base(context)
        {
            _govermentService = govermentService;
        }



        [HttpPost("GetGov01PDF")]
        public IActionResult GetGov01PDF(GovermentRequest req)
        {
            try
            {
                var result = _govermentService.GetGov01PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetGov03PDF")]
        public IActionResult GetGov03PDF(GovermentRequest req)
        {
            try
            {
                var result = _govermentService.GetGov03PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetGov05PDF")]
        public IActionResult GetGov05PDF(GovermentRequest req)
        {
            try
            {
                var result = _govermentService.GetGov05PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetGov06PDF")]
        public async Task<IActionResult> GetGov06PDF(GovermentRequest req)
        {
            try
            {
                var result = await _govermentService.GetGov06PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetGov07PDF")]
        public IActionResult GetGov07PDF(GovermentRequest req)
        {
            try
            {
                var result = _govermentService.GetGov07PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GetGov08PDF")]
        public IActionResult GetGov08PDF(GovermentRequest req)
        {
            try
            {
                var result = _govermentService.GetGov08PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetGov09PDF")]
        public IActionResult GetGov09PDF(GovermentRequest req)
        {
            try
            {
                var result = _govermentService.GetGov09PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetGov11PDF")]
        public IActionResult GetGov11PDF(GovermentRequest req)
        {
            try
            {
                var result = _govermentService.GetGov11PDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

    }
}
