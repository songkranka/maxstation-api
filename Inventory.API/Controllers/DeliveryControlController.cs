using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using Inventory.API.Services;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryControlController : Controller
    {

        private static readonly ILog _log = LogManager.GetLogger(typeof(DeliveryControlController));

        DeliveryControlService _svDelivery = null;

        public DeliveryControlController(PTMaxstationContext pContext, IUnitOfWork pUnitOfWork)
        {
            _svDelivery = new DeliveryControlService(pContext, pUnitOfWork);
        }

        [HttpGet("GetMasMapping")]
        public async Task<IActionResult> GetMasMapping()
        {
            return await DefaultService.DoActionAsync(
                "GetMasMapping", 
                async () => await _svDelivery.GetMasMapping(), 
                _log
            );
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            return await DefaultService.DoActionAsync(
                "GetProducts",
                async () => await _svDelivery.GetProducts(),
                _log
            );
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(InvDeliveryCtrlHd param)
        {
            return await DefaultService.DoActionAsync(
                "UpdateStatus",
                async () => await _svDelivery.UpdateStatus(param),
                _log
            );
        }

        [HttpGet("GetDeliveryControl/{pStrGuid}")]
        public async Task<IActionResult> GetDeliveryControl(string pStrGuid)
        {
            return await DefaultService.DoActionAsync(
                "GetDeliveryControl",
                async () => await _svDelivery.GetDeliveryControl(pStrGuid),
                _log
            );
        }     
        
        [HttpPost("SaveDeliveryControl")]
        public async Task<IActionResult> SaveDeliveryControl(ModelDeliveryControl param)
        {
            return await DefaultService.DoActionAsync(
                "SaveDeliveryControl",
                async () => await _svDelivery.SaveDeliveryControl(param),
                _log
            );
        }
        
        [HttpPost("SearchDelivery")]
        public async Task<IActionResult> SearchDelivery(ModelParamSearchDelivery param)
        {
            return await DefaultService.DoActionAsync(
                "SearchDelivery",
                async () => await _svDelivery.SearchDelivery(param),
                _log
            );
        }
        
        [HttpPost("SearchReceive")]
        public async Task<IActionResult> SearchReceive(ModelParamSearchReceive param)
        {
            return await DefaultService.DoActionAsync(
                "SearchReceive",
                async () => await _svDelivery.SearchReceive(param),
                _log
            );
        }


    }
}
