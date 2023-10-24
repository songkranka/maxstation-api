using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Repositories
{
    public interface ICreditSaleRepository
    {
        int GetRunNumber(SalCreditsaleHd creditSaleHd);
        Task<QueryResult<SalCreditsaleHd>> ListAsync(RequestData req);
        Task AddHdAsync(SalCreditsaleHd creditSaleHd);
        Task AddDtAsync(SalCreditsaleDt creditSaleDt);
        Task<SalCreditsaleHd> FindByIdAsync(Guid guid);
        void UpdateAsync(SalCreditsaleHd cashsaleHd);
        void RemoveDtAsync(IEnumerable<SalCreditsaleDt> creditSaleDt);
        void AddDtListAsync(IEnumerable<SalCreditsaleDt> cashsaleDt);
        Task UpdateRemainQuotation(SalCreditsaleHd obj);
        decimal CalculateStockQty(string pdId, decimal itemQty);
        Task ReturnRemainQuotation(SalCreditsaleHd obj);
        Task<List<SalQuotationDt>> CheckRemainQuotation(SalCreditsaleHd obj);
        Task<int> CheckDataDuplicate(SalCreditsaleHd obj);
        Task<List<MasCustomerCar>> GetCustomerCar(string pStrCusCode);
        List<Products> GetProductListWithOutMaterialCode(ProductRequest req);
    }
}
