using AutoMapper;
using Inventory.API.Domain.Repositories;
using Inventory.API.Services;
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
    public class ReturnOilController : BaseController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ReturnOilController));
        private const string _appJson = "application/json";
        private readonly IMapper _mapper;

        private PTMaxstationContext _context = null;
        private IUnitOfWork _unitOfWork = null;

        public ReturnOilController(PTMaxstationContext pContex, IMapper mapper, IUnitOfWork pUnitOfWork):base(pContex)
        {
            _context = pContex;
            _unitOfWork = pUnitOfWork;
            _mapper = mapper;
        }

        #region - Controller -

        [HttpGet("GetArrayBranch")]
        public async Task<IActionResult> GetArrayBranch()
        {
            return await doAction("GetArrayBranch", async () => await getArrayBranch());            
        }

        [HttpGet("GetArrayOilTerminal")]
        public async Task<IActionResult> GetArrayOilTerminal(string CompCode)
        {
            return await doAction("GetArrayOilTerminal", async () => await getArrayOilTerminal(CompCode));
        }

        [HttpGet("GetArrayReason")]
        public async Task<IActionResult> GetArrayReason()
        {
            return await doAction("GetArrayReason", async () => await getArrayReason());
        }

        [HttpPost("GetArrayReturnOilHeader")]
        public async Task<IActionResult> GetArrayReturnOilHeader(ModelReturnOilParam param)
        {
            return await doAction("GetArrayReturnOilHeader", async () => await getArrayReturnOilHeader(param));
        }
        /*
        [HttpGet("GetArrayPoHeader/{pStrBrnCode}")]
        public async Task<IActionResult> GetArrayPoHeader(string pStrBrnCode)
        {
            return await doAction("GetArrayPoHeader", async () => await getArrayPoHeader(pStrBrnCode));

        }

        [HttpGet("GetArrayPoItem/{pStrPoNumber}")]
        public async Task<IActionResult> GetArrayPoItem(string pStrPoNumber)
        {
            return await doAction("GetArrayPoItem", async () => await getArrayPoItem(pStrPoNumber));
        }
        */
        [HttpPost("GetArrayReceiveProduct")]
        public async Task<IActionResult> GetArrayReceiveProduct(InvReturnOilHd param)
        {
            return await doAction("GetArrayReceiveProduct", async () => await getArrayReceiveProduct(param));
        }

        [HttpPost("GetArrayReceiveProdDt")]
        public async Task<IActionResult> GetArrayReceiveProdDt(InvReceiveProdHd param)
        {
            return await doAction("GetArrayReceiveProdDt", async () => await getArrayReceiveProdDt(param));
        }

        [HttpGet("GetArrayProductWithoutPO")]
        public async Task<IActionResult> GetArrayProductWithoutPO()
        {
            return await doAction("GetArrayProductWithoutPO", async () => await getArrayProductWithoutPO());
        }

        [HttpGet("GetEmployee/{pStrEmpCode}")]
        public async Task<IActionResult> GetEmployee(string pStrEmpCode)
        {
            return await doAction("GetEmployee", async () => await getEmployee(pStrEmpCode));            
        }

        [HttpGet("GetReturnOil/{pStrGuid}")]
        public async Task<IActionResult> GetReturnOil(string pStrGuid)
        {
            return await doAction("GetReturnOil", async () => await getReturnOil(pStrGuid));
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(InvReturnOilHd pHeader)
        {
            return await doAction("UpdateStatus", async () => await updateStatus(pHeader));            
        }

        [HttpPost("SaveReturnOil")]
        public async Task<IActionResult> SaveReturnOil(ModelReturnOil param)
        {
            return await doAction("SaveReturnOil", async () => await saveReturnOil(param));
        }
        [HttpPost("GetArrayProduct")]
        public async Task<IActionResult> GetArrayProduct(string[] pArrProductId)
        {
            return await doAction("GetArrayProduct", async () => await getArrayProduct(pArrProductId));
        }
        #endregion

        #region - Model -
        public class ModelGetArrayPoHeaderParam
        {
            public string CompCode { get; set; }
            public string BrnCode { get; set; }
            public string LocCode { get; set; }
        }

        public class ModelGetArrayPoItemResult
        {
            public InfPoItem[] ArrPoItem { get; set; }
            public MasProduct[] ArrayProduct { get; set; }
            public MasUnit[] ArrayUnit { get; set; }
        }

        public class ModelReturnOil
        {
            public InvReturnOilHd Header { get; set; }
            public InvReturnOilDt[] ArrayDetail { get; set; }
        }
        public class ModelReturnOilParam
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
        public class ModelReturnOilResult
        {
            public InvReturnOilHd[] ArrayReturnOilHeader { get; set; }
            public MasEmployee[] ArrayEmployee { get; set; }
            public int TotalItems { get; set; }
        }
        #endregion

        #region - Function -

        private const string _strReturnOil = "ReturnOil";
        private const string _strActive = "Active";
        private const string _strNew = "New";
        private const string _strPoDelete = "L";
        private const string _strOil = "Oil";
        private async Task< IActionResult> doAction<T>( string pStrFunctionName , Func<Task<T>> pFunc)
        {
            try
            {
                T result;
                result = await pFunc();
                _log.Info(pStrFunctionName + " Complete");
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
            result = Content(strJson, _appJson);
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
            if(pException == null)
            {
                return string.Empty;
            }
            string result = string.Empty;
            result = pException.StackTrace;
            while(pException.InnerException != null)
            {
                pException = pException.InnerException;
            }
            result = pException.Message + Environment.NewLine + result;
            return result;
        }
        private async Task<MasBranch[]> getArrayBranch()
        {
            IQueryable<MasBranch> qryBranch = null;
            qryBranch = _context.MasBranches
                .Where(x=> _strActive.Equals(x.BrnStatus))
                .AsNoTracking();

            MasBranch[] result = null;
            result = await qryBranch.ToArrayAsync();
            return result;
        }
        private async Task<MasWarehouse[]> getArrayOilTerminal(string CompCode)
        {
            IQueryable<MasWarehouse> qryWareHouse = null;
            qryWareHouse = _context.MasWarehouses
                .Where(x => x.CompCode == CompCode && _strActive.Equals(x.WhStatus))
                .AsNoTracking();


            MasWarehouse[] result = null;
            result = await qryWareHouse.ToArrayAsync();
            return result;
        }
        private async Task< MasReason[]> getArrayReason()
        {
            IQueryable<MasReason> qryReason = null;
            qryReason = _context.MasReasons.Where(
                x => _strReturnOil.Equals(x.ReasonGroup)
                && _strActive.Equals(x.ReasonStatus)
            ).AsNoTracking();
            MasReason[] result = null;
            result = await qryReason.ToArrayAsync();
            return result;
        }
        private async Task<ModelReturnOilResult> getArrayReturnOilHeader(ModelReturnOilParam param)
        {
            if (param == null)
            {
                return null;
            }
            IQueryable<InvReturnOilHd> qryReturnOil = null;
            qryReturnOil = _context.InvReturnOilHds.Where(
                x => x.CompCode == param.CompCode
                && x.BrnCode == param.BrnCode
                && x.LocCode == param.LocCode
            ).AsNoTracking();
            if (param.FromDate.HasValue)
            {
                qryReturnOil = qryReturnOil.Where(x => x.DocDate >= param.FromDate);
            }
            if (param.ToDate.HasValue)
            {
                qryReturnOil = qryReturnOil.Where(x => x.DocDate <= param.ToDate);
            }
            if (!string.IsNullOrWhiteSpace(param.Keyword))
            {
                qryReturnOil = qryReturnOil.Where(
                    x => x.DocNo.Contains(param.Keyword)
                    || x.CreatedBy.Contains(param.Keyword)
                );
            }
            int intTotalItem =0 ;
            intTotalItem = await qryReturnOil.CountAsync();
            qryReturnOil = qryReturnOil.OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo);
            if (param.Page > 0 && param.ItemsPerPage > 0)
            {
                qryReturnOil = qryReturnOil
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip((param.Page - 1) * param.ItemsPerPage)
                    .Take(param.ItemsPerPage);
            }
            
            InvReturnOilHd[] arrayReturnOilHeader = null;
            arrayReturnOilHeader = await qryReturnOil.ToArrayAsync();

            if (arrayReturnOilHeader == null || !arrayReturnOilHeader.Any())
            {
                return null;
            }
            string[] arrCreateBy = null;
            arrCreateBy = arrayReturnOilHeader
                .Select(x => (x.CreatedBy ?? string.Empty).Trim())
                .Where(x => x.Length > 0)
                .ToArray();

            string[] arrUpdateBy = null;
            arrUpdateBy = arrayReturnOilHeader
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
                qryCustomer = _context.MasEmployees
                    .Where(x => arrActionBy.Contains(x.EmpCode))
                    .AsNoTracking();
                arrEmployee = await qryCustomer.ToArrayAsync();
            }
            ModelReturnOilResult result = null;
            result = new ModelReturnOilResult()
            {
                ArrayEmployee = arrEmployee,
                ArrayReturnOilHeader = arrayReturnOilHeader,
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
            string strBrnCode = DefaultService.EncodeSqlString(param.BrnCode);
            string strCompCode = DefaultService.EncodeSqlString(param.CompCode);
            string strLocCode = DefaultService.EncodeSqlString(param.LocCode);
            string strSql = $@"select distinct rcp.*
from INV_RECEIVE_PROD_HD(nolock)rcp
cross apply(
	select PD_ID ,STOCK_QTY ITEM_QTY , DOC_NO
	from INV_RECEIVE_PROD_DT(nolock)
	where DOC_NO = rcp.DOC_NO
		and DOC_TYPE = rcp.DOC_TYPE
		and COMP_CODE = rcp.COMP_CODE
		and BRN_CODE = rcp.BRN_CODE
) rcp2
outer apply(
	select SUM( dt.ITEM_QTY)qty from INV_RETURN_OIL_HD(nolock)hd
	cross apply(
		select  ITEM_QTY 
		from INV_RETURN_OIL_DT(nolock)
		where DOC_NO = hd.DOC_NO
			and COMP_CODE = hd.COMP_CODE
			and BRN_CODE = hd.BRN_CODE
			and LOC_CODE = hd.LOC_CODE
			and PD_ID = rcp2.PD_ID
	) dt
	where REF_NO = rcp2.DOC_NO
		and DOC_STATUS <> 'Cancel'
)rto
where PO_TYPE_ID in ( select PO_TYPE_ID from INF_PO_TYPE(nolock)where PO_TYPE_DESC = 'Oil' )
	and ISNULL( rto.qty,0) < rcp2.ITEM_QTY
	and rcp.DOC_STATUS <> 'Cancel'
	and rcp.BRN_CODE = '{strBrnCode}'
	and rcp.COMP_CODE = '{strCompCode}'
	and rcp.LOC_CODE = '{strLocCode}'";
            string strCon = _context.Database.GetConnectionString();
            var result = await DefaultService.GetEntityFromSql<InvReceiveProdHd[]>(strCon, strSql);
            return result;
        }
        private async Task<InvReceiveProdHd[]> getArrayReceiveProductOld(InvReturnOilHd param)
        {
            if(param == null)
            {
                return null;
            }
            IQueryable<InfPoType> qryPoType = null;
            qryPoType = _context.InfPoTypes.Where(
                x => _strOil.Equals(x.PoTypeDesc)
            ).AsNoTracking();

            IQueryable<InvReceiveProdHd> qryReceiveProd = null;
            qryReceiveProd = _context.InvReceiveProdHds.Where(
                x => qryPoType.Any(y=> y.PoTypeId == x.PoTypeId)
                //_strOil.Equals(x.DocType)
                && _strActive.Equals(x.DocStatus)
                && x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.LocCode == param.LocCode
            ).AsNoTracking();
            
            InvReceiveProdHd[] result = null;
            result = await qryReceiveProd.ToArrayAsync();

            return result;
        }

        private async Task<InvReceiveProdDt[]> getArrayReceiveProdDt(InvReceiveProdHd param)
        {
            if(param == null)
            {
                return null;
            }
            string strSql = @$"select distinct rcp2.*
from INV_RECEIVE_PROD_HD(nolock)rcp
cross apply(
	select *
	from INV_RECEIVE_PROD_DT(nolock)
	where DOC_NO = rcp.DOC_NO
		and DOC_TYPE = rcp.DOC_TYPE
		and COMP_CODE = rcp.COMP_CODE
		and BRN_CODE = rcp.BRN_CODE
) rcp2
outer apply(
	select SUM( dt.ITEM_QTY)qty from INV_RETURN_OIL_HD(nolock)hd
	cross apply(
		select  ITEM_QTY 
		from INV_RETURN_OIL_DT(nolock)
		where DOC_NO = hd.DOC_NO
			and COMP_CODE = hd.COMP_CODE
			and BRN_CODE = hd.BRN_CODE
			and LOC_CODE = hd.LOC_CODE
			and PD_ID = rcp2.PD_ID
	) dt
	where REF_NO = rcp2.DOC_NO
		and DOC_STATUS <> 'Cancel'
)rto
where PO_TYPE_ID in ( select PO_TYPE_ID from INF_PO_TYPE(nolock)where PO_TYPE_DESC = 'Oil' )
	and ISNULL( rto.qty,0) < rcp2.ITEM_QTY
	and rcp.DOC_STATUS <> 'Cancel'
	and rcp.BRN_CODE = '{DefaultService.EncodeSqlString(param.BrnCode)}'
	and rcp.COMP_CODE = '{DefaultService.EncodeSqlString(param.CompCode)}'
	and rcp.LOC_CODE = '{DefaultService.EncodeSqlString(param.LocCode)}'
	and rcp.PO_NO = '{DefaultService.EncodeSqlString(param.PoNo)}'";
            var result = await DefaultService.GetEntityFromSql<InvReceiveProdDt[]>(_context, strSql);
            return result;
        }
        /*
         private async Task<InvReceiveProdDt[]> getArrayReceiveProdDt(InvReceiveProdHd param)
        {
            if(param == null)
            {
                return null;
            }
            IQueryable<InvReceiveProdDt> qryInvReceiveProdDt = null;
            qryInvReceiveProdDt = _context.InvReceiveProdDts.Where(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.DocNo == param.DocNo
                && x.DocType == param.DocType
                && x.LocCode == param.LocCode
            ).AsNoTracking();
            InvReceiveProdDt[] result = null;
            result = await qryInvReceiveProdDt.ToArrayAsync();
            return result;
        }
         */

        private async Task<InvReceiveProdDt[]> getArrayProductWithoutPO()
        {
            var groupProductOil = await context.MasProducts.Where(x => x.GroupId == "0000").AsNoTracking().ToListAsync();
            var result = _mapper.Map<MasProduct[], InvReceiveProdDt[]>(groupProductOil.ToArray());
            return result;
        }

        private async Task<MasProduct[]> getArrayProduct(string[] pArrProductId)
        {
            if(pArrProductId == null || !pArrProductId.Any())
            {
                return null;
            }
            IQueryable<MasProduct> qryProd = null;
            qryProd = _context.MasProducts
                .Where(x => pArrProductId.Contains(x.PdId))
                .AsNoTracking();
            MasProduct[] result = null;
            result = await qryProd.ToArrayAsync();
            return result;
        }

        /*
        private async Task<InfPoHeader[]> getArrayPoHeader(string pStrBrnCode)
        {
            pStrBrnCode = (pStrBrnCode ?? string.Empty).Trim();
            if (0.Equals(pStrBrnCode.Length))
            {
                return null;
            }
            IQueryable<InfPoItem> qryPoItem = null;
            qryPoItem = _context.InfPoItems.Where(
                x => x.Plant == pStrBrnCode
                && !_strPoDelete.Equals( x.DeleteInd)
            ).AsNoTracking();

            IQueryable<InfPoHeader> qryPoHeader = null;
            qryPoHeader = _context.InfPoHeaders.Where(
                x=> !_strPoDelete.Equals(x.DeleteInd)
                && string.IsNullOrEmpty(x.ReceiveStatus)
                && qryPoItem.Any(y=> y.PoNumber == x.PoNumber)
            ).AsNoTracking();

            
            InfPoHeader[] result = null;
            result = await qryPoHeader.ToArrayAsync();
            return result;
        }
        private async Task<ModelGetArrayPoItemResult> getArrayPoItem(string pStrPoNumber)
        {
            pStrPoNumber = (pStrPoNumber ?? string.Empty).Trim();
            if (0.Equals(pStrPoNumber.Length))
            {
                return null;
            }
            IQueryable<InfPoItem> qryPoItem = null;
            qryPoItem = _context.InfPoItems
                .Where(x => pStrPoNumber.Equals(x.PoNumber))
                .AsNoTracking();

            InfPoItem[] arrPoItem = null;
            arrPoItem = await qryPoItem.ToArrayAsync();

            ModelGetArrayPoItemResult result = null;
            result = new ModelGetArrayPoItemResult()
            {
                ArrPoItem = arrPoItem,
                ArrayProduct = await getArrayProductUnit(arrPoItem),
                ArrayUnit = await getArrayUnit(arrPoItem)
               
            };            
            return result;
        }
        
        private async Task<MasProduct[]> getArrayProductUnit(InfPoItem[] pArrPoItem)
        {
            if(pArrPoItem == null || !pArrPoItem.Any())
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
            qryProduct = _context.MasProducts
                .Where(x=> arrMaterial.Contains(x.PdId))
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
            qryUnit = _context.MasUnits
                .Where(x => arrUnit.Contains(x.MapUnitId))
                .AsNoTracking();

            MasUnit[] result = null;
            result = await qryUnit.ToArrayAsync();

            return result;
        }

        */
        private async Task<MasEmployee> getEmployee(string pStrEmpCode)
        {
            pStrEmpCode = (pStrEmpCode ?? string.Empty).Trim();
            if (0.Equals(pStrEmpCode.Length))
            {
                return null;
            }
            IQueryable<MasEmployee> qryEmp = null;
            qryEmp = _context.MasEmployees
                .Where(x => pStrEmpCode.Equals(x.EmpCode))
                .AsNoTracking();

            MasEmployee result = null;
            result = await qryEmp.FirstOrDefaultAsync();
            return result;
        }
        private async Task<ModelReturnOil> getReturnOil(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();

            if(0.Equals(pStrGuid.Length))
            {
                return null;
            }

            Guid guidHeader;

            if(!Guid.TryParse(pStrGuid , out guidHeader))
            {
                return null;
            }


            IQueryable<InvReturnOilHd> qryHeader = null;
            qryHeader = _context.InvReturnOilHds
                .Where(x => guidHeader.Equals(x.Guid))
                .AsNoTracking();

            InvReturnOilHd header = null;
            header = await qryHeader.FirstOrDefaultAsync();

            if(header == null)
            {
                return null;
            }

            IQueryable<InvReturnOilDt> qryDetail = null;
            qryDetail = _context.InvReturnOilDts.Where(
                x=> x.CompCode == header.CompCode
                && x.BrnCode == header.BrnCode
                && x.LocCode == header.LocCode
                && x.DocNo == header.DocNo
            ).AsNoTracking();

            InvReturnOilDt[] arrDetail = null;
            arrDetail = await qryDetail.ToArrayAsync();
            ModelReturnOil result = null;
            result = new ModelReturnOil()
            {
                Header = header,
                ArrayDetail = arrDetail
            };

            return result;
        }
        private async Task<InvReturnOilHd> updateStatus(InvReturnOilHd pHeader)
        {
            if(pHeader == null)
            {
                return null;
            }
            EntityEntry<InvReturnOilHd> entHeader = null;
            entHeader =_context.Attach(pHeader);
            pHeader.UpdatedDate = DateTime.Now;
            entHeader.Property(x => x.DocStatus).IsModified = true;
            await _unitOfWork.CompleteAsync();
            return pHeader;
        }
        private async Task<ModelReturnOil> saveReturnOil(ModelReturnOil param)
        {
            if(param == null)
            {
                return null;
            }
            InvReturnOilHd header = null;
            header = param.Header;
            if(header == null)
            {
                return null;
            }
            if (_strNew.Equals(header.DocStatus))
            {
                header.CreatedDate = DateTime.Now;
                header.DocStatus = _strActive;
                header.Guid = Guid.NewGuid();
                await adjustHeaderRunningNo(header);
                await _context.InvReturnOilHds.AddAsync(header);
            }
            else
            {
                header.UpdatedDate = DateTime.Now;
                EntityEntry<InvReturnOilHd> entHeader = null;
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

            IQueryable<InvReturnOilDt> qryDetail = null;
            qryDetail = _context.InvReturnOilDts.Where(
                x => x.CompCode == header.CompCode
                && x.BrnCode == header.BrnCode
                && x.LocCode == header.LocCode
                && x.DocNo == header.DocNo
            );
            _context.InvReturnOilDts.RemoveRange(qryDetail);

            var branch = this.context.MasBranches.ToList();
            InvReturnOilDt[] arrDetail = null;
            arrDetail = param.ArrayDetail;
            if(arrDetail != null && arrDetail.Any())
            {
                int intSeqNo = 0;
                foreach (var item in arrDetail)
                {
                    item.SeqNo = ++intSeqNo;
                    item.CompCode = header.CompCode;
                    item.BrnCode = header.BrnCode;
                    item.LocCode = header.LocCode;
                    item.DocNo = header.DocNo;
                    item.BrnNameFrom = branch.Any(x => x.BrnCode == item.BrnCodeFrom) ? branch.FirstOrDefault(x => x.BrnCode == item.BrnCodeFrom).BrnName : "";
                    item.UnitBarcode = (item.UnitBarcode == null) ? item.PdId : item.UnitBarcode;
                    item.StockQty = (item.StockQty == null) ? item.ItemQty : item.StockQty;
                }
                await _context.InvReturnOilDts.AddRangeAsync(arrDetail);
            }
            await _unitOfWork.CompleteAsync();
            return param;
        }
        private async Task adjustHeaderRunningNo(InvReturnOilHd pInvReturnOil)
        {
            string strRunningDocNo = string.Empty;
            IQueryable<MasDocPatternDt> qryDocPattern = null;
            qryDocPattern = from dp in _context.MasDocPatterns.AsNoTracking()
                            join dt in _context.MasDocPatternDts.AsNoTracking()
                            on dp.DocId equals dt.DocId
                            where _strReturnOil.Equals( dp.DocType)
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
            if (pInvReturnOil.DocDate != null && pInvReturnOil.DocDate.HasValue)
            {
                intDay = pInvReturnOil.DocDate.Value.Day;
                intMonth = pInvReturnOil.DocDate.Value.Month;
                intYear = pInvReturnOil.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<InvReturnOilHd> qryReturnOilHD = null;
            qryReturnOilHD = _context.InvReturnOilHds.Where(
                x => x.BrnCode == pInvReturnOil.BrnCode
                && x.CompCode == pInvReturnOil.CompCode
                && x.LocCode == pInvReturnOil.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                qryReturnOilHD = qryReturnOilHD.Where(
                    x => intYear.Equals(x.DocDate.Value.Year)
                    && intMonth.Equals(x.DocDate.Value.Month)
                );
            }
            else
            {
                listDocPatternDetail = listDocPatternDetail.OrderBy(x => x.SeqNo).ToList();
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryReturnOilHD = qryReturnOilHD.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryReturnOilHD = qryReturnOilHD.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryReturnOilHD = qryReturnOilHD.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }

            if (await qryReturnOilHD.AnyAsync())
            {
                int intMaxRunning = await qryReturnOilHD.MaxAsync(x => x.RunNumber.Value);
                int intRowCount = await qryReturnOilHD.CountAsync();
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
                            case "Comp": strRunningDocNo += pInvReturnOil.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pInvReturnOil.BrnCode; break;
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

            } while (await _context.InvReceiveProdHds.AnyAsync(
                x => x.BrnCode == pInvReturnOil.BrnCode
                && x.CompCode == pInvReturnOil.CompCode
                && x.LocCode == pInvReturnOil.LocCode
                && x.DocNo == strRunningDocNo
            ));
            pInvReturnOil.RunNumber = intLastRunning;
            pInvReturnOil.DocNo = strRunningDocNo;
        }

        #endregion
    }
}
