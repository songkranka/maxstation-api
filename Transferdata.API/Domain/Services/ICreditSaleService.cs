using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Resources;

namespace Transferdata.API.Domain.Services
{

    public interface ICreditSaleService
    {
        Task<List<SalCreditsaleHd>> ListCrditSaleAsync(CreditsaleQuery query);        
        Task<LogResource> SaveListAsync(List<SalCreditsaleHd> creditsalelist);

        //Task<SalCreditsaleHd> UpdateRemainQuotation(SalCreditsaleHd obj);
    }
}
