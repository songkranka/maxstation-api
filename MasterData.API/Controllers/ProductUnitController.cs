using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services;
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
    public class ProductUnitController : BaseController
    {

        private readonly IProductUnitService _productUnitService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        public ProductUnitController(
            IProductUnitService productUnitService,
            PTMaxstationContext context) : base(context)

        {
            _productUnitService = productUnitService;
        }

        [HttpPost("GetProductUnitList")]
        public IActionResult GetProductUnitList([FromBody] ProductUnitRequest req)
        {
            ResponseData<List<MasProductUnit>> response = new ResponseData<List<MasProductUnit>>();
            try
            {
                response.Data = _productUnitService.GetProductUnitList(req);
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
    }
}
