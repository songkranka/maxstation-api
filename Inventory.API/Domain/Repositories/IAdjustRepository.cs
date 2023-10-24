using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Resources.Adjust;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Repositories
{
    public interface IAdjustRepository
    {
        Task<QueryResult<InvAdjustHd>> GetAdjustHDList(AdjustQueryResource req);
        int GetRunNumber(InvAdjustHd AdjustHd);
        string GenDocNo(InvAdjustHd AdjustHd, int runNumber);
        Task AddHdAsync(InvAdjustHd AdjustHd);
        Task AddDtAsync(List<InvAdjustDt> AdjustDt);
        Task<InvAdjustHd> FindByIdAsync(Guid guid);
        Task<List<InvAdjustDt>> GetDetailAsync(string compCode, string brnCode, string locCode, string docNo);
        void UpdateAsync(InvAdjustHd AdjustHd);
        void AddDetailAsync(IEnumerable<InvAdjustDt> AdjustDts);
        void RemoveDetailAsync(IEnumerable<InvAdjustDt> AdjustDts);
        Task<string> GetBranchName(string brnCode);
        decimal CalculateStockQty(string pdId, decimal itemQty);
        void RemainAdjustRequest(InvAdjustHd AdjustHd);

        Task<MasReason[]> GetReasonAdjusts();
    }
}
