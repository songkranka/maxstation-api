using log4net;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Repositories;
using Sale.API.Helpers;
using Sale.API.Resources.CashSale;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Repositories
{
    public class CashSaleRepository : SqlDataAccessHelper, ICashSaleRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CashSaleRepository));

        public CashSaleRepository(PTMaxstationContext context) : base(context)
        {

        }

        public async Task<int> CheckDataDuplicate(SalCashsaleHd obj)
        {
            return await context.SalCashsaleHds.AsNoTracking().Where(
                x => x.CompCode == obj.CompCode
                && x.BrnCode == obj.BrnCode 
                && x.LocCode == obj.LocCode 
                && x.PosNo == obj.PosNo).CountAsync();
        }

        public int GetRunNumber(SalCashsaleHd cashSaleHd)
        {
            int runNumber = 1;
            SalCashsaleHd resp = new SalCashsaleHd();
            resp = this.context.SalCashsaleHds.OrderByDescending(y => y.RunNumber).FirstOrDefault(
                x => (x.DocPattern == cashSaleHd.DocPattern || cashSaleHd.DocPattern == "" || cashSaleHd.DocPattern == null)
            );

            if (resp != null)
            {
                runNumber = (int)resp.RunNumber + 1;
            }
            else
            {
                runNumber = 1;
            }
            return runNumber;
        }
        public async Task<QueryResult<SalCashsaleHd>> ListAsync(CashSaleHdQuery query)
        {
            Console.WriteLine($"Start ListAsync : {DateTime.Now}","Tests");            


            if (query == null)
            {
                return null;
            }            
            string strIsoLevel = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            string strSelect = @"select doc_no , doc_date , net_amt , doc_status , ref_no , guid from SAL_CASHSALE_HD(nolock)";
            string strCount = @"select COUNT(*) from SAL_CASHSALE_HD(nolock)";
            string strWhere = @" where 1=1 ";
            string strOrderBy = @" order by COMP_CODE, BRN_CODE, DOC_NO desc ";
            string strComCode = DefaultService.EncodeSqlString(query.CompCode);
            if (!0.Equals(strComCode.Length))
            {
                strWhere += $" and COMP_CODE = '{strComCode}'";
            }
            string strBrnCode = DefaultService.EncodeSqlString(query.BrnCode);
            if (!0.Equals(strBrnCode.Length))
            {
                strWhere += $" and BRN_CODE = '{strBrnCode}'";
            }
            if(query.FromDate.HasValue && query.ToDate.HasValue)
            {
                strWhere += $" and DOC_DATE between '{query.FromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}' and '{query.ToDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}'";
            }
            string strKeyWord = DefaultService.EncodeSqlString(query.Keyword);
            if (!0.Equals(strKeyWord.Length))
            {
                strWhere += $" and ( DOC_NO like '%{strKeyWord}%' or DOC_STATUS like '%{strKeyWord}%' or ref_no like '%{strKeyWord}%' )";
            }
            //string strCon = context.Database.GetConnectionString();
            int intTotal = await DefaultService.ExecuteScalar<int>(context, strIsoLevel + strCount + strWhere);
            string strPage = string.Empty;
            if(query.Page > 0 && query.ItemsPerPage > 0)
            {
                strPage = $" OFFSET {(query.Page - 1) * query.ItemsPerPage} row fetch next {query.ItemsPerPage} row only";
            }
            var listCashSale = await DefaultService.GetEntityFromSql<List<SalCashsaleHd>>(
                context, strIsoLevel + strSelect + strWhere + strOrderBy + strPage
            );

            Console.WriteLine($"Finish ListAsync : {DateTime.Now}", "Tests");
            log.Info($"Finish ListAsync : {DateTime.Now}");

            return new QueryResult<SalCashsaleHd>
            {
                Items = listCashSale ?? new List<SalCashsaleHd>(),
                TotalItems = intTotal,
                ItemsPerPage = query.ItemsPerPage
            };
        }
        public async Task<QueryResult<SalCashsaleHd>> ListAsyncOld(CashSaleHdQuery query)
        {
            //var queryable = context.SalCashsaleHds
            //    .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode)
            //    .OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
            //    .AsNoTracking();

            //if (!string.IsNullOrEmpty(query.Keyword))
            //{
            //    queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
            //        || p.VatAmtCur.ToString().Contains(query.Keyword)
            //        || p.DocStatus.Contains(query.Keyword));
            //}
            //else if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            //{
            //    queryable = queryable.Where(p => p.DocDate >= query.FromDate && p.DocDate <= query.ToDate);
            //}

            //int totalItems = await queryable.CountAsync();
            //var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
            //                               .Take(query.ItemsPerPage)
            //                               .ToListAsync();

            var resp = new List<SalCashsaleHd>();
            int totalItems = 0;

            if (query.FromDate != DateTime.MinValue && query.ToDate != DateTime.MinValue)
            {
                var queryable = context.SalCashsaleHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate >= query.FromDate && x.DocDate <= query.ToDate)
                .OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
                .AsNoTracking();

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                        || p.VatAmtCur.ToString().Contains(query.Keyword)
                        || p.DocStatus.Contains(query.Keyword));
                }

                resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();
                totalItems = await queryable.CountAsync();
            }
            else
            {
                var queryable = context.SalCashsaleHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocDate >= query.SysDate.AddDays(-30))
                .OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo)
                .AsNoTracking();

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                        || p.VatAmtCur.ToString().Contains(query.Keyword)
                        || p.DocStatus.Contains(query.Keyword));
                }

                resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();
                totalItems = await queryable.CountAsync();
            }

            return new QueryResult<SalCashsaleHd>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<QueryResult<SalCashsaleHd>> ListActiveAsync(CashSaleHdQuery query)
        {
            var queryable = context.SalCashsaleHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && x.DocStatus == "Active")
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                    || p.RefNo.Contains(query.Keyword));
            }

            int totalItems = await queryable.CountAsync();

            return new QueryResult<SalCashsaleHd>
            {
                Items = queryable.Skip((query.Page - 1) * query.ItemsPerPage).Take(query.ItemsPerPage).ToList(),
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }


        public async Task<CashSale> FindByIdAsync(Guid guid)
        {
            var response = new CashSale();
            var cashSaleHd = await context.SalCashsaleHds.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == guid);
            
            if(cashSaleHd != null)
            {
                 var cashSaleDt =  context.SalCashsaleDts.AsNoTracking().Where(x => x.CompCode == cashSaleHd.CompCode && x.BrnCode == cashSaleHd.BrnCode && x.DocNo == cashSaleHd.DocNo).OrderBy(y => y.SeqNo).ToList();
                response.CompCode = cashSaleHd.CompCode;
                response.BrnCode = cashSaleHd.BrnCode;
                response.LocCode = cashSaleHd.LocCode;
                response.DocNo = cashSaleHd.DocNo;
                response.DocStatus = cashSaleHd.DocStatus;
                response.DocDate = cashSaleHd.DocDate;
                response.RefNo = cashSaleHd.RefNo;
                response.ItemCount = cashSaleHd.ItemCount;
                response.Currency = cashSaleHd.Currency;
                response.CurRate = cashSaleHd.CurRate;
                response.SubAmt = cashSaleHd.SubAmt;
                response.SubAmtCur = cashSaleHd.SubAmtCur;
                response.DiscRate = cashSaleHd.DiscRate;
                response.DiscAmt = cashSaleHd.DiscAmt;
                response.DiscAmtCur = cashSaleHd.DiscAmtCur;
                response.TaxBaseAmt = cashSaleHd.TaxBaseAmt;
                response.TaxBaseAmtCur = cashSaleHd.TaxBaseAmtCur;
                response.NetAmt = cashSaleHd.NetAmt;
                response.NetAmtCur = cashSaleHd.NetAmtCur;
                response.VatRate = cashSaleHd.VatRate;
                response.VatAmt = cashSaleHd.VatAmt;
                response.VatAmtCur = cashSaleHd.VatAmtCur;
                response.TotalAmt = cashSaleHd.TotalAmt;
                response.TotalAmtCur = cashSaleHd.TotalAmtCur;
                response.Post = cashSaleHd.Post;
                response.RunNumber = cashSaleHd.RunNumber;
                response.DocPattern = cashSaleHd.DocPattern;
                response.Guid = cashSaleHd.Guid;
                response.CreatedDate = cashSaleHd.CreatedDate;
                response.CreatedBy = cashSaleHd.CreatedBy;
                response.QtNo = cashSaleHd.QtNo;
                response.SalCashsaleDt = cashSaleDt;
            }

            return response;
        }

        public async Task<List<SalCashsaleDt>> GetCashSaleDtByDocNoAsync(string docNo)
        {
            return await context.SalCashsaleDts.AsNoTracking().Where(x => x.DocNo == docNo).ToListAsync();
        }

        public async Task AddHdAsync(SalCashsaleHd cashSaleHd)
        {
            await context.SalCashsaleHds.AddAsync(cashSaleHd);
        }
        
        public async Task AddDtAsync(SalCashsaleDt cashSaleDt)
        {
            await context.SalCashsaleDts.AddAsync(cashSaleDt);
        }

        public void UpdateAsync(SalCashsaleHd cashSaleHd)
        {
            context.SalCashsaleHds.Update(cashSaleHd);
        }

        public void UpdateStatusAsync(string docNo, string status)
        {
            var salCashsaleHd = context.SalCashsaleHds.FirstOrDefault(s => s.DocNo == docNo);
            
            if (salCashsaleHd != null)
            {
                salCashsaleHd.DocStatus = status;
                context.SalCashsaleHds.Update(salCashsaleHd);
            }
        }

        public void AddDtListAsync(IEnumerable<SalCashsaleDt> cashSaleDt)
        {
            context.SalCashsaleDts.AddRange(cashSaleDt);
        }

        public void RemoveDtAsync(IEnumerable<SalCashsaleDt> cashSaledt)
        {
            context.SalCashsaleDts.RemoveRange(cashSaledt);
        }

        public async Task<List<SalQuotationHd>> GetQuotation()
        {
            IQueryable<SalQuotationHd> qry = context.SalQuotationHds
                .AsNoTracking()
                .Where(x => "CashSale".Equals(x.DocType) && x.DocStatus == "Ready");
            List<SalQuotationHd> result = await qry.ToListAsync();
            return result;
        }

        public async Task<List<SalQuotationHd>> GetQuotationListByCashSale(SalCashsaleHd pCashSale)
        {
            if(pCashSale == null)
            {
                return null;
            }
            string strCashSale = "CashSale";
            string strReady = "Ready";
            string strCompCode = (pCashSale?.CompCode ?? string.Empty).Trim();
            string strBrnCode = (pCashSale?.BrnCode ?? string.Empty).Trim();
            string strLocCode = (pCashSale?.LocCode ?? string.Empty).Trim();
            IQueryable<SalQuotationHd> qry = context.SalQuotationHds.AsNoTracking().Where(
                x => strCashSale.Equals(x.DocType) 
                && strReady.Equals(x.DocStatus)
                && strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
            );
            List<SalQuotationHd> result = await qry.ToListAsync();
            return result;
        }


        public async Task<QuotationDetail[]> GetQuotationDetail(SalQuotationHd pQuotationHeader)
        {
            if(pQuotationHeader == null)
            {
                return null;
            }
            string strBrnCode = (pQuotationHeader?.BrnCode ?? string.Empty).Trim();
            string strCompCode = (pQuotationHeader?.CompCode ?? string.Empty).Trim();
            string strLocCode = (pQuotationHeader?.LocCode ?? string.Empty).Trim();
            string strDocNo = (pQuotationHeader?.DocNo ?? string.Empty).Trim();

            IQueryable<SalQuotationDt> qryQuotationDt = null;
            qryQuotationDt = context.SalQuotationDts.AsNoTracking().Where(
                x => x.BrnCode == strBrnCode
                && x.CompCode == strCompCode
                && x.LocCode == strLocCode
                && x.DocNo == strDocNo
                && x.StockRemain.HasValue
                && x.StockRemain.Value > decimal.Zero
            );

            SalQuotationDt[] arrSalQuotationDt = null;
            arrSalQuotationDt = await qryQuotationDt.ToArrayAsync();

            if(arrSalQuotationDt == null || !arrSalQuotationDt.Any())
            {
                return null;
            }

            QuotationDetail[] result = null;
            result = arrSalQuotationDt
                .Select(x => convertObject<QuotationDetail>(x))
                .ToArray();

            string[] arrUnitBarCode = null;
            arrUnitBarCode = arrSalQuotationDt
                    .Select(x => (x?.UnitBarcode ?? string.Empty).Trim())
                    .Where(x=> !0.Equals(x.Length))
                    .Distinct()
                    .ToArray();
            if (arrUnitBarCode == null || !arrUnitBarCode.Any())
            {
                return result;
            }

            MasProductPrice[] arrProductPrice = null;
            arrProductPrice = await context.MasProductPrices.AsNoTracking().Where(
                    y => y.BrnCode == strBrnCode
                    && y.CompCode == strCompCode
                    && y.LocCode == strLocCode
                    && arrUnitBarCode.Contains( y.UnitBarcode ) 
                    && "Active".Equals(y.PdStatus)
                ).ToArrayAsync();
            if (arrUnitBarCode == null || !arrUnitBarCode.Any())
            {
                return result;
            }
            for (int i = 0; i < result.Length; i++)
            {
                result[i].MasProductPrice = arrProductPrice.FirstOrDefault(x => x.UnitBarcode == result[i].UnitBarcode);
            }
            return result;
        }

        public async Task UpdateQuotationByCashSale(SalCashsaleHd pCashSale)
        {
            if (pCashSale == null)
            {
                return;
            }
            string strQtNo = (pCashSale?.QtNo ?? string.Empty).Trim();
            if (0.Equals(strQtNo.Length))
            {
                return;
            }
            string strCompCode = (pCashSale?.CompCode ?? string.Empty).Trim();
            string strBrnCode = (pCashSale?.BrnCode ?? string.Empty).Trim();
            string strLocCode = (pCashSale?.LocCode ?? string.Empty).Trim();

            var quotation = await context.SalQuotationHds.FirstOrDefaultAsync(
                x => strQtNo.Equals(x.DocNo)
                && strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
            );
            if(quotation == null)
            {
                return;
            }
            quotation.DocStatus = "Reference";
            quotation.UpdatedDate = pCashSale.CreatedDate;
            quotation.UpdatedBy = pCashSale.CreatedBy;
        }
        private async Task< decimal> CalculateStockQty2(string pStrProductId, String pStrUnitId , decimal pDecItemQty)
        {
            decimal result = decimal.Zero;
            pStrProductId = (pStrProductId ?? string.Empty).Trim();
            pStrUnitId = (pStrUnitId ?? string.Empty).Trim();
            if (!(0.Equals(pStrProductId.Length) || 0.Equals(pStrUnitId.Length)))
            {
                var qryUnit = context.MasProductUnits.Where(
                    x=> x.PdId == pStrProductId 
                    && x.UnitId == pStrUnitId)
                .AsNoTracking();
                var productUnit = await qryUnit.FirstOrDefaultAsync();
                if (productUnit != null)
                {
                    var decUnitStock = productUnit.UnitStock ?? decimal.Zero;
                    var decUnitRatio = productUnit.UnitRatio ?? 1;
                    result = pDecItemQty * decUnitStock / decUnitRatio;
                    result = (pDecItemQty * (productUnit.UnitStock / productUnit.UnitRatio)).Value;
                }
           
            }
            //MasProductUnit productUnit = this.context.MasProductUnits.FirstOrDefault(x => x.PdId == pStrProductId);
            return result;
        }
        private decimal CalculateStockQty(string pdId, decimal itemQty)
        {
            decimal stockQty = 0m;
            MasProductUnit productUnit = this.context.MasProductUnits.FirstOrDefault(x => x.PdId == pdId);
            if (productUnit != null)
            {
                stockQty = (itemQty * (productUnit.UnitStock / productUnit.UnitRatio)).Value;
            }
            return stockQty;
        }

        public async Task SaveCashSale2(CashSaleResource2 pInput)
        {
            if(pInput == null 
            || pInput.CashSaleHeader == null 
            || pInput.ArrCashSaleDetail == null 
            || !pInput.ArrCashSaleDetail.Any())
            {
                return;
            }            
            string strDocStatus = (pInput?.CashSaleHeader?.DocStatus ?? string.Empty).Trim();
            
            string strCompCode = (pInput?.CashSaleHeader?.CompCode ?? string.Empty).Trim();
            string strBrnCode = (pInput?.CashSaleHeader?.BrnCode ?? string.Empty).Trim();
            string strLocCode = (pInput?.CashSaleHeader?.LocCode ?? string.Empty).Trim();

            string strDocNo = string.Empty;

            if ("New".Equals(strDocStatus))
            {
                pInput.CashSaleHeader.DocStatus = "Active";
                pInput.CashSaleHeader.DiscAmt = pInput.CashSaleHeader.DiscAmt ?? 0;
                pInput.CashSaleHeader.DiscAmtCur = pInput.CashSaleHeader.DiscAmtCur ?? 0;
                pInput.CashSaleHeader.CreatedDate = DateTime.Now;
                pInput.CashSaleHeader.Guid = Guid.NewGuid();
                await adjustHeaderRunningNo(pInput.CashSaleHeader);
                strDocNo = (pInput?.CashSaleHeader?.DocNo ?? string.Empty).Trim(); 
                await context.SalCashsaleHds.AddAsync(pInput.CashSaleHeader);
            }
            else
            {
                strDocNo = (pInput?.CashSaleHeader?.DocNo ?? string.Empty).Trim();
                pInput.CashSaleHeader.DiscAmt = pInput.CashSaleHeader.DiscAmt ?? 0;
                pInput.CashSaleHeader.DiscAmtCur = pInput.CashSaleHeader.DiscAmtCur ?? 0;
                pInput.CashSaleHeader.UpdatedDate = DateTime.Now;
                context.SalCashsaleHds.Update(pInput.CashSaleHeader);
                //bool isCashSaleExist = false;
                //isCashSaleExist = await context.SalCashsaleHds.AnyAsync(
                //    x => strDocNo.Equals(x.DocNo)
                //    && strCompCode.Equals(x.CompCode)
                //    && strBrnCode.Equals(x.BrnCode)
                //    && strLocCode.Equals(x.LocCode)
                //);
                //if ( isCashSaleExist)
                //{
                //    context.SalCashsaleHds.Update(pInput.CashSaleHeader);
                //}
                //else
                //{
                //    await context.SalCashsaleHds.AddAsync(pInput.CashSaleHeader);
                //}
            }

            SalCashsaleDt[] arrExitsCashsaleDetail = null;
            arrExitsCashsaleDetail = await context.SalCashsaleDts.Where(
                x => strDocNo.Equals(x.DocNo)
                && strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
            ).ToArrayAsync();
            if (arrExitsCashsaleDetail != null && arrExitsCashsaleDetail.Any())
            {
                context.SalCashsaleDts.RemoveRange(arrExitsCashsaleDetail);
            }
            int intSeqNo = 1;
            foreach (var csdt in pInput.ArrCashSaleDetail)
            {
                if (csdt == null)
                {
                    continue;
                }
                csdt.DocNo = strDocNo;
                csdt.CompCode = strCompCode;
                csdt.BrnCode = strBrnCode;
                csdt.LocCode = strLocCode;
                csdt.SeqNo = intSeqNo++;
                //csdt.StockQty = CalculateStockQty(csdt.PdId, csdt.ItemQty.Value);
            }
            await context.SalCashsaleDts.AddRangeAsync(pInput.ArrCashSaleDetail);
            if(pInput.QuotationHeader != null)
            {
                context.SalQuotationHds.Update(pInput.QuotationHeader);
                //pInput.QuotationHeader.DocStatus = "Reference";
                //context.SalQuotationHds.Attach(pInput.QuotationHeader);
            }
            if(pInput.ArrQuotationDetail != null && pInput.ArrQuotationDetail.Any())
            {
                context.SalQuotationDts.UpdateRange(pInput.ArrQuotationDetail);
            }
        }

        public async Task<CashSaleResource2> GetCashSale2(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if( 0.Equals( pStrGuid.Length))
            {
                return null;
            }
            Guid guidCashSale;
            if(!Guid.TryParse(pStrGuid , out guidCashSale))
            {
                return null;
            }
            CashSaleResource2 result = new CashSaleResource2();
            result.CashSaleHeader = await context.SalCashsaleHds.FirstOrDefaultAsync(x => guidCashSale.Equals(x.Guid));
            if(result.CashSaleHeader == null)
            {
                return null;
            }
            string strCompCode = (result.CashSaleHeader.CompCode ?? string.Empty).Trim();
            string strBrnCode = (result.CashSaleHeader.BrnCode ?? string.Empty).Trim();
            string strLocCode = (result.CashSaleHeader.LocCode ?? string.Empty).Trim();
            string strDocNo = (result.CashSaleHeader.DocNo ?? string.Empty).Trim();
            string strQtNo = (result.CashSaleHeader.QtNo ?? string.Empty).Trim();

            result.ArrCashSaleDetail = await context.SalCashsaleDts.Where(
                x => strDocNo.Equals(x.DocNo)
                && strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
            ).ToArrayAsync();

            if (0.Equals(strQtNo.Length))
            {
                return result;
            }
            result.QuotationHeader = await context.SalQuotationHds.FirstOrDefaultAsync(
                x=> strQtNo.Equals(x.DocNo)
                && strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
            );
            result.ArrQuotationDetail = await context.SalQuotationDts.Where(
                x => strQtNo.Equals(x.DocNo)
                && strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
            ).ToArrayAsync();
            return result;
        }

        private static T convertObject<T>(object pObjInput)
        {
            if (pObjInput == null)
            {
                return default(T);
            }
            var serialized = JsonConvert.SerializeObject(pObjInput);
            var result = JsonConvert.DeserializeObject<T>(serialized);
            return result;
        }

        private async Task adjustHeaderRunningNo(SalCashsaleHd pCashsale)
        {
            string strRunningDocNo = string.Empty;
            var qryDocPattern =
                from dp in context.MasDocPatterns.AsNoTracking()
                join dt in context.MasDocPatternDts.AsNoTracking()
                on dp.DocId equals dt.DocId
                where "CashSale".Equals(dp.DocType)
                select new MasDocPatternDt()
                {
                    DocValue = dt.DocValue,
                    DocCode = dt.DocCode,
                    SeqNo = dt.SeqNo
                };
            List<MasDocPatternDt> listDocPatternDetail = null;
            //listDocPatternDetail = (await qryDocPattern.ToListAsync()).OrderBy(x => x.SeqNo).ToList();
            listDocPatternDetail = await qryDocPattern.ToListAsync();
            bool isUseDefaultPattern = true;
            isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            int intLastRunning = 0;
            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pCashsale.DocDate != null && pCashsale.DocDate.HasValue)
            {
                intDay = pCashsale.DocDate.Value.Day;
                intMonth = pCashsale.DocDate.Value.Month;
                intYear = pCashsale.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<SalCashsaleHd> qryCashSale = context.SalCashsaleHds.Where(
                x => x.BrnCode == pCashsale.BrnCode
                && x.CompCode == pCashsale.CompCode
                && x.LocCode == pCashsale.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                qryCashSale = qryCashSale.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                listDocPatternDetail = listDocPatternDetail.OrderBy(x => x.SeqNo).ToList();
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryCashSale = qryCashSale.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryCashSale = qryCashSale.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryCashSale = qryCashSale.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }

            if (await qryCashSale.AnyAsync())
            {
                int intMaxRunning = await qryCashSale.MaxAsync(x => x.RunNumber.Value);
                int intRowCount = await qryCashSale.CountAsync();
                intLastRunning = Math.Max(intMaxRunning, intRowCount);
            }
            do
            {
                strRunningDocNo = string.Empty;
                intLastRunning++;
                if (isUseDefaultPattern)
                {
                    strRunningDocNo = string.Format("{0}{1}{2:D5}", intYear, intMonth, intLastRunning);
                }
                else
                {
                    foreach (var item in listDocPatternDetail)
                    {
                        if (item == null) continue;
                        switch (item.DocCode)
                        {
                            case "-": strRunningDocNo += "-"; break;
                            case "MM": strRunningDocNo += intMonth.ToString("00"); break;
                            case "Comp": strRunningDocNo += pCashsale.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pCashsale.BrnCode; break;
                            case "yyyy": strRunningDocNo += intYear.ToString("0000"); break;
                            case "yy": strRunningDocNo += intYear.ToString().Substring(2, 2); break;
                            case "[#]":
                                int intDocValue = 0;
                                int.TryParse(item.DocValue, out intDocValue);
                                strRunningDocNo += intLastRunning.ToString().PadLeft(intDocValue, '0');
                                break;
                            default:
                                break;
                        }
                    }
                }

            } while (await context.SalCashsaleHds.AnyAsync(
                x => x.BrnCode == pCashsale.BrnCode
                && x.CompCode == pCashsale.CompCode
                && x.LocCode == pCashsale.LocCode
                && x.DocNo == strRunningDocNo
            ));
            pCashsale.RunNumber = intLastRunning;
            pCashsale.DocNo = strRunningDocNo;
        }

    

    }
}

