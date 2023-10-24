using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sal.API.Controllers;
using Sale.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CheckerController : BaseController
    {
        private readonly ICashTaxService _cashTaxService;
        private readonly IMapper _mapper;

        public CheckerController(
            ICashTaxService cashTaxService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _cashTaxService = cashTaxService;
            _mapper = mapper;
        }

        [HttpGet("Get")]
        public IActionResult Get()
        {
            return Ok($"Max Station : Get Action");
        }


        [HttpPost("Post")]
        public IActionResult Post([FromBody] string data)
        {
            return Ok($"Max Station : Post = {data}");
        }

    }
}
