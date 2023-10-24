using MaxStation.Entities.Models;
using PostDay.API.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Services
{
    public interface ILoadTestService
    {
        Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query);
    }
}
