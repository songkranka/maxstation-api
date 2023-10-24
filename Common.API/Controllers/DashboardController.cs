using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Common.API.Domain.Services;
using Common.API.Controllers;
using Common.API.Domain.Models;
using Common.API.Resources;
using Common.API.Resource;
using Microsoft.AspNetCore.Authorization;

namespace Common.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        private static readonly ILog log = LogManager.GetLogger(typeof(DashboardController));
        public DashboardController(
            IDashboardService dashboardService,
            PTMaxstationContext context) : base(context)
        {
            _dashboardService = dashboardService;
        }

        //============================== HttpPost ==============================
        [HttpPost("GetRequestList")]
        [ProducesResponseType(typeof(QueryResultResource<InvRequestHd>), 200)]
        public async Task<QueryResultResource<InvRequestHd>> GetRequestList(RequestGetRequestList req)
        {
            QueryResultResource<InvRequestHd> response = new QueryResultResource<InvRequestHd>();
            try
            {
                response = await _dashboardService.GetRequestList(req);
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        [HttpPost("GetTransferOutList")]
        [ProducesResponseType(typeof(QueryResultResource<InvTranoutHd>), 200)]
        public async Task<QueryResultResource<InvTranoutHd>> GetTransferOutList(RequestGetRequestList req)
        {
            QueryResultResource<InvTranoutHd> response = new QueryResultResource<InvTranoutHd>();
            try
            {
                response = await _dashboardService.GetTransferOutList(req);
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }

        [HttpPost("GetProductDisplay")]
        [ProducesResponseType(typeof(QueryResultResource<ProductDisplayResponse>), 200)]
        public async Task<QueryObjectResource<ProductDisplayResponse>> GetProductDisplay(RequestGetRequestList req)
        {
            QueryObjectResource<ProductDisplayResponse> response = new QueryObjectResource<ProductDisplayResponse>();
            try
            {
                response = await _dashboardService.GetProductDisplay(req);
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
