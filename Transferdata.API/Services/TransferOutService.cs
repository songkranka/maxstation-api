using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Services
{

    public class TransferOutService : ITransferOutService
    {

        private readonly ITransferOutRepository _transoutRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public TransferOutService(
            ITransferOutRepository transoutRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _transoutRepository = transoutRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<List<InvTranoutHd>> ListTransferOutAsync(TransferOutQuery query)
        {
            return await this._transoutRepository.ListTransferOutAsync(query);
        }
    }
}
