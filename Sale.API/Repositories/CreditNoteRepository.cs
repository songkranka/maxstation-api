using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Sale.API.Resources;
using Sale.API.Resources.CreditNote;
using Sale.API.Resources.Invoice;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Repositories
{
    public class CreditNoteRepository : Domain.Repositories.ICreditNoteRepository
    {
        private PTMaxstationContext _context = null;
        public CreditNoteRepository(PTMaxstationContext context)
        {
            _context = context;
        }
        public async Task<QueryResultResource<SalCndnHd>> ListAsync(CreditNoteQueryResource query)
        {
            if (query == null)
            {
                return null;
            }
            string strIsoLevel = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            string strSelect = @"select doc_no , doc_date , doc_type , Cust_Code , Cust_Name , net_Amt , doc_status , guid from SAL_CNDN_HD(nolock)";
            string strCount = @"select COUNT(*) from SAL_CNDN_HD(nolock)";
            string strWhere = string.Empty;
            string strOrderBy = @" order by COMP_CODE, BRN_CODE, DOC_NO desc ";
            var listWhere = new List<string>();
            string strComCode = DefaultService.EncodeSqlString(query.COMP_CODE);
            if (!0.Equals(strComCode.Length))
            {
                listWhere.Add($"COMP_CODE = '{strComCode}'");
            }
            string strBrnCode = DefaultService.EncodeSqlString(query.BRN_CODE);
            if (!0.Equals(strBrnCode.Length))
            {
                listWhere.Add($"BRN_CODE = '{strBrnCode}'");
            }
            if (query.StartDate.HasValue && query.EndDate.HasValue)
            {
                listWhere.Add($"DOC_DATE between '{DefaultService.EncodeSqlDate(query.StartDate.Value)}' and '{DefaultService.EncodeSqlDate(query.EndDate.Value)}'");
            }
            string strKeyWord = DefaultService.EncodeSqlString(query.KeyWord);
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
            var listCreditNote = await DefaultService.GetEntityFromSql<List<SalCndnHd>>(
                _context, strIsoLevel + strSelect + strWhere + strOrderBy + strPage
            );
            int intTotal = await DefaultService.ExecuteScalar<int>(_context, strIsoLevel + strCount + strWhere);
            return new QueryResultResource<SalCndnHd>
            {
                Items = listCreditNote ?? new List<SalCndnHd>(),
                TotalItems = intTotal,
                ItemsPerPage = query.ItemsPerPage
            };
        }
        public async Task<QueryResultResource<SalCndnHd>>
        ListAsyncOld(Resources.CreditNote.CreditNoteQueryResource query)
        {
            IQueryable<SalCndnHd> dbSetCreditSales = _context.SalCndnHds.AsNoTracking();
            int intCount = 0;
            List<MaxStation.Entities.Models.SalCndnHd> listCreditSales = null;
            //dbSetCreditSales = dbSetCreditSales.Where(x => "Invoice".Equals(x.DocType));
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.BRN_CODE))
                {
                    dbSetCreditSales = dbSetCreditSales.Where(x => query.BRN_CODE.Equals(x.BrnCode));
                }
                if (!string.IsNullOrEmpty(query.COMP_CODE))
                {
                    dbSetCreditSales = dbSetCreditSales.Where(x => query.COMP_CODE.Equals(x.CompCode));
                }
                if (!string.IsNullOrEmpty(query.LOC_CODE))
                {
                    dbSetCreditSales = dbSetCreditSales.Where(x => query.LOC_CODE.Equals(x.LocCode));
                }
                if (query.StartDate != null)
                {
                    dbSetCreditSales = dbSetCreditSales.Where(x => query.StartDate <= x.DocDate);
                }
                if (query.EndDate != null)
                {
                    dbSetCreditSales = dbSetCreditSales.Where(x => query.EndDate >= x.DocDate);
                }
                if (!string.IsNullOrEmpty(query.KeyWord))
                {

                    dbSetCreditSales = dbSetCreditSales.Where(x =>
                        (!string.IsNullOrEmpty(x.DocNo) && x.DocNo.Contains(query.KeyWord)) ||
                        (!string.IsNullOrEmpty(x.CustCode) && x.CustCode.Contains(query.KeyWord)) ||
                        (!string.IsNullOrEmpty(x.CustName) && x.CustName.Contains(query.KeyWord))
                    );
                }
                intCount = await dbSetCreditSales.CountAsync();
                if (query.ItemsPerPage > 0)
                {
                    if (query.Page > 0)
                    {
                        dbSetCreditSales = dbSetCreditSales.Skip((query.Page - 1) * query.ItemsPerPage);
                    }
                    dbSetCreditSales = dbSetCreditSales.Take(query.ItemsPerPage);
                }
            }
            listCreditSales = await dbSetCreditSales.OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo).ToListAsync();
            if (0.Equals(intCount))
            {
                intCount = listCreditSales.Count;
            }
            var result = new QueryResultResource<MaxStation.Entities.Models.SalCndnHd>()
            {
                Items = listCreditSales,
                TotalItems = intCount
            };

            return result;
        }


        public async Task< CreditNoteQueryResource2 > GetCreditNote(String pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }

            CreditNoteQueryResource2 result = null;
            result = new CreditNoteQueryResource2();

            Guid guid = Guid.Parse(pStrGuid);

            result.CreditNoteHeader = await _context.SalCndnHds.FirstOrDefaultAsync(x => x.Guid == guid);
            if(result.CreditNoteHeader != null)
            {
                string strDocNo = string.Empty;
                strDocNo = (result.CreditNoteHeader.DocNo ?? string.Empty).Trim();

                string strComCode = string.Empty;
                strComCode = (result.CreditNoteHeader.CompCode ?? string.Empty).Trim();

                string strBrnCode = string.Empty;
                strBrnCode = (result.CreditNoteHeader.BrnCode ?? string.Empty).Trim();

                string strLocCode = string.Empty;
                strLocCode = (result.CreditNoteHeader.LocCode ?? string.Empty).Trim();

                string strDocType = string.Empty;
                strDocType = (result.CreditNoteHeader.DocType ?? string.Empty).Trim();

                var custPrefix = await _context.MasCustomers.Where(x => x.CustCode == result.CreditNoteHeader.CustCode).Select(x => x.CustPrefix).FirstOrDefaultAsync();
                result.CreditNoteHeader.CustPrefix = custPrefix;

                IQueryable<SalCndnDt> qryDetail = null;
                qryDetail = _context.SalCndnDts.Where(
                    x => strDocNo.Equals(x.DocNo)
                    && strDocType.Equals(x.DocType)
                    && strComCode.Equals(x.CompCode)
                    && strBrnCode.Equals(x.BrnCode)
                    && strLocCode.Equals(x.LocCode)
                );
                result.ArrCreditNoteDetail = await qryDetail.ToArrayAsync();

                result.FinBalance = await _context.FinBalances.FirstOrDefaultAsync(
                    x => strBrnCode.Equals(x.BrnCode)
                    && strComCode.Equals(x.CompCode)
                    && strDocNo.Equals(x.DocNo)
                    && strDocType.Equals(x.DocType)
                    && strLocCode.Equals(x.LocCode)
                );
            }
            return result;
        }


        public async Task SaveCreditNote(CreditNoteQueryResource2 pInput)
        {
            if(pInput == null || pInput.CreditNoteHeader == null)
            {
                return;
            }
            SalCndnHd header = null;
            header = pInput.CreditNoteHeader;
            string strDocNo = string.Empty;
            

            string strComCode = string.Empty;
            strComCode = (header.CompCode ?? string.Empty).Trim();

            string strBrnCode = string.Empty;
            strBrnCode = (header.BrnCode ?? string.Empty).Trim();

            string strLocCode = string.Empty;
            strLocCode = (header.LocCode ?? string.Empty).Trim();

            string strDocType = string.Empty;
            strDocType = (header.DocType ?? string.Empty).Trim();

            if ("New".Equals(header.DocStatus))
            {
                await adjustHeaderRunningNo(header);
                strDocNo = (header.DocNo ?? string.Empty).Trim();
                header.CreatedDate = DateTime.Now;
                header.Guid = Guid.NewGuid();
                header.DocStatus = "Active";
                await _context.SalCndnHds.AddAsync(header);
            }
            else
            {
                strDocNo = (header.DocNo ?? string.Empty).Trim();
                header.UpdatedDate = DateTime.Now;
                _context.SalCndnHds.Update(header);
            }


            //CreditNote Detail
            SalCndnDt[] arrExistDetail = null;
            arrExistDetail = await _context.SalCndnDts.Where(
                x => x.DocNo == strDocNo
                && x.DocType == strDocType
                && x.LocCode == strLocCode
                && x.BrnCode == strBrnCode
                && x.CompCode == strComCode
            ).ToArrayAsync();
            if(arrExistDetail != null && arrExistDetail.Any())
            {
                _context.RemoveRange(arrExistDetail);
            }
            SalCndnDt[] arrDetail = null;
            arrDetail = pInput.ArrCreditNoteDetail;
            if(arrDetail != null && arrDetail.Any())
            {
                int intSeq = 1;
                foreach (SalCndnDt item in arrDetail)
                {
                    if(item == null)
                    {
                        continue;
                    }
                    item.SeqNo = intSeq++;
                    item.DocNo = strDocNo;
                    item.DocType = strDocType;
                    item.CompCode = strComCode;
                    item.BrnCode = strBrnCode;
                    item.LocCode = strLocCode;

                }
                await _context.SalCndnDts.AddRangeAsync(arrDetail);
            }

            //FinBalance
            if(pInput.FinBalance != null)
            {
                FinBalance fin = null;
                fin = pInput.FinBalance;
                fin.DocNo = strDocNo;
                fin.DocType = strDocType;
                fin.CompCode = strComCode;
                fin.BrnCode = strBrnCode;
                fin.LocCode = strLocCode;
                bool isExist = await _context.FinBalances.AnyAsync(
                    x=> strDocNo.Equals(x.DocNo)
                    && strDocType.Equals(x.DocType)
                    && strComCode.Equals(x.CompCode)
                    && strBrnCode.Equals(x.BrnCode)
                    && strLocCode.Equals(x.LocCode)
                );
                if (isExist)
                {
                    fin.UpdatedDate = DateTime.Now;
                    _context.FinBalances.Update(fin);
                }
                else
                {
                    fin.CreatedDate = DateTime.Now;
                    await _context.FinBalances.AddAsync(fin);
                }
            }
        }
        public async Task<SearchTaxInvoiceResult> SearchTaxInvoice(SearchTaxInvoiceParam param)
        {
            if(param == null)
            {
                return null;
            }
            var qryTax = _context.SalTaxinvoiceHds.AsNoTracking();
            param.CompCode = (param.CompCode ?? string.Empty).Trim();
            if (!0.Equals(param.CompCode.Length))
            {
                qryTax = qryTax.Where(x => x.CompCode == param.CompCode);
            }
            param.CustCode = (param.CustCode ?? string.Empty).Trim();
            if (!0.Equals(param.CustCode.Length))
            {
                qryTax = qryTax.Where(x => x.CustCode == param.CustCode);
            }
            param.DocNo = (param.DocNo ?? string.Empty).Trim();
            if (!0.Equals(param.DocNo.Length))
            {
                qryTax = qryTax.Where(x => x.DocNo == param.DocNo);
            }
            var result = new SearchTaxInvoiceResult();
            result.TotalItem = await qryTax.CountAsync();
            if(result.TotalItem > 0)
            {
                qryTax = qryTax.OrderByDescending(x => x.DocDate);
                if (param.ItemsPerPage > 0)
                {
                    if (param.Page > 0)
                    {
                        var intSkip = (param.Page - 1) * param.ItemsPerPage;
                        qryTax = qryTax.Skip((param.Page - 1) * param.ItemsPerPage);
                    }
                    qryTax = qryTax.Take(param.ItemsPerPage);
                }
                result.ArrTaxInvoice = await qryTax.ToArrayAsync();
            }
            return result;
        }

        public async Task<SalTaxinvoiceHd[]> GetTaxInvoiceList(CreditNoteQueryResource2 pInput)
        {
            string strComcode = string.Empty; 
            strComcode = (pInput?.CreditNoteHeader?.CompCode ?? string.Empty).Trim();

            string strCusCode = string.Empty;
            strCusCode = (pInput?.CreditNoteHeader?.CustCode ?? string.Empty).Trim();

            IQueryable<SalTaxinvoiceHd> qryTax = null;
            qryTax = _context.SalTaxinvoiceHds.AsNoTracking()
                .Where(x=> !"Cancel".Equals( x.DocStatus));
            if (!0.Equals(strComcode.Length))
            {
                qryTax = qryTax.Where(x => strComcode.Equals(x.CompCode));
            }
            if (!0.Equals(strCusCode.Length))
            {
                qryTax = qryTax.Where(x => strCusCode.Equals(x.CustCode));
            }

            SalTaxinvoiceHd[] result = null;
            result = await qryTax.ToArrayAsync();
            return result;
        }

        public async Task<SalTaxinvoiceDt[]> GetTaxInvoiceDetailList(SalTaxinvoiceHd pInput)
        {
            if(pInput == null)
            {
                return null;
            }
            
            string strDocNo = string.Empty;
            strDocNo = (pInput.DocNo ?? string.Empty).Trim();

            string strComCode = string.Empty;
            strComCode = (pInput.CompCode ?? string.Empty).Trim();

            string strBrnCode = string.Empty;
            strBrnCode = (pInput.BrnCode ?? string.Empty).Trim();

            string strLocCode = string.Empty;
            strLocCode = (pInput.LocCode ?? string.Empty).Trim();

            IQueryable<SalTaxinvoiceDt> qryDetail = null;
            qryDetail = _context.SalTaxinvoiceDts.AsNoTracking().Where(
                x => strDocNo == x.DocNo
                && strComCode == x.CompCode
                && strBrnCode == x.BrnCode
                && strLocCode == x.LocCode
            );

            SalTaxinvoiceDt[] result = null;
            result = await qryDetail.ToArrayAsync();

            return result;
        }

        private async Task adjustHeaderRunningNo(SalCndnHd pCreditNote)
        {
            if(pCreditNote == null)
            {
                return;
            }
            
            string strRunningDocNo = string.Empty;
            var qryDocPattern =
                from dp in _context.MasDocPatterns.AsNoTracking()
                join dt in _context.MasDocPatternDts.AsNoTracking()
                on dp.DocId equals dt.DocId
                where pCreditNote.DocType == dp.DocType
                select new MasDocPatternDt()
                {
                    DocValue = dt.DocValue,
                    DocCode = dt.DocCode,
                    SeqNo = dt.SeqNo
                };
            List<MasDocPatternDt> listDocPatternDetail = null;
            listDocPatternDetail = await qryDocPattern.ToListAsync();
            bool isUseDefaultPattern = true;
            isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            int intLastRunning = 0;
            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pCreditNote.DocDate != null && pCreditNote.DocDate.HasValue)
            {
                intDay = pCreditNote.DocDate.Value.Day;
                intMonth = pCreditNote.DocDate.Value.Month;
                intYear = pCreditNote.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<SalCndnHd> qryCreditNote = _context.SalCndnHds.Where(
                x => x.BrnCode == pCreditNote.BrnCode
                && x.CompCode == pCreditNote.CompCode
                && x.LocCode == pCreditNote.LocCode
                && x.DocType == pCreditNote.DocType
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                qryCreditNote = qryCreditNote.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                listDocPatternDetail = listDocPatternDetail.OrderBy(x => x.SeqNo).ToList();
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryCreditNote = qryCreditNote.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryCreditNote = qryCreditNote.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryCreditNote = qryCreditNote.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }

            if (await qryCreditNote.AnyAsync())
            {
                int intMaxRunning = await qryCreditNote.MaxAsync(x => x.RunNumber.Value);
                int intRowCount = await qryCreditNote.CountAsync();
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
                            case "Comp": strRunningDocNo += pCreditNote.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pCreditNote.BrnCode; break;
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

            } while (await _context.SalCndnHds.AnyAsync(
                x => x.BrnCode == pCreditNote.BrnCode
                && x.CompCode == pCreditNote.CompCode
                && x.LocCode == pCreditNote.LocCode
                && x.DocType == pCreditNote.DocType
                && x.DocNo == strRunningDocNo
            ));
            pCreditNote.RunNumber = intLastRunning;
            pCreditNote.DocNo = strRunningDocNo;
        }
    }
}
