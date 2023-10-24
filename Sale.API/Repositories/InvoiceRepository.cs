using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Sale.API.Resources;
using Sale.API.Resources.Invoice;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sale.API.Repositories
{
    public class InvoiceRepository : Domain.Repositories.IInvoiceRepository
    {
        private const string _strInvoice = "Invoice";
        private PTMaxstationContext _context = null;
        public InvoiceRepository(PTMaxstationContext context)
        {
            _context = context;
        }

        public async Task<QueryResultResource<SalCreditsaleHd>> ListAsync(Resources.Invoice.InvoiceQueryResource query)
        {
            if (query == null)
            {
                return null;
            }
            string strIsoLevel = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            string strSelect = @"select doc_no , doc_date , Cust_Code , Cust_Name , net_Amt , doc_status , guid from SAL_CREDITSALE_HD(nolock)";
            string strCount = @"select COUNT(*) from SAL_CREDITSALE_HD(nolock)";
            string strWhere = @" where DOC_TYPE = 'Invoice' ";
            string strOrderBy = @" order by COMP_CODE, BRN_CODE, DOC_NO desc ";
            string strComCode = DefaultService.EncodeSqlString(query.COMP_CODE);
            if (!0.Equals(strComCode.Length))
            {
                strWhere += $" and COMP_CODE = '{strComCode}'";
            }
            string strBrnCode = DefaultService.EncodeSqlString(query.BRN_CODE);
            if (!0.Equals(strBrnCode.Length))
            {
                strWhere += $" and BRN_CODE = '{strBrnCode}'";
            }
            if (query.StartDate.HasValue && query.EndDate.HasValue)
            {
                strWhere += $" and DOC_DATE between '{query.StartDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}' and '{query.EndDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}'";
            }
            string strKeyWord = DefaultService.EncodeSqlString(query.KeyWord);
            if (!0.Equals(strKeyWord.Length))
            {
                strWhere += $" and ( DOC_NO like '%{strKeyWord}%' or DOC_STATUS like '%{strKeyWord}%' )";
            }
            //string strCon = _context.Database.GetConnectionString();
            int intTotal = await DefaultService.ExecuteScalar<int>(_context, strIsoLevel + strCount + strWhere);
            string strPage = string.Empty;
            if (query.Page > 0 && query.ItemsPerPage > 0)
            {
                strPage = $" OFFSET {(query.Page - 1) * query.ItemsPerPage} row fetch next {query.ItemsPerPage} row only";
            }
            var listCreditSale = await DefaultService.GetEntityFromSql<List<SalCreditsaleHd>>(
                _context, strIsoLevel + strSelect + strWhere + strOrderBy + strPage
            );
            return new QueryResultResource<SalCreditsaleHd>
            {
                Items = listCreditSale ?? new List<SalCreditsaleHd>(),
                TotalItems = intTotal,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        public async Task<QueryResultResource<SalCreditsaleHd>>
        ListAsyncOld(Resources.Invoice.InvoiceQueryResource query)
        {
            IQueryable<SalCreditsaleHd> dbSetCreditSales = _context.SalCreditsaleHds.AsNoTracking();
            int intCount = 0;
            List<MaxStation.Entities.Models.SalCreditsaleHd> listCreditSales = null;
            dbSetCreditSales = dbSetCreditSales.Where(x => "Invoice".Equals(x.DocType));
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
                dbSetCreditSales = dbSetCreditSales.OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo);
                if (query.ItemsPerPage > 0)
                {
                    if (query.Page > 0)
                    {
                        dbSetCreditSales = dbSetCreditSales.Skip((query.Page - 1) * query.ItemsPerPage);
                    }
                    dbSetCreditSales = dbSetCreditSales.Take(query.ItemsPerPage);
                }
            }
            listCreditSales = await dbSetCreditSales.ToListAsync();
            if (0.Equals(intCount))
            {
                intCount = listCreditSales.Count;
            }
            var result = new QueryResultResource<MaxStation.Entities.Models.SalCreditsaleHd>()
            {
                Items = listCreditSales,
                TotalItems = intCount
            };

            return result;
        }

        public async Task<List<Domain.Services.Communication.InvoiceDropdownResponse>>
        GetProductService()
        {
            IQueryable<MaxStation.Entities.Models.MasProduct> iqaMasProduct;
            iqaMasProduct = _context.MasProducts.AsNoTracking();

            IQueryable<MaxStation.Entities.Models.MasProductType> iqaMasProductType;
            iqaMasProductType = _context.MasProductTypes.AsNoTracking();

            IQueryable<MaxStation.Entities.Models.MasDocumentType> iqaMasDocumentType;
            iqaMasDocumentType = _context.MasDocumentTypes.AsNoTracking();

            List<Domain.Services.Communication.InvoiceDropdownResponse> result = null;
            var qryProduct = (
                from mp in iqaMasProduct
                join mpt in iqaMasProductType on mp.GroupId equals mpt.GroupId
                join mdt in iqaMasDocumentType on mpt.DocTypeId equals mdt.DocTypeId
                where "Invoice".Equals(mdt.DocTypeDesc) && "Active".Equals(mp.PdStatus)
                select new Domain.Services.Communication.InvoiceDropdownResponse()
                {
                    PdId = mp.PdId,
                    PdName = mp.PdName,
                    VatRate = mp.VatRate ,
                    VatType = mp.VatType
                }
             );
            result = await qryProduct.ToListAsync();
            return result;
        }

        public async Task<Resources.QueryResultResource<Domain.Services.Communication.InvoiceDropdownResponse>>
        GetProductService3()
        {
            IQueryable<MaxStation.Entities.Models.MasProduct> iqaMasProduct;
            iqaMasProduct = _context.MasProducts.AsNoTracking();

            IQueryable<MaxStation.Entities.Models.MasProductType> iqaMasProductType;
            iqaMasProductType = _context.MasProductTypes.AsNoTracking();

            IQueryable<MaxStation.Entities.Models.MasDocumentType> iqaMasDocumentType;
            iqaMasDocumentType = _context.MasDocumentTypes.AsNoTracking();

            List<Domain.Services.Communication.InvoiceDropdownResponse> listInvoiceDropDown;
            listInvoiceDropDown = await (
                from mp in iqaMasProduct
                join mpt in iqaMasProductType on mp.GroupId equals mpt.GroupId
                join mdt in iqaMasDocumentType on mpt.DocTypeId equals mdt.DocTypeId
                where "Invoice".Equals(mdt.DocTypeDesc) && "Active".Equals(mp.PdStatus)
                select new Domain.Services.Communication.InvoiceDropdownResponse()
                {
                    PdId = mp.PdId,
                    PdName = mp.PdName ,
                    VatType = mp.VatType ,
                    VatRate = mp.VatRate
                }
             ).ToListAsync();

            Resources.QueryResultResource<Domain.Services.Communication.InvoiceDropdownResponse> result;
            result = new QueryResultResource<Domain.Services.Communication.InvoiceDropdownResponse>();
            result.Items = listInvoiceDropDown;
            result.TotalItems = listInvoiceDropDown.Count;
            return result;
        }


        public async Task<string>
        GetRunningDocNo(string pStrCompanyCode, string pStrBranchCode, string pStrLocationCode)
        {
            if(string.IsNullOrEmpty(pStrCompanyCode))
            {
                pStrCompanyCode = string.Empty;
            }
            if (string.IsNullOrEmpty(pStrBranchCode))
            {
                pStrBranchCode = string.Empty;
            }
            if (string.IsNullOrEmpty(pStrLocationCode))
            {
                pStrLocationCode = string.Empty;
            }
            DateTime dtCurrent = DateTime.Now;
            string strCurrentMonth = dtCurrent.Month.ToString("00");
            string strCurrentYear = dtCurrent.Year.ToString();
            strCurrentYear = strCurrentYear.Substring(strCurrentYear.Length - 2, 2);
            string result = strCurrentYear + strCurrentMonth;
            IQueryable<SalCreditsaleHd> iqCreditSalesHd = _context.SalCreditsaleHds.AsNoTracking();
            List<string> listDocNo = await iqCreditSalesHd.Where(x =>
                pStrCompanyCode.Equals(x.CompCode) &&
                pStrBranchCode.Equals(x.BrnCode) &&
                pStrLocationCode.Equals(x.LocCode) &&
                (!String.IsNullOrEmpty(x.DocNo) && x.DocNo.StartsWith(result)) &&
                "Invoice".Equals(x.DocType)
                //(!x.DocDate.HasValue || (intCurrentMonth.Equals(x.DocDate.Value.Month) && intCurrentYear.Equals(x.DocDate.Value.Year)))
            ).Select(x=> x.DocNo).ToListAsync();
            if(listDocNo == null || 0.Equals(listDocNo.Count()))
            {
                result += "00001";
            }
            else
            {
                int intMaxRunning = listDocNo.Select(x => x.Substring(4)).Select(x =>
                {
                    int intDocRunningNo = 0;
                    int.TryParse(x, out intDocRunningNo);
                    return intDocRunningNo;
                }).Max();
                result += (++intMaxRunning).ToString("00000");
            }
            return result;
        }

        public List<MaxStation.Entities.Models.SalCreditsaleDt> Test()
        {

            return _context.SalCreditsaleDts.ToList();
        }

        public async Task<List<MasProduct>> GetProductService2()
        {
            IQueryable<MaxStation.Entities.Models.MasProduct> iqaMasProduct;
            iqaMasProduct = _context.MasProducts.AsNoTracking();

            IQueryable<MaxStation.Entities.Models.MasProductType> iqaMasProductType;
            iqaMasProductType = _context.MasProductTypes.AsNoTracking();

            IQueryable<MaxStation.Entities.Models.MasDocumentType> iqaMasDocumentType;
            iqaMasDocumentType = _context.MasDocumentTypes.AsNoTracking();

            List<MaxStation.Entities.Models.MasProduct> result;
            result = await(
                from mp in iqaMasProduct
                join mpt in iqaMasProductType on mp.GroupId equals mpt.GroupId
                join mdt in iqaMasDocumentType on mpt.DocTypeId equals mdt.DocTypeId
                where "Invoice".Equals(mdt.DocTypeDesc) && "Active".Equals(mp.PdStatus)
                select mp
             ).ToListAsync();
            return result;
        }

        public async Task InsertCreditSales(SalCreditsaleHd pCreditSaleHeader , SalCreditsaleDt[] pArrCreditSaleDetail)
        {
            pCreditSaleHeader = await adjustSaleHeader(pCreditSaleHeader);
            pCreditSaleHeader.CreatedDate = DateTime.Now;
            pCreditSaleHeader.Guid = Guid.NewGuid();
            pCreditSaleHeader.DocStatus = "Active";
            await _context.SalCreditsaleHds.AddAsync(pCreditSaleHeader);
            for (int i = 0; pArrCreditSaleDetail != null && i < pArrCreditSaleDetail.Length; i++)
            {
                if(pArrCreditSaleDetail[i] == null)
                {
                    continue;
                }
                pArrCreditSaleDetail[i].DocNo = pCreditSaleHeader.DocNo;
                pArrCreditSaleDetail[i].VatType = (pArrCreditSaleDetail[i].VatType ?? string.Empty).Trim();
                if ("NV".Equals(pArrCreditSaleDetail[i].VatType))
                {
                    pArrCreditSaleDetail[i].VatRate = 0;
                }
            }
            var listDt = _context.SalCreditsaleDts.Where(x =>
                x.CompCode == pCreditSaleHeader.CompCode
                && x.BrnCode == pCreditSaleHeader.BrnCode
                && x.LocCode == pCreditSaleHeader.LocCode
                && "Invoice".Equals(x.DocType)
                && x.DocNo == pCreditSaleHeader.DocNo
            );
            if(await listDt.AnyAsync())
            {
                _context.SalCreditsaleDts.RemoveRange(listDt);
            }
            await _context.SalCreditsaleDts.AddRangeAsync(pArrCreditSaleDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCreditSales(SalCreditsaleHd pCreditSaleHeader, SalCreditsaleDt[] pArrCreditSaleDetail)
        {
            _context.SalCreditsaleHds.Update(pCreditSaleHeader);
            pCreditSaleHeader.UpdatedDate = DateTime.Now;
            var listDt = await _context.SalCreditsaleDts.Where(x => 
                x.BrnCode == pCreditSaleHeader.BrnCode &&
                x.CompCode == pCreditSaleHeader.CompCode &&
                x.DocNo == pCreditSaleHeader.DocNo &&
                x.LocCode == pCreditSaleHeader.LocCode &&
                "Invoice".Equals(x.DocType)
            ).ToListAsync();
            if (listDt.Any())
            {
                _context.SalCreditsaleDts.RemoveRange(listDt);
            }
            int intSeqNo = 1;
            foreach (var item in pArrCreditSaleDetail)
            {
                if(item == null)
                {
                    continue;
                }
                item.SeqNo = intSeqNo++;
                item.DocNo = pCreditSaleHeader.DocNo;
                item.CompCode = pCreditSaleHeader.CompCode;
                item.BrnCode = pCreditSaleHeader.BrnCode;
                item.LocCode = pCreditSaleHeader.LocCode;
                item.VatType = (item.VatType ?? string.Empty).Trim();
                if ("NV".Equals(item.VatType))
                {
                    item.VatRate = 0;
                }
            }
            await _context.SalCreditsaleDts.AddRangeAsync(pArrCreditSaleDetail);
            //await _context.SaveChangesAsync();
        }

        public async Task<string> GetRunningPattern(string pStrCompanyCode, string pStrLocationCode, string pStrBranchCode)
        {
            string result = string.Empty;
            var docPattern = await _context.MasDocPatterns.FirstOrDefaultAsync(x => "Invoice".Equals(x.DocType));
            List<MasDocPatternDt> listDocPatternDetail = null;
            bool isUseDefaultPattern = true;
            if (docPattern != null)
            {
                listDocPatternDetail = await _context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
                isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            }
            //int intLastRunning = 1;
            int intDay = DateTime.Now.Day;
            int intMonth = DateTime.Now.Month;
            int intYear = DateTime.Now.Year;
            //var invoice = _context.SalCreditsaleHds.Where(x => x.BrnCode == pStrBranchCode && x.CompCode == pStrCompanyCode && x.LocCode == pStrLocationCode && x.DocDate.HasValue && "Invoice".Equals(x.DocType));
            //if (isUseDefaultPattern)
            //{
            //    invoice = invoice.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            //}
            //else
            //{
            //    if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
            //    {
            //        invoice = invoice.Where(x => intYear.Equals(x.DocDate.Value.Year));
            //        if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
            //        {
            //            invoice = invoice.Where(x => intMonth.Equals(x.DocDate.Value.Month));
            //            if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
            //            {
            //                invoice = invoice.Where(x => intDay.Equals(x.DocDate.Value.Day));
            //            }
            //        }
            //    }
            //}
            //var listInvoice = await invoice.ToListAsync();

            //if (listInvoice.Any())
            //{
            //    intLastRunning = listInvoice.Max(x => x.RunNumber.Value) + 1;
            //}
            if (isUseDefaultPattern)
            {
                result = string.Format("{0}{1}#####", intYear, intMonth);
            }
            else
            {
                foreach (var item in listDocPatternDetail)
                {
                    if (item == null) continue;
                    switch (item.DocCode)
                    {
                        case "-": result += "-"; break;
                        case "MM": result += intMonth.ToString("00"); break;
                        case "Comp": result += pStrCompanyCode; break;
                        case "[Pre]": result += item.DocValue; break;
                        case "dd": result += intDay.ToString("00"); break;
                        case "Brn": result += pStrBranchCode; break;
                        case "yyyy": result += intYear.ToString("0000"); break;
                        case "yy": result += intYear.ToString().Substring(2, 2); break;
                        case "[#]":
                            int intDocValue = 0;
                            int.TryParse(item.DocValue, out intDocValue);
                            result += new string('#', intDocValue);
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }
        
        public async Task<string> GetRunningNo(string pStrCompanyCode, string pStrLocationCode, string pStrBranchCode)
        {
            string result = string.Empty;
            var docPattern = await _context.MasDocPatterns.FirstOrDefaultAsync(x => "Invoice".Equals(x.DocType));
            List<MasDocPatternDt> listDocPatternDetail = null;
            bool isUseDefaultPattern = true;
            if (docPattern != null)
            {
                listDocPatternDetail = await _context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
                isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            }
            int intLastRunning = 1;
            int intDay = DateTime.Now.Day;
            int intMonth = DateTime.Now.Month;
            int intYear = DateTime.Now.Year;
            var invoice = _context.SalCreditsaleHds.Where(x => x.BrnCode == pStrBranchCode && x.CompCode == pStrCompanyCode && x.LocCode == pStrLocationCode && x.DocDate.HasValue && "Invoice".Equals(x.DocType));
            if (isUseDefaultPattern)
            {
                invoice = invoice.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    invoice = invoice.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        invoice = invoice.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            invoice = invoice.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            var listInvoice = await invoice.ToListAsync();

            if (listInvoice.Any())
            {
                intLastRunning = listInvoice.Max(x => x.RunNumber.Value) + 1;
            }            
            if (isUseDefaultPattern)
            {
                result = string.Format("{0}{1}{2:D5}", intYear, intMonth, intLastRunning);
            }
            else
            {
                foreach (var item in listDocPatternDetail)
                {
                    if (item == null) continue;
                    switch (item.DocCode)
                    {
                        case "-": result += "-"; break;
                        case "MM": result += intMonth.ToString("00"); break;
                        case "Comp": result += pStrCompanyCode; break;
                        case "[Pre]": result += item.DocValue; break;
                        case "dd": result += intDay.ToString("00"); break;
                        case "Brn": result += pStrBranchCode; break;
                        case "yyyy": result += intYear.ToString("0000"); break;
                        case "yy": result += intYear.ToString().Substring(2, 2); break;
                        case "[#]":
                            int intDocValue = 0;
                            int.TryParse(item.DocValue, out intDocValue);
                            result += intLastRunning.ToString().PadLeft(intDocValue, '0');
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        private async Task<SalCreditsaleHd> adjustSaleHeader( SalCreditsaleHd pCreditSalesHeader)
        {
            if(pCreditSalesHeader == null)
            {
                return null;
            }
            string pStrCompanyCode = pCreditSalesHeader.CompCode;
            string pStrLocationCode = pCreditSalesHeader.LocCode; 
            string pStrBranchCode = pCreditSalesHeader.BrnCode;
            string strDocNo = string.Empty;
            var docPattern = await _context.MasDocPatterns.FirstOrDefaultAsync(x => "Invoice".Equals(x.DocType));
            List<MasDocPatternDt> listDocPatternDetail = null;
            bool isUseDefaultPattern = true;
            if (docPattern != null)
            {
                if(docPattern.MasDocPatternDt != null && docPattern.MasDocPatternDt.Any())
                {
                    listDocPatternDetail = docPattern.MasDocPatternDt;
                }
                else
                {
                    listDocPatternDetail = await _context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
                }
                isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            }
            int intLastRunning = 0;
            var dateSystem = pCreditSalesHeader?.DocDate ?? DateTime.Now;
            int intDay = dateSystem.Day;
            int intMonth = dateSystem.Month;
            int intYear = dateSystem.Year;
            var invoice = _context.SalCreditsaleHds.Where(x => 
                x.BrnCode == pStrBranchCode 
                && x.CompCode == pStrCompanyCode 
                && x.LocCode == pStrLocationCode 
                && x.DocDate.HasValue 
                && "Invoice".Equals(x.DocType)
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                invoice = invoice.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    invoice = invoice.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        invoice = invoice.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            invoice = invoice.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            var listInvoice = await invoice.ToListAsync();

            if (listInvoice.Any())
            {
                intLastRunning = listInvoice.Max(x => x.RunNumber.Value);
            }

            Func<Task<bool>> funcIsDuplicate = async () =>
            {
                bool isDuplicate = await _context.SalCreditsaleHds.AnyAsync(x =>
                    x.BrnCode == pStrBranchCode
                    && x.CompCode == pStrCompanyCode
                    && x.LocCode == pStrLocationCode
                    && x.DocNo == strDocNo
                    && "Invoice".Equals(x.DocType)
                );
                return isDuplicate;
            };
            do
            {
                intLastRunning++;
                strDocNo = string.Empty;
                if (isUseDefaultPattern)
                {
                    strDocNo = string.Format("{0}{1}{2:D5}", intYear, intMonth, intLastRunning);
                }
                else
                {
                    foreach (var item in listDocPatternDetail)
                    {
                        if (item == null) continue;
                        switch (item.DocCode)
                        {
                            case "-": strDocNo += "-"; break;
                            case "MM": strDocNo += intMonth.ToString("00"); break;
                            case "Comp": strDocNo += pStrCompanyCode; break;
                            case "[Pre]": strDocNo += item.DocValue; break;
                            case "dd": strDocNo += intDay.ToString("00"); break;
                            case "Brn": strDocNo += pStrBranchCode; break;
                            case "yyyy": strDocNo += intYear.ToString("0000"); break;
                            case "yy": strDocNo += intYear.ToString().Substring(2, 2); break;
                            case "[#]":
                                int intDocValue = 0;
                                int.TryParse(item.DocValue, out intDocValue);
                                strDocNo += intLastRunning.ToString().PadLeft(intDocValue, '0');
                                break;
                            default:
                                break;
                        }
                    }
                }
            } while (await funcIsDuplicate());
            
            //pCreditSalesHeader.DocPattern = docPattern.Pattern;
            pCreditSalesHeader.DocType = "Invoice";
            pCreditSalesHeader.RunNumber = intLastRunning;
            pCreditSalesHeader.DocNo = strDocNo;
            //pCreditSalesHeader.Guid = new Guid();
            return pCreditSalesHeader;
        }

        private async Task<SalCreditsaleHd> adjustSaleHeaderOld(SalCreditsaleHd pCreditSalesHeader)
        {
            if (pCreditSalesHeader == null)
            {
                return null;
            }
            string pStrCompanyCode = pCreditSalesHeader.CompCode;
            string pStrLocationCode = pCreditSalesHeader.LocCode;
            string pStrBranchCode = pCreditSalesHeader.BrnCode;
            string result = string.Empty;
            var docPattern = await _context.MasDocPatterns.FirstOrDefaultAsync(x => "Invoice".Equals(x.DocType));
            List<MasDocPatternDt> listDocPatternDetail = null;
            bool isUseDefaultPattern = true;
            if (docPattern != null)
            {
                if (docPattern.MasDocPatternDt != null && docPattern.MasDocPatternDt.Any())
                {
                    listDocPatternDetail = docPattern.MasDocPatternDt;
                }
                else
                {
                    listDocPatternDetail = await _context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
                }
                isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            }
            int intLastRunning = 1;
            int intDay = DateTime.Now.Day;
            int intMonth = DateTime.Now.Month;
            int intYear = DateTime.Now.Year;
            var invoice = _context.SalCreditsaleHds.Where(x =>
                x.BrnCode == pStrBranchCode
                && x.CompCode == pStrCompanyCode
                && x.LocCode == pStrLocationCode
                && x.DocDate.HasValue
                && "Invoice".Equals(x.DocType)
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                invoice = invoice.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    invoice = invoice.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        invoice = invoice.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            invoice = invoice.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            var listInvoice = await invoice.ToListAsync();

            if (listInvoice.Any())
            {
                intLastRunning = listInvoice.Max(x => x.RunNumber.Value) + 1;
            }
            if (isUseDefaultPattern)
            {
                result = string.Format("{0}{1}{2:D5}", intYear, intMonth, intLastRunning);
            }
            else
            {
                foreach (var item in listDocPatternDetail)
                {
                    if (item == null) continue;
                    switch (item.DocCode)
                    {
                        case "-": result += "-"; break;
                        case "MM": result += intMonth.ToString("00"); break;
                        case "Comp": result += pStrCompanyCode; break;
                        case "[Pre]": result += item.DocValue; break;
                        case "dd": result += intDay.ToString("00"); break;
                        case "Brn": result += pStrBranchCode; break;
                        case "yyyy": result += intYear.ToString("0000"); break;
                        case "yy": result += intYear.ToString().Substring(2, 2); break;
                        case "[#]":
                            int intDocValue = 0;
                            int.TryParse(item.DocValue, out intDocValue);
                            result += intLastRunning.ToString().PadLeft(intDocValue, '0');
                            break;
                        default:
                            break;
                    }
                }
            }
            pCreditSalesHeader.DocPattern = docPattern.Pattern;
            pCreditSalesHeader.DocType = "Invoice";
            pCreditSalesHeader.RunNumber = intLastRunning;
            pCreditSalesHeader.DocNo = result;
            pCreditSalesHeader.Guid = new Guid();
            return pCreditSalesHeader;
        }

        public async Task<InsertCreditSalesQuery>  GetCreditSales( GetInvoiceQueryResource param)
        {
            var result = new InsertCreditSalesQuery();
            result.CreditSaleHeader = await _context.SalCreditsaleHds
                    .FirstOrDefaultAsync(x =>
                        x.DocNo == param.DocNo &&
                        x.BrnCode == param.BrnCode &&
                        x.CompCode == param.CompCode &&
                        x.LocCode == param.LocCode &&
                        "Invoice".Equals(x.DocType)
                    );
            result.ArrCreditSaleDetail = await _context.SalCreditsaleDts
                .Where(x =>
                    x.DocNo == param.DocNo &&
                    x.BrnCode == param.BrnCode &&
                    x.CompCode == param.CompCode &&
                    x.CompCode == param.CompCode &&
                    x.LocCode == param.LocCode &&
                    "Invoice".Equals(x.DocType)
                ).ToArrayAsync();
            return result;
        }
        public async Task<InsertCreditSalesQuery> GetCreditSalesByGuid(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }
            Guid guid;
            if(!Guid.TryParse(pStrGuid , out guid))
            {
                return null;
            }
            IQueryable<SalCreditsaleHd> qryHeader = null;
            qryHeader = _context.SalCreditsaleHds
                .AsNoTracking()
                .Where(x => guid.Equals(x.Guid));

            SalCreditsaleHd header = null;
            header = await qryHeader.FirstOrDefaultAsync();

            SalCreditsaleDt[] arrDetail = null;
            if(header != null)
            {
                IQueryable<SalCreditsaleDt> qryDetail = null;
                qryDetail = _context.SalCreditsaleDts.AsNoTracking().Where(
                    x => _strInvoice.Equals(x.DocType)
                    && x.DocNo == header.DocNo
                    && x.CompCode == header.CompCode
                    && x.BrnCode == header.BrnCode
                    && x.LocCode == header.LocCode
                );
                arrDetail = await qryDetail.ToArrayAsync();
            }

            InsertCreditSalesQuery result = null;
            result = new InsertCreditSalesQuery() { 
                CreditSaleHeader = header,
                ArrCreditSaleDetail = arrDetail
            };
            return result;
        }
    }
}
