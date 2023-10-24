using MaxStation.Entities.Models;
using Sale.API.Domain.Repositories;
using Sale.API.Domain.Services;
using Sale.API.Domain.Services.Communication;
using Sale.API.Resources;
using Sale.API.Resources.CreditNote;
using Sale.API.Resources.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Services
{
    public class CreditNoteService : ICreditNoteService
    {
        ICreditNoteRepository _repo = null;
        IUnitOfWork _unitOfWork = null;
        public CreditNoteService(ICreditNoteRepository repo , IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueryResultResource<SalCndnHd>> ListAsync(CreditNoteQueryResource query)
        {
            return await _repo.ListAsync(query);
        }

        public async Task<CreditNoteQueryResource2> GetCreditNote(String pStrGuid)
        {
            return await _repo.GetCreditNote(pStrGuid);
        }

        public async Task SaveCreditNote(CreditNoteQueryResource2 pInput)
        {
            await _repo.SaveCreditNote(pInput);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<SalTaxinvoiceHd[]> GetTaxInvoiceList(CreditNoteQueryResource2 pInput)
        {
            return await _repo.GetTaxInvoiceList(pInput);
        }
        public async Task<SalTaxinvoiceDt[]> GetTaxInvoiceDetailList(SalTaxinvoiceHd pInput)
        {
            return await _repo.GetTaxInvoiceDetailList(pInput);
        }

        public async Task<SearchTaxInvoiceResult> SearchTaxInvoice(SearchTaxInvoiceParam param)
        {
            return await _repo.SearchTaxInvoice(param);
        }
    }
}
