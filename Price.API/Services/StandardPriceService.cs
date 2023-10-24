using Abp.Linq.Expressions;
using MaxStation.Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Price.API.Domain.Models;
using Price.API.Domain.Repositories;
using Price.API.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Price.API.Services
{
    
    public class StandardPriceService : IStandardPriceService
    {
        #region - Variable -
        private const string _strNew = "New";
        private const string _strActive = "Active";
        private const string _strCancel = "Cancel";
        private const string _strOther = "Other";
        private const string _strReady = "Ready";
        private const string _strOil = "Oil";
        private const string _strGas = "Gas";
        private const string _strStandardPrice = "StandardPrice";
        private const string _strOilGroupId = "0000";
        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWork = null;
        private string[] _arrNoUpdateField = new[] {
            "Guid", "DocDate", "CreatedDate", "CreatedBy"
        };
        private string[] _arrFilterDeiselId = new[] {
            "000001" , "000071" , "000072" ,
            "000073" , "000074"
        };
        private string[] _arrFilterBenzineId = new[] {
            "000002","000004" , "000005",
            "000010","000006" , "000008" ,
        };
        private string[] _arrFilterGasId = new[] { "000011" };
        #endregion
        #region - Start -
        public StandardPriceService(PTMaxstationContext pContext , IUnitOfWork pUnitOfWork)
        {
            _context = pContext;
            _unitOfWork = pUnitOfWork;
        }
        #endregion
        #region - Public Method -
        public async Task<MasBranch[]> GetArrayBranch(string pStrComCode)
        {
            pStrComCode = DefaultService.GetString(pStrComCode);
            ExpressionStarter<MasBranch> exsBranch = null;
            exsBranch = PredicateBuilder.New<MasBranch>(x => _strActive.Equals(x.BrnStatus));
            if (pStrComCode.Length > 0)
            {
                exsBranch = exsBranch.And(x => pStrComCode.Equals(x.CompCode));
            }
            IQueryable<MasBranch> qryBranch = null;
            qryBranch = _context.MasBranches.Where(exsBranch).AsNoTracking();
            MasBranch[] result = null;
            result = await qryBranch.ToArrayAsync();
            return result;
        }
        public async Task<ModelStandardPriceResult> GetArrayHeader(ModelStandardPriceParam param)
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
	FROM OIL_STANDARD_PRICE_HD(NOLOCK)
	WHERE COMP_CODE = '{strComCode}'
	{strFilterDocDate}
	GROUP BY DOC_NO , COMP_CODE
) dn
CROSS APPLY(
	SELECT TOP 1 * 
	FROM OIL_STANDARD_PRICE_HD(NOLOCK)	
	WHERE DOC_NO = dn.DOC_NO AND COMP_CODE = dn.COMP_CODE
) hd";
            string strSqlCount = strSql.Replace("{SELECT}", "COUNT(*)");
            string strSqlSelect = strSql.Replace("{SELECT}", "hd.*") + strOrderBy + strPaging;
            string strCon = DefaultService.GetString(_context.Database.GetConnectionString());
            int intCount = await DefaultService.ExecuteScalar<int>(strCon, strSqlCount);
            OilStandardPriceHd[] arrHeader 
                = await DefaultService.GetEntityFromSql<OilStandardPriceHd[]>(strCon, strSqlSelect);
            if(arrHeader == null)
            {
                return null;
            }
            //using (DataTable dt = await DefaultService.GetDataTable(strCon, strSqlSelect))
            //{
            //    arrHeader = DefaultService.GetEntityFromDataTable<OilStandardPriceHd[]>(dt);
            //}
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
        public async Task<ModelStandardPriceProduct> GetArrayProduct(MasProductPrice pInput)
        {
            if (pInput == null)
            {
                return null;
            }
            Expression<Func<MasProduct, bool>> expProduct = null;
            expProduct = x => _strOilGroupId.Equals(x.GroupId)
                && _strActive.Equals(x.PdStatus);
            IQueryable<MasProduct> qryProduct = null;
            qryProduct = _context.MasProducts.AsNoTracking().Where(expProduct);
            MasProduct[] arrProduct = null;
            MasProductPrice[] arrProductPrice = null;
            arrProduct = await qryProduct.ToArrayAsync();
            ModelStandardPriceProduct result = null;
            result = new ModelStandardPriceProduct()
            {
                ArrayProduct = arrProduct,
                ArrayProductPrice = arrProductPrice,
            };
            return result;
        }
        public async Task<ModelStandardPrice> GetStandardPrice(string pStrGuid)
        {
            pStrGuid = DefaultService.GetString(pStrGuid);
            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }
            Guid guid;
            if (!Guid.TryParse(pStrGuid, out guid))
            {
                return null;
            }
            Expression<Func<OilStandardPriceHd, bool>> exHeader = null;
            exHeader = x => guid.Equals(x.Guid);
            IQueryable<OilStandardPriceHd> qryHeader = null;
            qryHeader = _context.OilStandardPriceHds.AsNoTracking().Where(exHeader);
            OilStandardPriceHd header = null;
            header = await qryHeader.FirstOrDefaultAsync();
            if (header == null)
            {
                return null;
            }
            ExpressionStarter<OilStandardPriceDt> expDt = null;
            expDt = PredicateBuilder.New<OilStandardPriceDt>(
                x => x.CompCode == header.CompCode
                && x.DocNo == header.DocNo
            );
            if (!_strOther.Equals(header.DocType))
            {
                expDt = expDt.And(x => x.BrnCode == header.BrnCode);
            }
            IQueryable<OilStandardPriceDt> qryDetail = null;
            qryDetail = _context.OilStandardPriceDts.AsNoTracking().Where(expDt);
            OilStandardPriceDt[] arrDetail = null;
            arrDetail = await qryDetail.ToArrayAsync();
            ModelStandardPrice result = null;
            result = new ModelStandardPrice()
            {
                ArrayDetail = arrDetail,
                Header = header
            };
            return result;
        }
        public async Task<string> SaveStandardPrice(ModelStandardPrice pStandardPrice)
        {
            OilStandardPriceHd header = pStandardPrice.Header;
            OilStandardPriceDt[] arrDetail = pStandardPrice.ArrayDetail;
            bool isInsert = false;
            if (_strNew.Equals(header.DocStatus))
            {
                await adjustHeaderRunningNo(header);
                header.DocStatus = _strActive;
                header.CreatedDate = DateTime.Now;
                header.Guid = Guid.NewGuid();
                isInsert = true;
            }
            else
            {
                header.UpdatedDate = DateTime.Now;
            }
            foreach (OilStandardPriceDt detail in arrDetail)
            {
                detail.CompCode = header.CompCode;
                detail.DocNo = header.DocNo;
            }
            string strComCode = pStandardPrice.Header.CompCode;
            string[] arrBranch = null;
            DataTable dtDetail = null;
            if (_strOther.Equals(header.DocType))
            {
                arrBranch = arrDetail.Select(x => x.BrnCode).Distinct().ToArray();
                dtDetail = DefaultService.GetDataTableFromIenum(arrDetail);
            }
            else
            {
                arrBranch = await _context.MasBranches
                    .Where(x => x.CompCode == strComCode).Select(x => x.BrnCode)
                    .AsNoTracking().ToArrayAsync();
                dtDetail = await getDtDetail(
                    pArrBranch: arrBranch,
                    pArrDetail: arrDetail, 
                    pStrComCode: header.CompCode
                );
            }
            DataTable dtHeader = await Task.Run(() => getDtHeader(arrBranch, header));
            var strCon = _context.Database.GetConnectionString();
            SqlConnection sqlCon = new SqlConnection(strCon);
            if (sqlCon.State != ConnectionState.Open)
            {
                await sqlCon.OpenAsync();
            }
            SqlTransaction sqlTran = sqlCon.BeginTransaction();
            Func<Task> funcCloseConnection = async () =>
            {
                await sqlTran.DisposeAsync();
                sqlTran = null;
                await sqlCon.CloseAsync();
                await sqlCon.DisposeAsync();
                sqlCon = null;
            };
            SqlBulkCopy bulkHeader = new SqlBulkCopy(sqlCon, SqlBulkCopyOptions.KeepIdentity, sqlTran);
            bulkHeader.DestinationTableName = "OIL_STANDARD_PRICE_HD";
            SqlBulkCopy bulkDetail = new SqlBulkCopy(sqlCon, SqlBulkCopyOptions.KeepIdentity, sqlTran);
            bulkDetail.DestinationTableName = "OIL_STANDARD_PRICE_DT";
            var bulkApprove = new SqlBulkCopy(sqlCon, SqlBulkCopyOptions.KeepIdentity, sqlTran) { 
                DestinationTableName = "SYS_APPROVE_STEP",
            };
            if (!isInsert)
            {
                string strDocNo2 = DefaultService.EncodeSqlString(header.DocNo);
                string strComCode2 = DefaultService.EncodeSqlString(header.CompCode);
                string strSqlDelHeader = $"DELETE OIL_STANDARD_PRICE_HD WHERE DOC_NO='{strDocNo2}' AND COMP_CODE = '{strComCode2}'";
                string strSqlDelDetail = $"DELETE OIL_STANDARD_PRICE_DT WHERE DOC_NO='{strDocNo2}' AND COMP_CODE = '{strComCode2}'";
                SqlCommand sqlComDeleteHeader = new SqlCommand(strSqlDelHeader, sqlCon, sqlTran);
                SqlCommand sqlComDeleteDetail = new SqlCommand(strSqlDelDetail, sqlCon, sqlTran);
                try
                {
                    await sqlComDeleteHeader.ExecuteNonQueryAsync();
                    await sqlComDeleteDetail.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    await sqlTran.RollbackAsync();
                    await funcCloseConnection();
                    throw ex;
                }
                finally
                {
                    await sqlComDeleteHeader.DisposeAsync();
                    await sqlComDeleteDetail.DisposeAsync();
                    sqlComDeleteHeader = null;
                    sqlComDeleteDetail = null;
                }
            }
            try
            {
                if (isInsert)
                {
                    var dtApprove = await getApproveTable(header);
                    if (dtApprove != null && dtApprove.Rows.Count > 0)
                    {
                        await bulkApprove.WriteToServerAsync(dtApprove);
                    }
                }
                if (dtHeader != null && dtHeader.Rows.Count > 0)
                {
                    await bulkHeader.WriteToServerAsync(dtHeader);
                }
                if(dtDetail != null && dtDetail.Rows.Count > 0)
                {
                    await bulkDetail.WriteToServerAsync(dtDetail);
                }               
                await sqlTran.CommitAsync();
            }
            catch (Exception ex)
            {
                await sqlTran.RollbackAsync();
                throw ex;
            }
            finally
            {
                await funcCloseConnection();
                bulkHeader.Close();
                bulkDetail.Close();
                bulkHeader = null;
                bulkDetail = null;
                dtDetail.Clear();
                dtDetail.Dispose();
                dtDetail = null;
                dtHeader.Clear();
                dtHeader.Dispose();
                dtHeader = null;
                bulkApprove.Close();
                bulkApprove = null;
                GC.Collect();
            }
            return header.Guid.ToString();
        }
        public async Task<string> UpdateStatus(OilStandardPriceHd pHeader)
        {
            if (pHeader == null)
            {
                return string.Empty;
            }
            if(_strCancel.Equals( pHeader.DocStatus))
            {
                await cancelApprove(pHeader);
            }
            Expression<Func<OilStandardPriceHd, bool>> expHeader = null;
            expHeader = x => x.CompCode == pHeader.CompCode && x.DocNo == pHeader.DocNo;
            IQueryable<OilStandardPriceHd> qryHeader = null;
            qryHeader = _context.OilStandardPriceHds.Where(expHeader);
            OilStandardPriceHd[] arrHeader = null;
            arrHeader = await qryHeader.ToArrayAsync();
            DateTime now = DateTime.Now;
            foreach (OilStandardPriceHd hd in arrHeader)
            {
                if (_strCancel.Equals(pHeader.DocStatus))
                {
                    hd.ApproveStatus = "N";
                    hd.ApproveDate = DateTime.Now;
                }
                hd.DocStatus = pHeader.DocStatus;
                hd.UpdatedDate = now;
                hd.UpdatedBy = pHeader.UpdatedBy;
            }
            _context.UpdateRange(arrHeader);
            await _unitOfWork.CompleteAsync();
            string result = string.Empty;
            result = DefaultService.GetString(pHeader.Guid);
            return result;
        }
        public async Task<OilStandardPriceDt[]> GetArrayStandardPriceDetail(string pStrCompCode, string pStrBrnCode)
        {
            pStrCompCode = DefaultService.EncodeSqlString(pStrCompCode);
            pStrBrnCode = DefaultService.EncodeSqlString(pStrBrnCode);
            string strSql = $@"select std.* from
(
	select distinct PD_ID from MAS_PRODUCT(nolock)
	where GROUP_ID = '0000'
) pd
cross apply(
	select top 1 dt.* from OIL_STANDARD_PRICE_HD(nolock)hd
	join OIL_STANDARD_PRICE_DT(nolock)dt 
		on hd.DOC_NO = dt.DOC_NO
		and hd.COMP_CODE = dt.COMP_CODE
		and hd.BRN_CODE = dt.BRN_CODE
	where dt.PD_ID = pd.PD_ID
		and hd.COMP_CODE = '{pStrCompCode}'
		and hd.BRN_CODE = '{pStrBrnCode}'
        and ( hd.APPROVE_STATUS = 'Y')
	order by hd.EFFECTIVE_DATE desc
) std";
            string strCon = _context.Database.GetConnectionString();
            OilStandardPriceDt[] result = null;
            result = await DefaultService.GetEntityFromSql<OilStandardPriceDt[]>(strCon, strSql);
            return result;
        }
        public async Task<OilStandardPriceHd> GetUnApproveDocument(string pStrCompCode)
        {
            pStrCompCode = DefaultService.GetString(pStrCompCode);
            if (0.Equals(pStrCompCode.Length))
            {
                return null;
            }
            Expression<Func<OilStandardPriceHd, bool>> expHeader = null;
            //expHeader = x => pStrCompCode.Equals(x.CompCode) && x.ApproveStatus != "Y";
            expHeader = x => pStrCompCode.Equals(x.CompCode) && (x.ApproveStatus ?? string.Empty) == string.Empty;
            IQueryable<OilStandardPriceHd> qryHeader = null;
            qryHeader = _context.OilStandardPriceHds.Where(expHeader).AsNoTracking();
            OilStandardPriceHd result = null;
            result = await qryHeader.FirstOrDefaultAsync();
            return result;

        }
        
        #endregion
        #region - Private Method -
        private async Task adjustHeaderRunningNo(OilStandardPriceHd pStandardPrice)
        {
            string strRunningDocNo = string.Empty;
            MasDocPatternDt[] arrDocPatternDetail = null;
            arrDocPatternDetail = await DefaultService.GetArrayDocPattern(_strStandardPrice, _context);
            bool isUseDefaultPattern = true;
            isUseDefaultPattern = arrDocPatternDetail == null || !arrDocPatternDetail.Any();
            int intLastRunning = 0;
            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pStandardPrice.DocDate != null && pStandardPrice.DocDate.HasValue)
            {
                intDay = pStandardPrice.DocDate.Value.Day;
                intMonth = pStandardPrice.DocDate.Value.Month;
                intYear = pStandardPrice.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            Expression<Func<OilStandardPriceHd, bool>> expStandardPrice = null;
            expStandardPrice = x => x.CompCode == pStandardPrice.CompCode
                && x.DocDate.HasValue;
            IQueryable<OilStandardPriceHd> qryStandardPrice = null;
            qryStandardPrice = _context.OilStandardPriceHds.Where(expStandardPrice);
            if (isUseDefaultPattern)
            {
                qryStandardPrice = qryStandardPrice.Where(
                    x => intYear.Equals(x.DocDate.Value.Year)
                    && intMonth.Equals(x.DocDate.Value.Month)
                );
            }
            else
            {
                arrDocPatternDetail = arrDocPatternDetail.OrderBy(x => x.SeqNo).ToArray();
                if (arrDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryStandardPrice = qryStandardPrice.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (arrDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryStandardPrice = qryStandardPrice.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (arrDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryStandardPrice = qryStandardPrice.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            if (await qryStandardPrice.AnyAsync())
            {
                int intRowCount = await qryStandardPrice.CountAsync();
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
                Expression<Func<OilStandardPriceHd, bool>> expDupplicate = null;
                expDupplicate = x => x.CompCode == pStandardPrice.CompCode
                    && x.DocNo == y;
                IQueryable<OilStandardPriceHd> qryDuplicate = null;
                qryDuplicate = _context.OilStandardPriceHds.AsNoTracking().Where(expDupplicate);
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
                            case "Comp": strRunningDocNo += pStandardPrice.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pStandardPrice.BrnCode; break;
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
            pStandardPrice.DocNo = strRunningDocNo;
        }
        private async Task<DataTable> getDtDetail(string[] pArrBranch, OilStandardPriceDt[] pArrDetail, string pStrComCode)
        {
            DataTable result = new DataTable();
            result.Columns.Add("COMP_CODE", typeof(string));
            result.Columns.Add("BRN_CODE", typeof(string));
            result.Columns.Add("DOC_NO", typeof(string));
            result.Columns.Add("PD_ID", typeof(string));
            result.Columns.Add("UNIT_BARCODE", typeof(string));
            result.Columns.Add("BEFORE_PRICE", typeof(decimal));
            result.Columns.Add("ADJUST_PRICE", typeof(decimal));
            result.Columns.Add("CURRENT_PRICE", typeof(decimal));
            OilStandardPriceDt[] arrCurrentPrice = null;
            arrCurrentPrice = await getArrayCurrentStandardPrice(pStrComCode);
            foreach (string strBranch in pArrBranch)
                foreach (OilStandardPriceDt dt in pArrDetail)
                {
                    decimal decBeforePrice = decimal.Zero;
                    OilStandardPriceDt spCurrent = null;
                    spCurrent = arrCurrentPrice.FirstOrDefault(x => x.BrnCode == strBranch && x.PdId == dt.PdId);
                    if (spCurrent != null)
                    {
                        decBeforePrice = spCurrent.CurrentPrice ?? decimal.Zero;
                    }
                    DataRow dr = result.NewRow();
                    dr["COMP_CODE"] = dt.CompCode;
                    dr["BRN_CODE"] = strBranch;
                    dr["DOC_NO"] = dt.DocNo;
                    dr["PD_ID"] = dt.PdId;
                    dr["UNIT_BARCODE"] = dt.UnitBarcode;
                    dr["BEFORE_PRICE"] = decBeforePrice;
                    dr["ADJUST_PRICE"] = dt.AdjustPrice;
                    dr["CURRENT_PRICE"] = decBeforePrice + dt.AdjustPrice;
                    result.Rows.Add(dr);
                }
            return result;
        }
        private async Task<OilStandardPriceDt[]> getArrayCurrentStandardPrice(string pStrComCode)
        {
            string strComCode = string.Empty;
            strComCode = DefaultService.GetString(pStrComCode);
            string strSql = $@"select std.* from
(
	select distinct PD_ID from MAS_PRODUCT(nolock)
	where GROUP_ID = '{_strOilGroupId}'
) pd
cross apply
(
	select distinct BRN_CODE from MAS_BRANCH(nolock)where COMP_CODE = '{strComCode}'
) brn
cross apply(
	select top 1 dt.* from OIL_STANDARD_PRICE_HD(nolock)hd
	join OIL_STANDARD_PRICE_DT(nolock)dt 
		on hd.DOC_NO = dt.DOC_NO
		and hd.COMP_CODE = dt.COMP_CODE
		and hd.BRN_CODE = dt.BRN_CODE
	where dt.PD_ID = pd.PD_ID
		and hd.COMP_CODE = '{strComCode}'
		and hd.BRN_CODE = brn.BRN_CODE
        and (  hd.APPROVE_STATUS = 'Y')
	order by hd.EFFECTIVE_DATE desc
) std
";
            string strSqlConn = DefaultService.GetString(_context.Database.GetConnectionString());
            OilStandardPriceDt[] result = null;
            result = await DefaultService.GetEntityFromSql<OilStandardPriceDt[]>(strSqlConn, strSql);
            return result;
        }
        private DataTable getDtHeader(string[] pArrBranch, OilStandardPriceHd pHeader)
        {
            DataTable result = new DataTable();
            result.Columns.Add("COMP_CODE", typeof(string));
            result.Columns.Add("BRN_CODE", typeof(string));
            result.Columns.Add("DOC_NO", typeof(string));
            result.Columns.Add("DOC_DATE", typeof(DateTime));
            result.Columns.Add("DOC_STATUS", typeof(string));
            result.Columns.Add("DOC_TYPE", typeof(string));
            result.Columns.Add("EFFECTIVE_DATE", typeof(DateTime));
            result.Columns.Add("REMARK", typeof(string));
            result.Columns.Add("APPROVE_STATUS", typeof(string));
            result.Columns.Add("APPROVE_DATE", typeof(DateTime));
            result.Columns.Add("GUID", typeof(Guid));
            result.Columns.Add("CREATED_DATE", typeof(DateTime));
            result.Columns.Add("CREATED_BY", typeof(string));
            result.Columns.Add("UPDATED_DATE", typeof(string));
            result.Columns.Add("UPDATED_BY", typeof(string));
            Func<object, object> funcDbNullValue = x =>
            {
                if (x == null)
                {
                    return DBNull.Value;
                }
                else
                {
                    return x;
                }
            };
            foreach (var branch in pArrBranch)
            {
                DataRow dr = result.NewRow();
                dr["COMP_CODE"] = pHeader.CompCode;
                dr["BRN_CODE"] = branch;
                dr["DOC_NO"] = pHeader.DocNo;
                dr["DOC_DATE"] = pHeader.DocDate;
                dr["DOC_STATUS"] = pHeader.DocStatus;
                dr["DOC_TYPE"] = pHeader.DocType;
                dr["EFFECTIVE_DATE"] = funcDbNullValue(pHeader.EffectiveDate);
                dr["REMARK"] = pHeader.Remark;
                dr["APPROVE_STATUS"] = pHeader.ApproveStatus;
                dr["APPROVE_DATE"] = funcDbNullValue(pHeader.ApproveDate);
                dr["GUID"] = pHeader.Guid;
                dr["CREATED_DATE"] = funcDbNullValue(pHeader.CreatedDate);
                dr["CREATED_BY"] = (pHeader.CreatedBy);
                dr["UPDATED_DATE"] = funcDbNullValue(pHeader.UpdatedDate);
                dr["UPDATED_BY"] = (pHeader.UpdatedBy);
                result.Rows.Add(dr);
            }
            return result;
        }
        private async Task cancelApprove(OilStandardPriceHd param)
        {
            var arrStep = await _context.SysApproveSteps.Where(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.DocNo == param.DocNo
            ).ToArrayAsync();
            foreach (var item in arrStep)
            {
                item.ApprStatus = "C";
            }
        }
        private async Task<DataTable> getApproveTable(OilStandardPriceHd param)
        {
            if (param == null)
            {
                return null;
            }
            SysApproveConfig config = await _context.SysApproveConfigs.FirstOrDefaultAsync(x => "OilPrice" == x.DocType);
            if (config == null)
            {
                return null;
            }
            var appResult = await DefaultService.GetApproverStep(config.StepCount ?? 1, param.CreatedBy, _context);
            if (!appResult?.workflowApprover?.Any() ?? true)
            {
                return null;
            }
            var arrEmpId = appResult.workflowApprover.Select(x => x.empId).ToArray();
            var arrEmp = await _context.MasEmployees
                .Where(x => arrEmpId.Contains(x.EmpCode))
                .AsNoTracking().ToArrayAsync();
            int intStepSeq = 1;
            var listStep = new List<SysApproveStep>();
            foreach (var item in appResult.workflowApprover)
            {
                SysApproveStep step = new SysApproveStep();
                step.BrnCode = param.BrnCode;
                step.CompCode = param.CompCode;
                step.LocCode = string.Empty;
                step.DocType = "OilPrice";
                step.CreatedBy = param.CreatedBy;
                step.CreatedDate = DateTime.Now;
                step.DocNo = param.DocNo;
                step.StepNo = intStepSeq++;
                step.EmpCode = item.empId;
                step.Guid = param.Guid;
                step.EmpName = arrEmp?.FirstOrDefault(x => x.EmpCode == item.empId)?.EmpName ?? string.Empty;
                listStep.Add(step);
            }
            DataTable result = null;
            using (var dtStep = DefaultService.GetDataTableFromIenum(listStep)) {
                if(dtStep != null && dtStep.Rows.Count > 0 && dtStep.Columns.Contains("GUID"))
                {
                    result = dtStep.Clone();
                    result.Columns["GUID"].DataType = typeof(Guid);
                    foreach (DataRow row in dtStep.Rows)
                    {
                        result.ImportRow(row);
                    }
                }
            }           
            return result;
        }
        #endregion
    }
}
