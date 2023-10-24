using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Requests.MeterRequest;
using static Report.API.Domain.Models.Response.MeterResponse;

namespace Report.API.Repositories
{
    public class MeterRepository : SqlDataAccessHelper, IMeterRepository
    {
        public MeterRepository(PTMaxstationContext context) : base(context)
        {
        }

        public List<MeterTestResponse> GetMeterTestPDF(MeterTestResquest req)
        {
            List<MeterTestResponse> response = new List<MeterTestResponse>();

            var query = (from p in this.context.DopPeriods
                         join m in this.context.DopPeriodMeters on new { p.CompCode, p.BrnCode, p.DocDate, p.PeriodNo } equals new { m.CompCode, m.BrnCode, m.DocDate, m.PeriodNo }
                         where p.CompCode == req.CompCode
                         && p.BrnCode == req.BrnCode
                         && p.DocDate >= req.DateFrom
                         && p.DocDate <= req.DateTo
                         && (m.TestQty > 0 || m.RepairQty > 0)
                         select new { p, m }
                         ).AsQueryable();

            response = query.Select(x => new MeterTestResponse
            {
                docDate = x.p.DocDate.ToString("yyyy-MM-dd"),
                periodNo = x.p.PeriodNo,
                timeStart = x.p.TimeStart,
                timeFinish = x.p.TimeFinish,
                dispId = x.m.DispId,
                pdName = x.m.PdName,
                meterStart = x.m.MeterStart ?? 0,
                meterFinish = x.m.MeterFinish ?? 0,
                remark = x.m.Remark ?? "",
                testQty = x.m.TestQty ?? 0,
                testAmt = (x.m.Unitprice * x.m.TestQty) ?? 0,
                totalQty = x.m.TotalQty ?? 0,
                totalAmt = x.m.SaleAmt ?? 0
            }).ToList();


            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode &&
                          b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.registerId = branch.c.RegisterId;
                x.compImage = branch.c.CompImage;
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.brnAddress = branch.b.Address;
            });
            return response.OrderBy(x => x.docDate).ThenBy(x => x.periodNo).ThenBy(x => x.dispId).ToList();
        }

        public MemoryStream GetMeterTestExcel(MeterTestResquest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = this.context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = this.context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = GetMeterTestPDF(req);
            var docDate = data.Select(x => x.docDate).Distinct();

            int rowIndex = 1;
            //Title
            var title = ws.Cells[rowIndex, 4, rowIndex, 8];
            title.Merge = true;
            title.Value = "รายงานบีบทดสอบมิเตอร์หัวจ่าย";

            //Title
            var date = ws.Cells[rowIndex, 9, rowIndex, 9];
            date.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            foreach (var docdate in docDate)
            {
                #region title
                var ddate = DateTime.Parse(docdate);
                //docDate
                var cDate = ws.Cells[rowIndex, 4, rowIndex, 8];
                cDate.Merge = true;
                cDate.Value = $"ณ วันที่ {ddate.ToString("dd/MM/yyyy")}";
                rowIndex++;

                //Company
                var companyTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
                companyTitle.Merge = true;
                companyTitle.Value = "ชื่อผู้ประกอบการ";
                //CompanyTitle
                var companyTitleName = ws.Cells[rowIndex, 3, rowIndex, 5];
                companyTitleName.Merge = true;
                companyTitleName.Value = company.CompName;
                //Tax
                var taxTitle = ws.Cells[rowIndex, 8, rowIndex, 8];
                taxTitle.Merge = true;
                taxTitle.Value = "เลขประจําตัวผู้เสียภาษีอากร";
                //TaxNo.
                var taxTitleName = ws.Cells[rowIndex, 9, rowIndex, 9];
                taxTitleName.Merge = true;
                taxTitleName.Value = data.FirstOrDefault().registerId;
                rowIndex++;

                //Branch
                var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
                branchTitle.Merge = true;
                branchTitle.Value = "ชื่อสถานีบริการน้ำมัน";
                var branchTitleName = ws.Cells[rowIndex, 3, rowIndex, 5];
                branchTitleName.Merge = true;
                branchTitleName.Value = brn.BrnName;
                rowIndex++;

                //Address
                var addrTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
                addrTitle.Merge = true;
                addrTitle.Value = "ที่อยู่";
                var addressTitle = ws.Cells[rowIndex, 5, rowIndex, 5];
                addressTitle.Merge = true;
                addressTitle.Value = data.FirstOrDefault().brnAddress;
                rowIndex++;
                #endregion

                #region column header
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Value = "กะที่";
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Merge = true;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Value = "เวลา";
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Merge = true;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Value = "หัวจ่าย";
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Merge = true;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Value = "ผลิตภัณฑ์";
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Merge = true;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 5, rowIndex, 5].Value = "มิเตอร์";
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Value = "ก่อนเปลี่ยน";
                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 6, rowIndex, 6].Value = "มิเตอร์";
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Value = "หลังเปลี่ยน";
                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 7, rowIndex, 7].Value = "ปริมาณ";
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Value = "ผลต่าง";
                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Value = "บาท";
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Merge = true;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Value = "สาเหตุ";
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Merge = true;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                rowIndex += 2;
                #endregion

                #region data
                var items = data.Where(x => x.docDate == docdate).ToList();
                foreach (var item in items)
                {
                    ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.periodNo;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.timeStart + " - " + item.timeFinish;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.dispId;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 4, rowIndex, 4].Value = item.pdName;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.meterStart;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.meterFinish;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.testQty;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.testAmt;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.remark;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    rowIndex++;
                }
                #endregion

                rowIndex+=2;
            }

            return new MemoryStream(pck.GetAsByteArray());
        }

        public List<MeterRepairResponse> GetMeterRepairPDF(MeterTestResquest req)
        {
            List<MeterRepairResponse> response = new List<MeterRepairResponse>();

            var query = (from p in this.context.DopPeriods
                         join m in this.context.DopPeriodMeters on new { p.CompCode, p.BrnCode, p.DocDate, p.PeriodNo } equals new { m.CompCode, m.BrnCode, m.DocDate, m.PeriodNo }
                         where p.CompCode == req.CompCode
                         && p.BrnCode == req.BrnCode
                         && p.DocDate >= req.DateFrom
                         && p.DocDate <= req.DateTo
                         && ( m.RepairQty > 0)
                         select new { p, m }
                         ).AsQueryable();

            response = query.Select(x => new MeterRepairResponse
            {
                docDate = x.p.DocDate.ToString("yyyy-MM-dd"),
                periodNo = x.p.PeriodNo,
                timeStart = x.p.TimeStart,
                timeFinish = x.p.TimeFinish,
                dispId = x.m.DispId,
                pdName = x.m.PdName,
                repairStart = x.m.RepairStart ?? 0,
                repairFinish = x.m.RepairFinish ?? 0,
                remark = x.m.Remark ?? "",
                repairQty = x.m.RepairQty ?? 0,
                repairAmt = x.m.RepairAmt ?? 0
            }).ToList();


            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode &&
                          b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.registerId = branch.c.RegisterId;
                x.compImage = branch.c.CompImage;
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.brnAddress = branch.b.Address;
            });
            return response.OrderBy(x => x.docDate).ThenBy(x => x.periodNo).ThenBy(x => x.dispId).ToList();
        }

        public MemoryStream GetMeterRepairExcel(MeterTestResquest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = this.context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = this.context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetMeterRepairPDF(req);
            var docDate = data.Select(x => x.docDate).Distinct();

            int rowIndex = 1;
            //Title
            var title = ws.Cells[rowIndex, 4, rowIndex, 8];
            title.Merge = true;
            title.Value = "รายงานการเปลี่ยนแปลงตัวเลขมิเตอร์";

            //Title
            var date = ws.Cells[rowIndex, 9, rowIndex, 9];
            date.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            foreach (var docdate in docDate)
            {
                #region title
                var ddate = DateTime.Parse(docdate);
                //docDate
                var cDate = ws.Cells[rowIndex, 4, rowIndex, 8];
                cDate.Merge = true;
                cDate.Value = $"ณ วันที่ {ddate.ToString("dd/MM/yyyy")}";
                rowIndex++;

                //Company
                var companyTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
                companyTitle.Merge = true;
                companyTitle.Value = "ชื่อผู้ประกอบการ";
                //CompanyTitle
                var companyTitleName = ws.Cells[rowIndex, 3, rowIndex, 5];
                companyTitleName.Merge = true;
                companyTitleName.Value = company.CompName;
                //Tax
                var taxTitle = ws.Cells[rowIndex, 8, rowIndex, 8];
                taxTitle.Merge = true;
                taxTitle.Value = "เลขประจําตัวผู้เสียภาษีอากร";
                //TaxNo.
                var taxTitleName = ws.Cells[rowIndex, 9, rowIndex, 9];
                taxTitleName.Merge = true;
                taxTitleName.Value = data.FirstOrDefault().registerId;
                rowIndex++;

                //Branch
                var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
                branchTitle.Merge = true;
                branchTitle.Value = "ชื่อสถานีบริการน้ำมัน";
                var branchTitleName = ws.Cells[rowIndex, 3, rowIndex, 5];
                branchTitleName.Merge = true;
                branchTitleName.Value = brn.BrnName;
                rowIndex++;

                //Address
                var addrTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
                addrTitle.Merge = true;
                addrTitle.Value = "ที่อยู่";
                var addressTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
                addressTitle.Merge = true;
                addressTitle.Value = data.FirstOrDefault().brnAddress;
                rowIndex++;
                #endregion

                #region column header
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Value = "กะที่";
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Merge = true;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 1, rowIndex + 1, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Value = "เวลา";
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Merge = true;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex + 1, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Value = "หัวจ่าย";
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Merge = true;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex + 1, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Value = "ผลิตภัณฑ์";
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Merge = true;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 4, rowIndex + 1, 4].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 5, rowIndex, 5].Value = "มิเตอร์";
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Value = "ก่อนเปลี่ยน";
                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex + 1, 5, rowIndex + 1, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 6, rowIndex, 6].Value = "มิเตอร์";
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Value = "หลังเปลี่ยน";
                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 7, rowIndex, 7].Value = "ปริมาณ";
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Value = "ผลต่าง";
                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Value = "บาท";
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Merge = true;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex + 1, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Value = "สาเหตุ";
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Merge = true;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex + 1, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                rowIndex += 2;
                #endregion

                #region data
                var items = data.Where(x => x.docDate == docdate).ToList();
                foreach (var item in items)
                {
                    ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.periodNo;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.timeStart + " - " + item.timeFinish;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.dispId;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 4, rowIndex, 4].Value = item.pdName;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.repairStart;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.repairFinish;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.repairQty;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.repairAmt;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                    ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.remark;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    rowIndex++;
                }
                #endregion

                rowIndex += 2;
            }

            return new MemoryStream(pck.GetAsByteArray());
        }
    }
}