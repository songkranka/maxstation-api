using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaxStation.Entities.Models;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;

namespace Report.API.Domain.Repositories
{
    public interface ITaxInvoiceRepository
    {
        Task<TaxInvoice> FindByIdAsync(string guid,string printby);
        void UpdateSalTaxInvoiceAsync(SalTaxinvoiceHd salTaxinvoiceHd);

        Task<TaxInvoiceHd> GetTaxInvoiceAsync(string guid, string empcode);

        Task<TaxInvoiceResponse> GetTaxInvoice2Async(TaxInvoiceRequest request);

        //Task<List<TaxInvoice>> GetTaxInvoiceListAsync(string guid, string printby);
        //Task<Demo> Demo();
    }
}
