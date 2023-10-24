using Inventory.API.Domain.Repositories;
using log4net;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UnusableController : BaseController
    {
        #region - Declare Variable -

        private const string _strActive = "Active";
        private const string _strAppJson = "application/json";
        private const string _strProduct = "Product";
        private const string _strUnusable = "Unusable";
        private const string _strNew = "New";

        private static readonly ILog _log = LogManager.GetLogger(typeof(UnusableController));
        private IUnitOfWork _unitOfWork = null;

        #endregion

        #region - Model -

        public class ModelUnusable
        {
            public InvUnuseHd Header { get; set; }
            public InvUnuseDt[] ArrayDetail { get; set; }
        }

        public class ModelUnusableParam
        {
            public string BrnCode { get; set; }
            public string CompCode { get; set; }
            public string LocCode { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int ItemsPerPage { get; set; }

        }

        public class ModelUnusableResult
        {
            public InvUnuseHd[] ArrayHeader { get; set; }
            public MasEmployee[] ArrayEmployee { get; set; }
            public int TotalItems { get; set; }
        }
        #endregion

        #region - Start -

        public UnusableController(PTMaxstationContext pContex, IUnitOfWork pUnitOfWork):base(pContex)
        {
            _unitOfWork = pUnitOfWork;
        }

        #endregion

        #region - Controller -
        [HttpPost("GetArrayHeader")]
        public async Task<IActionResult> GetArrayHeader(ModelUnusableParam param)
        {
            return await doAction("GetArrayHeader", async () => await getArrayHeader(param));
        }

        [HttpGet("GetArrayProduct")]
        public async Task<IActionResult> GetArrayProduct()
        {
            return await doAction("GetArrayProduct", async () => await getArrayProduct());
        }
        [HttpGet("GetArrayReason")]
        public async Task<IActionResult> GetArrayReason()
        {
            return await doAction("GetArrayReason", async () => await getArrayReason());
        }

        [HttpGet("GetUnusable/{pStrGuid}")]
        public async Task<IActionResult> GetUnusable(string pStrGuid)
        {
            return await doAction("GetUnusable", async () => await getUnusable(pStrGuid));
        }

        [HttpPost("SaveUnusable")]
        public async Task<IActionResult> SaveUnusable(ModelUnusable param)
        {
            return await doAction("SaveUnusable", async () => await saveUnusable(param));
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(InvUnuseHd pHeader)
        {
            return await doAction("UpdateStatus", async () => await updateStatus(pHeader));
        }

        #endregion

        #region - Function -
        private async Task<IActionResult> doAction<T>(string pStrFunctionName, Func<Task<T>> pFunc)
        {
            try
            {
                T result;
                result = await pFunc();
                //_log.Info(pStrFunctionName + " Complete");
                return jsonResult(result);
            }
            catch (Exception ex)
            {
                _log.Error(pStrFunctionName, ex);
                return exeptionResult(ex);
            }
        }
        private ContentResult jsonResult(object pInput)
        {
            string strJson = string.Empty;

            strJson = JsonConvert.SerializeObject(pInput);
            ContentResult result = null;
            result = Content(strJson, _strAppJson);
            return result;
        }
        private string getErrorMessage(Exception pException)
        {
            if (pException == null)
            {
                return string.Empty;
            }
            string result = string.Empty;
            result = pException.StackTrace;
            while (pException.InnerException != null)
            {
                pException = pException.InnerException;
            }
            result = pException.Message + Environment.NewLine + result;
            return result;
        }
        private BadRequestObjectResult exeptionResult(Exception pException)
        {
            string strErrorMessage = string.Empty;
            strErrorMessage = getErrorMessage(pException);
            return BadRequest(strErrorMessage);
        }
        
        private async Task adjustHeaderRunningNo(InvUnuseHd pUnuse)
        {
            string strRunningDocNo = string.Empty;
            MasDocPatternDt[] arrDocPatternDetail = null;
            arrDocPatternDetail = await getArrayDocPattern(_strUnusable);
            bool isUseDefaultPattern = true;
            isUseDefaultPattern = arrDocPatternDetail == null || !arrDocPatternDetail.Any();
            int intLastRunning = 0;
            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pUnuse.DocDate != null && pUnuse.DocDate.HasValue)
            {
                intDay = pUnuse.DocDate.Value.Day;
                intMonth = pUnuse.DocDate.Value.Month;
                intYear = pUnuse.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<InvUnuseHd> qryUnusable = null;
            qryUnusable = context.InvUnuseHds.Where(
                x => x.BrnCode == pUnuse.BrnCode
                && x.CompCode == pUnuse.CompCode
                && x.LocCode == pUnuse.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                qryUnusable = qryUnusable.Where(
                    x => intYear.Equals(x.DocDate.Value.Year)
                    && intMonth.Equals(x.DocDate.Value.Month)
                );
            }
            else
            {
                arrDocPatternDetail = arrDocPatternDetail.OrderBy(x => x.SeqNo).ToArray();
                if (arrDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryUnusable = qryUnusable.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (arrDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryUnusable = qryUnusable.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (arrDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryUnusable = qryUnusable.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }

            if (await qryUnusable.AnyAsync())
            {
                int intMaxRunning = await qryUnusable.MaxAsync(x => x.RunNumber.Value);
                int intRowCount = await qryUnusable.CountAsync();
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
                    foreach (var item in arrDocPatternDetail)
                    {
                        if (item == null) continue;
                        switch (item.DocCode)
                        {
                            case "-": strRunningDocNo += "-"; break;
                            case "MM": strRunningDocNo += intMonth.ToString("00"); break;
                            case "Comp": strRunningDocNo += pUnuse.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pUnuse.BrnCode; break;
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

            } while (await context.InvUnuseHds.AnyAsync(
                x => x.BrnCode == pUnuse.BrnCode
                && x.CompCode == pUnuse.CompCode
                && x.LocCode == pUnuse.LocCode
                && x.DocNo == strRunningDocNo
            ));
            pUnuse.RunNumber = intLastRunning;
            pUnuse.DocNo = strRunningDocNo;
        }
        private async Task<MasDocPatternDt[]> getArrayDocPattern(string pStrDocType)
        {
            pStrDocType = (pStrDocType ?? string.Empty).Trim();
            if( 0.Equals(pStrDocType.Length))
            {
                return null;
            }
            IQueryable<MasDocPatternDt> qryDocPattern = null;
            qryDocPattern = from dp in context.MasDocPatterns.AsNoTracking()
                            join dt in context.MasDocPatternDts.AsNoTracking()
                            on dp.DocId equals dt.DocId
                            where pStrDocType.Equals(dp.DocType)
                            select new MasDocPatternDt()
                            {
                                DocValue = dt.DocValue,
                                DocCode = dt.DocCode,
                                SeqNo = dt.SeqNo
                            };
            MasDocPatternDt[] result = null;
            result = await qryDocPattern.ToArrayAsync();
            return result;
        }

        private async Task<ModelUnusableResult> getArrayHeader(ModelUnusableParam param)
        {
            if (param == null)
            {
                return null;
            }
            IQueryable<InvUnuseHd> qryHeader = null;
            qryHeader = context.InvUnuseHds.Where(
                x => x.CompCode == param.CompCode
                && x.BrnCode == param.BrnCode
                && x.LocCode == param.LocCode
            ).AsNoTracking();
            if (param.FromDate.HasValue)
            {
                qryHeader = qryHeader.Where(x => x.DocDate >= param.FromDate);
            }
            if (param.ToDate.HasValue)
            {
                qryHeader = qryHeader.Where(x => x.DocDate <= param.ToDate);
            }
            if (!string.IsNullOrWhiteSpace(param.Keyword))
            {
                qryHeader = qryHeader.Where(
                    x => x.DocNo.Contains(param.Keyword)
                    || x.CreatedBy.Contains(param.Keyword)
                );
            }
            int intTotalItem = 0;
            intTotalItem = await qryHeader.CountAsync();
            qryHeader = qryHeader.OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo);
            if (param.Page > 0 && param.ItemsPerPage > 0)
            {
                int intSkip = (param.Page - 1) * param.ItemsPerPage;
                qryHeader = qryHeader
                    .Skip(intSkip)
                    .Take(param.ItemsPerPage);
            }
            
            InvUnuseHd[] arrayHeader = null;
            arrayHeader = await qryHeader.ToArrayAsync();

            if (arrayHeader == null || !arrayHeader.Any())
            {
                return null;
            }
            string[] arrCreateBy = null;
            arrCreateBy = arrayHeader
                .Select(x => (x.CreatedBy ?? string.Empty).Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            string[] arrUpdateBy = null;
            arrUpdateBy = arrayHeader
                .Select(x => (x.UpdatedBy ?? string.Empty).Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            string[] arrActionBy = null;
            arrActionBy = arrCreateBy
                .Concat(arrUpdateBy)
                .Distinct()
                .ToArray();

            MasEmployee[] arrEmployee = null;

            if (arrActionBy != null && arrActionBy.Any())
            {
                IQueryable<MasEmployee> qryCustomer = null;
                qryCustomer = context.MasEmployees
                    .Where(x => arrActionBy.Contains(x.EmpCode))
                    .AsNoTracking();
                arrEmployee = await qryCustomer.ToArrayAsync();
            }
            ModelUnusableResult result = null;
            result = new ModelUnusableResult()
            {
                ArrayEmployee = arrEmployee,
                ArrayHeader = arrayHeader,
                TotalItems = intTotalItem,
            };
            return result;
        }

        private async Task<MasProduct[]> getArrayProduct()
        {
            IQueryable<MasProduct> qryProduct = null;
            qryProduct = context.MasProducts.Where(
                x => _strActive.Equals(x.PdStatus) 
                && _strProduct.Equals(x.PdType)
            ).AsNoTracking();
            MasProduct[] result = null;
            result = await qryProduct.ToArrayAsync();
            return result;
        }
        private async Task<MasReason[]> getArrayReason()
        {
            IQueryable<MasReason> qryReason = null;
            qryReason = context.MasReasons.Where(
                x => _strUnusable.Equals(x.ReasonGroup)
                && _strActive.Equals(x.ReasonStatus)
            ).AsNoTracking();
            MasReason[] result = null;
            result = await qryReason.ToArrayAsync();
            return result;
        }
        private async Task<ModelUnusable> getUnusable(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();

            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }
            Guid guid;
            if (!Guid.TryParse(pStrGuid, out guid)){
                return null;
            }
            IQueryable<InvUnuseHd> qryHeader = null;
            qryHeader = context.InvUnuseHds
                .Where(x => guid.Equals(x.Guid))
                .AsNoTracking();
            InvUnuseHd header = null;
            header = await qryHeader.FirstOrDefaultAsync();

            if(header == null)
            {
                return null;
            }
            IQueryable<InvUnuseDt> qryDetail = null;
            qryDetail = context.InvUnuseDts.Where(
                x => x.CompCode == header.CompCode
                && x.BrnCode == header.BrnCode
                && x.LocCode == header.LocCode
                && x.DocNo == header.DocNo         
            ).AsNoTracking();
            InvUnuseDt[] arrDetail = null;
            arrDetail = await qryDetail.ToArrayAsync();

            ModelUnusable result = null;
            result = new ModelUnusable()
            {
                Header = header ,
                ArrayDetail = arrDetail
            };
            return result;
        }

        private async Task<ModelUnusable> saveUnusable(ModelUnusable param)
        {
            if (param == null)
            {
                return null;
            }
            InvUnuseHd header = null;
            header = param.Header;
            if (header == null)
            {
                return null;
            }
            if (_strNew.Equals(header.DocStatus))
            {
                header.CreatedDate = DateTime.Now;
                header.DocStatus = _strActive;
                header.Guid = Guid.NewGuid();
                await adjustHeaderRunningNo(header);
                await context.AddAsync(header);
            }
            else
            {
                header.UpdatedDate = DateTime.Now;
                EntityEntry<InvUnuseHd> entHeader = null;
                entHeader = context.Update(header);
                string[] arrNoUpdateField = null;
                arrNoUpdateField = new[] {
                    "RunNumber", "Guid", "DocPattern",
                    "DocDate", "CreatedDate", "CreatedBy"
                };
                foreach (string strField in arrNoUpdateField)
                {
                    entHeader.Property(strField).IsModified = false;
                }
            }
            IQueryable<InvUnuseDt> qryDetail = null;
            qryDetail = context.InvUnuseDts.Where(
                x => x.CompCode == header.CompCode
                && x.BrnCode == header.BrnCode
                && x.LocCode == header.LocCode
                && x.DocNo == header.DocNo
            );
            context.RemoveRange(qryDetail);
            InvUnuseDt[] arrDetail = null;
            arrDetail = param.ArrayDetail;
            if (arrDetail != null && arrDetail.Any())
            {
                int intSeqNo = 0;
                foreach (var item in arrDetail)
                {
                    item.SeqNo = ++intSeqNo;
                    item.CompCode = header.CompCode;
                    item.BrnCode = header.BrnCode;
                    item.LocCode = header.LocCode;
                    item.DocNo = header.DocNo;
                }
                await context.AddRangeAsync(arrDetail);
            }
            await _unitOfWork.CompleteAsync();
            return param;
        }

        private async Task<InvUnuseHd> updateStatus(InvUnuseHd pHeader)
        {
            if (pHeader == null)
            {
                return null;
            }
            EntityEntry<InvUnuseHd> entHeader = null;
            entHeader = context.Attach(pHeader);
            pHeader.UpdatedDate = DateTime.Now;
            entHeader.Property(x => x.DocStatus).IsModified = true;
            await _unitOfWork.CompleteAsync();
            return pHeader;
        }

        #endregion
    }
}
