using Inventory.API.Domain.Models;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Services
{
    public interface IRequestService
    {
        List<InvRequestHds> GetRequestHDList(RequestQueryResource query);
        InvRequest GetRequest(RequestQueryResource req);
        Task<InvRequest> CreateRequest(InvRequest obj);
        Task<InvRequest> UpdateRequest(InvRequest obj);
        Task<List<InvRequestHds>> GetRequestHDListNew(RequestData req);
        Task<int> GetRequestHDCount(RequestData req);
    }
}
