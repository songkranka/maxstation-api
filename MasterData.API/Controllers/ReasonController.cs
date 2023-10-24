using AutoMapper;
using log4net;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Services;
using MasterData.API.Resources;
using MasterData.API.Resources.Reason;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Controllers.Reason
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonController : BaseController
    {
        readonly IReasonService _reasonService;
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));

        public ReasonController(
            IReasonService reasonService,
            IMapper mapper,
            PTMaxstationContext context) : base(context)
        {
            _reasonService = reasonService;
            _mapper = mapper;
        }

        [HttpPost("WithdrawList")]
        [ProducesResponseType(typeof(QueryResultResource<MasReason>), 200)]
        public async Task<QueryResult<MasReason>> WithdrawList([FromBody] WithdrawQueryResource query)
        {
            var reasonQuery = _mapper.Map<WithdrawQueryResource, WithdrawQuery>(query);
            var resource = await _reasonService.WithdrawListAsync(reasonQuery);
            return resource;
        }

        [HttpPost("ProductReasonList")]
        [ProducesResponseType(typeof(QueryResultResource<MasReason>), 200)]
        public async Task<QueryResult<MasReason>> ProductReasonList([FromBody] ProductReasonQueryResource query)
        {
            var reasonQuery = _mapper.Map<ProductReasonQueryResource, ProductReasonQuery>(query);
            var resource = await _reasonService.ProductReasonListAsync(reasonQuery);
            return resource;
        }

        /// <summary>
        /// Lists all reason.
        /// </summary>
        /// <returns>List of reason.</returns>
        [HttpPost("GetReasonList")]
        public async Task<IActionResult> GetReasonListAsync([FromBody] ReasonQuery req)
        {
            ResponseData<QueryResult<MasReason>> response = new ResponseData<QueryResult<MasReason>>();
            try
            {
                response.Data =  await _reasonService.ListAsync(req);
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
