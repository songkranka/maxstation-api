using AutoMapper;
using DailyOperation.API.Domain.Models;
using DailyOperation.API.Domain.Models.Meter;
using DailyOperation.API.Domain.Models.Queries;
using DailyOperation.API.Domain.Repositories;
using DailyOperation.API.Helpers;
using DailyOperation.API.Resources.POS;
using DailyOperation.API.Domain.Services.Communication;
using MaxStation.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Cash = DailyOperation.API.Domain.Models.Meter.Cash;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using log4net;
using DailyOperation.API.Resources.Meter;
using DailyOperation.API.Services;
using Newtonsoft.Json;

namespace DailyOperation.API.Repositories
{
    public class MeterRepository : SqlDataAccessHelper, IMeterRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private static readonly ILog log = LogManager.GetLogger(typeof(MeterRepository));
        private readonly string schema = "raptorpos.";//raptorpos.

        public MeterRepository(
            POSConnection posConnect,
            PTMaxstationContext context,
            IMapper mapper,
            IUnitOfWork unitOfWork) : base(context)
        {
            _connectionString = posConnect.ConnectionString;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MeterResponse> SubmitDocument(SaveDocumentRequest req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var periodStatus = "Active";

                //check previous period 
                if (req.PeriodNo > 1)
                {
                    var previousPeriodMeter = await context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == (req.PeriodNo - 1)).AsNoTracking().FirstOrDefaultAsync();
                    if (previousPeriodMeter != null)
                    {
                        if (previousPeriodMeter.PeriodStatus != "Active")
                        {
                            return new MeterResponse()
                            {
                                StatusCode = StatusCodes.Status200OK,
                                Status = "Fail",
                                Message = "ไม่สามารถ Submit กะปัจจุบันได้กรุณา Submit กะก่อนหน้าก่อน"
                            };
                        }
                    }

                    //var previousPeriod = await context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == (req.PeriodNo - 1)).AsNoTracking().FirstOrDefaultAsync();
                    //var currentStartTime = TimeSpan.Parse(req.Meter.PeriodStart);
                    //var previousFinishTime = TimeSpan.Parse(previousPeriod.TimeFinish);
                    //if (TimeSpan.Compare(previousFinishTime, currentStartTime) >= 1) 
                    //{
                    //    return new MeterResponse()
                    //    {
                    //        StatusCode = StatusCodes.Status200OK,
                    //        Status = "Fail",
                    //        Message = "ไม่สามารถ Submit กะปัจจุบันได้เนื่องจากเวลาเริ่มกะปัจจุบัน จะต้องไม่น้อยกว่าเวลาสิ้นสุดของกะก่อนหน้า"
                    //    };
                    //}

                }


                //check old data meter
                var listOldMeterData = await context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var oldPeriodData = await context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().FirstOrDefaultAsync();
                var listOldEmpData = await context.DopPeriodEmps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();

                //check old data tank
                var listOldTankData = await context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var listOldSumTankData = await context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();

                //check old data cash
                var listOldCashData = await context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var listOldSumCashData = await context.DopPeriodCashSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var listOldCashDataGl = await context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();

                //insert dop meter
                var listInsertMeter = new List<DopPeriodMeter>();
                foreach (var item in req.Meter.Items)
                {
                    var insertMeter = _mapper.Map<MasBranchDisp, DopPeriodMeter>(item);
                    insertMeter.CompCode = req.CompCode;
                    insertMeter.BrnCode = req.BrnCode;
                    insertMeter.CreatedBy = req.User;
                    insertMeter.CreatedDate = DateTime.Now;
                    insertMeter.DocDate = docDate;
                    insertMeter.PeriodNo = req.PeriodNo;
                    insertMeter.PeriodStatus = periodStatus;
                    insertMeter.SaleAmt = Math.Round(insertMeter.SaleAmt ?? 0, 2);
                    insertMeter.SaleQty = Math.Round(insertMeter.SaleQty ?? 0, 2);
                    insertMeter.TotalQty = Math.Round(insertMeter.TotalQty ?? 0, 2);
                    listInsertMeter.Add(insertMeter);
                }

                //insert dop tank
                var listInsertTank = new List<DopPeriodTank>();
                foreach (var item in req.Tank.TankItems)
                {
                    var insertTank = _mapper.Map<MasBranchTank, DopPeriodTank>(item);
                    insertTank.CompCode = req.CompCode;
                    insertTank.BrnCode = req.BrnCode;
                    insertTank.CreatedBy = req.User;
                    insertTank.CreatedDate = DateTime.Now;
                    insertTank.DocDate = docDate;
                    insertTank.PeriodNo = req.PeriodNo;
                    insertTank.PeriodStatus = periodStatus;
                    insertTank.RemainQty = Math.Round(insertTank.RemainQty ?? 0, 2);
                    insertTank.DiffQty = Math.Round(insertTank.DiffQty ?? 0, 2);
                    listInsertTank.Add(insertTank);
                }

                //insert dop sum tank
                var listInsertSumTank = new List<DopPeriodTankSum>();
                foreach (var item in req.Tank.SumTankItems)
                {
                    item.CompCode = req.CompCode;
                    item.BrnCode = req.BrnCode;
                    item.CreatedBy = req.User;
                    item.CreatedDate = DateTime.Now;
                    item.DocDate = docDate;
                    item.PeriodNo = req.PeriodNo;
                    item.SaleQty = Math.Round(item.SaleQty ?? 0, 2);
                    item.IssueQty = Math.Round(item.IssueQty ?? 0, 2);
                    item.AdjustQty = Math.Round(item.AdjustQty ?? 0, 2);
                    listInsertSumTank.Add(item);
                }

                //insert dop cash
                var listInsertCash = new List<DopPeriodCash>();
                foreach (var item in req.Cash.CashItems)
                {
                    item.CompCode = req.CompCode;
                    item.BrnCode = req.BrnCode;
                    item.CreatedBy = req.User;
                    item.CreatedDate = DateTime.Now;
                    item.DocDate = docDate;
                    item.PeriodNo = req.PeriodNo;
                    item.CashAmt = Math.Round(item.CashAmt ?? 0, 2);
                    item.TotalAmt = Math.Round(item.TotalAmt ?? 0, 2);
                    listInsertCash.Add(item);
                }

                //insert dop sum cash
                req.Cash.SumCashItems.CompCode = req.CompCode;
                req.Cash.SumCashItems.BrnCode = req.BrnCode;
                req.Cash.SumCashItems.CreatedBy = req.User;
                req.Cash.SumCashItems.CreatedDate = DateTime.Now;
                req.Cash.SumCashItems.DocDate = docDate;
                req.Cash.SumCashItems.PeriodNo = req.PeriodNo;

                //insert dop cash gl
                var listInsertCashGl = new List<DopPeriodCashGl>();
                var joinListCashGl = req.Cash.DrItems.Concat(req.Cash.CrItems).ToList();
                var indexCashGl = 1;
                foreach (var item in joinListCashGl)
                {
                    var insertCashGl = _mapper.Map<DopPeriodGl, DopPeriodCashGl>(item);
                    insertCashGl.CompCode = req.CompCode;
                    insertCashGl.BrnCode = req.BrnCode;
                    insertCashGl.CreatedBy = req.User;
                    insertCashGl.CreatedDate = DateTime.Now;
                    insertCashGl.DocDate = docDate;
                    insertCashGl.PeriodNo = req.PeriodNo;
                    insertCashGl.SeqNo = indexCashGl++;
                    listInsertCashGl.Add(insertCashGl);
                }

                //insert period
                var insertPeriod = new DopPeriod()
                {
                    CompCode = req.CompCode,
                    BrnCode = req.BrnCode,
                    DocDate = docDate,
                    PeriodNo = req.PeriodNo,
                    IsPos = req.IsPos,
                    TimeStart = req.Meter.PeriodStart,
                    TimeFinish = req.Meter.PeriodFinish,
                    SumMeterSaleQty = req.Meter.Items.Sum(x => Math.Round(x.SaleQty, 2)),
                    SumMeterTotalQty = req.Meter.Items.Sum(x => Math.Round(x.TotalQty, 2)),
                    Post = "N",
                    CreatedBy = req.User,
                    CreatedDate = DateTime.Now
                };


                //insert emp
                var listReqEmpCode = new List<string>();
                foreach (var item in req.Meter.Employee)
                {
                    listReqEmpCode.Add(item);
                }

                var listEmp = await context.MasEmployees.Where(x => listReqEmpCode.Contains(x.EmpCode)).AsNoTracking().ToListAsync();

                var indexEmp = 1;
                var listInsertEmp = new List<DopPeriodEmp>();
                foreach (var item in listEmp)
                {
                    var insertEmp = new DopPeriodEmp()
                    {
                        CompCode = req.CompCode,
                        BrnCode = req.BrnCode,
                        DocDate = docDate,
                        PeriodNo = req.PeriodNo,
                        SeqNo = indexEmp++,
                        EmpCode = item.EmpCode,
                        EmpName = item.EmpName
                    };
                    listInsertEmp.Add(insertEmp);
                }

                //remove old data
                context.DopPeriodMeters.RemoveRange(listOldMeterData);
                context.DopPeriodTanks.RemoveRange(listOldTankData);
                context.DopPeriodTankSums.RemoveRange(listOldSumTankData);
                context.DopPeriodCashes.RemoveRange(listOldCashData);
                context.DopPeriodCashSums.RemoveRange(listOldSumCashData);
                context.DopPeriodCashGls.RemoveRange(listOldCashDataGl);
                if (oldPeriodData != null)
                {
                    context.DopPeriods.Remove(oldPeriodData);
                }
                context.DopPeriodEmps.RemoveRange(listOldEmpData);

                await context.DopPeriodMeters.AddRangeAsync(listInsertMeter);
                await context.DopPeriodTanks.AddRangeAsync(listInsertTank);
                await context.DopPeriodTankSums.AddRangeAsync(listInsertSumTank);
                await context.DopPeriodCashes.AddRangeAsync(listInsertCash);
                await context.DopPeriodCashGls.AddRangeAsync(listInsertCashGl);
                await context.DopPeriods.AddAsync(insertPeriod);
                await context.DopPeriodCashSums.AddAsync(req.Cash.SumCashItems);
                await context.DopPeriodEmps.AddRangeAsync(listInsertEmp);
                await _unitOfWork.CompleteAsync();

                return new MeterResponse()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Status = "Success",
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                log.Error("An error occurred when submit document meter", ex);
                throw new Exception($"An error occurred when submit document meter : {ex.Message}");
            }
        }

        public async Task<MeterResponse> SaveDocument(SaveDocumentRequest req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var periodStatus = "Draft";

                //check old data meter
                var listOldMeterData = await context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var oldPeriodData = await context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().FirstOrDefaultAsync();
                var listOldEmpData = await context.DopPeriodEmps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();

                //check old data tank
                var listOldTankData = await context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var listOldSumTankData = await context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();

                //check old data cash
                var listOldCashData = await context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var listOldSumCashData = await context.DopPeriodCashSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var listOldCashDataGl = await context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();

                //insert dop meter
                var listInsertMeter = new List<DopPeriodMeter>();
                foreach (var item in req.Meter.Items)
                {
                    var insertMeter = _mapper.Map<MasBranchDisp, DopPeriodMeter>(item);
                    insertMeter.CompCode = req.CompCode;
                    insertMeter.BrnCode = req.BrnCode;
                    insertMeter.CreatedBy = req.User;
                    insertMeter.CreatedDate = DateTime.Now;
                    insertMeter.DocDate = docDate;
                    insertMeter.PeriodNo = req.PeriodNo;
                    insertMeter.PeriodStatus = periodStatus;
                    listInsertMeter.Add(insertMeter);
                }

                //insert dop tank
                var listInsertTank = new List<DopPeriodTank>();
                foreach (var item in req.Tank.TankItems)
                {
                    var insertTank = _mapper.Map<MasBranchTank, DopPeriodTank>(item);
                    insertTank.CompCode = req.CompCode;
                    insertTank.BrnCode = req.BrnCode;
                    insertTank.CreatedBy = req.User;
                    insertTank.CreatedDate = DateTime.Now;
                    insertTank.DocDate = docDate;
                    insertTank.PeriodNo = req.PeriodNo;
                    insertTank.PeriodStatus = periodStatus;
                    listInsertTank.Add(insertTank);
                }

                //insert dop sum tank
                var listInsertSumTank = new List<DopPeriodTankSum>();
                foreach (var item in req.Tank.SumTankItems)
                {
                    item.CompCode = req.CompCode;
                    item.BrnCode = req.BrnCode;
                    item.CreatedBy = req.User;
                    item.CreatedDate = DateTime.Now;
                    item.DocDate = docDate;
                    item.PeriodNo = req.PeriodNo;
                    listInsertSumTank.Add(item);
                }

                //insert dop cash
                var listInsertCash = new List<DopPeriodCash>();
                foreach (var item in req.Cash.CashItems)
                {
                    item.CompCode = req.CompCode;
                    item.BrnCode = req.BrnCode;
                    item.CreatedBy = req.User;
                    item.CreatedDate = DateTime.Now;
                    item.DocDate = docDate;
                    item.PeriodNo = req.PeriodNo;
                    listInsertCash.Add(item);
                }

                //insert dop sum cash
                req.Cash.SumCashItems.CompCode = req.CompCode;
                req.Cash.SumCashItems.BrnCode = req.BrnCode;
                req.Cash.SumCashItems.CreatedBy = req.User;
                req.Cash.SumCashItems.CreatedDate = DateTime.Now;
                req.Cash.SumCashItems.DocDate = docDate;
                req.Cash.SumCashItems.PeriodNo = req.PeriodNo;

                //insert dop cash gl
                var listInsertCashGl = new List<DopPeriodCashGl>();
                var joinListCashGl = req.Cash.DrItems.Concat(req.Cash.CrItems).ToList();
                var indexCashGl = 1;
                foreach (var item in joinListCashGl)
                {
                    var insertCashGl = _mapper.Map<DopPeriodGl, DopPeriodCashGl>(item);
                    insertCashGl.CompCode = req.CompCode;
                    insertCashGl.BrnCode = req.BrnCode;
                    insertCashGl.CreatedBy = req.User;
                    insertCashGl.CreatedDate = DateTime.Now;
                    insertCashGl.DocDate = docDate;
                    insertCashGl.PeriodNo = req.PeriodNo;
                    insertCashGl.SeqNo = indexCashGl++;
                    listInsertCashGl.Add(insertCashGl);
                }

                //insert period
                var insertPeriod = new DopPeriod()
                {
                    CompCode = req.CompCode,
                    BrnCode = req.BrnCode,
                    DocDate = docDate,
                    PeriodNo = req.PeriodNo,
                    IsPos = req.IsPos,
                    TimeStart = req.Meter.PeriodStart,
                    TimeFinish = req.Meter.PeriodFinish,
                    SumMeterSaleQty = req.Meter.Items.Sum(x => x.SaleQty),
                    SumMeterTotalQty = req.Meter.Items.Sum(x => x.TotalQty),
                    Post = "N",
                    CreatedBy = req.User,
                    CreatedDate = DateTime.Now
                };


                //insert emp
                var listReqEmpCode = new List<string>();
                foreach (var item in req.Meter.Employee)
                {
                    listReqEmpCode.Add(item);
                }

                var listEmp = await context.MasEmployees.Where(x => listReqEmpCode.Contains(x.EmpCode)).AsNoTracking().ToListAsync();

                var indexEmp = 1;
                var listInsertEmp = new List<DopPeriodEmp>();
                foreach (var item in listEmp)
                {
                    var insertEmp = new DopPeriodEmp()
                    {
                        CompCode = req.CompCode,
                        BrnCode = req.BrnCode,
                        DocDate = docDate,
                        PeriodNo = req.PeriodNo,
                        SeqNo = indexEmp++,
                        EmpCode = item.EmpCode,
                        EmpName = item.EmpName
                    };
                    listInsertEmp.Add(insertEmp);
                }

                //remove old data
                context.DopPeriodMeters.RemoveRange(listOldMeterData);
                context.DopPeriodTanks.RemoveRange(listOldTankData);
                context.DopPeriodTankSums.RemoveRange(listOldSumTankData);
                context.DopPeriodCashes.RemoveRange(listOldCashData);
                context.DopPeriodCashSums.RemoveRange(listOldSumCashData);
                context.DopPeriodCashGls.RemoveRange(listOldCashDataGl);
                if (oldPeriodData != null)
                {
                    context.DopPeriods.Remove(oldPeriodData);
                }
                context.DopPeriodEmps.RemoveRange(listOldEmpData);

                await context.DopPeriodMeters.AddRangeAsync(listInsertMeter);
                await context.DopPeriodTanks.AddRangeAsync(listInsertTank);
                await context.DopPeriodTankSums.AddRangeAsync(listInsertSumTank);
                await context.DopPeriodCashes.AddRangeAsync(listInsertCash);
                await context.DopPeriodCashSums.AddAsync(req.Cash.SumCashItems);
                await context.DopPeriodCashGls.AddRangeAsync(listInsertCashGl);
                await context.DopPeriods.AddAsync(insertPeriod);
                await context.DopPeriodEmps.AddRangeAsync(listInsertEmp);


                //check over period is active
                var firstInsertMeter = listInsertMeter.First();
                var listOverPeriodMeter = await context.DopPeriodMeters.Where(x => x.CompCode == firstInsertMeter.CompCode && x.BrnCode == firstInsertMeter.BrnCode && x.DocDate == docDate && x.PeriodNo > firstInsertMeter.PeriodNo).ToListAsync();
                if (listOverPeriodMeter.Count > 0)
                {
                    var listOverPeriodTank = await context.DopPeriodTanks.Where(x => x.CompCode == firstInsertMeter.CompCode && x.BrnCode == firstInsertMeter.BrnCode && x.DocDate == docDate && x.PeriodNo > firstInsertMeter.PeriodNo).ToListAsync();
                    listOverPeriodMeter.ForEach(x => { x.PeriodStatus = periodStatus; });
                    listOverPeriodTank.ForEach(x => { x.PeriodStatus = periodStatus; });

                    context.DopPeriodMeters.UpdateRange(listOverPeriodMeter);
                    context.DopPeriodTanks.UpdateRange(listOverPeriodTank);
                }

                await _unitOfWork.CompleteAsync();

                return new MeterResponse()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Status = "Success",
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                log.Error("An error occurred when save document meter", ex);
                throw new Exception($"An error occurred when save document meter : {ex.Message}");
            }
        }

        public async Task<GetResponse> GetDocument(GetDocumentRequest req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var listResult = new List<GetDocumentResponse>();

                //var data = await context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var qryData = context.DopPeriodMeters.Where(
                    x => x.CompCode == req.CompCode
                    && x.BrnCode == req.BrnCode
                    && x.DocDate == docDate
                    && x.PeriodNo == req.PeriodNo
                    && x.DispStatus != "Cancel"
                ).AsNoTracking();
                var data = await qryData.ToListAsync();
                List<MasBranchDisp> listMasBranchDisp = null;
                if (data != null && data.Any())
                {
                    var qryMasBranchDisp = context.MasBranchDisps.Where(
                        x => x.CompCode == req.CompCode
                        && x.BrnCode == req.BrnCode
                        && x.DispStatus != "Cancel"
                    ).AsNoTracking();
                    listMasBranchDisp = await qryMasBranchDisp.ToListAsync();
                }

                var groupPeriod = data.GroupBy(x => x.PeriodNo).Select(x => new
                {
                    periodNo = x.First().PeriodNo,
                    data = x.ToList()
                });

                foreach (var item in groupPeriod)
                {
                    var periodData = await context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == item.periodNo).AsNoTracking().FirstOrDefaultAsync();

                    var result = new GetDocumentResponse();
                    result.PeriodNo = item.periodNo;
                    result.Post = periodData.Post;
                    result.IsPos = periodData.IsPos;

                    //add meter data
                    var meter = new Meter();
                    var getPreviousMeter = await GetPreviousMeter(item.data);
                    meter.Items = _mapper.Map<List<DopPeriodMeter>, List<MasBranchDisp>>(getPreviousMeter);
                    if (meter.Items != null && meter.Items.Any())
                    {
                        for (int i = meter.Items.Count - 1; i >= 0; i--)
                        {
                            var mbd = meter.Items[i];
                            string strStatus = "Cancel";
                            if (listMasBranchDisp != null && listMasBranchDisp.Any())
                            {
                                strStatus = listMasBranchDisp.FirstOrDefault(x => x.DispId == mbd.DispId)?.DispStatus ?? "Cancel";
                            }
                            if (strStatus == "Cancel")
                            {
                                meter.Items.RemoveAt(i);
                            }
                            else
                            {
                                mbd.DispStatus = strStatus;
                            }
                        }
                    }
                    meter.PeriodStart = periodData.TimeStart;
                    meter.PeriodFinish = periodData.TimeFinish;
                    meter.SumMeterSaleQty = periodData.SumMeterSaleQty.HasValue ? periodData.SumMeterSaleQty.Value : 0m;
                    meter.SumMeterTotalQty = periodData.SumMeterTotalQty.HasValue ? periodData.SumMeterTotalQty.Value : 0m;
                    meter.Employee = await context.DopPeriodEmps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == item.periodNo).AsNoTracking().Select(x => $"{x.EmpCode} : {x.EmpName}").ToListAsync();
                    result.Meter = meter;

                    //add tank data
                    var tank = new Tank();
                    var listTankData = await context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == item.periodNo).AsNoTracking().ToListAsync();
                    var getPreviousTank = await GetPreviousTank(listTankData);
                    var masBranchTank = _mapper.Map<List<DopPeriodTank>, List<MasBranchTank>>(getPreviousTank);

                    //join find pd image
                    masBranchTank.ForEach(x => { x.PdImage = $"data:image/png;base64, {context.MasProducts.Where(y => y.PdId == x.PdId).Select(y => y.PdImage).AsNoTracking().FirstOrDefault()}"; });

                    var qryMasBranchTank = context.MasBranchTanks.Where(
                        x => x.CompCode == req.CompCode
                        && x.BrnCode == req.BrnCode
                    ).AsNoTracking();
                    var listMasBranchTank = await qryMasBranchTank.ToListAsync();
                    foreach (var mbt in masBranchTank)
                    {
                        string strTankStatus = "Cancel";
                        if (listMasBranchTank != null && listMasBranchTank.Any())
                        {
                            var mbt2 = listMasBranchTank.FirstOrDefault(x => x.TankId == mbt.TankId);
                            if (mbt2 != null)
                            {
                                strTankStatus = mbt2.TankStatus;
                            }
                        }
                        mbt.TankStatus = strTankStatus;
                    }

                    tank.TankItems = masBranchTank.Where(x => x.TankStatus != "Cancel").ToList();
                    result.Tank = tank;

                    //add cash data
                    var cash = new Cash();
                    var listCashItems = await context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == item.periodNo).AsNoTracking().ToListAsync();
                    var SumCashItems = await context.DopPeriodCashSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == item.periodNo).AsNoTracking().FirstOrDefaultAsync();
                    //var listDrData = await context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == item.periodNo && x.GlType == "DR").AsNoTracking().ToListAsync();
                    //var listCrData = await context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == item.periodNo && x.GlType == "CR").AsNoTracking().ToListAsync();
                    var listDrJoinData = await (from dop in context.DopPeriodCashGls
                                                join mas in context.DopPeriodGls on new { a = dop.CompCode, b = dop.BrnCode, c = dop.GlType, d = dop.GlNo } equals new { a = mas.CompCode, b = mas.BrnCode, c = mas.GlType, d = mas.GlNo }
                                                where dop.CompCode == req.CompCode && dop.BrnCode == req.BrnCode && dop.DocDate == docDate && dop.PeriodNo == item.periodNo && dop.GlType == "DR"
                                                select new { dop, mas }).AsNoTracking()
                                                //.OrderBy(x=>x.mas.GlSeqNo)
                                                .ToListAsync();
                    var listDrData = new List<DopPeriodGl>();
                    listDrJoinData.ForEach(x =>
                    {
                        var drData = _mapper.Map<DopPeriodCashGl, DopPeriodGl>(x.dop);
                        drData.GlLock = x.mas.GlLock;
                        drData.GlSlip = x.mas.GlSlip;
                        drData.GlSeqNo = x.mas.GlSeqNo;
                        listDrData.Add(drData);
                    });

                    var listCrJoinData = await (from dop in context.DopPeriodCashGls
                                                join mas in context.DopPeriodGls on new { a = dop.CompCode, b = dop.BrnCode, c = dop.GlType, d = dop.GlNo } equals new { a = mas.CompCode, b = mas.BrnCode, c = mas.GlType, d = mas.GlNo }
                                                where dop.CompCode == req.CompCode && dop.BrnCode == req.BrnCode && dop.DocDate == docDate && dop.PeriodNo == item.periodNo && dop.GlType == "CR"
                                                select new { dop, mas }).AsNoTracking()
                                                //.OrderBy(x=>x.mas.GlSeqNo)
                                                .ToListAsync();
                    var listCrData = new List<DopPeriodGl>();
                    listCrJoinData.ForEach(x =>
                    {
                        var crData = _mapper.Map<DopPeriodCashGl, DopPeriodGl>(x.dop);
                        crData.GlLock = x.mas.GlLock;
                        crData.GlSlip = x.mas.GlSlip;
                        crData.GlSeqNo = x.mas.GlSeqNo;
                        listCrData.Add(crData);
                    });

                    cash.CashItems = listCashItems;
                    cash.SumCashItems = SumCashItems;
                    cash.DrItems = listDrData;
                    cash.CrItems = listCrData;
                    result.Cash = cash;

                    listResult.Add(result);
                }

                var countAllPeriod = context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate).AsNoTracking().Select(x => x.PeriodNo).Distinct().Count();

                return new GetResponse(listResult, countAllPeriod);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                log.Error("An error occurred when get document meter", ex);
                return new GetResponse($"An error occurred when get document meter : {ex.Message}");
            }
        }

        private async Task<List<DopPeriodMeter>> GetPreviousMeter(List<DopPeriodMeter> data)
        {
            foreach (var item in data)
            {
                if (item.MeterStart <= 0)
                {
                    var meterPrevious = await context.DopPeriodMeters
                    .Where(x => x.CompCode == item.CompCode
                    && x.BrnCode == item.BrnCode
                    && x.DocDate == item.DocDate
                    && x.PeriodNo == (item.PeriodNo - 1)
                    && x.TankId == item.TankId
                    && x.DispId == item.DispId
                    && x.PdId == item.PdId
                    ).AsNoTracking().Select(x => x.MeterFinish).FirstOrDefaultAsync();

                    if (meterPrevious == null)
                    {
                        var meterPreviousYesterDay = await context.DopPeriodMeters
                        .Where(x => x.CompCode == item.CompCode
                        && x.BrnCode == item.BrnCode
                        && x.DocDate == item.DocDate.AddDays(-1)
                        && x.TankId == item.TankId
                        && x.DispId == item.DispId
                        && x.PdId == item.PdId
                        ).AsNoTracking().OrderByDescending(x => x.PeriodNo).Select(x => x.MeterFinish).FirstOrDefaultAsync();

                        if (item.MeterStart != meterPreviousYesterDay)
                        {
                            item.MeterStart = meterPreviousYesterDay;
                        }
                    }
                    else
                    {
                        if (item.MeterStart != meterPrevious)
                        {
                            item.MeterStart = meterPrevious;
                        }
                    }
                }
            }
            return data;
        }

        private async Task<List<DopPeriodTank>> GetPreviousTank(List<DopPeriodTank> data)
        {
            foreach (var item in data)
            {
                //if (item.BeforeQty <= 0) 
                //{
                //    var tankPrevious = await context.DopPeriodTanks
                //    .Where(x => x.CompCode == item.CompCode
                //    && x.BrnCode == item.BrnCode
                //    && x.DocDate == item.DocDate
                //    && x.PeriodNo == (item.PeriodNo - 1)
                //    && x.TankId == item.TankId
                //    && x.PdId == item.PdId
                //    ).AsNoTracking().Select(x => x.RealQty).FirstOrDefaultAsync();

                //    if (tankPrevious == null)
                //    {
                //        var tankPreviousYesterday = await context.DopPeriodTanks
                //        .Where(x => x.CompCode == item.CompCode
                //        && x.BrnCode == item.BrnCode
                //        && x.DocDate == item.DocDate.AddDays(-1)
                //        && x.TankId == item.TankId
                //        && x.PdId == item.PdId
                //        ).AsNoTracking().OrderByDescending(x => x.PeriodNo).Select(x => x.RealQty).FirstOrDefaultAsync();

                //        if (item.BeforeQty != tankPreviousYesterday)
                //        {
                //            item.BeforeQty = tankPreviousYesterday;
                //        }
                //    }
                //    else
                //    {
                //        if (item.BeforeQty != tankPrevious)
                //        {
                //            item.BeforeQty = tankPrevious;
                //        }
                //    }
                //}

                var tankPrevious = await context.DopPeriodTanks
                .Where(x => x.CompCode == item.CompCode
                && x.BrnCode == item.BrnCode
                && x.DocDate == item.DocDate
                && x.PeriodNo == (item.PeriodNo - 1)
                && x.TankId == item.TankId
                && x.PdId == item.PdId
                ).AsNoTracking().Select(x => x.RealQty).FirstOrDefaultAsync();

                if (tankPrevious == null)
                {
                    var tankPreviousYesterday = await context.DopPeriodTanks
                    .Where(x => x.CompCode == item.CompCode
                    && x.BrnCode == item.BrnCode
                    && x.DocDate == item.DocDate.AddDays(-1)
                    && x.TankId == item.TankId
                    && x.PdId == item.PdId
                    ).AsNoTracking().OrderByDescending(x => x.PeriodNo).Select(x => x.RealQty).FirstOrDefaultAsync();

                    if (item.BeforeQty != tankPreviousYesterday)
                    {
                        item.BeforeQty = tankPreviousYesterday;
                    }
                }
                else
                {
                    if (item.BeforeQty != tankPrevious)
                    {
                        item.BeforeQty = tankPrevious;
                    }
                }
            }
            return data;
        }

        public async Task<MeterResponse> DeleteDocument(DeleteDocumentRequest req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                //check old data
                var listOldMeterData = await context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var oldPeriodData = await context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().FirstOrDefaultAsync();
                var listOldEmpData = await context.DopPeriodEmps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();

                //check old data tank
                var listOldTankData = await context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();
                var listOldSumTankData = await context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo).AsNoTracking().ToListAsync();

                //remove old data
                context.DopPeriodMeters.RemoveRange(listOldMeterData);
                context.DopPeriodTanks.RemoveRange(listOldTankData);
                context.DopPeriodTankSums.RemoveRange(listOldSumTankData);
                if (oldPeriodData != null)
                {
                    context.DopPeriods.Remove(oldPeriodData);
                }
                context.DopPeriodEmps.RemoveRange(listOldEmpData);
                await _unitOfWork.CompleteAsync();
                return new MeterResponse()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Status = "Success",
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                log.Error("An error occurred when delete document meter", ex);
                throw new Exception($"An error occurred when delete document meter : {ex.Message}");
            }
        }

        //public QueryResult<MasBranchMeterResponse> GetPosMeter(BranchMeterRequest req)
        //{
        //    try
        //    {
        //        var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        //        var listMasBranchDisp = new List<MasBranchDisp>();
        //        var respDisp = new List<MasBranchDisp>();

        //        var queryableDisp = context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo);
        //        var query = queryableDisp.OrderBy(x => x.DispId).AsNoTracking().ToList();

        //        if (query.Count > 0)
        //        {
        //            respDisp = _mapper.Map<List<DopPeriodMeter>, List<MasBranchDisp>>(query);
        //            var queryMas = context.MasBranchDisps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).OrderBy(x => x.DispId).AsNoTracking().ToList();
        //            respDisp.ForEach(x => 
        //            {
        //                x.HoseId = queryMas.Where(y => y.DispId == x.DispId).Select(y => y.HoseId).FirstOrDefault();
        //            });
        //        }
        //        else 
        //        {
        //            respDisp = context.MasBranchDisps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).OrderBy(x => x.DispId).AsNoTracking().ToList();
        //        }

        //        var mpCreditSale = (from mas in context.MasPayGroups
        //                            join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
        //                            where mas.PayGroupName == "Credit"
        //                            select type.PayTypeId).ToList();

        //        var listGroupCashAndDisc = new List<string>() 
        //        {
        //            "Cash",
        //            "Discount"
        //        };

        //        var mpCashAndDisc = (from mas in context.MasPayGroups
        //                          join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
        //                          where listGroupCashAndDisc.Contains(mas.PayGroupName) 
        //                          select type.PayTypeId).ToList();


        //        var mpDisc = (from mas in context.MasPayGroups
        //                            join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
        //                            where mas.PayGroupName == "Discount"
        //                            select type.PayTypeId).ToList();


        //        var mpCoupon = (from mas in context.MasPayGroups
        //                      join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
        //                      where mas.PayGroupName == "Coupon"
        //                      select type.PayTypeId).ToList();


        //        var mpTest = (from mas in context.MasPayGroups
        //                        join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
        //                        where mas.PayGroupName == "Test"
        //                        select type.PayTypeId).ToList();



        //        OracleConnection con = new OracleConnection
        //        {
        //            ConnectionString = _connectionString
        //        };
        //        con.Open();

        //        OracleCommand cmd = new OracleCommand
        //        {
        //            Connection = con,
        //            CommandTimeout = 90,
        //            CommandText = $@"select * 			
        //                                from {schema}function2max 			
        //                                where SITE_ID = '52{req.BrnCode}'			
        //                                and  trunc(business_date) = to_date('{req.DocDate}','yyyy-MM-dd')			
        //                                and shift_no = {req.PeriodNo}"
        //        };

        //        OracleDataReader oracleDataReader = cmd.ExecuteReader();
        //        while (oracleDataReader.Read())
        //        {
        //            var masBranchDisp = new MasBranchDisp();
        //            masBranchDisp.HoseId = Convert.ToInt32(oracleDataReader["HOSE_ID"]);
        //            masBranchDisp.MeterStart = Convert.ToDecimal(oracleDataReader["OPEN_METER_VOLUME"]);
        //            masBranchDisp.MeterFinish = Convert.ToDecimal(oracleDataReader["CLOSE_METER_VOLUME"]);

        //            #region Sale Qty (liter and bath)
        //            OracleCommand cmdTotalSaleQty = new OracleCommand
        //            {
        //                Connection = con,
        //                CommandTimeout = 90,
        //                CommandText = $@"select f5.sell_qty, f5.goods_amt, f5.tax_amt, f5.disc_amt
        //                                    from {schema}function4max f4 			
        //                                    inner join {schema}function5max f5 on f4.journal_id = f5.journal_id			
        //                                    where  f4.site_id = '52{req.BrnCode}'			
        //                                    and  trunc(f4.business_date) = to_date('{req.DocDate}','yyyy-MM-dd')		
        //                                    and f4.shift_no = {req.PeriodNo}			
        //                                    and f5.hose_id = {Convert.ToString(oracleDataReader["HOSE_ID"])}"
        //            };

        //            OracleDataReader readerTotalSaleQty = cmdTotalSaleQty.ExecuteReader();
        //            var listSaleQty = new List<decimal>();
        //            var listSaleQtyBath = new List<decimal>();
        //            while (readerTotalSaleQty.Read())
        //            {
        //                var saleQty = readerTotalSaleQty["SELL_QTY"] != DBNull.Value ? Convert.ToDecimal(readerTotalSaleQty["SELL_QTY"]) : 0;

        //                var goodAmt = readerTotalSaleQty["GOODS_AMT"] != DBNull.Value ? Convert.ToDecimal(readerTotalSaleQty["GOODS_AMT"]) : 0;
        //                var taxAmt = readerTotalSaleQty["TAX_AMT"] != DBNull.Value ? Convert.ToDecimal(readerTotalSaleQty["TAX_AMT"]) : 0;
        //                var discAmt = readerTotalSaleQty["DISC_AMT"] != DBNull.Value ? Convert.ToDecimal(readerTotalSaleQty["DISC_AMT"]) : 0;

        //                listSaleQty.Add(saleQty);
        //                listSaleQtyBath.Add(goodAmt + taxAmt + discAmt);
        //            }
        //            masBranchDisp.SaleQty = listSaleQty.Sum();
        //            masBranchDisp.SaleQtyBath = listSaleQtyBath.Sum();
        //            #endregion

        //            #region Credit
        //            OracleCommand cmdCreditSale = new OracleCommand
        //            {
        //                Connection = con,
        //                CommandTimeout = 90,
        //                CommandText = $@"select sum(f5.goods_amt + f5.tax_amt + f5.disc_amt) as total_value		
        //                                    from {schema}function4max f4 			
        //                                    inner join {schema}function5max f5 on f4.journal_id = f5.journal_id			
        //                                    where  f4.site_id = '52{req.BrnCode}'			
        //                                    and  trunc(f4.business_date) = to_date('{req.DocDate}','yyyy-MM-dd')		
        //                                    and f4.shift_no = {req.PeriodNo}			
        //                                    and f5.hose_id = {Convert.ToString(oracleDataReader["HOSE_ID"])}
        //                                    and exists 
        //                                        ( select null 
        //                                            from {schema}function14max f14 
        //                                            where f14.mop_code in ({string.Join(',', mpCreditSale)})
        //                                            and f14.journal_id = f4.journal_id
        //                                            and f14.site_id = f4.site_id )"
        //            };
        //            var creditSale = cmdCreditSale.ExecuteScalar();
        //            masBranchDisp.CreditSaleQty = creditSale != DBNull.Value ? Convert.ToDecimal(creditSale) : 0;
        //            #endregion

        //            #region Cash
        //            OracleCommand cmdCashAndDisc = new OracleCommand
        //            {
        //                Connection = con,
        //                CommandTimeout = 90,
        //                CommandText = $@"select sum(f5.goods_amt + f5.tax_amt + f5.disc_amt) as total_value		
        //                                    from {schema}function4max f4 			
        //                                    inner join {schema}function5max f5 on f4.journal_id = f5.journal_id			
        //                                    where  f4.site_id = '52{req.BrnCode}'			
        //                                    and  trunc(f4.business_date) = to_date('{req.DocDate}','yyyy-MM-dd')		
        //                                    and f4.shift_no = {req.PeriodNo}			
        //                                    and f5.hose_id = {Convert.ToString(oracleDataReader["HOSE_ID"])}
        //                                    and exists 
        //                                        ( select null 
        //                                            from {schema}function14max f14 
        //                                            where f14.mop_code in ({string.Join(',', mpCashAndDisc)})
        //                                            and f14.journal_id = f4.journal_id
        //                                            and f14.site_id = f4.site_id )"
        //            };
        //            var cashAndDisc = cmdCashAndDisc.ExecuteScalar();
        //            masBranchDisp.CashSaleQty = cashAndDisc != DBNull.Value ? Convert.ToDecimal(cashAndDisc) : 0;
        //            #endregion

        //            #region Discount
        //            OracleCommand cmdDisc = new OracleCommand
        //            {
        //                Connection = con,
        //                CommandTimeout = 90,
        //                CommandText = $@"select sum(f5.disc_amt) as total_value		
        //                                    from {schema}function4max f4 			
        //                                    inner join {schema}function5max f5 on f4.journal_id = f5.journal_id			
        //                                    where  f4.site_id = '52{req.BrnCode}'			
        //                                    and  trunc(f4.business_date) = to_date('{req.DocDate}','yyyy-MM-dd')		
        //                                    and f4.shift_no = {req.PeriodNo}			
        //                                    and f5.hose_id = {Convert.ToString(oracleDataReader["HOSE_ID"])}
        //                                    and exists 
        //                                        ( select null 
        //                                            from {schema}function14max f14 
        //                                            where f14.mop_code in ({string.Join(',', mpDisc)})
        //                                            and f14.journal_id = f4.journal_id
        //                                            and f14.site_id = f4.site_id )"
        //            };
        //            var disc = cmdDisc.ExecuteScalar();
        //            masBranchDisp.DiscQty = disc != DBNull.Value ? Convert.ToDecimal(disc) : 0;
        //            #endregion

        //            #region Coupon
        //            OracleCommand cmdCoupon = new OracleCommand
        //            {
        //                Connection = con,
        //                CommandTimeout = 90,
        //                CommandText = $@"select sum(f5.disc_amt) as total_value		
        //                                    from {schema}function4max f4 			
        //                                    inner join {schema}function5max f5 on f4.journal_id = f5.journal_id			
        //                                    where  f4.site_id = '52{req.BrnCode}'			
        //                                    and  trunc(f4.business_date) = to_date('{req.DocDate}','yyyy-MM-dd')		
        //                                    and f4.shift_no = {req.PeriodNo}			
        //                                    and f5.hose_id = {Convert.ToString(oracleDataReader["HOSE_ID"])}
        //                                    and exists 
        //                                        ( select null 
        //                                            from {schema}function14max f14 
        //                                            where f14.mop_code in ({string.Join(',', mpCoupon)})
        //                                            and f14.journal_id = f4.journal_id
        //                                            and f14.site_id = f4.site_id )"
        //            };
        //            var coupon = cmdCoupon.ExecuteScalar();
        //            masBranchDisp.CouponQty = coupon != DBNull.Value ? Convert.ToDecimal(coupon) : 0;
        //            #endregion


        //            #region Test
        //            OracleCommand cmdTest = new OracleCommand
        //            {
        //                Connection = con,
        //                CommandTimeout = 90,
        //                CommandText = $@"select sum(f5.disc_amt) as total_value		
        //                                    from {schema}function4max f4 			
        //                                    inner join {schema}function5max f5 on f4.journal_id = f5.journal_id			
        //                                    where  f4.site_id = '52{req.BrnCode}'			
        //                                    and  trunc(f4.business_date) = to_date('{req.DocDate}','yyyy-MM-dd')		
        //                                    and f4.shift_no = {req.PeriodNo}			
        //                                    and f5.hose_id = {Convert.ToString(oracleDataReader["HOSE_ID"])}
        //                                    and exists 
        //                                        ( select null 
        //                                            from {schema}function14max f14 
        //                                            where f14.mop_code in ({string.Join(',', mpTest)})
        //                                            and f14.journal_id = f4.journal_id
        //                                            and f14.site_id = f4.site_id )"
        //            };
        //            var Test = cmdTest.ExecuteScalar();
        //            masBranchDisp.TestQty = Test != DBNull.Value ? Convert.ToDecimal(Test) : 0;
        //            #endregion

        //            listMasBranchDisp.Add(masBranchDisp);
        //        }

        //        con.Close();
        //        con.Dispose();


        //        respDisp.ForEach(x => 
        //        {
        //            var pos = listMasBranchDisp.Where(y => y.HoseId == x.HoseId).FirstOrDefault();
        //            if (pos != null) 
        //            {
        //                x.MeterStart = pos.MeterStart;
        //                x.MeterFinish = pos.MeterFinish;
        //                x.SaleQty = pos.SaleQty;
        //                x.SaleQtyBath = pos.SaleQtyBath;
        //                x.CreditSaleQty = pos.CreditSaleQty;
        //                x.CashSaleQty = pos.CashSaleQty;
        //                x.DiscQty = pos.DiscQty;
        //                x.CouponQty = pos.CouponQty;
        //                x.TestQty = pos.TestQty;
        //                x.TotalQty = x.SaleQty - x.TestQty;
        //            }
        //        });

        //        var result = new List<MasBranchMeterResponse>()
        //        {
        //            new MasBranchMeterResponse()
        //            {
        //                MasBranchDispItems = listMasBranchDisp.Count > 0 ? respDisp : new List<MasBranchDisp>(),
        //            }
        //        };

        //        return new QueryResult<MasBranchMeterResponse>
        //        {
        //            Items = result
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("An error occurred when get pos meter", ex);
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<QueryResult<MasBranchMeterResponse>> GetPosMeter(BranchMeterRequest req)
        {
            try
            {
                if (true || CheckHaveDataPos(req.BrnCode, req.DocDate, req.PeriodNo) > 0)
                {
                    var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    var listMasBranchDisp = new List<MasBranchDisp>();
                    var respDisp = new List<MasBranchDisp>();

                    var queryableDisp = context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate && x.PeriodNo == req.PeriodNo);
                    var query = queryableDisp.OrderBy(x => x.DispId).AsNoTracking().ToList();

                    if (query.Count > 0)
                    {
                        respDisp = _mapper.Map<List<DopPeriodMeter>, List<MasBranchDisp>>(query);
                        var queryMas = context.MasBranchDisps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).OrderBy(x => x.DispId).AsNoTracking().ToList();
                        respDisp.ForEach(x =>
                        {
                            x.HoseId = queryMas.Where(y => y.DispId == x.DispId).Select(y => y.HoseId).FirstOrDefault();
                        });
                    }
                    else
                    {
                        respDisp = context.MasBranchDisps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).OrderBy(x => x.DispId).AsNoTracking().ToList();
                    }

                    var mpCreditSale = (from mas in context.MasPayGroups
                                        join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
                                        where mas.PayGroupName == "Credit"
                                        select type.PayTypeId).ToList();

                    var listGroupCashAndDisc = new List<string>()
                    {
                        "Cash",
                        "Discount",
                        "Coupon",
                        "Withdraw"
                    };

                    var mpCashAndDisc = (from mas in context.MasPayGroups
                                         join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
                                         where listGroupCashAndDisc.Contains(mas.PayGroupName)
                                         select type.PayTypeId).ToList();


                    var mpDisc = (from mas in context.MasPayGroups
                                  join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
                                  where mas.PayGroupName == "Discount"
                                  select type.PayTypeId).ToList();


                    var mpCoupon = (from mas in context.MasPayGroups
                                    join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
                                    where mas.PayGroupName == "Coupon"
                                    select type.PayTypeId).ToList();


                    var mpTest = (from mas in context.MasPayGroups
                                  join type in context.MasPayTypes on mas.PayGroupId equals type.PayGroupId
                                  where mas.PayGroupName == "Test"
                                  select type.PayTypeId).ToList();

                    var mpCard = new List<int>() { 2 };
                    Task<List<SaleQtyAndSaleAmtPos>> listSaleQtyAndSaleAmt = null;
                    Task<List<CreditPos>> listCreditAmt = null;
                    Task<List<CashPos>> listCashAmt = null;
                    Task<List<DiscPos>> listDiscAmt = null;
                    Task<List<CouponPos>> listCouponAmt = null;
                    Task<List<CardPos>> listCardAmt = null;
                    Task<List<TestPos2>> listTestAmt2 = null;
                    Task<List<ModelMaxOrderPos>> listMaxOrderPos = null;

                    //var qryDpc = context.DopPosConfigs.Where(x => x.DocType == "Query");
                    //var dpc = await qryDpc.FirstOrDefaultAsync();
                    //bool isV2 = false;
                    //if (dpc != null)
                    //{
                    //    switch (dpc.DocDesc)
                    //    {
                    //        case "Version2":
                    //            isV2 = true;
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    #region Meter & saleQty
                    List<SaleQtyAndSaleAmtPos> listSaleQtyAndSaleAmtResult = await GetSaleQtyAndSaleAmtFromSqlServer(req.BrnCode, req.DocDate, req.PeriodNo);
                    if (listSaleQtyAndSaleAmtResult == null)
                    {
                        listSaleQtyAndSaleAmt = Task.Run(() => GetSaleQtyAndSaleAmtV2(req.BrnCode, req.DocDate, req.PeriodNo));
                    }
                    else
                    {
                        listSaleQtyAndSaleAmt = Task.FromResult(listSaleQtyAndSaleAmtResult);
                    }
                    #endregion

                    #region Credit
                    List<CreditPos> listCreditAmtResult = await GetCreditAmtFromSqlServer(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpCreditSale));
                    if (listCreditAmtResult == null)
                    {
                        listCreditAmt = Task.Run(() => GetCreditAmtV2(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpCreditSale)));
                    }
                    else
                    {
                        listCreditAmt = Task.FromResult(listCreditAmtResult);
                    }
                    #endregion

                    #region Cash
                    List<CashPos> listCashAmtResult = await GetCashAmtFromSqlServer(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpCashAndDisc));
                    if (listCashAmtResult == null)
                    {
                        listCashAmt = Task.Run(() => GetCashAmtV2(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpCashAndDisc)));
                    }
                    else
                    {
                        listCashAmt = Task.FromResult(listCashAmtResult);
                    }
                    #endregion

                    #region Discount
                    var listDiscAmtResult = await GetDiscAmtFromSqlServer(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpDisc));
                    if (listDiscAmtResult == null)
                    {
                        listDiscAmt = Task.Run(() => GetDiscAmtV3(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpDisc)));
                    }
                    else
                    {
                        listDiscAmt = Task.FromResult(listDiscAmtResult);
                    }
                    #endregion

                    #region Coupon
                    var listCouponAmtResult = await GetCouponAmtFromSqlServer(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpCoupon));
                    if (listCouponAmtResult == null)
                    {
                        listCouponAmt = Task.Run(() => GetCouponAmtV3(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpCoupon)));
                    }
                    else
                    {
                        listCouponAmt = Task.FromResult(listCouponAmtResult);
                    }
                    #endregion

                    #region บัตรเครดิต
                    var listCardAmtResult = await GetCardAmtFromSqlServer(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpCard));
                    if (listCardAmtResult == null)
                    {
                        listCardAmt = Task.Run(() => GetCardAmtV2(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpCard)));
                    }
                    else
                    {
                        listCardAmt = Task.FromResult(listCardAmtResult);
                    }
                    #endregion

                    #region ทดสอบเทคืน
                    var listTestAmt2Result = await GetTestAmt2FromSqlServer(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpTest));
                    if (listTestAmt2Result == null)
                    {
                        listTestAmt2 = Task.Run(() => GetTestAmt2V2(req.BrnCode, req.DocDate, req.PeriodNo, string.Join("','", mpTest)));
                    }
                    else
                    {
                        listTestAmt2 = Task.FromResult(listTestAmt2Result);
                    }
                    #endregion

                    #region Max Order
                    var listMaxOrderPosResult = await getMaxOrderFromSqlServer(req.BrnCode, req.DocDate, req.PeriodNo);
                    if (listMaxOrderPosResult == null)
                    {
                        listMaxOrderPos = Task.Run(() => getMaxOrder(req.BrnCode, req.DocDate, req.PeriodNo));
                    }
                    else
                    {
                        listMaxOrderPos = Task.FromResult(listMaxOrderPosResult);
                    }
                    #endregion

                    // MockUp Data
                    //var listSaleQtyAndSaleAmt = Task.Run(() => GetSaleQtyAndSaleAmtMockup(req.BrnCode, req.DocDate, req.PeriodNo).ToList());
                    //var listCreditAmt = Task.Run(() => GetCreditAmtMockup(req.BrnCode, req.DocDate, req.PeriodNo, string.Join(',', mpCreditSale)).ToList());
                    //var listCashAmt = Task.Run(() => GetCashAmtMockup(req.BrnCode, req.DocDate, req.PeriodNo, string.Join(',', mpCashAndDisc)).ToList());
                    //var listDiscAmt = Task.Run(() => GetDiscAmtMockup(req.BrnCode, req.DocDate, req.PeriodNo, string.Join(',', mpDisc)).ToList());
                    //var listCouponAmt = Task.Run(() => GetCouponAmtMockup(req.BrnCode, req.DocDate, req.PeriodNo, string.Join(',', mpCoupon)).ToList());

                    //var listCardAmt = Task.Run(() => GetCardAmtMockup(req.BrnCode, req.DocDate, req.PeriodNo, string.Join(',', mpCard)).ToList());
                    //var listTestAmt2 = Task.Run(() => GetTestAmt2Mockup(req.BrnCode, req.DocDate, req.PeriodNo, string.Join(',', mpTest)).ToList());
                    //var listMaxOrderPos = Task.Run(()=>getMaxOrderMockUp(req.BrnCode, req.DocDate, req.PeriodNo));
                    await Task.WhenAll(listSaleQtyAndSaleAmt, listCreditAmt, listCashAmt, listDiscAmt, listCouponAmt, listTestAmt2, listCardAmt, listMaxOrderPos);

                    listSaleQtyAndSaleAmt.Result.ForEach(x =>
                    {
                        var masBranchDisp = new MasBranchDisp()
                        {
                            HoseId = x.HostId,
                            MeterStart = x.MeterStart,
                            MeterFinish = x.MeterFinish,
                            SaleQty = x.SaleQty,
                            SaleAmt = x.SaleAmt,

                        };
                        listMasBranchDisp.Add(masBranchDisp);
                    });
                    foreach (var x in listMasBranchDisp)
                    //listMasBranchDisp.ForEach(x =>
                    {
                        if (x == null)
                        {
                            continue;
                        }
                        var credit = listCreditAmt.Result.Where(y => y.HostId == x.HoseId).FirstOrDefault();
                        var cash = listCashAmt.Result.Where(y => y.HostId == x.HoseId).FirstOrDefault();
                        var disc = listDiscAmt.Result.Where(y => y.HostId == x.HoseId).FirstOrDefault();
                        var coupon = listCouponAmt.Result.Where(y => y.HostId == x.HoseId).FirstOrDefault();
                        //var test = listTestAmt.Result.Where(y => y.HostId == x.HoseId).FirstOrDefault();
                        var test2 = listTestAmt2.Result.Where(y => y.HoseId == x.HoseId).FirstOrDefault();
                        var card = listCardAmt.Result.Where(y => y.HostId == x.HoseId).FirstOrDefault();

                        x.CreditAmt = credit != null ? credit.CreditAmt : 0;
                        x.CashAmt = cash != null ? cash.CashAmt : 0;
                        x.DiscAmt = disc != null ? disc.DiscAmt : 0;
                        x.CouponAmt = coupon != null ? coupon.CouponAmt : 0;
                        //x.TestQty = test != null ? test.TestAmt : 0;
                        x.TestQty = test2?.TestQty ?? 0;
                        x.SaleAmt -= test2?.TestAmt ?? 0;
                        x.CardAmt = card != null ? card.CardAmt : 0;
                    }//);
                    foreach (var x in respDisp)
                    //respDisp.ForEach(x =>
                    {
                        if (x == null)
                        {
                            continue;
                        }
                        x.MeterFinish = x.MeterStart;
                        x.SaleAmt = 0;
                        x.SaleQty = 0;
                        x.TotalQty = 0;
                        var pos = listMasBranchDisp.Where(y => y.HoseId == x.HoseId).FirstOrDefault();
                        if (pos != null)
                        {
                            x.MeterStart = pos.MeterStart;
                            x.MeterFinish = pos.MeterFinish;
                            x.SaleQty = pos.SaleQty;
                            x.SaleAmt = pos.SaleAmt;
                            x.CreditAmt = pos.CreditAmt;
                            x.CashAmt = pos.CashAmt;
                            x.DiscAmt = pos.DiscAmt;
                            x.CouponAmt = pos.CouponAmt;
                            x.TestQty = pos.TestQty;
                            x.TotalQty = x.SaleQty - x.TestQty;
                            x.CardAmt = x.CardAmt;
                        }
                    
                    }//);

                    var result = new List<MasBranchMeterResponse>()
                    {
                        new MasBranchMeterResponse()
                        {
                            MasBranchDispItems = listMasBranchDisp.Count > 0 ? respDisp : new List<MasBranchDisp>(),
                            MasBranchCashDrItems = getListDoperiodGl(listMaxOrderPos.Result),
                        }
                    };

                    return new QueryResult<MasBranchMeterResponse>
                    {
                        Items = result
                    };
                }
                else
                {
                    var result = new List<MasBranchMeterResponse>()
                    {
                        new MasBranchMeterResponse()
                        {
                            MasBranchDispItems =  new List<MasBranchDisp>(),
                        }
                    };
                    return new QueryResult<MasBranchMeterResponse>
                    {
                        Items = result
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occurred when get pos meter", ex);
                //throw new Exception(ex.Message);
                throw new Exception(getMessage(ex));
            }
        }

        private string getMessage(Exception ex)
        {
            string result = "";
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            result = ex.Message + Environment.NewLine + ex.StackTrace;
            return result;
        }

        public QueryResult<MasBranchCalibrate> GetMasBranchCalibrate(MasBranchCalibrateRequest req)
        {
            try
            {
                var masBranchCalibrate = context.MasBranchCalibrates.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().ToList();

                return new QueryResult<MasBranchCalibrate>
                {
                    Items = masBranchCalibrate
                };
            }
            catch (Exception ex)
            {
                log.Error("An error occurred when get mas branch calibrate", ex);
                throw new Exception(ex.Message);
            }
        }
        private int CheckHaveDataPos(string brnCode, string docDate, int periodNo)
        {
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                CommandText = $@"select count(*)
                                from {schema}function2
                                where site_id = '52{brnCode}'
                                and  trunc(business_date) =  to_date('{docDate}','yyyy-MM-dd') 
                                and shift_no = {periodNo}"
            };

            var oracleDataReader = cmd.ExecuteScalar();

            var result = oracleDataReader != DBNull.Value ? Convert.ToInt32(oracleDataReader) : 0;

            con.Close();
            con.Dispose();
            return result;
        }
        private IEnumerable<SaleQtyAndSaleAmtPos> GetSaleQtyAndSaleAmtMockup(string brnCode, string docDate, int periodNo)
        {
            yield return new SaleQtyAndSaleAmtPos()
            {
                HostId = 1,
                MeterFinish = 100,
                MeterStart = 50,
                SaleAmt = 900,
                SaleQty = 100
            };
            for (int i = 2; i < 11; i++)
            {
                yield return new SaleQtyAndSaleAmtPos()
                {
                    HostId = i,
                    MeterFinish = 0,//i * 100,
                    MeterStart = 0,//i * 50,
                    SaleAmt = 0,//i * 30,
                    SaleQty = 0,//i * 15
                };

            }

        }

        private async Task<List<SaleQtyAndSaleAmtPos>> GetSaleQtyAndSaleAmtFromSqlServer(string brnCode, string docDate, int periodNo)
        {
            //AND trunc(f2.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
            //AND  f2.business_date = f4.business_date
            string strSql = $@"SELECT  f2.hose_id Host_Id,f2.open_meter_volume Meter_Start,f2.close_meter_volume Meter_Finish
                                ,isnull(round(f2.TOTAL_VOLUME,2),0) Sale_Qty
                                ,isnull(sum(f5.goods_amt + f5.tax_amt + f5.disc_amt), 0) Sale_Amt
                                FROM inf_pos_function2 f2
                                left join inf_pos_function4 f4 
                                on f2.site_id = f4.site_id
                                AND convert( date , f2.business_date ) = f4.business_date                                
                                AND f2.shift_no = f4.shift_no
                                left JOIN inf_pos_function5 f5 
                                ON f4.journal_id = f5.journal_id
                                AND f2.hose_id = f5.hose_id
                                WHERE f2.site_id = '52{brnCode}'
                                AND CONVERT(char(10) , f2.business_date , 23) = '{docDate}'                                
                                AND f2.shift_no = {periodNo}
                                group by f2.hose_id,f2.open_meter_volume,f2.close_meter_volume,f2.TOTAL_VOLUME";
            List<SaleQtyAndSaleAmtPos> result = null;
            result = await DefaultService.GetEntityFromSql<List<SaleQtyAndSaleAmtPos>>(context, strSql);
            return result;
        }

        private List<SaleQtyAndSaleAmtPos> GetSaleQtyAndSaleAmt(string brnCode, string docDate, int periodNo)
        {
            var listResult = new List<SaleQtyAndSaleAmtPos>();
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                //CommandText = $@"SELECT  f2.hose_id,f2.open_meter_volume,f2.close_meter_volume
                //                ,nvl(round(f2.TOTAL_VOLUME,2),0) AS sell_qty  ,nvl(f2.TOTAL_VALUE,0)  AS sell_amt
                //                FROM {schema}function2 f2
                //                WHERE f2.site_id = '52{brnCode}'
                //                AND trunc(f2.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
                //                AND f2.shift_no = {periodNo}"

                CommandText = $@"SELECT  f2.hose_id,f2.open_meter_volume,f2.close_meter_volume
                                ,nvl(round(f2.TOTAL_VOLUME,2),0) AS sell_qty  --,nvl(f2.TOTAL_VALUE,0)  AS sell_amt
                                ,nvl(sum(f5.goods_amt + f5.tax_amt + f5.disc_amt), 0) AS sell_amt
                                FROM  {schema}function2 f2
                                left join {schema}function4 f4 
                                on f2.site_id = f4.site_id
                                AND trunc(f2.business_date) = f4.business_date
                                AND f2.shift_no = f4.shift_no
                                left JOIN {schema}function5 f5 
                                ON f4.journal_id = f5.journal_id
                                AND f2.hose_id = f5.hose_id
                                WHERE f2.site_id = '52{brnCode}'
                                AND trunc(f2.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
                                AND f2.shift_no = {periodNo}
                                group by f2.hose_id,f2.open_meter_volume,f2.close_meter_volume,f2.TOTAL_VOLUME"

                //CommandText = $@"SELECT  f2.hose_id,f2.open_meter_volume,f2.close_meter_volume,
                //                 nvl( sum(f5.sell_qty), 0) AS sell_qty,
                //                 nvl(sum(f5.goods_amt + f5.tax_amt + f5.disc_amt), 0) AS sell_amt
                //                FROM
                //                 {schema}function4 f4
                //                INNER JOIN {schema}function5 f5 
                //                ON
                //                 f4.journal_id = f5.journal_id
                //                RIGHT JOIN {schema}function2 f2
                //                ON
                //                 f2.site_id = f4.site_id
                //                 AND f2.business_date = f4.business_date
                //                 AND f2.shift_no = f4.shift_no
                //                 AND f2.hose_id = f5.hose_id
                //                WHERE
                //                 f2.site_id = '52{brnCode}'
                //                 AND trunc(f2.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
                //                 AND f2.shift_no = {periodNo}
                //                GROUP BY f2.hose_id, f2.open_meter_volume,f2.close_meter_volume"

            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();
            while (oracleDataReader.Read())
            {
                var result = new SaleQtyAndSaleAmtPos()
                {
                    HostId = oracleDataReader["HOSE_ID"] != DBNull.Value ? Convert.ToInt32(oracleDataReader["HOSE_ID"]) : 0,
                    MeterStart = oracleDataReader["OPEN_METER_VOLUME"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["OPEN_METER_VOLUME"]) : 0,
                    MeterFinish = oracleDataReader["CLOSE_METER_VOLUME"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["CLOSE_METER_VOLUME"]) : 0,
                    SaleQty = oracleDataReader["SELL_QTY"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["SELL_QTY"]) : 0,
                    SaleAmt = oracleDataReader["SELL_AMT"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["SELL_AMT"]) : 0
                };
                listResult.Add(result);
            }

            con.Close();
            con.Dispose();
            return listResult.OrderBy(x => x.HostId).ThenBy(x => x.MeterStart).ThenBy(x => x.MeterFinish).ToList();
        }

        private List<SaleQtyAndSaleAmtPos> GetSaleQtyAndSaleAmtV2(string brnCode, string docDate, int periodNo)
        {
            var result = new List<SaleQtyAndSaleAmtPos>();
            string strCommand = $@"select fd.mapping_nozzle hose_id
  , to_number(sm.start_meter) open_meter_volume
  , to_number(sm.end_meter) close_meter_volume
  ,p.PRODUCT_CODE  AS grade_id
  ,p.PRODUCT_DESC  AS grade_name
  , nvl(round(to_number(sm.end_meter) - to_number(sm.start_meter),2),0) sell_qty
  , nvl(goods_amt + tax_amt + disc_amt,0) sell_amt
from raptorpos.shift_meter sm, raptorpos.fuel_dispenser fd, raptorpos.branch br, raptorpos.day DAY, RAPTORPOS.PRODUCT p
  ,(select trd.hose_id hose_id, sum(bd.netamt - bd.vatamt) goods_amt
      , sum(bd.vatamt) tax_amt, sum(bd.discount) disc_amt
    from raptorpos.bill_header bh, raptorpos.bill_detail bd
      , raptorpos.s_transaction_detail trd, raptorpos.s_transaction_h trh
    where bh.com_code = bd.com_code
      and bh.branch_code = bd.branch_code
      and bh.pos_id = bd.pos_id
      and bh.pos_day_id = bd.pos_day_id
      and bh.shift_no = bd.shift_no
      and bh.bill_id = bd.bill_id
      and bh.com_code = trd.com_code
      and bh.branch_code = trd.branch_code
      and bh.pos_id = trd.pos_id
      and bh.pos_day_id = trd.pos_day_id
      and bh.shift_no = trd.shift_no
      and bh.sale_id = trd.sale_id
      and bd.branch_code = trd.branch_code
      and bd.pos_id = trd.pos_id
      and bd.pos_day_id = trd.pos_day_id
      and bd.shift_no = trd.shift_no
      and bd.ref_t_trans_id = trd.ref_t_trans_id
      and bd.item_id = trd.sale_item_id
      and bh.com_code = trh.com_code
      and bh.branch_code = trh.branch_code
      and bh.pos_id = trh.pos_id
      and bh.pos_day_id = trh.pos_day_id
      and bh.shift_no = trh.shift_no
      and bh.sale_id = trh.sale_id
      and bh.business_date is not null
      and bh.bill_status <> '0002'
      and trd.itemtype in ('O', 'N', 'F')
  and trunc(bh.business_date) = to_date('{docDate}','yyyy-mm-dd') --trunc(sysdate) - 4  --> Parameter
  and bh.branch_at = '52{brnCode}'  --> Parameter
  and bh.shift_no = {periodNo}  --> Parameter
  group by trd.hose_id ) sal
where sm.fuel_dispenser_id = fd.fuel_dispenser_id
  and sm.branch_code = fd.branch_code
  and sm.shift_no = fd.shift_no
  and sm.pos_day_id = fd.pos_day_id
  and sm.pos_id = fd.pos_id 
  and sm.branch_code = br.branch_code
  and sm.com_code = day.com_code
  and sm.branch_code = day.branch_code
  and sm.shift_no = day.shift_no
  and sm.pos_id = day.pos_id
  and sm.pos_day_id = day.day_id
  and fd.mapping_nozzle = sal.hose_id(+)
  and fd.Product_Code = P.PRODUCT_CODE
  and br.active = 'A'
  and sm.shift_no = {periodNo}  --> Parameter
  and sm.branch_at = '52{brnCode}'  --> Parameter
  and trunc(day.account_date) = to_date('{docDate}','yyyy-mm-dd')  --> PARAMETER";
            readOracle(strCommand, dr => {
                var saleAmt = new SaleQtyAndSaleAmtPos();
                saleAmt.HostId = DefaultService.GetInt(dr["HOSE_ID"]);
                saleAmt.MeterStart = DefaultService.GetDecimal(dr["OPEN_METER_VOLUME"]);
                saleAmt.MeterFinish = DefaultService.GetDecimal(dr["CLOSE_METER_VOLUME"]);
                saleAmt.SaleQty = DefaultService.GetDecimal(dr["SELL_QTY"]);
                saleAmt.SaleAmt = DefaultService.GetDecimal(dr["SELL_AMT"]);
                result.Add(saleAmt);
            });
            return result;
            //return result.OrderBy(x => x.HostId).ThenBy(x => x.MeterStart).ThenBy(x => x.MeterFinish).ToList();
        }


        private IEnumerable<CreditPos> GetCreditAmtMockup(string brnCode, string docDate, int periodNo, string mopCode)
        {
            for (int i = 1; i < 11; i++)
            {
                yield return new CreditPos()
                {
                    CreditAmt = i * 2,
                    HostId = i
                };
            }
        }
        private async Task<List<CreditPos>> GetCreditAmtFromSqlServer(string brnCode, string docDate, int periodNo, string mopCode)
        {
            string strCommandText = $@"SELECT
	                                f5.hose_id Host_Id,
	                                sum(f5.goods_amt + f5.tax_amt + f5.disc_amt) AS credit_amt
                                FROM
	                                inf_pos_function4 f4
                                INNER JOIN inf_pos_function5 f5 
                                ON
	                                f4.journal_id = f5.journal_id
                                WHERE
	                                f4.site_id = '52{brnCode}'
                                    AND CONVERT(char(10) , f4.business_date , 23) = '{docDate}'	                                
	                                AND f4.shift_no = {periodNo}
	                                AND f5.hose_id >0
	                                AND EXISTS 
                                    (
	                                SELECT
		                                NULL
	                                FROM
		                                inf_pos_function14 f14
	                                WHERE
		                                f14.mop_code IN ('{mopCode}')
			                                AND f14.journal_id = f4.journal_id
			                                AND f14.site_id = f4.site_id )
                                GROUP BY f5.hose_id";
            List<CreditPos> result = await DefaultService.GetEntityFromSql<List<CreditPos>>(context, strCommandText);
            return result;
        }
        private List<CreditPos> GetCreditAmtV2(string brnCode, string docDate, int periodNo, string mopCode)
        {
            List<CreditPos> result = new List<CreditPos>();
            string strCommand = $@"select c.hose_id hose_id, sum((b.netamt - b.vatamt) + b.vatamt + b.discount) credit_amt
                                from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.s_transaction_detail c
                                where a.com_code = b.com_code
                                  and a.branch_code = b.branch_code
                                  and a.pos_id = b.pos_id
                                  and a.pos_day_id = b.pos_day_id
                                  and a.shift_no = b.shift_no
                                  and a.bill_id = b.bill_id
                                  and a.com_code = c.com_code
                                  and a.branch_code = c.branch_code
                                  and a.pos_id = c.pos_id
                                  and a.pos_day_id = c.pos_day_id
                                  and a.shift_no = c.shift_no
                                  and a.sale_id = c.sale_id
                                  and b.branch_code = c.branch_code
                                  and b.pos_id = c.pos_id
                                  and b.pos_day_id = c.pos_day_id
                                  and b.shift_no = c.shift_no
                                  and b.ref_t_trans_id = c.ref_t_trans_id
                                  and b.item_id = c.sale_item_id
                                  and c.itemtype in ('O','N','F') 
                                  and a.bill_status <> '0002'
                                  and a.business_date is not null  
                                  and c.hose_id > 0
                                  and a.branch_at = '52{brnCode}' --> Parameter
                                  and trunc(a.business_date) = to_date('{docDate}','yyyy-mm-dd') -- trunc(sysdate) - 3 --> Parameter
                                  and a.shift_no = {periodNo} --> Parameter
                                  and exists
                                    ( select null
                                      from raptorpos.m_desc m
                                      where a.bill_status = m.desc_code
                                        and m.desc_parent_id = '2060')                
                                  and exists                
                                    ( select null                
                                      from raptorpos.payment_header ph, raptorpos.payment_detail pd               
                                      where ph.com_code = pd.com_code
                                        and ph.branch_code = pd.branch_code
                                        and ph.pos_id = pd.pos_id
                                        and ph.pos_day_id = pd.pos_day_id
                                        and ph.shift_no = pd.shift_no
                                        and ph.payment_id = pd.payment_id
                                        and a.com_code = ph.com_code                
                                        and a.branch_code = ph.branch_code                
                                        and a.pos_id = ph.pos_id                
                                        and a.pos_day_id = ph.pos_day_id                
                                        and a.shift_no = ph.shift_no                
                                        and a.sale_id = ph.sale_id
                                        and pd.pay_types in ('{mopCode}')) 
                                  and exists
                                    ( select null
                                      from raptorpos.s_transaction_h f
                                      where a.com_code = f.com_code
                                        and a.branch_code = f.branch_code
                                        and a.pos_id = f.pos_id
                                        and a.pos_day_id = f.pos_day_id
                                        and a.shift_no = f.shift_no
                                        and a.sale_id = f.sale_id )       
                                  and exists
                                    ( select null
                                      from raptorpos.product p
                                      where b.ref_t_product_code = p.product_code )                 
                                group by c.hose_id";
            readOracle(strCommand, dr =>
            {
                var cp = new CreditPos();
                cp.HostId = DefaultService.GetInt(dr["HOSE_ID"]);
                cp.CreditAmt = DefaultService.GetDecimal(dr["CREDIT_AMT"]);
                result.Add(cp);
            });
            return result;
        }
        private List<CreditPos> GetCreditAmt(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var listResult = new List<CreditPos>();
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                CommandText = $@"SELECT
	                                f5.hose_id,
	                                sum(f5.goods_amt + f5.tax_amt + f5.disc_amt) AS credit_amt
                                FROM
	                                {schema}function4 f4
                                INNER JOIN {schema}function5 f5 
                                ON
	                                f4.journal_id = f5.journal_id
                                WHERE
	                                f4.site_id = '52{brnCode}'
	                                AND trunc(f4.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
	                                AND f4.shift_no = {periodNo}
	                                AND f5.hose_id >0
	                                AND EXISTS 
                                    (
	                                SELECT
		                                NULL
	                                FROM
		                                {schema}function14 f14
	                                WHERE
		                                f14.mop_code IN ('{mopCode}')
			                                AND f14.journal_id = f4.journal_id
			                                AND f14.site_id = f4.site_id )
                                GROUP BY f5.hose_id"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();
            while (oracleDataReader.Read())
            {
                var result = new CreditPos()
                {
                    HostId = oracleDataReader["HOSE_ID"] != DBNull.Value ? Convert.ToInt32(oracleDataReader["HOSE_ID"]) : 0,
                    CreditAmt = oracleDataReader["CREDIT_AMT"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["CREDIT_AMT"]) : 0
                };
                listResult.Add(result);
            }

            con.Close();
            con.Dispose();
            return listResult;
        }
        private IEnumerable<CashPos> GetCashAmtMockup(string brnCode, string docDate, int periodNo, string mopCode)
        {
            for (int i = 1; i < 11; i++)
            {
                yield return new CashPos()
                {
                    HostId = i,
                    CashAmt = i * 100
                };
            }
        }
        private async Task<List<CashPos>> GetCashAmtFromSqlServer(string brnCode, string docDate, int periodNo, string mopCode)
        {
            string strCommandText = $@"SELECT
	                                f5.hose_id Host_Id,
	                                sum(f5.goods_amt + f5.tax_amt + f5.disc_amt) AS cash_amt
                                FROM
	                                inf_pos_function4 f4
                                INNER JOIN inf_pos_function5 f5 
                                ON
	                                f4.journal_id = f5.journal_id
                                WHERE
	                                f4.site_id = '52{brnCode}'
                                    AND CONVERT(char(10) , f4.business_date , 23) = '{docDate}'	                                
	                                AND f4.shift_no = {periodNo}
	                                AND f5.hose_id >0
	                                AND EXISTS 
                                    (
	                                SELECT
		                                NULL
	                                FROM
		                                inf_pos_function14 f14
	                                WHERE
		                                f14.mop_code IN ('{mopCode}')
			                                AND f14.journal_id = f4.journal_id
			                                AND f14.site_id = f4.site_id )
                                GROUP BY f5.hose_id";
            List<CashPos> result = await DefaultService.GetEntityFromSql<List<CashPos>>(context, strCommandText);
            return result;
        }
        private List<CashPos> GetCashAmtV2(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var result = new List<CashPos>();
            string strCommand = $@"select c.hose_id hose_id, sum((b.netamt - b.vatamt) + b.vatamt + b.discount) cash_amt
                                from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.s_transaction_detail c
                                where a.com_code = b.com_code
                                  and a.branch_code = b.branch_code
                                  and a.pos_id = b.pos_id
                                  and a.pos_day_id = b.pos_day_id
                                  and a.shift_no = b.shift_no
                                  and a.bill_id = b.bill_id
                                  and a.com_code = c.com_code
                                  and a.branch_code = c.branch_code
                                  and a.pos_id = c.pos_id
                                  and a.pos_day_id = c.pos_day_id
                                  and a.shift_no = c.shift_no
                                  and a.sale_id = c.sale_id
                                  and b.branch_code = c.branch_code
                                  and b.pos_id = c.pos_id
                                  and b.pos_day_id = c.pos_day_id
                                  and b.shift_no = c.shift_no
                                  and b.ref_t_trans_id = c.ref_t_trans_id
                                  and b.item_id = c.sale_item_id
                                  and c.itemtype in ('O','N','F') 
                                  and a.bill_status <> '0002'
                                  and a.business_date is not null  
                                  and c.hose_id > 0
                                  and a.branch_at = '52{brnCode}' --> Parameter
                                  and trunc(a.business_date)  = to_date('{docDate}','yyyy-MM-dd')  --trunc(sysdate) - 3 --> Parameter
                                  and a.shift_no = {periodNo} --> Parameter
                                  and exists
                                    ( select null
                                      from raptorpos.m_desc m
                                      where a.bill_status = m.desc_code
                                        and m.desc_parent_id = '2060')                
                                  and exists                
                                    ( select null                
                                      from raptorpos.payment_header ph, raptorpos.payment_detail pd               
                                      where ph.com_code = pd.com_code
                                        and ph.branch_code = pd.branch_code
                                        and ph.pos_id = pd.pos_id
                                        and ph.pos_day_id = pd.pos_day_id
                                        and ph.shift_no = pd.shift_no
                                        and ph.payment_id = pd.payment_id
                                        and a.com_code = ph.com_code                
                                        and a.branch_code = ph.branch_code                
                                        and a.pos_id = ph.pos_id                
                                        and a.pos_day_id = ph.pos_day_id                
                                        and a.shift_no = ph.shift_no                
                                        and a.sale_id = ph.sale_id
                                        and pd.pay_types in ('{mopCode}')) 
                                  and exists
                                    ( select null
                                      from raptorpos.s_transaction_h f
                                      where a.com_code = f.com_code
                                        and a.branch_code = f.branch_code
                                        and a.pos_id = f.pos_id
                                        and a.pos_day_id = f.pos_day_id
                                        and a.shift_no = f.shift_no
                                        and a.sale_id = f.sale_id )       
                                  and exists
                                    ( select null
                                      from raptorpos.product p
                                      where b.ref_t_product_code = p.product_code )                 
                                group by c.hose_id";
            readOracle(strCommand, dr =>
            {
                var cp = new CashPos();
                cp.HostId = DefaultService.GetInt(dr["HOSE_ID"]);
                cp.CashAmt = DefaultService.GetDecimal(dr["CASH_AMT"]);
                result.Add(cp);
            });
            return result;
        }
        private List<CashPos> GetCashAmt(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var listResult = new List<CashPos>();
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                CommandText = $@"SELECT
	                                f5.hose_id,
	                                sum(f5.goods_amt + f5.tax_amt + f5.disc_amt) AS cash_amt
                                FROM
	                                {schema}function4 f4
                                INNER JOIN {schema}function5 f5 
                                ON
	                                f4.journal_id = f5.journal_id
                                WHERE
	                                f4.site_id = '52{brnCode}'
	                                AND trunc(f4.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
	                                AND f4.shift_no = {periodNo}
	                                AND f5.hose_id >0
	                                AND EXISTS 
                                    (
	                                SELECT
		                                NULL
	                                FROM
		                                {schema}function14 f14
	                                WHERE
		                                f14.mop_code IN ('{mopCode}')
			                                AND f14.journal_id = f4.journal_id
			                                AND f14.site_id = f4.site_id )
                                GROUP BY
	                                f5.hose_id"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();
            while (oracleDataReader.Read())
            {
                var result = new CashPos()
                {
                    HostId = oracleDataReader["HOSE_ID"] != DBNull.Value ? Convert.ToInt32(oracleDataReader["HOSE_ID"]) : 0,
                    CashAmt = oracleDataReader["CASH_AMT"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["CASH_AMT"]) : 0
                };
                listResult.Add(result);
            }

            con.Close();
            con.Dispose();
            return listResult;
        }
        private IEnumerable<DiscPos> GetDiscAmtMockup(string brnCode, string docDate, int periodNo, string mopCode)
        {
            for (int i = 1; i < 11; i++)
            {
                yield return new DiscPos()
                {
                    HostId = i,
                    DiscAmt = i * 3
                };
            }
        }

        private async Task<List<DiscPos>> GetDiscAmtFromSqlServer(string brnCode, string docDate, int periodNo, string mopCode)
        {
            string Sql = $@"SELECT  f5.HOSE_ID as Host_Id,sum(bill.discount) AS disc_amt
                            FROM INF_POS_FUNCTION4 f4
                            INNER JOIN INF_POS_FUNCTION5 f5
                            ON f4.JOURNAL_ID = f5.JOURNAL_ID 
                            INNER JOIN 
                            (
	                            SELECT f5.JOURNAL_ID ,sum(f5.disc_amt) AS discount
	                            FROM INF_POS_FUNCTION4 f4
	                            INNER JOIN INF_POS_FUNCTION5 f5 
	                            ON f4.journal_id = f5.journal_id
	                            WHERE f4.site_id =  '52{brnCode}'
	                            AND convert(date,f4.business_date) = '{docDate}' 
	                            AND f4.shift_no  = {periodNo}
	                            AND EXISTS (SELECT NULL
		                            FROM INF_POS_FUNCTION14 f14
		                            WHERE f14.mop_code IN ('{mopCode}')
		                            AND f14.journal_id = f4.journal_id
		                            AND f14.site_id = f4.site_id )
	                            GROUP BY f5.JOURNAL_ID
                            ) bill
                            ON f5.JOURNAL_ID  = bill.journal_id
                            WHERE f5.HOSE_ID >0
                            and f4.site_id =  '52{brnCode}'
                            AND convert(date,f4.BUSINESS_DATE) =  '{docDate}' 
                            AND f4.SHIFT_NO = {periodNo}
                            AND EXISTS (SELECT NULL
	                            FROM INF_POS_FUNCTION14 f14
	                            WHERE f14.mop_code IN ('{mopCode}')
	                            AND f14.journal_id = f4.journal_id
	                            AND f14.site_id = f4.site_id )
                            GROUP BY f5.HOSE_ID";


            //string strCommandText = $@"SELECT
	           //                     f5.hose_id Host_Id,
	           //                     sum(f5.disc_amt) AS disc_amt
            //                    FROM
	           //                     inf_pos_function4 f4
            //                    INNER JOIN inf_pos_function5 f5 
            //                    ON
	           //                     f4.journal_id = f5.journal_id
            //                    WHERE
	           //                     f4.site_id = '52{brnCode}'
	           //                     AND CONVERT(char(10) , f4.business_date , 23) = '{docDate}' 
	           //                     AND f4.shift_no = {periodNo}
	           //                     AND f5.hose_id >0
	           //                     AND EXISTS 
            //                        (
	           //                     SELECT
		          //                      NULL
	           //                     FROM
		          //                      inf_pos_function14 f14
	           //                     WHERE
		          //                      f14.mop_code IN ('{mopCode}')
			         //                       AND f14.journal_id = f4.journal_id
			         //                       AND f14.site_id = f4.site_id )
            //                    GROUP BY
	           //                     f5.hose_id";
            List<DiscPos> result = await DefaultService.GetEntityFromSql<List<DiscPos>>(context, Sql);
            return result;
        }

        //private async Task<List<DiscPos>> GetDiscAmtFromSqlServer(string brnCode, string docDate, int periodNo, string mopCode)
        //{
        //    string strCommandText = $@"SELECT
        //                         f5.hose_id Host_Id,
        //                         sum(f5.disc_amt) AS disc_amt
        //                        FROM
        //                         inf_pos_function4 f4
        //                        INNER JOIN inf_pos_function5 f5 
        //                        ON
        //                         f4.journal_id = f5.journal_id
        //                        WHERE
        //                         f4.site_id = '52{brnCode}'
        //                         AND CONVERT(char(10) , f4.business_date , 23) = '{docDate}' 
        //                         AND f4.shift_no = {periodNo}
        //                         AND f5.hose_id >0
        //                         AND EXISTS 
        //                            (
        //                         SELECT
        //                          NULL
        //                         FROM
        //                          inf_pos_function14 f14
        //                         WHERE
        //                          f14.mop_code IN ('{mopCode}')
        //                           AND f14.journal_id = f4.journal_id
        //                           AND f14.site_id = f4.site_id )
        //                        GROUP BY
        //                         f5.hose_id";
        //    List<DiscPos> result = await DefaultService.GetEntityFromSql<List<DiscPos>>(context, strCommandText);
        //    return result;
        //}

        private List<DiscPos> GetDiscAmtV3(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var result = new List<DiscPos>();
            string Sql = $@"
                        SELECT  c.HOSE_ID as hose_id,sum(bd.disc_amt) AS disc_amt
                        from raptorpos.bill_header bh
                        INNER JOIN (
	                        SELECT a.BILL_NO,sum( b.DISCOUNT) AS disc_amt 
	                        from raptorpos.bill_header a
	                        , raptorpos.bill_detail b
	                        where a.com_code = b.com_code
	                          and a.branch_code = b.branch_code
	                          and a.pos_id = b.pos_id
	                          and a.pos_day_id = b.pos_day_id
	                          and a.shift_no = b.shift_no
	                          and a.bill_id = b.bill_id	   
	                          and a.bill_status <> '0002'
	                          and a.business_date is not NULL
	                          and a.branch_at =  '52{brnCode}' --> Parameter
	                          and trunc(a.BUSINESS_DATE) = to_date('{docDate}','yyyy-MM-dd')  --> Parameter
	                          and a.shift_no = {periodNo} --> Parameter
                              and exists
		                            ( select null
		                              from raptorpos.product p
		                              where b.ref_t_product_code = p.product_code )  
	                          GROUP BY a.BILL_NO
                        )bd
                        ON bh.bill_no = bd.bill_no
                        INNER JOIN raptorpos.s_transaction_detail c
                        on bh.com_code = c.com_code
                        and bh.branch_code = c.branch_code
                        and bh.pos_id = c.pos_id
                        and bh.pos_day_id = c.pos_day_id
                        and bh.shift_no = c.shift_no
                        and bh.sale_id = c.sale_id
                          where bh.bill_status <> '0002'
                          and bh.business_date is not NULL
                          and c.itemtype in ('O','N','F')
                         and c.hose_id > 0
                          and bh.branch_at = '52{brnCode}' --> Parameter
                          and trunc(bh.BUSINESS_DATE) = to_date('{docDate}','yyyy-MM-dd')  --> Parameter
                          and bh.shift_no = {periodNo} --> PARAMETER
                          and exists
                            ( select null
                              from raptorpos.m_desc m
                              where bh.bill_status = m.desc_code
                                and m.desc_parent_id = '2060')                
                          and exists                
                            ( select null                
                              from raptorpos.payment_header ph, raptorpos.payment_detail pd               
                              where ph.com_code = pd.com_code
                                and ph.branch_code = pd.branch_code
                                and ph.pos_id = pd.pos_id
                                and ph.pos_day_id = pd.pos_day_id
                                and ph.shift_no = pd.shift_no
                                and ph.payment_id = pd.payment_id
                                and bh.com_code = ph.com_code                
                                and bh.branch_code = ph.branch_code                
                                and bh.pos_id = ph.pos_id                
                                and bh.pos_day_id = ph.pos_day_id                
                                and bh.shift_no = ph.shift_no                
                                and bh.sale_id = ph.sale_id
                                and pd.pay_types in ('{mopCode}')) 
                          and exists
                            ( select null
                              from raptorpos.s_transaction_h f
                              where bh.com_code = f.com_code
                                and bh.branch_code = f.branch_code
                                and bh.pos_id = f.pos_id
                                and bh.pos_day_id = f.pos_day_id
                                and bh.shift_no = f.shift_no
                                and bh.sale_id = f.sale_id )       
                        GROUP BY c.HOSE_ID";


            readOracle(Sql, dr =>
            {
                var dp = new DiscPos();
                dp.HostId = DefaultService.GetInt(dr["hose_id"]);
                dp.DiscAmt = DefaultService.GetDecimal(dr["disc_amt"]);
                result.Add(dp);
            });
            return result;
        }

        private List<DiscPos> GetDiscAmtV2(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var result = new List<DiscPos>();
            string strCommand = $@"select c.hose_id hose_id, sum(b.discount) disc_amt
from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.s_transaction_detail c
where a.com_code = b.com_code
  and a.branch_code = b.branch_code
  and a.pos_id = b.pos_id
  and a.pos_day_id = b.pos_day_id
  and a.shift_no = b.shift_no
  and a.bill_id = b.bill_id
  and a.com_code = c.com_code
  and a.branch_code = c.branch_code
  and a.pos_id = c.pos_id
  and a.pos_day_id = c.pos_day_id
  and a.shift_no = c.shift_no
  and a.sale_id = c.sale_id
  and b.branch_code = c.branch_code
  and b.pos_id = c.pos_id
  and b.pos_day_id = c.pos_day_id
  and b.shift_no = c.shift_no
  and b.ref_t_trans_id = c.ref_t_trans_id
  and b.item_id = c.sale_item_id
  and c.itemtype in ('O','N','F') 
  and a.bill_status <> '0002'
  and a.business_date is not null  
  and c.hose_id > 0
  and a.branch_at = '52{brnCode}' --> Parameter
  and trunc(a.BUSINESS_DATE) = to_date('{docDate}','yyyy-MM-dd')  --> Parameter
  and a.shift_no = {periodNo} --> Parameter
  and exists
    ( select null
      from raptorpos.m_desc m
      where a.bill_status = m.desc_code
        and m.desc_parent_id = '2060')                
  and exists                
    ( select null                
      from raptorpos.payment_header ph, raptorpos.payment_detail pd               
      where ph.com_code = pd.com_code
        and ph.branch_code = pd.branch_code
        and ph.pos_id = pd.pos_id
        and ph.pos_day_id = pd.pos_day_id
        and ph.shift_no = pd.shift_no
        and ph.payment_id = pd.payment_id
        and a.com_code = ph.com_code                
        and a.branch_code = ph.branch_code                
        and a.pos_id = ph.pos_id                
        and a.pos_day_id = ph.pos_day_id                
        and a.shift_no = ph.shift_no                
        and a.sale_id = ph.sale_id
        and pd.pay_types in ('{mopCode}')) 
  and exists
    ( select null
      from raptorpos.s_transaction_h f
      where a.com_code = f.com_code
        and a.branch_code = f.branch_code
        and a.pos_id = f.pos_id
        and a.pos_day_id = f.pos_day_id
        and a.shift_no = f.shift_no
        and a.sale_id = f.sale_id )       
  and exists
    ( select null
      from raptorpos.product p
      where b.ref_t_product_code = p.product_code )                 
group by c.hose_id";
            readOracle(strCommand, dr =>
            {
                var dp = new DiscPos();
                dp.HostId = DefaultService.GetInt(dr["hose_id"]);
                dp.DiscAmt = DefaultService.GetDecimal(dr["disc_amt"]);
                result.Add(dp);
            });
            return result;
        }

        private List<DiscPos> GetDiscAmt(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var listResult = new List<DiscPos>();
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                CommandText = $@"SELECT
	                                f5.hose_id,
	                                sum(f5.disc_amt) AS disc_amt
                                FROM
	                                {schema}function4 f4
                                INNER JOIN {schema}function5 f5 
                                ON
	                                f4.journal_id = f5.journal_id
                                WHERE
	                                f4.site_id = '52{brnCode}'
	                                AND trunc(f4.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
	                                AND f4.shift_no = {periodNo}
	                                AND f5.hose_id >0
	                                AND EXISTS 
                                    (
	                                SELECT
		                                NULL
	                                FROM
		                                {schema}function14 f14
	                                WHERE
		                                f14.mop_code IN ('{mopCode}')
			                                AND f14.journal_id = f4.journal_id
			                                AND f14.site_id = f4.site_id )
                                GROUP BY
	                                f5.hose_id"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();
            while (oracleDataReader.Read())
            {
                var result = new DiscPos()
                {
                    HostId = oracleDataReader["HOSE_ID"] != DBNull.Value ? Convert.ToInt32(oracleDataReader["HOSE_ID"]) : 0,
                    DiscAmt = oracleDataReader["DISC_AMT"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["DISC_AMT"]) : 0
                };
                listResult.Add(result);
            }

            con.Close();
            con.Dispose();
            return listResult;
        }
        private IEnumerable<CouponPos> GetCouponAmtMockup(string brnCode, string docDate, int periodNo, string mopCode)
        {
            for (int i = 1; i < 11; i++)
            {
                yield return new CouponPos()
                {
                    HostId = i,
                    CouponAmt = i * 5
                };
            }
        }
        private async Task<List<CouponPos>> GetCouponAmtFromSqlServer(string brnCode, string docDate, int periodNo, string mopCode)
        {

            string Sql = $@"SELECT  f5.HOSE_ID as Host_Id,sum(bill.discount) AS coupon_amt
                            FROM INF_POS_FUNCTION4 f4
                            INNER JOIN INF_POS_FUNCTION5 f5
                            ON f4.JOURNAL_ID = f5.JOURNAL_ID 
                            INNER JOIN 
                            (
	                            SELECT f5.JOURNAL_ID ,sum(f5.disc_amt) AS discount
	                            FROM INF_POS_FUNCTION4 f4
	                            INNER JOIN INF_POS_FUNCTION5 f5 
	                            ON f4.journal_id = f5.journal_id
	                            WHERE f4.site_id =  '52{brnCode}'
	                            AND convert(date,f4.business_date) = '{docDate}' 
	                            AND f4.shift_no  = {periodNo}
	                            AND EXISTS (SELECT NULL
		                            FROM INF_POS_FUNCTION14 f14
		                            WHERE f14.mop_code IN ('{mopCode}')
		                            AND f14.journal_id = f4.journal_id
		                            AND f14.site_id = f4.site_id )
	                            GROUP BY f5.JOURNAL_ID
                            ) bill
                            ON f5.JOURNAL_ID  = bill.journal_id
                            WHERE f5.HOSE_ID >0
                            and f4.site_id =  '52{brnCode}'
                            AND convert(date,f4.BUSINESS_DATE) =  '{docDate}' 
                            AND f4.SHIFT_NO = {periodNo}
                            AND EXISTS (SELECT NULL
	                            FROM INF_POS_FUNCTION14 f14
	                            WHERE f14.mop_code IN ('{mopCode}')
	                            AND f14.journal_id = f4.journal_id
	                            AND f14.site_id = f4.site_id )
                            GROUP BY f5.HOSE_ID";

            //string strCommandText = $@"SELECT
            //                     f5.hose_id Host_Id,
            //                     sum(f5.disc_amt) AS coupon_amt
            //                    FROM
            //                     inf_pos_function4 f4
            //                    INNER JOIN inf_pos_function5 f5 
            //                    ON
            //                     f4.journal_id = f5.journal_id
            //                    WHERE
            //                     f4.site_id = '52{brnCode}'
            //                        AND CONVERT(char(10) , f4.business_date , 23) = '{docDate}'
            //                     AND f4.shift_no = {periodNo}
            //                     AND f5.hose_id >0
            //                     AND EXISTS 
            //                        (
            //                     SELECT
            //                      NULL
            //                     FROM
            //                      inf_pos_function14 f14
            //                     WHERE
            //                      f14.mop_code IN ('{mopCode}')
            //                       AND f14.journal_id = f4.journal_id
            //                       AND f14.site_id = f4.site_id )
            //                    GROUP BY
            //                     f5.hose_id";
            List<CouponPos> result = await DefaultService.GetEntityFromSql<List<CouponPos>>(context, Sql);
            return result;
        }



        private List<CouponPos> GetCouponAmtV3(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var result = new List<CouponPos>();
            string Sql = $@"
                        SELECT  c.HOSE_ID as hose_id,sum(bd.disc_amt) AS disc_amt
                        from raptorpos.bill_header bh
                        INNER JOIN (
	                        SELECT a.BILL_NO,sum( b.DISCOUNT) AS disc_amt 
	                        from raptorpos.bill_header a
	                        , raptorpos.bill_detail b
	                        where a.com_code = b.com_code
	                          and a.branch_code = b.branch_code
	                          and a.pos_id = b.pos_id
	                          and a.pos_day_id = b.pos_day_id
	                          and a.shift_no = b.shift_no
	                          and a.bill_id = b.bill_id	   
	                          and a.bill_status <> '0002'
	                          and a.business_date is not NULL
	                          and a.branch_at =  '52{brnCode}' --> Parameter
	                          and trunc(a.BUSINESS_DATE) = to_date('{docDate}','yyyy-MM-dd')  --> Parameter
	                          and a.shift_no = {periodNo} --> Parameter
                              and exists
		                            ( select null
		                              from raptorpos.product p
		                              where b.ref_t_product_code = p.product_code )  
	                          GROUP BY a.BILL_NO
                        )bd
                        ON bh.bill_no = bd.bill_no
                        INNER JOIN raptorpos.s_transaction_detail c
                        on bh.com_code = c.com_code
                        and bh.branch_code = c.branch_code
                        and bh.pos_id = c.pos_id
                        and bh.pos_day_id = c.pos_day_id
                        and bh.shift_no = c.shift_no
                        and bh.sale_id = c.sale_id
                          where bh.bill_status <> '0002'
                          and bh.business_date is not NULL
                          and c.itemtype in ('O','N','F')
                         and c.hose_id > 0
                          and bh.branch_at = '52{brnCode}' --> Parameter
                          and trunc(bh.BUSINESS_DATE) = to_date('{docDate}','yyyy-MM-dd')  --> Parameter
                          and bh.shift_no = {periodNo} --> PARAMETER
                          and exists
                            ( select null
                              from raptorpos.m_desc m
                              where bh.bill_status = m.desc_code
                                and m.desc_parent_id = '2060')                
                          and exists                
                            ( select null                
                              from raptorpos.payment_header ph, raptorpos.payment_detail pd               
                              where ph.com_code = pd.com_code
                                and ph.branch_code = pd.branch_code
                                and ph.pos_id = pd.pos_id
                                and ph.pos_day_id = pd.pos_day_id
                                and ph.shift_no = pd.shift_no
                                and ph.payment_id = pd.payment_id
                                and bh.com_code = ph.com_code                
                                and bh.branch_code = ph.branch_code                
                                and bh.pos_id = ph.pos_id                
                                and bh.pos_day_id = ph.pos_day_id                
                                and bh.shift_no = ph.shift_no                
                                and bh.sale_id = ph.sale_id
                                and pd.pay_types in ('{mopCode}')) 
                          and exists
                            ( select null
                              from raptorpos.s_transaction_h f
                              where bh.com_code = f.com_code
                                and bh.branch_code = f.branch_code
                                and bh.pos_id = f.pos_id
                                and bh.pos_day_id = f.pos_day_id
                                and bh.shift_no = f.shift_no
                                and bh.sale_id = f.sale_id )       
                        GROUP BY c.HOSE_ID";


            readOracle(Sql, dr =>
            {
                var cp = new CouponPos();
                cp.CouponAmt = DefaultService.GetDecimal(dr["disc_amt"]);
                cp.HostId = DefaultService.GetInt(dr["hose_id"]);
                result.Add(cp);
            });
            return result;
        }

        //        private List<CouponPos> GetCouponAmtV2(string brnCode, string docDate, int periodNo, string mopCode)
        //        {
        //            var result = new List<CouponPos>();
        //            string strCommand = $@"select c.hose_id hose_id, sum(b.discount) disc_amt                                
        //from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.s_transaction_detail c                                
        //where a.com_code = b.com_code                                
        //  and a.branch_code = b.branch_code                                
        //  and a.pos_id = b.pos_id                                
        //  and a.pos_day_id = b.pos_day_id                                
        //  and a.shift_no = b.shift_no                                
        //  and a.bill_id = b.bill_id                                
        //  and a.com_code = c.com_code                                
        //  and a.branch_code = c.branch_code                                
        //  and a.pos_id = c.pos_id                                
        //  and a.pos_day_id = c.pos_day_id                                
        //  and a.shift_no = c.shift_no                                
        //  and a.sale_id = c.sale_id                                
        //  and b.branch_code = c.branch_code                                
        //  and b.pos_id = c.pos_id                                
        //  and b.pos_day_id = c.pos_day_id                                
        //  and b.shift_no = c.shift_no                                
        //  and b.ref_t_trans_id = c.ref_t_trans_id                                
        //  and b.item_id = c.sale_item_id                                
        //  and c.itemtype in ('O','N','F')                                                            
        //  and a.bill_status <> '0002'                                
        //  and a.business_date is not null                                  
        //  and c.hose_id > 0                                
        //  and a.branch_at = '52{brnCode}' --> Parameter                                
        //  and trunc(a.BUSINESS_DATE) = to_date('{docDate}','yyyy-MM-dd')   --  trunc(sysdate) - 3 --> Parameter                                
        //  and a.shift_no = {periodNo} --> Parameter                                
        //  and exists                                
        //    ( select null                                
        //      from raptorpos.m_desc m                                
        //      where a.bill_status = m.desc_code                                
        //        and m.desc_parent_id = '2060')                                                
        //  and exists                                                
        //    ( select null                                                
        //      from raptorpos.payment_header ph, raptorpos.payment_detail pd                                               
        //      where ph.com_code = pd.com_code                                
        //        and ph.branch_code = pd.branch_code                                
        //        and ph.pos_id = pd.pos_id                                
        //        and ph.pos_day_id = pd.pos_day_id                                
        //        and ph.shift_no = pd.shift_no                                
        //        and ph.payment_id = pd.payment_id                                
        //        and a.com_code = ph.com_code                                                
        //        and a.branch_code = ph.branch_code                                                
        //        and a.pos_id = ph.pos_id                                                
        //        and a.pos_day_id = ph.pos_day_id                                                
        //        and a.shift_no = ph.shift_no                                                
        //        and a.sale_id = ph.sale_id                                
        //        and pd.pay_types in ('{mopCode}'))                                 
        //  and exists                                
        //    ( select null                                
        //      from raptorpos.s_transaction_h f                                
        //      where a.com_code = f.com_code                                
        //        and a.branch_code = f.branch_code                                
        //        and a.pos_id = f.pos_id                                
        //        and a.pos_day_id = f.pos_day_id                                
        //        and a.shift_no = f.shift_no                                
        //        and a.sale_id = f.sale_id )                                       
        //  and exists                                
        //    ( select null                                
        //      from raptorpos.product p                                
        //      where b.ref_t_product_code = p.product_code )                                                 
        //group by c.hose_id";
        //            readOracle(strCommand, dr =>
        //            {
        //                var cp = new CouponPos();
        //                cp.CouponAmt = DefaultService.GetDecimal(dr["disc_amt"]);
        //                cp.HostId = DefaultService.GetInt(dr["hose_id"]);
        //                result.Add(cp);
        //            });
        //            return result;
        //        }

        private List<CouponPos> GetCouponAmt(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var listResult = new List<CouponPos>();
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                CommandText = $@"SELECT
	                                f5.hose_id,
	                                sum(f5.disc_amt) AS coupon_amt
                                FROM
	                                {schema}function4 f4
                                INNER JOIN {schema}function5 f5 
                                ON
	                                f4.journal_id = f5.journal_id
                                WHERE
	                                f4.site_id = '52{brnCode}'
	                                AND trunc(f4.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
	                                AND f4.shift_no = {periodNo}
	                                AND f5.hose_id >0
	                                AND EXISTS 
                                    (
	                                SELECT
		                                NULL
	                                FROM
		                                {schema}function14 f14
	                                WHERE
		                                f14.mop_code IN ('{mopCode}')
			                                AND f14.journal_id = f4.journal_id
			                                AND f14.site_id = f4.site_id )
                                GROUP BY
	                                f5.hose_id"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();
            while (oracleDataReader.Read())
            {
                var result = new CouponPos()
                {
                    HostId = oracleDataReader["HOSE_ID"] != DBNull.Value ? Convert.ToInt32(oracleDataReader["HOSE_ID"]) : 0,
                    CouponAmt = oracleDataReader["COUPON_AMT"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["COUPON_AMT"]) : 0
                };
                listResult.Add(result);
            }

            con.Close();
            con.Dispose();
            return listResult;
        }
        //private List<TestPos> GetTestAmt(string brnCode, string docDate, int periodNo, string mopCode)
        //{
        //    var listResult = new List<TestPos>();
        //    OracleConnection con = new OracleConnection
        //    {
        //        ConnectionString = _connectionString
        //    };
        //    con.Open();

        //    OracleCommand cmd = new OracleCommand
        //    {
        //        Connection = con,
        //        CommandTimeout = 90,
        //        CommandText = $@"SELECT
        //                         f5.hose_id,
        //                         sum(f5.disc_amt) AS test_amt
        //                        FROM
        //                         {schema}function4 f4
        //                        INNER JOIN {schema}function5 f5 
        //                        ON
        //                         f4.journal_id = f5.journal_id
        //                        WHERE
        //                         f4.site_id = '52{brnCode}'
        //                         AND trunc(f4.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
        //                         AND f4.shift_no = {periodNo}
        //                         AND f5.hose_id >0
        //                         AND EXISTS 
        //                            (
        //                         SELECT
        //                          NULL
        //                         FROM
        //                          {schema}function14 f14
        //                         WHERE
        //                          f14.mop_code IN ({mopCode})
        //                           AND f14.journal_id = f4.journal_id
        //                           AND f14.site_id = f4.site_id )
        //                        GROUP BY
        //                         f5.hose_id"
        //    };

        //    OracleDataReader oracleDataReader = cmd.ExecuteReader();
        //    while (oracleDataReader.Read())
        //    {
        //        var result = new TestPos()
        //        {
        //            HostId = oracleDataReader["HOSE_ID"] != DBNull.Value ? Convert.ToInt32(oracleDataReader["HOSE_ID"]) : 0,
        //            TestAmt = oracleDataReader["TEST_AMT"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["TEST_AMT"]) : 0
        //        };
        //        listResult.Add(result);
        //    }

        //    con.Close();
        //    con.Dispose();
        //    return listResult;
        //}
        private IEnumerable<TestPos2> GetTestAmt2Mockup(string brnCode, string docDate, int periodNo, string mopCode)
        {
            yield return new TestPos2()
            {
                HoseId = 1,
                PluNumber = 1.ToString().PadLeft(10, '0'),
                TestAmt = 270,
                TestQty = 30
            };
            for (int i = 1; i < 11; i++)
            {
                yield return new TestPos2()
                {
                    HoseId = i,
                    PluNumber = i.ToString().PadLeft(10, '0'),
                    TestAmt = 0,// i * 20,
                    TestQty = 0,// i * 30
                };
            }
        }
        private async Task<List<TestPos2>> GetTestAmt2FromSqlServer(string brnCode, string docDate, int periodNo, string mopCode)
        {
            string strSql = @$"select f5.hose_id,f5.plu_number,sum(f5.SELL_QTY) as test_qty,sum(f5.GOODS_AMT+f5.DISC_AMT +f5.TAX_AMT  ) AS Test_amt 
                                from inf_pos_function4 f4 
                                inner join inf_pos_function5 f5 
                                on f4.journal_id = f5.journal_id
                                where  f4.site_id = '52{brnCode}'     --brn_code
                                AND CONVERT(char(10) , f4.business_date , 23) = '{docDate}'
                                and f4.shift_no = {periodNo} -- period
                                and f5.hose_id >0
                                and exists 
                                    ( select null 
                                        from inf_pos_function14 f14 
                                        where f14.mop_code in ('{mopCode}')  -- mas_pay_type
                                        and f14.journal_id = f4.journal_id
                                        and f14.site_id = f4.site_id )
                                group by f5.hose_id,f5.plu_number";
            List<TestPos2> result = await DefaultService.GetEntityFromSql<List<TestPos2>>(context, strSql);
            return result;
        }
        private List<TestPos2> GetTestAmt2V2(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var result = new List<TestPos2>();
            string strCommand = $@"select c.hose_id hose_id, b.ref_t_product_code plu_number
                      , sum(b.sell_qty) as test_qty
                      , sum((b.netamt - b.vatamt) + b.vatamt + b.discount) test_amt
                    from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.s_transaction_detail c                                
                    where a.com_code = b.com_code                                
                      and a.branch_code = b.branch_code                                
                      and a.pos_id = b.pos_id                                
                      and a.pos_day_id = b.pos_day_id                                
                      and a.shift_no = b.shift_no                                
                      and a.bill_id = b.bill_id                                
                      and a.com_code = c.com_code                                
                      and a.branch_code = c.branch_code                                
                      and a.pos_id = c.pos_id                                
                      and a.pos_day_id = c.pos_day_id                                
                      and a.shift_no = c.shift_no                                
                      and a.sale_id = c.sale_id                                
                      and b.branch_code = c.branch_code                                
                      and b.pos_id = c.pos_id                                
                      and b.pos_day_id = c.pos_day_id                                
                      and b.shift_no = c.shift_no                                
                      and b.ref_t_trans_id = c.ref_t_trans_id                                
                      and b.item_id = c.sale_item_id                                
                      and c.itemtype in ('O','N','F')                                                         
                      and a.bill_status <> '0002'                                
                      and a.business_date is not null                                  
                      and c.hose_id > 0                                
                      and a.branch_at = '52{brnCode}' --> Parameter                                
                      and trunc(a.BUSINESS_DATE) = to_date('{docDate}','yyyy-MM-dd')  -- trunc(sysdate) - 3 --> Parameter                                
                      and a.shift_no = {periodNo} --> Parameter                                
                      and exists                                
                        ( select null                                
                          from raptorpos.m_desc m                                
                          where a.bill_status = m.desc_code                                
                            and m.desc_parent_id = '2060')                                                
                      and exists                                                
                        ( select null                                                
                          from raptorpos.payment_header ph, raptorpos.payment_detail pd                                               
                          where ph.com_code = pd.com_code                                
                            and ph.branch_code = pd.branch_code                                
                            and ph.pos_id = pd.pos_id                                
                            and ph.pos_day_id = pd.pos_day_id                                
                            and ph.shift_no = pd.shift_no                                
                            and ph.payment_id = pd.payment_id                                
                            and a.com_code = ph.com_code                                                
                            and a.branch_code = ph.branch_code                                                
                            and a.pos_id = ph.pos_id                                                
                            and a.pos_day_id = ph.pos_day_id                                                
                            and a.shift_no = ph.shift_no                                                
                            and a.sale_id = ph.sale_id                                
                            and pd.pay_types in ('{mopCode}'))                                 
                      and exists                                
                        ( select null                                
                          from raptorpos.s_transaction_h f                                
                          where a.com_code = f.com_code                                
                            and a.branch_code = f.branch_code                                
                            and a.pos_id = f.pos_id                                
                            and a.pos_day_id = f.pos_day_id                                
                            and a.shift_no = f.shift_no                                
                            and a.sale_id = f.sale_id )                                       
                      and exists                                
                        ( select null                                
                          from raptorpos.product p                                
                          where b.ref_t_product_code = p.product_code )                                                 
                    group by c.hose_id, b.ref_t_product_code";
            readOracle(strCommand, dr => {
                var tp = new TestPos2();
                tp.HoseId = DefaultService.GetInt(dr["hose_id"]);
                tp.PluNumber = DefaultService.GetString(dr["plu_number"]);
                tp.TestAmt = DefaultService.GetDecimal(dr["test_amt"]);
                tp.TestQty = DefaultService.GetDecimal(dr["test_qty"]);
                result.Add(tp);
            });
            return result;
        }
        private List<TestPos2> GetTestAmt2(string brnCode, string docDate, int periodNo, string mopCode)
        {
            string strSql = @$"select f5.hose_id,f5.plu_number,sum(f5.SELL_QTY) as test_qty,sum(f5.GOODS_AMT+f5.DISC_AMT +f5.TAX_AMT  ) AS Test_amt 
                                from {schema}function4 f4 
                                inner join {schema}function5 f5 
                                on f4.journal_id = f5.journal_id
                                where  f4.site_id = '52{brnCode}'     --brn_code
                                and  trunc(f4.business_date) =  to_date('{docDate}','yyyy-MM-dd')   --doc_date
                                and f4.shift_no = {periodNo} -- period
                                and f5.hose_id >0
                                and exists 
                                    ( select null 
                                        from {schema}function14 f14 
                                        where f14.mop_code in ('{mopCode}')  -- mas_pay_type
                                        and f14.journal_id = f4.journal_id
                                        and f14.site_id = f4.site_id )
                                group by f5.hose_id,f5.plu_number";
            List<TestPos2> result = new List<TestPos2>();
            using (var da = new OracleDataAdapter(strSql, _connectionString))
            {
                using (var dt = new DataTable())
                {
                    da.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        bool haveHoseId = dt.Columns.Contains("hose_id");
                        bool havePluNumber = dt.Columns.Contains("plu_number");
                        bool haveTestQty = dt.Columns.Contains("test_qty");
                        bool haveTestAmt = dt.Columns.Contains("Test_amt");
                        foreach (DataRow dr in dt.Rows)
                        {
                            var tp = new TestPos2();
                            if (haveHoseId)
                            {
                                tp.HoseId = DefaultService.GetInt(dr["hose_id"]);
                            }
                            if (havePluNumber)
                            {
                                tp.PluNumber = DefaultService.GetString(dr["plu_number"]);
                            }
                            if (haveTestQty)
                            {
                                tp.TestQty = DefaultService.GetDecimal(dr["test_qty"]);
                            }
                            if (haveTestAmt)
                            {
                                tp.TestAmt = DefaultService.GetDecimal(dr["Test_amt"]);
                            }
                            result.Add(tp);
                        }
                    }
                }//dt

                //result = DefaultService.GetEntityFromDataTable<List<TestPos2>>(dt);
            }//da
            return result;
        }
        private IEnumerable<CardPos> GetCardAmtMockup(string brnCode, string docDate, int periodNo, string mopCode)
        {
            for (int i = 1; i < 11; i++)
            {
                yield return new CardPos()
                {
                    CardAmt = i * 25,
                    HostId = 1
                };
            }
        }
        private async Task<List<CardPos>> GetCardAmtFromSqlServer(string brnCode, string docDate, int periodNo, string mopCode)
        {
            string strCommandText = $@"SELECT
	                                f5.hose_id Host_Id,
	                                sum(f5.goods_amt + f5.tax_amt) AS card_amt
                                FROM
	                                inf_pos_function4 f4
                                INNER JOIN inf_pos_function5 f5 
                                ON
	                                f4.journal_id = f5.journal_id
                                WHERE
	                                f4.site_id = '52{brnCode}'
                                    AND CONVERT(char(10) , f4.business_date , 23) = '{docDate}'
	                                AND f4.shift_no = {periodNo}
	                                AND f5.hose_id >0
	                                AND EXISTS 
                                    (
	                                SELECT
		                                NULL
	                                FROM
		                                inf_pos_function14 f14
	                                WHERE
		                                f14.mop_code IN ('{mopCode}')
			                                AND f14.journal_id = f4.journal_id
			                                AND f14.site_id = f4.site_id )
                                GROUP BY
	                                f5.hose_id";
            List<CardPos> result = await DefaultService.GetEntityFromSql<List<CardPos>>(context, strCommandText);
            return result;
        }
        private List<CardPos> GetCardAmtV2(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var result = new List<CardPos>();
            string strCommand = $@"select c.hose_id hose_id,sum(b.netamt) AS card_amt
                                from raptorpos.bill_header a, raptorpos.bill_detail b, raptorpos.s_transaction_detail c                                
                                where a.com_code = b.com_code                                
                                  and a.branch_code = b.branch_code                                
                                  and a.pos_id = b.pos_id                                
                                  and a.pos_day_id = b.pos_day_id                                
                                  and a.shift_no = b.shift_no                                
                                  and a.bill_id = b.bill_id                                
                                  and a.com_code = c.com_code                                
                                  and a.branch_code = c.branch_code                                
                                  and a.pos_id = c.pos_id                                
                                  and a.pos_day_id = c.pos_day_id                                
                                  and a.shift_no = c.shift_no                                
                                  and a.sale_id = c.sale_id                                
                                  and b.branch_code = c.branch_code                                
                                  and b.pos_id = c.pos_id                                
                                  and b.pos_day_id = c.pos_day_id                                
                                  and b.shift_no = c.shift_no                                
                                  and b.ref_t_trans_id = c.ref_t_trans_id                                
                                  and b.item_id = c.sale_item_id                                
                                  and c.itemtype in ('O','N','F')                                                         
                                  and a.bill_status <> '0002'                                
                                  and a.business_date is not null                                  
                                  and c.hose_id > 0                                
                                  and a.branch_at = '52{brnCode}' --> Parameter brn_code                        
                                  and trunc(a.BUSINESS_DATE) = to_date('{docDate}','yyyy-MM-dd')  --> Parameter doc_date                                
                                  and a.shift_no = 1 --> Parameter  period_no                     
                                  and exists                                
                                    ( select null                                
                                      from raptorpos.m_desc m                                
                                      where a.bill_status = m.desc_code                                
                                        and m.desc_parent_id = '2060')                                                
                                  and exists                                                
                                    ( select null                                                
                                      from raptorpos.payment_header ph, raptorpos.payment_detail pd                                               
                                      where ph.com_code = pd.com_code                                
                                        and ph.branch_code = pd.branch_code                                
                                        and ph.pos_id = pd.pos_id                                
                                        and ph.pos_day_id = pd.pos_day_id                                
                                        and ph.shift_no = pd.shift_no                                
                                        and ph.payment_id = pd.payment_id                                
                                        and a.com_code = ph.com_code                                                
                                        and a.branch_code = ph.branch_code                                                
                                        and a.pos_id = ph.pos_id                                                
                                        and a.pos_day_id = ph.pos_day_id                                                
                                        and a.shift_no = ph.shift_no                                                
                                        and a.sale_id = ph.sale_id                                
                                        and pd.pay_types IN ('{mopCode}'))   --> parameter mop_code                               
                                  and exists                                
                                    ( select null                                
                                      from raptorpos.s_transaction_h f                                
                                      where a.com_code = f.com_code                                
                                        and a.branch_code = f.branch_code                                
                                        and a.pos_id = f.pos_id                                
                                        and a.pos_day_id = f.pos_day_id                                
                                        and a.shift_no = f.shift_no                                
                                        and a.sale_id = f.sale_id )                                       
                                  and exists                                
                                    ( select null                                
                                      from raptorpos.product p                                
                                      where b.ref_t_product_code = p.product_code )                                                 
                                group by c.hose_id";
            readOracle(strCommand, dr => {
                var cp = new CardPos();
                cp.CardAmt = DefaultService.GetDecimal(dr["card_amt"]);
                cp.HostId = DefaultService.GetInt(dr["hose_id"]);
                result.Add(cp);
            });
            return result;
        }
        private List<CardPos> GetCardAmt(string brnCode, string docDate, int periodNo, string mopCode)
        {
            var listResult = new List<CardPos>();
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();

            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                CommandText = $@"SELECT
	                                f5.hose_id,
	                                sum(f5.goods_amt + f5.tax_amt) AS card_amt
                                FROM
	                                {schema}function4 f4
                                INNER JOIN {schema}function5 f5 
                                ON
	                                f4.journal_id = f5.journal_id
                                WHERE
	                                f4.site_id = '52{brnCode}'
	                                AND trunc(f4.business_date) = to_date('{docDate}', 'yyyy-MM-dd')
	                                AND f4.shift_no = {periodNo}
	                                AND f5.hose_id >0
	                                AND EXISTS 
                                    (
	                                SELECT
		                                NULL
	                                FROM
		                                {schema}function14 f14
	                                WHERE
		                                f14.mop_code IN ('{mopCode}')
			                                AND f14.journal_id = f4.journal_id
			                                AND f14.site_id = f4.site_id )
                                GROUP BY f5.hose_id"
            };

            OracleDataReader oracleDataReader = cmd.ExecuteReader();
            while (oracleDataReader.Read())
            {
                var result = new CardPos()
                {
                    HostId = oracleDataReader["HOSE_ID"] != DBNull.Value ? Convert.ToInt32(oracleDataReader["HOSE_ID"]) : 0,
                    CardAmt = oracleDataReader["CARD_AMT"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["CARD_AMT"]) : 0
                };
                listResult.Add(result);
            }

            con.Close();
            con.Dispose();
            return listResult;
        }

        private async Task<MasGlMap[]> getArrGlmap()
        {
            var qryGlMap = context.MasGlMaps
                .Where(x => context.MasGls.Any(y => x.GlNo == y.GlNo && y.GlStatus == "Active"))
                .AsNoTracking();
            var result = await qryGlMap.ToArrayAsync();
            return result;
        }
        private void readOracle(string pStrSql, Action<OracleDataReader> pAcReader)
        {
            if (pAcReader == null)
            {
                return;
            }
            pStrSql = DefaultService.GetString(pStrSql);
            if (0.Equals(pStrSql.Length))
            {
                return;
            }
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();
            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                CommandText = pStrSql
            };
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pAcReader(reader);
            }
            reader.Close();
            reader.Dispose();
            reader = null;
            cmd.Dispose();
            cmd = null;
            con.Close();
            con.Dispose();
            con = null;
            GC.Collect();
        }
        private async Task<T> ExcecuteScalarOracle<T>(string pStrSql)
        {
            pStrSql = DefaultService.GetString(pStrSql);
            if (0.Equals(pStrSql.Length))
            {
                return default(T);
            }
            OracleConnection con = new OracleConnection
            {
                ConnectionString = _connectionString
            };
            con.Open();
            OracleCommand cmd = new OracleCommand
            {
                Connection = con,
                CommandTimeout = 90,
                CommandText = pStrSql
            };
            object objExc = await cmd.ExecuteScalarAsync();
            T result = default(T);
            result = (T)objExc;
            cmd.Dispose();
            cmd = null;
            con.Close();
            con.Dispose();
            con = null;
            GC.Collect();
            return result;
        }
        private async Task<List<ModelMaxOrderPos>> getMaxOrderFromSqlServer(string brnCode, string docDate, int periodNo)
        {
            var arrGlMap = await getArrGlmap();
            if (arrGlMap == null || arrGlMap.Length == 0)
            {
                return null;
            }
            var arrMopCode = arrGlMap.Select(x => x.MopCode).Distinct().ToArray();
            var strMopCode = string.Join(',', arrMopCode);
            string strSql = @$"select f14.site_id , f14.shift_no, f14.mop_code Mob_Code, f14.mop_info Mob_Info, Sum(f14.amount) as amount
            from inf_pos_function14 f14
            where  f14.site_id = '52{brnCode}'
            AND CONVERT(char(10) , f14.business_date , 23) = '{docDate}'
            and f14.shift_no = {periodNo}
            and f14.mop_code in ({strMopCode})
            group by f14.site_id, CONVERT(char(10) , f14.business_date , 23), f14.shift_no, f14.mop_code, f14.mop_info
            order by f14.site_id, f14.shift_no, f14.mop_code, f14.mop_info";
            List<ModelMaxOrderPos> result = await DefaultService.GetEntityFromSql<List<ModelMaxOrderPos>>(context, strSql);
            if (result != null)
            {
                foreach (var item in result)
                {
                    string strGlNo = arrGlMap.FirstOrDefault(x => x.MopCode == item.MobCode)?.GlNo;
                    item.GlNo = DefaultService.GetString(strGlNo);
                }
            }
            return result;
        }
        private async Task<List<ModelMaxOrderPos>> getMaxOrder(string brnCode, string docDate, int periodNo)
        {
            var arrGlMap = await getArrGlmap();
            if (arrGlMap == null || arrGlMap.Length == 0)
            {
                return null;
            }
            var arrMopCode = arrGlMap.Select(x => x.MopCode).Distinct().ToArray();
            var strMopCode = string.Join(',', arrMopCode);
            var qrySysConfig = context.SysConfigApis.Where(x => x.SystemId == "POS").AsNoTracking();
            var sysConfig = await qrySysConfig.FirstOrDefaultAsync();
            if (sysConfig == null || string.IsNullOrWhiteSpace(sysConfig.ApiUrl))
            {
                return null;
            }
            string strSql2 = sysConfig.ApiUrl
                .Replace(";", string.Empty)
                .Replace("@site_id", brnCode)
                .Replace("@business_date", docDate)
                .Replace("@shift_no", periodNo.ToString())
                .Replace("@mop_code", strMopCode)
                ;
            //            string strSql = @$"select f14.site_id , f14.shift_no, f14.mop_code, f14.mop_info, Sum(f14.amount) as amount				
            //from function14 f14				
            //where  f14.site_id = '52{brnCode}'				
            //and trunc(f14.business_date) = to_date('{docDate}','yyyy-mm-dd')				
            //and f14.shift_no = {periodNo}				
            //and f14.mop_code in ({strMopCode})				
            //group by f14.site_id, to_date(f14.business_date,'yyyy/mm/dd'), f14.shift_no, f14.mop_code, f14.mop_info				
            //order by f14.site_id, f14.shift_no, f14.mop_code, f14.mop_info";
            var result = new List<ModelMaxOrderPos>();
            readOracle(strSql2, dr =>
            {
                var mop = new ModelMaxOrderPos();
                mop.MobCode = DefaultService.GetInt(dr["mop_code"]);
                mop.MobInfo = DefaultService.GetString(dr["mop_info"]);
                mop.ShiftNo = DefaultService.GetInt(dr["shift_no"]);
                mop.SiteId = DefaultService.GetString(dr["site_id"]);
                mop.Amount = DefaultService.GetDecimal(dr["amount"]);
                string strGlNo = arrGlMap.FirstOrDefault(x => x.MopCode == mop.MobCode)?.GlNo;
                mop.GlNo = DefaultService.GetString(strGlNo);
                result.Add(mop);
            });
            return result;
        }
        private async Task<List<ModelMaxOrderPos>> getMaxOrderMockUp(string brnCode, string docDate, int periodNo)
        {
            var arrGlMap = await getArrGlmap();
            decimal start = 100;
            int intIndex = 1;
            var result = new List<ModelMaxOrderPos>();
            foreach (var item in arrGlMap)
            {
                var mop = new ModelMaxOrderPos()
                {
                    Amount = start * (++intIndex),
                    GlNo = item.GlNo,
                    MobCode = item.MopCode,
                    MobInfo = "MobInfo : " + intIndex.ToString(),
                    ShiftNo = periodNo,
                    SiteId = brnCode,
                };
                result.Add(mop);
            }
            return result;
        }
        private List<DopPeriodGl> getListDoperiodGl(IEnumerable<ModelMaxOrderPos> pIenMaxOrderPos)
        {
            if (pIenMaxOrderPos == null)
            {
                return null;
            }
            var result = new List<DopPeriodGl>();
            foreach (var item in pIenMaxOrderPos)
            {
                DopPeriodGl dpg = null;
                dpg = result.FirstOrDefault(x => x.GlNo == item.GlNo);
                if (dpg != null)
                {
                    dpg.GlAmt += item.Amount ?? decimal.Zero;
                    continue;
                }
                dpg = new DopPeriodGl();
                dpg.GlNo = item.GlNo;
                dpg.GlAmt = item.Amount ?? decimal.Zero;
                result.Add(dpg);
            }
            return result;
        }

        public async Task<MeterResponse> ValidatePOS(string pStrBrnCode, int pIntPeriodNo, DateTime pDatDocDate)
        {
            MeterResponse result = new MeterResponse()
            {
                Message = "Success",
                Status = "Success",
                StatusCode = StatusCodes.Status200OK
            };
            Func<Exception, MeterResponse> funcGetException = e =>
            {
                var resp = new MeterResponse();
                resp.Status = "Fail";
                resp.StatusCode = StatusCodes.Status400BadRequest;
                while (e.InnerException != null) e = e.InnerException;
                resp.Message = e.Message;
                return resp;
            };

            var docDate = pDatDocDate.ToString("yyyy-MM-dd");
            //            string strCommand = $@"SELECT  count(*)
            //FROM RAPTORPOS.BILL_HEADER bh
            //WHERE bh.BRANCH_AT  = '52{pStrBrnCode}'  --> PARAMETER brn_code
            //AND bh.SHIFT_NO = {pIntPeriodNo} --> PARAMETER period_no
            //AND trunc(bh.BUSINESS_DATE) = TO_DATE('{docDate}','yyyy-MM-dd')  --> PARAMETER doc_date";

            string strCommand = @$"SELECT  count(*)
                                FROM RAPTORPOS.BILL_HEADER bh
                                WHERE bh.BRANCH_AT  = '52{pStrBrnCode}'  --> PARAMETER brn_code
                                AND TRUNC(bh.BILL_DATE) = TO_DATE('{docDate}','yyyy-MM-dd')  --> PARAMETER doc_date
                                and bh.business_date is  NULL
                                AND bh.SHIFT_NO = {pIntPeriodNo}";
            decimal decCount = 0;
            try
            {
                decCount = await ExcecuteScalarOracle<decimal>(strCommand);
            }
            catch (Exception ex)
            {
                result = funcGetException(ex);
                return result;
            }

            if (decCount > 0)
            {
                result.Message = ($"ข้อมูลกะที่ {pIntPeriodNo} อยู่ในระหว่างการส่งข้อมูลจากตู้จ่ายมายังส่วนกลาง");
                result.StatusCode = StatusCodes.Status400BadRequest;
                result.Status = "Fail";
            }
            return result;
        }

        public async Task<bool> SaveLog(SaveDocumentRequest req)
        {
            try
            {
                req.Tank.TankItems.ForEach(x => {
                    x.PdImage = "";
                });

                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var jsoin = JsonConvert.SerializeObject(req);
                DopPeriodLog log = new DopPeriodLog();
                log.CompCode = req.CompCode;
                log.BrnCode = req.BrnCode;
                log.DocDate = docDate;
                log.PeriodNo = req.PeriodNo;
                log.JsonData = jsoin;
                log.CreatedBy = req.User;
                log.CreatedDate = DateTime.Now;
                await this.context.DopPeriodLogs.AddAsync(log);
                return true;
            }
            catch
            {
                return false;
            }


        }
    }
}
