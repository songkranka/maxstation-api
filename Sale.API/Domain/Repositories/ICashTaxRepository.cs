using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Models.Request;
using Sale.API.Resources.CashTax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Repositories
{
    public interface ICashTaxRepository
    {
        int GetRunNumber(SalTaxinvoiceHd cashTaxHd);
        Task<QueryResult<SalTaxinvoiceHd>> ListAsync(CashTaxHdQuery query);
        Task<CashTax> FindByIdAsync(Guid guid);
        Task<List<SalTaxinvoiceDt>> GetCashTaxDtByDocNoAsync(string docNo);
        Task AddHdAsync(SalTaxinvoiceHd cashTaxHd);
        Task AddDtAsync(SalTaxinvoiceDt cashTaxDt);
        void UpdateAsync(SalTaxinvoiceHd cashtaxHd);
        void AddDtListAsync(IEnumerable<SalTaxinvoiceDt> cashtaxDt);
        void RemoveDtAsync(IEnumerable<SalTaxinvoiceDt> cashtaxdt);
        Task CancelAndReplace(CashTaxCancelAndReplaceRequest pInput);
        Task<FinBalance> GetFinBalanceByCashTax(FinBalanceByCasTaxRequest pCashTax);
        Task<MasCustomer> GetCustomerByCustCode(CustomerByCustCodeRequset request);
    }
}
