using MasterData.API.Domain.Models.Queries;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface IReasonService
    {
        Task<QueryResult<MasReason>> ListAsync(ReasonQuery query);
        Task<QueryResult<MasReason>> WithdrawListAsync(WithdrawQuery query);
        Task<QueryResult<MasReason>> ProductReasonListAsync(ProductReasonQuery query);
    }
}
