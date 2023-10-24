using Inventory.API.Domain.Models;
using Inventory.API.Domain.Repositories;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Services
{
    public class DeliveryControlService
    {

        private PTMaxstationContext _context = null;
        IUnitOfWork _unitOfWork = null;
        public DeliveryControlService(PTMaxstationContext pContext  , IUnitOfWork pUnitOfWork)
        {
            _context = pContext;
            _unitOfWork = pUnitOfWork;
        }


        public async Task<MasMapping[]> GetMasMapping()
        {
            var qryMapping = _context.MasMappings
                .Where(x => x.MapValue == "DeliveryCtrlWarehouse" || x.MapValue == "DeliveryCtrlCollect")
                .AsNoTracking();

            var result = await qryMapping.ToArrayAsync();

            return result;
        }

        public async Task<InvDeliveryCtrlHd> UpdateStatus(InvDeliveryCtrlHd param)
        {
            if(param == null)
            {
                return null;
            }
            var entDeliveryControl = _context.Entry(param);
            entDeliveryControl.Property(x => x.DocStatus).IsModified = true;
            entDeliveryControl.Property(x => x.UpdatedBy).IsModified = true;
            param.UpdatedDate = DateTime.Now;
            if ("Cancel".Equals(param.DocStatus))
            {
                await cancelReceive(param);
            }
            await _unitOfWork.CompleteAsync();
            return param;
        }

        public async Task<ModelDeliveryControl> GetDeliveryControl(string pStrGuid)
        {
            pStrGuid = (pStrGuid ?? string.Empty).Trim();
            if (0.Equals(pStrGuid))
            {
                return null;
            }
            Guid guid;
            if(!Guid.TryParse(pStrGuid , out guid))
            {
                return null;
            }
            var qryDeliveryControl = _context.InvDeliveryCtrlHds
                .Where(x => x.Guid == guid)
                .AsNoTracking();
            var deliveryHeader = await qryDeliveryControl.FirstOrDefaultAsync();
            InvDeliveryCtrlDt[] arrDeliveryDetail = null;

            if(deliveryHeader != null)
            {
                var qryDt = _context.InvDeliveryCtrlDts.Where(
                    x => x.DocNo == deliveryHeader.DocNo
                    && x.CompCode == deliveryHeader.CompCode
                    && x.BrnCode == deliveryHeader.BrnCode
                    && x.LocCode == deliveryHeader.LocCode
                ).AsNoTracking();
                arrDeliveryDetail = await qryDt.ToArrayAsync();
            }

            ModelDeliveryControl result = new ModelDeliveryControl()
            {
                Header = deliveryHeader,
                ArrDetail = arrDeliveryDetail
            };

            return result ;
        }

        public async Task<ModelDeliveryControl> SaveDeliveryControl(ModelDeliveryControl param)
        {            
            if (param == null || param.Header == null)
            {
                return null;
            }
            if(param.Header.DocStatus == "New")
            {
                await adjustHeaderRunningNo(param.Header);
                param.Header.Guid = Guid.NewGuid();
                param.Header.DocStatus = "Active";
                param.Header.CreatedDate = DateTime.Now;
                await _context.AddAsync(param.Header);
                await updateReceive(param.Header);
            }
            else
            {

                var entDeliveryControlHd = _context.Update(param.Header);
                param.Header.UpdatedDate = DateTime.Now;

                string[] arrNotUpdateField = null;
                arrNotUpdateField = new[] {
                    "DocDate", "RunNumber" , "DocPattern" ,
                    "Guid" , "CreatedDate" , "CreatedBy" , "ReceiveNo"
                };
                foreach (var item in arrNotUpdateField)
                {
                    entDeliveryControlHd.Property(item).IsModified = false;
                }
            }

            var qryDeleteDetail = _context.InvDeliveryCtrlDts.Where(
                x => x.DocNo == param.Header.DocNo
                && x.CompCode == param.Header.CompCode
                && x.BrnCode == param.Header.BrnCode
                && x.LocCode == param.Header.LocCode
            ).AsNoTracking();
            _context.RemoveRange(qryDeleteDetail);
            if(param.ArrDetail != null && param.ArrDetail.Length > 0)
            {
                foreach (var dt in param.ArrDetail)
                {
                    dt.BrnCode = param.Header.BrnCode;
                    dt.CompCode = param.Header.CompCode;
                    dt.LocCode = param.Header.LocCode;
                    dt.DocNo = param.Header.DocNo;
                }
                await _context.AddRangeAsync(param.ArrDetail);
            }
            await _unitOfWork.CompleteAsync();
            return param;
        }

        public async Task<ModelResultSearchDelivery> SearchDelivery(ModelParamSearchDelivery param)
        {            
            if(param == null)
            {
                return null;
            }
            var result = new ModelResultSearchDelivery();
            IQueryable<InvDeliveryCtrlHd> qryDelivery = _context.InvDeliveryCtrlHds
                .AsNoTracking()
                .OrderByDescending(x=> x.DocNo);

            param.BrnCode = (param.BrnCode ?? string.Empty).Trim();
            if(param.BrnCode.Length > 0)
            {
                qryDelivery = qryDelivery.Where(x => x.BrnCode == param.BrnCode);
            }
            param.CompCode = (param.CompCode ?? string.Empty).Trim();
            if(param.CompCode.Length > 0)
            {
                qryDelivery = qryDelivery.Where(x => x.CompCode == param.CompCode);
            }
            param.LocCode = (param.LocCode ?? string.Empty).Trim();
            if(param.LocCode.Length > 0)
            {
                qryDelivery = qryDelivery.Where(x => x.LocCode == param.LocCode);
            }
            if (param.FromDate.HasValue)
            {
                qryDelivery = qryDelivery.Where(x => x.DocDate >= param.FromDate.Value);
            }
            if (param.ToDate.HasValue)
            {
                qryDelivery = qryDelivery.Where(x => x.DocDate <= param.ToDate.Value);
            }
            param.Keyword = (param.Keyword ?? string.Empty).Trim();
            if(param.Keyword.Length > 0)
            {
                qryDelivery = qryDelivery.Where(x => x.DocNo.Contains(param.Keyword));
            }
            result.TotalItems = await qryDelivery.CountAsync();
            if(result.TotalItems <= 0)
            {
                return result;
            }
            if (param.ItemsPerPage > 0 && param.Page > 0)
            {
                qryDelivery = qryDelivery
                    .Skip((param.Page - 1) * param.ItemsPerPage)
                    .Take(param.ItemsPerPage);
            }
            result.Items = await qryDelivery.ToArrayAsync();
            return result;
        }

        public async Task<InvReceiveProdHd[]> SearchReceive(ModelParamSearchReceive param)
        {
            
            if(param == null)
            {
                return null;
            }
            var qryReceive = _context.InvReceiveProdHds.AsNoTracking()
                .Where(x => x.DocStatus != "Cancel" && string.IsNullOrEmpty(x.DeliveryNo));
            if(param.ArrPotypeId != null && param.ArrPotypeId.Length > 0)
            {
                qryReceive = qryReceive.Where(x => param.ArrPotypeId.Contains(x.PoTypeId));
            }
            param.BrnCode = (param.BrnCode ?? string.Empty).Trim();
            if(param.BrnCode.Length > 0)
            {
                qryReceive = qryReceive.Where(x => x.BrnCode == param.BrnCode);
            }
            param.CompCode = (param.CompCode ?? string.Empty).Trim();
            if(param.CompCode.Length > 0)
            {
                qryReceive = qryReceive.Where(x => x.CompCode == param.CompCode);
            }
            if(param.FromDate != null && param.FromDate.HasValue)
            {
                qryReceive = qryReceive.Where(x => x.DocDate >= param.FromDate);
            }
            if(param.ToDate != null && param.ToDate.HasValue)
            {
                qryReceive = qryReceive.Where(x => x.DocDate <= param.ToDate);
            }
            param.LocCode = (param.LocCode ?? string.Empty).Trim();
            if(param.LocCode.Length > 0)
            {
                qryReceive = qryReceive.Where(x => x.LocCode == param.LocCode);
            }
            param.Keyword = (param.Keyword ?? string.Empty).Trim();
            if(param.Keyword.Length > 0)
            {
                qryReceive = qryReceive.Where(x => x.DocNo.Contains(param.Keyword) || param.Keyword.Contains(x.DocNo));
            }
            var result = await qryReceive.ToArrayAsync();
            return result;
        }

        public async Task<MasProduct[]> GetProducts()
        {
            var qryProduct = _context.MasProducts.Where(
                x => x.PdStatus == "Active" 
                && x.GroupId == "0000"
            ).AsNoTracking();
            var result = await qryProduct.ToArrayAsync();
            return result;
        }

        private async Task adjustHeaderRunningNo(InvDeliveryCtrlHd param)
        {
            string strRunningDocNo = string.Empty;
            IQueryable<MasDocPatternDt> qryDocPattern = null;
            qryDocPattern = from dp in _context.MasDocPatterns.AsNoTracking()
                            join dt in _context.MasDocPatternDts.AsNoTracking()
                            on dp.DocId equals dt.DocId
                            where dp.DocType == "DeliveryCtrl"
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
            if (param.DocDate != null && param.DocDate.HasValue)
            {
                intDay = param.DocDate.Value.Day;
                intMonth = param.DocDate.Value.Month;
                intYear = param.DocDate.Value.Year;
            }
            else
            {
                intDay = DateTime.Now.Day;
                intMonth = DateTime.Now.Month;
                intYear = DateTime.Now.Year;
            }
            IQueryable<InvDeliveryCtrlHd> qryDeliveryControl = null;
            qryDeliveryControl = _context.InvDeliveryCtrlHds.Where(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.LocCode == param.LocCode
                && x.DocDate.HasValue
                && x.RunNumber.HasValue
            );
            if (isUseDefaultPattern)
            {
                qryDeliveryControl = qryDeliveryControl.Where(
                    x => intYear.Equals(x.DocDate.Value.Year)
                    && intMonth.Equals(x.DocDate.Value.Month)
                );
            }
            else
            {
                listDocPatternDetail = listDocPatternDetail.OrderBy(x => x.SeqNo).ToList();
                if (listDocPatternDetail.Any(x => "yy".Equals(x.DocCode) || "yyyy".Equals(x.DocCode)))
                {
                    qryDeliveryControl = qryDeliveryControl.Where(x => intYear.Equals(x.DocDate.Value.Year));
                    if (listDocPatternDetail.Any(x => "MM".Equals(x.DocCode)))
                    {
                        qryDeliveryControl = qryDeliveryControl.Where(x => intMonth.Equals(x.DocDate.Value.Month));
                        if (listDocPatternDetail.Any(x => "dd".Equals(x.DocCode)))
                        {
                            qryDeliveryControl = qryDeliveryControl.Where(x => intDay.Equals(x.DocDate.Value.Day));
                        }
                    }
                }
            }

            if (await qryDeliveryControl.AnyAsync())
            {
                int intMaxRunning = await qryDeliveryControl.MaxAsync(x => x.RunNumber.Value);
                int intRowCount = await qryDeliveryControl.CountAsync();
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
                            case "Comp": strRunningDocNo += param.CompCode; break;
                            case "[Pre]": strRunningDocNo += item.DocValue; break;
                            case "dd": strRunningDocNo += intDay.ToString("00"); break;
                            case "Brn": strRunningDocNo += param.BrnCode; break;
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

            } while (await _context.InvDeliveryCtrlHds.AnyAsync(
                x => x.BrnCode == param.BrnCode
                && x.CompCode == param.CompCode
                && x.LocCode == param.LocCode
                && x.DocNo == strRunningDocNo
            ));
            param.RunNumber = intLastRunning;
            param.DocNo = strRunningDocNo;
        }


        private void updateReceive2(InvDeliveryCtrlHd param)
        {
            if (param == null)
            {
                return;
            }
            InvReceiveProdHd receiveHeader = new InvReceiveProdHd()
            {
                BrnCode = param.BrnCode,
                LocCode = param.LocCode,
                CompCode = param.CompCode,
                DocNo = param.ReceiveNo,
                DeliveryNo = param.DocNo,
            };

            var entReceiveHeader = _context.Attach(receiveHeader);
            entReceiveHeader.Property(x => x.DeliveryNo).IsModified = true;
        }

        private async Task updateReceive(InvDeliveryCtrlHd param)
        {
            if(param == null)
            {
                return;
            }
            var qryReceive = _context.InvReceiveProdHds.Where(
                x => x.BrnCode == param.BrnCode
                && x.LocCode == param.LocCode
                && x.CompCode == param.CompCode
                && x.DocNo == param.ReceiveNo
            );
            var receive = await qryReceive.FirstOrDefaultAsync();
            if(receive != null)
            {
                receive.DeliveryNo = param.DocNo;
            }

        }

        //private void cancelReceive(InvDeliveryCtrlHd param)
        //{
        //    if (param == null)
        //    {
        //        return;
        //    }
        //    InvReceiveProdHd receiveHeader = new InvReceiveProdHd()
        //    {
        //        BrnCode = param.BrnCode,
        //        LocCode = param.LocCode,
        //        CompCode = param.CompCode,
        //        DocNo = param.ReceiveNo,
        //        DeliveryNo = string.Empty,
        //    };

        //    var entReceiveHeader = _context.Attach(receiveHeader);
        //    entReceiveHeader.Property(x => x.DeliveryNo).IsModified = true;
        //}


        private async Task cancelReceive(InvDeliveryCtrlHd param)
        {
            if (param == null)
            {
                return;
            }
            var qryReceive = _context.InvReceiveProdHds.Where(
                x => x.BrnCode == param.BrnCode
                && x.LocCode == param.LocCode
                && x.CompCode == param.CompCode
                && x.DocNo == param.ReceiveNo
            );            
            var receive = await qryReceive.FirstOrDefaultAsync();
            if (receive != null)
            {
                receive.DeliveryNo = string.Empty;
            }

        }

    }
}
