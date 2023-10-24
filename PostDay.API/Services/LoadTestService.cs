using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using PostDay.API.Domain.Models.Queries;
using PostDay.API.Domain.Repositories;
using PostDay.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Services
{
    public class LoadTestService : ILoadTestService
    {
        private readonly ILoadTestRepository _loadtestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public LoadTestService(ILoadTestRepository loadtestRepository, IUnitOfWork unitOfWork, PTMaxstationContext context, IServiceScopeFactory serviceScopeFactory)
        {
            _loadtestRepository = loadtestRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }


        public async Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query)
        {
            return await _loadtestRepository.ListAsync(query);
        }
    }
}
