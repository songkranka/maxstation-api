using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Sale.API.Domain.Repositories;
using Sale.API.Domain.Services;
using Sale.API.Repositories;
using Sale.API.Resources;
using Sale.API.Resources.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sale.API.Resources.Billing.BillingQueryResource;

namespace Sale.API.Services
{
    public class BillingService 
    {
        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWork = null;
        public BillingService(PTMaxstationContext pContext , IUnitOfWork pUnitOfWork)
        {
            _context = pContext;
            _unitOfWork = pUnitOfWork;
        }

        public async Task<ModelBillingResult> SearchBilling(SearchBillingQueryResource query)
        {
            if (query == null)
            {
                return null;
            }
            string strIsoLevel = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            string strSelect = @"select doc_no , doc_date , Created_By , Cust_Code , Cust_Name , Total_Amt , doc_status , guid from SAL_BILLING_HD(nolock)";
            string strCount = @"select COUNT(*) from SAL_BILLING_HD(nolock)";
            string strWhere = string.Empty;
            string strOrderBy = @" order by COMP_CODE, BRN_CODE, DOC_NO desc ";
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
            var arrHeader = await DefaultService.GetEntityFromSql<SalBillingHd[]>(
                _context, strIsoLevel + strSelect + strWhere + strOrderBy + strPage
            );
            int intTotal = await DefaultService.ExecuteScalar<int>(_context, strIsoLevel + strCount + strWhere);
            var result = new ModelBillingResult();
            result.ArrHeader = arrHeader ?? new SalBillingHd[0];
            result.TotalItems = intTotal;
            if (arrHeader != null && arrHeader.Any())
            {
                var arrEmpId = arrHeader.Where(x => !string.IsNullOrWhiteSpace(x.CreatedBy))
                    .Select(x => x.CreatedBy)
                    .Distinct().ToArray();
                if (arrEmpId != null && arrEmpId.Any())
                {
                    var qryEmp = _context.MasEmployees.Where(
                        x => arrEmpId.Contains(x.EmpCode)
                        && x.WorkStatus == "Active"
                    ).AsNoTracking();
                    result.ArrEmployee = await qryEmp.ToArrayAsync();
                }
            }
            return result;
        }
        public async Task<ModelBillingResult> SearchBillingOld(SearchBillingQueryResource query)
        {
            var result = new ModelBillingResult();
            var salBillingHeader = _context.SalBillingHds.AsNoTracking().Where(x =>
                x.CompCode == query.CompCode
                && x.BrnCode == query.BrnCode
                && x.LocCode == query.LocCode
            );
            if (query.FromDate.HasValue && query.ToDate.HasValue)
            {
                salBillingHeader = salBillingHeader.Where(
                    x => x.DocDate >= query.FromDate.Value
                    && x.DocDate <= query.ToDate.Value);
            }
            string strKeyWord = string.Empty;
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                strKeyWord = query.Keyword.Trim();
            }
            if (!0.Equals(strKeyWord.Length))
            {
                salBillingHeader = salBillingHeader.Where(x =>
                    (!string.IsNullOrEmpty(x.CustCode) && x.CustCode.Contains(strKeyWord))
                    || (!string.IsNullOrEmpty(x.CustName) && x.CustName.Contains(strKeyWord))
                    || (!string.IsNullOrEmpty(x.DocNo) && x.DocNo.Contains(strKeyWord))
                );
            }
            int intCount = await salBillingHeader.CountAsync();
            salBillingHeader = salBillingHeader.OrderByDescending(x => x.CreatedDate);
            if(query.Page > 0 && query.ItemsPerPage > 0)
            {
                salBillingHeader = salBillingHeader.Skip((query.Page - 1) * query.ItemsPerPage).Take(query.ItemsPerPage);
            }
            var arrHeader = await salBillingHeader.ToArrayAsync();
            result.ArrHeader = arrHeader;
            result.TotalItems = intCount;
            
            if(arrHeader != null && arrHeader.Any())
            {
                var arrEmpId = arrHeader.Where(x=> !string.IsNullOrWhiteSpace(x.CreatedBy))
                    .Select(x => x.CreatedBy)
                    .Distinct().ToArray();
                if(arrEmpId != null && arrEmpId.Any())
                {
                    var qryEmp = _context.MasEmployees.Where(
                        x => arrEmpId.Contains(x.EmpCode)
                        && x.WorkStatus == "Active"
                    ).AsNoTracking();
                    result.ArrEmployee = await qryEmp.ToArrayAsync();
                }
            }
            return result;
        }
        public async Task<ModelBilling> GetBilling(string pStrGuid)
        {
            pStrGuid = DefaultService.GetString(pStrGuid);
            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }
            Guid guid;
            if(!Guid.TryParse(pStrGuid,out guid))
            {
                return null;
            }
            var qryBillingHeader = _context.SalBillingHds
                .Where(x => guid == x.Guid)
                .AsNoTracking();
            var header = await qryBillingHeader.FirstOrDefaultAsync();
            if(header == null)
            {
                return null;
            }
            var qryBillingDetail = _context.SalBillingDts.Where(
                x => x.DocNo == header.DocNo
                && x.CompCode == header.CompCode
                && x.LocCode == header.LocCode
                && x.BrnCode == header.BrnCode
            ).AsNoTracking();
            var arrDetail = await qryBillingDetail.ToArrayAsync();
            var result = new ModelBilling()
            {
                Header = header ,
                ArrDetail = arrDetail
            };
            return result;
        }
        public async Task<SalTaxinvoiceHd[]> GetTaxInvoice(string pStrCusCode)
        {
            pStrCusCode = DefaultService.GetString(pStrCusCode);
            if (0.Equals(pStrCusCode.Length))
            {
                return null;
            }
            var qryTaxInvoice = _context.SalTaxinvoiceHds.Where(
                x => x.CustCode == pStrCusCode
                && "Active".Equals(x.DocStatus)
                && ("Invoice".Equals(x.DocType) || "CreditSale".Equals(x.DocType))
            ).AsNoTracking();
            var result = await qryTaxInvoice.ToArrayAsync();
            return result;
        }
        public async Task<string> UpdateBiling(ModelBilling param)
        {
            if(param == null)
            {
                return string.Empty;
            }
            param.Header.UpdatedDate = DateTime.Now;
            var entBilling = _context.Update(param.Header);
            entBilling.Property(x => x.DocDate).IsModified = false;
            entBilling.Property(x => x.RunNumber).IsModified = false;
            entBilling.Property(x => x.DocPattern).IsModified = false;
            entBilling.Property(x => x.Guid).IsModified = false;
            entBilling.Property(x => x.CreatedDate).IsModified = false;
            entBilling.Property(x => x.CreatedBy).IsModified = false;
            var qryDetail = _context.SalBillingDts.Where(
                x => x.DocNo == param.Header.DocNo
                && x.BrnCode == param.Header.BrnCode
                && x.CompCode == param.Header.CompCode
                && x.LocCode == param.Header.LocCode              
            ).AsNoTracking();
            _context.SalBillingDts.RemoveRange(qryDetail);
            await _context.SalBillingDts.AddRangeAsync(param.ArrDetail);
            await _unitOfWork.CompleteAsync();
            return param.Header.Guid.ToString();
        }
        public async Task<string> InsertBilling(ModelBilling param)
        {
            if(param == null 
            || param.Header == null 
            || param.ArrDetail == null 
            || !param.ArrDetail.Any())
            {
                return string.Empty;
            }
            var header = param.Header;
            header.CreatedDate = DateTime.Now;
            header.DocStatus = "Active";
            header.Guid = Guid.NewGuid();
            await adjustHeaderRunningNo(header);
            var arrDetail = param.ArrDetail;
            int intSeqNo = 1;
            string strCusCode = DefaultService.GetString(header.CustCode);
            foreach (var dt in arrDetail)
            {
                if (dt == null)
                {
                    continue;
                }
                dt.DocNo = header.DocNo;
                dt.CompCode = header.CompCode;
                dt.BrnCode = header.BrnCode;
                dt.LocCode = header.LocCode;
                dt.SeqNo = intSeqNo++;
                if (!0.Equals(strCusCode.Length))
                {
                    string strTaxNo = DefaultService.GetString(dt.TxNo);
                    if (!0.Equals(strTaxNo.Length))
                    {
                        var qryTaxInvoice = _context.SalTaxinvoiceHds.Where(
                            x => x.DocNo == dt.TxNo
                            && x.CustCode == strCusCode
                        );
                        var taxHd = await qryTaxInvoice.FirstOrDefaultAsync();
                        if (taxHd != null)
                        {                           
                            taxHd.DocStatus = "Reference";
                            taxHd.UpdatedDate = DateTime.Now;
                            taxHd.UpdatedBy = header.CreatedBy;
                        }
                    }                    
                }
            }
            var qryDetail = _context.SalBillingDts.Where(
                x => x.BrnCode == header.BrnCode
                && x.CompCode == header.CompCode
                && x.LocCode == header.LocCode
                && x.DocNo == header.DocNo
            );
            _context.SalBillingDts.RemoveRange (qryDetail);
            await _context.SalBillingHds.AddAsync(header);
            await _context.SalBillingDts.AddRangeAsync(arrDetail);
            await _unitOfWork.CompleteAsync();
            return header.Guid.ToString();
        }
        public async Task<string> UpdateStatus(SalBillingHd param)
        {
            if(param == null)
            {
                return string.Empty;
            }
            var entBilling = _context.SalBillingHds.Attach(param);
            param.UpdatedDate = DateTime.Now;
            entBilling.Property(x => x.DocStatus).IsModified = true;
            entBilling.Property(x => x.UpdatedBy).IsModified = true;
            if ("Cancel".Equals(param.DocStatus))
            {
                var qryBillingDt = _context.SalBillingDts.Where(
                    x => x.DocNo == param.DocNo
                    && x.BrnCode == param.BrnCode
                    && x.CompCode == param.CompCode
                    && x.LocCode == param.LocCode
                ).AsNoTracking();
                var qryTaxInv = _context.SalTaxinvoiceHds.Where(
                    x => qryBillingDt.Any(y=> y.TxNo == x.DocNo )
                    && x.CustCode == param.CustCode
                ).AsNoTracking();
                var arrTax = await qryTaxInv.ToArrayAsync();
                if(arrTax != null && arrTax.Any())
                {
                    foreach (var tax in arrTax)
                    {
                        _context.SalTaxinvoiceHds.Attach(tax);
                        tax.DocStatus = "Active";
                        tax.UpdatedDate = DateTime.Now;
                        tax.UpdatedBy = param.CreatedBy;
                    }
                }
            }
            await _unitOfWork.CompleteAsync();
            return param.Guid.ToString();
        }

        private async Task adjustHeaderRunningNo(SalBillingHd pBillingHeader)
        {
            string strRunningDocNo = string.Empty;           
            var docPattern = await _context.MasDocPatterns.FirstOrDefaultAsync(x => "Billing".Equals(x.DocType));
            List<MasDocPatternDt> listDocPatternDetail = null;
            bool isUseDefaultPattern = true;
            if (docPattern != null)
            {
                listDocPatternDetail = await _context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
                isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            }
            int intLastRunning = 0;
            var dateSystem = pBillingHeader?.DocDate ?? DateTime.Now;
            //int intDay = DateTime.Now.Day;
            //int intMonth = DateTime.Now.Month;
            //int intYear = DateTime.Now.Year;
            int intDay = dateSystem.Day;
            int intMonth = dateSystem.Month;
            int intYear = dateSystem.Year;
            var billing = _context.SalBillingHds.Where(
                x => x.BrnCode == pBillingHeader.BrnCode
                && x.CompCode == pBillingHeader.CompCode
                && x.LocCode == pBillingHeader.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue);
            if (isUseDefaultPattern)
            {
                billing = billing.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    billing = billing.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        billing = billing.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            billing = billing.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            var listBilling = await billing.ToListAsync();

            if (listBilling.Any())
            {
                intLastRunning = listBilling.Max(x => x.RunNumber.Value);
            }
            //intLastRunning = await billing.DefaultIfEmpty().MaxAsync(x => x.RunNumber.Value) + 1;
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
                    strRunningDocNo = string.Empty;
                    foreach (var item in listDocPatternDetail)
                    {
                        if (item == null) continue;
                        switch (item.DocCode)
                        {
                            case "-": strRunningDocNo += "-"; break;
                            case "MM": strRunningDocNo += intMonth.ToString("00"); break;
                            case "Comp": strRunningDocNo += pBillingHeader.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pBillingHeader.BrnCode; break;
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

            } while (await _context.SalBillingHds.AnyAsync(
                x => x.BrnCode == pBillingHeader.BrnCode
                && x.CompCode == pBillingHeader.CompCode
                && x.LocCode == pBillingHeader.LocCode
                && x.DocNo == strRunningDocNo
            ));
            //pBillingHeader.DocPattern = docPattern?.Pattern ?? string.Empty;
            pBillingHeader.RunNumber = intLastRunning;
            pBillingHeader.DocNo = strRunningDocNo;
        }
    }
}
