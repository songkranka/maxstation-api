using MaxStation.Entities.Models;
using Sale.API.Resources;
using Sale.API.Resources.CreditNote;
using Sale.API.Resources.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Domain.Services
{
    public interface ICreditNoteService
    {
        Task<QueryResultResource<MaxStation.Entities.Models.SalCndnHd>> ListAsync(Resources.CreditNote.CreditNoteQueryResource query);
        Task<CreditNoteQueryResource2> GetCreditNote(String pStrGuid);
        Task SaveCreditNote(CreditNoteQueryResource2 pInput);
        Task<SalTaxinvoiceHd[]> GetTaxInvoiceList(CreditNoteQueryResource2 pInput);
        Task<SalTaxinvoiceDt[]> GetTaxInvoiceDetailList(SalTaxinvoiceHd pInput);
        Task<SearchTaxInvoiceResult> SearchTaxInvoice(SearchTaxInvoiceParam param);
    }
}
