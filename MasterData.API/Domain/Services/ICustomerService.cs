using MasterData.API.Domain.Models.Queries;
using MasterData.API.Domain.Models.Request;
using MasterData.API.Domain.Models.Responses;
using MasterData.API.Resources.Customer;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Services
{
    public interface ICustomerService
    {
        List<MasCustomer> GetCustomerList(CustomerRequest req);
        Task<QueryResult<MasCustomer>> ListAsync(CustomerQuery query);
        Task<Revenue> GetTaxInfoAsync(TaxQueryResource query);
        Task<ModelCustomer> GetCustomer(string pStrGuid);
        Task<bool> CheckDuplicateCustCode(string pStrCuscode);
        Task<ModelCustomer> InsertCustomer(ModelCustomer param);
        Task<ModelCustomer> UpdateCustomer(ModelCustomer param);
        Task<MasCustomer> UpdateStatus(MasCustomer param);
        Task<ModelGetCustomerListResult> GetCustomerList2(ModelGetCustomerLisParam param);
        Task<QueryResult<MasCustomer>> FindAllAsync(CustomerQuery query);

    }
}
