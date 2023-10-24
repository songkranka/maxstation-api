using Inventory.API.Domain.Models;
using Inventory.API.Domain.Services;
using Inventory.API.Resources.Request;
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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransferInController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(TransferInController));
        private ITransferInService _svTransferIn = null;
        public TransferInController(ITransferInService pSvTransferIn  )
        {
            _svTransferIn = pSvTransferIn;
        }

        [HttpPost("SearchTranIn")]
        public async Task<IActionResult> SearchTranIn(SearchTranInQueryResource param)
        {
            ModelResponseData<List<ModelTransferInHeader>> result = null;
            try
            {
                result = await _svTransferIn.SearchTranIn(param);
            }
            catch (Exception ex)
            {
                result = new ModelResponseData<List<ModelTransferInHeader>>();
                result.SetException(ex);
                _log.Error("SearchTranIn Error ", ex);
            }
            return Ok(result);
        }
        [HttpPost("GetListTransferInDetail")]
        public async Task<IActionResult> GetListTransferInDetail(ModelTransferInHeader pHeader)
        {
            var result = new ModelResponseData<List<InvTraninDt>>();
            try
            {
                List<InvTraninDt> listTranIn = await _svTransferIn.GetListTransferInDetail(pHeader);
                result.SetData(listTranIn);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("GetListTransferInDetail Error ", ex);
            }
            return Ok(result);
        }

        [HttpPost("GetTransOutHdList")]
        public async Task<IActionResult> GetTransOutHdList(GetTransOutHdListQueryResource param)
        {
            var result = new ModelResponseData<List<InvTranoutHd>>();
            try
            {
                List<InvTranoutHd> listTranoutHds = await _svTransferIn.GetTransOutHdList(param);
                result.SetData(listTranoutHds);
            }
            catch (Exception ex)
            {
                _log.Error("GetTransOutHdList Error ", ex);
                result.SetException(ex);
            }
            return Ok(result);
        }

        [HttpPost("GetTransOutDtList")]
        public async Task<IActionResult> GetTransOutDtList(ModelTransferOutHeader param)
        {
            var result = new ModelResponseData<List<InvTranoutDt>>();
            try
            {
                List<InvTranoutDt> listTranOutDT = await _svTransferIn.GetTransOutDtList(param);
                result.SetData(listTranOutDT);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("GetTransOutHdList Error ", ex);
            }
            return Ok(result);
        }
        [HttpPost("InsertTransferIn")]
        public async Task<IActionResult> InsertTransferIn(ModelTransferInHeader param)
        {
            //var result = new ModelResponseData<ModelTransferInHeader>();
            //try
            //{
            //    await _svTransferIn.InsertTransferIn(param);
            //    result.SetData(param);
            //    _log.Info("InsertTransferIn Complete ");
            //}
            //catch (Exception ex)
            //{
            //    _log.Error("InsertTransferIn Error ", ex);
            //    result.SetException(ex);

            //}
            //return Ok(result);

            try
            {
                ResponseData<ModelTransferInHeader> response = new ResponseData<ModelTransferInHeader>();
                var receiveStatus = await _svTransferIn.CheckTranferByRefNo(param);

                if (receiveStatus)
                {
                    response.Data = null;
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = $"เอกสาร {param.RefNo} ได้รับโอนแล้ว กรุณาตรวจสอบ";
                    return Ok(JsonConvert.SerializeObject(response));
                }

                var result = await _svTransferIn.InsertTransferIn(param);
                response.Data = result;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("GetReceiveList", ex);
                string strError = getErrorMessage(ex);
                return BadRequest(strError);
            }
        }
        [HttpPost("UpdateTransferIn")]
        public async Task<IActionResult> UpdateTransferIn(ModelTransferInHeader param)
        {
            var result = new ModelResponseData<ModelTransferInHeader>();
            try
            {
                await _svTransferIn.UpdateTransferIn(param);
                result.SetData(param);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("UpdateTransferIn Error ", ex);
            }
            return Ok(result);
        }

        [HttpGet("GetTranferInHdByGuid/{guid}")]
        public async Task<IActionResult> GetBranchDetail(Guid guid)
        {
            ResponseData<InvTraninHd> response = new ResponseData<InvTraninHd>();
           
            try
            {
                response.Data = await _svTransferIn.GetTranferInHdByGuid(guid);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                _log.Error("GetBranchDetail Error", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        private string getErrorMessage(Exception pException)
        {
            if (pException == null)
            {
                return string.Empty;
            }
            string result = string.Empty;
            result = pException.StackTrace;
            while (pException.InnerException != null)
            {
                pException = pException.InnerException;
            }
            result = pException.Message + Environment.NewLine + result;
            return result;
        }
    }
}
