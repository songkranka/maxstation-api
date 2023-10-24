using MaxStation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Services
{
    public interface ITaxInvoiceService
    {
        Task<List<SalTaxinvoiceHd>> ListTaxInvoiceAsync(TaxInvoiceQuery query);

        //Task<SalCreditsaleHd> UpdateRemainQuotation(SalCreditsaleHd obj);
    }
}
