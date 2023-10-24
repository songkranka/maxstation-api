//using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Sale.API.Resources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using Sale.API.Domain.Services.Communication;
using Sale.API.Resources.CreditNote;
using log4net;
using Sale.API.Domain.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Sale.API.Services;

namespace Sale.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CreditNoteController : Controller
    {
        private const string _appJson = "application/json";

        private readonly Domain.Repositories.ICreditNoteRepository _repCreditNote;
        private readonly ICreditNoteService _serviceCreditNote = null;
        private PTMaxstationContext _context = null;
        private static readonly ILog _log = LogManager.GetLogger(typeof(CreditNoteController));
        public CreditNoteController(Domain.Repositories.ICreditNoteRepository pRepCreditNote, ICreditNoteService pService , PTMaxstationContext context)
        {
            _repCreditNote = pRepCreditNote;
            _serviceCreditNote = pService;
            _context = context;
        }

        #region - Controller -

        [HttpGet("SearchCreditNote")]
        public async Task<IActionResult>
        SearchCreditNote([FromQuery] Resources.CreditNote.CreditNoteQueryResource query)
        {
            Resources.QueryResultResource<MaxStation.Entities.Models.SalCndnHd> result = null;
            try
            {                
                result = await _repCreditNote.ListAsync(query);                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            
            //return result;
        }

        [HttpGet("GetCreditNote/{pStrGuid}")]
        public async Task<IActionResult> GetCreditNote(string pStrGuid)
        {
            try
            {
                CreditNoteQueryResource2 apiResult = null;
                apiResult = await _serviceCreditNote.GetCreditNote(pStrGuid);

                string strJson = JsonConvert.SerializeObject(apiResult);                
                return Content(strJson, _appJson);
            }
            catch (Exception ex)
            {
                _log.Error("GetCreditNote" ,ex);
                string strErrorMessage = getErrorMessage(ex);
                return BadRequest(strErrorMessage);
            }
        }
        [HttpPost("SaveCreditNote")]
        public async Task<IActionResult> SaveCreditNote(CreditNoteQueryResource2 pInput)
        {
            try
            {
                await _serviceCreditNote.SaveCreditNote(pInput);
                string strJson = JsonConvert.SerializeObject(pInput);                
                return Content(strJson, _appJson);
            }
            catch (Exception ex)
            {
                _log.Error("SaveCreditNote", ex);
                string strErrorMessage = getErrorMessage(ex);
                return BadRequest(strErrorMessage);
            }
        }
        [HttpPost("GetTaxInvoiceList")]
        public async Task<IActionResult> GetTaxInvoiceList(CreditNoteQueryResource2 pInput)
        {
            try
            {
                SalTaxinvoiceHd[] arrTax = null;
                arrTax = await _serviceCreditNote.GetTaxInvoiceList(pInput);
                string strJson = JsonConvert.SerializeObject(arrTax);                
                return Content(strJson, _appJson);
            }
            catch (Exception ex)
            {
                _log.Error("GetTaxInvoiceList", ex);
                string strErrorMessage = getErrorMessage(ex);
                return BadRequest(strErrorMessage);
            }
        }
        [HttpPost("GetTaxInvoiceDetailList")]
        public async Task<IActionResult> GetTaxInvoiceDetailList(SalTaxinvoiceHd pInput)
        {
            try
            {
                SalTaxinvoiceDt[] arrTaxDetail = null;
                arrTaxDetail = await _serviceCreditNote.GetTaxInvoiceDetailList(pInput);
                string strJson = JsonConvert.SerializeObject(arrTaxDetail);                
                return Content(strJson, _appJson);
            }
            catch (Exception ex)
            {
                _log.Error("GetTaxInvoiceDetailList", ex);
                string strErrorMessage = getErrorMessage(ex);
                return BadRequest(strErrorMessage);
            }
        }

        [HttpGet("GetArrayReason")]
        public async Task<IActionResult> GetArrayReason()
        {
            return await doAction("GetArrayReason", async () => await getArrayReason());
        }

        //public async Task<SearchTaxInvoiceResult> SearchTaxInvoice(SearchTaxInvoiceParam param)
        [HttpPost("SearchTaxInvoice")]
        public async Task<IActionResult> SearchTaxInvoice(SearchTaxInvoiceParam param)
        {
            return await DefaultService.DoActionAsync(
                pStrFunctionName    : "SearchTaxInvoice", 
                pFunc               : async () => await _serviceCreditNote.SearchTaxInvoice(param), 
                pLog                : _log
            );
            
        }

        #endregion

        //private string getErrorMessage(Exception pException)
        //{
        //    if(pException == null)
        //    {
        //        return string.Empty;
        //    }
        //    string strStack = pException.StackTrace;
        //    while (pException.InnerException != null) pException = pException.InnerException;
        //    return pException.Message + Environment.NewLine + strStack;
        //}

        #region - DoAction -

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
            result = Content(strJson, _appJson);
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


        #region - Function -

        private const string _strActive = "Active";
        private const string _strCreditNote = "CreditNote";

        private async Task<MasReason[]> getArrayReason()
        {
            IQueryable<MasReason> qryReason = null;
            qryReason = _context.MasReasons.Where(
                x => _strCreditNote.Equals(x.ReasonGroup) 
                && _strActive.Equals(x.ReasonStatus) 
            ).AsNoTracking();

            MasReason[] result = null;
            result = await qryReason.ToArrayAsync();
            return result;
        }

        #endregion
    }
}
