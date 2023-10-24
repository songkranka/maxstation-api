using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.AdjustRequest;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface IAdjustRequestService
    {
        Task<QueryResult<InvAdjustRequestHd>> GetAdjustRequestHDList(AdjustRequestQueryResource query);
        Task<AdjustRequestResponse> CreateAsync(InvAdjustRequestHd AdjustRequestHd);
        Task<InvAdjustRequestHd> FindByIdAsync(Guid guid);
        Task<AdjustRequestResponse> UpdateAsync(Guid guid, InvAdjustRequestHd AdjustRequestHd);
        Task<List<InvAdjustRequestDt>> GetDetailAsync(string compCode, string brnCode, string locCode, string docNo);
    }
}
