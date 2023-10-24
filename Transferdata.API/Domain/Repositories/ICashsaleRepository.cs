using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Repositories
{
    public interface ICashsaleRepository
    {
        Task SaveListAsync(List<SalCashsaleHd> cashsalelist);
        bool CheckExistsPOS(SalCashsaleHd obj);
        Task AddHdAsync(SalCashsaleHd cashSaleHd);
        Task AddDtAsync(SalCashsaleDt cashSaleDt);

        Task AddLogAsync(SalCashsaleLog cashSaleLog);

        Task<List<SalCashsaleHd>> ListCashsaleAsync(CashsaleQuery query);

        Task<List<CashsaleDisc>> ListCashsaleSummaryDiscAsync(SummaryQuery query);

        Task<List<SaleNonOil>> ListCashsaleNonOilAmountAsync(SummaryQuery query);
        

    }
}
