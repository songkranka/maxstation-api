using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Resources.ReceiveOil;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Repositories
{
    public interface IReceiveOilRepository
    {
        Task<QueryResult<InvReceiveProdHd>> ReceiveOilHdListAsync(ReceiveOilHdQuery query);
        Task<ReceiveOil> FindByIdAsync(Guid guid);
        Task AddReceiveOil(SaveReceiveOilResource saveReceiveOil);
        Task UpdateStatus(SaveReceiveOilResource saveReceiveOil);
        Task<bool> CheckReceiveStatus(SaveReceiveOilResource saveReceiveOil);

    }
}
