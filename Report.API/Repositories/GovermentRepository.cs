using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class GovermentRepository : SqlDataAccessHelper, IGovermentRepository
    {

        private readonly IMapper _mapper;
        public GovermentRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public Gov01Response GetGov01PDF(GovermentRequest req)
        {
            Gov01Response result = new Gov01Response();

            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode &&
                          b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            result.brnCode = branch.b.BrnCode;
            result.brnName = branch.b.BrnName;
            result.branchNo = branch.b.BranchNo;
            result.brnAddress = branch.b.FullAddress;
            result.compCode = branch.b.CompCode;
            result.compName = branch.c.CompName;
            result.docDates = new List<Gov01Head>();
            result.meters = new List<Gov01Item>();
            result.diffs = new List<Gov01Summary>();
            result.summary = new List<Gov01Summary>();

            var query = this.context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo).ToList();
            int minPeriod = query.Min(x => x.PeriodNo);
            int maxPeriod = query.Max(x => x.PeriodNo);

            var day = req.DateFrom;
            while (day.Date <= req.DateTo)
            {
                Gov01Head docdate = new Gov01Head() { docDate = day.ToString("yyyy-MM-dd") };
                result.docDates.Add(docdate);


                var meters = this.context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == day.Date).ToList();
                var items = meters.GroupBy(x => new { x.DispId, x.PdId, x.PdName }).Select(x => new Gov01Item
                {
                    dispId = x.Key.DispId
                }).ToList();

                items.ForEach(x =>
                {
                    x.docDate = day.ToString("yyyy-MM-dd");
                    x.meterStart = meters.FirstOrDefault(q => q.PeriodNo == minPeriod && q.DispId == x.dispId).MeterStart ?? 0;
                    x.meterFinish = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).MeterFinish ?? 0;
                    x.itemQty1 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000001" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.TotalQty) ?? 0 : 0;
                    x.itemAmt1 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000001" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.SaleAmt) ?? 0 : 0;

                    x.itemQty2 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000002" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.TotalQty) ?? 0 : 0;
                    x.itemAmt2 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000002" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.SaleAmt) ?? 0 : 0;

                    x.itemQty3 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000004" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.TotalQty) ?? 0 : 0;
                    x.itemAmt3 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000004" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.SaleAmt) ?? 0 : 0;

                    x.itemQty4 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000005" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.TotalQty) ?? 0 : 0;
                    x.itemAmt4 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000005" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.SaleAmt) ?? 0 : 0;

                    x.itemQty5 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000006" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.TotalQty) ?? 0 : 0;
                    x.itemAmt5 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000006" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.SaleAmt) ?? 0 : 0;

                    x.itemQty6 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000010" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.TotalQty) ?? 0 : 0;
                    x.itemAmt6 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000010" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.SaleAmt) ?? 0 : 0;

                    x.itemQty7 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000073" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.TotalQty) ?? 0 : 0;
                    x.itemAmt7 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000073" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.SaleAmt) ?? 0 : 0;

                    x.itemQty8 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000074" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.TotalQty) ?? 0 : 0;
                    x.itemAmt8 = meters.FirstOrDefault(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).PdId == "000074" ? meters.Where(q => q.PeriodNo == maxPeriod && q.DispId == x.dispId).Sum(s => s.SaleAmt) ?? 0 : 0;
                });
                result.meters.AddRange(items);


                //1 sum from meters
                Gov01Summary summary = new Gov01Summary();
                summary.docDate = day.ToString("yyyy-MM-dd");
                summary.seqNo = 1;
                summary.detail = "1.รวม";
                summary.itemQty1 = items.Sum(x => x.itemQty1);
                summary.itemAmt1 = items.Sum(x => x.itemAmt1);
                summary.itemQty2 = items.Sum(x => x.itemQty2);
                summary.itemAmt2 = items.Sum(x => x.itemAmt2);
                summary.itemQty3 = items.Sum(x => x.itemQty3);
                summary.itemAmt3 = items.Sum(x => x.itemAmt3);
                summary.itemQty4 = items.Sum(x => x.itemQty4);
                summary.itemAmt4 = items.Sum(x => x.itemAmt4);
                summary.itemQty5 = items.Sum(x => x.itemQty5);
                summary.itemAmt5 = items.Sum(x => x.itemAmt5);
                summary.itemQty6 = items.Sum(x => x.itemQty6);
                summary.itemAmt6 = items.Sum(x => x.itemAmt6);
                summary.itemQty7 = items.Sum(x => x.itemQty7);
                summary.itemAmt7 = items.Sum(x => x.itemAmt7);
                summary.itemQty8 = items.Sum(x => x.itemQty8);
                summary.itemAmt8 = items.Sum(x => x.itemAmt8);
                result.summary.Add(summary);


                //2.1 หักเบิกใช้ในกิจการ
                List<Gov01Summary> diffs = new List<Gov01Summary>();
                Gov01Summary diff = new Gov01Summary();
                var tanks = this.context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == day.Date).ToList();
                diff.docDate = day.ToString("yyyy-MM-dd");
                diff.seqNo = 1;
                diff.detail = "2.1 หักเบิกใช้ในกิจการ";
                diff.itemQty1 = tanks.Where(x => x.PdId == "000001").Sum(s => s.WithdrawQty) ?? 0;
                diff.itemQty2 = tanks.Where(x => x.PdId == "000002").Sum(s => s.WithdrawQty) ?? 0;
                diff.itemQty3 = tanks.Where(x => x.PdId == "000004").Sum(s => s.WithdrawQty) ?? 0;
                diff.itemQty4 = tanks.Where(x => x.PdId == "000005").Sum(s => s.WithdrawQty) ?? 0;
                diff.itemQty5 = tanks.Where(x => x.PdId == "000006").Sum(s => s.WithdrawQty) ?? 0;
                diff.itemQty6 = tanks.Where(x => x.PdId == "000010").Sum(s => s.WithdrawQty) ?? 0;
                diff.itemQty7 = tanks.Where(x => x.PdId == "000073").Sum(s => s.WithdrawQty) ?? 0;
                diff.itemQty8 = tanks.Where(x => x.PdId == "000074").Sum(s => s.WithdrawQty) ?? 0;
                diffs.Add(diff);

                //2.2 หักรับคืน/เทคืน
                diff = new Gov01Summary();
                diff.docDate = day.ToString("yyyy-MM-dd");
                diff.seqNo = 2;
                diff.detail = "2.2 หักรับคืน/เทคืน";
                diffs.Add(diff);

                //2.3 หักทดสอบ
                diff = new Gov01Summary();
                diff.docDate = day.ToString("yyyy-MM-dd");
                diff.seqNo = 3;
                diff.detail = "2.3 หักทดสอบ";
                diff.itemQty1 = meters.Where(x => x.PdId == "000001").Sum(s => s.TestQty) ?? 0;
                diff.itemQty2 = meters.Where(x => x.PdId == "000002").Sum(s => s.TestQty) ?? 0;
                diff.itemQty3 = meters.Where(x => x.PdId == "000004").Sum(s => s.TestQty) ?? 0;
                diff.itemQty4 = meters.Where(x => x.PdId == "000005").Sum(s => s.TestQty) ?? 0;
                diff.itemQty5 = meters.Where(x => x.PdId == "000006").Sum(s => s.TestQty) ?? 0;
                diff.itemQty6 = meters.Where(x => x.PdId == "000010").Sum(s => s.TestQty) ?? 0;
                diff.itemQty7 = meters.Where(x => x.PdId == "000073").Sum(s => s.TestQty) ?? 0;
                diff.itemQty8 = meters.Where(x => x.PdId == "000074").Sum(s => s.TestQty) ?? 0;
                diffs.Add(diff);
                result.diffs.AddRange(diffs);


                //3. รวมยอดขายประจำวัน
                summary = new Gov01Summary();
                summary.docDate = day.ToString("yyyy-MM-dd");
                summary.seqNo = 3;
                summary.detail = "3. รวมยอดขายประจำวัน";
                summary.itemQty1 = summary.itemQty1 - diffs.Sum(x => x.itemQty1);
                summary.itemAmt1 = summary.itemAmt1 - diffs.Sum(x => x.itemAmt1);
                summary.itemQty2 = summary.itemQty2 - diffs.Sum(x => x.itemQty2);
                summary.itemAmt2 = summary.itemAmt2 - diffs.Sum(x => x.itemAmt2);
                summary.itemQty3 = summary.itemQty3 - diffs.Sum(x => x.itemQty3);
                summary.itemAmt3 = summary.itemAmt3 - diffs.Sum(x => x.itemAmt3);
                summary.itemQty4 = summary.itemQty4 - diffs.Sum(x => x.itemQty4);
                summary.itemAmt4 = summary.itemAmt4 - diffs.Sum(x => x.itemAmt4);
                summary.itemQty5 = summary.itemQty5 - diffs.Sum(x => x.itemQty5);
                summary.itemAmt5 = summary.itemAmt5 - diffs.Sum(x => x.itemAmt5);
                summary.itemQty6 = summary.itemQty6 - diffs.Sum(x => x.itemQty6);
                summary.itemAmt6 = summary.itemAmt6 - diffs.Sum(x => x.itemAmt6);
                summary.itemQty7 = summary.itemQty7 - diffs.Sum(x => x.itemQty7);
                summary.itemAmt7 = summary.itemAmt7 - diffs.Sum(x => x.itemAmt7);
                summary.itemQty8 = summary.itemQty8 - diffs.Sum(x => x.itemQty8);
                summary.itemAmt8 = summary.itemAmt8 - diffs.Sum(x => x.itemAmt8);
                result.summary.Add(summary);

                var cashs = this.context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == day.Date).ToList();
                //4. หักส่วนลดการค้า
                summary = new Gov01Summary();
                summary.seqNo = 4;
                summary.detail = "4. หักส่วนลดการค้า";
                summary.itemAmt1 = cashs.Where(x => x.PdId == "000001").Sum(x => x.DiscAmt) ?? 0;
                summary.itemAmt2 = cashs.Where(x => x.PdId == "000002").Sum(x => x.DiscAmt) ?? 0;
                summary.itemAmt3 = cashs.Where(x => x.PdId == "000004").Sum(x => x.DiscAmt) ?? 0;
                summary.itemAmt4 = cashs.Where(x => x.PdId == "000005").Sum(x => x.DiscAmt) ?? 0;
                summary.itemAmt5 = cashs.Where(x => x.PdId == "000006").Sum(x => x.DiscAmt) ?? 0;
                summary.itemAmt6 = cashs.Where(x => x.PdId == "000010").Sum(x => x.DiscAmt) ?? 0;
                summary.itemAmt7 = cashs.Where(x => x.PdId == "000073").Sum(x => x.DiscAmt) ?? 0;
                summary.itemAmt8 = cashs.Where(x => x.PdId == "000074").Sum(x => x.DiscAmt) ?? 0;
                result.summary.Add(summary);


                day = day.AddDays(1);
            }


            return result;
        }

        public Gov03Response GetGov03PDF(GovermentRequest req)
        {
            Gov03Response result = new Gov03Response();

            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode &&
                          b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            result.brnCode = branch.b.BrnCode;
            result.brnName = branch.b.BrnName;
            result.branchNo = branch.b.BranchNo;
            result.brnAddress = branch.b.FullAddress;
            result.compCode = branch.b.CompCode;
            result.compName = branch.c.CompName;
            result.registerId = branch.c.RegisterId;
            result.monthName = req.DateFrom.ToString("MMMM", new System.Globalization.CultureInfo("th-TH"));
            result.year = req.DateFrom.ToString("yyyy", new System.Globalization.CultureInfo("th-TH"));
            result.tanks = new List<Gov03Tank>();
            result.summaries = new List<Gov03Summary>();

            var queryTank = this.context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo).ToList();
            var queryMeter = this.context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo).ToList();
            int minPeriod = queryTank.Min(x => x.PeriodNo);
            int maxPeriod = queryTank.Max(x => x.PeriodNo);

            var lastPeriods = queryTank.Where(x => x.DocDate == req.DateTo && x.PeriodNo == maxPeriod).ToList();
            foreach (var item in lastPeriods)
            {
                Gov03Tank tank = new Gov03Tank();
                tank.tankId = item.TankId;
                tank.pdId = item.PdId;
                tank.itemQty1 = item.PdId == "000001" ? item.RealQty ?? 0 : 0;
                tank.itemQty2 = item.PdId == "000002" ? item.RealQty ?? 0 : 0;
                tank.itemQty3 = item.PdId == "000004" ? item.RealQty ?? 0 : 0;
                tank.itemQty4 = item.PdId == "000005" ? item.RealQty ?? 0 : 0;
                tank.itemQty5 = item.PdId == "000006" ? item.RealQty ?? 0 : 0;
                tank.itemQty6 = item.PdId == "000010" ? item.RealQty ?? 0 : 0;
                tank.itemQty7 = item.PdId == "000073" ? item.RealQty ?? 0 : 0;
                tank.itemQty8 = item.PdId == "000074" ? item.RealQty ?? 0 : 0;
                result.tanks.Add(tank);
            }
            //2.รวมยอดน้ำมันสะสมในถังใต้ดิน
            Gov03Summary summary = new Gov03Summary();
            summary.desc = "2.รวมยอดน้ำมันสะสมในถังใต้ดิน";
            summary.itemQty1 = result.tanks.Sum(x => x.itemQty1);
            summary.itemQty2 = result.tanks.Sum(x => x.itemQty2);
            summary.itemQty3 = result.tanks.Sum(x => x.itemQty3);
            summary.itemQty4 = result.tanks.Sum(x => x.itemQty4);
            summary.itemQty5 = result.tanks.Sum(x => x.itemQty5);
            summary.itemQty6 = result.tanks.Sum(x => x.itemQty6);
            summary.itemQty7 = result.tanks.Sum(x => x.itemQty7);
            summary.itemQty8 = result.tanks.Sum(x => x.itemQty8);
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "ที่วัดได้ ณ วันปิดสำรวจ";
            result.summaries.Add(summary);

            var firstPeriods = queryTank.Where(x => x.DocDate == req.DateFrom && x.PeriodNo == minPeriod).ToList();
            summary = new Gov03Summary();
            summary.desc = "3.น้ำมันในถังใต้ดิน ณ วันปิดสำรวจต้นเดือน";
            summary.itemQty1 = firstPeriods.Where(x => x.PdId == "000001").Sum(x => x.BeforeQty) ?? 0;
            summary.itemQty2 = firstPeriods.Where(x => x.PdId == "000002").Sum(x => x.BeforeQty) ?? 0;
            summary.itemQty3 = firstPeriods.Where(x => x.PdId == "000004").Sum(x => x.BeforeQty) ?? 0;
            summary.itemQty4 = firstPeriods.Where(x => x.PdId == "000005").Sum(x => x.BeforeQty) ?? 0;
            summary.itemQty5 = firstPeriods.Where(x => x.PdId == "000006").Sum(x => x.BeforeQty) ?? 0;
            summary.itemQty6 = firstPeriods.Where(x => x.PdId == "000010").Sum(x => x.BeforeQty) ?? 0;
            summary.itemQty7 = firstPeriods.Where(x => x.PdId == "000073").Sum(x => x.BeforeQty) ?? 0;
            summary.itemQty8 = firstPeriods.Where(x => x.PdId == "000074").Sum(x => x.BeforeQty) ?? 0;
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "4.บวกยอดน้ำมันรับระหว่างเดือน";
            summary.itemQty1 = queryTank.Where(x => x.PdId == "000001").Sum(x => x.ReceiveQty) ?? 0;
            summary.itemQty2 = queryTank.Where(x => x.PdId == "000002").Sum(x => x.ReceiveQty) ?? 0;
            summary.itemQty3 = queryTank.Where(x => x.PdId == "000004").Sum(x => x.ReceiveQty) ?? 0;
            summary.itemQty4 = queryTank.Where(x => x.PdId == "000005").Sum(x => x.ReceiveQty) ?? 0;
            summary.itemQty5 = queryTank.Where(x => x.PdId == "000006").Sum(x => x.ReceiveQty) ?? 0;
            summary.itemQty6 = queryTank.Where(x => x.PdId == "000010").Sum(x => x.ReceiveQty) ?? 0;
            summary.itemQty7 = queryTank.Where(x => x.PdId == "000073").Sum(x => x.ReceiveQty) ?? 0;
            summary.itemQty8 = queryTank.Where(x => x.PdId == "000074").Sum(x => x.ReceiveQty) ?? 0;
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "    4.1 บวก รับคืน/เทคืน ระหว่างเดือน";
            result.summaries.Add(summary);


            summary = new Gov03Summary();
            summary.desc = "    4.2 บวก เทคืนจากบีบทดสอบ ระหว่างเดือน";
            summary.itemQty1 = queryMeter.Where(x => x.PdId == "000001").Sum(x => x.TestQty) ?? 0;
            summary.itemQty2 = queryMeter.Where(x => x.PdId == "000002").Sum(x => x.TestQty) ?? 0;
            summary.itemQty3 = queryMeter.Where(x => x.PdId == "000004").Sum(x => x.TestQty) ?? 0;
            summary.itemQty4 = queryMeter.Where(x => x.PdId == "000005").Sum(x => x.TestQty) ?? 0;
            summary.itemQty5 = queryMeter.Where(x => x.PdId == "000006").Sum(x => x.TestQty) ?? 0;
            summary.itemQty6 = queryMeter.Where(x => x.PdId == "000010").Sum(x => x.TestQty) ?? 0;
            summary.itemQty7 = queryMeter.Where(x => x.PdId == "000073").Sum(x => x.TestQty) ?? 0;
            summary.itemQty8 = queryMeter.Where(x => x.PdId == "000074").Sum(x => x.TestQty) ?? 0;
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "5.หักยอดน้ำมันที่ขายระหว่างเดือน -ผ่านมิเตอร์";
            summary.itemQty1 = queryMeter.Where(x => x.PdId == "000001").Sum(x => x.TotalQty) ?? 0;
            summary.itemQty2 = queryMeter.Where(x => x.PdId == "000002").Sum(x => x.TotalQty) ?? 0;
            summary.itemQty3 = queryMeter.Where(x => x.PdId == "000004").Sum(x => x.TotalQty) ?? 0;
            summary.itemQty4 = queryMeter.Where(x => x.PdId == "000005").Sum(x => x.TotalQty) ?? 0;
            summary.itemQty5 = queryMeter.Where(x => x.PdId == "000006").Sum(x => x.TotalQty) ?? 0;
            summary.itemQty6 = queryMeter.Where(x => x.PdId == "000010").Sum(x => x.TotalQty) ?? 0;
            summary.itemQty7 = queryMeter.Where(x => x.PdId == "000073").Sum(x => x.TotalQty) ?? 0;
            summary.itemQty8 = queryMeter.Where(x => x.PdId == "000074").Sum(x => x.TotalQty) ?? 0;
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "                         -ผ่านมิเตอร์";
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "    5.1 เบิกทดสอบ ระหว่างเดือน";
            summary.itemQty1 = queryMeter.Where(x => x.PdId == "000001").Sum(x => x.TestQty) ?? 0;
            summary.itemQty2 = queryMeter.Where(x => x.PdId == "000002").Sum(x => x.TestQty) ?? 0;
            summary.itemQty3 = queryMeter.Where(x => x.PdId == "000004").Sum(x => x.TestQty) ?? 0;
            summary.itemQty4 = queryMeter.Where(x => x.PdId == "000005").Sum(x => x.TestQty) ?? 0;
            summary.itemQty5 = queryMeter.Where(x => x.PdId == "000006").Sum(x => x.TestQty) ?? 0;
            summary.itemQty6 = queryMeter.Where(x => x.PdId == "000010").Sum(x => x.TestQty) ?? 0;
            summary.itemQty7 = queryMeter.Where(x => x.PdId == "000073").Sum(x => x.TestQty) ?? 0;
            summary.itemQty8 = queryMeter.Where(x => x.PdId == "000074").Sum(x => x.TestQty) ?? 0;
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "    5.2 หักเบิกใช้ในกิจการ ระหว่างเดือน";
            summary.itemQty1 = queryTank.Where(x => x.PdId == "000001").Sum(x => x.WithdrawQty) ?? 0;
            summary.itemQty2 = queryTank.Where(x => x.PdId == "000002").Sum(x => x.WithdrawQty) ?? 0;
            summary.itemQty3 = queryTank.Where(x => x.PdId == "000004").Sum(x => x.WithdrawQty) ?? 0;
            summary.itemQty4 = queryTank.Where(x => x.PdId == "000005").Sum(x => x.WithdrawQty) ?? 0;
            summary.itemQty5 = queryTank.Where(x => x.PdId == "000006").Sum(x => x.WithdrawQty) ?? 0;
            summary.itemQty6 = queryTank.Where(x => x.PdId == "000010").Sum(x => x.WithdrawQty) ?? 0;
            summary.itemQty7 = queryTank.Where(x => x.PdId == "000073").Sum(x => x.WithdrawQty) ?? 0;
            summary.itemQty8 = queryTank.Where(x => x.PdId == "000074").Sum(x => x.WithdrawQty) ?? 0;
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "    5.3 หักรับคืน/เทคืน ระหว่างเดือน";
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "6.น้ำมันคงเหลือในบัญชี";
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "7.จำนวนน้ำมันเพิ่มขึ้น/(ลดลง) ในเดือนนี้";
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "8.บวก ผลต่างสะสมยกมาจากเดือนก่อน";
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "9.ผลต่างสะสมยกไปเดือนหน้า";
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "10.ร้อยละของจำนวนน้ำมันเพิ่มขึ้น/(ลดลง)";
            result.summaries.Add(summary);

            summary = new Gov03Summary();
            summary.desc = "ต่อปริมาณน้ำมันที่ขายระหว่างเดือน";
            result.summaries.Add(summary);
            return result;
        }

        public List<Gov05Response> GetGov05PDF(GovermentRequest req)
        {
            List<Gov05Response> result = new List<Gov05Response>();

            result = (from h in this.context.InvReceiveProdHds
                      join d in this.context.InvReceiveProdDts on new { h.CompCode, h.BrnCode, h.LocCode, h.DocNo } equals new { d.CompCode, d.BrnCode, d.LocCode, d.DocNo }
                      join p in this.context.MasProducts on d.PdId equals p.PdId
                      join s in this.context.MasSuppliers on h.SupCode equals s.SupCode
                      where h.DocStatus != "Cancel"
                         && h.DocDate >= req.DateFrom && h.DocDate <= req.DateTo
                         && p.GroupId == "0000"
                         && h.CompCode == req.CompCode && h.BrnCode == req.BrnCode
                      select new { h.SupCode, h.SupName, s.Address, d.PdId, d.PdName, d.ItemQty }
                         ).GroupBy(x => new { x.SupCode, x.SupName, x.Address, x.PdId, x.PdName })
                         .Select(x => new Gov05Response
                         {

                             supCode = x.Key.SupCode,
                             supName = x.Key.SupName,
                             supAddress = x.Key.Address ?? "",
                             pdId = x.Key.PdId,
                             pdName = x.Key.PdName,
                             itemQty = x.Sum(s => s.ItemQty) ?? 0
                         }).ToList();
            int seq = 0;

            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode &&
                          b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            result.ForEach(x =>
            {
                x.seqNo = ++seq;
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.branchNo = branch.b.BranchNo;
                x.brnAddress = branch.b.FullAddress;
                x.compCode = branch.b.CompCode;
                x.compName = branch.c.CompName;
                x.month = req.DateFrom.ToString("MMMM", new CultureInfo("th-TH"));
                x.year = req.DateFrom.ToString("yyyy", new CultureInfo("th-TH"));
            });
            return result;
        }

        public async Task<List<Gov06Response>> GetGov06PDF(GovermentRequest req)
        {
            List<Gov06Response> result = new List<Gov06Response>();

            decimal densityBase = 1;
            var density = await this.context.MasDensities.OrderByDescending(x => x.StartDate).FirstOrDefaultAsync(x => x.CompCode == req.CompCode);
            if (req.UnitType == GovermentRequest.eUnitType.Kilo)
            {
                densityBase = (density == null) ? 0.54m : density.DensityBase ?? 0;
            }
            var branchtax = await this.context.MasBranchTaxes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).ToListAsync();

            var taxtype = this.context.MasBranchConfigs.FirstOrDefault(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).ReportTaxType;
            var queryTankSum = this.context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo).AsQueryable();
            switch (taxtype)
            {
                case "Sell":
                    result = await queryTankSum.GroupBy(x=> new { x.PdId,x.PdName})
                                            .Select(x=> new Gov06Response
                                            {
                                                pdId = x.Key.PdId,
                                                pdName = x.Key.PdName,
                                                meterAmt = Math.Round((x.Sum(s => s.IssueQty) ?? 0) * densityBase, 2),
                                            }).OrderBy(x=>x.pdId).ToListAsync();
                    break;

                case "Buy":
                    result = await queryTankSum.GroupBy(x => new { x.PdId, x.PdName })
                                            .Select(x => new Gov06Response
                                            {
                                                pdId = x.Key.PdId,
                                                pdName = x.Key.PdName,
                                                meterAmt = Math.Round((x.Sum(s => s.ReceiveQty) ?? 0) * densityBase, 2),
                                            }).OrderBy(x => x.pdId).ToListAsync();
                    break;
                default:
                    result = new List<Gov06Response>();
                    break;
            }

            //result = this.context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo)
            //            .GroupBy(x => new { x.PdId, x.PdName })
            //              .Select(x => new Gov06Response
            //              {
            //                  pdId = x.Key.PdId,
            //                  pdName = x.Key.PdName,
            //                  meterAmt = Math.Round((x.Sum(s => s.MeterAmt) ?? 0) * densityBase, 2),
            //              }).OrderBy(x => x.pdId).ToList();
            int seq = 0;

            var branch = await (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode &&
                          b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefaultAsync();


            result.ForEach(x =>
            {
                x.seqNo = ++seq;
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.branchNo = branch.b.BranchNo;
                x.brnAddress = branch.b.FullAddress??"";
                x.phone = branch.b.Phone??"";
                x.compCode = branch.b.CompCode;
                x.compName = branch.c.CompName;
                x.registerId = branch.c.RegisterId??"";
                x.month = req.DateFrom.ToString("MMMM", new CultureInfo("th-TH"));
                x.year = req.DateFrom.ToString("yyyy", new CultureInfo("th-TH"));
                x.vatRate = branchtax.Any(t => t.TaxId == x.pdId) ? branchtax.FirstOrDefault(t => t.TaxId == x.pdId).TaxAmt : 0;
                x.vatAmt = Math.Round(x.meterAmt * x.vatRate, 2);
            });
            var vatTotal = result.Sum(x => x.vatAmt);
            var vatTotalText = Function.ThaiBahtText(vatTotal);
            result.ForEach(x =>
            {
                x.vatTotal = vatTotal;
                x.vatTotalText = vatTotalText;
            });
            return result;
        }

        public List<Gov07Response> GetGov07PDF(GovermentRequest req)
        {
            List<Gov07Response> result = new List<Gov07Response>();

            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode &&
                          b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            var cashsaleList = (from hd in this.context.SalCashsaleHds
                                join dt in this.context.SalCashsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                                join p in this.context.MasProducts on new { dt.PdId } equals new { p.PdId }
                                where hd.CompCode == req.CompCode
                                && hd.BrnCode == req.BrnCode
                                && hd.DocStatus != "Cancel"
                                && p.GroupId != "0000"
                                && hd.DocDate >= req.DateFrom
                                && hd.DocDate <= req.DateTo
                                select new { hd, dt }
                         ).GroupBy(x => new { x.hd.DocDate, x.dt.DocNo })
                         .Select(x => new Gov07Response
                         {
                             //DocNo = x.Key.DocNo,
                             docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                             invType = "ย่อ",
                             desc = "ขายสินค้าหรือให้บริการ",
                             taxbaseAmt = x.Sum(s => s.dt.TaxBaseAmt) ?? 0,
                             vatAmt = x.Sum(s => s.dt.VatAmt) ?? 0,
                             totalAmt = x.Sum(s => s.dt.TotalAmt) ?? 0
                         }).ToList();
            result.AddRange(cashsaleList);


            result.ForEach(x =>
            {
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.branchNo = branch.b.BranchNo;
                x.brnAddress = branch.b.FullAddress;
                x.compCode = branch.b.CompCode;
                x.compName = branch.c.CompName;
                x.registerId = branch.c.RegisterId;

            });
            return result.OrderBy(x => x.docDate).ToList();
        }

        public  List<Gov08Response> GetGov08PDF(GovermentRequest req)
        {
            List<Gov08Response> result = new List<Gov08Response>();
            decimal densityBase = 1;
            var density =   this.context.MasDensities.OrderByDescending(x => x.StartDate).FirstOrDefault(x => x.CompCode == req.CompCode);
            if (req.UnitType == GovermentRequest.eUnitType.Kilo)
            {
                densityBase = (density == null) ? 0.54m : density.DensityBase ?? 0;
            }

            var datefrom = req.DateFrom;
            var dateto = req.DateTo;
            var dateprev = datefrom.AddDays(-1);

            var queryTankSum = this.context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= datefrom && x.DocDate <= dateto).ToList();
            var maxperiod = queryTankSum.Max(x => x.PeriodNo);
            var products = queryTankSum.Select(x => new { x.PdId, x.PdName }).Distinct();

            var queryReceiveProd = (from h in this.context.InvReceiveProdHds
                                join d in this.context.InvReceiveProdDts on new { h.CompCode, h.BrnCode, h.LocCode, h.DocNo } equals new { d.CompCode, d.BrnCode, d.LocCode, d.DocNo }
                                join p in this.context.MasProducts on d.PdId equals p.PdId
                                where p.GroupId == "0000"
                                && h.DocStatus != "Cancel"
                                && h.CompCode == req.CompCode
                                && h.BrnCode == req.BrnCode
                                && h.DocDate >= req.DateFrom
                                && h.DocDate <= req.DateTo
                                select new { h, d, p }
                         ).AsQueryable();

            #region sum year
            var sumYearReceiveQty = (from h in this.context.InvReceiveProdHds
                                     join d in this.context.InvReceiveProdDts on new { h.CompCode, h.BrnCode, h.LocCode, h.DocNo } equals new { d.CompCode, d.BrnCode, d.LocCode, d.DocNo }
                                     join p in this.context.MasProducts on d.PdId equals p.PdId
                                     where p.GroupId == "0000"
                                     && h.DocStatus != "Cancel"
                                     && h.CompCode == req.CompCode
                                     && h.BrnCode == req.BrnCode
                                     && h.DocDate.Value.Year == req.DateFrom.Year
                                     select new { h, d, p }
                                    ).AsQueryable(); //.Sum(s=>s.d.ItemQty);
            var sumYearSaleQty = this.context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate.Year == req.DateFrom.Year).AsQueryable(); //.Sum(s=>s.IssueQty);

            var sumAdjustQty = this.context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate.Year == req.DateFrom.Year).AsQueryable(); //.Sum(s => s.AdjustQty);
            
            #endregion


            List<Gov08Response> gov08list = new List<Gov08Response>();
            foreach (var prod in products)  //loop by product
            {
                var day = datefrom;
                while (day.Date <= dateto)  //loop by date
                {
                    Gov08Response gov08 = new Gov08Response();
                    gov08.pdId = prod.PdId;
                    gov08.pdName = prod.PdName;
                    gov08.docDate = day.ToString("yyyy-MM-dd");

                    var beforeqty = queryTankSum.FirstOrDefault(x => x.DocDate == day && x.PeriodNo == 1 && x.PdId == prod.PdId) == null ? 0 : queryTankSum.FirstOrDefault(x => x.DocDate == day && x.PeriodNo == 1 && x.PdId == prod.PdId).BeforeQty;
                    var remainqty = queryTankSum.FirstOrDefault(x => x.DocDate == day && x.PeriodNo == maxperiod && x.PdId == prod.PdId) == null ? 0 : queryTankSum.FirstOrDefault(x => x.DocDate == day && x.PeriodNo == maxperiod && x.PdId == prod.PdId).BalanceQty;
                    gov08.balanceQty = beforeqty ?? 0;
                    gov08.remainQty = remainqty ?? 0;

                    var receiveProds =  queryReceiveProd.Where(x => x.h.DocDate == day && x.d.PdId == prod.PdId)
                        .GroupBy(x=> new {x.d.PdId,x.d.PdName,x.h.DocDate,x.d.ItemQty})
                        .Select(x=> new Gov08Response {
                         pdId = x.Key.PdId,
                         pdName = x.Key.PdName,
                         docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                         receiveQty = x.Sum(s=>s.d.ItemQty)??0
                        })
                        .ToList();

                    var sales = queryTankSum.Where(x => x.DocDate == day && x.PdId == prod.PdId).ToList();
                    if (sales.Count > 0 && receiveProds.Count > 0)
                    {
                        var govrec = new Gov08Response()
                        {
                            pdId = gov08.pdId,
                            pdName = gov08.pdName,
                            docDate = gov08.docDate,
                            receiveQty = receiveProds.Sum(s => s.receiveQty),
                            adjustQty = sales.Sum(x=>x.AdjustQty)??0,
                            balanceQty = gov08.balanceQty,
                            saleQty = sales.Sum(x => x.IssueQty) ?? 0,
                            remainQty = gov08.remainQty,

                            sumYearReceiveQty = sumYearReceiveQty.Where(x=>x.d.PdId == gov08.pdId).Sum(s=> s.d.ItemQty) ?? 0,
                            sumYearSaleQty = sumYearSaleQty.Where(x=>x.PdId == gov08.pdId).Sum(s => s.IssueQty) ?? 0,
                            subYearAdjustQty = sumAdjustQty.Where(x=>x.PdId == gov08.pdId).Sum(s => s.AdjustQty) ?? 0,
                        };
                        gov08list.Add(govrec);
                    }
                    else
                    {
                        // gov08.invNo = "";
                        gov08.receiveQty = 0;
                        gov08.saleQty = sales.Sum(x => x.IssueQty) ?? 0;
                        gov08list.Add(gov08);
                    }

                    day = day.AddDays(1);
                }
            }

            result = gov08list;
            var branch =  (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode &&
                          b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();
            int seq = 0;
            result.ForEach(x =>
            {
                x.seqNo = ++seq;
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.branchNo = branch.b.BranchNo;
                x.brnAddress = branch.b.FullAddress;
                x.compCode = branch.b.CompCode;
                x.compName = branch.c.CompName;
                x.month = req.DateFrom.ToString("MMMM", new CultureInfo("th-TH"));
                x.year = req.DateFrom.ToString("yyyy", new CultureInfo("th-TH"));
                x.balanceQty = x.balanceQty * densityBase;
                x.receiveQty = x.receiveQty * densityBase;
                x.remainQty = x.remainQty * densityBase;
                x.saleQty = x.saleQty * densityBase;
                //x.adjustQty = x.remainQty - (x.balanceQty + x.receiveQty - x.saleQty);
            });

            return  result.OrderBy(x => x.pdId).ThenBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public List<Gov09Response> GetGov09PDF(GovermentRequest req)
        {
            List<Gov09Response> response = new List<Gov09Response>();
            decimal densityBase = 1;
            var density = this.context.MasDensities.OrderByDescending(x => x.StartDate).FirstOrDefault(x => x.CompCode == req.CompCode);
            if (req.UnitType == GovermentRequest.eUnitType.Kilo)
            {
                densityBase = (density == null) ? 0.54m : density.DensityBase ?? 0;
            }

            var brnCodes = new List<string>();
            if (req.BranchType == GovermentRequest.eBranchType.Multi)
            {
                brnCodes = (from mb in this.context.MasBranches
                            join dp in this.context.DopPeriods on new { mb.CompCode, mb.BrnCode } equals new { dp.CompCode, dp.BrnCode }
                            where dp.DocDate == req.DateFrom
                            && mb.ProvCode.StartsWith("10")
                            && mb.CompCode == req.CompCode
                            select mb
                            ).Select(x => x.BrnCode).Distinct().ToList();
            }
            else
            {
                brnCodes.Add(req.BrnCode);
            }

            foreach (var brnCode in brnCodes)
            {
                List<Gov09Response> result = new List<Gov09Response>();
                Gov09Response gov09 = new Gov09Response();
                //1
                var bfdate = this.context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == brnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo).Min(x => x.DocDate).AddDays(-1);                                
                var query = this.context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == brnCode && x.DocDate == bfdate).AsQueryable();
                var countTankSum = query.Count();
                if (countTankSum > 0)
                {
                    var maxperiod = query.Max(x => x.PeriodNo);
                   
                    gov09.seqNo = 1;
                    gov09.description = "คงเหลือยกมา";
                    gov09.itemQty1 = (query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000001") == null ? 0 : query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000001").BalanceQty ?? 0) * densityBase;
                    gov09.itemQty2 = (query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000002") == null ? 0 : query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000002").BalanceQty ?? 0) * densityBase;
                    gov09.itemQty3 = (query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000005") == null ? 0 : query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000005").BalanceQty ?? 0) * densityBase;
                    gov09.itemQty4 = (query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000006") == null ? 0 : query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000006").BalanceQty ?? 0) * densityBase;
                    gov09.itemQty5 = (query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000010") == null ? 0 : query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000010").BalanceQty ?? 0) * densityBase;
                    gov09.itemQty6 = (query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000073") == null ? 0 : query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000073").BalanceQty ?? 0) * densityBase;
                    gov09.itemQty7 = (query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000074") == null ? 0 : query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000074").BalanceQty ?? 0) * densityBase;
                    gov09.itemQty8 = (query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000011") == null ? 0 : query.FirstOrDefault(x => x.PeriodNo == maxperiod && x.PdId == "000011").BalanceQty ?? 0) * densityBase;                   
                }
                else
                {
                    gov09.seqNo = 1;
                    gov09.description = "คงเหลือยกมา";
                    gov09.itemQty1 = 0;
                    gov09.itemQty2 = 0;
                    gov09.itemQty3 = 0;
                    gov09.itemQty4 = 0;
                    gov09.itemQty5 = 0;
                    gov09.itemQty6 = 0;
                    gov09.itemQty7 = 0;
                    gov09.itemQty8 = 0;                    
                }
                result.Add(gov09);

                //2
                query = this.context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == brnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo).AsQueryable();
                gov09 = new Gov09Response();
                gov09.seqNo = 2;
                gov09.description = "ปริมาณการรับเข้า";
                gov09.itemQty1 = (query.Where(x => x.PdId == "000001") == null ? 0 : query.Where(x => x.PdId == "000001").Sum(x => x.ReceiveQty) ?? 0) * densityBase;
                gov09.itemQty2 = (query.Where(x => x.PdId == "000002") == null ? 0 : query.Where(x => x.PdId == "000002").Sum(x => x.ReceiveQty) ?? 0) * densityBase;
                gov09.itemQty3 = (query.Where(x => x.PdId == "000005") == null ? 0 : query.Where(x => x.PdId == "000005").Sum(x => x.ReceiveQty) ?? 0) * densityBase;
                gov09.itemQty4 = (query.Where(x => x.PdId == "000006") == null ? 0 : query.Where(x => x.PdId == "000006").Sum(x => x.ReceiveQty) ?? 0) * densityBase;
                gov09.itemQty5 = (query.Where(x => x.PdId == "000010") == null ? 0 : query.Where(x => x.PdId == "000010").Sum(x => x.ReceiveQty) ?? 0) * densityBase;
                gov09.itemQty6 = (query.Where(x => x.PdId == "000073") == null ? 0 : query.Where(x => x.PdId == "000073").Sum(x => x.ReceiveQty) ?? 0) * densityBase;
                gov09.itemQty7 = (query.Where(x => x.PdId == "000074") == null ? 0 : query.Where(x => x.PdId == "000074").Sum(x => x.ReceiveQty) ?? 0) * densityBase;
                gov09.itemQty8 = (query.Where(x => x.PdId == "000011") == null ? 0 : query.Where(x => x.PdId == "000011").Sum(x => x.ReceiveQty) ?? 0) * densityBase;
                result.Add(gov09);

                //3
                gov09 = new Gov09Response();
                gov09.seqNo = 3;
                gov09.description = "รวมรับเข้า";
                gov09.itemQty1 = result.Where(x => x.seqNo == 1 || x.seqNo == 2).Sum(x => x.itemQty1);
                gov09.itemQty2 = result.Where(x => x.seqNo == 1 || x.seqNo == 2).Sum(x => x.itemQty2);
                gov09.itemQty3 = result.Where(x => x.seqNo == 1 || x.seqNo == 2).Sum(x => x.itemQty3);
                gov09.itemQty4 = result.Where(x => x.seqNo == 1 || x.seqNo == 2).Sum(x => x.itemQty4);
                gov09.itemQty5 = result.Where(x => x.seqNo == 1 || x.seqNo == 2).Sum(x => x.itemQty5);
                gov09.itemQty6 = result.Where(x => x.seqNo == 1 || x.seqNo == 2).Sum(x => x.itemQty6);
                gov09.itemQty7 = result.Where(x => x.seqNo == 1 || x.seqNo == 2).Sum(x => x.itemQty7);
                gov09.itemQty8 = result.Where(x => x.seqNo == 1 || x.seqNo == 2).Sum(x => x.itemQty8);
                result.Add(gov09);

                //4
                gov09 = new Gov09Response();
                gov09.seqNo = 4;
                gov09.description = "ปริมาณการจำหน่าย";
                gov09.itemQty1 = (query.Where(x => x.PdId == "000001") == null ? 0 : query.Where(x => x.PdId == "000001").Sum(x => x.SaleQty) ?? 0) * densityBase;
                gov09.itemQty2 = (query.Where(x => x.PdId == "000002") == null ? 0 : query.Where(x => x.PdId == "000002").Sum(x => x.SaleQty) ?? 0) * densityBase;
                gov09.itemQty3 = (query.Where(x => x.PdId == "000005") == null ? 0 : query.Where(x => x.PdId == "000005").Sum(x => x.SaleQty) ?? 0) * densityBase;
                gov09.itemQty4 = (query.Where(x => x.PdId == "000006") == null ? 0 : query.Where(x => x.PdId == "000006").Sum(x => x.SaleQty) ?? 0) * densityBase;
                gov09.itemQty5 = (query.Where(x => x.PdId == "000010") == null ? 0 : query.Where(x => x.PdId == "000010").Sum(x => x.SaleQty) ?? 0) * densityBase;
                gov09.itemQty6 = (query.Where(x => x.PdId == "000073") == null ? 0 : query.Where(x => x.PdId == "000073").Sum(x => x.SaleQty) ?? 0) * densityBase;
                gov09.itemQty7 = (query.Where(x => x.PdId == "000074") == null ? 0 : query.Where(x => x.PdId == "000074").Sum(x => x.SaleQty) ?? 0) * densityBase;
                gov09.itemQty8 = (query.Where(x => x.PdId == "000011") == null ? 0 : query.Where(x => x.PdId == "000011").Sum(x => x.SaleQty) ?? 0) * densityBase;
                result.Add(gov09);

                //5
                gov09 = new Gov09Response();
                gov09.seqNo = 5;
                gov09.description = "ปริมาณการเบิก";
                gov09.itemQty1 = (query.Where(x => x.PdId == "000001") == null ? 0 : query.Where(x => x.PdId == "000001").Sum(x => x.WithdrawQty) ?? 0) * densityBase;
                gov09.itemQty2 = (query.Where(x => x.PdId == "000002") == null ? 0 : query.Where(x => x.PdId == "000002").Sum(x => x.WithdrawQty) ?? 0) * densityBase;
                gov09.itemQty3 = (query.Where(x => x.PdId == "000005") == null ? 0 : query.Where(x => x.PdId == "000005").Sum(x => x.WithdrawQty) ?? 0) * densityBase;
                gov09.itemQty4 = (query.Where(x => x.PdId == "000006") == null ? 0 : query.Where(x => x.PdId == "000006").Sum(x => x.WithdrawQty) ?? 0) * densityBase;
                gov09.itemQty5 = (query.Where(x => x.PdId == "000010") == null ? 0 : query.Where(x => x.PdId == "000010").Sum(x => x.WithdrawQty) ?? 0) * densityBase;
                gov09.itemQty6 = (query.Where(x => x.PdId == "000073") == null ? 0 : query.Where(x => x.PdId == "000073").Sum(x => x.WithdrawQty) ?? 0) * densityBase;
                gov09.itemQty7 = (query.Where(x => x.PdId == "000074") == null ? 0 : query.Where(x => x.PdId == "000074").Sum(x => x.WithdrawQty) ?? 0) * densityBase;
                gov09.itemQty8 = (query.Where(x => x.PdId == "000011") == null ? 0 : query.Where(x => x.PdId == "000011").Sum(x => x.WithdrawQty) ?? 0) * densityBase;
                result.Add(gov09);

                //6
                gov09 = new Gov09Response();
                gov09.seqNo = 6;
                gov09.description = "รวมจ่าย";
                gov09.itemQty1 = result.Where(x => x.seqNo == 4 || x.seqNo == 5).Sum(x => x.itemQty1);
                gov09.itemQty2 = result.Where(x => x.seqNo == 4 || x.seqNo == 5).Sum(x => x.itemQty2);
                gov09.itemQty3 = result.Where(x => x.seqNo == 4 || x.seqNo == 5).Sum(x => x.itemQty3);
                gov09.itemQty4 = result.Where(x => x.seqNo == 4 || x.seqNo == 5).Sum(x => x.itemQty4);
                gov09.itemQty5 = result.Where(x => x.seqNo == 4 || x.seqNo == 5).Sum(x => x.itemQty5);
                gov09.itemQty6 = result.Where(x => x.seqNo == 4 || x.seqNo == 5).Sum(x => x.itemQty6);
                gov09.itemQty7 = result.Where(x => x.seqNo == 4 || x.seqNo == 5).Sum(x => x.itemQty7);
                gov09.itemQty8 = result.Where(x => x.seqNo == 4 || x.seqNo == 5).Sum(x => x.itemQty8);
                result.Add(gov09);

                //7
                gov09 = new Gov09Response();
                gov09.seqNo = 7;
                gov09.description = "ปริมาณการปรับปรุง";
                gov09.itemQty1 = (query.Where(x => x.PdId == "000001") == null ? 0 : query.Where(x => x.PdId == "000001").Sum(x => x.AdjustQty) ?? 0) * densityBase;
                gov09.itemQty2 = (query.Where(x => x.PdId == "000002") == null ? 0 : query.Where(x => x.PdId == "000002").Sum(x => x.AdjustQty) ?? 0) * densityBase;
                gov09.itemQty3 = (query.Where(x => x.PdId == "000005") == null ? 0 : query.Where(x => x.PdId == "000005").Sum(x => x.AdjustQty) ?? 0) * densityBase;
                gov09.itemQty4 = (query.Where(x => x.PdId == "000006") == null ? 0 : query.Where(x => x.PdId == "000006").Sum(x => x.AdjustQty) ?? 0) * densityBase;
                gov09.itemQty5 = (query.Where(x => x.PdId == "000010") == null ? 0 : query.Where(x => x.PdId == "000010").Sum(x => x.AdjustQty) ?? 0) * densityBase;
                gov09.itemQty6 = (query.Where(x => x.PdId == "000073") == null ? 0 : query.Where(x => x.PdId == "000073").Sum(x => x.AdjustQty) ?? 0) * densityBase;
                gov09.itemQty7 = (query.Where(x => x.PdId == "000074") == null ? 0 : query.Where(x => x.PdId == "000074").Sum(x => x.AdjustQty) ?? 0) * densityBase;
                gov09.itemQty8 = (query.Where(x => x.PdId == "000011") == null ? 0 : query.Where(x => x.PdId == "000011").Sum(x => x.AdjustQty) ?? 0) * densityBase;
                result.Add(gov09);

                //8
                gov09 = new Gov09Response();
                gov09.seqNo = 8;
                gov09.description = "คงเหลือยกไป";
                gov09.itemQty1 = result.FirstOrDefault(x => x.seqNo == 3).itemQty1 - result.FirstOrDefault(x => x.seqNo == 6).itemQty1 + result.FirstOrDefault(x => x.seqNo == 7).itemQty1;
                gov09.itemQty2 = result.FirstOrDefault(x => x.seqNo == 3).itemQty2 - result.FirstOrDefault(x => x.seqNo == 6).itemQty2 + result.FirstOrDefault(x => x.seqNo == 7).itemQty2;
                gov09.itemQty3 = result.FirstOrDefault(x => x.seqNo == 3).itemQty3 - result.FirstOrDefault(x => x.seqNo == 6).itemQty3 + result.FirstOrDefault(x => x.seqNo == 7).itemQty3;
                gov09.itemQty4 = result.FirstOrDefault(x => x.seqNo == 3).itemQty4 - result.FirstOrDefault(x => x.seqNo == 6).itemQty4 + result.FirstOrDefault(x => x.seqNo == 7).itemQty4;
                gov09.itemQty5 = result.FirstOrDefault(x => x.seqNo == 3).itemQty5 - result.FirstOrDefault(x => x.seqNo == 6).itemQty5 + result.FirstOrDefault(x => x.seqNo == 7).itemQty5;
                gov09.itemQty6 = result.FirstOrDefault(x => x.seqNo == 3).itemQty6 - result.FirstOrDefault(x => x.seqNo == 6).itemQty6 + result.FirstOrDefault(x => x.seqNo == 7).itemQty6;
                gov09.itemQty7 = result.FirstOrDefault(x => x.seqNo == 3).itemQty7 - result.FirstOrDefault(x => x.seqNo == 6).itemQty7 + result.FirstOrDefault(x => x.seqNo == 7).itemQty7;
                gov09.itemQty8 = result.FirstOrDefault(x => x.seqNo == 3).itemQty8 - result.FirstOrDefault(x => x.seqNo == 6).itemQty8 + result.FirstOrDefault(x => x.seqNo == 7).itemQty8;
                result.Add(gov09);

                var branch = (from b in this.context.MasBranches
                              join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                              where b.CompCode == req.CompCode &&
                              b.BrnCode == brnCode
                              select new { b, c }).FirstOrDefault();
                result.ForEach(x =>
                {
                    x.brnCode = branch.b.BrnCode;
                    x.brnName = branch.b.BrnName;
                    x.branchNo = branch.b.BranchNo;
                    x.brnAddress = branch.b.FullAddress;
                    x.compCode = branch.b.CompCode;
                    x.compName = branch.c.CompName;
                    x.month = req.DateFrom.ToString("MMMM", new CultureInfo("th-TH"));
                    x.year = req.DateFrom.ToString("yyyy", new CultureInfo("th-TH"));
                });

                response.AddRange(result);
            }


            return response;
        }

        public List<Gov11Response> GetGov11PDF(GovermentRequest req)
        {
            List<Gov11Response> response = new List<Gov11Response>();

            decimal densityBase = 1;
            var density = this.context.MasDensities.OrderByDescending(x => x.StartDate).FirstOrDefault(x => x.CompCode == req.CompCode);
            if (req.UnitType == GovermentRequest.eUnitType.Kilo)
            {
                densityBase = (density == null) ? 0.54m : density.DensityBase ?? 0;
            }
            var branchtax = this.context.MasBranchTaxes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).ToList();

            var branchs = (from b in this.context.MasBranches
                           join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                           where b.CompCode == req.CompCode
                           select new { b, c }).AsQueryable();

            var branchconfigs = this.context.MasBranchConfigs.Where(x => x.CompCode == req.CompCode).AsQueryable();

            var brnCodes = new List<string>();
            if (req.BranchType == GovermentRequest.eBranchType.Multi)
            {
                brnCodes = (from mb in this.context.MasBranches
                            join dp in this.context.DopPeriods on new { mb.CompCode, mb.BrnCode } equals new { dp.CompCode, dp.BrnCode }
                            where dp.DocDate == req.DateFrom
                            && mb.ProvCode.StartsWith("10")
                            && mb.CompCode == req.CompCode
                            select mb
                            ).Select(x => x.BrnCode).Distinct().ToList();
            }
            else
            {
                brnCodes.Add(req.BrnCode);
            }
            foreach (var brnCode in brnCodes)
            {

                List<Gov11Response> items = new List<Gov11Response>();

                items = this.context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode
                                                                    && x.BrnCode == brnCode
                                                                    && x.DocDate >= req.DateFrom
                                                                    && x.DocDate <= req.DateTo)
                                                                .GroupBy(x => new { x.PdId, x.PdName })
                                                                .Select(x => new Gov11Response
                                                                {
                                                                    pdId = x.Key.PdId,
                                                                    pdName = x.Key.PdName,
                                                                    itemQty = Math.Round((x.Sum(s => s.TotalQty) ?? 0) * densityBase, 2)
                                                                })
                                                                .Where(x => x.itemQty > 0).ToList();


                var branch = branchs.FirstOrDefault(x => x.b.BrnCode == brnCode);
                var config = branchconfigs.FirstOrDefault(x => x.BrnCode == brnCode);

                int seqNo = 1;
                items.ForEach(x =>
                        {
                            x.seqNo = seqNo++;
                            x.vatRate = branchtax.Any(t => t.TaxId == x.pdId) ? branchtax.FirstOrDefault(t => t.TaxId == x.pdId).TaxAmt : 0;
                            x.subAmt = x.itemQty * x.vatRate;
                            x.compCode = branch.c.CompCode;
                            x.compName = branch.c.CompName;
                            x.compAddress = branch.c.Address ?? "";
                            x.registerId = branch.c.RegisterId;
                            x.month = req.DateFrom.ToString("MMMM", new CultureInfo("th-TH"));
                            x.year = req.DateFrom.ToString("yyyy", new CultureInfo("th-TH"));
                            x.brnCode = branch.b.BrnCode;
                            x.brnName = branch.b.BrnName;
                            x.branchNo = branch.b.BranchNo;
                            x.brnAddress = branch.b.Address ?? "";
                            x.postCode = branch.b.Postcode ?? "";
                            x.phone = branch.b.Phone ?? "";
                            x.trader = (config != null) ? config.Trader : "";
                            x.traderPosition = (config != null) ? config.TraderPosition : "";
                        });

                response.AddRange(items);
            }

            decimal totalAmt = response.Sum(s => s.subAmt);
            string totalAmtText = Function.ThaiBahtText(totalAmt);
            response.ForEach(x =>
            {
                x.totalAmt = totalAmt;
                x.totalAmtText = totalAmtText;
            });
            return response;
        }



        //public Gov05Response GetGov05PDF(GovermentRequest req)
        //{
        //    Gov05Response result = new Gov05Response();
        //    //var branch = this.context.MasBranches.FirstOrDefault(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode);
        //    List<string> months = new List<string>() {"", "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };
        //    var period = (req.Period != null)?req.Period.Split("-"):null;
        //    var mm = (period != null)? Convert.ToInt32(period.First()):0;
        //    var yy = (period != null) ? Convert.ToInt32(period.Last()) : 0;

        //    var branch = (from b in this.context.MasBranches
        //                 join c in this.context.MasCompanies on b.CompCode equals c.CompCode
        //                 where b.CompCode == req.CompCode &&
        //                 b.BrnCode == req.BrnCode
        //                 select new { b, c }).FirstOrDefault();

        //    result.brnCode = branch.b.BrnCode;
        //    result.brnName = branch.b.BrnName;
        //    result.branchNo = branch.b.BranchNo;
        //    result.brnAddress = branch.b.FullAddress;
        //    result.compName = branch.c.CompName;
        //    result.month = months[mm];
        //    result.year = (period != null)?period.Last():"";

        //    var query = (from h in this.context.InvReceiveProdHds
        //                  join d in this.context.InvReceiveProdDts on new { h.CompCode, h.BrnCode, h.LocCode, h.DocNo } equals new { d.CompCode, d.BrnCode, d.LocCode, d.DocNo }
        //                  join p in this.context.MasProducts on d.PdId equals p.PdId
        //                  where h.DocStatus != "Cancel"
        //                  && h.DocDate.Value.Month == mm && h.DocDate.Value.Year == yy
        //                  && p.GroupId == "0000"
        //                  && h.CompCode == req.CompCode && h.BrnCode == req.BrnCode
        //                  select new { h.SupCode, h.SupName, d.PdId, d.PdName, d.ItemQty }
        //                  ).GroupBy(x => new { x.SupCode, x.SupName, x.PdId, x.PdName })
        //                  .Select(x => new Gov05Detail
        //                  {
        //                      supCode = x.Key.SupCode,
        //                      supName = x.Key.SupName,
        //                      pdId = x.Key.PdId,
        //                      pdName = x.Key.PdName,
        //                      itemQty = x.Sum(s => s.ItemQty) ?? 0
        //                  }).ToList();

        //    var supcode = query.GroupBy(x => new { x.supCode,x.supName }).Select(x => x.Key).ToList();
        //    List<Gov05Detail> details = new List<Gov05Detail>();
        //    int seq = 0;
        //    foreach(var sup in supcode)
        //    {
        //        Gov05Detail detail = new Gov05Detail();
        //        detail.seqNo = ++seq;
        //        detail.supCode = sup.supCode;
        //        detail.supName = sup.supName;
        //        detail.itemQty000001 = query.Where(x => x.supCode == sup.supCode && x.pdId == "000001").Sum(x=>x.itemQty);
        //        detail.itemQty000002 = query.Where(x => x.supCode == sup.supCode && x.pdId == "000002").Sum(x => x.itemQty);
        //        detail.itemQty000004 = query.Where(x => x.supCode == sup.supCode && x.pdId == "000004").Sum(x => x.itemQty);
        //        detail.itemQty000005 = query.Where(x => x.supCode == sup.supCode && x.pdId == "000005").Sum(x => x.itemQty);
        //        detail.itemQty000006 = query.Where(x => x.supCode == sup.supCode && x.pdId == "000006").Sum(x => x.itemQty);
        //        detail.itemQty000010 = query.Where(x => x.supCode == sup.supCode && x.pdId == "000010").Sum(x => x.itemQty);
        //        detail.itemQty000073 = query.Where(x => x.supCode == sup.supCode && x.pdId == "000073").Sum(x => x.itemQty);
        //        detail.itemQty000074 = query.Where(x => x.supCode == sup.supCode && x.pdId == "000074").Sum(x => x.itemQty);
        //        details.Add(detail);
        //    }

        //    result.detailList = details;
        //    return result;
        //}
    }
}
