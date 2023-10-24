using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.ProductGroup;
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
    public class ProductGroupController : BaseController
    {
        private readonly IProductGroupService _productGroupService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        public ProductGroupController(
            IProductGroupService productGroupService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _productGroupService = productGroupService;
            _mapper = mapper;
        }

        [HttpPost("GetProductGroupList")]
        public IActionResult GetProductGroupList([FromBody] ProductGroupRequest req)
        {
            ResponseData<List<MasProductGroup>> response = new ResponseData<List<MasProductGroup>>();
            try
            {
                response.Data = _productGroupService.GetProductGroupList(req);
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
        [ProducesResponseType(typeof(QueryResultResource<MasProductGroup>), 200)]
        public async Task<QueryResult<MasProductGroup>> FindAllAsync([FromBody] ProductGroupQueryResource query)
        {
            var customerCarQuery = _mapper.Map<ProductGroupQueryResource, ProductGroupQuery>(query);
            var resource = await _productGroupService.FindAllAsync(customerCarQuery);
            return resource;
        }
    }
}
