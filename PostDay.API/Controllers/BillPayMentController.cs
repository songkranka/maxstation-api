using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using PostDay.API.Domain.Models;
using PostDay.API.Domain.Repositories;
using PostDay.API.Domain.Services;
using PostDay.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillPaymentController : Controller
    {
        private IBillPaymentService _svBillPayment;
        private static readonly ILog log = LogManager.GetLogger(typeof(BillPaymentController));
        public BillPaymentController(IBillPaymentService service)
        {
            _svBillPayment = service;
        }
        [HttpGet("GetBankList")]
        public async Task<IActionResult> GetBankList()
        {
            return await DefaultService.DoActionAsync(
                "GetBankList", 
                async () => await _svBillPayment.GetBankList(),
                log
            );
        }
        [HttpPost("GetPostDay")]
        public async Task<IActionResult> GetPostDay(GetPostDayParam param)
        {
            return await DefaultService.DoActionAsync(
                "GetPostDay",
                async () => await _svBillPayment.GetPostDay(param),
                log
            );
        }

        [HttpPost("UpdateBillPayment")]
        public async Task<IActionResult> UpdateBillPayment(UpdateBillPaymentParam param)
        {
            return await DefaultService.DoActionAsync(
                "UpdateBillPayment",
                async () => await _svBillPayment.UpdateBillPayment(param),
                log
            );
        }

    }
}
