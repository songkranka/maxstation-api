using AutoMapper;
using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : BaseController
    {
        private readonly IPurchaseOrderService _purchaseOilService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ReceiveGasController));
        public PurchaseOrderController(
            IPurchaseOrderService purchaseOilService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _purchaseOilService = purchaseOilService;
            _mapper = mapper;

        }

        [HttpPost("HeaderList")]
        public async Task<IActionResult> ListAsync(PurchaseOrderHdQuery query)
        {
            ResponseData<PurchaseOrder> response = new ResponseData<PurchaseOrder>();

            try
            {
                response.Data = await _purchaseOilService.ListAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("DetailList")]
        public async Task<IActionResult> DetailListAsync(PurchaseOrderHdQuery query)
        {
            ResponseData<PurchaseOrder> response = new ResponseData<PurchaseOrder>();

            try
            {
                response.Data = await _purchaseOilService.DetailListAsync(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        //[HttpGet("SupplierList")]
        //public async Task<IActionResult> SupplierListAsync()
        //{
        //    ResponseData<ReceiveOil> response = new ResponseData<ReceiveOil>();

        //    try
        //    {
        //        response.Data = await _purchaseOilService.SupplierListAsync();
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.Message = "Success";
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = StatusCodes.Status400BadRequest;
        //        response.Message = ex.Message;
        //        return BadRequest(response);
        //    }
        //}
    }
}
