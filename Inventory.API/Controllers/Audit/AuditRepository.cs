using Inventory.API.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Controllers.Audit
{
    public class AuditRepository :  IAuditRepository
    {
        private const string _strActive = "Active";
        private const string _strNew = "New";
        private const string _strCancel = "Cancel";
        private const string _strAudit = "Audit";
        private const string _strAdjust = "Adjust";
        private const string _strOilGroupId = "0000";

        private string[] _arrForbidenUpdateField = {
            "DocDate", "Post" , "RunNumber" ,
            "DocPattern" , "Guid" , "CreatedDate" , "CreatedBy"
        };

        private PTMaxstationContext _context = null;

        public AuditRepository(PTMaxstationContext pContext)
        {
            _context = pContext;
        }

        public string SayHello()
        {
            //_context.Set<InvAuditDt>().Where(x=> x.)
            return "Hell World";
        }

        public async Task<ModelAudit> SaveData(ModelAudit pInput)
        {
            if (pInput == null
                || pInput.Header == null
                || pInput.ArrayDetail == null
                || !pInput.ArrayDetail.Any()
            )
            {
                return null;
            }
            InvAuditHd hd = pInput.Header;
            InvAuditDt[] arrDt = pInput.ArrayDetail;
            string strDocStatus = DefaultService.GetString(pInput?.Header?.DocStatus);
            if (_strNew.Equals(strDocStatus))
            {
                MasDocPatternDt[] arrPatternDt = null;
                arrPatternDt = await DefaultService.GetArrayDocPattern(_strAudit, _context);
                await adjustRunningAudit(hd, arrPatternDt);
                hd.CreatedDate = DateTime.Now;
                hd.Guid = Guid.NewGuid();
                hd.DocStatus = _strActive;
                await _context.AddAsync(hd);
                await insertAdjust(pInput);
            }
            else
            {
                hd.UpdatedDate = DateTime.Now;
                EntityEntry<InvAuditHd> entAudit = null;
                entAudit = _context.Update(hd);
                foreach (var item in _arrForbidenUpdateField)
                {
                    entAudit.Property(item).IsModified = false;
                }
                await updateAdjust(pInput);
            }
            IQueryable<InvAuditDt> qryDt = null;
            qryDt = _context.InvAuditDts.Where(
                x => x.DocNo == hd.DocNo
                && x.BrnCode == hd.BrnCode
                && x.CompCode == hd.CompCode
            ).AsNoTracking();
            _context.RemoveRange(qryDt);
            int intSeqNo = 0;
            foreach (var item in arrDt)
            {
                item.DocNo = hd.DocNo;
                item.BrnCode = hd.BrnCode;
                item.CompCode = hd.CompCode;
                item.LocCode = hd.LocCode;
                item.SeqNo = ++intSeqNo;
            }
            await _context.AddRangeAsync(arrDt);
            return pInput;
        }

        private async Task insertAdjust(ModelAudit pInput)
        {
            InvAdjustHd header = null;
            header = DefaultService.ConvertObject<InvAdjustHd>(pInput.Header);

            header.DocType = _strAudit;
            header.RefNo = DefaultService.GetString( pInput.Header?.DocNo);
            header.Guid = Guid.NewGuid();
            //MasDocPatternDt[] arrDocPattern = null;
            //arrDocPattern = await DefaultService.GetArrayDocPattern(_strAdjust, _context);

            //await adjustRunningAdjust(header, arrDocPattern);
            header.DocNo = header.RefNo;
            int intSeq = 0;

            InvAdjustDt[] arrDetail = null;
            arrDetail = pInput.ArrayDetail
            .Where(x=> x.AdjustQty != 0)
            .Select(x =>
            {                
                InvAdjustDt dt = null;
                dt = DefaultService.ConvertObject<InvAdjustDt>(x);
                dt.CompCode = header.CompCode;
                dt.BrnCode = header.BrnCode;
                dt.LocCode = header.LocCode;
                dt.DocType = _strAudit;
                dt.DocNo = header.DocNo;
                //dt.StockQty = dt.ItemQty;
                dt.StockQty = x.AdjustQty;
                dt.ItemQty = dt.StockQty;
                dt.SeqNo = ++intSeq;
                return dt;
            }).ToArray();

            await _context.AddAsync(header);
            await _context.AddRangeAsync(arrDetail);
        }

        private async Task updateAdjust(ModelAudit pInput)
        {
            if(pInput == null)
            {
                return;
            }
            InvAuditHd auditHeader = pInput.Header;
            if(auditHeader == null)
            {
                return;
            }

            IQueryable< InvAdjustHd> qryAdjust = null;
            qryAdjust = _context.InvAdjustHds.Where(
                x => x.RefNo == auditHeader.DocNo 
                && x.CompCode == auditHeader.CompCode
                && x.BrnCode == auditHeader.BrnCode
                && x.LocCode == auditHeader.LocCode
                && _strAudit.Equals(x.DocType)
            );
            InvAdjustHd header = null;
            header = await qryAdjust.FirstOrDefaultAsync();
            if(header != null)
            {
                header.Remark = auditHeader.Remark;
                header.EmpCode = auditHeader.EmpCode;
                header.EmpName = auditHeader.EmpName;
                header.UpdatedBy = auditHeader.UpdatedBy;
                header.UpdatedDate = auditHeader.UpdatedDate;
            }

            IQueryable<InvAdjustDt> qryAdjustDt = null;
            qryAdjustDt = _context.InvAdjustDts.Where(
                x => x.BrnCode == header.BrnCode
                && x.CompCode == header.CompCode
                && x.DocNo == header.DocNo
                && x.DocType == header.DocType
                && x.LocCode == header.LocCode
            ).AsNoTracking();

            _context.RemoveRange(qryAdjustDt);

            int intSeq = 0;

            InvAdjustDt[] arrDetail = null;
            arrDetail = pInput.ArrayDetail
            .Where(x => x.AdjustQty != 0)
            .Select(x =>
            {
                InvAdjustDt dt = null;
                dt = DefaultService.ConvertObject<InvAdjustDt>(x);
                dt.CompCode = header.CompCode;
                dt.BrnCode = header.BrnCode;
                dt.LocCode = header.LocCode;
                dt.DocType = _strAudit;
                dt.DocNo = header.DocNo;
                //dt.StockQty = dt.ItemQty;
                dt.StockQty = x.AdjustQty;
                dt.ItemQty = dt.StockQty;
                dt.SeqNo = ++intSeq;
                
                return dt;
            }).ToArray();

            //await _context.AddAsync(header);
            await _context.AddRangeAsync(arrDetail);
        }

        public InvAuditHd UpdateStatus(InvAuditHd pInput)
        {
            if(pInput == null)
            {
                return null;
            }
            EntityEntry<InvAuditHd> entAudit = null;
            entAudit = _context.Attach(pInput);
            pInput.UpdatedDate = DateTime.Now;
            entAudit.Property(x => x.UpdatedBy).IsModified = true;
            entAudit.Property(x => x.DocStatus).IsModified = true;
            if(_strCancel.Equals(pInput.DocStatus))
            {
                IQueryable<InvAdjustHd> qryAdjust = null;
                qryAdjust = _context.InvAdjustHds.Where(
                    x => x.RefNo == pInput.DocNo
                    && x.CompCode == pInput.CompCode
                    && x.BrnCode == pInput.BrnCode
                    && x.LocCode == pInput.LocCode
                    && _strAudit.Equals(x.DocType)
                );
                InvAdjustHd header = null;
                header = qryAdjust.FirstOrDefault();
                if (header != null)
                {
                    header.DocStatus = _strCancel;
                }
            }
            return pInput;
        }

        public async Task<ModelAuditResult> GetArrayAudit(ModelAuditParam pInput)
        {
            if (pInput == null)
            {
                return null;
            }
            IQueryable<InvAuditHd> qryHeader = null;
            qryHeader = _context.InvAuditHds.Where(
                x => x.CompCode == pInput.CompCode
                && x.BrnCode == pInput.BrnCode
                && x.LocCode == pInput.LocCode
            ).AsNoTracking();
            if (pInput.FromDate.HasValue)
            {
                qryHeader = qryHeader.Where(x => x.DocDate >= pInput.FromDate);
            }
            if (pInput.ToDate.HasValue)
            {
                qryHeader = qryHeader.Where(x => x.DocDate <= pInput.ToDate);
            }
            IQueryable<MasEmployee> qryEmp = null;
            qryEmp = _context.MasEmployees
                .Where(x => qryHeader.Any(y => y.CreatedBy == x.EmpCode))
                .Distinct()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(pInput.Keyword))
            {
                //qryEmp = qryEmp.Where(
                //    x => x.EmpName.Contains(pInput.Keyword)
                //    || x.EmptypeCode.Contains(pInput.Keyword)
                //);
                qryHeader = qryHeader.Where(
                    x => x.DocNo.Contains(pInput.Keyword)
                    || x.EmpCode.Contains(pInput.Keyword)
                    || x.EmpName.Contains(pInput.Keyword)
                    //&& qryEmp.Any(y => y.EmpCode == x.CreatedBy)
                );
            }
            int intTotalItem = 0;
            intTotalItem = await qryHeader.CountAsync();

            if (pInput.Page > 0 && pInput.ItemsPerPage > 0)
            {
                int intSkip = (pInput.Page - 1) * pInput.ItemsPerPage;
                qryHeader = qryHeader
                    .Skip(intSkip)
                    .Take(pInput.ItemsPerPage);
            }
            qryHeader = qryHeader.OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo);
            InvAuditHd[] arrayHeader = null;
            arrayHeader = await qryHeader.ToArrayAsync();

            MasEmployee[] arrEmployee = null;
            arrEmployee = await qryEmp.ToArrayAsync();

            ModelAuditResult result = new ModelAuditResult();
            result.ArrayEmployee = arrEmployee;
            result.ArrayHeader = arrayHeader;
            result.TotalItems = intTotalItem;
            return result;
        }

        public async Task<ModelAudit> GetAudit(string pStrGuid)
        {
            pStrGuid = DefaultService.GetString(pStrGuid);
            if (0.Equals(pStrGuid.Length))
            {
                return null;
            }
            Guid guid = Guid.Parse(pStrGuid);
            InvAuditHd hd = null;
            hd = await _context.InvAuditHds.FirstOrDefaultAsync(x => guid.Equals(x.Guid));

            if (hd == null)
            {
                return null;
            }
            IQueryable<InvAuditDt> qryDetail = null;
            qryDetail = _context.InvAuditDts.Where(
                x => x.BrnCode == hd.BrnCode
                && x.CompCode == hd.CompCode
                && x.DocNo == hd.DocNo
                && x.LocCode == hd.LocCode
            ).AsNoTracking();

            InvAuditDt[] arrDetail = null;
            arrDetail = await qryDetail.ToArrayAsync();
            ModelAudit result = null;
            result = new ModelAudit();
            result.ArrayDetail = arrDetail;
            result.Header = hd;
            return result;
        }

        public async Task<int> GetAuditCount(string compCode,string brnCode, int pIntAuditYear)
        {

            IQueryable<InvAuditHd> qryAudit = null;
            qryAudit = _context.InvAuditHds.Where(x =>x.CompCode == compCode && x.BrnCode == brnCode && x.DocDate.Value.Year == pIntAuditYear);
            int result = ( await qryAudit.MaxAsync(x => x.AuditSeq) ?? 0 ) +1;
            //int result = await qryAudit.CountAsync() + 1;
            return result;
        }

        public async Task<ModelAuditProduct> GetAuditProduct(ModelAuditProductParam pInput )
        {
            if(pInput == null)
            {
                return null;
            }
            MasProduct[] arrProduct = null;
            InvStockDaily[] arrStockDaily = null;
            IQueryable <InvStockDaily> qryStockDaily = null;
            qryStockDaily = _context.InvStockDailies.Where(
                x => x.CompCode == pInput.CompCode
                && x.BrnCode == pInput.BrnCode
                && x.LocCode == pInput.LocCode
                && x.StockDate ==  pInput.StockDate.Value.AddDays(-1)
                && x.Remain > 0
            ).AsNoTracking();

            //bool haveRow = false;
            //haveRow = await qryStockDaily.AnyAsync();

            //DateTime? datMax = null;
            //if (haveRow)
            //{
            //    datMax = await qryStockDaily.MaxAsync(x => x.StockDate);
            //    if (datMax.HasValue)
            //    {
            //        qryStockDaily = qryStockDaily.Where(x => x.StockDate == datMax);
            //    }
            //}

            arrStockDaily = await qryStockDaily.ToArrayAsync();
            if(arrStockDaily != null && arrStockDaily.Any())
            {
                string[] arrProductId = null;
                arrProductId = arrStockDaily
                    .Select(x => DefaultService.GetString(x.PdId))
                    .Where(x=> x.Length > 0)
                    .Distinct()
                    .ToArray();
                IQueryable<MasProduct> qryProduct = null;
                qryProduct = _context.MasProducts.Where(
                    x => !_strOilGroupId.Equals(x.GroupId)
                    && arrProductId.Contains(x.PdId)
                    && _strActive.Equals(x.PdStatus)
                ).AsNoTracking();
                arrProduct = await qryProduct.ToArrayAsync();
            }

            ModelAuditProduct result = null;
            result = new ModelAuditProduct()
            {
                ArrayProduct = arrProduct,
                ArrayStockDaily = arrStockDaily
            };
            return result;
        }

        public async Task<MasProduct[]> GetArrayProduct(ModelAuditProductParam pInput)
        {
            if (pInput == null)
            {
                return null;
            }            
            IQueryable<InvStockDaily> qryStockDaily = null;
            qryStockDaily = _context.InvStockDailies.Where(
                x => x.CompCode == pInput.CompCode
                && x.BrnCode == pInput.BrnCode
                && x.LocCode == pInput.LocCode
                && x.StockDate == pInput.StockDate.Value.AddDays(-1)
                && x.Remain > 0
            ).AsNoTracking();

            //DateTime? datMax = null;
            //datMax = await qryStockDaily.MaxAsync(x => x.StockDate);
            //if (datMax.HasValue)
            //{
            //    qryStockDaily = qryStockDaily.Where(x => x.StockDate == datMax);
            //}

            //IQueryable<MasProduct> qryProduct = null;
            //qryProduct = _context.MasProducts.Where(
            //    x => !qryStockDaily.Any(y => y.PdId == x.PdId)
            //        && !_strOilGroupId.Equals(x.GroupId)
            //        && _strActive.Equals(x.PdStatus)
            //).AsNoTracking();

            IQueryable<MasProduct> qryProduct = null;
            qryProduct = _context.MasProducts.Where(
                x => !_strOilGroupId.Equals(x.GroupId)
                //&& qryStockDaily.Contains(x.PdId)
                && _strActive.Equals(x.PdStatus)
            ).AsNoTracking();

            MasProduct[] result = null;
            result = await qryProduct.ToArrayAsync();
            return result;
        }

        public async Task<MasEmployee> GetEmployee(string pStrEmpCode)
        {
            pStrEmpCode = DefaultService.GetString(pStrEmpCode);
            if (0.Equals(pStrEmpCode.Length))
            {
                return null;
            }
            IQueryable<MasEmployee> qryEmp = null;
            qryEmp = _context.MasEmployees.Where(
                x => pStrEmpCode.Equals(x.EmpCode)
                && _strActive.Equals(x.WorkStatus)
            ).AsNoTracking();
            MasEmployee result = null;
            result = await qryEmp.FirstOrDefaultAsync();
            return result;
        }

        private async Task adjustRunningAudit(InvAuditHd pInput, MasDocPatternDt[] pArrDocPatternDt)
        {

            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pInput.DocDate.HasValue)
            {
                intDay = pInput.DocDate.Value.Day;
                intMonth = pInput.DocDate.Value.Month;
                intYear = pInput.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            string strCompCode = DefaultService.GetString(pInput.CompCode);
            string strBrnCode = DefaultService.GetString(pInput.BrnCode);
            string strLocCode = DefaultService.GetString(pInput.LocCode);

            IQueryable<InvAuditHd> qryAudit = null;
            qryAudit = _context.InvAuditHds.Where(
                x => strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            ).AsNoTracking();

            Func<string, Task<bool>> funcWhileAnyAsync = null;
            funcWhileAnyAsync = async y =>
            {
                bool result = false;
                result = await _context.InvAuditHds.AnyAsync(
                    x => strCompCode.Equals(x.CompCode)
                    && strBrnCode.Equals(x.BrnCode)
                    && strLocCode.Equals(x.LocCode)
                    && y.Equals(x.DocNo)
                );
                return result;
            };

            DefaultService.GetRunningInput<InvAuditHd> input = null;
            input = new DefaultService.GetRunningInput<InvAuditHd>()
            {
                ArrayDocPattern = pArrDocPatternDt,
                BrnCode = strBrnCode,
                CompCode = strCompCode,
                DocDate = pInput.DocDate,
                //FuncFilterDay = x => intDay.Equals(pInput.DocDate.Value.Day),
                //FuncFilterMonth = x => intMonth.Equals(pInput.DocDate.Value.Month),
                //FuncFilterYear = x => intYear.Equals(pInput.DocDate.Value.Year),
                FuncFindMax = x => x.RunNumber.Value,
                IQueryable = qryAudit,
                FuncWhileAnyAsync = funcWhileAnyAsync
            };
            input.FuncFilterDay = x => x.DocDate.HasValue && x.DocDate.Value.Day == intDay;
            input.FuncFilterMonth = x => x.DocDate.HasValue && x.DocDate.Value.Month == intMonth;
            input.FuncFilterYear = x => x.DocDate.HasValue && x.DocDate.Value.Year == intYear;
            DefaultService.GetRunningOutPut running = null;
            running = await DefaultService.GetRunning<InvAuditHd>(input);

            pInput.DocNo = running.DocNo;
            pInput.RunNumber = running.RunningNumber;
            pInput.DocPattern = running.DocPattern;
            //return result;
        }

        private async Task adjustRunningAdjust(InvAdjustHd pInput, MasDocPatternDt[] pArrDocPatternDt)
        {

            int intDay = 0;
            int intMonth = 0;
            int intYear = 0;
            if (pInput.DocDate.HasValue)
            {
                intDay = pInput.DocDate.Value.Day;
                intMonth = pInput.DocDate.Value.Month;
                intYear = pInput.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            string strCompCode = DefaultService.GetString(pInput.CompCode);
            string strBrnCode = DefaultService.GetString(pInput.BrnCode);
            string strLocCode = DefaultService.GetString(pInput.LocCode);

            IQueryable<InvAdjustHd> qryAudit = null;
            qryAudit = _context.InvAdjustHds.Where(
                x => strCompCode.Equals(x.CompCode)
                && strBrnCode.Equals(x.BrnCode)
                && strLocCode.Equals(x.LocCode)
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            ).AsNoTracking();

            Func<string, Task<bool>> funcWhileAnyAsync = null;
            funcWhileAnyAsync = async y =>
            {
                bool result = false;
                result = await _context.InvAuditHds.AnyAsync(
                    x => strCompCode.Equals(x.CompCode)
                    && strBrnCode.Equals(x.BrnCode)
                    && strLocCode.Equals(x.LocCode)
                    && y.Equals(x.DocNo)
                );
                return result;
            };

            DefaultService.GetRunningInput<InvAdjustHd> input = null;
            input = new DefaultService.GetRunningInput<InvAdjustHd>()
            {
                ArrayDocPattern = pArrDocPatternDt,
                BrnCode = strBrnCode,
                CompCode = strCompCode,
                DocDate = pInput.DocDate,
                FuncFilterDay = x => intDay.Equals(pInput.DocDate.Value.Day),
                FuncFilterMonth = x => intMonth.Equals(pInput.DocDate.Value.Month),
                FuncFilterYear = x => intYear.Equals(pInput.DocDate.Value.Year),
                FuncFindMax = x => x.RunNumber.Value,
                IQueryable = qryAudit,
                FuncWhileAnyAsync = funcWhileAnyAsync
            };

            DefaultService.GetRunningOutPut running = null;
            running = await DefaultService.GetRunning<InvAdjustHd>(input);

            pInput.DocNo = running.DocNo;
            pInput.RunNumber = running.RunningNumber;
            pInput.DocPattern = running.DocPattern;
            //return result;
        }
        private void test<T>() where T : class
        {
            _context.Set<T>().FromSqlRaw("");
        }
    }
}
