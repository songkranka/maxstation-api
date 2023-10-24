using MaxStation.Entities.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Repositories;
using Sale.API.Domain.Services;
using Sale.API.Domain.Services.Communication;
using Sale.API.Infrastructure;
using Sale.API.Resources.CashSale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Services
{
    public class CashSaleService : ICashSaleService
    {
        private readonly ICashSaleRepository _cashSaleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory serviceScopeFactory;

        private readonly IMemoryCache _cache;
        public CashSaleService(
            ICashSaleRepository cashSaleRepository,
            IUnitOfWork unitOfWork,
            IServiceScopeFactory serviceScopeFactory,
            IMemoryCache cache)
        {
            _cashSaleRepository = cashSaleRepository;
            _unitOfWork = unitOfWork;
            this.serviceScopeFactory = serviceScopeFactory;
            _cache = cache;
        }

        public async Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query)
        {
            //string cacheKey = GetCacheKeyForProductsQuery(query);
            //var cashSales = await _cache.GetOrCreateAsync(cacheKey, (entry) => {
            //    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            //    return _cashSaleRepository.ListAsync(query);
            //});

            //return cashSales;

            return await _cashSaleRepository.ListAsync(query);
        }

        private string GetCacheKeyForProductsQuery(CashSaleHdQuery query)
        {
            string key = CacheKeys.CashSaleList.ToString();

            //if (query.CompCode.HasValue && query.CompCode > 0)
            //{
            //    key = string.Concat(key, "_", query.CompCode.Value);
            //}

            key = string.Concat(key, "_", query.Page, "_", query.ItemsPerPage);
            return key;
        }

        public async Task<QueryResult<SalCashsaleHd>> ListActiveAsync(CashSaleHdQuery query)
        {
            return await _cashSaleRepository.ListActiveAsync(query);
        }

        public async Task<CashSale> FindByIdAsync(CashSaleHdQuery query)
        {
            return await _cashSaleRepository.FindByIdAsync(query.Guid);
        }

        public async Task<CashsaleHdResponse> SaveAsync(CashSale cashSale)
        {
            try
            {
                var cashSaleHd = new SalCashsaleHd
                {
                    CompCode = cashSale.CompCode,
                    BrnCode = cashSale.BrnCode,
                    LocCode = cashSale.LocCode,
                    DocNo = cashSale.DocNo,
                    DocStatus = cashSale.DocStatus,
                    DocDate = cashSale.DocDate.Value.AddHours(7),
                    RefNo = cashSale.RefNo,
                    ItemCount = cashSale.ItemCount,
                    Currency = cashSale.Currency,
                    CurRate = cashSale.CurRate,
                    SubAmt = cashSale.SubAmt,
                    SubAmtCur = cashSale.SubAmtCur,
                    DiscRate = cashSale.DiscRate,
                    DiscAmt = cashSale.DiscAmt ?? 0,
                    DiscAmtCur = cashSale.DiscAmtCur ?? 0,
                    TotalAmt = cashSale.TotalAmt,
                    TotalAmtCur = cashSale.TotalAmtCur,
                    TaxBaseAmt = cashSale.TaxBaseAmt,
                    TaxBaseAmtCur = cashSale.TaxBaseAmtCur,
                    VatRate = cashSale.VatRate,
                    VatAmt = cashSale.VatAmt,
                    VatAmtCur = cashSale.VatAmtCur,
                    NetAmt = cashSale.NetAmt,
                    NetAmtCur = cashSale.NetAmtCur,
                    Post = cashSale.Post,
                    RunNumber = cashSale.RunNumber,
                    DocPattern = cashSale.DocPattern,
                    CreatedDate = DateTime.Now,
                    CreatedBy = cashSale.CreatedBy ,
                    QtNo = cashSale.QtNo ,
                };

                int runNumber = _cashSaleRepository.GetRunNumber(cashSaleHd);
                cashSaleHd.RunNumber = runNumber;
                var docNo = cashSaleHd.DocNo.Replace("#", "0").Substring(0, cashSaleHd.DocNo.Length - runNumber.ToString().Length) + cashSaleHd.RunNumber.ToString();
                cashSaleHd.DocNo = docNo;
                await _cashSaleRepository.AddHdAsync(cashSaleHd);
                for (int i = 0; i < cashSale.SalCashsaleDt.Count; i++)
                {
                    var cs = cashSale.SalCashsaleDt[i];
                    cs.DocNo = docNo;
                    cs.SeqNo = i + 1;
                    
                }
                _cashSaleRepository.AddDtListAsync(cashSale.SalCashsaleDt);
                // update status Reference Quotation
                await _cashSaleRepository.UpdateQuotationByCashSale(cashSale);
                await _unitOfWork.CompleteAsync();
                cashSale.Guid = cashSaleHd.Guid;
                return new CashsaleHdResponse(cashSale);
            }
            catch (Exception ex)
            {
                string strMessage = ex.StackTrace;
                while (ex.InnerException != null) ex = ex.InnerException;
                strMessage = ex.Message + Environment.NewLine + strMessage;
                // Do some logging stuff
                return new CashsaleHdResponse($"An error occurred when saving the cashSaleHd: {strMessage}");
            }
        }

        /* old
        public async Task<CashsaleHdResponse> SaveAsync(CashSale cashSale)
        {
            try
            {
                var cashSaleHd = new SalCashsaleHd
                {
                    CompCode = cashSale.CompCode,
                    BrnCode = cashSale.BrnCode,
                    LocCode = cashSale.LocCode,
                    DocNo = cashSale.DocNo,
                    DocStatus = cashSale.DocStatus,
                    DocDate = cashSale.DocDate.Value.AddHours(7),
                    RefNo = cashSale.RefNo,
                    ItemCount = cashSale.ItemCount,
                    Currency = cashSale.Currency,
                    CurRate = cashSale.CurRate,
                    SubAmt = cashSale.SubAmt,
                    SubAmtCur = cashSale.SubAmtCur,
                    DiscRate = cashSale.DiscRate,
                    DiscAmt = cashSale.DiscAmt,
                    DiscAmtCur = cashSale.DiscAmtCur,
                    TotalAmt = cashSale.TotalAmt,
                    TotalAmtCur = cashSale.TotalAmtCur,
                    TaxBaseAmt = cashSale.TaxBaseAmt,
                    TaxBaseAmtCur = cashSale.TaxBaseAmtCur,
                    VatRate = cashSale.VatRate,
                    VatAmt = cashSale.VatAmt,
                    VatAmtCur = cashSale.VatAmtCur,
                    NetAmt = cashSale.NetAmt,
                    NetAmtCur = cashSale.NetAmtCur,
                    Post = cashSale.Post,
                    RunNumber = cashSale.RunNumber,
                    DocPattern = cashSale.DocPattern,
                    CreatedDate = DateTime.Now,
                    CreatedBy = cashSale.CreatedBy
                };

                int runNumber = _cashSaleRepository.GetRunNumber(cashSaleHd);
                cashSaleHd.RunNumber = runNumber;
                var docNo = cashSaleHd.DocNo.Replace("#", "0").Substring(0, cashSaleHd.DocNo.Length - runNumber.ToString().Length) + cashSaleHd.RunNumber.ToString();
                cashSaleHd.DocNo = docNo;
                await _cashSaleRepository.AddHdAsync(cashSaleHd);

                int seqNo = 0;

                foreach (var salCashsaleDt in cashSale.SalCashsaleDt)
                {
                    var cashSaleDt = new SalCashsaleDt
                    {
                        CompCode = salCashsaleDt.CompCode,
                        BrnCode = salCashsaleDt.BrnCode,
                        LocCode = salCashsaleDt.LocCode,
                        DocNo = docNo,
                        SeqNo = ++seqNo,
                        PdId = salCashsaleDt.PdId,
                        PdName = salCashsaleDt.PdName,
                        IsFree = salCashsaleDt.IsFree,
                        UnitId = salCashsaleDt.UnitId,
                        UnitBarcode = salCashsaleDt.UnitBarcode,
                        UnitName = salCashsaleDt.UnitName,
                        ItemQty = salCashsaleDt.ItemQty,
                        StockQty = salCashsaleDt.StockQty,
                        UnitPrice = salCashsaleDt.UnitPrice,
                        UnitPriceCur = salCashsaleDt.UnitPriceCur,
                        SumItemAmt = salCashsaleDt.SumItemAmt,
                        SumItemAmtCur = salCashsaleDt.SumItemAmtCur,
                        DiscAmt = salCashsaleDt.DiscAmt,
                        DiscAmtCur = salCashsaleDt.DiscAmtCur,
                        DiscHdAmt = salCashsaleDt.DiscAmt,
                        DiscHdAmtCur = salCashsaleDt.DiscHdAmtCur,
                        SubAmt = salCashsaleDt.SubAmt,
                        SubAmtCur = salCashsaleDt.SubAmtCur,
                        VatType = salCashsaleDt.VatType,
                        VatRate = salCashsaleDt.VatRate,
                        VatAmt = salCashsaleDt.VatAmt,
                        VatAmtCur = salCashsaleDt.VatAmtCur,
                        TaxBaseAmt = salCashsaleDt.TaxBaseAmt,
                        TaxBaseAmtCur = salCashsaleDt.TaxBaseAmtCur,
                        TotalAmt = salCashsaleDt.TotalAmt,
                        TotalAmtCur = salCashsaleDt.TotalAmtCur
                    };

                    await _cashSaleRepository.AddDtAsync(cashSaleDt);
                }
                
                await _unitOfWork.CompleteAsync();
                cashSale.Guid = cashSaleHd.Guid;

                return new CashsaleHdResponse(cashSale);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CashsaleHdResponse($"An error occurred when saving the cashSaleHd: {ex.Message}");
            }
        }
        */

        public async Task<CashsaleHdResponse> UpdateAsync(Guid guid, CashSale cashSale)
        {
            var existingCashsaleHd = await _cashSaleRepository.FindByIdAsync(guid);

            if (existingCashsaleHd == null)
                return new CashsaleHdResponse("CashsaleHd not found.");

            //var existingCashsaleDt = existingCashsaleHd.SalCashsaleDt
            //    .Where(x => x.CompCode == existingCashsaleHd.CompCode && x.BrnCode == existingCashsaleHd.BrnCode 
            //    && x.LocId == existingCashsaleHd.LocId && x.DocNo == existingCashsaleHd.DocNo);

            var existingCashsaleDt = await _cashSaleRepository.GetCashSaleDtByDocNoAsync(cashSale.DocNo);

            if (existingCashsaleDt == null)
                return new CashsaleHdResponse("Invalid CashSaleDT.");

            existingCashsaleHd.DocStatus = cashSale.DocStatus;
            existingCashsaleHd.DocDate = cashSale.DocDate.Value.AddHours(7);
            existingCashsaleHd.RefNo = cashSale.RefNo;
            existingCashsaleHd.ItemCount = cashSale.ItemCount;
            existingCashsaleHd.Currency = cashSale.Currency;
            existingCashsaleHd.CurRate = cashSale.CurRate;
            existingCashsaleHd.SubAmt = cashSale.SubAmt;
            existingCashsaleHd.SubAmtCur = cashSale.SubAmtCur;
            existingCashsaleHd.DiscRate = cashSale.DiscRate;
            existingCashsaleHd.DiscAmt = cashSale.DiscAmt??0;
            existingCashsaleHd.DiscAmtCur = cashSale.DiscAmtCur??0;
            existingCashsaleHd.TotalAmt = cashSale.TotalAmt;
            existingCashsaleHd.TotalAmtCur = cashSale.TotalAmtCur;
            existingCashsaleHd.TaxBaseAmt = cashSale.TaxBaseAmt;
            existingCashsaleHd.TaxBaseAmtCur = cashSale.TaxBaseAmtCur;
            existingCashsaleHd.VatRate = cashSale.VatRate;
            existingCashsaleHd.VatAmt = cashSale.VatAmt;
            existingCashsaleHd.VatAmtCur = cashSale.VatAmtCur;
            existingCashsaleHd.NetAmt = cashSale.NetAmt;
            existingCashsaleHd.NetAmtCur = cashSale.NetAmtCur;
            existingCashsaleHd.Post = cashSale.Post;
            existingCashsaleHd.RunNumber = cashSale.RunNumber;
            existingCashsaleHd.DocPattern = cashSale.DocPattern;
            existingCashsaleHd.UpdatedDate = DateTime.Now;
            existingCashsaleHd.UpdatedBy = cashSale.UpdatedBy;
            existingCashsaleHd.SalCashsaleDt = cashSale.SalCashsaleDt.Select(x => new SalCashsaleDt
            {
                CompCode = x.CompCode,
                BrnCode = x.BrnCode,
                LocCode = x.LocCode,
                DocNo = x.DocNo,
                SeqNo = x.SeqNo,
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
                DiscAmt = x.DiscAmt??0,
                DiscAmtCur = x.DiscAmtCur??0,
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
                _cashSaleRepository.UpdateAsync(existingCashsaleHd);
                _cashSaleRepository.RemoveDtAsync(existingCashsaleDt);
                _cashSaleRepository.AddDtListAsync(existingCashsaleHd.SalCashsaleDt);

                await _unitOfWork.CompleteAsync();

                return new CashsaleHdResponse(existingCashsaleHd);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CashsaleHdResponse($"An error occurred when updating the cashsalehd: {ex.Message}");
            }
        }

        public async Task<CashsaleHdResponse> SaveListAsync(List<SalCashsaleHd> cashSaleList)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    int runNumber = _cashSaleRepository.GetRunNumber(cashSaleList.First()); //เฉพาะ [0] เพราะทุกตัวต้องมี Pattern เดียวกันในแต่ละรอบ
                    foreach (SalCashsaleHd hd in cashSaleList)
                    {
                        hd.RunNumber = runNumber;
                        var docNo = hd.DocNo.Replace("#", "0").Substring(0, hd.DocNo.Length - runNumber.ToString().Length) + hd.RunNumber.ToString();
                        hd.DocNo = docNo;
                        hd.CreatedDate = DateTime.Now;
                        hd.UpdatedDate = DateTime.Now;

                        //Reject ใบซ้ำ
                        var duplicateDoc = await _cashSaleRepository.CheckDataDuplicate(hd);
                        if (duplicateDoc == 0)
                        {
                            await _cashSaleRepository.AddHdAsync(hd);
                            int seq = 1;
                            foreach (SalCashsaleDt dt in hd.SalCashsaleDt)
                            {
                                dt.CompCode = hd.CompCode;
                                dt.BrnCode = hd.BrnCode;
                                dt.LocCode = hd.LocCode;
                                dt.DocNo = docNo;
                                dt.SeqNo = seq;
                                seq++;
                                await _cashSaleRepository.AddDtAsync(dt);
                            }
                            runNumber++;
                        }
                        else { 
                            //Not Insert Duplicate Doc From POS
                        }
                    }
                    await _unitOfWork.CompleteAsync();
                }
                return new CashsaleHdResponse(cashSaleList[0]);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new CashsaleHdResponse($"An error occurred when saving the cashSaleHd: {ex.Message}");
            }
        }

        public async Task<List<SalQuotationHd>> GetQuotation()
        {
            return await _cashSaleRepository.GetQuotation();
        }
        public async Task<QuotationDetail[]> GetQuotationDetail(SalQuotationHd pQuotationHeader)
        {
            return await _cashSaleRepository.GetQuotationDetail(pQuotationHeader);
        }
        public async Task<List<SalQuotationHd>> GetQuotationListByCashSale(SalCashsaleHd pCashSale)
        {
            return await _cashSaleRepository.GetQuotationListByCashSale(pCashSale);
        }
        public async Task SaveCashSale2(CashSaleResource2 pInput)
        {
            await _cashSaleRepository.SaveCashSale2(pInput);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<CashSaleResource2> GetCashSale2(string pStrGuid)
        {
            return await _cashSaleRepository.GetCashSale2(pStrGuid);
        }


    }
}
