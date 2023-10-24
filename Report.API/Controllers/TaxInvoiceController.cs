using AutoMapper;
using FastReport.Web;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Services;
using Report.API.Resources;
using Report.API.Resources.TaxInvoice;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaxInvoiceController : BaseController
    {
        private readonly ITaxInvoiceService _taxInvoiceService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly ILog log = LogManager.GetLogger(typeof(TaxInvoiceController));


        public TaxInvoiceController(
            ITaxInvoiceService taxInvoiceService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            PTMaxstationContext context) : base(context)

        {
            _taxInvoiceService = taxInvoiceService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
        }


        //[HttpGet("Print")]
        //[ProducesResponseType(typeof(TaxInvoiceResource), 200)]
        //[ProducesResponseType(typeof(ErrorResource), 400)]
        //public async Task<IActionResult> Print(string guid,  string printby)
        //{
        //    ResponseData<TaxInvoiceResource> response = new ResponseData<TaxInvoiceResource>();

        //    try
        //    {
        //        var taxInvoiceData = await _taxInvoiceService.GetReportData(guid, printby);

        //        if (!taxInvoiceData.Success)
        //        {
        //            return BadRequest(new ErrorResource(taxInvoiceData.Message));
        //        }          

        //        var resource = _mapper.Map<TaxInvoice, TaxInvoiceResource>(taxInvoiceData.Resource);
        //        //response.Data = taxInvoiceData.Resource; 
        //        response.Data = resource;
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.Message = "Success";

        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error", ex);
        //        //response.IsSuccess = false;
        //        string strStackTrace = ex.StackTrace;
        //        while (ex.InnerException != null) ex = ex.InnerException;
        //        response.Message = ex.Message + Environment.NewLine + strStackTrace;
        //        return BadRequest(response);
        //    }
        //}

        [HttpPost("PrintPdf")]
        [ProducesResponseType(typeof(TaxInvoiceResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> PrintPdf(TaxInvoiceRequest req)
        {
            TaxInvoiceResponse response = new TaxInvoiceResponse();
            try
            {
                response = await _taxInvoiceService.GetTaxInvoice2(req);
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetTaxInvoice")]
        [ProducesResponseType(typeof(TaxInvoiceResource), 200)]
        [ProducesResponseType(typeof(ErrorResource), 400)]
        public async Task<IActionResult> GetTaxInvoice(string guid, string empcode)
        {
            TaxInvoiceHd response = new TaxInvoiceHd();

            try
            {
                response = await _taxInvoiceService.GetTaxInvoice(guid, empcode);
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                return BadRequest(ex.Message);
            }
        }


        //[HttpGet("GetTaxInvoiceList")]
        //[ProducesResponseType(typeof(TaxInvoiceResource), 200)]
        //[ProducesResponseType(typeof(ErrorResource), 400)]
        //public async Task<IActionResult> GetTaxInvoiceList(string docno, string printby)
        //{
        //    ResponseData<List<TaxInvoiceResource>> response = new ResponseData<List<TaxInvoiceResource>>();

        //    try
        //    {
        //        var taxInvoiceData = await _taxInvoiceService.GetTaxInvoiceListAsync(docno, printby);
        //        var resource = _mapper.Map<List<TaxInvoice>, List<TaxInvoiceResource>>(taxInvoiceData.Resource).ToList();
        //        response.Data = resource;
        //        response.StatusCode = StatusCodes.Status200OK;
        //        response.Message = "Success";

        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error", ex);
        //        return BadRequest(ex.Message);
        //    }
        //}

    }
}
