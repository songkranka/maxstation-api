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
using Sale.API.Resources.Invoice;
using log4net;
using Sale.API.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace Sale.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : Controller
    {
        
        private readonly Domain.Repositories.IInvoiceRepository _repInvoice;
        private readonly IInvoiceService _serviceInvoice = null;
        private static readonly ILog _log = LogManager.GetLogger(typeof(InvoiceController));
        public InvoiceController(Domain.Repositories.IInvoiceRepository pRepInvoice , IInvoiceService pService)
        {
            _repInvoice = pRepInvoice;
            _serviceInvoice = pService;
        }

        [HttpGet("SearchInvoice")]
        public async Task<IActionResult>
        SearchInvoice([FromQuery] Resources.Invoice.InvoiceQueryResource query)
        {
            Resources.QueryResultResource<MaxStation.Entities.Models.SalCreditsaleHd> result = null;
            try
            {                
                result = await _repInvoice.ListAsync(query);                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            
            //return result;
        }

        private async Task<Resources.QueryResultResource<MaxStation.Entities.Models.SalCreditsaleHd>>
        SearchInvoiceOld([FromQuery] Resources.Invoice.InvoiceQueryResource query)
        {
            Resources.QueryResultResource<MaxStation.Entities.Models.SalCreditsaleHd> result = null;
            try
            {
                result = await _repInvoice.ListAsync(query);

            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
            }

            return result;
        }

        //----[ Deprecate ]---------//
        [HttpGet("GetRunningDocNo")]
        public async Task<Sale.API.Domain.Services.Communication.ApiResponse<string>> 
        GetRunningDocNo([FromQuery] Resources.Invoice.RunningDocNoQuery query)
        {
            
            var result = new Sale.API.Domain.Services.Communication.ApiResponse<string>();
            try
            {
                string strRunningDocNo = await _repInvoice.GetRunningDocNo(query.compCode, query.brnCode, query.locCode);
                result.SetResult(strRunningDocNo);                
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("Error", ex);
            }
            return result;
        }
        [HttpPost("GetRunning")]
        public async Task<IActionResult> GetRunning([FromBody] RunningDocNoQuery query)
        {
            try
            {
                string strRunning = await _repInvoice.GetRunningNo(query.compCode, query.locCode, query.brnCode);
                return Ok(strRunning);
            }
            catch (Exception ex)
            {
                string strMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                _log.Error("Error", ex);
                return BadRequest(strMessage);
            }
        }
        [HttpPost("GetRunningPattern")]
        public async Task<IActionResult> GetRunningPattern([FromBody] RunningDocNoQuery query)
        {
            try
            {
                string strRunning = await _repInvoice.GetRunningPattern(query.compCode, query.locCode, query.brnCode);
                return Ok(strRunning);
            }
            catch (Exception ex)
            {
                string strMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                _log.Error("Error", ex);
                return BadRequest(strMessage);
            }
        }

        [HttpGet("GetProductService")]
        public async Task<IActionResult> GetProductService()
        {
            try
            {
                List<InvoiceDropdownResponse> listProductService = await _serviceInvoice.GetProductService();                
                return Ok(listProductService);
            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        //[HttpGet("GetProductService")]
        private async Task<Sale.API.Domain.Services.Communication.ApiResponse<List<Domain.Services.Communication.InvoiceDropdownResponse>>>
        GetProductServiceOld()
        {
            var result = new Sale.API.Domain.Services.Communication.ApiResponse<List<Domain.Services.Communication.InvoiceDropdownResponse>>();
            try
            {
                List<Domain.Services.Communication.InvoiceDropdownResponse> listProductService = await _repInvoice.GetProductService();
                result.SetResult(listProductService);                

            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("Error", ex);
            }            
            return result;
        }


        //[HttpGet("Test")]
        //public List<MaxStation.Entities.Models.SalCreditsaleDt> Test()
        //{

        //    return _repInvoice.Test();
        //}

        
        [HttpPost("InsertCreditSales")]
        public async Task<Sale.API.Domain.Services.Communication.ApiResponse<bool>>
        InsertCreditSales([FromBody] Sale.API.Resources.Invoice.InsertCreditSalesQuery param)
        {
            var result = new Sale.API.Domain.Services.Communication.ApiResponse<bool>();
            try
            {
                await _serviceInvoice.InsertCreditSales(param.CreditSaleHeader, param.ArrCreditSaleDetail);
                result.SetResult(true);                
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("Error", ex);
            }
            return result;
        }
        [HttpPost("InsertCreditSales2")]
        public async Task<ApiResponse<InsertCreditSalesQuery>>
        InsertCreditSales2([FromBody] InsertCreditSalesQuery param)
        {
            var result = new ApiResponse<InsertCreditSalesQuery>();
            try
            {
                await _serviceInvoice.InsertCreditSales(param.CreditSaleHeader, param.ArrCreditSaleDetail);
                result.SetResult(param);                
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("Error", ex);
            }
            return result;
        }
        [HttpPost("UpdateCreditSales")]
        public async Task<Sale.API.Domain.Services.Communication.ApiResponse<bool>>
        UpdateCreditSales([FromBody] Sale.API.Resources.Invoice.InsertCreditSalesQuery param)
        {
            var result = new Sale.API.Domain.Services.Communication.ApiResponse<bool>();
            try
            {
                await _serviceInvoice.UpdateCreditSales(param.CreditSaleHeader, param.ArrCreditSaleDetail);
                result.SetResult(true);                
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("Error", ex);
            }
            return result;
        }

        public class GetCreditSalesParam
        {
            public string LocCode { get; set; }
            public string BrnCode { get; set; }
            public string CompCode { get; set; }
            public string DocNo { get; set; }
        }

        [HttpPost("GetCreditSales")]
        public async Task<ApiResponse<InsertCreditSalesQuery>>
        GetCreditSales([FromBody] GetInvoiceQueryResource param)
        {
            var result = new ApiResponse<InsertCreditSalesQuery>();
            var creditSales = new InsertCreditSalesQuery();
            try
            {
                creditSales = await _serviceInvoice.GetCreditSales(param);
                //creditSales.CreditSaleHeader = await _context.SalCreditsaleHds
                //    .FirstOrDefaultAsync(x => 
                //        x.DocNo == param.DocNo && 
                //        x.BrnCode == param.BrnCode && 
                //        x.CompCode == param.CompCode && 
                //        "Invoice".Equals( x.DocType)
                //    );
                //creditSales.ArrCreditSaleDetail = await _context.SalCreditsaleDts
                //    .Where(x => 
                //        x.DocNo == param.DocNo && 
                //        x.BrnCode == param.BrnCode && 
                //        x.CompCode == param.CompCode
                //    ).ToArrayAsync();
                result.SetResult(creditSales);                
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("Error", ex);
            }
            return result;
        }

        [HttpGet("GetCreditSalesByGuid/{pStrGuid}")]
        public async Task<ApiResponse<InsertCreditSalesQuery>> GetCreditSalesByGuid(string pStrGuid)
        {
            var result = new ApiResponse<InsertCreditSalesQuery>();
            var creditSales = new InsertCreditSalesQuery();
            try
            {
                creditSales = await _serviceInvoice.GetCreditSalesByGuid(pStrGuid);                
                result.SetResult(creditSales);                
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                _log.Error("Error", ex);
            }
            return result;
        }

        private string getErrorMessage(Exception pException)
        {
            if(pException == null)
            {
                return string.Empty;
            }
            string strErrorStackTrace = pException.StackTrace;
            while (pException.InnerException != null) pException = pException.InnerException;
            string strErrorMessage = pException.Message;
            return $"{strErrorMessage} \r\n {strErrorStackTrace}";
        }
    }
}
