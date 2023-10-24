using Inventory.API.Domain.Models;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Repositories
{
    public interface IRequestRepository
    {
        List<InvRequestHds> GetRequestHDList(RequestQueryResource req);
        InvRequest GetRequest(RequestQueryResource req);
        int GetRunNumber(InvRequest obj);
        Task CreateRequest(InvRequest obj);
        Task UpdateRequest(InvRequest obj);
        Task<List<InvRequestHds>> GetRequestHdListNew(RequestData req);
        Task<int> GetRequestHdCount(RequestData req);
        decimal CalculateStockQty(string pdId, string unitId, decimal itemQty);
    }
}
