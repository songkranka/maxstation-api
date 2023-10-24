using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostDay.API.Domain.Models.Queries;
using PostDay.API.Domain.Services;
using PostDay.API.Resources.CashSale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadTestController : BaseController
    {
        private readonly ILoadTestService _loadtestService;
        private readonly IMapper _mapper;
        private const string _appJson = "application/json";
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadTestController));
        public LoadTestController(
            ILoadTestService loadtestService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _loadtestService = loadtestService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lists all existing cashsalehd.
        /// </summary>
        /// <returns>List of cashSaleHd.</returns>
        [HttpPost("GetCashSaleHdList")]
        [ProducesResponseType(typeof(QueryResult<SalCashsaleHd>), 200)]
        public async Task<QueryResult<SalCashsaleHd>> CashSaleHdListAsync([FromBody] CashSaleHdQueryResource query)
        {
            log.Info($"start GetCashSaleHdList");
            Console.WriteLine($"Start GetCashSaleHdList : {DateTime.Now}", "Tests");

            var cashSaleQuery = _mapper.Map<CashSaleHdQueryResource, CashSaleHdQuery>(query);
            var queryResult = await _loadtestService.ListAsync(cashSaleQuery);

            log.Info($"finish GetCashSaleHdList : {DateTime.Now}");
            Console.WriteLine($"finish GetCashSaleHdList : {DateTime.Now}", "Tests");

            //var resource = _mapper.Map<QueryResult<SalCashsaleHd>, QueryResultResource<CashSaleHdResource>>(queryResult);
            return queryResult;
        }
    }
}
