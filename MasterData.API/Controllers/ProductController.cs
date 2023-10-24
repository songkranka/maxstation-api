using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.Product;
using MasterData.API.Services;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ISoapPTWebServiceApi _soapPTWebServiceApi;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        public ProductController(
            IProductService productService,
            ISoapPTWebServiceApi soapPTWebServiceApi,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _productService = productService;
            _soapPTWebServiceApi = soapPTWebServiceApi;
            _mapper = mapper;
        }

        [HttpPost("GetProductList")]
        public IActionResult GetProductList([FromBody] ProductRequest req)
        {
            ResponseData<List<Products>> response = new ResponseData<List<Products>>();
            try
            {
                response.Data = _productService.GetProductList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetProductListWithDocumentType")]
        public IActionResult GetProductListWithDocumentType([FromBody] ProductRequest req)
        {
            ResponseData<List<Products>> response = new ResponseData<List<Products>>();
            try
            {
                response.Data = _productService.GetProductListWithDocumentType(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetProductAllTypeList")]
        public IActionResult GetProductAllTypeList([FromBody] ProductRequest req)
        {
            ResponseData<List<Products>> response = new ResponseData<List<Products>>();
            try
            {
                response.Data = _productService.GetProductAllTypeList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
                response.StatusCode = StatusCodes.Status400BadRequest;
                string strStackTrace = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                response.Message = ex.Message + Environment.NewLine + strStackTrace;
                return BadRequest(response);
            }
        }

        //============================== HttpPost ==============================

        [HttpPost("GetProductServiceList")]
        public IActionResult GetProductServiceList([FromBody] ProductRequest req)
        {
            ResponseData<List<Products>> response = new ResponseData<List<Products>>();
            try
            {
                response.Data = _productService.GetProductServiceList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetProductListWithOutMaterialCode")]
        public IActionResult GetProductListWithOutMaterialCode([FromBody] ProductRequest req)
        {
            ResponseData<List<Products>> response = new ResponseData<List<Products>>();
            try
            {
                response.Data = _productService.GetProductListWithOutMaterialCode(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("GetProductReasonList")]
        public IActionResult GetProductReasonList([FromBody] ProductReasonRequest req)
        {
            ResponseData<List<Products>> response = new ResponseData<List<Products>>();
            try
            {
                response.Data = _productService.GetProductReasonList(req);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "เรียกข้อมูลสำเร็จ";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                log.Error("Error", ex);
                return BadRequest(response);
            }
        }

        [HttpPost("FindAll")]
        [ProducesResponseType(typeof(QueryResultResource<MasProduct>), 200)]
        public async Task<QueryResult<MasProduct>> FindAllAsync([FromBody] ProductQueryResource query)
        {
            var customerCarQuery = _mapper.Map<ProductQueryResource, ProductQuery>(query);
            var resource = await _productService.FindAllAsync(customerCarQuery);
            return resource;
        }

        [HttpPost("FindById")]
        [ProducesResponseType(typeof(QueryResultResource<MasProduct>), 200)]
        public async Task<MasProduct> FindByIdAsync([FromBody] ProductResource query)
        {
            var resource = await _productService.FindByIdAsync(query);
            return resource;
        }

        [HttpPost("FindProductOilType")]
        public async Task<IActionResult> FindProductOilTypeAsync([FromBody] ProductTypeOilRequest query)
        {
            ResponseData<MasProduct> response = new ResponseData<MasProduct>();

            try
            {
                response.Data = await _productService.FindProductOilTypeAsync(query.PdId);
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

        [HttpGet("GetProduct/{pStrGuid}")]
        public async Task<IActionResult> GetProduct(string pStrGuid)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "GetProduct", 
                pFunc: async() => await _productService.GetProduct(pStrGuid), 
                pLog: log
            );
            return result;
        }


        [HttpGet("IsDuplicateProductId/{pStrProductId}")]
        public async Task<IActionResult> IsDuplicateProductId(string pStrProductId)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "IsDuplicateProductId",
                pFunc: async () => await _productService.IsDuplicateProductId(pStrProductId),
                pLog: log
            );
            return result;
        }

        [HttpGet("GetAllUnit")]
        public async Task<IActionResult> GetAllUnit()
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "GetAllUnit",
                pFunc: async () => await _productService.GetAllUnit(),
                pLog: log
            );
            return result;
        }

        [HttpPost("InsertProduct")]
        public async Task<IActionResult> InsertProduct(ModelProduct param)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "InsertProduct",
                pFunc: async () => await _productService.InsertProduct(param),
                pLog: log
            );
            return result;
        }

        [HttpPost("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(ModelProduct param)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "UpdateProduct",
                pFunc: async () => await _productService.UpdateProduct(param),
                pLog: log
            );
            return result;
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(MasProduct param)
        {
            IActionResult result = null;
            result = await DefaultService.DoActionAsync(
                pStrFunctionName: "UpdateStatus",
                pFunc: async () => await _productService.UpdateStatus(param),
                pLog: log
            );
            return result;
        }

        [HttpPost("GetProductOilType")]
        public async Task<IActionResult> GetProductOilType()
        {
            ResponseData<List<MasProduct>> response = new ResponseData<List<MasProduct>>();

            try
            {
                response.Data = await _productService.GetProductOilTypeAsync();
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

        //[HttpPost("GetOilPrice")]
        //public async Task<IActionResult> GetOilPrice([FromBody] OilPriceRequest req)
        //{
        //    ResponseData<OilPrice.Data> response = new ResponseData<OilPrice.Data>();
        //    try
        //    {
        //        var result = await _soapPTWebServiceApi.GetOilPriceByBrn(req);
        //        response.Data = result.data;
        //        response.StatusCode = int.Parse(result.response.resCode);
        //        response.Message = "เรียกข้อมูลสำเร็จ";
        //        log.Info(req);
        //        return Ok(JsonConvert.SerializeObject(response));
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = StatusCodes.Status400BadRequest;
        //        response.Message = ex.Message;
        //        log.Error("Error", ex);
        //        return BadRequest(response);
        //    }
        //}
    }
}
