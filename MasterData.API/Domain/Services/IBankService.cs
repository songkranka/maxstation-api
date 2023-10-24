using MasterData.API.Domain.Models.Queries;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IBankService
    {
        Task<QueryResult<MasBank>> ListAsync(BankQuery query);
    }
}
