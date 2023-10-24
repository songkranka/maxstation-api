using Inventory.API.Domain.Models.Queries;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Repositories
{
    public interface IWithdrawRepository
    {
        Task<QueryResult<InvWithdrawHd>> ListAsync(WithdrawQuery query);
        Task<InvWithdrawHd> FindByIdAsync(Guid guid, string compcode, string brnCode, string locCode);
        Task<List<InvWithdrawDt>> GetDetailAsync(string compCode,string brnCode,string locCode,string docNo);
        Task<List<InvWithdrawDt>> GetDetailByDocNoAsync(string docNo);
        int GetRunNumber(string compCode, string brnCode, string pattern);
        decimal CalculateStockQty(string pdId, string unitId, decimal itemQty);
        string GenDocNo(InvWithdrawHd withdrawHd, string pattern, int runNumber);
        Task AddHdAsync(InvWithdrawHd withdrawHd);
        Task AddDtAsync(List<InvWithdrawDt> withdrawDts);
        void UpdateAsync(InvWithdrawHd withdrawHd);
        Task CancelAsync(InvWithdrawHd withdrawHd);
        void AddDetailAsync(IEnumerable<InvWithdrawDt> withdrawDts);
        void RemoveDetailAsync(IEnumerable<InvWithdrawDt> withdrawDts);
        Task<MasReason[]> GetReasons();
        Task<MasReasonGroup[]> GetReasonGroups(string pStrReasonId);
        Task<bool> IsDupplicateDocNo(InvWithdrawHd pHeader, string pStrDocNo);
        Task<bool> CheckPOSWater(InvWithdrawHd withdraw);
    }
}
