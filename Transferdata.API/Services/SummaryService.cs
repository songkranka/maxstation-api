using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly ICashsaleRepository _cashsaleRepository;
        private readonly ICreditsaleRepository _creditsaleRepository;
        private readonly ISummaryRepository _summaryRepository;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public SummaryService(
            ICashsaleRepository cashsaleRepository,
            ICreditsaleRepository creditsaleRepository,
            ISummaryRepository summaryRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _cashsaleRepository = cashsaleRepository;
            _creditsaleRepository = creditsaleRepository;
            _summaryRepository = summaryRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        //1
        public async Task<List<CashsaleDisc>> ListCashsaleSummaryDiscAsync(SummaryQuery query)
        {
            return await this._cashsaleRepository.ListCashsaleSummaryDiscAsync(query);
        }
        //2
        public async Task<List<SaleEngineOil>> ListSaleEngineOilSummaryDiscAsync(SummaryQuery query)
        {
            return await this._summaryRepository.ListCashsaleSummaryDiscAsync(query);
        }
        //3 Summary จำนวนเงินขายเชื่อ กลุ่มน้ำมันใส และ LPG
        public async Task<List<CreditsaleAmount>> ListCreditsaleOilAmountAsync(SummaryQuery query)
        {
            return await this._creditsaleRepository.ListOilAmountAsync(query);
        }
        //4
        public async Task<List<SaleNonOil>> ListCashsaleNonOilAmountAsync(SummaryQuery query)
        {
            return await this._cashsaleRepository.ListCashsaleNonOilAmountAsync(query);
        }
        //5
        public async Task<List<CreditsaleAmount>> ListCreditsaleAmountAsync(SummaryQuery query)
        {
            return await this._creditsaleRepository.ListCreditsaleAmountAsync(query);
        }
    }
}
