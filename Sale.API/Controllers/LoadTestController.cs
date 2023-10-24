using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sal.API.Controllers;
using Sale.API.Domain.Models.Queries;
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
    public class LoadTestController : BaseController
    {

        private readonly ICashSaleService _cashSaleService;
        private readonly IMapper _mapper;
        private const string _appJson = "application/json";
        private static readonly ILog log = LogManager.GetLogger(typeof(CashSaleController));
        public LoadTestController(
            ICashSaleService cashSaleService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _cashSaleService = cashSaleService;
            _mapper = mapper;
        }




    }
}
