using AutoMapper;
using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Services;
using DailyOperation.API.Resources;
using DailyOperation.API.Resources.POS;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : BaseController
    {
        private readonly IPosService _posService;
        private readonly IQueueService _queueService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(PosController));

        public QueueController(IPosService posService, IQueueService queueService, IMapper mapper,PTMaxstationContext context) : base(context)
        {
            _posService = posService;
            _queueService = queueService;
            _mapper = mapper;
        }


        [HttpPost("TransferPOS")]
        [ProducesResponseType(typeof(QueryResultResource<POSCash>), 200)]
        public async Task<TranferPos> TransferPOS([FromBody] TranferPosResource query)
        {
            var  result = new TranferPos();

            try
            {
                result = await _queueService.TransferPOS(query);
                return result;
            }
            catch (Exception ex)
            {
                log.Error($"TranferPos : {ex.StackTrace}");
                return result;
            }
        }

    }
}
