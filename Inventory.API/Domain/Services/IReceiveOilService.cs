using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Services.Communication;
using Inventory.API.Resources.ReceiveOil;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface IReceiveOilService
    {
        Task<QueryResult<InvReceiveProdHd>> ListAsync(ReceiveOilHdQuery query);
        Task<ReceiveOil> FindByIdAsync(Guid guid);
        Task<ReceiveOilResponse> SaveAsync(SaveReceiveOilResource cashTax);
        Task<ReceiveOilResponse> UpdateStatusAsync(SaveReceiveOilResource cashTax);
    }
}
