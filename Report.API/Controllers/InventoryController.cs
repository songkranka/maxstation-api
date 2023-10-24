using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : BaseController
    {

        private readonly IInventoryService _inventoryService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummarySaleController));
        public InventoryController(IInventoryService intentoryService, PTMaxstationContext context) : base(context)
        {
            _inventoryService = intentoryService;
        }

        [HttpPost("GetReceiveProdPDF")]
        public IActionResult GetReceiveProdPDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetReceiveProdPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetReceiveProdExcel")]
        public IActionResult GetReceiveProdExcel(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetReceiveProdExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetWithdrawPDF")]
        public IActionResult GetWithdrawPDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetWithdrawPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetWithdrawExcel")]
        public IActionResult GetWithdrawExcel(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetWithdrawExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetTransferOutPDF")]
        public IActionResult GetTransferOutPDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetTransferOutPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetTransferOutExcel")]
        public IActionResult GetTransferOutExcel(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetTransferOutExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetTransferInPDF")]
        public IActionResult GetTransferInPDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetTransferInPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetTransferInExcel")]
        public IActionResult GetTransferInExcel(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetTransferInExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetTransferComparePDF")]
        public IActionResult GetTransferComparePDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetTransferComparePDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetTransferCompareExcel")]
        public IActionResult GetTransferCompareExcel(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetTransferCompareExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetTransferNotInPDF")]
        public IActionResult GetTransferNotInPDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetTransferNotInPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetTransferNotInExcel")]
        public IActionResult GetTransferNotInExcel(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetTransferNotInExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetReturnSupPDF")]
        public IActionResult GetReturnSupPDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetReturnSupPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetReturnSupExcel")]
        public IActionResult GetReturnSupExcel(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetReturnSupExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("GetReturnOilPDF")]
        public IActionResult GetReturnOilPDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetReturnOilPDF(req);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetReturnOilExcel")]
        public IActionResult GetReturnOilExcel(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetReturnOilExcel(req).ToArray();
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetAdjustPDF")]
        public IActionResult GetAdjustPDF(InventoryRequest req)
        {
            try
            {
                var result = _inventoryService.GetAdjustPDF(req);
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
