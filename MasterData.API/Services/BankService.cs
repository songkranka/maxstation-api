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
    public class BankService : IBankService
    {
        private readonly IBankRepository bankRepo;

        public BankService(
            IBankRepository bankRepository)
        {
            bankRepo = bankRepository;
        }

        public async Task<QueryResult<MasBank>> ListAsync(BankQuery query)
        {
            return await bankRepo.ListAsync(query);
        }
    }
}
