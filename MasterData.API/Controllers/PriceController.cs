using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses.Price;
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
    public class PriceController : BaseController
    {
        private readonly IPriceService _priceService;
        private readonly IMapper _mapper;
        private static readonly ILog _log = LogManager.GetLogger(typeof(MenuController));
        public PriceController(
            IPriceService priceService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _priceService = priceService;
            _mapper = mapper;

        }

        /// <summary>
        /// existing comp_code, brn_code.
        /// </summary>
        /// <returns>oil price.</returns>
        [HttpPost("CurrentOilPrice")]
        public IActionResult GetCurrentOilPriceAsync(OilPriceRequest request)
        {
            ResponseData<List<OilPrice>> response = new ResponseData<List<OilPrice>>();

            try
            {
                response.Data =  _priceService.GetCurrentOilPriceAsync(request.compCode, request.brnCode, request.systemDate);
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
    }
}
