using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ReceiveGasController : BaseController
    {
        #region - Declare Valiable - 
        private static readonly ILog _log = LogManager.GetLogger(typeof(ReceiveGasController));
        private const string _strAppJson = "application/json";
        private const string _strComplete = " Complete";
        private const string _strGetPoHeaderList = "GetPoHeaderList";
        private const string _strGetPoItemList = "GetPoItemList";
        private const string _strActive = "Active";
        private const string _strSupPayAddr = "SupPayAddr";
        private const string _strSupTaxAddr = "SupTaxAddr";
        IReceiveGasService _svReceiveGas = null;

        #endregion

        #region - Controller -
        public ReceiveGasController(IReceiveGasService pSvReceiveGas , PTMaxstationContext pContext):base(pContext)
        {
            _svReceiveGas = pSvReceiveGas;
        }
        [HttpPost(_strGetPoHeaderList)]
        public async Task<IActionResult> GetPoHeaderList(PoHeaderListQuery pQuery)
        {
            try
            {
                InfPoHeader[] arrPoHeader = null;
                arrPoHeader = await _svReceiveGas.GetPoHeaderList(pQuery);
                string strJson = JsonConvert.SerializeObject(arrPoHeader);
                return Content(strJson, _strAppJson);
            }
            catch (Exception ex)
            {
                _log.Error(_strGetPoHeaderList, ex);
                string strError = getErrorMessage(ex);
                return BadRequest(strError);
            }
        }
        [HttpPost(_strGetPoItemList)]
        public async Task<IActionResult> GetPoItemList(PoItemListParam pQuery)
        {
            return await doAction("GetPoItemList", async () => await _svReceiveGas.GetPoItemList(pQuery));            
        }
        [HttpGet("GetReceiveGas/{pStrGuid}")]
        public async Task<IActionResult> GetReceiveGas(string pStrGuid)
        {
            try
            {
                ReceiveGasQuery receiveGasQuery = null;
                receiveGasQuery = await _svReceiveGas.GetReceive(pStrGuid);
                string strJson = JsonConvert.SerializeObject(receiveGasQuery);
                return Content(strJson, _strAppJson);
            }
            catch (Exception ex)
            {
                _log.Error("GetReceive", ex);
                string strError = getErrorMessage(ex);
                return BadRequest(strError);
            }
        }
        [HttpPost("GetReceiveGasList")]
        public async Task<IActionResult> GetReceiveGasList(ReceiveGasListQuery pQuery)
        {
            try
            {
                QueryResult<InvReceiveProdHd> apiResult = null;
                apiResult = await _svReceiveGas.GetReceiveList(pQuery);
                string strJson = JsonConvert.SerializeObject(apiResult);
                return Content(strJson, _strAppJson);
            }
            catch (Exception ex)
            {
                _log.Error("GetReceiveList", ex);
                string strError = getErrorMessage(ex);
                return BadRequest(strError);
            }
        }
        [HttpPost("SaveReceiveGas")]
        public async Task<IActionResult> SaveReceiveGas(ReceiveGasQuery pQuery)
        {
            try
            {
                ResponseData<ReceiveGasQuery> response = new ResponseData<ReceiveGasQuery>();

                if(pQuery.Header.DocStatus == "New")
                {
                    var receiveStatus = await _svReceiveGas.CheckReceiveStatus(pQuery);

                    if (receiveStatus)
                    {
                        response.Data = null;
                        response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                        response.Message = $"ใบสั่งซื้อเลขที่ {pQuery.Header.PoNo} ได้ทำรับแล้ว กรุณาตรวจสอบ";
                        return Ok(JsonConvert.SerializeObject(response));
                    }
                }

                var result = await _svReceiveGas.SaveReceive(pQuery);
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

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(InvReceiveProdHd pInput)
        {
            try
            {
                await _svReceiveGas.UpdateStatus(pInput);
                string strJson = JsonConvert.SerializeObject(pInput);
                return Content(strJson, _strAppJson);
            }
            catch (Exception ex)
            {
                _log.Error("UpdateStatus", ex);
                string strError = getErrorMessage(ex);
                return BadRequest(strError);
            }
        }

        [HttpGet("GetArraySupplier")]
        public async Task<IActionResult> GetArraySupplier()
        {
            return await doAction("GetArraySupplier", async () => await _svReceiveGas.GetArraySupplier());
        }

        //private async Task<ModelSupplierResult> GetSupplier(string pStrCompCode , string pStrSupCode)
        [HttpGet("GetSupplier/{pStrSupCode}/{pStrCompCode}")]
        public async Task<IActionResult> GetSupplier(string pStrCompCode, string pStrSupCode)
        {
            return await doAction("GetSupplier", async () => await getSupplier(pStrCompCode, pStrSupCode));
        }
        #endregion

        #region - DoAction -

        private string getCompleteMessage(string pStrActionName)
        {
            string result = string.Empty;
            result = pStrActionName + _strComplete; 
            return result;
        }

        private async Task<IActionResult> doAction<T>(string pStrFunctionName, Func<Task<T>> pFunc)
        {
            try
            {
                T result;
                result = await pFunc();
                return jsonResult(result);
            }
            catch (Exception ex)
            {
                _log.Error(pStrFunctionName, ex);
                return exeptionResult(ex);
            }
        }
        private ContentResult jsonResult(object pInput)
        {
            string strJson = string.Empty;

            strJson = JsonConvert.SerializeObject(pInput);
            ContentResult result = null;
            result = Content(strJson, _strAppJson);
            return result;
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
        private BadRequestObjectResult exeptionResult(Exception pException)
        {
            string strErrorMessage = string.Empty;
            strErrorMessage = getErrorMessage(pException);
            return BadRequest(strErrorMessage);
        }

        #endregion

        #region - Provate Function -

        private async Task<ModelSupplierResult> getSupplier(string pStrCompCode , string pStrSupCode)
        {
            pStrSupCode = (pStrSupCode ?? string.Empty).Trim();
            if (0.Equals(pStrSupCode))
            {
                return null;
            }
            ModelSupplierResult result = null;
            result = new ModelSupplierResult();

            IQueryable<MasSupplier> qrySupplier = null;
            qrySupplier = context.MasSuppliers.Where(
                x => pStrSupCode.Equals(x.SupCode)
                && _strActive.Equals( x.SupStatus)
            ).AsNoTracking();

            result.Supplier = await qrySupplier.FirstOrDefaultAsync();

            if(result.Supplier == null)
            {
                return result;
            }
            pStrCompCode = (pStrCompCode ?? string.Empty).ToString();
            

            IQueryable<MasSupplierPay> qrySupplierPay = null;
            qrySupplierPay = context.MasSupplierPays.Where(
                x => pStrSupCode.Equals(x.SupCode)
                && pStrCompCode.Equals(x.CompCode)
            ).AsNoTracking();

            IQueryable<MasMapping> qryMapping = null;
            qryMapping = context.MasMappings.Where(
                x => _strActive.Equals(x.MapStatus)
                && qrySupplierPay.Any(
                    y => (_strSupPayAddr.Equals(x.MapValue) && x.MapId == y.PayAddrId)
                    || (_strSupTaxAddr.Equals(x.MapValue) && x.MapId == y.TaxAddrId)
                )
            ).AsNoTracking();

            result.ArrayMapping = await qryMapping.ToArrayAsync();

            return result;
        }

        #endregion
    }
}
