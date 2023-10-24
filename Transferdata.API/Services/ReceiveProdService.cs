using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Services
{
    public class ReceiveProdService : IReceiveProdService
    {

        private readonly IReceiveProdRepository _receiveProdRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public ReceiveProdService(
            IReceiveProdRepository receiveProdRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _receiveProdRepository = receiveProdRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<InvReceiveProdHd>> ListReceiveGasAsync(ReceiveProdQuery query)
        {
            return await this._receiveProdRepository.ListReceiveGasAsync(query);
        }

        public async Task<List<InvReceiveProdHd>> ListReceiveOilAsync(ReceiveProdQuery query)
        {
            return await this._receiveProdRepository.ListReceiveOilAsync(query);
        }

        public async Task<List<InvReceiveProdHd>> ListReceiveProdAsync(ReceiveProdQuery query)
        {
            return await this._receiveProdRepository.ListReceiveProdAsync(query);
        }
    }
}
