using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Repositories
{


    public interface ICreditsaleRepository
    {
        bool CheckExistsPOS(SalCreditsaleHd obj);
        Task AddHdAsync(SalCreditsaleHd creditSaleHd);
        Task AddDtAsync(SalCreditsaleDt creditSaleDt);

        Task AddLogAsync(SalCreditsaleLog creditSaleLog);


        Task<List<SalCreditsaleHd>> ListCreditsaleAsync(CreditsaleQuery query);

        Task<List<CreditsaleAmount>> ListOilAmountAsync(SummaryQuery query);

        Task<List<CreditsaleAmount>> ListCreditsaleAmountAsync(SummaryQuery query);

        //Task<SalCreditsaleHd> UpdateRemainQuotation(SalCreditsaleHd obj);
    }
}
