using Inventory.API.Domain.Models.Queries;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Repositories
{
    public interface IReceiveGasRepository
    {
        Task<InfPoHeader[]> GetPoHeaderList(PoHeaderListQuery pQuery);
        Task<PoItemListResult> GetPoItemList(PoItemListParam param);
        Task<QueryResult<InvReceiveProdHd>> GetReceiveList(ReceiveGasListQuery pQuery);
        Task<ReceiveGasQuery> GetReceive(string pStrGuid);
        Task<ReceiveGasQuery> SaveReceive(ReceiveGasQuery pQuery);
        Task UpdateStatus(InvReceiveProdHd pInput);
        Task<MasSupplier[]> GetArraySupplier();
        Task<bool> CheckReceiveStatus(ReceiveGasQuery saveReceiveGas);
    }
}
