using Abp.Linq.Expressions;
using MasterData.API.Domain.Models;
using MasterData.API.Domain.Repositories;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MasterData.API.Services
{
	public class ApproveService : IDisposable
	{
        private const string _strApprove = "Approve";
        private const string _strActive = "Active";
		private PTMaxstationContext _context = null;
		private IUnitOfWork _unitOfWork = null;
		public ApproveService(PTMaxstationContext pContext, IUnitOfWork pUnitOfWork)
		{
			_context = pContext;
			_unitOfWork = pUnitOfWork;
		}

		public ApproveService()
		{
			_context = new PTMaxstationContext();
		}

		public async Task<ModelArrayApprove> GetArraySysApprove(string pStrEmpCode) {
			var qryStep = _context.SysApproveSteps.Where(
				x => x.EmpCode == pStrEmpCode
				&& !string.IsNullOrWhiteSpace(x.ApprCode)
			).AsNoTracking();
			var arrStep = await qryStep.ToArrayAsync();
			SysApproveHd[] arrHeader = null;
			if (arrStep != null && arrStep.Any())
			{
				var arrDocNo = arrStep.Select(x => x.ApprCode).Distinct().ToArray();
				var qryHeader = _context.SysApproveHds
                    .Where(x => arrDocNo.Contains(x.DocNo))
                    .OrderByDescending(x=> x.CreatedDate);
				arrHeader = await qryHeader.ToArrayAsync();
			}
            MasEmployee[] arrEmp = null;
            if(arrHeader != null && arrHeader.Any())
            {
                var arrempCode = arrHeader.Select(x => x.CreatedBy).Distinct().ToArray();
                var qryEmp = _context.MasEmployees.Where(x => arrempCode.Contains(x.EmpCode) && _strActive.Equals(x.WorkStatus));
                arrEmp = await qryEmp.ToArrayAsync();
            }
            var result = new ModelArrayApprove()
            {
                ArrayHeader = arrHeader,
                ArrayStep = arrStep,
                ArrayEmployee = arrEmp,
			};
			return result;
	}
		public async Task<SysApproveStep[]> GetPendingApprove(string pStrEmpCode)
		{
			string strSql = $@"SELECT * FROM SYS_APPROVE_STEP(NOLOCK) sas
WHERE EMP_CODE = '{pStrEmpCode}'
	AND ISNULL( APPR_STATUS,'') = ''
	AND ( STEP_NO = 1 OR 
		EXISTS(SELECT 1 FROM SYS_APPROVE_STEP(NOLOCK) WHERE STEP_NO = sas.STEP_NO -1 
			AND APPR_STATUS = 'Y'
			AND comp_code = sas.comp_code
			AND brn_code = sas.brn_code
			AND loc_code = sas.loc_code
			AND doc_no = sas.doc_no
		)	 
	)";
			string strCon = _context.Database.GetConnectionString();
			SysApproveStep[] result = null;
			result = await DefaultService.GetEntityFromSql<SysApproveStep[]>(strCon, strSql);
			return result;
		}
        public async Task<ModelApprove> GetApproveByGuid(string strGuid)
        {
            Guid guid;
            if (!Guid.TryParse(strGuid , out guid))
            {
                return null;
            }
            var qryHd = _context.SysApproveHds.Where(x => guid.Equals(x.Guid));
            var header = await qryHd.FirstOrDefaultAsync();
            if(header == null)
            {
                return null;
            }
            var qryStep = _context.SysApproveSteps.Where(
                x => x.ApprCode == header.DocNo
                //&& x.CompCode == header.CompCode
            ).AsNoTracking();
            var arrStep = await qryStep.ToArrayAsync();
            var result = new ModelApprove()
            {
                Header = header,
                ArrayStep = arrStep
            };
            return result;
        }
        public async Task<SysApproveStep[]> ValidateApproveDocument(ModelApproveParam param)
        {
            if(param == null)
            {
                return null;
            }
            var qryStep = _context.SysApproveSteps.Where(
                x => param.DocNo == x.DocNo
                && param.BrnCode == x.BrnCode
                && param.CompCode == x.CompCode
                && param.LocCode == x.LocCode
                && param.DocType == x.DocType
                && x.ApprStatus == "Y"
            ).AsNoTracking();
            var result = await qryStep.ToArrayAsync();
            return result;
        }

        public async Task<ModelApprove> GetApprove(ModelApproveParam param)
        {
            if(param == null)
            {
                return null;
            }
            var qryHd = _context.SysApproveHds.Where(
                x => x.DocNo == param.DocNo
                && x.CompCode == param.CompCode
                && x.LocCode == param.LocCode
                && x.BrnCode == param.BrnCode
            ).AsNoTracking();
            var header = await qryHd.FirstOrDefaultAsync();
			var qryStep = _context.SysApproveSteps.Where(
                x => x.ApprCode == param.DocNo
                && x.CompCode == param.CompCode
                && x.BrnCode == param.BrnCode
                && x.EmpCode == param.EmpCode
                && x.LocCode == param.LocCode
            ).AsNoTracking();
            var arrStep = await qryStep.ToArrayAsync();
			var result = new ModelApprove() { 
                Header = header ,
                ArrayStep = arrStep
            };
			return result;
        }
        public async Task<ModelApprove> SaveApprove(ModelApprove param)
        {
            await adjustHeaderRunningNo(param.Header);
            param.Header.Guid = Guid.NewGuid();
            param.Header.CreatedDate = DateTime.Now;
            await _context.AddAsync(param.Header);
            _context.AttachRange(param.ArrayStep);
            foreach (var step in param.ArrayStep)
            {
                step.ApprCode = param.Header.DocNo;
                step.ApprStatus = param.Header.ApproveStatus;
                step.UpdatedDate = DateTime.Now;
                step.UpdatedBy = param.Header.CreatedBy;
            }
            await updateDestinationStatus(param.ArrayStep);
            await _unitOfWork.CompleteAsync();
            return param;
        }
        public async Task<SysApproveConfig[]> GetArrayConfig()
        {
            var qryConfig = _context.SysApproveConfigs.AsNoTracking();
            var result = await qryConfig.ToArrayAsync();
            return result;
        }

		public async Task<SysApproveHd[]> GetArraySysApproveOld(string pStrEmpCode) {
			pStrEmpCode = DefaultService.EncodeSqlString(pStrEmpCode);
			string strSql = @$"
SELECT * FROM SYS_APPROVE_HD(NOLOCK)hd
WHERE EXISTS(
	SELECT 1 FROM SYS_APPROVE_STEP(NOLOCK)sas
	WHERE EMP_CODE = '{pStrEmpCode}'	
		AND APPR_STATUS = ''
		AND EXISTS(SELECT 1 FROM SYS_APPROVE_STEP(NOLOCK) WHERE 
			STEP_NO = sas.STEP_NO -1 
			AND	hd.DOC_NO = APPR_CODE
			AND APPR_STATUS = 'Y'
			AND comp_code = sas.comp_code
			AND brn_code = sas.brn_code
			AND loc_code = sas.loc_code
			AND doc_no = sas.doc_no
		)		
)";
			string strCon = _context.Database.GetConnectionString();
			var result = await DefaultService.GetEntityFromSql<SysApproveHd[]>(strCon, strSql);
			return result;
		}
        public async Task< DataTable> GetPendingApproveOld(string pStrCompCode) {
            string strSql = @"
DECLARE @sql nvarchar(max) = (
 select stuff((
	select ' Union select Doc_No DocNo , Doc_Date DocDate , Brn_Code BrnCode , Guid , '''
		+ Table_name 
		+ ''' TableName '
		+ case when Doc_Type = 1 then ', Doc_Type DocType ' else ' , null DocType ' end
		+ ' from ' 
		+ Table_name 
		+ '(nolock) where doc_status = ''Active'' and Comp_code= ''B'' ' 

	from
	(
		select Table_name
		from INFORMATION_SCHEMA.COLUMNS
		where COLUMN_NAME = 'DOC_STATUS'
		group by TABLE_NAME
	) tblName
	outer apply(
		select top 1 1 Doc_Type
		from INFORMATION_SCHEMA.COLUMNS
		where Table_Name = tblName.Table_name
			and COLUMN_NAME = 'Doc_type'
	) Doc_type
	FOR XML PATH(''))
,1,7,'')
);
EXECUTE sp_executesql @sql;
";
			string strCon = _context.Database.GetConnectionString();
			DataTable result = await DefaultService.GetDataTable(strCon , strSql);
			return result;
        }

        public async Task<ModelSearchApproveResult> SearchApprove(ModelSearchApproveParam param)
        {
            if(param == null)
            {
                return null;
            }
            var result = new ModelSearchApproveResult();
            var esApprove = PredicateBuilder.New<SysApproveHd>();
            if( !string.IsNullOrWhiteSpace(param.EmpCode))
            {
                esApprove = esApprove.And(x => x.CreatedBy == param.EmpCode);
            }
            if(param.EndDate.HasValue)
            {
                esApprove = esApprove.And(x => x.DocDate <= param.EndDate);
            }
            if (param.StartDate.HasValue)
            {
                esApprove = esApprove.And(x => x.DocDate >= param.StartDate);
            }
            if (!string.IsNullOrWhiteSpace(param.KeyWord))
            {
                esApprove = esApprove.And(x=> x.DocNo.Contains(param.KeyWord)
                    || param.KeyWord.Contains(x.DocNo)
                );
            }
            var qryApprove = _context.SysApproveHds
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedDate)
                .Where(esApprove);
            result.TotalItem = await qryApprove.CountAsync();
            if(param.ItemPerPage > 0 && param.PageIndex > 0)
            {

                qryApprove = qryApprove.Skip((param.PageIndex - 1) * param.ItemPerPage).Take(param.ItemPerPage);
            }
            result.ArrApproveHeader =await  qryApprove.ToArrayAsync();
            if(result.ArrApproveHeader != null && result.ArrApproveHeader.Any())
            {
                var arrEmpCode = result.ArrApproveHeader
                    .Select(x => x.CreatedBy).Distinct().ToArray();
                var qryEmp = _context.MasEmployees.Where(
                    x => arrEmpCode.Contains(x.EmpCode)
                ).AsNoTracking();
                result.ArrEmployee = await qryEmp.ToArrayAsync();
            }
            return result;
        }

        private async Task adjustHeaderRunningNo(SysApproveHd pApproveHd)
        {
            string strRunningDocNo = string.Empty;
            MasDocPatternDt[] arrDocPatternDetail = null;
            arrDocPatternDetail = await DefaultService.GetArrayDocPattern(_strApprove, _context);
            bool isUseDefaultPattern = true;
            isUseDefaultPattern = arrDocPatternDetail == null || !arrDocPatternDetail.Any();
            int intLastRunning = 0;
            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pApproveHd.DocDate != null && pApproveHd.DocDate.HasValue)
            {
                intDay = pApproveHd.DocDate.Value.Day;
                intMonth = pApproveHd.DocDate.Value.Month;
                intYear = pApproveHd.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            Expression<Func<SysApproveHd, bool>> expApproveHd = null;
            expApproveHd = x => x.CompCode == pApproveHd.CompCode
                && x.DocDate.HasValue;
            IQueryable<SysApproveHd> qryApprove = null;
            qryApprove = _context.SysApproveHds.Where(expApproveHd);
            if (isUseDefaultPattern)
            {
                qryApprove = qryApprove.Where(
                    x => intYear.Equals(x.DocDate.Value.Year)
                    && intMonth.Equals(x.DocDate.Value.Month)
                );
            }
            else
            {
                arrDocPatternDetail = arrDocPatternDetail.OrderBy(x => x.SeqNo).ToArray();
                if (arrDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryApprove = qryApprove.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (arrDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryApprove = qryApprove.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (arrDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryApprove = qryApprove.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            if (await qryApprove.AnyAsync())
            {
                int intRowCount = await qryApprove.CountAsync();
                intLastRunning = intRowCount;
            }
            bool isDuplicate = false;
            Func<string, Task<bool>> funcIsDuplicate = null;
            funcIsDuplicate = async y =>
            {
                y = DefaultService.GetString(y);
                if (0.Equals(y.Length))
                {
                    return false;
                }
                Expression<Func<SysApproveHd, bool>> expDupplicate = null;
                expDupplicate = x => x.CompCode == pApproveHd.CompCode
                    && x.DocNo == y;
                IQueryable<SysApproveHd> qryDuplicate = null;
                qryDuplicate = _context.SysApproveHds.AsNoTracking().Where(expDupplicate);
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
                            case "Comp": strRunningDocNo += pApproveHd.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pApproveHd.BrnCode; break;
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
            pApproveHd.DocNo = strRunningDocNo;
            pApproveHd.RunNumber = intLastRunning;
        }
        private async Task updateDestinationStatus(SysApproveStep[] pArrStep)
        {
            if(pArrStep == null || !pArrStep.Any())
            {
                return;
            }
            Func<SysApproveStep , Task> funcUpDateQuotation = async x =>
            {
                if (x == null)
                {
                    return;
                }
                var qryQuotation = _context.SalQuotationHds.Where(
                    y => y.DocNo == x.DocNo 
                    &&  y.CompCode == x.CompCode
                    && y.BrnCode == x.BrnCode
                    && y.LocCode == x.LocCode
                );
                var quotation = await qryQuotation.FirstOrDefaultAsync();
                if(quotation == null)
                {
                    return;
                }
                if(x.ApprStatus == "Y")
                {
                    quotation.DocStatus = "Ready";
                }
                else
                {
                    quotation.DocStatus = "Cancel";
                }
            };
            Func<SysApproveStep, Task> funcUpDateRequest = async x =>
            {
                if (x == null)
                {
                    return;
                }
                var qryRequest = _context.InvRequestHds.Where(
                    y => y.DocNo == x.DocNo
                    && y.CompCode == x.CompCode
                    && y.BrnCode == x.BrnCode
                    && y.LocCode == x.LocCode
                );
                var request = await qryRequest.FirstOrDefaultAsync();
                if (request == null)
                {
                    return;
                }
                if (x.ApprStatus == "Y")
                {
                    request.DocStatus = "Ready";
                }
                else
                {
                    request.DocStatus = "Cancel";
                }
            };
            Func<SysApproveStep, Task> funcUpDateOilPrice = async x => {
                if (x == null)
                {
                    return;
                }
                var qryOilPrice = _context.OilStandardPriceHds.Where(
                    y => y.DocNo == x.DocNo
                    && y.CompCode == x.CompCode                   
                );
                var arrOilPrice = await qryOilPrice.ToArrayAsync();
                if (arrOilPrice == null || !arrOilPrice.Any())
                {
                    return;
                }
                var now = DateTime.Now;
                if (x.ApprStatus == "Y")
                {
                    foreach (var item in arrOilPrice)
                    {
                        item.DocStatus = "Ready";
                        item.ApproveStatus = "Y";
                        item.ApproveDate = now;
                    }
                }
                else
                {
                    foreach (var item in arrOilPrice)
                    {
                        item.DocStatus = "Cancel";
                        item.ApproveStatus = "N";
                        item.ApproveDate = now;
                    }
                }
            };
            Func<SysApproveStep, Task> funcUpDateNonOilPrice = async x => {
                if (x == null)
                {
                    return;
                }
                var qryOilPrice = _context.PriNonoilHds.Where(
                    y => y.DocNo == x.DocNo
                    && y.CompCode == x.CompCode
                );
                var arrOilPrice = await qryOilPrice.ToArrayAsync();
                if (arrOilPrice == null || !arrOilPrice.Any())
                {
                    return;
                }
                var now = DateTime.Now;
                if (x.ApprStatus == "Y")
                {
                    foreach (var item in arrOilPrice)
                    {
                        item.DocStatus = "Ready";
                        item.ApproveStatus = "Y";
                        item.ApproveDate = now;
                    }
                }
                else
                {
                    foreach (var item in arrOilPrice)
                    {
                        item.DocStatus = "Cancel";
                        item.ApproveStatus = "N";
                        item.ApproveDate = now;
                    }
                }
            };
            Func <SysApproveStep, Task<bool>> funcIsAllApprove = async x =>
            {
                var qryStep = _context.SysApproveSteps.Where(
                    y => x.DocNo == y.DocNo
                    && x.DocType == y.DocType
                    && x.CompCode == y.CompCode
                    && x.BrnCode == y.BrnCode
                    && x.LocCode == y.LocCode
                    && x.StepNo != y.StepNo
                ).AsNoTracking();
                //var arrStep = await qryStep.ToArrayAsync();
                //bool isAllApprove = true;
                //foreach (var step in arrStep)
                //{
                //    if(step.ApprStatus != "Y")
                //    {
                //        isAllApprove = false;
                //        break;
                //    }
                //}
                bool isAllApprove = await qryStep.AllAsync(x=> x.ApprStatus == "Y");
                return isAllApprove;
            };
            foreach (var step in pArrStep)
            {
                if(step == null)
                {
                    continue;
                }
                if( step.ApprStatus == "N" || (step.ApprStatus == "Y" && await funcIsAllApprove(step)))
                {
                    switch (step.DocType)
                    {
                        case "Quotation":
                            await funcUpDateQuotation(step);
                            break;
                        case "Request":
                            await funcUpDateRequest(step);
                            break;
                        case "OilPrice":
                            await funcUpDateOilPrice(step);
                            break;
                        case "NonOilPrice":
                            await funcUpDateNonOilPrice(step);
                            break;
                        default:
                            break;
                    }
                }
                
            }
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
    }
}
