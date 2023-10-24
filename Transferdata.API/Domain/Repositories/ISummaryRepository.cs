using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Repositories
{
    public interface ISummaryRepository
    {
        Task<List<SaleEngineOil>> ListCashsaleSummaryDiscAsync(SummaryQuery query);
    }
}
