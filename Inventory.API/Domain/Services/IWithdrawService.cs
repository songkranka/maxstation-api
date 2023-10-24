using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.ReceiveOil;
using Inventory.API.Resources.Withdraw;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface IWithdrawService
    {        
        Task<InvWithdrawHd> FindByIdAsync(Guid guid, string compcode, string brnCode, string locCode);
        Task<QueryResult<InvWithdrawHd>> ListAsync(WithdrawQuery query);
        Task<WithdrawResponse> CreateAsync(InvWithdrawHd withdraw);
        Task<WithdrawResponse> UpdateAsync(Guid guid,InvWithdrawHd withdraw);
        Task<WithdrawResponse> CancelAsync(Guid guid,InvWithdrawHd withdraw);
        Task<MasReason[]> GetReasons();
        Task<MasReasonGroup[]> GetReasonGroups(string pStrReasonId);        
    }
}
