using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Repositories;
using MasterData.API.Domain.Services;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
    public class ReasonService : IReasonService
    {
        private readonly IReasonRepository _reasonRepo;

        public ReasonService(IReasonRepository reasonRepo)
        {
            _reasonRepo = reasonRepo;
        }

        public async Task<QueryResult<MasReason>> ListAsync(ReasonQuery query)
        {
            return await _reasonRepo.ListAsync(query);
        }

        public async Task<QueryResult<MasReason>> WithdrawListAsync(WithdrawQuery query)
        {
            return await _reasonRepo.WithdrawListAsync(query);
        }
        
        public async Task<QueryResult<MasReason>> ProductReasonListAsync(ProductReasonQuery query)
        {
            return await _reasonRepo.ProductReasonListAsync(query);
        }
    }
}
