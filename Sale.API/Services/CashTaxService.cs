using MaxStation.Entities.Models;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Models.Request;
using Sale.API.Domain.Repositories;
using Sale.API.Domain.Services;
using Sale.API.Domain.Services.Communication;
using Sale.API.Resources.CashTax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Services
{
    public class CashTaxService : ICashTaxService
    {
        private readonly ICashTaxRepository _cashTaxRepository;
        private readonly ICashSaleRepository _cashSaleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CashTaxService(
            ICashTaxRepository cashTaxRepository,
            ICashSaleRepository cashSaleRepository,
            IUnitOfWork unitOfWork)
        {
            _cashTaxRepository = cashTaxRepository;
            _cashSaleRepository = cashSaleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueryResult<SalTaxinvoiceHd>> ListAsync(CashTaxHdQuery query)
        {
            return await _cashTaxRepository.ListAsync(query);
        }

        public async Task<CashTax> FindByIdAsync(CashTaxHdQuery query)
        {
            return await _cashTaxRepository.FindByIdAsync(query.Guid);
        }

        public async Task<CashTaxHdResponse> SaveAsync(CashTax cashTax)
        {
            try
            {                
                var cashTaxHd = new SalTaxinvoiceHd
                {
                    CompCode = cashTax.CompCode,
                    BrnCode = cashTax.BrnCode,
                    LocCode = cashTax.LocCode,
                    DocNo = cashTax.DocNo,
                    DocStatus = cashTax.DocStatus,
                    DocType = cashTax.DocType,
                    DocDate = cashTax.DocDate.Value.AddHours(7),
                    CustCode = cashTax.CustCode,
                    CitizenId = cashTax.CitizenId,
                    CustName = cashTax.CustName,
                    CustAddr1 = cashTax.CustAddr1,
                    CustAddr2 = cashTax.CustAddr2,
                    RefNo = cashTax.RefNo,
                    ItemCount = cashTax.ItemCount,
                    Currency = cashTax.Currency,
                    CurRate = cashTax.CurRate,
                    SubAmt = cashTax.SubAmt,
                    SubAmtCur = cashTax.SubAmtCur,
                    DiscRate = cashTax.DiscRate,
                    DiscAmt = cashTax.DiscAmt,
                    DiscAmtCur = cashTax.DiscAmtCur,
                    TotalAmt = cashTax.TotalAmt,
                    TotalAmtCur = cashTax.TotalAmtCur,
                    TaxBaseAmt = cashTax.TaxBaseAmt,
                    TaxBaseAmtCur = cashTax.TaxBaseAmtCur,
                    VatRate = cashTax.VatRate,
                    VatAmt = cashTax.VatAmt,
                    VatAmtCur = cashTax.VatAmtCur,
                    NetAmt = cashTax.NetAmt,
                    NetAmtCur = cashTax.NetAmtCur,
                    Post = cashTax.Post,
                    RunNumber = cashTax.RunNumber,
                    DocPattern = cashTax.DocPattern,
                    CreatedDate = DateTime.Now,
                    CreatedBy = cashTax.CreatedBy
                };

                int runNumber = _cashTaxRepository.GetRunNumber(cashTaxHd);
                cashTaxHd.RunNumber = runNumber;
                var docNo = cashTaxHd.DocNo.Replace("#", "0").Substring(0, cashTaxHd.DocNo.Length - runNumber.ToString().Length) + cashTaxHd.RunNumber.ToString();
                cashTaxHd.DocNo = docNo;

                await _cashTaxRepository.AddHdAsync(cashTaxHd);

                int seqNo = 0;

                foreach (var salTaxinvoiceDt in cashTax.SalTaxinvoiceDt)
                {
                    var cashTaxDt = new SalTaxinvoiceDt
                    {
                        CompCode = salTaxinvoiceDt.CompCode,
                        BrnCode = salTaxinvoiceDt.BrnCode,
                        LocCode = salTaxinvoiceDt.LocCode,
                        DocNo = docNo,
                        SeqNo = ++seqNo,
                        LicensePlate = salTaxinvoiceDt.LicensePlate,
                        PdId = salTaxinvoiceDt.PdId,
                        PdName = salTaxinvoiceDt.PdName,
                        IsFree = salTaxinvoiceDt.IsFree,
                        UnitId = salTaxinvoiceDt.UnitId,
                        UnitBarcode = salTaxinvoiceDt.UnitBarcode,
                        UnitName = salTaxinvoiceDt.UnitName,
                        ItemQty = salTaxinvoiceDt.ItemQty,
                        StockQty = salTaxinvoiceDt.StockQty,
                        UnitPrice = salTaxinvoiceDt.UnitPrice,
                        UnitPriceCur = salTaxinvoiceDt.UnitPriceCur,
                        SumItemAmt = salTaxinvoiceDt.SumItemAmt,
                        SumItemAmtCur = salTaxinvoiceDt.SumItemAmtCur,
                        DiscAmt = salTaxinvoiceDt.DiscAmt,
                        DiscAmtCur = salTaxinvoiceDt.DiscAmtCur,
                        DiscHdAmt = salTaxinvoiceDt.DiscHdAmt,
                        DiscHdAmtCur = salTaxinvoiceDt.DiscHdAmtCur,
                        SubAmt = salTaxinvoiceDt.SubAmt,
                        SubAmtCur = salTaxinvoiceDt.SubAmtCur,
                        VatType = salTaxinvoiceDt.VatType,
                        VatRate = salTaxinvoiceDt.VatRate,
                        VatAmt = salTaxinvoiceDt.VatAmt,
                        VatAmtCur = salTaxinvoiceDt.VatAmtCur,
                        TaxBaseAmt = salTaxinvoiceDt.TaxBaseAmt,
                        TaxBaseAmtCur = salTaxinvoiceDt.TaxBaseAmtCur,
                        TotalAmt = salTaxinvoiceDt.TotalAmt,
                        TotalAmtCur = salTaxinvoiceDt.TaxBaseAmtCur
                    };

                    await _cashTaxRepository.AddDtAsync(cashTaxDt);
                }

                _cashSaleRepository.UpdateStatusAsync(cashTaxHd.RefNo, "Reference");

                await _unitOfWork.CompleteAsync();
                cashTax.Guid = cashTaxHd.Guid;

                return new CashTaxHdResponse(cashTax);
            }
            catch (Exception ex)
            {
                if(await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                // Do some logging stuff
                return new CashTaxHdResponse($"An error occurred when saving the cash tax: {ex.Message}");
            }
        }

        public async Task<CashTaxHdResponse> UpdateAsync(Guid guid, CashTax cashTax)
        {
            var existingCashtaxHd = await _cashTaxRepository.FindByIdAsync(guid);

            if (existingCashtaxHd == null)
                return new CashTaxHdResponse("SalTaxinvoiceHd not found.");

            var existingCashtaxDt = await _cashTaxRepository.GetCashTaxDtByDocNoAsync(cashTax.DocNo);

            if (existingCashtaxDt == null)
                return new CashTaxHdResponse("Invalid SalTaxinvoiceDt.");

            existingCashtaxHd.DocStatus = cashTax.DocStatus;
            existingCashtaxHd.DocType = cashTax.DocType;
            existingCashtaxHd.DocDate = cashTax.DocDate.Value.AddHours(7);
            existingCashtaxHd.CustCode = cashTax.CustCode;
            existingCashtaxHd.CitizenId = cashTax.CitizenId;
            existingCashtaxHd.CustName = cashTax.CustName;
            existingCashtaxHd.CustAddr1 = cashTax.CustAddr1;
            existingCashtaxHd.CustAddr2 = cashTax.CustAddr2;
            existingCashtaxHd.RefNo = cashTax.RefNo;
            existingCashtaxHd.ItemCount = cashTax.ItemCount;
            existingCashtaxHd.Currency = cashTax.Currency;
            existingCashtaxHd.CurRate = cashTax.CurRate;
            existingCashtaxHd.SubAmt = cashTax.SubAmt;
            existingCashtaxHd.SubAmtCur = cashTax.SubAmtCur;
            existingCashtaxHd.DiscRate = cashTax.DiscRate;
            existingCashtaxHd.DiscAmt = cashTax.DiscAmt;
            existingCashtaxHd.DiscAmtCur = cashTax.DiscAmtCur;
            existingCashtaxHd.TotalAmt = cashTax.TotalAmt;
            existingCashtaxHd.TotalAmtCur = cashTax.TotalAmtCur;
            existingCashtaxHd.TaxBaseAmt = cashTax.TaxBaseAmt;
            existingCashtaxHd.TaxBaseAmtCur = cashTax.TaxBaseAmtCur;
            existingCashtaxHd.VatRate = cashTax.VatRate;
            existingCashtaxHd.VatAmt = cashTax.VatAmt;
            existingCashtaxHd.VatAmtCur = cashTax.VatAmtCur;
            existingCashtaxHd.NetAmt = cashTax.NetAmt;
            existingCashtaxHd.NetAmtCur = cashTax.NetAmtCur;
            existingCashtaxHd.Post = cashTax.Post;
            existingCashtaxHd.RunNumber = cashTax.RunNumber;
            existingCashtaxHd.DocPattern = cashTax.DocPattern;
            existingCashtaxHd.UpdatedDate = DateTime.Now;
            existingCashtaxHd.UpdatedBy = cashTax.UpdatedBy;
            existingCashtaxHd.SalTaxinvoiceDt = cashTax.SalTaxinvoiceDt.Select(x => new SalTaxinvoiceDt
            {
                CompCode = x.CompCode,
                BrnCode = x.BrnCode,
                LocCode = x.LocCode,
                DocNo = x.DocNo,
                SeqNo = x.SeqNo,
                LicensePlate = x.LicensePlate,
                PdId = x.PdId,
                PdName = x.PdName,
                IsFree = x.IsFree,
                UnitId = x.UnitId,
                UnitBarcode = x.UnitBarcode,
                UnitName = x.UnitName,
                ItemQty = x.ItemQty,
                StockQty = x.StockQty,
                UnitPrice = x.UnitPrice,
                UnitPriceCur = x.UnitPriceCur,
                SumItemAmt = x.SumItemAmt,
                SumItemAmtCur = x.SumItemAmtCur,
                DiscAmt = x.DiscAmt,
                DiscAmtCur = x.DiscAmtCur,
                DiscHdAmt = x.DiscHdAmt,
                DiscHdAmtCur = x.DiscHdAmtCur,
                SubAmt = x.SubAmt,
                SubAmtCur = x.SubAmtCur,
                VatType = x.VatType,
                VatRate = x.VatRate,
                VatAmt = x.VatAmt,
                VatAmtCur = x.VatAmtCur,
                TaxBaseAmt = x.TaxBaseAmt,
                TaxBaseAmtCur = x.TaxBaseAmtCur,
                TotalAmt = x.TotalAmt,
                TotalAmtCur = x.TotalAmtCur
            }).ToList();

            try
            {
                _cashTaxRepository.UpdateAsync(existingCashtaxHd);
                _cashTaxRepository.RemoveDtAsync(existingCashtaxDt);
                _cashTaxRepository.AddDtListAsync(existingCashtaxHd.SalTaxinvoiceDt);
                await _unitOfWork.CompleteAsync();

                return new CashTaxHdResponse(existingCashtaxHd);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CashTaxHdResponse($"An error occurred when updating the SalTaxinvoice: {ex.Message}");
            }
        }

        public async Task CancelAndReplace(CashTaxCancelAndReplaceRequest pInput)
        {
            await _cashTaxRepository.CancelAndReplace(pInput);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<FinBalance> GetFinBalanceByCashTax(FinBalanceByCasTaxRequest pCashTax)
        {
            return await _cashTaxRepository.GetFinBalanceByCashTax(pCashTax);
        }

        public async Task<MasCustomer> GetCustomerByCustCode(CustomerByCustCodeRequset request)
        {
            return await _cashTaxRepository.GetCustomerByCustCode(request);
        }
    }
}
