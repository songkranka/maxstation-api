using Inventory.API.Domain.Models;
using Inventory.API.Domain.Services;
using Inventory.API.Resources;
using Inventory.API.Resources.Request;
using Inventory.API.Services;
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
    public class TransferOutController : BaseController
    {
        private ITransferOutService _svTransferOut = null;
        private static readonly ILog _log = LogManager.GetLogger(typeof(TransferOutController));
        public TransferOutController(
            ITransferOutService pTransferOutService,
            PTMaxstationContext context
        ) : base(context){
            _svTransferOut = pTransferOutService;
        }
        [HttpPost("SearchTranOut")]
        public async Task<IActionResult> SearchTranOut(TransferOutQueryResource param)
        {           
            try
            {
                var result = await _svTransferOut.GetTransferOutList(param);
                _log.Info("SearchTranOut Successfull");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex);                
            }
        }

        [HttpPost("GetRequestDtList")]
        public async Task<IActionResult> GetRequestDtList(GetRequestDtListQueryResource param)
        {
            try
            {
                var result = await _svTransferOut.GetRequestDtList(param);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex);
            }
        }

        [HttpPost("GetRequestHdList")]
        public async Task<IActionResult> GetRequestHdList(GetRequestHdListQueryResource param)
        {
            try
            {
                var result = await _svTransferOut.GetRequestHdList(param);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex);
            }
        }
        [HttpPost("InsertTransferOut")]
        public async Task<IActionResult> InsertTransferOut(ModelTransferOutHeader param)
        {
            ResponseData<Guid> response = new ResponseData<Guid>();

            try
            {
                var guid = await _svTransferOut.InsertTransferOut(param);
                response.Data = guid;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "บันทึกสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
        [HttpPost("UpdateTransferOut")]
        public async Task<IActionResult> UpdateTransferOut(ModelTransferOutHeader param)
        {
            try
            {
                await _svTransferOut.UpdateTransferOut(param);
                return Ok(param.Guid);
            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                return BadRequest(ex);
            }
        }

        [HttpPost("CheckStockRealTime")]
        public async Task<IActionResult> CheckStockRealTime(ModelCheckStockRealtimeParam param)
        {
            return await DefaultService.DoActionAsync("CheckStockRealTime", async () => await _svTransferOut.CheckStockRealTime(param), _log);
        }
    }
}
