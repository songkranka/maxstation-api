using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Repositories
{
    public interface ICustomerRepository
    {
        List<MasCustomer> GetCustomerList(CustomerRequest req);
        Task<QueryResult<MasCustomer>> ListAsync(CustomerQuery query);
        Task<MasCustomer> GetCustomerFromCitizenId(string pStrCitizenId);
        Task<QueryResult<MasCustomer>> FindAllAsync(CustomerQuery query);
    }
}
