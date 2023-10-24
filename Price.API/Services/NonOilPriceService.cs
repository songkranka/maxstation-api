using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Price.API.Domain.Models;
using Price.API.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Price.API.Services
{
    public class NonOilPriceService : IDisposable
    {
        #region - Variable -
        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWork = null;

        #endregion

        public NonOilPriceService(PTMaxstationContext pContext, IUnitOfWork pUnitOfWork)
        {
            _context = pContext;
            _unitOfWork = pUnitOfWork;
        }

        public async Task<MasProduct[]> GetNonOilProduct()
        {
            var qryProduct = _context.MasProducts.Where(
                x => Convert.ToInt32( x.GroupId) > 7000
                && Convert.ToInt32(x.GroupId) < 9000
                && x.PdStatus == "Active"
            ).AsNoTracking();

            var result = await qryProduct.ToArrayAsync();
            return result;
        }
        public async Task<PriNonoilDt> GetNonOilPriceDetail(PriNonoilDt param)
        {
            if(param == null)
            {
                return null;
            }
            string strCon = DefaultService.GetString( _context.Database.GetDbConnection()?.ConnectionString);
            if (string.IsNullOrWhiteSpace(strCon))
            {
                return null;
            }
            string strProductId = DefaultService.EncodeSqlString(param.PdId);
            string strBrnCode = DefaultService.EncodeSqlString(param.BrnCode);
            string strCompCode = DefaultService.EncodeSqlString(param.CompCode);
            string strSql = $@"select top 1 non.*
from PRI_NONOIL_DT(nolock) non
cross apply
(
	select CREATED_DATE from PRI_NONOIL_HD(nolock)
	where non.DOC_NO = DOC_NO
		and non.COMP_CODE = COMP_CODE
		and non.BRN_CODE = BRN_CODE
        and APPROVE_STATUS = 'Y'
) header
where PD_ID = '{strProductId}'
	and BRN_CODE = '{strBrnCode}'
	and COMP_CODE = '{strCompCode}'
order by header.CREATED_DATE desc";
            PriNonoilDt result = null;
            var arrDetail = await DefaultService.GetEntityFromSql<PriNonoilDt[]>(strCon, strSql);
            if(arrDetail != null && arrDetail.Any())
            {
                result = arrDetail[0];
            }
            return result;
        }
        public async Task<ModelNonOilPrice> GetNonOilPrice(string pStrGuid)
        {
            if (string.IsNullOrWhiteSpace(pStrGuid))
            {
                return null;
            }
            Guid guid;
            if(!Guid.TryParse(pStrGuid , out guid))
            {
                return null;
            }
            var headerQry = _context.PriNonoilHds.Where(x => x.Guid == guid);
            var header = await headerQry.FirstOrDefaultAsync();
            if(header == null)
            {
                return null;
            }
            var qryDetail = _context.PriNonoilDts.Where(
                x => x.DocNo == header.DocNo
                && x.CompCode == header.CompCode
                //&& x.BrnCode == header.BrnCode
            );
            var arrDetail = await qryDetail.ToArrayAsync();
            var result = new ModelNonOilPrice()
            {
                ArrDetail = arrDetail,
                Header = header,
            };
            return result;
        }
        public async Task<ModelNonOilPrice> SaveNonOil(ModelNonOilPrice param)
        {
            if(param == null)
            {
                return null;
            }
            var header = param.Header;
            var arrDetail = param.ArrDetail;
            if(header == null || arrDetail == null || !arrDetail.Any())
            {
                return null;
            }
            if(header.DocStatus == "New")
            {
                await adjustHeaderRunningNo(header);
                header.DocStatus = "Active";
                header.Guid = Guid.NewGuid();
                header.CreatedDate = DateTime.Now;
                await creatApprove(header);
            }
            else
            {
                header.UpdatedDate = DateTime.Now;
                var qryHeader = _context.PriNonoilHds.Where(x => x.Guid == header.Guid);
                _context.PriNonoilHds.RemoveRange(qryHeader);
                var qryDetail = _context.PriNonoilDts.Where(
                    x => qryHeader.Any(
                        y => y.DocNo == x.DocNo 
                        && y.CompCode == x.CompCode
                    )
                );
                _context.PriNonoilDts.RemoveRange(qryDetail);
            }
            var listHeader = new List<PriNonoilHd>();
            foreach (var dt in arrDetail)
            {
                var hd = DefaultService.CloneObject(header);
                hd.BrnCode = dt.BrnCode;
                listHeader.Add(hd);
                dt.DocNo = hd.DocNo;
                dt.CompCode = hd.CompCode;
            }
            await _context.AddRangeAsync(listHeader);
            await _context.AddRangeAsync(arrDetail);
            await _unitOfWork.CompleteAsync();
            return param;
        }
        public async Task<PriNonoilHd> CancelNonOil(PriNonoilHd param)
        {
            if(param == null)
            {
                return null;
            }
            var qryHeader = _context.PriNonoilHds.Where(x => x.Guid == param.Guid);
            var arrHeader = await qryHeader.ToArrayAsync();
            if(arrHeader == null || !arrHeader.Any())
            {
                return null;
            }
            foreach (var item in arrHeader)
            {
                item.DocStatus = "Cancel";
                item.UpdatedDate = DateTime.Now;
                item.ApproveStatus = "N";
            }
            await cancelApprove(arrHeader[0]);
            await _unitOfWork.CompleteAsync();
            return param;
        }
        public async Task<ModelStandardPriceResult> GetArrayHeader(ModelNonOilPriceParam param)
        {
            if (param == null)
            {
                return null;
            }
            string strComCode = DefaultService.EncodeSqlString(param.CompCode);
            string strFilterDocDate = string.Empty;
            if (param.FromDate.HasValue && param.ToDate.HasValue)
            {
                string strFromDate = param.FromDate.Value.ToString("yyyy-MM-dd");
                string strToDate = param.ToDate.Value.ToString("yyyy-MM-dd");
                strFilterDocDate = $"AND DOC_DATE BETWEEN '{strFromDate}' AND '{strToDate}'";
            }
            string strPaging = string.Empty;
            if (param.ItemsPerPage > 0 && param.Page > 0)
            {
                int intOffset = (param.Page - 1) * param.ItemsPerPage;
                strPaging = $" OFFSET {intOffset} ROW";
                strPaging += $" FETCH NEXT {param.ItemsPerPage} ROWS ONLY ";
            }
            string strOrderBy = " ORDER BY CREATED_DATE DESC";
            //hd.*
            string strSql = $@"
SELECT {{SELECT}} FROM
(
	SELECT DOC_NO , COMP_CODE 
	FROM PRI_NONOIL_HD(NOLOCK)
	WHERE COMP_CODE = '{strComCode}'
	{strFilterDocDate}
	GROUP BY DOC_NO , COMP_CODE
) dn
CROSS APPLY(
	SELECT TOP 1 * 
	FROM PRI_NONOIL_HD(NOLOCK)	
	WHERE DOC_NO = dn.DOC_NO AND COMP_CODE = dn.COMP_CODE
) hd";
            string strSqlCount = strSql.Replace("{SELECT}", "COUNT(*)");
            string strSqlSelect = strSql.Replace("{SELECT}", "hd.*") + strOrderBy + strPaging;
            string strCon = DefaultService.GetString(_context.Database.GetConnectionString());
            int intCount = await DefaultService.ExecuteScalar<int>(strCon, strSqlCount);
            OilStandardPriceHd[] arrHeader
                = await DefaultService.GetEntityFromSql<OilStandardPriceHd[]>(strCon, strSqlSelect);
            if (arrHeader == null)
            {
                return null;
            }
            string[] arrCreateBy = null;
            arrCreateBy = arrHeader
                .Select(x => (x.CreatedBy ?? string.Empty).Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            string[] arrUpdateBy = null;
            arrUpdateBy = arrHeader
                .Select(x => (x.UpdatedBy ?? string.Empty).Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            string[] arrActionBy = null;
            arrActionBy = arrCreateBy.Concat(arrUpdateBy).Distinct().ToArray();
            MasEmployee[] arrEmployee = null;
            if (arrActionBy != null && arrActionBy.Any())
            {
                IQueryable<MasEmployee> qryCustomer = null;
                qryCustomer = _context.MasEmployees
                    .Where(x => arrActionBy.Contains(x.EmpCode))
                    .AsNoTracking();
                arrEmployee = await qryCustomer.ToArrayAsync();
            }
            ModelStandardPriceResult result = new ModelStandardPriceResult()
            {
                ArrayHeader = arrHeader,
                TotalItems = intCount,
                ArrayEmployee = arrEmployee
            };
            return result;
        }
        public async Task<PriNonoilHd> GetUnApproveDocument(string pStrCompCode)
        {
            pStrCompCode = DefaultService.GetString(pStrCompCode);
            if (0.Equals(pStrCompCode.Length))
            {
                return null;
            }
            Expression<Func<PriNonoilHd, bool>> expHeader = null;
            //expHeader = x => pStrCompCode.Equals(x.CompCode) && x.ApproveStatus != "Y";
            expHeader = x => pStrCompCode.Equals(x.CompCode) && (x.ApproveStatus ?? string.Empty) == string.Empty;
            IQueryable<PriNonoilHd> qryHeader = null;
            qryHeader = _context.PriNonoilHds.Where(expHeader).AsNoTracking();
            PriNonoilHd result = null;
            result = await qryHeader.FirstOrDefaultAsync();
            return result;

        }
        #region - Private Function -
        private async Task adjustHeaderRunningNo(PriNonoilHd pNonOilPrice)
        {
            string strRunningDocNo = string.Empty;
            var arrDocPatternDetail =  await DefaultService.GetArrayDocPattern("NonOilPrice", _context);
            bool isUseDefaultPattern = arrDocPatternDetail == null || !arrDocPatternDetail.Any();
            int intLastRunning = 0;
            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pNonOilPrice.DocDate != null && pNonOilPrice.DocDate.HasValue)
            {
                intDay = pNonOilPrice.DocDate.Value.Day;
                intMonth = pNonOilPrice.DocDate.Value.Month;
                intYear = pNonOilPrice.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
           
            var qryNonOilPrice = _context.PriNonoilHds.Where(
                x => x.CompCode == pNonOilPrice.CompCode
                && x.DocDate.HasValue);
            if (isUseDefaultPattern)
            {
                qryNonOilPrice = qryNonOilPrice.Where(
                    x => intYear.Equals(x.DocDate.Value.Year)
                    && intMonth.Equals(x.DocDate.Value.Month)
                );
            }
            else
            {
                arrDocPatternDetail = arrDocPatternDetail.OrderBy(x => x.SeqNo).ToArray();
                if (arrDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryNonOilPrice = qryNonOilPrice.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (arrDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryNonOilPrice = qryNonOilPrice.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (arrDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryNonOilPrice = qryNonOilPrice.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            if (await qryNonOilPrice.AnyAsync())
            {
                int intRowCount = await qryNonOilPrice.CountAsync();
                intLastRunning = intRowCount;
            }
            bool isDuplicate = false;
            Func<string, Task<bool>> funcIsDuplicate = async y =>
            {
                y = DefaultService.GetString(y);
                if (0.Equals(y.Length))
                {
                    return false;
                }
                var qryDuplicate = _context.PriNonoilHds.Where(
                    x => x.CompCode == pNonOilPrice.CompCode
                    && x.DocNo == y
                ).AsNoTracking();
                bool result = false;
                result = await qryDuplicate.AnyAsync();
                return result;
            };
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
                            case "Comp": strRunningDocNo += pNonOilPrice.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pNonOilPrice.BrnCode; break;
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
                isDuplicate = await funcIsDuplicate(strRunningDocNo);
            } while (isDuplicate);
            pNonOilPrice.DocNo = strRunningDocNo;           
        }
        private async Task creatApprove(PriNonoilHd param)
        {
            if (param == null)
            {
                return;
            }
            SysApproveConfig config = await _context.SysApproveConfigs.FirstOrDefaultAsync(x => "NonOilPrice" == x.DocType);
            if (config == null || !config.StepCount.HasValue)
            {
                return;
            }
            var appResult = await DefaultService.GetApproverStep(config.StepCount.Value, param.CreatedBy, _context);
            if (!appResult?.workflowApprover?.Any() ?? true)
            {
                return;
            }
            var arrEmpId = appResult.workflowApprover.Select(x => x.empId).ToArray();
            var arrEmp = await _context.MasEmployees
                .Where(x => arrEmpId.Contains(x.EmpCode))
                .AsNoTracking().ToArrayAsync();
            int intStepSeq = 1;
            var arrApproveStep = appResult.workflowApprover.Select(item => new SysApproveStep() {
                BrnCode = param.BrnCode,
                CompCode = param.CompCode,
                LocCode = string.Empty,
                DocType = "NonOilPrice",
                CreatedBy = param.CreatedBy,
                CreatedDate = DateTime.Now,
                DocNo = param.DocNo,
                StepNo = intStepSeq++,
                EmpCode = item.empId,
                Guid = param.Guid,
                EmpName = arrEmp?.FirstOrDefault(x => x.EmpCode == item.empId)?.EmpName ?? string.Empty,
            }).ToArray();
            await _context.SysApproveSteps.AddRangeAsync(arrApproveStep);
        }
        private async Task cancelApprove(PriNonoilHd param)
        {
            var qryApproveStep = _context.SysApproveSteps
                .Where(x => x.Guid == param.Guid);
            var arrStep = await qryApproveStep.ToArrayAsync();
            if(arrStep == null || !arrStep.Any())
            {
                return;
            }
            foreach (var item in arrStep)
            {
                item.ApprStatus = "C";
            }
            var qryApproveHeader = _context.SysApproveHds.Where(
                x => qryApproveStep.Any(
                    y => y.DocNo == x.DocNo
                    && y.CompCode == x.CompCode
                )
            );
            var header = await qryApproveHeader.FirstOrDefaultAsync();
            if(header == null)
            {
                return;
            }
            header.ApproveStatus = "C";
        }

        public void Dispose()
        {
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
        #endregion

    }
}
