using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Domain.Repositories;
using Transferdata.API.Domain.Services;

namespace Transferdata.API.Services
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IWithdrawRepository _withdrawRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private PTMaxstationContext Context;

        public WithdrawService(
            IWithdrawRepository withdrawRepository,
            IUnitOfWork unitOfWork,
            PTMaxstationContext context,
            IServiceScopeFactory serviceScopeFactory)
        {
            _withdrawRepository = withdrawRepository;
            _unitOfWork = unitOfWork;
            Context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }


        public async Task<List<InvWithdrawHd>> ListWithdrawAsync(WithdrawQuery query)
        {
            return await this._withdrawRepository.ListWithdrawAsync(query);
        }
    }
}
