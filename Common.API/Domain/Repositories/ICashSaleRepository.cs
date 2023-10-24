using Common.API.Domain.Models.Queries;
using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.API.Domain.Repositories
{
    public interface ICashSaleRepository
    {
        Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query);
    }
}
