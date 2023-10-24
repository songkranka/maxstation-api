using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Domain.Services
{
    public interface ITaxInvoiceService
    {
       // Task<TaxInvoice> GetReportData(string guid, string printby);

        Task<TaxInvoiceHd> GetTaxInvoice(string guid, string empcode);
        Task<TaxInvoiceResponse> GetTaxInvoice2(TaxInvoiceRequest request);
        //Task<TaxInvoiceListResponse> GetTaxInvoiceListAsync(string docno, string printby);
        //Task<Demo> Demo();
    }
}
