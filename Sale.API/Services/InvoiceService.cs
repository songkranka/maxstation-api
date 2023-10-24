using MaxStation.Entities.Models;
using Sale.API.Domain.Repositories;
using Sale.API.Domain.Services;
using Sale.API.Domain.Services.Communication;
using Sale.API.Resources;
using Sale.API.Resources.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        IInvoiceRepository _repo = null;
        IUnitOfWork _unitOfWork = null;
        public InvoiceService(IInvoiceRepository repo , IUnitOfWork pUnitOfWork)
        {
            _repo = repo;
            _unitOfWork = pUnitOfWork;
        }

        public async Task<InsertCreditSalesQuery> GetCreditSales(GetInvoiceQueryResource param)
        {
            return await _repo.GetCreditSales(param);
        }

        public async Task<List<InvoiceDropdownResponse>> GetProductService()
        {
            return await _repo.GetProductService();
        }

        public async Task<List<MasProduct>> GetProductService2()
        {
            return await _repo.GetProductService2();
        }

        public async Task<string> GetRunningDocNo(string pStrCompanyCode, string pStrBranchCode, string pStrLocationCode)
        {
            return await _repo.GetRunningDocNo(pStrCompanyCode, pStrBranchCode, pStrLocationCode);
        }

        public async Task<string> GetRunningNo(string pStrCompanyCode, string pStrLocationCode, string pStrBranchCode)
        {
            return await _repo.GetRunningNo(pStrCompanyCode, pStrLocationCode, pStrBranchCode);
        }

        public async Task<string> GetRunningPattern(string pStrCompanyCode, string pStrLocationCode, string pStrBranchCode)
        {
            return await _repo.GetRunningPattern(pStrCompanyCode, pStrLocationCode, pStrBranchCode);
        }

        public async Task InsertCreditSales(SalCreditsaleHd pCreditSaleHeader, SalCreditsaleDt[] pArrCreditSaleDetail)
        {
            await _repo.InsertCreditSales(pCreditSaleHeader, pArrCreditSaleDetail);
        }

        public async Task<QueryResultResource<SalCreditsaleHd>> ListAsync(InvoiceQueryResource query)
        {
            return await _repo.ListAsync(query);
        }

        public List<SalCreditsaleDt> Test()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCreditSales(SalCreditsaleHd pCreditSaleHeader, SalCreditsaleDt[] pArrCreditSaleDetail)
        {
            await _repo.UpdateCreditSales( pCreditSaleHeader,  pArrCreditSaleDetail);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<InsertCreditSalesQuery> GetCreditSalesByGuid(string pStrGuid)
        {
            return await _repo.GetCreditSalesByGuid(pStrGuid);
        }
    }
}
