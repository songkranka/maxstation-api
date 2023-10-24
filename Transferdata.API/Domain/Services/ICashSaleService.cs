using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Resources;
using Transferdata.API.Resources.CashSale;

namespace Transferdata.API.Domain.Services
{
    public interface ICashSaleService
    {
        Task<List<SalCashsaleHd>> ListCashSaleAsync(CashsaleQuery query);

        //Task<List<CashSaleHdResource>> SaveListAsync(List<SalCashsaleHd> cashsaleList);

        Task<LogResource> SaveListAsync(List<SalCashsaleHd> cashsalelist);
    }
}
