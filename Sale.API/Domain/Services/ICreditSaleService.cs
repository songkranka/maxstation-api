using MaxStation.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services
{
    public interface ICreditSaleService
    {
        Task<QueryResult<SalCreditsaleHd>> ListAsync(RequestData req);
        Task<SalCreditsaleHd> FindByIdAsync(RequestData req);
        Task<ResponseService<SalCreditsaleHd>> SaveAsync(SalCreditsaleHd obj, IServiceScopeFactory serviceScopeFactory);
        Task<ResponseService<SalCreditsaleHd>> UpdateAsync(SalCreditsaleHd obj, IServiceScopeFactory serviceScopeFactory);
        Task<ResponseService<List<SalCreditsaleHd>>> SaveListAsync(List<SalCreditsaleHd> creditSaleList, IServiceScopeFactory serviceScopeFactory);
        Task<List<MasCustomerCar>> GetCustomerCar(string pStrCusCode);
        List<Products> GetProductListWithOutMaterialCode(ProductRequest req);
        Task<MasCompanyCar[]> GetCompCar(string pStrCustcode);
    }
}
