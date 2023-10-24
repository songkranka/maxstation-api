using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Services;
using System;
using System.Threading.Tasks;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryCtrlController : BaseController
    {
        private readonly IDeliveryCtrlService service;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummarySaleController));

        public DeliveryCtrlController(IDeliveryCtrlService _service, PTMaxstationContext context) : base(context)
        {
            this.service = _service;
        }

        [HttpPost("PrintPdf")]
        public async Task<IActionResult> PrintPdf(DeliveryCtrlRequest req)
        {
            try
            {
                var result = await service.GetDocument(req);
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
