using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Resources.AdjustRequest;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Repositories
{
    public interface IAdjustRequestRepository
    {
        Task<QueryResult<InvAdjustRequestHd>> GetAdjustRequestHDList(AdjustRequestQueryResource req);
        int GetRunNumber(InvAdjustRequestHd AdjustRequestHd);
        string GenDocNo(InvAdjustRequestHd AdjustRequestHd, int runNumber);
        Task AddHdAsync(InvAdjustRequestHd AdjustRequestHd);
        decimal CalculateStockQty(string pdId, string unitId, decimal itemQty);
        Task AddDtAsync(List<InvAdjustRequestDt> AdjustRequestDt);
        Task<InvAdjustRequestHd> FindByIdAsync(Guid guid);
        Task<List<InvAdjustRequestDt>> GetDetailAsync(string compCode, string brnCode, string locCode, string docNo);
        void UpdateAsync(InvAdjustRequestHd AdjustRequestHd);
        void AddDetailAsync(IEnumerable<InvAdjustRequestDt> AdjustRequestDts);
        void RemoveDetailAsync(IEnumerable<InvAdjustRequestDt> AdjustRequestDts);
    }
}
