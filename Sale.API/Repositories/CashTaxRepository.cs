using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Sale.API.Domain.Models;
using Sale.API.Domain.Models.Queries;
using Sale.API.Domain.Models.Request;
using Sale.API.Domain.Repositories;
using Sale.API.Helpers;
using Sale.API.Resources.CashTax;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sale.API.Domain.Models.Request.CashTaxCancelAndReplaceRequest;

namespace Sale.API.Repositories
{
    public class CashTaxRepository : SqlDataAccessHelper, ICashTaxRepository
    {
        public CashTaxRepository(PTMaxstationContext context) : base(context) { }

        public int GetRunNumber(SalTaxinvoiceHd cashSaleHd)
        {
            int runNumber = 1;
            SalTaxinvoiceHd resp = new SalTaxinvoiceHd();
            resp = this.context.SalTaxinvoiceHds.OrderByDescending(y => y.RunNumber).FirstOrDefault(
                x => (x.DocPattern == cashSaleHd.DocPattern || cashSaleHd.DocPattern == "" || cashSaleHd.DocPattern == null)
                && x.DocType == "CashTax"
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
        public async Task<QueryResult<SalTaxinvoiceHd>> ListAsync(CashTaxHdQuery query)
        {
            if (query == null)
            {
                return null;
            }
            string strIsoLevel = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            string strSelect = @"select doc_no , doc_date , doc_type , Cust_Code , Cust_Name , net_Amt , doc_status , guid from SAL_TAXINVOICE_HD(nolock)";
            string strCount = @"select COUNT(*) from SAL_TAXINVOICE_HD(nolock)";
            string strWhere = string.Empty;
            string strOrderBy = @" order by CREATED_DATE desc ";
            var listWhere = new List<string>();
            string strComCode = DefaultService.EncodeSqlString(query.CompCode);
            if (!0.Equals(strComCode.Length))
            {
                listWhere.Add($"COMP_CODE = '{strComCode}'");
            }
            string strBrnCode = DefaultService.EncodeSqlString(query.BrnCode);
            if (!0.Equals(strBrnCode.Length))
            {
                listWhere.Add($"BRN_CODE = '{strBrnCode}'");
            }
            if (query.FromDate.HasValue && query.ToDate.HasValue)
            {
                listWhere.Add($"DOC_DATE between '{DefaultService.EncodeSqlDate(query.FromDate.Value)}' and '{DefaultService.EncodeSqlDate(query.ToDate.Value)}'");
            }
            string strKeyWord = DefaultService.EncodeSqlString(query.Keyword);
            if (!0.Equals(strKeyWord.Length))
            {
                listWhere.Add($"( DOC_NO like '%{strKeyWord}%' or DOC_STATUS like '%{strKeyWord}%' )");
            }
            if (listWhere.Any())
            {
                strWhere = " where " + listWhere.Aggregate((x, y) => x + Environment.NewLine + "and " + y);
            }
            string strPage = string.Empty;
            if (query.Page > 0 && query.ItemsPerPage > 0)
            {
                strPage = $" OFFSET {(query.Page - 1) * query.ItemsPerPage} row fetch next {query.ItemsPerPage} row only";
            }
            var listCreditNote = await DefaultService.GetEntityFromSql<List<SalTaxinvoiceHd>>(
                context, strIsoLevel + strSelect + strWhere + strOrderBy + strPage
            );
            int intTotal = await DefaultService.ExecuteScalar<int>(context, strIsoLevel + strCount + strWhere);
            return new QueryResult<SalTaxinvoiceHd>
            {
                Items = listCreditNote ?? new List<SalTaxinvoiceHd>(),
                TotalItems = intTotal,
                ItemsPerPage = query.ItemsPerPage
            };
        }
        public async Task<QueryResult<SalTaxinvoiceHd>> ListAsyncOld(CashTaxHdQuery query)
        {
            var queryable = context.SalTaxinvoiceHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                var isDecimal = decimal.TryParse(query.Keyword, out decimal decim);

                queryable = queryable.Where(p => p.DocNo.Contains(query.Keyword)
                || p.DocType.Contains(query.Keyword)
                || p.CustName.Contains(query.Keyword)
                || p.DocStatus.Contains(query.Keyword)
                || (isDecimal && (p.NetAmt.Equals(decim))));
            }
            else if (query.FromDate != null && query.FromDate != DateTime.MinValue && query.ToDate != null && query.ToDate != DateTime.MinValue)
            {
                queryable = queryable.Where(p => p.DocDate >= query.FromDate && p.DocDate <= query.ToDate);
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<SalTaxinvoiceHd>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<CashTax> FindByIdAsync(Guid guid)
        {
            var response = new CashTax();
            var cashTaxHd = await context.SalTaxinvoiceHds.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == guid);

            if (cashTaxHd != null)
            {
                var cashTaxDt = context.SalTaxinvoiceDts.AsNoTracking().Where(x => x.CompCode == cashTaxHd.CompCode && x.BrnCode == cashTaxHd.BrnCode && x.DocNo == cashTaxHd.DocNo).OrderBy(y => y.SeqNo).ToList();
                response.CompCode = cashTaxHd.CompCode;
                response.BrnCode = cashTaxHd.BrnCode;
                response.LocCode = cashTaxHd.LocCode;
                response.DocNo = cashTaxHd.DocNo;
                response.DocStatus = cashTaxHd.DocStatus;
                response.DocType = cashTaxHd.DocType;
                response.DocDate = cashTaxHd.DocDate;
                response.CustCode = cashTaxHd.CustCode;
                response.CitizenId = cashTaxHd.CitizenId;
                response.CustName = cashTaxHd.CustName;
                response.CustAddr1 = cashTaxHd.CustAddr1;
                response.CustAddr2 = cashTaxHd.CustAddr2;
                response.RefNo = cashTaxHd.RefNo;
                response.ItemCount = cashTaxHd.ItemCount;
                response.Currency = cashTaxHd.Currency;
                response.CurRate = cashTaxHd.CurRate;
                response.SubAmt = cashTaxHd.SubAmt;
                response.SubAmtCur = cashTaxHd.SubAmtCur;
                response.DiscRate = cashTaxHd.DiscRate;
                response.DiscAmt = cashTaxHd.DiscAmt;
                response.DiscAmtCur = cashTaxHd.DiscAmtCur;
                response.TotalAmt = cashTaxHd.TotalAmt;
                response.TotalAmtCur = cashTaxHd.TotalAmtCur;
                response.TaxBaseAmt = cashTaxHd.TaxBaseAmt;
                response.TaxBaseAmtCur = cashTaxHd.TaxBaseAmtCur;
                response.VatRate = cashTaxHd.VatRate;
                response.VatAmt = cashTaxHd.VatAmt;
                response.VatAmtCur = cashTaxHd.VatAmtCur;
                response.NetAmt = cashTaxHd.NetAmt;
                response.NetAmtCur = cashTaxHd.NetAmtCur;
                response.Post = cashTaxHd.Post;
                response.RunNumber = cashTaxHd.RunNumber;
                response.DocPattern = cashTaxHd.DocPattern;
                response.Guid = cashTaxHd.Guid;
                response.CreatedDate = cashTaxHd.CreatedDate;
                response.CreatedBy = cashTaxHd.CreatedBy;
                response.SalTaxinvoiceDt = cashTaxDt;
            }

            return response;
        }

        public async Task<List<SalTaxinvoiceDt>> GetCashTaxDtByDocNoAsync(string docNo)
        {
            return await context.SalTaxinvoiceDts.AsNoTracking().Where(x => x.DocNo == docNo).ToListAsync();
        }

        public async Task AddHdAsync(SalTaxinvoiceHd cashTaxHd)
        {
            await context.SalTaxinvoiceHds.AddAsync(cashTaxHd);
        }

        public async Task AddDtAsync(SalTaxinvoiceDt cashTaxDt)
        {
            await context.SalTaxinvoiceDts.AddAsync(cashTaxDt);
        }

        public void UpdateAsync(SalTaxinvoiceHd cashTaxHd)
        {
            context.SalTaxinvoiceHds.Update(cashTaxHd);
        }

        public void AddDtListAsync(IEnumerable<SalTaxinvoiceDt> cashTaxDts)
        {
            context.SalTaxinvoiceDts.AddRange(cashTaxDts);
        }

        public void RemoveDtAsync(IEnumerable<SalTaxinvoiceDt> cashTaxDts)
        {
            context.SalTaxinvoiceDts.RemoveRange(cashTaxDts);
        }

        public async Task CancelAndReplace(CashTaxCancelAndReplaceRequest pInput)
        {
            if (pInput == null || pInput.CancelCashTax == null || pInput.NewCashTax == null)
            {
                return;
            }
            //if (pInput.FinBalance != null)
            //{
            //    context.Attach(pInput.FinBalance);
            //    pInput.FinBalance.NetAmt = decimal.Zero;
            //    pInput.FinBalance.NetAmtCur = decimal.Zero;

            //    var finBalance = context.FinBalances.FirstOrDefault(x => x.DocNo == pInput.FinBalance.DocNo);
            //    finBalance.NetAmt = decimal.Zero;
            //    finBalance.NetAmtCur = decimal.Zero;
            //    context.FinBalances.Update(finBalance);
            //}

            await adjustHeaderRunningNo(pInput.NewCashTax);
            pInput.NewCashTax.CreatedDate = DateTime.Now;
            pInput.NewCashTax.Guid = Guid.NewGuid();
            pInput.NewCashTax.DocStatus = "Active";
            var salTaxinvoiceHd = new SalTaxinvoiceHd
            {
                BrnCode = pInput.NewCashTax.BrnCode,
                CompCode = pInput.NewCashTax.CompCode,
                LocCode = pInput.NewCashTax.LocCode,
                DocNo = pInput.NewCashTax.DocNo,
                DocStatus = pInput.NewCashTax.DocStatus,
                DocType = pInput.NewCashTax.DocType,
                DocDate = pInput.NewCashTax.DocDate,
                CustCode = pInput.NewCashTax.CustCode,
                CitizenId = pInput.NewCashTax.CitizenId,
                CustName = pInput.NewCashTax.CustName,
                CustAddr1 = pInput.NewCashTax.CustAddr1,
                CustAddr2 = pInput.NewCashTax.CustAddr2,
                RefNo = pInput.NewCashTax.RefNo,
                ItemCount = pInput.NewCashTax.ItemCount,
                Currency = pInput.NewCashTax.Currency,
                CurRate = pInput.NewCashTax.CurRate,
                SubAmt = pInput.NewCashTax.SubAmt,
                SubAmtCur = pInput.NewCashTax.SubAmtCur,
                DiscRate = pInput.NewCashTax.DiscRate,
                DiscAmt = pInput.NewCashTax.DiscAmt,
                DiscAmtCur = pInput.NewCashTax.DiscAmtCur,
                TotalAmt = pInput.NewCashTax.TotalAmt,
                TotalAmtCur = pInput.NewCashTax.TotalAmtCur,
                TaxBaseAmt = pInput.NewCashTax.TaxBaseAmt,
                TaxBaseAmtCur = pInput.NewCashTax.TaxBaseAmtCur,
                VatRate = pInput.NewCashTax.VatRate,
                VatAmt = pInput.NewCashTax.VatAmt,
                VatAmtCur = pInput.NewCashTax.VatAmtCur,
                NetAmt = pInput.NewCashTax.NetAmt,
                NetAmtCur = pInput.NewCashTax.NetAmtCur,
                EmpCode = pInput.NewCashTax.EmpCode,
                EmpName = pInput.NewCashTax.EmpName,
                Post = pInput.CancelCashTax.Post, //pInput.NewCashTax.Post,
                RefDocNo = pInput.CancelCashTax.DocNo,
                RunNumber = pInput.NewCashTax.RunNumber,
                DocPattern = pInput.NewCashTax.DocPattern,
                Guid = pInput.NewCashTax.Guid,
                PrintCount = pInput.NewCashTax.PrintCount,
                PrintBy = pInput.NewCashTax.PrintBy,
                PrintDate = pInput.NewCashTax.PrintDate,
                CreatedDate = pInput.NewCashTax.CreatedDate,
                CreatedBy = pInput.NewCashTax.CreatedBy,
                UpdatedDate = pInput.NewCashTax.UpdatedDate,
                UpdatedBy = pInput.NewCashTax.UpdatedBy,
            };

            await context.SalTaxinvoiceHds.AddAsync(salTaxinvoiceHd);

            var cashTaxHd = context.SalTaxinvoiceHds.FirstOrDefault(x => x.DocNo == pInput.CancelCashTax.DocNo);
            cashTaxHd.DocStatus = "Cancel";
            cashTaxHd.RefDocNo = "";
            cashTaxHd.UpdatedDate = DateTime.Now;
            context.SalTaxinvoiceHds.Update(cashTaxHd);

            var finBalance = context.FinBalances.FirstOrDefault(x =>x.CompCode == pInput.CancelCashTax.CompCode 
                                                                && x.BrnCode == pInput.CancelCashTax.BrnCode 
                                                                && x.LocCode == pInput.CancelCashTax.LocCode
                                                                && x.DocNo == pInput.CancelCashTax.DocNo);            
            if(finBalance != null)
            {
                finBalance.BalanceAmt = decimal.Zero;
                finBalance.BalanceAmtCur = decimal.Zero;
                context.FinBalances.Update(finBalance);
            }

            if(pInput.NewCashTax.DocType == "CreditSale" || pInput.NewCashTax.DocType == "Invoice")
            {
                var finBalanceNew = new FinBalance
                {
                    CompCode = pInput.NewCashTax.CompCode,
                    BrnCode = pInput.NewCashTax.BrnCode,
                    LocCode = pInput.NewCashTax.LocCode,
                    DocType = pInput.NewCashTax.DocType,
                    DocNo = pInput.NewCashTax.DocNo,
                    DocDate = pInput.NewCashTax.DocDate,
                    CustCode = pInput.NewCashTax.CustCode,
                    Currency = pInput.NewCashTax.Currency,
                    NetAmt = Math.Round((decimal)pInput.NewCashTax.NetAmt, 2),
                    NetAmtCur = Math.Round((decimal)pInput.NewCashTax.NetAmtCur, 2),
                    BalanceAmt = Math.Round((decimal)pInput.NewCashTax.NetAmt, 2),
                    BalanceAmtCur = Math.Round((decimal)pInput.NewCashTax.NetAmtCur, 2),
                    CreatedBy = pInput.NewCashTax.CreatedBy,
                    CreatedDate = DateTime.Now
                };
                this.context.FinBalances.Add(finBalanceNew);
            }
            

            List<SalTaxinvoiceDt> listDetail = await context.SalTaxinvoiceDts.Where(
                x => x.CompCode == pInput.NewCashTax.CompCode
                && x.BrnCode == pInput.NewCashTax.BrnCode
                && x.DocNo == pInput.NewCashTax.DocNo
                && x.LocCode == pInput.NewCashTax.LocCode
            ).ToListAsync();

            if (listDetail != null && listDetail.Any())
            {
                context.SalTaxinvoiceDts.RemoveRange(listDetail);
            }
            if (pInput.NewCashTax.SalTaxinvoiceDt != null && pInput.NewCashTax.SalTaxinvoiceDt.Any())
            {
                int intSeqNo = 1;

                foreach (var taxinvoiceDt in pInput.NewCashTax.SalTaxinvoiceDt)
                {
                    var salTaxinvoiceDt = new SalTaxinvoiceDt
                    {
                        SeqNo = intSeqNo++,
                        DocNo = pInput.NewCashTax.DocNo,
                        BrnCode = pInput.NewCashTax.BrnCode,
                        CompCode = pInput.NewCashTax.CompCode,
                        LocCode = pInput.NewCashTax.LocCode,
                        LicensePlate = taxinvoiceDt.LicensePlate,
                        PdId = taxinvoiceDt.PdId,
                        PdName = taxinvoiceDt.PdName,
                        IsFree = taxinvoiceDt.IsFree,
                        UnitId = taxinvoiceDt.UnitId,
                        UnitBarcode = taxinvoiceDt.UnitBarcode,
                        UnitName = taxinvoiceDt.UnitName,
                        ItemQty = taxinvoiceDt.ItemQty,
                        StockQty = taxinvoiceDt.StockQty,
                        UnitPrice = taxinvoiceDt.UnitPrice,
                        UnitPriceCur = taxinvoiceDt.UnitPriceCur,
                        SumItemAmt = taxinvoiceDt.SumItemAmt,
                        SumItemAmtCur = taxinvoiceDt.SumItemAmtCur,
                        DiscAmt = taxinvoiceDt.DiscAmt,
                        DiscAmtCur = taxinvoiceDt.DiscAmtCur,
                        DiscHdAmt = taxinvoiceDt.DiscHdAmt,
                        DiscHdAmtCur = taxinvoiceDt.DiscHdAmtCur,
                        SubAmt = taxinvoiceDt.SubAmt,
                        SubAmtCur = taxinvoiceDt.SubAmtCur,
                        VatType = taxinvoiceDt.VatType,
                        VatRate = taxinvoiceDt.VatRate,
                        VatAmt = taxinvoiceDt.VatAmt,
                        VatAmtCur = taxinvoiceDt.VatAmtCur,
                        TaxBaseAmt = taxinvoiceDt.TaxBaseAmt,
                        TaxBaseAmtCur = taxinvoiceDt.TaxBaseAmtCur,
                        TotalAmt = taxinvoiceDt.TotalAmt,
                        TotalAmtCur = taxinvoiceDt.TotalAmtCur,

                    };
                    await context.SalTaxinvoiceDts.AddAsync(salTaxinvoiceDt);
                }
            }
        }

        public async Task<FinBalance> GetFinBalanceByCashTax(FinBalanceByCasTaxRequest pCashTax)
        {
            if (pCashTax == null)
            {
                return null;
            }
            string strBrnCode = (pCashTax?.BrnCode ?? string.Empty).Trim();
            string strLocCode = (pCashTax?.LocCode ?? string.Empty).Trim();
            string strCompCode = (pCashTax?.CompCode ?? string.Empty).Trim();
            string strDocNo = (pCashTax?.DocNo ?? string.Empty).Trim();
            //string strTaxInvoice = "TaxInvoice";
            FinBalance result = null;
            result = await context.FinBalances.Where(
                x => strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
                && strCompCode.Equals(x.CompCode)
                && strDocNo.Equals(x.DocNo)
                //&& strTaxInvoice.Equals(x.DocType)
            ).FirstOrDefaultAsync();
            return result;
        }

        public async Task<MasCustomer> GetCustomerByCustCode(CustomerByCustCodeRequset request)
        {
            var result = await context.MasCustomers.FirstOrDefaultAsync(x => x.CustCode == request.CustCode);
            return result;
        }
        private async Task adjustHeaderRunningNo(_CashTax pCashTax)
        {
            string strRunningDocNo = string.Empty;
            var docType = pCashTax.DocType == "CashTax" ? "CashTax" : "TaxInvoice";
            var qryDocPattern =
                from dp in context.MasDocPatterns.AsNoTracking()
                join dt in context.MasDocPatternDts.AsNoTracking()
                on dp.DocId equals dt.DocId
                //where "CashTax".Equals(dp.DocType)
                where dp.DocType.Equals(docType)
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
            if (pCashTax.DocDate != null && pCashTax.DocDate.HasValue)
            {
                intDay = pCashTax.DocDate.Value.Day;
                intMonth = pCashTax.DocDate.Value.Month;
                intYear = pCashTax.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<SalTaxinvoiceHd> qryTax = context.SalTaxinvoiceHds.Where(
                x => x.BrnCode == pCashTax.BrnCode
                && x.CompCode == pCashTax.CompCode
                && x.LocCode == pCashTax.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue);
            if (isUseDefaultPattern)
            {
                qryTax = qryTax.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                listDocPatternDetail = listDocPatternDetail.OrderBy(x => x.SeqNo).ToList();
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryTax = qryTax.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryTax = qryTax.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryTax = qryTax.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }

            if (await qryTax.AnyAsync())
            {
                int intMaxRunning = await qryTax.MaxAsync(x => x.RunNumber.Value);
                int intRowCount = await qryTax.CountAsync();
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
                            case "Comp": strRunningDocNo += pCashTax.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pCashTax.BrnCode; break;
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

            } while (await context.SalTaxinvoiceHds.AnyAsync(
                x => x.BrnCode == pCashTax.BrnCode
                && x.CompCode == pCashTax.CompCode
                && x.LocCode == pCashTax.LocCode
                && x.DocNo == strRunningDocNo
            ));
            pCashTax.RunNumber = intLastRunning;
            pCashTax.DocNo = strRunningDocNo;
        }
    }
}
