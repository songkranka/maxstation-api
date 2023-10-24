using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.Adjust;
using Inventory.API.Resources.SupplyTransferIn;
using MaxStation.Entities.Models;
using System;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface ISupplyTransferInService
    {
        Task<QueryResult<InvSupplyRequestHd>> GetSupplyTransferInHDList(SupplyTransferInQueryResource query);
        //Task<AdjustResponse> CreateAsync(InvAdjustHd AdjustHd);
        //Task<InvAdjustHd> FindByIdAsync(Guid guid);
        //Task<AdjustResponse> UpdateAsync(Guid guid, InvAdjustHd AdjustHd);
    }
}
