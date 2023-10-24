using AutoMapper;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Report.API.Domain.Models;
using Report.API.Domain.Models.Queries;
using Report.API.Domain.Services;
using Report.API.Resources;
using Report.API.Resources.SysReportConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportSummaryOilBalanceController));
        private readonly IMapper _mapper;

        public ReportController(
            IReportService reportService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)

        {
            _reportService = reportService;
            _mapper = mapper;
        }

        [HttpPost("FindReportConfigByGroup")]
        [ProducesResponseType(typeof(QueryResultResource<SysReportConfig>), 200)]
        public async Task<QueryResult<SysReportConfig>> FindReportConfigByGroupAsync([FromBody] SysReportConfigQueryResource query)
        {
            var reportConfigQuery = _mapper.Map<SysReportConfigQueryResource, SysReportConfigQuery>(query);
            var resource = await _reportService.FindReportConfigByGroup(reportConfigQuery);
            return resource;
        }

        [HttpPost("FindReportConfig")]
        [ProducesResponseType(typeof(SysReportConfig), 200)]
        public async Task<IActionResult> FindReportConfig([FromBody] SysReportConfigByGroupQuery query)
        {
            ResponseData<SysReportConfig> response = new ResponseData<SysReportConfig>();

            try
            {
                response.Data = await _reportService.FindReportConfig(query);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Success";
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.Error($"CreditList : {ex.StackTrace}");
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
