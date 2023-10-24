using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Services
{
    public class TransferInService : ITransferInService
    {

        private readonly ITransferInRepository _transinRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public TransferInService(
            ITransferInRepository transinRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _transinRepository = transinRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }


        public async Task<List<InvTraninHd>> ListTransferInAsync(TransferInQuery query)
        {
            return await this._transinRepository.ListTransferInAsync(query);
        }
    }
}
