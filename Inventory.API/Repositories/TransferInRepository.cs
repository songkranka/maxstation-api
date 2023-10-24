using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using Inventory.API.Resources.Request;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class TransferInRepository : SqlDataAccessHelper , ITransferInRepository
    {
        public TransferInRepository(PTMaxstationContext context) : base(context) { }

        public async Task<ModelResponseData<List<ModelTransferInHeader>>>
        SearchTranIn(SearchTranInQueryResource param)
        {
            if(param == null)
            {
                return null;
            }
            var result = new ModelResponseData<List<ModelTransferInHeader>>();
            IQueryable<InvTraninHd> transferIn = context.InvTraninHds
                .OrderByDescending(x => x.CompCode)
                .OrderByDescending(x => x.BrnCode)
                .OrderByDescending(x => x.LocCode)
                .OrderByDescending(x => x.DocNo)
                .AsNoTracking();

            if(String.IsNullOrWhiteSpace(param.Guid))
            {
                if (!string.IsNullOrWhiteSpace(param.LocCode))
                {
                    transferIn = transferIn.Where(x => param.LocCode.Equals(x.LocCode));
                }
                if (!string.IsNullOrWhiteSpace(param.BrnCode))
                {
                    transferIn = transferIn.Where(x => param.BrnCode.Equals(x.BrnCode));
                }
                if (!string.IsNullOrWhiteSpace(param.CompCode))
                {
                    transferIn = transferIn.Where(x => param.CompCode.Equals(x.CompCode));
                }
                if (param.FromDate.HasValue || param.ToDate.HasValue)
                {
                    transferIn = transferIn.Where(x => x.DocDate.HasValue);
                    if (param.FromDate.HasValue)
                    {
                        transferIn = transferIn.Where(x => x.DocDate >= param.FromDate);
                    }
                    if (param.ToDate.HasValue)
                    {
                        transferIn = transferIn.Where(x => x.DocDate <= param.ToDate);
                    }
                }
                if (!string.IsNullOrEmpty(param.Keyword))
                {
                    transferIn = transferIn.Where(
                        x => !string.IsNullOrWhiteSpace(x.DocNo) 
                        && x.DocNo.Contains(param.Keyword)
                    );
                }
                result.totalItems = await transferIn.CountAsync();
                if (param.Skip > 0)
                {
                    transferIn = transferIn.Skip(param.Skip);
                }
                if(param.Take > 0)
                {
                    transferIn = transferIn.Take(param.Take);
                }                
                List<InvTraninHd> listTranferInHeader = await transferIn.ToListAsync();
                if (listTranferInHeader.Any())
                {
                    List<ModelTransferInHeader> listTransferInHeader2 
                        = listTranferInHeader
                        .Select(x => convertObject<ModelTransferInHeader>(x))
                        .ToList();

                    result.SetData(listTransferInHeader2);
                }
                //if(list)
            }
            else
            {
                Guid guid = Guid.Parse(param.Guid);
                InvTraninHd transferInHeader = await transferIn.FirstOrDefaultAsync(x => x.Guid == guid);
                if(transferInHeader != null)
                {
                    //transferInHeader.
                    ModelTransferInHeader transferInHeader2 
                        = convertObject<ModelTransferInHeader>(transferInHeader);
                    await transferInHeader2.LoadDetailAsync(context);
                    var listTransferInHeader 
                        = new List<ModelTransferInHeader>() { transferInHeader2 };
                    result.SetData(listTransferInHeader);

                }
            }
            return result;
        }


        public async Task<List<InvTraninDt>>
        GetListTransferInDetail(ModelTransferInHeader pHeader)
        {
            if(pHeader == null)
            {
                return null;
            }
            await pHeader.LoadDetailAsync(context);
            return pHeader.ListTransferInDetail;
        }

        public async Task<List<InvTranoutDt>> GetTransOutDtList(ModelTransferOutHeader param)
        {
            if(param == null)
            {
                return null;
            }
            IQueryable<InvTranoutDt> transOutDt = context.InvTranoutDts.AsNoTracking().Where(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.DocNo == param.DocNo
                && x.LocCode == param.LocCode
            );
            List<InvTranoutDt> result = await transOutDt.ToListAsync();
            return result;
        }

        public async Task<List<InvTranoutHd>> GetTransOutHdList(GetTransOutHdListQueryResource param)
        {
            if(param == null)
            {
                return null;
            }
            IQueryable<InvTranoutHd> qryTransOut = context.InvTranoutHds.AsNoTracking()
                .Where( x=> "Active".Equals( x.DocStatus));
            if (!string.IsNullOrWhiteSpace(param.BrnCodeTo))
            {
                qryTransOut = qryTransOut.Where(x => x.BrnCodeTo == param.BrnCodeTo);
            }
            if(!string.IsNullOrWhiteSpace(param.CompCode))
            {
                qryTransOut = qryTransOut.Where(x => x.CompCode == param.CompCode);
            }
            if (!string.IsNullOrWhiteSpace(param.Keyword))
            {
                qryTransOut = qryTransOut.Where(
                    x => x.DocNo.Contains(param.Keyword)
                    || x.BrnCodeTo.Contains(param.Keyword)
                    || x.BrnNameTo.Contains(param.Keyword)
                    || context.MasBranches.Where(
                        y=> !string.IsNullOrWhiteSpace(y.BrnName) 
                        && y.BrnName.Contains(param.Keyword) 
                    ).Any(y=> y.BrnCode == x.BrnCode)
                );
            }
            if (!string.IsNullOrEmpty(param.SysDate)) 
            {
                var docDate = DateTime.ParseExact(param.SysDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                qryTransOut = qryTransOut.Where(x => x.DocDate <= docDate);
            }
            List<InvTranoutHd> result = await qryTransOut.ToListAsync();
            return result;
        }

        public async Task<ModelTransferInHeader> InsertTransferIn(ModelTransferInHeader param)
        {
            await adjustHeaderRunningNo(param);
            param.Guid = Guid.NewGuid();
            param.CreatedDate = DateTime.Now;
            param.DocStatus = "Active";

            InvTranoutHd tranoutHd = context.InvTranoutHds.FirstOrDefault(x => x.DocNo == param.RefNo);
            tranoutHd.DocStatus = "Reference";

            List<InvTranoutDt> tranoutDt = context.InvTranoutDts.Where(x => x.DocNo == param.RefNo).ToList();
            List<InvTranoutDt> updateTranoutDt = new List<InvTranoutDt>();
            if (param.ListTransferInDetail != null && param.ListTransferInDetail.Any())
            {
                foreach (var item in param.ListTransferInDetail)
                {
                    item.DocNo = param.DocNo;
                    item.LocCode = param.LocCode;
                    var itemTranoutDt = tranoutDt.Where(x => x.PdId == item.PdId).FirstOrDefault();
                    if (itemTranoutDt != null) 
                    {
                        decimal sumStockRemain = (decimal)(itemTranoutDt.StockRemain - item.StockQty);
                        itemTranoutDt.StockRemain = sumStockRemain;
                        updateTranoutDt.Add(itemTranoutDt);
                    }
                }
            }
            IQueryable<InvTraninDt> qryTranferInDetail = context.InvTraninDts.Where(
                x => x.DocNo == param.DocNo
                && x.CompCode == param.CompCode
                && x.BrnCode == param.BrnCode
                && x.LocCode == param.LocCode);
            if(await qryTranferInDetail.AnyAsync())
            {
                context.InvTraninDts.RemoveRange(qryTranferInDetail);
            }

            await context.InvTraninHds.AddAsync(param);
            await context.InvTraninDts.AddRangeAsync(param.ListTransferInDetail);
            context.InvTranoutHds.Update(tranoutHd);
            context.InvTranoutDts.UpdateRange(updateTranoutDt);

            #region Send data to warpad

            var configApi = context.SysConfigApis.Where(x => x.SystemId == "Warpad" && x.ApiId == "M003").FirstOrDefault();

            if (configApi != null)
            {
                var devCodeFrom = context.MasOrganizes.Where(x => x.OrgComp == param.CompCode && x.OrgCode == param.BrnCodeFrom).Select(x => x.OrgCodedev).FirstOrDefault();
                var devCodeTo = context.MasOrganizes.Where(x => x.OrgComp == param.CompCode && x.OrgCode == param.BrnCode).Select(x => x.OrgCodedev).FirstOrDefault();

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
                foreach (var item in param.ListTransferInDetail)
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

            #endregion

            return param;
        }

        public void UpdateTransferIn(ModelTransferInHeader param)
        {
            if(param == null)
            {
                return;
            }

            if (param.DocStatus.ToUpper() == "CANCEL") 
            {
                var transferOutHd = context.InvTranoutHds.Where(x => x.DocNo == param.RefNo).FirstOrDefault();
                transferOutHd.DocStatus = "Active";
                context.InvTranoutHds.Update(transferOutHd);
            }


            param.UpdatedDate = DateTime.Now;
            context.Entry(param).State = EntityState.Modified;
        }

        public async Task<bool> CheckTranferByRefNo(ModelTransferInHeader param)
        {
            var receive = await context.InvTraninHds.FirstOrDefaultAsync(x => x.RefNo == param.RefNo && x.DocStatus == "Active");

            if (receive != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<InvTraninHd> GetTranferInHdByGuid(Guid guid)
        {
            var qryBranch = await context.InvTraninHds.FirstOrDefaultAsync(x => x.Guid == guid);
            return qryBranch;
        }

        private static T convertObject<T>(object pObjInput)
        {
            if (pObjInput == null)
            {
                return default(T);
            }
            var serialized = JsonConvert.SerializeObject(pObjInput);
            var result = JsonConvert.DeserializeObject<T>(serialized);
            return result;
        }


        private async Task adjustHeaderRunningNo(ModelTransferInHeader pTransferInHeader)
        {
            string strRunningDocNo = string.Empty;
            var qryDocPattern =
                from dp in context.MasDocPatterns.AsNoTracking()
                join dt in context.MasDocPatternDts.AsNoTracking()
                on dp.DocId equals dt.DocId
                where "TransferIn".Equals(dp.DocType)
                select new MasDocPatternDt()
                {
                    DocValue = dt.DocValue,
                    DocCode = dt.DocCode,
                    SeqNo = dt.SeqNo
                };
            List<MasDocPatternDt> listDocPatternDetail = null;
            listDocPatternDetail = (await qryDocPattern.ToListAsync()).OrderBy(x => x.SeqNo).ToList();
            bool isUseDefaultPattern = true;
            isUseDefaultPattern = listDocPatternDetail == null || !listDocPatternDetail.Any();
            int intLastRunning = 0;
            int intDay = pTransferInHeader.DocDate.Value.Day; //DateTime.Now.Day;
            int intMonth = pTransferInHeader.DocDate.Value.Month; //DateTime.Now.Month;
            int intYear = pTransferInHeader.DocDate.Value.Year; //DateTime.Now.Year;
            var transferIn = context.InvTraninHds.Where(
                x => x.BrnCode == pTransferInHeader.BrnCode
                && x.CompCode == pTransferInHeader.CompCode
                && x.LocCode == pTransferInHeader.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue);
            if (isUseDefaultPattern)
            {
                transferIn = transferIn.Where(x => intYear.Equals(x.DocDate.Value.Year) && intMonth.Equals(x.DocDate.Value.Month));
            }
            else
            {
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    transferIn = transferIn.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        transferIn = transferIn.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            transferIn = transferIn.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }
            var listTransfer = await transferIn.ToListAsync();

            if (listTransfer.Any())
            {
                intLastRunning = listTransfer.Max(x => x.RunNumber.Value);
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
                    foreach (var item in listDocPatternDetail)
                    {
                        if (item == null) continue;
                        switch (item.DocCode)
                        {
                            case "-": strRunningDocNo += "-"; break;
                            case "MM": strRunningDocNo += intMonth.ToString("00"); break;
                            case "Comp": strRunningDocNo += pTransferInHeader.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pTransferInHeader.BrnCode; break;
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

            } while (await context.InvTraninHds.AnyAsync(
                x => x.BrnCode == pTransferInHeader.BrnCode
                && x.CompCode == pTransferInHeader.CompCode
                && x.LocCode == pTransferInHeader.LocCode
                && x.DocNo == strRunningDocNo
            ));
            pTransferInHeader.RunNumber = intLastRunning;
            pTransferInHeader.DocNo = strRunningDocNo;
        }


    }
}
