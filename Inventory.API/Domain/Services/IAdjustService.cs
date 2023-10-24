using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.Adjust;
using MaxStation.Entities.Models;
using System;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface IAdjustService
    {
        Task<QueryResult<InvAdjustHd>> GetAdjustHDList(AdjustQueryResource query);
        Task<AdjustResponse> CreateAsync(InvAdjustHd AdjustHd);
        Task<InvAdjustHd> FindByIdAsync(Guid guid);
        Task<AdjustResponse> UpdateAsync(Guid guid, InvAdjustHd AdjustHd);
        Task<MasReason[]> GetReasonAdjusts();
    }
}
