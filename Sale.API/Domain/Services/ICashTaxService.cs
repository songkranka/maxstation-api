using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Models.Request;
using Sale.API.Domain.Services.Communication;
using Sale.API.Resources.CashTax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services
{
    public interface ICashTaxService
    {
        Task<QueryResult<SalTaxinvoiceHd>> ListAsync(CashTaxHdQuery query);
        Task<CashTax> FindByIdAsync(CashTaxHdQuery query);
        Task<CashTaxHdResponse> SaveAsync(CashTax cashTax);
        Task<CashTaxHdResponse> UpdateAsync(Guid guid, CashTax cashtax);
        Task CancelAndReplace(CashTaxCancelAndReplaceRequest pInput);
        Task<FinBalance> GetFinBalanceByCashTax(FinBalanceByCasTaxRequest pCashTax);
        Task<MasCustomer> GetCustomerByCustCode(CustomerByCustCodeRequset request);
    }
}
