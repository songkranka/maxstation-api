using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
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
    public class SupplierReturnController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SupplierReturnController));

        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWork = null;
        public SupplierReturnController(PTMaxstationContext context, IUnitOfWork pUnitOfWork) : base(context)
        {
            _context = context;
            _unitOfWork = pUnitOfWork;
        }

        //GetArraySupplierReturnHeader
        [HttpPost("GetArraySupplierReturnHeader")]
        public async Task<IActionResult> GetArraySupplierReturnHeader(ModelSupplierReturnParam param)
        {
            return await doAction("GetArraySupplierReturnHeader", async () => await getArraySupplierReturnHeader(param));
        }

        [HttpPost("GetArrayReceiveProduct")]
        public async Task<IActionResult> GetArrayReceiveProduct(InvReturnOilHd param)
        {
            return await doAction("GetArrayReceiveProduct", async () => await getArrayReceiveProduct(param));
        }

        [HttpGet("GetArrayBranch")]
        public async Task<IActionResult> GetArrayBranch()
        {
            return await doAction("GetArrayBranch", async () => await getArrayBranch());
        }

        [HttpGet("GetArrayPoItem/{pStrPoNumber}")]
        public async Task<IActionResult> GetArrayPoItem(string pStrPoNumber)
        {
            return await doAction("GetArrayPoItem", async () => await getArrayPoItem(pStrPoNumber));
        }

        [HttpGet("GetArrayReason")]
        public async Task<IActionResult> GetArrayReason()
        {
            return await doAction("GetArrayReason", async () => await getArrayReason());
        }

        [HttpGet("GetArrayReasonDesc/{ReasonID}")]
        public async Task<IActionResult> GetArrayReasonDesc(string ReasonID)
        {
            return await doAction("GetArrayReasonDesc", async () => await getArrayReasonDesc(ReasonID));
        }

        [HttpGet("GetSupplier/{pStrSupCode}")]
        public async Task<IActionResult> GetSupplier(string pStrSupCode)
        {
            return await doAction("GetSupplier", async () => await getSupplier(pStrSupCode));
        }

        [HttpGet("GetEmployee/{pStrEmpCode}")]
        public async Task<IActionResult> GetEmployee(string pStrEmpCode)
        {
            return await doAction("GetEmployee", async () => await getEmployee(pStrEmpCode));
        }

        [HttpGet("GetSupplierReturn/{pStrGuid}")]
        public async Task<IActionResult> GetSupplierReturn(string pStrGuid)
        {
            return await doAction("GetSupplierReturn", async () => await getSupplierReturn(pStrGuid));
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(InvReturnSupHd pHeader)
        {
            return await doAction("UpdateStatus", async () => await updateStatus(pHeader));
        }

        [HttpPost("SaveSupplierReturn")]
        public async Task<IActionResult> SaveSupplierReturn(ModelSupplierReturn param)
        {
            return await doAction("SaveSupplierReturn", async () => await saveSupplierReturn(param));
        }

        [HttpPost("GetArrayReceiveProdDt")]
        public async Task<IActionResult> GetArrayReceiveProdDt(InvReceiveProdHd param)
        {
            return await doAction("GetArrayReceiveProdDt", async () => await getArrayReceiveProdDt(param));
        }

        #region function
        private const string _strReturnSup = "ReturnSup";
        private const string _strActive = "Active";
        private const string _strNew = "New";
        private const string _strPoDelete = "L";
        private const string _strPo_type_desc = "Lube";

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
            result = Content(strJson, "application/json");
            return result;
        }

        private BadRequestObjectResult exeptionResult(Exception pException)
        {
            string strErrorMessage = string.Empty;
            strErrorMessage = getErrorMessage(pException);
            return BadRequest(strErrorMessage);
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

        private async Task<ModelSupplierReturnResult> getArraySupplierReturnHeader(ModelSupplierReturnParam param)
        {
            if (param == null)
            {
                return null;
            }
            IQueryable<InvReturnSupHd> qrySupplierReturn = null;
            qrySupplierReturn = context.InvReturnSupHds.Where(
                x => x.CompCode == param.CompCode
                && x.BrnCode == param.BrnCode
                && x.LocCode == param.LocCode
            ).AsNoTracking();
            if (param.FromDate.HasValue)
            {
                DateTime from_date = param.FromDate ?? DateTime.Now;
                from_date = from_date.AddDays(1);
                qrySupplierReturn = qrySupplierReturn.Where(x => x.DocDate >= from_date);
            }
            if (param.ToDate.HasValue)
            {
                DateTime to_date = param.ToDate ?? DateTime.Now;
                to_date = to_date.AddDays(1);
                qrySupplierReturn = qrySupplierReturn.Where(x => x.DocDate <= to_date);
            }
            if (!string.IsNullOrWhiteSpace(param.Keyword))
            {
                qrySupplierReturn = qrySupplierReturn.Where(
                    x => x.DocNo.Contains(param.Keyword)
                    || x.SupCode.Contains(param.Keyword)
                    || x.SupName.Contains(param.Keyword)
                    || x.CreatedBy.Contains(param.Keyword)
                );
            }
            int intTotalItem = 0;
            intTotalItem = await qrySupplierReturn.CountAsync();
            qrySupplierReturn = qrySupplierReturn.OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo);
            if (param.Page > 0 && param.ItemsPerPage > 0)
            {
                qrySupplierReturn = qrySupplierReturn
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip((param.Page - 1) * param.ItemsPerPage)
                    .Take(param.ItemsPerPage);
            }
            //qrySupplierReturn = qrySupplierReturn.OrderByDescending(x => x.CreatedDate);
            InvReturnSupHd[] arraySupplierReturnHeader = null;
            arraySupplierReturnHeader = await qrySupplierReturn.ToArrayAsync();

            if (arraySupplierReturnHeader == null || !arraySupplierReturnHeader.Any())
            {
                return null;
            }
            string[] arrCreateBy = null;
            arrCreateBy = arraySupplierReturnHeader
                .Select(x => (x.CreatedBy ?? string.Empty).Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            string[] arrUpdateBy = null;
            arrUpdateBy = arraySupplierReturnHeader
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
            ModelSupplierReturnResult result = null;
            result = new ModelSupplierReturnResult()
            {
                ArrayEmployee = arrEmployee,
                ArraySupplierReturnHeader = arraySupplierReturnHeader,
                TotalItems = intTotalItem,
            };
            return result;
        }


        private async Task<InvReceiveProdHd[]> getArrayReceiveProduct(InvReturnOilHd param)
        {

            if (param == null)
            {
                return null;
            }
            IQueryable<InfPoType> qryPoType = null;
            qryPoType = context.InfPoTypes.Where(
                    x => _strPo_type_desc.Equals(x.PoTypeDesc)
                ).AsNoTracking();
            string[] _strPoType = (await qryPoType.ToArrayAsync()).Select(x => x.PoTypeId).ToArray();

            IQueryable<InvReceiveProdHd> qryReceiveProd = null;
            //qryReceiveProd = context.InvReceiveProdHds.Where(
            //    x => _strActive.Equals(x.DocStatus)
            //    && x.BrnCode == param.BrnCode
            //    && x.CompCode == param.CompCode
            //    && x.LocCode == param.LocCode
            //    && _strPoType.Contains(x.PoTypeId)
            //   )
            //    .AsNoTracking();
            qryReceiveProd = (from rh in context.InvReceiveProdHds.AsNoTracking()
                              join rd in context.InvReceiveProdDts.AsNoTracking()
                              on rh.DocNo equals rd.DocNo
                              where rd.ItemQty - rd.ReturnQty > 0
                              && rh.BrnCode == param.BrnCode
                              && rh.CompCode == param.CompCode
                              && rh.LocCode == param.LocCode
                               && _strPoType.Contains(rh.PoTypeId)
                              && _strActive.Equals(rh.DocStatus)
                              select new InvReceiveProdHd()
                              {
                                  BrnCode = rh.BrnCode,
                                  CompCode = rh.CompCode,
                                  LocCode = rh.LocCode,
                                  DocType = rh.DocType,
                                  DocNo = rh.DocNo,
                                  DocDate = rh.DocDate,
                                  PoNo = rh.PoNo,
                                  PoDate = rh.PoDate,
                                  SupCode = rh.SupCode,
                                  SupName = rh.SupName,
                              }).Distinct();

            InvReceiveProdHd[] result = null;
            result = await qryReceiveProd.ToArrayAsync();

            return result;
        }

        private async Task<InvReceiveProdDt[]> getArrayPoItem(string pStrPoNumber)
        {
            if (pStrPoNumber == null)
            {
                return null;
            }
            //IQueryable<InfPoItem> qryPoItem = null;
            //qryPoItem = context.InfPoItems.Where(
            //    x => x.Plant == pStrBrnCode
            //    && !context.Equals(x.DeleteInd)
            //).AsNoTracking();
            QueryResult<InvReceiveProdDt> result = null;
            result = new QueryResult<InvReceiveProdDt>();

            IQueryable<InvReceiveProdDt> qryReceiveDt = null;
            qryReceiveDt = context.InvReceiveProdDts
                .Where(x => pStrPoNumber.Equals(x.DocNo))
                .AsNoTracking();

            //InfPoHeader[] result = null;
            return await qryReceiveDt.ToArrayAsync();
        }
        private async Task<MasBranch[]> getArrayBranch()
        {
            IQueryable<MasBranch> qryBranch = null;
            qryBranch = context.MasBranches
                .Where(x => _strActive.Equals(x.BrnStatus))
                .AsNoTracking();

            MasBranch[] result = null;
            result = await qryBranch.ToArrayAsync();
            return result;
        }
        private async Task<MasSupplier> getSupplier(string pStrSupCode)
        {
            pStrSupCode = (pStrSupCode ?? string.Empty).Trim();
            if (0.Equals(pStrSupCode.Length))
            {
                return null;
            }
            IQueryable<MasSupplier> qrySup = null;
            qrySup = context.MasSuppliers
                .Where(x => pStrSupCode.Equals(x.SupCode))
                .AsNoTracking();

            MasSupplier result = null;
            result = await qrySup.FirstOrDefaultAsync();
            return result;
        }
        private async Task<MasReason[]> getArrayReason()
        {
            IQueryable<MasReason> qryReason = null;
            qryReason = context.MasReasons.Where(
                x => _strReturnSup.Equals(x.ReasonGroup)
                && _strActive.Equals(x.ReasonStatus)
            ).AsNoTracking();
            MasReason[] result = null;
            result = await qryReason.ToArrayAsync();
            return result;
        }
        private async Task<MasReason[]> getArrayReasonDesc(string ReasonID)
        {
            IQueryable<MasReason> qryReason = null;
            qryReason = context.MasReasons.Where(
                x => _strReturnSup.Equals(x.ReasonGroup)
                && _strActive.Equals(x.ReasonStatus)
                && ReasonID.Equals(x.ReasonId)
            ).AsNoTracking();
            MasReason[] result = null;
            result = await qryReason.ToArrayAsync();
            return result;
        }

        private async Task<MasEmployee> getEmployee(string pStrEmpCode)
        {
            pStrEmpCode = (pStrEmpCode ?? string.Empty).Trim();
            if (0.Equals(pStrEmpCode.Length))
            {
                return null;
            }
            IQueryable<MasEmployee> qryEmp = null;
            qryEmp = context.MasEmployees
                .Where(x => pStrEmpCode.Equals(x.EmpCode))
                .AsNoTracking();

            MasEmployee result = null;
            result = await qryEmp.FirstOrDefaultAsync();
            return result;
        }

        private async Task<MasProduct[]> getArrayProductUnit(InfPoItem[] pArrPoItem)
        {
            if (pArrPoItem == null || !pArrPoItem.Any())
            {
                return null;
            }
            string[] arrMaterial = null;
            arrMaterial = pArrPoItem
                .Select(x => (x.Material ?? string.Empty).Trim())
                .Where(x => !0.Equals(x.Length))
                .Distinct()
                .ToArray();
            if (arrMaterial == null || !arrMaterial.Any())
            {
                return null;
            }
            IQueryable<MasProduct> qryProduct = null;
            qryProduct = context.MasProducts
                .Where(x => arrMaterial.Contains(x.PdId))
                .AsNoTracking();

            MasProduct[] result = null;
            result = await qryProduct.ToArrayAsync();

            return result;
        }
        private async Task<MasUnit[]> getArrayUnit(InfPoItem[] pArrPoItem)
        {
            if (pArrPoItem == null || !pArrPoItem.Any())
            {
                return null;
            }
            string[] arrUnit = null;
            arrUnit = pArrPoItem
                .Select(x => (x.PoUnit ?? string.Empty).Trim())
                .Where(x => !0.Equals(x.Length))
                .Distinct()
                .ToArray();
            if (arrUnit == null || !arrUnit.Any())
            {
                return null;
            }
            IQueryable<MasUnit> qryUnit = null;
            qryUnit = context.MasUnits
                .Where(x => arrUnit.Contains(x.MapUnitId))
                .AsNoTracking();

            MasUnit[] result = null;
            result = await qryUnit.ToArrayAsync();

            return result;
        }

        private async Task<ModelSupplierReturn> getSupplierReturn(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();

            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }

            Guid guidHeader;

            if (!Guid.TryParse(pStrGuid, out guidHeader))
            {
                return null;
            }


            IQueryable<InvReturnSupHd> qryHeader = null;
            qryHeader = context.InvReturnSupHds
                .Where(x => guidHeader.Equals(x.Guid))
                .AsNoTracking();

            InvReturnSupHd header = null;
            header = await qryHeader.FirstOrDefaultAsync();

            if (header == null)
            {
                return null;
            }

            IQueryable<InvReturnSupDt> qryDetail = null;
            qryDetail = context.InvReturnSupDts.Where(
                x => x.CompCode == header.CompCode
                && x.BrnCode == header.BrnCode
                && x.LocCode == header.LocCode
                && x.DocNo == header.DocNo
            ).AsNoTracking();

            InvReturnSupDt[] arrDetail = null;
            arrDetail = await qryDetail.ToArrayAsync();
            ModelSupplierReturn result = null;
            result = new ModelSupplierReturn()
            {
                Header = header,
                ArrayDetail = arrDetail
            };

            return result;
        }
        private async Task<InvReturnSupHd> updateStatus(InvReturnSupHd pHeader)
        {
            if (pHeader == null)
            {
                return null;
            }
            EntityEntry<InvReturnSupHd> entHeader = null;
            entHeader = context.Attach(pHeader);
            pHeader.UpdatedDate = DateTime.Now;
            entHeader.Property(x => x.DocStatus).IsModified = true;
            await _unitOfWork.CompleteAsync();
            return pHeader;
        }

        private async Task<ModelSupplierReturn> saveSupplierReturn(ModelSupplierReturn param)
        {
            if (param == null)
            {
                return null;
            }
            InvReturnSupHd header = null;
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
                await _context.InvReturnSupHds.AddAsync(header);
            }
            else
            {
                header.UpdatedDate = DateTime.Now;
                EntityEntry<InvReturnSupHd> entHeader = null;
                entHeader = _context.Update(header);

                string[] arrNoUpdateField = null;
                arrNoUpdateField = new[] {
                    "RunNumber", "Guid", "DocPattern",
                    "DocDate", "CreatedDate", "CreatedBy" , "PoNo"
                };
                foreach (string strField in arrNoUpdateField)
                {
                    entHeader.Property(strField).IsModified = false;
                }
            }

            IQueryable<InvReturnSupDt> qryDetail = null;
            qryDetail = _context.InvReturnSupDts.Where(
                x => x.CompCode == header.CompCode
                && x.BrnCode == header.BrnCode
                && x.LocCode == header.LocCode
                && x.DocNo == header.DocNo
            );
            _context.InvReturnSupDts.RemoveRange(qryDetail);

            InvReturnSupDt[] arrDetail = null;
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
                    item.UnitBarcode = (item.UnitBarcode == null) ? item.PdId : item.UnitBarcode;
                    item.StockQty = (item.StockQty == null) ? item.ItemQty : item.StockQty;
                }
                await _context.InvReturnSupDts.AddRangeAsync(arrDetail);

                foreach (var item in arrDetail)
                {
                    IQueryable<InvReceiveProdDt> qryDetail_2 = null;
                    qryDetail_2 = context.InvReceiveProdDts.Where(
                        x => x.CompCode == header.CompCode
                        && x.BrnCode == header.BrnCode
                        && x.LocCode == header.LocCode
                        && x.DocNo == header.RefNo
                        && x.PdId == item.PdId);
                    string _strSeqNo = string.Join("", (await qryDetail_2.ToArrayAsync()).Select(x => x.SeqNo).ToArray());
                    string _strDocType = string.Join("", (await qryDetail_2.ToArrayAsync()).Select(x => x.DocType).ToArray());
                    context.InvReceiveProdDts.RemoveRange(qryDetail_2);


                    var Returnqty = new InvReceiveProdDt
                    {
                        CompCode = item.CompCode,
                        BrnCode = item.BrnCode,
                        LocCode = item.LocCode,
                        DocNo = header.RefNo,
                        DocType = _strDocType,
                        SeqNo = Int16.Parse(_strSeqNo),
                        //PdId = item.PdId,
                        ReturnQty = item.ItemQty,
                        ReturnStock = item.StockQty
                    };
                    EntityEntry<InvReceiveProdDt> entDetail = null;
                    entDetail = _context.Attach(Returnqty);
                    entDetail.Property(x => x.ReturnQty).IsModified = true;
                    entDetail.Property(x => x.ReturnStock).IsModified = true;
                    await _unitOfWork.CompleteAsync();
                }
            }
            await _unitOfWork.CompleteAsync();
            return param;
        }

        private async Task adjustHeaderRunningNo(InvReturnSupHd pInvReturnSup)
        {
            string strRunningDocNo = string.Empty;
            IQueryable<MasDocPatternDt> qryDocPattern = null;
            qryDocPattern = from dp in context.MasDocPatterns.AsNoTracking()
                            join dt in context.MasDocPatternDts.AsNoTracking()
                            on dp.DocId equals dt.DocId
                            where _strReturnSup.Equals(dp.DocType)
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
            if (pInvReturnSup.DocDate != null && pInvReturnSup.DocDate.HasValue)
            {
                intDay = pInvReturnSup.DocDate.Value.Day;
                intMonth = pInvReturnSup.DocDate.Value.Month;
                intYear = pInvReturnSup.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<InvReturnSupHd> qrySupplierReturnHD = null;
            qrySupplierReturnHD = context.InvReturnSupHds.Where(
                x => x.BrnCode == pInvReturnSup.BrnCode
                && x.CompCode == pInvReturnSup.CompCode
                && x.LocCode == pInvReturnSup.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                qrySupplierReturnHD = qrySupplierReturnHD.Where(
                    x => intYear.Equals(x.DocDate.Value.Year)
                    && intMonth.Equals(x.DocDate.Value.Month)
                );
            }
            else
            {
                listDocPatternDetail = listDocPatternDetail.OrderBy(x => x.SeqNo).ToList();
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qrySupplierReturnHD = qrySupplierReturnHD.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qrySupplierReturnHD = qrySupplierReturnHD.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qrySupplierReturnHD = qrySupplierReturnHD.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }

            if (await qrySupplierReturnHD.AnyAsync())
            {
                int intMaxRunning = await qrySupplierReturnHD.MaxAsync(x => x.RunNumber.Value);
                int intRowCount = await qrySupplierReturnHD.CountAsync();
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
                            case "Comp": strRunningDocNo += pInvReturnSup.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pInvReturnSup.BrnCode; break;
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

            } while (await context.InvReceiveProdHds.AnyAsync(
                x => x.BrnCode == pInvReturnSup.BrnCode
                && x.CompCode == pInvReturnSup.CompCode
                && x.LocCode == pInvReturnSup.LocCode
                && x.DocNo == strRunningDocNo
            ));
            pInvReturnSup.RunNumber = intLastRunning;
            pInvReturnSup.DocNo = strRunningDocNo;
        }

        private async Task<InvReceiveProdDt[]> getArrayReceiveProdDt(InvReceiveProdHd param)
        {
            if (param == null)
            {
                return null;
            }
            IQueryable<InvReceiveProdDt> qryInvReceiveProdDt = null;
            //qryInvReceiveProdDt = _context.InvReceiveProdDts.Where(
            //    x => x.BrnCode == param.BrnCode
            //    && x.CompCode == param.CompCode
            //    && x.DocNo == param.DocNo
            //    && x.DocType == param.DocType
            //    && x.LocCode == param.LocCode
            //).AsNoTracking();
            qryInvReceiveProdDt = from rh in context.InvReceiveProdHds.AsNoTracking()
                                  join rd in context.InvReceiveProdDts.AsNoTracking()
                                  on rh.DocNo equals rd.DocNo
                                  where rd.ItemQty - rd.ReturnQty > 0
                                  && rh.BrnCode == param.BrnCode
                                  && rh.CompCode == param.CompCode
                                  && rh.LocCode == param.LocCode
                                  && _strActive.Equals(rh.DocStatus)
                                  && rh.DocNo == param.DocNo
                                  select new InvReceiveProdDt()
                                  {
                                      BrnCode = rh.BrnCode,
                                      CompCode = rh.CompCode,
                                      LocCode = rh.LocCode,
                                      DocType = rh.DocType,
                                      DocNo = rh.DocNo,
                                      PdId = rd.PdId,
                                      PdName = rd.PdName,
                                      ItemQty = rd.ItemQty,
                                      ReturnQty = rd.ReturnQty
                                  };
            InvReceiveProdDt[] result = null;
            result = await qryInvReceiveProdDt.ToArrayAsync();
            return result;
        }
        #endregion
    }
}
