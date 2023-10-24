using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Resources.CashSale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Repositories
{
    public interface ICashSaleRepository
    {
        int GetRunNumber(SalCashsaleHd cashSaleHd);
        Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query);
        Task<QueryResult<SalCashsaleHd>> ListActiveAsync(CashSaleHdQuery query);
        Task<CashSale> FindByIdAsync(Guid guid);
        Task<List<SalCashsaleDt>> GetCashSaleDtByDocNoAsync(string docNo);
        Task AddHdAsync(SalCashsaleHd cashSaleHd);
        Task AddDtAsync(SalCashsaleDt cashSaleDt);
        void UpdateAsync(SalCashsaleHd cashsaleHd);
        void UpdateStatusAsync(string refNo, string status);
        void AddDtListAsync(IEnumerable<SalCashsaleDt> cashsaleDt);
        void RemoveDtAsync(IEnumerable<SalCashsaleDt> cashsaledt);
        Task<int> CheckDataDuplicate(SalCashsaleHd obj);
        [Obsolete("use GetQuotationListByCashSale instead")]
        Task<List<SalQuotationHd>> GetQuotation();
        Task<QuotationDetail[]> GetQuotationDetail(SalQuotationHd pQuotationHeader);
        Task UpdateQuotationByCashSale(SalCashsaleHd pCashSale);
        Task<List<SalQuotationHd>> GetQuotationListByCashSale(SalCashsaleHd pCashSale);
        Task SaveCashSale2(CashSaleResource2 pInput);
        Task<CashSaleResource2> GetCashSale2(string pStrGuid);

    }
}
