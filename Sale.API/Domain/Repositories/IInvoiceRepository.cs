using Sale.API.Resources;
using Sale.API.Resources.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Task<QueryResultResource<MaxStation.Entities.Models.SalCreditsaleHd>> ListAsync(Resources.Invoice.InvoiceQueryResource query);
        Task<List<Domain.Services.Communication.InvoiceDropdownResponse>> GetProductService();
        Task<List<MaxStation.Entities.Models.MasProduct>> GetProductService2();
        Task<string> GetRunningDocNo(string pStrCompanyCode, string pStrBranchCode, string pStrLocationCode);
        List<MaxStation.Entities.Models.SalCreditsaleDt> Test();
        Task InsertCreditSales(MaxStation.Entities.Models.SalCreditsaleHd pCreditSaleHeader, MaxStation.Entities.Models.SalCreditsaleDt[] pArrCreditSaleDetail);
        Task UpdateCreditSales(MaxStation.Entities.Models.SalCreditsaleHd pCreditSaleHeader, MaxStation.Entities.Models.SalCreditsaleDt[] pArrCreditSaleDetail);
        Task<string> GetRunningPattern(string pStrCompanyCode, string pStrLocationCode, string pStrBranchCode);
        Task<string> GetRunningNo(string pStrCompanyCode, string pStrLocationCode, string pStrBranchCode);
        Task<InsertCreditSalesQuery> GetCreditSales(GetInvoiceQueryResource param);
        public Task<InsertCreditSalesQuery> GetCreditSalesByGuid(string pStrGuid);
    }
}
