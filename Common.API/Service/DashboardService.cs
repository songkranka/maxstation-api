using Common.API.Domain.Models;
using Common.API.Domain.Repositories;
using Common.API.Domain.Services;
using Common.API.Resource;
using Common.API.Resources;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Common.API.Services
{
    public class DashboardService : IDashboardService
    {
        readonly IDashboardRepository _dashboardRepositories;

        public DashboardService(IDashboardRepository dashboardRepositories)
        {
            _dashboardRepositories = dashboardRepositories;
        }

        public Task<QueryObjectResource<ProductDisplayResponse>> GetProductDisplay(RequestGetRequestList req)
        {
            return _dashboardRepositories.GetProductDisplay(req);
        }

        public Task<QueryResultResource<InvRequestHd>> GetRequestList(RequestGetRequestList req)
        {
            return _dashboardRepositories.GetRequestList(req);
        }

        public Task<QueryResultResource<InvTranoutHd>> GetTransferOutList(RequestGetRequestList req)
        {
            return _dashboardRepositories.GetTransferOutList(req);
        }
    }
}
