using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sal.API.Controllers;
using Sale.API.Domain.Models;
using Sale.API.Domain.Repositories;
using Sale.API.Repositories;
using Sale.API.Resources;
using Sale.API.Resources.Billing;
using Sale.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sale.API.Resources.Billing.BillingQueryResource;

namespace Sale.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : Controller //, BaseController
    {
        private PTMaxstationContext _context = null;
        IUnitOfWork _unitOfWork = null;
        private BillingService _svBilling = null;
        private static readonly ILog _log = LogManager.GetLogger(typeof(BillingController));
        public BillingController(PTMaxstationContext pContext , IUnitOfWork pUnitOfWork)
        {
            _context = pContext;
            _unitOfWork = pUnitOfWork;
            _svBilling = new BillingService(pContext, _unitOfWork);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_svBilling != null)
            {
                _svBilling = null;
            }
            if(_context != null)
            {
                _context.Dispose();
                _context = null;
            }
            if(_unitOfWork != null)
            {
                _unitOfWork = null;
            }
            GC.Collect();            
        }

        [HttpPost("AError")]
        public async Task<IActionResult> AError(SearchBillingQueryResource query)
        {
            try
            {
                //System.Threading.Thread.Sleep(9000);
                throw new Exception("AAAAAA");
            }
            catch (Exception ex)
            {
                if(await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                throw ex;
            }

        }

        [HttpGet("BError/{pStrGuid}")]
        public async Task<IActionResult> BError(string pStrGuid)
        {
            try
            {
                throw new Exception("BBBBB");
            }
            catch (Exception ex)
            {
                if (await LogErrorService.ConnectServer())
                {
                    await LogErrorService.WriteErrorLog(ex);
                    await LogErrorService.DisConnectServer();
                }
                throw ex;
            }

        }

        [HttpPost("SearchBilling")]
        public async Task<IActionResult> SearchBilling(SearchBillingQueryResource query)
        {
            return await DefaultService.DoActionAsync(
                "SearchBilling", 
                async () => await _svBilling.SearchBilling(query), 
                _log
            );
        }
        [HttpGet("GetBilling/{pStrGuid}")]
        public async Task<IActionResult> GetBilling(string pStrGuid)
        {
            return await DefaultService.DoActionAsync(
                "GetBilling",
                async () => await _svBilling.GetBilling(pStrGuid),
                _log
            );
        }

        [HttpGet("GetTaxInvoice/{pStrCustCode}")]
        public async Task<IActionResult> GetTaxInvoice(string pStrCustCode)
        {
            return await DefaultService.DoActionAsync(
                "GetTaxInvoice", 
                async () => await _svBilling.GetTaxInvoice(pStrCustCode), 
                _log
            );
        }
        [HttpPost("UpdateBilling")]
        public async Task<IActionResult> UpdateBilling(ModelBilling param)
        {
            return await DefaultService.DoActionAsync(
                "UpdateBilling",
                async () => await _svBilling.UpdateBiling(param),
                _log
            );
        }
        [HttpPost("InsertBilling")]
        public async Task<IActionResult> InsertBilling(ModelBilling param)
        {
            return await DefaultService.DoActionAsync(
                "InsertBilling",
                async () => await _svBilling.InsertBilling(param),
                _log
            );
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(SalBillingHd param)
        {
            return await DefaultService.DoActionAsync(
                "UpdateStatus",
                async () => await _svBilling.UpdateStatus(param),
                _log
            );
        }
        #region - SearchBilling -



        //[HttpPost("SearchBilling")]
        //public async Task<IActionResult> SearchBilling([FromBody] BillingQueryResource.SearchBillingQueryResource query)
        //{
        //    try
        //    {

        //        var result = new QueryResultResource<SalBillingHd>();
        //        var salBillingHeader = _context.SalBillingHds.Where(x =>
        //            x.CompCode == query.CompCode
        //            && x.BrnCode == query.BrnCode
        //            && x.LocCode == query.LocCode
        //        );
        //        if (query.FromDate.HasValue && query.ToDate.HasValue)
        //        {
        //            salBillingHeader = salBillingHeader.Where(
        //                x => x.DocDate >= query.FromDate.Value
        //                && x.DocDate <= query.ToDate.Value);
        //        }
        //        string strKeyWord = string.Empty;
        //        if (!string.IsNullOrWhiteSpace(query.Keyword))
        //        {
        //            strKeyWord = query.Keyword.Trim();
        //        }
        //        if (!0.Equals(strKeyWord.Length))
        //        {
        //            salBillingHeader = salBillingHeader.Where(x =>
        //                (!string.IsNullOrEmpty(x.CustCode) && x.CustCode.Contains(strKeyWord))
        //                || (!string.IsNullOrEmpty(x.CustName) && x.CustName.Contains(strKeyWord))
        //                || (!string.IsNullOrEmpty(x.DocNo) && x.DocNo.Contains(strKeyWord))
        //            );
        //        }
        //        salBillingHeader = salBillingHeader.OrderByDescending(y => y.CompCode).ThenByDescending(y => y.BrnCode).ThenByDescending(y => y.LocCode).ThenByDescending(y => y.DocNo);
        //        var listBillingHeader = await salBillingHeader.ToListAsync();

        //        result.Items = listBillingHeader.Skip((query.Page - 1) * query.ItemsPerPage).Take(query.ItemsPerPage).ToList();
        //        result.TotalItems = listBillingHeader.Count;

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        while (ex.InnerException != null) ex = ex.InnerException;
        //        return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //}
        //[HttpPost("SearchBilling")]

        //private async Task<QueryResultResource<SalBillingHd>> SearchBillingOld([FromBody] BillingQueryResource.SearchBillingQueryResource query)
        //{
        //    var result = new QueryResultResource<SalBillingHd>();
        //    query.BrnCode = getString(query.BrnCode);
        //    query.CompCode = getString(query.CompCode);
        //    Func<List<SalBillingHd>> acToList = () => Enumerable.Range(query.Page, query.ItemsPerPage + query.Page)
        //    .Select(x => new SalBillingHd() {
        //        DocNo = "DocNo" + x.ToString(),
        //        DocDate = DateTime.Now.AddDays(x),
        //        TotalAmt = x * 10,
        //        DocStatus = "Active"
        //    }).ToList();
        //    result.Items = await Task.Run(acToList);
        //    return result;
        //}

        #endregion

        #region - GetBilling -
        //public class GetBillingResult : QueryResultResource<SalBillingDt>
        //{
        //    public SalBillingHd Header { get; set; }
        //}

        //public class GetBillingResource : QueryResource
        //{
        //    public string BrnCode { get; set; }
        //    public string CompCode { get; set; }
        //    public string LocCode { get; set; }
        //    public string DocNo { get; set; }
        //}

        //[HttpPost("GetBilling")]
        //public async Task<IActionResult> GetBilling([FromBody] GetBillingResource query)
        //{
        //    try
        //    {
        //        var billingHeader = await _context.SalBillingHds.FirstOrDefaultAsync(
        //            x => x.CompCode == query.CompCode
        //            && x.BrnCode == query.BrnCode
        //            && x.LocCode == query.LocCode
        //            && x.DocNo == query.DocNo
        //        );

        //        var custPrefix = await _context.MasCustomers.Where(x => x.CustCode == billingHeader.CustCode).Select(x => x.CustPrefix).FirstOrDefaultAsync();
        //        billingHeader.CustPrefix = custPrefix;

        //        var listBillingDetail = await _context.SalBillingDts.Where(
        //            x => x.CompCode == query.CompCode
        //            && x.BrnCode == query.BrnCode
        //            && x.LocCode == query.LocCode
        //            && x.DocNo == query.DocNo
        //        ).ToArrayAsync();
        //        return Ok(new HeaderDetailResource<SalBillingHd, SalBillingDt>()
        //        {
        //            Header = billingHeader,
        //            ArrDetail = listBillingDetail
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        string strErrorMessage = getErrorMessage(ex);
        //        return BadRequest(strErrorMessage);
        //    }
        //}


        //public class HeaderDetailResource<h, d>
        //{
        //    public h Header { get; set; }
        //    public d[] ArrDetail { get; set; }
        //}

        //public class GetBillingModalItemParam
        //{
        //    public string CustomerCode { get; set; }
        //}


        //[HttpPost("GetBillingModalItem")]
        //public async Task<IActionResult> GetBillingModalItem([FromBody] GetBillingModalItemParam param)
        //{
        //    try
        //    {

        //        var listTaxInvoice = await _context.SalTaxinvoiceHds.Where(
        //            x => x.CustCode == param.CustomerCode
        //            && "Active".Equals(x.DocStatus)
        //            && ("Invoice".Equals(x.DocType) || "CreditSale".Equals(x.DocType))
        //        ).ToListAsync();
        //        return Ok(listTaxInvoice);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strErrorMessage = getErrorMessage(ex);
        //        return BadRequest(strErrorMessage);
        //    }
        //}


        //[HttpPost("InsertBilling")]
        //public async Task<IActionResult> InsertBilling([FromBody] HeaderDetailResource<SalBillingHd, SalBillingDt> param)
        //{
        //    try
        //    {
        //        await adjustHeaderRunningNo(param.Header);
        //        param.Header.DocStatus = "Active";
        //        param.Header.CreatedDate = DateTime.Now;
        //        param.Header.Guid = Guid.NewGuid();
        //        await _context.SalBillingHds.AddAsync(param.Header);
        //        for (int i = 0; i < param.ArrDetail.Length; i++)
        //        {
        //            var detailItem = param.ArrDetail[i];
        //            detailItem.DocNo = param.Header.DocNo;
        //            detailItem.SeqNo = i + 1;
        //        }
        //        var listDetail = _context.SalBillingDts.Where(
        //            x => x.BrnCode == param.Header.BrnCode
        //            && x.CompCode == param.Header.CompCode
        //            && x.LocCode == param.Header.LocCode
        //            && x.DocNo == param.Header.DocNo
        //        );
        //        if (await listDetail.AnyAsync())
        //        {
        //            _context.SalBillingDts.RemoveRange(listDetail);
        //        }

        //        if (param.ArrDetail != null && param.ArrDetail.Any())
        //        {
        //            string strCusCode = (param?.Header?.CustCode ?? string.Empty).Trim();

        //            foreach (SalBillingDt dt in param.ArrDetail)
        //            {
        //                if (dt == null)
        //                {
        //                    continue;
        //                }
        //                string strTaxNo = (dt?.TxNo ?? string.Empty).ToString();
        //                if (0.Equals(strTaxNo))
        //                {
        //                    continue;
        //                }
        //                //dt.TxNo  , cuscode ,  _context.SalTaxinvoiceHds.Where(
        //                SalTaxinvoiceHd taxHd = await _context.SalTaxinvoiceHds.FirstOrDefaultAsync(
        //                    x => x.DocNo == dt.TxNo
        //                    && x.CustCode == strCusCode);
        //                if (taxHd != null)
        //                {
        //                    _context.SalTaxinvoiceHds.Attach(taxHd);
        //                    taxHd.DocStatus = "Reference";
        //                    taxHd.UpdatedDate = DateTime.Now;
        //                    taxHd.UpdatedBy = param.Header.CreatedBy;
        //                }
        //            }
        //        }
        //        await _context.SalBillingDts.AddRangeAsync(param.ArrDetail);
        //        await _context.SaveChangesAsync();
        //        return Ok(param);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strErrorMessage = getErrorMessage(ex);
        //        return BadRequest(strErrorMessage);
        //    }
        //}



        //[HttpPost("UpdateBilling")]
        //public async Task<IActionResult> UpdateBilling([FromBody] HeaderDetailResource<SalBillingHd, SalBillingDt> param)
        //{
        //    try
        //    {
        //        bool isExists = await _context.SalBillingHds.AnyAsync(
        //            x => x.BrnCode == param.Header.BrnCode
        //            && x.CompCode == param.Header.CompCode
        //            && x.LocCode == param.Header.LocCode
        //            && x.DocNo == param.Header.DocNo
        //        );
        //        if (isExists)
        //        {
        //            //_context.SalBillingHds.Attach(param.Header);
        //            param.Header.UpdatedDate = DateTime.Now;
        //            _context.Entry(param.Header).State = EntityState.Modified;
        //        }
        //        else
        //        {
        //            await _context.SalBillingHds.AddAsync(param.Header);
        //        }
        //        var listDetail = await _context.SalBillingDts.Where(
        //            x => x.BrnCode == param.Header.BrnCode
        //            && x.CompCode == param.Header.CompCode
        //            && x.LocCode == param.Header.LocCode
        //            && x.DocNo == param.Header.DocNo
        //        ).ToListAsync();
        //        if (listDetail.Any())
        //        {
        //            _context.SalBillingDts.RemoveRange(listDetail);
        //        }
        //        for (int i = 0; i < listDetail.Count; i++)
        //        {
        //            listDetail[i].SeqNo = i + 1;
        //        }
        //        await _context.SalBillingDts.AddRangeAsync(param.ArrDetail);
        //        await _context.SaveChangesAsync();
        //        return Ok(param);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strErrorMessage = getErrorMessage(ex);
        //        return BadRequest(strErrorMessage);
        //    }
        //}
        //[HttpPost("GetBilling")]
        //private async Task<GetBillingResult> GetBillingOld([FromBody] GetBillingResource query)
        //{
        //    var header = new SalBillingHd()
        //    {
        //        DocNo = query.DocNo,
        //        CustCode = "CusCode" + query.DocNo,
        //        CustName = "CusName",
        //        CustAddr1 = "CustAddr1",
        //        CustAddr2 = "CustAddr2",
        //        DocDate = new DateTime(),
        //        CreditLimit = 10000,
        //        CreditTerm = 30,

        //    };

        //    Func<List<SalBillingDt>> funcX = () => Enumerable.Range(0, 10).Select(i => new SalBillingDt()
        //    {
        //        TxDate = new DateTime().AddDays(i),
        //        TxNo = "TX" + i.ToString("00000"),
        //        TxType = "ใบแจ้งหนี้",
        //        TxBrnCode = "001 : นครนายก เขต 1" + i.ToString() + query.BrnCode,
        //        TxAmtCur = (i + 1) * 5
        //    }).ToList();
        //    var result = new GetBillingResult()
        //    {
        //        Header = header,
        //        Items = await Task.Run(funcX)
        //    };
        //    return result;
        //}

        #endregion

        #region - GetRunning No -

        //public class GetRunningResource : QueryResource
        //{
        //    public string BrnCode { get; set; }
        //    public string CompCode { get; set; }
        //    public string LocCode { get; set; }
        //}
        //[HttpPost("GetRunning")]
        //public async Task<IActionResult> GetRunning([FromBody] GetRunningResource query)
        //{
        //    try
        //    {
        //        string strRunning = await getRunningNo(query.CompCode, query.LocCode, query.BrnCode);
        //        return Ok(strRunning);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strMessage = ex.Message + Environment.NewLine + ex.StackTrace;
        //        return BadRequest(strMessage);
        //    }
        //}
        //[HttpPost("GetRunningPattern")]
        //public async Task<IActionResult> GetRunningPattern([FromBody] GetRunningResource query)
        //{
        //    try
        //    {
        //        string strRunning = await getRunningPattern(query.CompCode, query.LocCode, query.BrnCode);
        //        return Ok(strRunning);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strMessage = ex.Message + Environment.NewLine + ex.StackTrace;
        //        return BadRequest(strMessage);
        //    }
        //}
        #endregion

        #region - Repository Function -

        //private async Task adjustHeaderRunningNo(SalBillingHd pBillingHeader)
        //{
        //    string strRunningDocNo = string.Empty;
        //    var docPattern = await _context.MasDocPatterns.FirstOrDefaultAsync(x => "Billing".Equals(x.DocType));
        //    List<MasDocPatternDt> listDocPatternDetail = null;
        //    bool isUseDefaultPattern = true;
        //    if (docPattern != null)
        //    {
        //        listDocPatternDetail = await _context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
        //        isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
        //    }
        //    int intLastRunning = 0;
        //    int intDay = DateTime.Now.Day;
        //    int intMonth = DateTime.Now.Month;
        //    int intYear = DateTime.Now.Year;
        //    var billing = _context.SalBillingHds.Where(
        //        x => x.BrnCode == pBillingHeader.BrnCode
        //        && x.CompCode == pBillingHeader.CompCode
        //        && x.LocCode == pBillingHeader.LocCode
        //        && x.DocDate.HasValue
        //        && x.RunNumber.HasValue);
        //    if (isUseDefaultPattern)
        //    {
        //        billing = billing.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
        //    }
        //    else
        //    {
        //        if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
        //        {
        //            billing = billing.Where(x => intYear.Equals(x.DocDate.Value.Year));
        //            if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
        //            {
        //                billing = billing.Where(x => intMonth.Equals(x.DocDate.Value.Month));
        //                if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
        //                {
        //                    billing = billing.Where(x => intDay.Equals(x.DocDate.Value.Day));
        //                }
        //            }
        //        }
        //    }
        //    var listBilling = await billing.ToListAsync();

        //    if (listBilling.Any())
        //    {
        //        intLastRunning = listBilling.Max(x => x.RunNumber.Value);
        //    }
        //    //intLastRunning = await billing.DefaultIfEmpty().MaxAsync(x => x.RunNumber.Value) + 1;
        //    do
        //    {
        //        strRunningDocNo = string.Empty;
        //        intLastRunning++;
        //        if (isUseDefaultPattern)
        //        {
        //            strRunningDocNo = string.Format("{0}{1}{2:D5}", intYear, intMonth, intLastRunning);
        //        }
        //        else
        //        {
        //            strRunningDocNo = string.Empty;
        //            foreach (var item in listDocPatternDetail)
        //            {
        //                if (item == null) continue;
        //                switch (item.DocCode)
        //                {
        //                    case "-": strRunningDocNo += "-"; break;
        //                    case "MM": strRunningDocNo += intMonth.ToString("00"); break;
        //                    case "Comp": strRunningDocNo += pBillingHeader.CompCode; break;
        //                    case "[Pre]": strRunningDocNo += item.DocValue; break;
        //                    case "dd": strRunningDocNo += intDay.ToString("00"); break;
        //                    case "Brn": strRunningDocNo += pBillingHeader.BrnCode; break;
        //                    case "yyyy": strRunningDocNo += intYear.ToString("0000"); break;
        //                    case "yy": strRunningDocNo += intYear.ToString().Substring(2, 2); break;
        //                    case "[#]":
        //                        int intDocValue = 0;
        //                        int.TryParse(item.DocValue, out intDocValue);
        //                        strRunningDocNo += intLastRunning.ToString().PadLeft(intDocValue, '0');
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //        }

        //    } while (await _context.SalBillingHds.AnyAsync(
        //        x => x.BrnCode == pBillingHeader.BrnCode
        //        && x.CompCode == pBillingHeader.CompCode
        //        && x.LocCode == pBillingHeader.LocCode
        //        && x.DocNo == strRunningDocNo
        //    ));
        //    pBillingHeader.RunNumber = intLastRunning;
        //    pBillingHeader.DocNo = strRunningDocNo;
        //}

        //private string getString(string pStrInput)
        //{
        //    if (String.IsNullOrEmpty(pStrInput))
        //    {
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        return pStrInput.Trim();
        //    }
        //}

        //private async Task<string> getRunningPattern(string pStrCompanyCode, string pStrLocationCode, string pStrBranchCode)
        //{
        //    string result = string.Empty;
        //    var docPattern = await _context.MasDocPatterns.FirstOrDefaultAsync(x => "Billing".Equals(x.DocType));
        //    List<MasDocPatternDt> listDocPatternDetail = null;
        //    bool isUseDefaultPattern = true;
        //    if (docPattern != null)
        //    {
        //        listDocPatternDetail = await _context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
        //        isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
        //    }
        //    int intDay = DateTime.Now.Day;
        //    int intMonth = DateTime.Now.Month;
        //    int intYear = DateTime.Now.Year;
        //    if (isUseDefaultPattern)
        //    {
        //        result = string.Format("{0}{1}#####", intYear, intMonth);
        //    }
        //    else
        //    {
        //        foreach (var item in listDocPatternDetail)
        //        {
        //            if (item == null) continue;
        //            switch (item.DocCode)
        //            {
        //                case "-": result += "-"; break;
        //                case "MM": result += intMonth.ToString("00"); break;
        //                case "Comp": result += pStrCompanyCode; break;
        //                case "[Pre]": result += item.DocValue; break;
        //                case "dd": result += intDay.ToString("00"); break;
        //                case "Brn": result += pStrBranchCode; break;
        //                case "yyyy": result += intYear.ToString("0000"); break;
        //                case "yy": result += intYear.ToString().Substring(2, 2); break;
        //                case "[#]":
        //                    int intDocValue = 0;
        //                    int.TryParse(item.DocValue, out intDocValue);
        //                    result += new string('#', intDocValue);
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }

        //    return result;
        //}

        //private async Task<string> getRunningNo(string pStrCompanyCode, string pStrLocationCode, string pStrBranchCode)
        //{
        //    string result = string.Empty;
        //    var docPattern = await _context.MasDocPatterns.FirstOrDefaultAsync(x => "Billing".Equals(x.DocType));
        //    List<MasDocPatternDt> listDocPatternDetail = null;
        //    bool isUseDefaultPattern = true;
        //    if (docPattern != null)
        //    {
        //        listDocPatternDetail = await _context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
        //        isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
        //    }
        //    int intLastRunning = 1;
        //    int intDay = DateTime.Now.Day;
        //    int intMonth = DateTime.Now.Month;
        //    int intYear = DateTime.Now.Year;
        //    var billing = _context.SalBillingHds.Where(x => x.BrnCode == pStrBranchCode && x.CompCode == pStrCompanyCode && x.LocCode == pStrLocationCode && x.DocDate.HasValue);
        //    if (isUseDefaultPattern)
        //    {
        //        billing = billing.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
        //    }
        //    else
        //    {
        //        if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
        //        {
        //            billing = billing.Where(x => intYear.Equals(x.DocDate.Value.Year));
        //            if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
        //            {
        //                billing = billing.Where(x => intMonth.Equals(x.DocDate.Value.Month));
        //                if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
        //                {
        //                    billing = billing.Where(x => intDay.Equals(x.DocDate.Value.Day));
        //                }
        //            }
        //        }
        //    }
        //    var listBilling = await billing.ToListAsync();

        //    if (listBilling.Any())
        //    {
        //        intLastRunning = listBilling.Max(x => x.RunNumber.Value) + 1;
        //    }
        //    //intLastRunning = await billing.DefaultIfEmpty().MaxAsync(x => x.RunNumber.Value) + 1;
        //    if (isUseDefaultPattern)
        //    {
        //        result = string.Format("{0}{1}{2:D5}", intYear, intMonth, intLastRunning);
        //    }
        //    else
        //    {
        //        foreach (var item in listDocPatternDetail)
        //        {
        //            if (item == null) continue;
        //            switch (item.DocCode)
        //            {
        //                case "-": result += "-"; break;
        //                case "MM": result += intMonth.ToString("00"); break;
        //                case "Comp": result += pStrCompanyCode; break;
        //                case "[Pre]": result += item.DocValue; break;
        //                case "dd": result += intDay.ToString("00"); break;
        //                case "Brn": result += pStrBranchCode; break;
        //                case "yyyy": result += intYear.ToString("0000"); break;
        //                case "yy": result += intYear.ToString().Substring(2, 2); break;
        //                case "[#]":
        //                    int intDocValue = 0;
        //                    int.TryParse(item.DocValue, out intDocValue);
        //                    result += intLastRunning.ToString().PadLeft(intDocValue, '0');
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }

        //    return result;
        //}

        //private string getErrorMessage(Exception pException)
        //{
        //    if (pException == null)
        //    {
        //        return string.Empty;
        //    }
        //    string result = pException.StackTrace;
        //    while (pException.InnerException != null)
        //    {
        //        pException = pException.InnerException;
        //    }
        //    result = pException.Message + Environment.NewLine + result;
        //    return result;
        //}

        #endregion
    }
}

