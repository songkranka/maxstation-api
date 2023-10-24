using Abp.Linq.Expressions;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using Inventory.API.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class ReceiveGasRepository : SqlDataAccessHelper , IReceiveGasRepository
    {

        private const string _strNew = "New";
        private const string _strActive = "Active";
        private const string _strGas = "Gas";
        private const string _strCancel = "Cancel";
        private const string _strPoDeleteFlag = "L";
        public ReceiveGasRepository(PTMaxstationContext context) : base(context)
        {
        }
        public async Task<InfPoHeader[]> GetPoHeaderList(PoHeaderListQuery pQuery)
        {
            if(pQuery == null)
            {
                return null;
            }

            IQueryable<InfPoType> qryPoType = null;
            qryPoType = context.InfPoTypes.Where(
                x => _strGas.Equals(x.PoTypeDesc)
            ).AsNoTracking();

            IQueryable<InvReceiveProdHd> qryReceiveGas = null;
            qryReceiveGas = context.InvReceiveProdHds.Where(
                x => !_strCancel.Equals(x.DocStatus) 
                && _strGas.Equals(x.DocType)
            ).AsNoTracking();

            IQueryable<InfPoItem> qryPoItem = null;
            qryPoItem = context.InfPoItems.Where(
                x=> x.Plant == pQuery.BrnCode
                && !_strPoDeleteFlag.Equals(x.DeleteInd)
            );
            //if (pQuery.SystemDate.HasValue)
            //{
            //    qryPoItem = qryPoItem.Where(x => pQuery.SystemDate.Value >= x.PoDate);
            //}
            IQueryable<InfPoHeader> qryPoHeader = null;
            qryPoHeader = context.InfPoHeaders.Where(
                x=> qryPoType.Any(y=> y.PoTypeId == x.DocType) 
                //&& !qryReceiveGas.Any(y=> y.PoNo == x.PoNumber)
                && qryPoItem.Any(y=> y.PoNumber == x.PoNumber)
                && x.ReceiveStatus != "Y"
                //&& pQuery.CompCode == x.CompCode
                && !_strPoDeleteFlag.Equals(x.DeleteInd)
            ).AsNoTracking();

            //if (pQuery.SystemDate.HasValue)
            //{
            //    qryPoHeader = qryPoHeader.Where(x => pQuery.SystemDate.Value >= x.DocDate);
            //}

            InfPoHeader[] result = null;
            result = await qryPoHeader.ToArrayAsync();

            Array.ForEach(result, x => {
                x.SupplierName = context.MasSuppliers.FirstOrDefault(y => y.SupCode == x.Vendor)?.SupName ?? string.Empty;
                //x.DocDate = context.InfPoItems.FirstOrDefault(y => y.PoNumber == x.PoNumber).PoDate ?? x.DocDate;
            });

            return result;
        }
        public async Task<PoItemListResult> GetPoItemList(PoItemListParam param)
        {
            IQueryable<InfPoItem> qryPoItem = null;
            qryPoItem = context.InfPoItems.Where(
                x => x.PoNumber == param.PoNumber
                && !_strPoDeleteFlag.Equals(x.DeleteInd)
            ).AsNoTracking();

            IQueryable<MasProduct> qryProduct = null;
            qryProduct = context.MasProducts.AsNoTracking();

            InfPoItem[] arrPoItem = null;
            arrPoItem = await qryPoItem.ToArrayAsync();

            MasProduct[] arrProduct = null;
            MasUnit[] arrUnit = null;
            if (arrPoItem != null && arrPoItem.Any())
            {
                string[] arrMatterial = null;
                arrMatterial = arrPoItem
                    .Select(x => (x?.Material ?? string.Empty).Trim())
                    .Where(x=> !0.Equals(x.Length))
                    .Distinct()
                    .ToArray();
                if(arrMatterial != null && arrMatterial.Any())
                {
                    arrProduct = await qryProduct
                        .Where(x => arrMatterial.Contains(x.PdId))
                        .ToArrayAsync();
                }

                string[] arrUnitId = null;
                arrUnitId = arrPoItem
                    .Select(x => (x.PoUnit ?? string.Empty).Trim())
                    .Where(x => x.Length > 0)
                    .Distinct()
                    .ToArray();
                if(arrUnitId != null && arrUnitId.Any())
                {
                    IQueryable<MasUnit> qryUnit = null;
                    qryUnit = context.MasUnits.Where(
                        x => arrUnitId.Contains(x.MapUnitId) &&
                        _strActive.Equals( x.UnitStatus)
                    ).AsNoTracking();
                    arrUnit = await qryUnit.ToArrayAsync();
                }
            }

            IQueryable<MasDensity> qryDensity = null;
            qryDensity = context.MasDensities.Where(
                x => x.CompCode == param.CompCode
            ).AsNoTracking();

            if (param.SystemDate.HasValue)
            {
                qryDensity = qryDensity.Where(x => x.StartDate <= param.SystemDate);
            }
            MasDensity density = null;
            density = await qryDensity.OrderByDescending(x=> x.StartDate).FirstOrDefaultAsync();
            
            PoItemListResult result = null;
            result = new PoItemListResult();
            result.ArrPoItem = arrPoItem;
            result.ArrProduct = arrProduct;
            result.ArrUnit = arrUnit;
            result.Density = density;
            
            return result;
        }
        public async Task<QueryResult<InvReceiveProdHd>> GetReceiveList(ReceiveGasListQuery pQuery)
        {
            if(pQuery == null)
            {
                return null;
            }

            QueryResult<InvReceiveProdHd> result = null;
            result = new QueryResult<InvReceiveProdHd>();

            IQueryable<InvReceiveProdHd> qryReceiveHd = null;
            qryReceiveHd = context.InvReceiveProdHds
                .AsNoTracking()
                .OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo);

            Regex rx = new Regex(@"([A-Za-z])\w");

            if (!string.IsNullOrWhiteSpace(pQuery.BrnCode))
            {
                qryReceiveHd = qryReceiveHd.Where(x => x.BrnCode == pQuery.BrnCode);
            }
            if (!string.IsNullOrWhiteSpace(pQuery.CompCode))
            {
                qryReceiveHd = qryReceiveHd.Where(x => x.CompCode == pQuery.CompCode);
            }
            if (!string.IsNullOrWhiteSpace(pQuery.DocType))
            {
                qryReceiveHd = qryReceiveHd.Where(x => x.DocType == pQuery.DocType);
            }
            if (pQuery.FromDate.HasValue)
            {
                qryReceiveHd = qryReceiveHd.Where(x => x.DocDate >= pQuery.FromDate.Value);
            }
            if (pQuery.ToDate.HasValue)
            {
                qryReceiveHd = qryReceiveHd.Where(x => x.DocDate <= pQuery.ToDate.Value);
            }
            if (!string.IsNullOrWhiteSpace(pQuery.Keyword))
            {
                ExpressionStarter<InvReceiveProdHd> esReceiveProdHD = null;
                esReceiveProdHD = PredicateBuilder.New<InvReceiveProdHd>(
                    x => (x.DocNo != null && x.DocNo.Contains(pQuery.Keyword))
                    || (x.InvNo != null && x.InvNo.Contains(pQuery.Keyword))
                    || (x.PoNo != null && x.PoNo.Contains(pQuery.Keyword))
                    || (x.SupCode != null && x.SupCode.Contains(pQuery.Keyword))
                    || (x.SupName != null && x.SupName.Contains(pQuery.Keyword))
                );

                List<string> listDocStatus = null;
                listDocStatus = DefaultService.GetListDocStatusFromKeyWord(pQuery.Keyword);
                if(listDocStatus != null && listDocStatus.Any())
                {
                    esReceiveProdHD = esReceiveProdHD.Or(x => listDocStatus.Contains(x.DocStatus));
                }

                qryReceiveHd = qryReceiveHd.Where(esReceiveProdHD);
            }
            if (!string.IsNullOrWhiteSpace(pQuery.LocCode))
            {
                qryReceiveHd = qryReceiveHd.Where(x => x.LocCode == pQuery.LocCode);
            }
            result.TotalItems = await qryReceiveHd.CountAsync();
            if(pQuery.ItemsPerPage > 0 && pQuery.Page > 0)
            {
                qryReceiveHd = qryReceiveHd
                    .Skip((pQuery.Page - 1) * pQuery.ItemsPerPage)
                    .Take(pQuery.ItemsPerPage);
            }
            result.Items = await qryReceiveHd.ToListAsync();
            return result;
        }
        public async Task<ReceiveGasQuery> GetReceive(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if (0.Equals(pStrGuid))
            {
                return null;
            }
            Guid guid = Guid.Parse(pStrGuid);

            IQueryable<InvReceiveProdHd> qryHeader = null;
            qryHeader = context.InvReceiveProdHds.AsNoTracking().Where(x => x.Guid == guid);


            InvReceiveProdHd header = null;
            header = await qryHeader.FirstOrDefaultAsync();

            if(header == null)
            {
                return null;
            }
            

            IQueryable<InvReceiveProdDt> qryDetail = null;
            qryDetail = context.InvReceiveProdDts.AsNoTracking().Where(
                x=> x.BrnCode == header.BrnCode
                && x.CompCode == header.CompCode
                && x.DocNo == header.DocNo
                && x.DocType == header.DocType
                && x.LocCode == header.LocCode
            );
            InvReceiveProdDt[] arrDetail = null;
            arrDetail = await qryDetail.ToArrayAsync();

            ReceiveGasQuery result = null;
            result = new ReceiveGasQuery()
            {
                Header = header,
                Details = arrDetail
            };
            return result;
        }
        public async Task<ReceiveGasQuery> SaveReceive(ReceiveGasQuery pQuery)
        {
            if(pQuery == null )
            {
                return null;
            }
            InvReceiveProdHd header = null;
            header = pQuery.Header;
            if(header == null)
            {
                return null;
            }

            string strDocStatus = header.DocStatus;
            if (_strNew.Equals(strDocStatus))
            {
                await adjustHeaderRunningNo(header);
                header.CreatedDate = DateTime.Now;
                header.Guid = Guid.NewGuid();
                header.DocStatus = _strActive;
                await context.InvReceiveProdHds.AddAsync(header);
                await updatePo(header, true);
            }
            else
            {
                EntityEntry<InvReceiveProdHd> entReceiveProdHd = null;

                header.UpdatedDate = DateTime.Now;
                entReceiveProdHd = context.InvReceiveProdHds.Update(header);

                string[] arrNotUpdateField = null;
                arrNotUpdateField = new[] { 
                    "DocDate", "RunNumber" , "DocPattern" , 
                    "Guid" , "CreatedDate" , "CreatedBy" 
                };
                foreach (var item in arrNotUpdateField)
                {
                    entReceiveProdHd.Property(item).IsModified = false;
                }

            }
            
            IQueryable<InvReceiveProdDt> qryExistsDt = null;
            qryExistsDt = context.InvReceiveProdDts.Where(
                x => x.BrnCode == header.BrnCode
                && x.CompCode == header.CompCode
                && x.DocNo == header.DocNo
                && x.DocType == header.DocType
                && x.LocCode == header.LocCode
            );
            context.InvReceiveProdDts.RemoveRange(qryExistsDt);

            InvReceiveProdDt[] arrDetail = null;
            arrDetail = pQuery.Details;

            if(arrDetail != null && arrDetail.Any())
            {
                foreach (InvReceiveProdDt item in arrDetail)
                {
                    item.BrnCode = header.BrnCode;
                    item.CompCode = header.CompCode;
                    item.DocNo = header.DocNo;
                    item.DocType = header.DocType;
                    item.LocCode = header.LocCode;
                    while(arrDetail.Count(x=> x.SeqNo == item.SeqNo) > 1)
                    {
                        item.SeqNo += 10;
                    }
                }
                await context.InvReceiveProdDts.AddRangeAsync(arrDetail);
                await addLog(header, strDocStatus);
            }
            return pQuery;
        }

        public async Task UpdateStatus(InvReceiveProdHd pInput)
        {
            if(pInput == null)
            {
                return;
            }
            EntityEntry<InvReceiveProdHd> entReceiveProdHD = null;
            entReceiveProdHD = context.Entry(pInput);
            entReceiveProdHD.Property(x=> x.DocStatus).IsModified = true;
            entReceiveProdHD.Property(x => x.UpdatedBy).IsModified = true;
            pInput.UpdatedDate = DateTime.Now;
            await addLog(pInput, pInput.DocStatus);
            if (_strCancel.Equals(pInput.DocStatus))
            {
                await updatePo(pInput, false);
            }
        }

        public async Task<bool> CheckReceiveStatus(ReceiveGasQuery saveReceiveGas)
        {
            var receive = await context.InfPoHeaders.FirstOrDefaultAsync(x => x.PoNumber == saveReceiveGas.Header.PoNo && x.ReceiveStatus == "Y");

            if (receive != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task updatePo(InvReceiveProdHd param , bool pBoolStatus)
        {
            if(param == null)
            {
                return;
            }
            string strPoNo = DefaultService.GetString(param.PoNo);
            if (0.Equals(strPoNo.Length))
            {
                return;
            }
            var poHeader = new InfPoHeader()
            {
                PoNumber = strPoNo,
                ReceiveStatus = pBoolStatus ? "Y" : "N",
                ReceiveUpdate = DateTime.Now,
                ReceiveBrnCode = param.BrnCode,
                ReceiveLocCode = param.LocCode,
                ReceiveDocNo = param.DocNo
            };
            var entPoHeader = context.Attach(poHeader);
            entPoHeader.Property(x => x.ReceiveStatus).IsModified = true;
            entPoHeader.Property(x => x.ReceiveUpdate).IsModified = true;
            entPoHeader.Property(x => x.ReceiveBrnCode).IsModified = true;
            entPoHeader.Property(x => x.ReceiveLocCode).IsModified = true;
            entPoHeader.Property(x => x.ReceiveDocNo).IsModified = true;
            //var qryPoHeader = context.InfPoHeaders.Where(x => x.PoNumber == strPoNo);
            //var poHeader = await qryPoHeader.FirstOrDefaultAsync();
            //if(poHeader == null)
            //{
            //    return;
            //}
            //poHeader.ReceiveStatus = pBoolStatus ? "Y" : "N";
            //poHeader.ReceiveUpdate = DateTime.Now;
            //poHeader.ReceiveBrnCode = param.BrnCode;
            //poHeader.ReceiveLocCode = param.LocCode;
            //poHeader.ReceiveDocNo = param.DocNo;
            var qryPoItem = context.InfPoItems.Where(x => x.PoNumber == strPoNo);
            var listPoItem = await qryPoItem.ToListAsync();
            if(listPoItem !=null && listPoItem.Any())
            {
                foreach (var item in listPoItem)
                {
                    item.ReceiveUpdate = poHeader.ReceiveUpdate;
                }
            }            
        }

        public async Task<MasSupplier[]> GetArraySupplier()
        {
            IQueryable<MasSupplier> qrySuppliers = null;
            qrySuppliers = context.MasSuppliers.Where(
                x => _strActive.Equals(x.SupStatus)
            ).AsNoTracking();

            MasSupplier[] result = null;
            result = await qrySuppliers.ToArrayAsync();

            return result;
        }

        private async Task adjustHeaderRunningNo(InvReceiveProdHd pInvReceiveProdHd)
        {
            string strRunningDocNo = string.Empty;
            IQueryable<MasDocPatternDt> qryDocPattern = null;
            qryDocPattern = from dp in context.MasDocPatterns.AsNoTracking()
                join dt in context.MasDocPatternDts.AsNoTracking()
                on dp.DocId equals dt.DocId
                where dp.DocType == "Receive"+ pInvReceiveProdHd.DocType
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
            if (pInvReceiveProdHd.DocDate != null && pInvReceiveProdHd.DocDate.HasValue)
            {
                intDay = pInvReceiveProdHd.DocDate.Value.Day;
                intMonth = pInvReceiveProdHd.DocDate.Value.Month;
                intYear = pInvReceiveProdHd.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<InvReceiveProdHd> qryReceiveProd = null;
            qryReceiveProd = context.InvReceiveProdHds.Where(
                x => x.BrnCode == pInvReceiveProdHd.BrnCode
                && x.CompCode == pInvReceiveProdHd.CompCode
                && x.LocCode == pInvReceiveProdHd.LocCode
                && x.DocType == pInvReceiveProdHd.DocType
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                qryReceiveProd = qryReceiveProd.Where(
                    x => intYear.Equals(x.DocDate.Value.Year) 
                    && intMonth.Equals(x.DocDate.Value.Month)
                );
            }
            else
            {
                listDocPatternDetail = listDocPatternDetail.OrderBy(x => x.SeqNo).ToList();
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryReceiveProd = qryReceiveProd.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryReceiveProd = qryReceiveProd.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryReceiveProd = qryReceiveProd.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }

            if (await qryReceiveProd.AnyAsync())
            {
                int intMaxRunning = await qryReceiveProd.MaxAsync(x => x.RunNumber.Value);
                int intRowCount = await qryReceiveProd.CountAsync();
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
                            case "Comp": strRunningDocNo += pInvReceiveProdHd.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += pInvReceiveProdHd.BrnCode; break;
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
                x => x.BrnCode == pInvReceiveProdHd.BrnCode
                && x.CompCode == pInvReceiveProdHd.CompCode
                && x.LocCode == pInvReceiveProdHd.LocCode
                && x.DocType == pInvReceiveProdHd.DocType
                && x.DocNo == strRunningDocNo
            ));
            pInvReceiveProdHd.RunNumber = intLastRunning;
            pInvReceiveProdHd.DocNo = strRunningDocNo;
        }
        private async Task addLog(InvReceiveProdHd param , string pStrStatus)
        {
            if(param == null)
            {
                return;
            }
            var log = new InvReceiveProdLog();
            log.BrnCode = param.BrnCode;
            log.CompCode = param.CompCode;
            log.CreatedBy = param.CreatedBy;
            log.CreatedDate = DateTime.Now;
            log.DocNo = param.DocNo;
            log.LocCode = param.LocCode;
            log.LogStatus = pStrStatus + "Gas";
            log.Remark = param.Remark;
            log.JsonData = JsonConvert.SerializeObject(param);
            await context.InvReceiveProdLogs.AddAsync(log);
        }
    }
}
