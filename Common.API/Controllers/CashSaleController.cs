using AutoMapper;
using Common.API.Domain.Models.Queries;
using Common.API.Domain.Service;
using Common.API.Resource.CashSale;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CashSaleController : BaseController
    {
        private readonly ICashSaleService _cashSaleService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(CashSaleController));

        public CashSaleController(
            ICashSaleService cashSaleService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _cashSaleService = cashSaleService;
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
            var cashSaleQuery = _mapper.Map<CashSaleHdQueryResource, CashSaleHdQuery>(query);
            var queryResult = await _cashSaleService.ListAsync(cashSaleQuery);
            return queryResult;

        }
    }
}
