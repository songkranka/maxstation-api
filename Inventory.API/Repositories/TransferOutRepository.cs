using Abp.Linq.Expressions;
using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using Inventory.API.Resources.Request;
using Inventory.API.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class TransferOutRepository : SqlDataAccessHelper, ITransferOutRepository
    {
        private string[] _arrForbidenUpdateField = {
            "DocDate", "Post" , "RunNumber" ,
            "DocPattern" , "Guid" , "CreatedDate" , "CreatedBy"
        };
        public TransferOutRepository(PTMaxstationContext context) : base(context){}

        public async Task<ResponseData<List<ModelTransferOutHeader>>>
        GetTransferOutList(TransferOutQueryResource param)
        {
            var result = new ResponseData<List<ModelTransferOutHeader>>();
            if(string.IsNullOrEmpty( param.Guid))
            {
                var qry = this.context.InvTranoutHds.AsNoTracking().Where(
                    x => x.LocCode == param.LocCode
                    && x.BrnCode == param.BrnCode
                    && x.CompCode == param.CompCode
                );
                bool haveFromDate = param.FromDate != null && param.FromDate.HasValue;
                bool haveToDate = param.ToDate != null && param.ToDate.HasValue;
                if (haveFromDate || haveToDate)
                {
                    qry = qry.Where(x => x.DocDate != null && x.DocDate.HasValue);
                    if (haveFromDate)
                    {
                        qry = qry.Where(x => x.DocDate >= param.FromDate);
                    }
                    if (haveToDate)
                    {
                        qry = qry.Where(x => x.DocDate <= param.ToDate);
                    }
                }
                string strKeyWord = string.Empty;
                strKeyWord = DefaultService.GetString(param.Keyword);
                if (!0.Equals(strKeyWord.Length))
                {
                    ExpressionStarter<InvTranoutHd> expHeader = null;
                    expHeader = PredicateBuilder.New<InvTranoutHd>(
                        x => x.DocNo.Contains(strKeyWord)
                        || x.BrnCodeTo.Contains(strKeyWord)
                        || x.BrnNameTo.Contains(strKeyWord)
                    );
                    List<string> listDocstatus = null;
                    listDocstatus = DefaultService.GetListDocStatusFromKeyWord(strKeyWord);
                    if(listDocstatus != null && listDocstatus.Any())
                    {
                        expHeader = expHeader.Or(x => listDocstatus.Contains(x.DocStatus));
                    }
                    qry = qry.Where(expHeader);
                }
                result.totalItems = await qry.CountAsync();
                qry = qry.OrderByDescending(x=> x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo);
                //result.totalItems = qry.Count();
                if (param.Skip > 0 || param.Take > 0)
                {
                    qry = qry.Skip(param.Skip).Take(param.Take);
                }                
                result.Data = await qry.Select(x=> convertObject<ModelTransferOutHeader>(x)).ToListAsync();
            }
            else
            {
                var guid = new Guid(param.Guid);
                var hd = await this.context.InvTranoutHds.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == guid);
                if(hd != null)
                {
                    var hd2 = convertObject<ModelTransferOutHeader>(hd);
                    hd2.ListTransOutDt = await this.context.InvTranoutDts.AsNoTracking().Where(
                        x => x.BrnCode == hd2.BrnCode
                        && x.CompCode == hd2.CompCode
                        && x.DocNo == hd2.DocNo
                        && x.LocCode == hd2.LocCode
                    ).ToListAsync();
                    result.Data = new List<ModelTransferOutHeader>() { hd2 };
                }
            }            
            return result;
        }

        /*
         var guid = new Guid(param.Guid);

                Func<InvTranoutHd, List<InvTranoutDt> , ModelTransferOutHeader>
                funcGetTranferOutHeader = (h, d) =>
                {
                    var result = convertObject<ModelTransferOutHeader>(h);                    
                    result.ListTransOutDt = d?.ToList();
                    return result;
                };

                var qry = from th in this.context.InvTranoutHds.AsNoTracking()
                          join td in this.context.InvTranoutDts.AsNoTracking()
                          on new { th.BrnCode, th.CompCode, th.LocCode, th.DocNo }
                          equals new { td.BrnCode, td.CompCode, td.LocCode, td.DocNo }
                          where th.Guid == guid
                          select new { td, th } into thd
                          group thd by thd.th into g2
                          select funcGetTranferOutHeader(
                              g2.Key ,                                
                              g2.Select(x=> x.td).ToList()
                        );
                var transferHeader = await qry.FirstOrDefaultAsync();
                result.Data = new List<ModelTransferOutHeader>() { transferHeader };
         
         */

        /*
         
         public async Task<ResponseData<List<ModelTransferOutHeader>>> GetTransferOutList(TransferOutQueryResource param)
        {
            var result = new ResponseData<List<ModelTransferOutHeader>>();
            if(string.IsNullOrEmpty( param.Guid))
            {
                var qry = this.context.InvTranoutHds.Where(
                    x => x.LocCode == param.LocCode
                    && x.BrnCode == param.BrnCode
                    && x.CompCode == param.CompCode
                );
                bool haveFromDate = param.FromDate != null && param.FromDate.HasValue;
                bool haveToDate = param.ToDate != null && param.ToDate.HasValue;
                if (haveFromDate || haveToDate)
                {
                    qry = qry.Where(x => x.DocDate != null && x.DocDate.HasValue);
                    if (haveFromDate)
                    {
                        qry = qry.Where(x => x.DocDate >= param.FromDate);
                    }
                    if (haveToDate)
                    {
                        qry = qry.Where(x => x.DocDate <= param.ToDate);
                    }
                }
                result.totalItems = await qry.CountAsync();
                if(param.Skip > 0 || param.Take > 0)
                {
                    qry = qry.Skip(param.Skip).Take(param.Take);
                }                
                result.Data = await qry.Select(x=> convertObject<ModelTransferOutHeader>(x)).ToListAsync();
            }
            else
            {
                var guid = new Guid(param.Guid);

                Func<InvTranoutHd, List<InvRequestHd>, List<InvTranoutDt> , ModelTransferOutHeader>
                funcGetTranferOutHeader = (t, r ,d) =>
                {
                    var result = convertObject<ModelTransferOutHeader>(t);
                    result.RefDocDate = r?.FirstOrDefault()?.DocDate;
                    result.ListTransOutDt = d?.Select(x => convertObject<ModelTransferOutDetail>(x))?.ToList();
                    return result;
                };

                var qry = from th in this.context.InvTranoutHds
                          join rqh in this.context.InvRequestHds
                          on th.RefNo equals rqh.DocNo
                          join td in this.context.InvTranoutDts
                          on new { th.BrnCode, th.CompCode, th.LocCode, th.DocNo }
                          equals new { td.BrnCode, td.CompCode, td.LocCode, td.DocNo }
                          where th.Guid == guid
                          select new { th, rqh, td } into g1
                          group g1 by g1.th into g2
                          select funcGetTranferOutHeader(
                              g2.Key , 
                              g2.Select(x=> x.rqh).ToList() , 
                              g2.Select(x=> x.td).ToList()
                        );
                var transferHeader = await qry.FirstOrDefaultAsync();
                result.Data = new List<ModelTransferOutHeader>() { transferHeader };

                var transferOut2 = await this.context.InvTranoutHds
                    .FirstOrDefaultAsync(x => x.Guid == guid);
                if(transferOut2 != null)
                {
                    var transferOut = convertObject<ModelTransferOutHeader>(transferOut2);
                    transferOut.ListTransOutDt = await this.context.InvTranoutDts.Where(
                        x => x.DocNo == transferOut.DocNo
                        && x.BrnCode == transferOut.BrnCode
                        && x.CompCode == transferOut.CompCode
                        && x.LocCode == transferOut.LocCode
                    ).Select(x=> convertObject<ModelTransferOutDetail>(x))
                    .ToListAsync();
                    result.Data = new List<ModelTransferOutHeader>() { transferOut };
                }
            }            
            return result;
        }
         
         
         */

        public async Task<List<InvRequestDt>>
        GetRequestDtList(GetRequestDtListQueryResource param)
        {
            var result = await this.context.InvRequestDts.AsNoTracking().Where(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.DocNo == param.DocNo
                && x.DocTypeId == param.DocTypeId
            ).ToListAsync();
            return result;
        }


        public async Task<List<InvRequestHd>>
        GetRequestHdList(GetRequestHdListQueryResource param)
        {
            return await getRequestHdListAll(param);
        }

        public async Task<Guid> InsertTransferOut(ModelTransferOutHeader param)
        {
            await adjustHeaderRunningNo(param);
            param.DocStatus = "Active";
            param.CreatedDate = DateTime.Now;
            param.Guid = Guid.NewGuid();

            if(param.ListTransOutDt != null && param.ListTransOutDt.Any())
            {
                for (int i = 0; i < param.ListTransOutDt.Count; i++)
                {
                    var item = param.ListTransOutDt[i];
                    item.DocNo = param.DocNo;
                    item.LocCode = param.LocCode;
                }

            }
            await context.InvTranoutHds.AddAsync(param);
            await context.InvTranoutDts.AddRangeAsync(param.ListTransOutDt);
            await updateRequest(param , true);
            await insertTransferOutLog(param);

            #region Send data to warpad

            var configApi = context.SysConfigApis.Where(x => x.SystemId == "Warpad" && x.ApiId == "M002").FirstOrDefault();

            if (configApi != null)
            {
                var devCodeFrom = context.MasOrganizes.Where(x => x.OrgComp == param.CompCode && x.OrgCode == param.BrnCode).Select(x => x.OrgCodedev).FirstOrDefault();
                var devCodeTo = context.MasOrganizes.Where(x => x.OrgComp == param.CompCode && x.OrgCode == param.BrnCodeTo).Select(x => x.OrgCodedev).FirstOrDefault();

                var toppic = configApi.Topic.Replace("{doc_date}", param.DocDate.Value.ToString("dd/MM/yyyy"));


                var request = new RequestWarpadModel()
                {
                    TOPIC = toppic,
                    CREATE_DATE = param.CreatedDate.Value.ToString("yyyyMMdd"),
                    CREATE_TIME = param.CreatedDate.Value.ToString("HH:mm"),
                    BRANCH_FROM = devCodeFrom ?? "",
                    BRANCH_TO = devCodeTo ?? "",
                    DOC_NUMBER = param.DocNo,
                    LINK = "https://maxstation.pt.co.th/TransferOutList",
                };

                var listData = new List<RequestWarpadDataMedel>();
                foreach (var item in param.ListTransOutDt)
                {
                    var data = new RequestWarpadDataMedel()
                    {
                        ITEM = $"{item.PdId} - {item.PdName} : {item.ItemQty} {item.UnitName}"
                    };

                    listData.Add(data);
                }

                request.DATA = listData;

                await SendDataToWarpadAsync(request, configApi.ApiUrl);
            }

            return (Guid)param.Guid;
            #endregion
        }

        public async Task UpdateTransferOut(ModelTransferOutHeader param)
        {
            if(param == null)
            {
                return;
            }
            param.UpdatedDate = DateTime.Now;
            //context.Entry(param).State = EntityState.Modified;

            EntityEntry<ModelTransferOutHeader> entTranOut = null;
            entTranOut = context.Update(param);
            foreach (var item in _arrForbidenUpdateField)
            {
                entTranOut.Property(item).IsModified = false;
            }
            await updateTransferOutLog(param);
            await updateRequest(param , false);
        }

        private async Task<List<InvRequestHd>>
        getRequestHdListAll(GetRequestHdListQueryResource param)
        {
            //context.InvRequestHds.frp

            //IQueryable<InvTranoutHd> qryTransOut = null;
            //qryTransOut = context.InvTranoutHds
            //    .Where(x => !"Cancel".Equals(x.DocStatus))
            //    .AsNoTracking();
            IQueryable<InvRequestHd> requestHeader = null;
            requestHeader = this.context.InvRequestHds
                //.Where(x=> !qryTransOut.Any(y=> x.DocNo == y.RefNo))
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(param.DocStatus))
            {
                requestHeader = requestHeader.Where(x => param.DocStatus.Equals(x.DocStatus));
            }
            if (!string.IsNullOrWhiteSpace(param.DocNo))
            {
                requestHeader = requestHeader.Where(x => param.DocNo.Equals(x.DocNo));
            }
            if (!string.IsNullOrWhiteSpace(param.DocTypeId))
            {
                requestHeader = requestHeader.Where(x => param.DocTypeId.Equals(x.DocTypeId));
            }
            if (!string.IsNullOrWhiteSpace(param.Keyword))
            {
                requestHeader = requestHeader.Where(
                    x => (!string.IsNullOrEmpty(x.DocNo) && x.DocNo.Contains(param.Keyword))
                    || (!string.IsNullOrEmpty(x.DocStatus) && x.DocStatus.Contains(param.Keyword)));
            }
            if (!string.IsNullOrWhiteSpace(param.BrnCodeFrom))
            {
                requestHeader = requestHeader.Where(x => param.BrnCodeFrom.Equals(x.BrnCodeFrom));
            }
            if (!string.IsNullOrWhiteSpace(param.CompCode))
            {
                requestHeader = requestHeader.Where(x => param.CompCode.Equals(x.CompCode));
            }
            if (param.SysDate.HasValue)
            {
                //DateTime? datStart = null;
                //datStart = param.SysDate.Value.AddDays(-30);
                //requestHeader = requestHeader.Where(
                //    x => x.DocDate >= datStart
                //    && x.DocDate <= param.SysDate.Value
                //);

                requestHeader = requestHeader.Where(x => x.DocDate <= param.SysDate.Value);
            }
            List<InvRequestHd> result = null;
            result = await requestHeader.ToListAsync();
            return result;
        }

        #region - Private Function -

        private async Task insertTransferOutLog(ModelTransferOutHeader pInput)
        {
            if(pInput == null)
            {
                return;
            }
            ModelTransferOutHeaderLog transOutLog = null;
            transOutLog = convertObject<ModelTransferOutHeaderLog>(pInput);
            string strJson = JsonConvert.SerializeObject(transOutLog);
            InvTranoutLog headerLog = new InvTranoutLog();
            headerLog.BrnCode = pInput.BrnCode;
            headerLog.CompCode = pInput.CompCode;
            headerLog.CreatedBy = pInput.CreatedBy;
            headerLog.CreatedDate = pInput.CreatedDate;
            headerLog.DocNo = pInput.DocNo;
            headerLog.JsonData = strJson;
            headerLog.LocCode = pInput.LocCode;            
            await context.InvTranoutLogs.AddAsync(headerLog);
        }
        private async Task updateTransferOutLog(ModelTransferOutHeader pInput)
        {
            if (pInput == null)
            {
                return;
            }
            ModelTransferOutHeaderLog transOutLog = null;
            transOutLog = convertObject<ModelTransferOutHeaderLog>(pInput);
            string strJson = JsonConvert.SerializeObject(transOutLog);            
            Expression<Func<InvTranoutLog,bool>> expLog = null;
            expLog = x => x.BrnCode == pInput.BrnCode
                && x.CompCode == pInput.CompCode
                && x.DocNo == pInput.DocNo
                && x.LocCode == pInput.LocCode;
            IQueryable<InvTranoutLog> qryTransOutLog = null;
            qryTransOutLog = context.InvTranoutLogs.Where(expLog);
            InvTranoutLog headerLog = null;
            headerLog = await qryTransOutLog.FirstOrDefaultAsync();
            if(headerLog != null)
            {
                headerLog.JsonData = strJson;
            }
        }
        private static T convertObject<T>(object pObjInput)
        {
            if(pObjInput == null)
            {
                return default(T);
            }
            var serialized = JsonConvert.SerializeObject(pObjInput);            
            var result = JsonConvert.DeserializeObject<T>(serialized);
            return result;
        }

        private async Task adjustHeaderRunningNo(ModelTransferOutHeader pTransferOutHeader)
        {
            string strRunningDocNo = string.Empty;
            var qryDocPattern = 
                from dp in context.MasDocPatterns.AsNoTracking()
                join dt in context.MasDocPatternDts.AsNoTracking()
                on dp.DocId equals dt.DocId
                where "TransferOut".Equals(dp.DocType)
                select new MasDocPatternDt() { 
                    DocValue = dt.DocValue , 
                    DocCode = dt.DocCode , 
                    SeqNo = dt.SeqNo 
                };
            List<MasDocPatternDt> listDocPatternDetail = null;
            listDocPatternDetail = (await qryDocPattern.ToListAsync()).OrderBy(x=> x.SeqNo).ToList();
            //var docPattern = await this.context.MasDocPatterns.AsNoTracking().FirstOrDefaultAsync(x => "TransferOut".Equals(x.DocType));
            bool isUseDefaultPattern = true;
            isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            //if (docPattern != null)
            //{
            //    listDocPatternDetail = await context.MasDocPatternDts.Where(x => x.DocId == docPattern.DocId.ToString()).OrderBy(x => x.SeqNo).ToListAsync();
                
            //}
            int intLastRunning = 0;
            int intDay = pTransferOutHeader.DocDate.Value.Day;// DateTime.Now.Day;
            int intMonth = pTransferOutHeader.DocDate.Value.Month;// DateTime.Now.Month;
            int intYear = pTransferOutHeader.DocDate.Value.Year;//DateTime.Now.Year;
            var transferOut = context.InvTranoutHds.Where(
                x => x.BrnCode == pTransferOutHeader.BrnCode
                && x.CompCode == pTransferOutHeader.CompCode
                && x.LocCode == pTransferOutHeader.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue);
            if (isUseDefaultPattern)
            {
                transferOut = transferOut.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    transferOut = transferOut.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        transferOut = transferOut.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            transferOut = transferOut.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            var listTransfer = await transferOut.ToListAsync();

            if (listTransfer.Any())
            {
                intLastRunning = listTransfer.Max(x => x.RunNumber.Value);
            }
            //intLastRunning = await billing.DefaultIfEmpty().MaxAsync(x => x.RunNumber.Value) + 1;
            do
            {
                intLastRunning++;
                strRunningDocNo = string.Empty;
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
                            case "Comp": strRunningDocNo += pTransferOutHeader.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pTransferOutHeader.BrnCode; break;
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

            } while (await context.InvTranoutHds.AnyAsync(
                x => x.BrnCode == pTransferOutHeader.BrnCode
                && x.CompCode == pTransferOutHeader.CompCode
                && x.LocCode == pTransferOutHeader.LocCode
                && x.DocNo == strRunningDocNo
            ));
            pTransferOutHeader.RunNumber = intLastRunning;
            pTransferOutHeader.DocNo = strRunningDocNo;
        }

        private async Task updateRequest(ModelTransferOutHeader pTransferOutHeader , bool pIsInsert)
        {
            InvRequestHd requestHeader = await context.InvRequestHds.AsNoTracking().FirstOrDefaultAsync(
                x => x.CompCode == pTransferOutHeader.CompCode
                && x.DocNo == pTransferOutHeader.RefNo
                && x.BrnCode == pTransferOutHeader.BrnCodeTo
            );
            if(requestHeader == null)
            {
                return;
            }
            context.InvRequestHds.Attach(requestHeader);
            if ("Cancel".Equals( pTransferOutHeader.DocStatus))
            {
                requestHeader.DocStatus = "Ready";
            }
            else
            {
                requestHeader.DocStatus = "Reference";
            }
            requestHeader.UpdatedDate = DateTime.Now;
            requestHeader.UpdatedBy = pTransferOutHeader.CreatedBy;
            List<InvTranoutDt> listTransOutDt = pTransferOutHeader.ListTransOutDt;
            if(listTransOutDt == null || !listTransOutDt.Any())
            {
                return;
            }
            IQueryable<InvRequestDt> qryRequestDetail = context.InvRequestDts.AsNoTracking().Where(
                x => x.CompCode == requestHeader.CompCode
                && x.BrnCode == requestHeader.BrnCode
                && x.LocCode == requestHeader.LocCode
                && x.DocNo == requestHeader.DocNo
                && x.DocTypeId == requestHeader.DocTypeId
            );
            List<InvRequestDt> listRequestDetail = await qryRequestDetail.ToListAsync();
            if(listRequestDetail == null || !listRequestDetail.Any())
            {
                return;
            }

            Dictionary<Tuple<string, string>, MasProductUnit> dicProductUnit = new Dictionary<Tuple<string, string>, MasProductUnit>();
            Func<string, string, Task< MasProductUnit>> 
            funcGetProductUnit = async (PdId, UnitId) =>
            {
                var tpKey = Tuple.Create(PdId, UnitId);
                if (dicProductUnit.ContainsKey(tpKey))
                {
                    return dicProductUnit[tpKey];
                }
                else
                {
                    MasProductUnit pu2 = await context.MasProductUnits.AsNoTracking().FirstOrDefaultAsync(x => x.PdId == PdId && x.UnitId == UnitId);
                    dicProductUnit.Add(tpKey, pu2);
                    return pu2;
                }
            };

            context.InvRequestDts.AttachRange(listRequestDetail);            
            foreach (var requestDt in listRequestDetail)
            {
                InvTranoutDt transOutDt = listTransOutDt.FirstOrDefault(x => x.SeqNo == requestDt.SeqNo);
                if (transOutDt == null)
                {
                    continue;
                }
                decimal decTransOutItemQty = transOutDt?.ItemQty ?? decimal.Zero;
                if (decimal.Zero.Equals(decTransOutItemQty))
                {
                    continue;
                }
                MasProductUnit productUnit = await funcGetProductUnit(requestDt.PdId, requestDt.UnitId);
                int intUnitRatio = productUnit?.UnitRatio ?? 1;
                int intUnitStock = productUnit?.UnitStock ?? 1;
                decimal decTransOutStockQty = decTransOutItemQty * (intUnitStock / intUnitRatio);
                transOutDt.StockQty = decTransOutStockQty;
                transOutDt.StockRemain = decTransOutStockQty;
                if ("Cancel".Equals(pTransferOutHeader.DocStatus))
                {
                    requestDt.StockRemain = requestDt.StockQty;
                    //requestDt.StockRemain += decTransOutStockQty;
                }
                else if(pIsInsert)
                {
                    requestDt.StockRemain = requestDt.StockQty - transOutDt.StockQty;
                    //requestDt.StockRemain -= decTransOutStockQty;
                }
            }
        }

        /*
        private async Task updateRequest(ModelTransferOutHeader pTransferOutHeader, bool pIsInsert)
        {
            InvRequestHd requestHeader = await context.InvRequestHds.AsNoTracking().FirstOrDefaultAsync(
                x => x.CompCode == pTransferOutHeader.CompCode
                && x.DocNo == pTransferOutHeader.RefNo
                && x.BrnCode == pTransferOutHeader.BrnCodeTo
            );
            if (requestHeader == null)
            {
                return;
            }
            context.InvRequestHds.Attach(requestHeader);
            if ("Cancel".Equals(pTransferOutHeader.DocStatus))
            {
                requestHeader.DocStatus = "Ready";
            }
            else
            {
                requestHeader.DocStatus = "Reference";
            }
            requestHeader.UpdatedDate = DateTime.Now;
            requestHeader.UpdatedBy = pTransferOutHeader.CreatedBy;
            List<InvTranoutDt> listTransOutDt = pTransferOutHeader.ListTransOutDt;
            if (listTransOutDt == null || !listTransOutDt.Any())
            {
                return;
            }
            IQueryable<InvRequestDt> qryRequestDetail = context.InvRequestDts.AsNoTracking().Where(
                x => x.CompCode == requestHeader.CompCode
                && x.BrnCode == requestHeader.BrnCode
                && x.LocCode == requestHeader.LocCode
                && x.DocNo == requestHeader.DocNo
                && x.DocTypeId == requestHeader.DocTypeId
            );
            List<InvRequestDt> listRequestDetail = await qryRequestDetail.ToListAsync();
            if (listRequestDetail == null || !listRequestDetail.Any())
            {
                return;
            }

            Dictionary<Tuple<string, string>, MasProductUnit> dicProductUnit = new Dictionary<Tuple<string, string>, MasProductUnit>();
            Func<string, string, Task<MasProductUnit>>
            funcGetProductUnit = async (PdId, UnitId) =>
            {
                var tpKey = Tuple.Create(PdId, UnitId);
                if (dicProductUnit.ContainsKey(tpKey))
                {
                    return dicProductUnit[tpKey];
                }
                else
                {
                    MasProductUnit pu2 = await context.MasProductUnits.AsNoTracking().FirstOrDefaultAsync(x => x.PdId == PdId && x.UnitId == UnitId);
                    dicProductUnit.Add(tpKey, pu2);
                    return pu2;
                }
            };

            context.InvRequestDts.AttachRange(listRequestDetail);
            foreach (var requestDt in listRequestDetail)
            {
                InvTranoutDt transOutDt = listTransOutDt.FirstOrDefault(x => x.SeqNo == requestDt.SeqNo);
                if (transOutDt == null)
                {
                    continue;
                }
                decimal decTransOutItemQty = transOutDt?.ItemQty ?? decimal.Zero;
                if (decimal.Zero.Equals(decTransOutItemQty))
                {
                    continue;
                }
                MasProductUnit productUnit = await funcGetProductUnit(requestDt.PdId, requestDt.UnitId);
                int intUnitRatio = productUnit?.UnitRatio ?? 1;
                int intUnitStock = productUnit?.UnitStock ?? 1;
                decimal decTransOutStockQty = decTransOutItemQty * (intUnitStock / intUnitRatio);
                if ("Cancel".Equals(pTransferOutHeader.DocStatus))
                {
                    requestDt.StockRemain += decTransOutStockQty;
                }
                else if (pIsInsert)
                {
                    requestDt.StockRemain -= decTransOutStockQty;
                }
            }
        }
        */

        #endregion

        public InvRequestHd GetRequest(ModelTransferOutHeader param)
        {
            return context.InvRequestHds.AsNoTracking().FirstOrDefault(x => x.CompCode == param.CompCode && x.BrnCode == param.BrnCodeTo && x.DocNo == param.RefNo );
        }
    }
}
