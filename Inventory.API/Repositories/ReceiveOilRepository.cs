using Abp.Linq.Expressions;
using Inventory.API.Domain.Models;
using Inventory.API.Domain.Models.Queries;
using Inventory.API.Domain.Repositories;
using Inventory.API.Helpers;
using Inventory.API.Resources.ReceiveOil;
using Inventory.API.Services;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inventory.API.Repositories
{
    public class ReceiveOilRepository : SqlDataAccessHelper, IReceiveOilRepository
    {
        private const string _strNew = "New";
        private const string _strActive = "Active";
        public ReceiveOilRepository(PTMaxstationContext context) : base(context) { }

        public async Task<QueryResult<InvReceiveProdHd>> ReceiveOilHdListAsync(ReceiveOilHdQuery query)
        {
            if(query == null)
            {
                return null;
            }
            List<string> listDocType = new List<string>()
            {
                "Oil",
                "Nonoil"
            };
            
            var queryable = context.InvReceiveProdHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && listDocType.Contains(x.DocType))
                .OrderByDescending(x => x.CompCode).ThenByDescending(x => x.BrnCode).ThenByDescending(x => x.LocCode).ThenByDescending(x => x.DocNo)
                .AsNoTracking();
            query.Keyword = DefaultService.GetString(query.Keyword);
            if (!0.Equals(query.Keyword.Length))
            {
                bool isDecimal = decimal.TryParse(query.Keyword, out decimal decim);

                ExpressionStarter<InvReceiveProdHd> esReceiveProdHD = null;
                esReceiveProdHD = PredicateBuilder.New<InvReceiveProdHd>(
                    p => p.DocNo.Contains(query.Keyword)
                    || p.InvNo.Contains(query.Keyword)
                    || p.PoNo.Contains(query.Keyword)
                    || p.SupCode.Contains(query.Keyword)
                    || p.SupName.Contains(query.Keyword)
                    || p.DocStatus.Contains(query.Keyword)
                    || p.DocType.Contains(query.Keyword)
                );                

                if (isDecimal)
                {
                    esReceiveProdHD = esReceiveProdHD.Or(x => decim.Equals(x.NetAmt));                    
                }
                List<string> listDocStatus = null;
                listDocStatus = DefaultService.GetListDocStatusFromKeyWord(query.Keyword);
                if(listDocStatus != null && listDocStatus.Any())
                {
                    esReceiveProdHD = esReceiveProdHD.Or(x=> listDocStatus.Contains(x.DocStatus));
                }                
                queryable = queryable.Where(esReceiveProdHD);
            }
            if (query.FromDate.HasValue)
            {
                queryable = queryable.Where(p => p.DocDate >= query.FromDate);
            }
            if (query.ToDate.HasValue)
            {
                queryable = queryable.Where(p => p.DocDate <= query.ToDate);
            }
            
            

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<InvReceiveProdHd>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }

        /*
         public async Task<QueryResult<InvReceiveProdHd>> ReceiveOilHdListAsync(ReceiveOilHdQuery query)
        {
            List<string> listDocType = new List<string>()
            {
                "Oil",
                "Nonoil"
            };

            var queryable = context.InvReceiveProdHds
                .Where(x => x.CompCode == query.CompCode && x.BrnCode == query.BrnCode && listDocType.Contains(x.DocType))
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                var isDecimal = decimal.TryParse(query.Keyword, out decimal decim);
                Regex rx = new Regex(@"([A-Za-z])\w");

                queryable = queryable.Where(p =>
                p.DocNo.Contains(query.Keyword)
                || p.InvNo.Contains(query.Keyword)
                || p.PoNo.Contains(query.Keyword)
                || p.SupCode.Contains(query.Keyword)
                || p.SupName.Contains(query.Keyword)
                || p.DocStatus.Contains(query.Keyword)
                || p.DocStatus.Contains(!rx.IsMatch(query.Keyword) ? MapDocStatusTHToEN(query.Keyword) : query.Keyword)
                || p.DocType.Contains(query.Keyword)
                || (isDecimal && (p.NetAmt.Equals(decim)))
                );
            }
            else if (query.FromDate != null && query.FromDate != DateTime.MinValue && query.ToDate != null && query.ToDate != DateTime.MinValue)
            {
                queryable = queryable.Where(p => p.DocDate >= query.FromDate && p.DocDate <= query.ToDate);
            }

            int totalItems = await queryable.CountAsync();
            var resp = await queryable.Skip((query.Page - 1) * query.ItemsPerPage)
                                           .Take(query.ItemsPerPage)
                                           .ToListAsync();

            return new QueryResult<InvReceiveProdHd>
            {
                Items = resp,
                TotalItems = totalItems,
                ItemsPerPage = query.ItemsPerPage
            };
        }
         */

        public async Task<ReceiveOil> FindByIdAsync(Guid guid)
        {
            var response = new ReceiveOil();
            var receiveOilHd = await context.InvReceiveProdHds.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == guid);

            if (receiveOilHd != null)
            {
                var receiveOilDts = context.InvReceiveProdDts.AsNoTracking().Where(x => x.CompCode == receiveOilHd.CompCode && x.BrnCode == receiveOilHd.BrnCode && x.DocNo == receiveOilHd.DocNo).OrderBy(y => y.SeqNo).ToList();
                response.InvReceiveProdHd = receiveOilHd;
                response.InvReceiveProdDts = receiveOilDts;
            }

            return response;
        }

        public async Task AddReceiveOil(SaveReceiveOilResource saveReceiveOil)
        {
            DateTime? duedate = saveReceiveOil.InvReceiveProdHd.DueDate;
            var supply = this.context.MasSuppliers.Where(x => x.SupCode == saveReceiveOil.InvReceiveProdHd.SupCode).FirstOrDefault();
            if (supply != null)
            {
                duedate = saveReceiveOil.InvReceiveProdHd.DocDate.Value.AddDays(supply.CreditTerm.Value);
            }

            string strDocStatus = saveReceiveOil?.InvReceiveProdHd?.DocStatus ?? string.Empty;
            if (saveReceiveOil.InvReceiveProdHd.DocStatus != null && saveReceiveOil.InvReceiveProdHd.DocStatus.Equals(_strNew))
            {
                await adjustHeaderRunningNo(saveReceiveOil.InvReceiveProdHd);
                saveReceiveOil.InvReceiveProdHd.CreatedDate = DateTime.Now;
                saveReceiveOil.InvReceiveProdHd.Guid = Guid.NewGuid();
                saveReceiveOil.InvReceiveProdHd.DocStatus = _strActive;
                saveReceiveOil.InvReceiveProdHd.Source = "SAP";
                saveReceiveOil.InvReceiveProdHd.DueDate = saveReceiveOil.InvReceiveProdHd.DueDate ?? duedate;
                saveReceiveOil.InvReceiveProdHd.DocNo = saveReceiveOil.InvReceiveProdHd.DocNo.ToUpper();
                saveReceiveOil.InvReceiveProdHd.BrnCode = saveReceiveOil.InvReceiveProdHd.BrnCode.ToUpper();
                await context.InvReceiveProdHds.AddAsync(saveReceiveOil.InvReceiveProdHd);
            }
            else
            {
                saveReceiveOil.InvReceiveProdHd.UpdatedDate = DateTime.Now;
                context.InvReceiveProdHds.Update(saveReceiveOil.InvReceiveProdHd);
            }

            var receiveOilDts = context.InvReceiveProdDts.Where(
                x => x.BrnCode == saveReceiveOil.InvReceiveProdHd.BrnCode
                && x.CompCode == saveReceiveOil.InvReceiveProdHd.CompCode
                && x.DocNo == saveReceiveOil.InvReceiveProdHd.DocNo
                && x.DocType == saveReceiveOil.InvReceiveProdHd.DocType
                && x.LocCode == saveReceiveOil.InvReceiveProdHd.LocCode
            );

            context.InvReceiveProdDts.RemoveRange(receiveOilDts);


            foreach(var receiveDt in saveReceiveOil.InvReceiveProdDts)
            {
                receiveDt.BrnCode = saveReceiveOil.InvReceiveProdHd.BrnCode.ToUpper();
                receiveDt.CompCode = saveReceiveOil.InvReceiveProdHd.CompCode;
                receiveDt.LocCode = saveReceiveOil.InvReceiveProdHd.LocCode;
                receiveDt.DocNo = saveReceiveOil.InvReceiveProdHd.DocNo.ToUpper();
                receiveDt.DocType = saveReceiveOil.InvReceiveProdHd.DocType;
                //receiveDt.SeqNo = intSeqNo++; 

                while (saveReceiveOil.InvReceiveProdDts.Count(x => x.SeqNo == receiveDt.SeqNo) > 1)
                {
                    receiveDt.SeqNo += 10;
                }

                //var masUnit = context.MasUnits.FirstOrDefault(u => u.UnitId == receiveDt.UnitId);              
                //if(masUnit != null)
                //{
                //    var masProductunit = context.MasProductUnits.FirstOrDefault(p => p.UnitId == masUnit.UnitId && p.PdId == receiveDt.PdId);
                //    receiveDt.UnitId = masUnit != null ? masUnit.UnitId : string.Empty;
                //    receiveDt.UnitName = masUnit != null ? masUnit.UnitName : string.Empty;
                //    receiveDt.UnitBarcode = masProductunit != null ? masProductunit.UnitBarcode : string.Empty;
                //}
            }

            context.InvReceiveProdDts.AddRange(saveReceiveOil.InvReceiveProdDts);

            foreach (var receiveItem in saveReceiveOil.InvReceiveProdDts)
            {
                var poItem = context.InfPoItems.FirstOrDefault(p => p.PoNumber == saveReceiveOil.InvReceiveProdHd.PoNo && p.Material == receiveItem.PdId);

                if(poItem != null )
                {
                    poItem.ReceiveQty = receiveItem.ItemQty;
                    poItem.ReceiveFloor = receiveItem.StockQty;
                    poItem.ReceiveUpdate = DateTime.Now;
                    //context.InfPoItems.Update(poItem);
                }
            }

            var poHd = context.InfPoHeaders.FirstOrDefault(h => h.PoNumber == saveReceiveOil.InvReceiveProdHd.PoNo);

            if(poHd != null)
            {
                poHd.ReceiveStatus = "Y";
                poHd.ReceiveUpdate = DateTime.Now;
                poHd.ReceiveBrnCode = saveReceiveOil.InvReceiveProdHd.BrnCode;
                poHd.ReceiveLocCode = saveReceiveOil.InvReceiveProdHd.LocCode;
                poHd.ReceiveDocNo = saveReceiveOil.InvReceiveProdHd.DocNo;
                context.InfPoHeaders.Update(poHd);
            }
            await addLog(saveReceiveOil.InvReceiveProdHd, strDocStatus);
        }

        public async Task UpdateStatus(SaveReceiveOilResource saveReceiveOil)
        {
            var receiveOilHd = context.InvReceiveProdHds.FirstOrDefault(s => s.DocNo == saveReceiveOil.InvReceiveProdHd.DocNo);

            if (receiveOilHd != null)
            {
                receiveOilHd.DocStatus = saveReceiveOil.InvReceiveProdHd.DocStatus;
                receiveOilHd.UpdatedDate = DateTime.Now;
                context.InvReceiveProdHds.Update(receiveOilHd);

                var poHd = context.InfPoHeaders.FirstOrDefault(p => p.PoNumber == receiveOilHd.PoNo);
                
                if (poHd != null)
                {
                    poHd.ReceiveStatus = "N";
                    context.InfPoHeaders.Update(poHd);
                }

                var receiveOilDts = context.InvReceiveProdDts.Where(d => d.DocNo == receiveOilHd.DocNo).ToList();
                if (receiveOilDts != null)
                {
                    foreach (var receiveOilDt in receiveOilDts)
                    {
                        var poItem = context.InfPoItems.FirstOrDefault(p => p.PoNumber == receiveOilHd.PoNo && p.Material == receiveOilDt.PdId);

                        var receiveQty = poItem.ReceiveQty - receiveOilDt.ItemQty;
                        var stockQty = poItem.ReceiveFloor - receiveOilDt.StockQty;
                        poItem.ReceiveQty = receiveQty;
                        poItem.ReceiveFloor = stockQty;
                        context.InfPoItems.Update(poItem);
                    }
                }
                await addLog(receiveOilHd, receiveOilHd.DocStatus);
            }
        }

        public async Task<bool> CheckReceiveStatus(SaveReceiveOilResource saveReceiveOil)
        {
            var receive = await context.InfPoHeaders.FirstOrDefaultAsync(x => x.PoNumber == saveReceiveOil.InvReceiveProdHd.PoNo && x.ReceiveStatus == "Y");

            if(receive != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task adjustHeaderRunningNo(InvReceiveProdHd pInvReceiveProdHd)
        {
            string strRunningDocNo = string.Empty;
            var qryDocPattern = (from dp in context.MasDocPatterns
                                 join dt in context.MasDocPatternDts on dp.DocId equals dt.DocId
                                 where dp.DocType == "ReceiveOil"
                                 select new MasDocPatternDt()
                                 {
                                     DocValue = dt.DocValue,
                                     DocCode = dt.DocCode,
                                     SeqNo = dt.SeqNo
                                 });

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
        private async Task addLog(InvReceiveProdHd param, string pStrStatus)
        {
            if (param == null)
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
            log.LogStatus = pStrStatus + "Oil";
            log.Remark = param.Remark;
            log.JsonData = JsonConvert.SerializeObject(param);
            await context.InvReceiveProdLogs.AddAsync(log);
        }
    }
}
