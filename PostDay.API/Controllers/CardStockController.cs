using System;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PostDay.API.Domain.Models.PostDay;
using PostDay.API.Domain.Services;

namespace PostDay.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CardStockController : BaseController
    {
        private readonly ICardStockService _cardStockService;

        public CardStockController(ICardStockService cardStockService, PTMaxstationContext context) : base(context)
        {
            _cardStockService = cardStockService;
        }

        [HttpPost("GetCardStock")]
        public async Task<IActionResult> GetCardStock([FromBody] CardStockRequest request)
        {
            try
            {
                var result = await _cardStockService.GetCardStock(request);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
