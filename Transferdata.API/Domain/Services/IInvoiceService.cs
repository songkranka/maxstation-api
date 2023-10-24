using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;
using Transferdata.API.Resources;

namespace Transferdata.API.Domain.Services
{
    public interface IInvoiceService
    {

        Task<LogResource> SaveListAsync(List<SalCreditsaleHd> invoicelist);

        //Task<SalCreditsaleHd> UpdateRemainQuotation(SalCreditsaleHd obj);
    }
}
