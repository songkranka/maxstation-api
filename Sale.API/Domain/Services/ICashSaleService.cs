using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Services.Communication;
using Sale.API.Resources.CashSale;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services
{
    public interface ICashSaleService
    {
        Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query);
        Task<QueryResult<SalCashsaleHd>> ListActiveAsync(CashSaleHdQuery query);
        Task<CashSale> FindByIdAsync(CashSaleHdQuery query);
        Task<CashsaleHdResponse> SaveAsync(CashSale cashsale);
        Task<CashsaleHdResponse> SaveListAsync(List<SalCashsaleHd> cashsaleList);
        Task<CashsaleHdResponse> UpdateAsync(Guid guid, CashSale cashsale);
        [Obsolete("use GetQuotationListByCashSale instead")]
        Task<List<SalQuotationHd>> GetQuotation();
        Task<QuotationDetail[]> GetQuotationDetail(SalQuotationHd pQuotationHeader);
        Task<List<SalQuotationHd>> GetQuotationListByCashSale(SalCashsaleHd pCashSale);
        Task SaveCashSale2(CashSaleResource2 pInput);
        Task<CashSaleResource2> GetCashSale2(string pStrGuid);
    }
}
