using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Response.StationResponse;

namespace Report.API.Repositories
{
    public class StationRepository : BaseRepositories, IStationRepository //SqlDataAccessHelper
    {
        private readonly IMapper _mapper;
        public StationRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }


        public List<ST312Response> GetST312PDF(StationRequest req)
        {
            List<ST312Response> response = new List<ST312Response>();

            var query = this.Context.DopPeriodEmps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo).ToList();

            var day = req.DateFrom;
            while (day <= req.DateTo)
            {
                ST312Response item = new ST312Response();
                item.docDate = day.ToString("yyyy-MM-dd");

                item.empList = query.Where(x => x.DocDate == day).GroupBy(x => new { x.EmpCode, x.EmpName }).Select(x => new ST312Detail
                {
                    empCode = x.Key.EmpCode,
                    empName = x.Key.EmpName,
                    periodNo = string.Join(",", x.Select(x => x.PeriodNo))
                }).ToList();

                response.Add(item);
                day = day.AddDays(1);
            }

            return response;
        }

        public List<ST313Response> GetST313PDF(StationRequest req)
        {
            List<ST313Response> response = new List<ST313Response>();

            var dopcash = this.Context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo)
                                                        .GroupBy(x => new { x.DocDate })
                                                        .Select(x => new
                                                        {
                                                            DocDate = x.Key.DocDate,
                                                            MeterAmt = x.Sum(s => s.MeterAmt) ?? 0,
                                                            SaleAmt = x.Sum(s => s.SaleAmt) ?? 0,
                                                            CreditAmt = x.Sum(s => s.CreditAmt) ?? 0,
                                                            CashAmt = x.Sum(s => s.SaleAmt - s.CreditAmt - s.DiscAmt) ?? 0
                                                        }).AsQueryable();

            var dopgl = this.Context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo)
                                                        .GroupBy(x => new { x.DocDate, x.GlType })
                                                        .Select(x => new
                                                        {
                                                            DocDate = x.Key.DocDate,
                                                            GlType = x.Key.GlType,
                                                            GlAmt = x.Sum(s => s.GlAmt) ?? 0
                                                        }).AsQueryable();

            var cashsum = this.Context.DopPeriodCashSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo)
                                                        .GroupBy(x => new { x.DocDate })
                                                        .Select(x => new
                                                        {
                                                            DocDate = x.Key.DocDate,
                                                            CashAmt = x.Sum(s => s.CashAmt) ?? 0,
                                                            DiffAmt = x.Sum(s => s.DiffAmt) ?? 0
                                                        }).AsQueryable();

            response = (from cs in cashsum
                        join ca in dopcash on cs.DocDate equals ca.DocDate
                        select new { cs, ca }).Select(x => new ST313Response
                        {
                            docDate = x.cs.DocDate.ToString("yyyy-MM-dd"),
                            diffAmt = x.cs.DiffAmt,
                            totalAmt = x.cs.CashAmt,
                            saleQty = x.ca.MeterAmt,
                            saleAmt = x.ca.SaleAmt,
                            creditAmt = x.ca.CreditAmt,
                            cashAmt = x.ca.CashAmt,

                        }).ToList();


            var branch = (from b in this.Context.MasBranches
                          join c in this.Context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                          && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.branchNo = branch.b.BranchNo;
                x.brnAddress = branch.b.Address;
                x.cashReceiveAmt = dopgl.FirstOrDefault(g => g.DocDate == Convert.ToDateTime(x.docDate) && g.GlType == "CR").GlAmt;
                x.cashPaymentAmt = dopgl.FirstOrDefault(g => g.DocDate == Convert.ToDateTime(x.docDate) && g.GlType == "DR").GlAmt;
            });

            return response;
        }

        public MemoryStream GetST313Excel(StationRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            // Title Company
            var headerCompany = ws.Cells[1, 4, 1, 6];
            headerCompany.Merge = true;
            headerCompany.Value = $"{company.CompName}";
            headerCompany.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var headerPrintDate = ws.Cells[1, 8, 1, 9];
            headerPrintDate.Merge = true;
            headerPrintDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // Report Title
            var headerTitle = ws.Cells[2, 4, 2, 6];
            headerTitle.Merge = true;
            headerTitle.Value = "รายงานขายประจำวัน";
            headerTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Branch Name
            ws.Cells[3, 1, 3, 1].Merge = true;
            ws.Cells[3, 1, 3, 1].Value = "สาขา :";

            ws.Cells[3, 2, 3, 3].Merge = true;
            ws.Cells[3, 2, 3, 3].Value = brn.BrnCode + " - " + brn.BrnName;

            // Date Range
            var headerDateRange = ws.Cells[3, 4, 3, 6];
            headerDateRange.Merge = true;
            headerDateRange.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            headerDateRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[4, 1, 6, 1].Merge = true;
            ws.Cells[4, 1, 6, 1].Value = "วันที่";
            ws.Cells[4, 1, 6, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 1, 6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 1, 6, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 6, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 6, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 6, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 2, 4, 5].Merge = true;
            ws.Cells[4, 2, 4, 5].Value = "ยอดขายน้ํามันใสหน้าลาน";
            ws.Cells[4, 2, 4, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 2, 4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 2, 4, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 2, 4, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 2, 4, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 2, 5, 3].Merge = true;
            ws.Cells[5, 2, 5, 3].Value = "ยอดขายรวม";
            ws.Cells[5, 2, 5, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 2, 5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 2, 5, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 2, 5, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 2, 5, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;

            ws.Cells[6, 2, 6, 2].Value = "ลิตร";
            ws.Cells[6, 2, 6, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[6, 2, 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[6, 2, 6, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 2, 6, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 2, 6, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 2, 6, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[6, 3, 6, 3].Value = "บาท";
            ws.Cells[6, 3, 6, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[6, 3, 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[6, 3, 6, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 3, 6, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 3, 6, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 3, 6, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 4, 5, 4].Value = "ขายเชื่อ";
            ws.Cells[5, 4, 5, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 4, 5, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 4, 5, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 4, 5, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 4, 5, 4].Style.Border.Right.Style = ExcelBorderStyle.Medium;

            ws.Cells[6, 4, 6, 4].Value = "บาท";
            ws.Cells[6, 4, 6, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[6, 4, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[6, 4, 6, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 4, 6, 4].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 4, 6, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 5, 5, 5].Value = "ขายสด";
            ws.Cells[5, 5, 5, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 5, 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 5, 5, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 5, 5, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 5, 5, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;

            ws.Cells[6, 5, 6, 5].Value = "บาท";
            ws.Cells[6, 5, 6, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[6, 5, 6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[6, 5, 6, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 5, 6, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[6, 5, 6, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 6, 6, 6].Merge = true;
            ws.Cells[4, 6, 6, 6].Value = "รายรับเงินสดหน้าลาน";
            ws.Cells[4, 6, 6, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 6, 6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 6, 6, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 6, 6, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 6, 6, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 6, 6, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 7, 6, 7].Merge = true;
            ws.Cells[4, 7, 6, 7].Value = "รายจ่าย";
            ws.Cells[4, 7, 6, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 7, 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 7, 6, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 7, 6, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 7, 6, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 7, 6, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 8, 6, 8].Merge = true;
            ws.Cells[4, 8, 6, 8].Value = "เงิน(ขาด)เกินหน้าลาน";
            ws.Cells[4, 8, 6, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 8, 6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 8, 6, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 8, 6, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 8, 6, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 8, 6, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 9, 6, 9].Merge = true;
            ws.Cells[4, 9, 6, 9].Value = "รวมยอดเงินสด";
            ws.Cells[4, 9, 6, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 9, 6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 9, 6, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 9, 6, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 9, 6, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 9, 6, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            var data = GetST313PDF(req);
            int rowIndex = 7;
            decimal totalSaleQty = 0;
            decimal totalSaleAmt = 0;
            decimal totalCreditAmt = 0;
            decimal totalCashAmt = 0;
            decimal totalCashReceiveAmt = 0;
            decimal totalCashPaymentAmt = 0;
            decimal totalDiffAmt = 0;
            decimal totalAmt = 0;
            foreach(var item in data)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = Convert.ToDateTime(item.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.saleQty;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalSaleQty += item.saleQty;

                ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.saleAmt;
                ws.Cells[rowIndex, 3, rowIndex, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalSaleAmt += item.saleAmt;

                ws.Cells[rowIndex, 4, rowIndex, 4].Value = item.creditAmt;
                ws.Cells[rowIndex, 4, rowIndex, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 4, rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalCreditAmt += item.creditAmt;

                ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.cashAmt;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalCashAmt += item.cashAmt;

                ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.cashReceiveAmt;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalCashReceiveAmt += item.cashReceiveAmt;

                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.cashPaymentAmt;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalCashPaymentAmt += item.cashPaymentAmt;

                ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.diffAmt;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalDiffAmt += item.diffAmt;

                ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.totalAmt;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalAmt += item.totalAmt;

                rowIndex++;
            }

            ws.Cells[rowIndex, 1, rowIndex, 1].Value = "รวม";
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 2, rowIndex, 2].Value = totalSaleQty;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 3, rowIndex, 3].Value = totalSaleAmt;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 4, rowIndex, 4].Value = totalCreditAmt;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 5, rowIndex, 5].Value = totalCashAmt;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 6, rowIndex, 6].Value = totalCashReceiveAmt;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 7, rowIndex, 7].Value = totalCashPaymentAmt;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 8, rowIndex, 8].Value = totalDiffAmt;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 9, rowIndex, 9].Value = totalAmt;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            return new MemoryStream(pck.GetAsByteArray());
        }

        public List<ST315Response> GetST315PDF(StationRequest req)
        {
            List<ST315Response> response = new List<ST315Response>();

            var sql = @$"declare @datefrom date = '{req.DateFrom.ToString("yyyy-MM-dd")}'
							declare @dateto date = '{req.DateTo.ToString("yyyy-MM-dd")}'
							declare @compcode varchar(10) = '{req.CompCode}'
							declare @brncode varchar(10) = '{req.BrnCode}'

							select a.CompCode,a.BrnCode,a.DocDate
							,isnull(a.p01,0) as p01,isnull(a.p02,0) as p02,isnull(a.p03,0) as p03,isnull(a.p04,0) as p04,isnull(a.p05,0) as p05,isnull(a.p06,0) as p06,isnull(a.p07,0) as p07,isnull(a.p08,0) as p08,isnull(a.p09,0) as p09
							,(a.p01+a.p02+a.p03+a.p04+a.p05+a.p06+a.p07+a.p08+a.p09) as sumOil
							,case b.d01 when 0 then 0 else round(a.p01 *100/b.d01,2) end as d01
							,case b.d02 when 0 then 0 else round(a.p02 *100/b.d02,2) end as d02
							,case b.d03 when 0 then 0 else round(a.p03 *100/b.d03,2) end as d03
							,case b.d04 when 0 then 0 else round(a.p04 *100/b.d04,2) end as d04
							,case b.d05 when 0 then 0 else round(a.p05 *100/b.d05,2) end as d05
							,case b.d06 when 0 then 0 else round(a.p06 *100/b.d06,2) end as d06
							,case b.d07 when 0 then 0 else round(a.p07 *100/b.d07,2) end as d07
							,case b.d08 when 0 then 0 else round(a.p08 *100/b.d08,2) end as d08
							,case b.d09 when 0 then 0 else round(a.p09 *100/b.d09,2) end as d09
							,(b.d01+b.d02+b.d03+b.d04+b.d05+b.d06+b.d07+b.d08+b.d09) as sumSale
							from (
								SELECT COMP_CODE as CompCode,BRN_CODE as BrnCode,DOC_DATE as DocDate
								,isnull([000001],0) as p01,isnull([000002],0)  as p02,isnull([000004],0)  as p03,isnull([000005],0)  as p04,isnull([000006],0)  as p05,isnull([000010],0)  as p06,isnull([000011],0)  as p07,isnull([000073],0)  as p08,isnull([000074],0)  as p09
								FROM (
									select tk.COMP_CODE, tk.BRN_CODE, tk.DOC_DATE, tk.PD_ID, (tk.real_qty - bal.bal_qty) as oilloss
								from (
									select a.COMP_CODE, a.BRN_CODE, a.DOC_DATE, a.PD_ID, sum(b.REAL_QTY) as REAL_QTY
									from (
											select COMP_CODE, BRN_CODE, max(PERIOD_NO) as PERIOD_NO , DOC_DATE,PD_ID
											from DOP_PERIOD_TANK 
											where DOC_DATE between  @datefrom and @dateto
											and COMP_CODE = @compcode
											and BRN_CODE = @brncode                                
											group by COMP_CODE, BRN_CODE, DOC_DATE, PD_ID
										) a
										inner join  DOP_PERIOD_TANK b
										on a.DOC_DATE = b.DOC_DATE
										and a.COMP_CODE = b.COMP_CODE
										and a.BRN_CODE = b.BRN_CODE
										and a.PD_ID = b.PD_ID
										and a.PERIOD_NO = b.PERIOD_NO
									group by a.COMP_CODE, a.BRN_CODE, a.DOC_DATE, a.PD_ID
								) tk
								inner join (
									select tbl.COMP_CODE, tbl.BRN_CODE, tbl.DOC_DATE, tbl.PD_ID, sum(tbl.QTY) as bal_qty
									from (
										select a.COMP_CODE , a.BRN_CODE , a.DOC_DATE, a.PD_ID, b.BEFORE_QTY as QTY
										from (
											select min(PERIOD_NO) PERIOD_NO, COMP_CODE, BRN_CODE,DOC_DATE, PD_ID
											from DOP_PERIOD_TANK
											where DOC_DATE between  @datefrom and @dateto			                        
											and COMP_CODE = @compcode
											and BRN_CODE = @brncode
											group by COMP_CODE, BRN_CODE, DOC_DATE, PD_ID
										) a
										inner join DOP_PERIOD_TANK b
										on a.DOC_DATE = b.DOC_DATE
										and a.BRN_CODE = b.BRN_CODE
										and a.PD_ID = b.PD_ID
										and a.PERIOD_NO = b.PERIOD_NO
										and a.COMP_CODE = b.COMP_CODE
											union all
										select a.COMP_CODE , a.BRN_CODE as brn, a.DOC_DATE as ddate, b.PD_ID, sum(b.ITEM_QTY) as QTY
										from INV_RECEIVE_PROD_HD a
										inner join INV_RECEIVE_PROD_DT b
										on a.COMP_CODE = b.COMP_CODE
										and a.BRN_CODE = b.BRN_CODE
										and a.LOC_CODE = b.LOC_CODE
										and a.DOC_NO = b.DOC_NO		
										inner join MAS_PRODUCT p
										on b.PD_ID = p.PD_ID
										where a.DOC_DATE between  @datefrom and @dateto
											and a.COMP_CODE = @compcode
											and a.BRN_CODE = @brncode
											and a.DOC_STATUS <> 'Cancel'			                        			                        
											and p.GROUP_ID = '0000'        
										group by  a.COMP_CODE , a.BRN_CODE, a.DOC_DATE , b.PD_ID 
											union all
										select COMP_CODE , BRN_CODE ,DOC_DATE, PD_ID , -sum(ISSUE_QTY) as QTY
										from DOP_PERIOD_TANK
										where DOC_DATE between  @datefrom and @dateto
										and COMP_CODE = @compcode
										and BRN_CODE = @brncode		                        
										group by COMP_CODE, BRN_CODE,DOC_DATE, PD_ID
											union all
										select h.COMP_CODE,h.BRN_CODE,h.DOC_DATE, d.PD_ID,-sum(d.item_qty) as QTY
										from INV_RETURN_OIL_HD h
										inner join INV_RETURN_OIL_DT d
										on h.COMP_CODE = d.COMP_CODE
										and h.BRN_CODE = d.BRN_CODE
										and h.LOC_CODE = d.LOC_CODE
										and h.DOC_NO = d.DOC_NO
										where h.comp_code =  @compcode
										and h.BRN_CODE = @brncode			
										and h.DOC_DATE between  @datefrom and @dateto
										and h.DOC_STATUS <> 'Cancel'		                        
										group by h.COMP_CODE,h.BRN_CODE,h.DOC_DATE, d.PD_ID
									)tbl
									group by tbl.COMP_CODE, tbl.BRN_CODE, tbl.DOC_DATE, tbl.PD_ID
								) bal
								on tk.DOC_DATE = bal.DOC_DATE
								and tk.BRN_CODE = bal.BRN_CODE
								and tk.PD_ID = bal.PD_ID
								and tk.COMP_CODE = bal.COMP_CODE		
								) table1
								PIVOT (
									sum(oilloss)   FOR [PD_ID] IN ([000001],[000002],[000004],[000005],[000006],[000010],[000011],[000073],[000074]) 
								) pv1
							)a
							inner join
							(
								SELECT COMP_CODE as CompCode,BRN_CODE as BrnCode,DOC_DATE as DocDate
								,isnull([000001],0) as d01,isnull([000002],0)  as d02,isnull([000004],0)  as d03,isnull([000005],0)  as d04,isnull([000006],0)  as d05,isnull([000010],0)  as d06,isnull([000011],0)  as d07,isnull([000073],0)  as d08,isnull([000074],0)  as d09
								FROM (
									select tk.COMP_CODE, tk.BRN_CODE, tk.DOC_DATE, tk.PD_ID,cash.cash_qty  as diff_amt
									from (
										select a.COMP_CODE, a.BRN_CODE, a.DOC_DATE, a.PD_ID, sum(b.REAL_QTY) as REAL_QTY
										from (
												select COMP_CODE, BRN_CODE, max(PERIOD_NO) as PERIOD_NO , DOC_DATE,PD_ID
												from DOP_PERIOD_TANK 
												where DOC_DATE between  @datefrom and @dateto
												and COMP_CODE = @compcode
												and BRN_CODE = @brncode                                
												group by COMP_CODE, BRN_CODE, DOC_DATE, PD_ID
											) a
											inner join  DOP_PERIOD_TANK b
											on a.DOC_DATE = b.DOC_DATE
											and a.COMP_CODE = b.COMP_CODE
											and a.BRN_CODE = b.BRN_CODE
											and a.PD_ID = b.PD_ID
											and a.PERIOD_NO = b.PERIOD_NO
										group by a.COMP_CODE, a.BRN_CODE, a.DOC_DATE, a.PD_ID
									) tk
									inner join (
										select tbl.COMP_CODE, tbl.BRN_CODE, tbl.DOC_DATE, tbl.PD_ID, sum(tbl.QTY) as bal_qty
										from (
											select a.COMP_CODE , a.BRN_CODE , a.DOC_DATE, a.PD_ID, b.BEFORE_QTY as QTY
											from (
												select min(PERIOD_NO) PERIOD_NO, COMP_CODE, BRN_CODE,DOC_DATE, PD_ID
												from DOP_PERIOD_TANK
												where DOC_DATE between  @datefrom and @dateto			                        
												and COMP_CODE = @compcode
												and BRN_CODE = @brncode
												group by COMP_CODE, BRN_CODE, DOC_DATE, PD_ID
											) a
											inner join DOP_PERIOD_TANK b
											on a.DOC_DATE = b.DOC_DATE
											and a.BRN_CODE = b.BRN_CODE
											and a.PD_ID = b.PD_ID
											and a.PERIOD_NO = b.PERIOD_NO
											and a.COMP_CODE = b.COMP_CODE
												union all
											select a.COMP_CODE , a.BRN_CODE as brn, a.DOC_DATE as ddate, b.PD_ID, sum(b.ITEM_QTY) as QTY
											from INV_RECEIVE_PROD_HD a
											inner join INV_RECEIVE_PROD_DT b
											on a.COMP_CODE = b.COMP_CODE
											and a.BRN_CODE = b.BRN_CODE
											and a.LOC_CODE = b.LOC_CODE
											and a.DOC_NO = b.DOC_NO		
											inner join MAS_PRODUCT p
											on b.PD_ID = p.PD_ID
											where a.DOC_DATE between  @datefrom and @dateto
												and a.COMP_CODE = @compcode
												and a.BRN_CODE = @brncode
												and a.DOC_STATUS <> 'Cancel'			                        			                        
												and p.GROUP_ID = '0000'        
											group by  a.COMP_CODE , a.BRN_CODE, a.DOC_DATE , b.PD_ID 
												union all
											select COMP_CODE , BRN_CODE ,DOC_DATE, PD_ID , -sum(ISSUE_QTY) as QTY
											from DOP_PERIOD_TANK
											where DOC_DATE between  @datefrom and @dateto
											and COMP_CODE = @compcode
											and BRN_CODE = @brncode		                        
											group by COMP_CODE, BRN_CODE,DOC_DATE, PD_ID
												union all
											select h.COMP_CODE,h.BRN_CODE,h.DOC_DATE, d.PD_ID,-sum(d.item_qty) as QTY
											from INV_RETURN_OIL_HD h
											inner join INV_RETURN_OIL_DT d
											on h.COMP_CODE = d.COMP_CODE
											and h.BRN_CODE = d.BRN_CODE
											and h.LOC_CODE = d.LOC_CODE
											and h.DOC_NO = d.DOC_NO
											where h.comp_code =  @compcode
											and h.BRN_CODE = @brncode			
											and h.DOC_DATE between  @datefrom and @dateto
											and h.DOC_STATUS <> 'Cancel'		                        
											group by h.COMP_CODE,h.BRN_CODE,h.DOC_DATE, d.PD_ID
										)tbl
										group by tbl.COMP_CODE, tbl.BRN_CODE, tbl.DOC_DATE, tbl.PD_ID
									) bal
									on tk.DOC_DATE = bal.DOC_DATE
									and tk.BRN_CODE = bal.BRN_CODE
									and tk.PD_ID = bal.PD_ID
									and tk.COMP_CODE = bal.COMP_CODE
									inner join (
										select pc.COMP_CODE, pc.BRN_CODE,pc.DOC_DATE,pc.PD_ID,sum(pc.meter_amt - tt.WITHDRAW_QTY) as cash_qty
										from DOP_PERIOD_CASH pc
										inner join (
											select t.COMP_CODE,t.BRN_CODE,t.DOC_DATE,t.PERIOD_NO,t.PD_ID,t.UNITPRICE,sum(t.WITHDRAW_QTY) as WITHDRAW_QTY
											from DOP_PERIOD_TANK t 
											group by t.COMP_CODE,t.BRN_CODE,t.DOC_DATE,t.PERIOD_NO,t.PD_ID,t.UNITPRICE
										)tt
										on pc.COMP_CODE = tt.COMP_CODE
										and pc.BRN_CODE = tt.BRN_CODE
										and pc.DOC_DATE = tt.DOC_DATE
										and pc.PERIOD_NO = tt.PERIOD_NO
										and pc.PD_ID = tt.PD_ID
										where pc.DOC_DATE between  @datefrom and @dateto
										and pc.BRN_CODE = @brncode 
										and pc.COMP_CODE = @compcode 
										group by  pc.COMP_CODE, pc.BRN_CODE,pc.DOC_DATE,pc.PD_ID

									)cash
									on tk.COMP_CODE = cash.COMP_CODE
									and tk.BRN_CODE = cash.BRN_CODE
									and tk.DOC_DATE = cash.DOC_DATE	
									and tk.PD_ID = cash.PD_ID	
								)table2
								PIVOT (
									sum(diff_amt)   FOR [PD_ID] IN ([000001],[000002],[000004],[000005],[000006],[000010],[000011],[000073],[000074]) 
								) pv2
							)b
							on a.CompCode = b.CompCode
							and a.BrnCode = b.BrnCode
							and a.DocDate = b.DocDate
                        ";


            //Task<List<ST315Response>> result =  Task.Run(() =>  this.GetEntityFromSql<List<ST315Response>>(Context, sql));
            //Task.WhenAll(result);
            //response = result.Result;

            response = this.RawSqlQuery(sql, x => new ST315Response
            {
                compCode = Convert.ToString(x[0]),
                brnCode = Convert.ToString(x[1]),
                docDate = ((DateTime)x[2]).ToString("yyyy-MM-dd"),
                p01 = Convert.ToDecimal(x[3]),
                p02 = Convert.ToDecimal(x[4]),
                p03 = Convert.ToDecimal(x[5]),
                p04 = Convert.ToDecimal(x[6]),
                p05 = Convert.ToDecimal(x[7]),
                p06 = Convert.ToDecimal(x[8]),
                p07 = Convert.ToDecimal(x[9]),
                p08 = Convert.ToDecimal(x[10]),
                p09 = Convert.ToDecimal(x[11]),
                sumOil = Convert.ToDecimal(x[12]),
                d01 = Convert.ToDecimal(x[13]),
                d02 = Convert.ToDecimal(x[14]),
                d03 = Convert.ToDecimal(x[15]),
                d04 = Convert.ToDecimal(x[16]),
                d05 = Convert.ToDecimal(x[17]),
                d06 = Convert.ToDecimal(x[18]),
                d07 = Convert.ToDecimal(x[19]),
                d08 = Convert.ToDecimal(x[20]),
                d09 = Convert.ToDecimal(x[21]),
                sumSale = Convert.ToDecimal(x[22]),
            }).ToList();

            var branch = (from b in this.Context.MasBranches
                          join c in this.Context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                          && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.brnAddress = branch.b.Address;
                x.percent = Math.Round(x.sumOil * 100 / x.sumSale, 2);
            });

            return response;
        }

        public MemoryStream GetST310Excel(StationRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            int rowIndex = 1;
            // title company
            var headerRowLeft = ws.Cells[rowIndex, 1, rowIndex, 8];
            headerRowLeft.Merge = true;
            headerRowLeft.Value = $"{company.CompName}";

            rowIndex++;
            var headerTitle = ws.Cells[rowIndex, 1, rowIndex, 8];
            headerTitle.Merge = true;
            headerTitle.Value = "รายงานตรวจสอบรวม";

            rowIndex++;
            var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 8];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";

            rowIndex++;
            var dateTitle = ws.Cells[rowIndex, 1, rowIndex, 8];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

            // start items					
            var docDates = this.Context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo).Select(x => x.DocDate).Distinct().ToList();

            #region session1
            //column header
            rowIndex += 2;
            ws.Cells[rowIndex, 1].Value = "1.รายงานการตรวจสอบเงินสดจากการขาย";
            int colIndex = 2;
            foreach (var ddate in docDates)
            {
                ws.Cells[rowIndex, colIndex].Value = ddate.ToString("dd/MM/yyyy");
                colIndex++;
            }
            ws.Cells[rowIndex, colIndex].Value = "สะสม";
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var cashsum = this.Context.DopPeriodCashSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo)
                                                        .GroupBy(x => new { x.DocDate })
                                                        .Select(x => new
                                                        {
                                                            DocDate = x.Key.DocDate,
                                                            CashAmt = x.Sum(s => s.CashAmt) ?? 0
                                                        })
                                                        .AsQueryable();

            var cashgl = (from gl in this.Context.DopPeriodGls
                          join cg in this.Context.DopPeriodCashGls on new { gl.CompCode, gl.BrnCode, gl.GlNo } equals new { cg.CompCode, cg.BrnCode, cg.GlNo }
                          where cg.CompCode == req.CompCode
                          && cg.BrnCode == req.BrnCode
                          && cg.DocDate >= req.DateFrom
                          && cg.DocDate <= req.DateTo
                          && gl.GlSlip == "Y"
                          select new { cg }
                          ).GroupBy(x => x.cg.DocDate)
                          .Select(x => new
                          {
                              DocDate = x.Key,
                              GlAmt = x.Sum(s => s.cg.GlAmt)
                          }).AsQueryable();

            // ขายน้ำมันเครื่อง(101)  , รายได้อื่นๆ(125) , ขายมาร์ท(126), รับชำระเงินสด(105)
            var glIn = new List<string>() { "101", "125", "126", "105" };
            // เงินสดย่อย(211)
            var glEx = new List<string>() { "211" };

            var incomes = this.Context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom
                                                        && x.DocDate <= req.DateTo && glIn.Contains(x.GlNo))
                                                        .GroupBy(x => new { x.DocDate, x.GlNo })
                                                        .Select(x => new
                                                        {
                                                            DocDate = x.Key.DocDate,
                                                            GlNo = x.Key.GlNo,
                                                            GlAmt = x.Sum(s => s.GlAmt)
                                                        }).AsQueryable();

            var expense = this.Context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom
                                                        && x.DocDate <= req.DateTo && glEx.Contains(x.GlNo))
                                                        .GroupBy(x => new { x.DocDate, x.GlNo })
                                                        .Select(x => new
                                                        {
                                                            DocDate = x.Key.DocDate,
                                                            GlNo = x.Key.GlNo,
                                                            GlAmt = x.Sum(s => s.GlAmt)
                                                        }).AsQueryable();

            var items = this.Context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo)
                                                    .GroupBy(x => x.DocDate)
                                                    .Select(x => new
                                                    {
                                                        DocDate = x.Key,
                                                        SaleAmt = x.Sum(s => s.SaleAmt) ?? 0,
                                                        CreditAmt = x.Sum(s => s.CreditAmt) ?? 0,
                                                        CashAmt = x.Sum(s => s.CashAmt) ?? 0,
                                                        CouponAmt = x.Sum(s => s.CouponAmt) ?? 0,
                                                        DiscAmt = x.Sum(s => s.DiscAmt) ?? 0,
                                                        TotalAmt = x.Sum(s => s.TotalAmt) ?? 0
                                                    }).ToList();

            rowIndex++;
            ws.Cells[rowIndex, 1].Value = "ขายน้ำมันในรวมเงิน";
            ws.Cells[rowIndex + 1, 1].Value = "หัก  ขายเครดิต";
            ws.Cells[rowIndex + 2, 1].Value = "    ส่วนลด";
            ws.Cells[rowIndex + 3, 1].Value = "    บัตรเครดิต";
            ws.Cells[rowIndex + 4, 1].Value = "    คูปอง";
            ws.Cells[rowIndex + 5, 1].Value = "บวก ขายน้ำมันเครื่อง";
            ws.Cells[rowIndex + 6, 1].Value = "    รายได้อื่นๆ";
            ws.Cells[rowIndex + 7, 1].Value = "รวมขายผ่านสถานีบริการ";
            ws.Cells[rowIndex + 8, 1].Value = "บวก ขายมาร์ท";
            ws.Cells[rowIndex + 9, 1].Value = "รวมเงินสดจากการขายตามบัญชี";

            ws.Cells[rowIndex + 10, 1].Value = "หัก เงินสดย่อย";
            ws.Cells[rowIndex + 11, 1].Value = "บวก รับชำระหนี้เป็นเงินสด";
            ws.Cells[rowIndex + 12, 1].Value = "รวมเงินสดตามบัญชี";
            ws.Cells[rowIndex + 13, 1].Value = "หัก เงินสดในมือรอนำฝาก";
            ws.Cells[rowIndex + 14, 1].Value = "เงินเกิน(ขาด) จากการขายตามบัญชี";

            colIndex = 2;
            foreach (var docdate in docDates)
            {
                ws.Cells[rowIndex, colIndex].Value = items.FirstOrDefault(x => x.DocDate == docdate).SaleAmt;
                ws.Cells[rowIndex + 1, colIndex].Value = items.FirstOrDefault(x => x.DocDate == docdate).CreditAmt;
                ws.Cells[rowIndex + 2, colIndex].Value = items.FirstOrDefault(x => x.DocDate == docdate).DiscAmt;
                ws.Cells[rowIndex + 3, colIndex].Value = cashgl.FirstOrDefault(x => x.DocDate == docdate).GlAmt;
                ws.Cells[rowIndex + 4, colIndex].Value = items.FirstOrDefault(x => x.DocDate == docdate).CouponAmt;
                ws.Cells[rowIndex + 5, colIndex].Value = incomes.FirstOrDefault(x => x.DocDate == docdate && x.GlNo == "101").GlAmt;
                ws.Cells[rowIndex + 6, colIndex].Value = incomes.FirstOrDefault(x => x.DocDate == docdate && x.GlNo == "125").GlAmt;
                var sumSaleAmt = items.Where(x => x.DocDate == docdate).Sum(x => x.SaleAmt - x.CreditAmt - x.DiscAmt - x.CouponAmt)
                                - cashgl.Where(x => x.DocDate == docdate).Sum(x => x.GlAmt)
                                + incomes.Where(x => x.DocDate == docdate && (x.GlNo == "101" || x.GlNo == "125")).Sum(x => x.GlAmt);

                ws.Cells[rowIndex + 7, colIndex].Value = sumSaleAmt;
                ws.Cells[rowIndex + 8, colIndex].Value = incomes.FirstOrDefault(x => x.DocDate == docdate && x.GlNo == "126").GlAmt;
                ws.Cells[rowIndex + 9, colIndex].Value = sumSaleAmt + incomes.FirstOrDefault(x => x.DocDate == docdate && x.GlNo == "126").GlAmt;

                ws.Cells[rowIndex + 10, colIndex].Value = expense.FirstOrDefault(x => x.DocDate == docdate && x.GlNo == "211").GlAmt;
                ws.Cells[rowIndex + 11, colIndex].Value = incomes.FirstOrDefault(x => x.DocDate == docdate && x.GlNo == "105").GlAmt;

                sumSaleAmt += incomes.Where(x => x.DocDate == docdate && (x.GlNo == "126" || x.GlNo == "105")).Sum(x => x.GlAmt) - expense.Where(x => x.DocDate == docdate && (x.GlNo == "211")).Sum(x => x.GlAmt);
                ws.Cells[rowIndex + 12, colIndex].Value = sumSaleAmt;
                ws.Cells[rowIndex + 13, colIndex].Value = cashsum.FirstOrDefault(x => x.DocDate == docdate).CashAmt;

                sumSaleAmt = cashsum.FirstOrDefault(x => x.DocDate == docdate).CashAmt - sumSaleAmt;
                ws.Cells[rowIndex + 14, colIndex].Value = sumSaleAmt;

                colIndex++;
            }

            // column สะสม
            ws.Cells[rowIndex, colIndex].Value = items.Sum(x => x.SaleAmt);
            ws.Cells[rowIndex + 1, colIndex].Value = items.Sum(x => x.CreditAmt);
            ws.Cells[rowIndex + 2, colIndex].Value = items.Sum(x => x.DiscAmt);
            ws.Cells[rowIndex + 3, colIndex].Value = cashgl.Sum(x => x.GlAmt);
            ws.Cells[rowIndex + 4, colIndex].Value = items.Sum(x => x.CouponAmt);
            ws.Cells[rowIndex + 5, colIndex].Value = incomes.Where(x => x.GlNo == "101").Sum(x => x.GlAmt);
            ws.Cells[rowIndex + 6, colIndex].Value = incomes.Where(x => x.GlNo == "125").Sum(x => x.GlAmt);

            var sumTotal = items.Sum(x => x.SaleAmt - x.CreditAmt - x.DiscAmt - x.CouponAmt) - cashgl.Sum(x => x.GlAmt) + incomes.Where(x => x.GlNo == "101" || x.GlNo == "125").Sum(x => x.GlAmt);
            ws.Cells[rowIndex + 7, colIndex].Value = sumTotal;
            ws.Cells[rowIndex + 8, colIndex].Value = incomes.Where(x => x.GlNo == "126").Sum(x => x.GlAmt);
            ws.Cells[rowIndex + 9, colIndex].Value = sumTotal + incomes.Where(x => x.GlNo == "126").Sum(x => x.GlAmt);
            ws.Cells[rowIndex + 10, colIndex].Value = expense.Where(x => x.GlNo == "211").Sum(x => x.GlAmt);
            ws.Cells[rowIndex + 11, colIndex].Value = incomes.Where(x => x.GlNo == "105").Sum(x => x.GlAmt);

            sumTotal += incomes.Where(x => x.GlNo == "126" || x.GlNo == "105").Sum(x => x.GlAmt) - expense.Where(x => x.GlNo == "211").Sum(x => x.GlAmt);
            ws.Cells[rowIndex + 12, colIndex].Value = sumTotal;
            ws.Cells[rowIndex + 13, colIndex].Value = cashsum.Sum(x => x.CashAmt);

            sumTotal = cashsum.Sum(x => x.CashAmt) - sumTotal;
            ws.Cells[rowIndex + 14, colIndex].Value = sumTotal;
            #endregion

            #region session2
            //column header
            rowIndex += 16;
            ws.Cells[rowIndex, 1].Value = "2.รายงานตรวจสอบ Stock น้ำมันคงเหลือ";
            colIndex = 2;
            foreach (var ddate in docDates)
            {
                ws.Cells[rowIndex, colIndex].Value = ddate.ToString("dd/MM/yyyy");
                colIndex++;
            }
            ws.Cells[rowIndex, colIndex].Value = "สะสม";
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            rowIndex++;
            ws.Cells[rowIndex, 1].Value = "ผลต่าง เกิน(ขาด) ดีเซล B7";
            ws.Cells[rowIndex + 1, 1].Value = "ผลต่าง เกิน(ขาด) เบนซิน";
            ws.Cells[rowIndex + 2, 1].Value = "ผลต่าง เกิน(ขาด) แก๊สโซฮอล์ 95";
            ws.Cells[rowIndex + 3, 1].Value = "รวม";
            ws.Cells[rowIndex + 4, 1].Value = "% เกิน(ขาด) ต่อยอดขาย";

            var pdIds = new List<string>() { "000001", "000002", "000005" };
            var tank = this.Context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode
                                                    && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo
                                                    && pdIds.Contains(x.PdId))
                                                    .GroupBy(x => new { x.DocDate, x.PdId })
                                                    .Select(x => new
                                                    {
                                                        DocDate = x.Key.DocDate,
                                                        PdId = x.Key.PdId,
                                                        DiffQty = x.Sum(s => s.DiffQty)
                                                    }).AsQueryable();

            colIndex = 2;
            foreach (var docdate in docDates)
            {
                ws.Cells[rowIndex, colIndex].Value = tank.Any(x => x.DocDate == docdate && x.PdId == "000001")?tank.FirstOrDefault(x => x.DocDate == docdate && x.PdId == "000001").DiffQty:0;
                ws.Cells[rowIndex + 1, colIndex].Value = tank.Any(x => x.DocDate == docdate && x.PdId == "000002")?tank.FirstOrDefault(x => x.DocDate == docdate && x.PdId == "000002").DiffQty:0;
                ws.Cells[rowIndex + 2, colIndex].Value = tank.Any(x => x.DocDate == docdate && x.PdId == "000005")?tank.FirstOrDefault(x => x.DocDate == docdate && x.PdId == "000005").DiffQty:0;
                ws.Cells[rowIndex + 3, colIndex].Value = tank.Where(x => x.DocDate == docdate).Sum(s => s.DiffQty);
                ws.Cells[rowIndex + 4, colIndex].Value = 0;
                colIndex++;
            }
            // column สะสม
            ws.Cells[rowIndex, colIndex].Value = tank.Where(x => x.PdId == "000001").Sum(s => s.DiffQty);
            ws.Cells[rowIndex + 1, colIndex].Value = tank.Where(x => x.PdId == "000002").Sum(s => s.DiffQty);
            ws.Cells[rowIndex + 2, colIndex].Value = tank.Where(x => x.PdId == "000005").Sum(s => s.DiffQty);
            ws.Cells[rowIndex + 3, colIndex].Value = tank.Sum(x => x.DiffQty);
            ws.Cells[rowIndex + 4, colIndex].Value = 0;

            #endregion

            #region session3
            //column header
            rowIndex += 6;
            ws.Cells[rowIndex, 1].Value = "3.เปรียบเทียบเงินเกิน(ขาด)";
            colIndex = 2;
            foreach (var ddate in docDates)
            {
                ws.Cells[rowIndex, colIndex].Value = ddate.ToString("dd/MM/yyyy");
                colIndex++;
            }
            ws.Cells[rowIndex, colIndex].Value = "สะสม";
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            rowIndex++;
            ws.Cells[rowIndex, 1].Value = "เงินขาดที่ยอมรับได้ 0.008 ต่อลิตร";
            ws.Cells[rowIndex + 1, 1].Value = "เปรียบเทียบเงินเกิน(ขาด)จริง";

            #endregion

            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetST315Excel(StationRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            ws.Column(11).Width = 2;

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            // Title Company
            var headerCompany = ws.Cells[1, 8, 1, 13];
            headerCompany.Merge = true;
            headerCompany.Value = $"{company.CompName}";
            headerCompany.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var headerPrintDate = ws.Cells[1, 18, 1, 20];
            headerPrintDate.Merge = true;
            headerPrintDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // Report Title
            var headerTitle = ws.Cells[2, 8, 2, 13];
            headerTitle.Merge = true;
            headerTitle.Value = "รายงานสรุปยอดขายขาดเกินน้ํามันใสที่สถานีบริการ";
            headerTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Branch Name
            ws.Cells[3, 1, 3, 1].Merge = true;
            ws.Cells[3, 1, 3, 1].Value = "สาขา :";

            ws.Cells[3, 2, 3, 7].Merge = true;
            ws.Cells[3, 2, 3, 7].Value = brn.BrnName;

            // Date Range
            var headerDateRange = ws.Cells[3, 8, 3, 13];
            headerDateRange.Merge = true;
            headerDateRange.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            headerDateRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchHeader = ws.Cells[4, 1, 5, 1];
            branchHeader.Merge = true;
            branchHeader.Value = "สาขา";
            branchHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            branchHeader.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            branchHeader.Style.Border.Top.Style = ExcelBorderStyle.Medium;


            var leftHeader = ws.Cells[4, 2, 4, 10];
            leftHeader.Merge = true;
            leftHeader.Value = "ยอดขาดเกิน";
            leftHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            leftHeader.Style.Border.Top.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 11, 4, 11].Style.Border.Top.Style = ExcelBorderStyle.Medium;

            var rightHeader = ws.Cells[4, 12, 4, 20];
            rightHeader.Merge = true;
            rightHeader.Value = "% ขาดเกินเทียบกับยอดขาย";
            rightHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rightHeader.Style.Border.Top.Style = ExcelBorderStyle.Medium;

            string[] productKey = { "ดีเซล B7", "ดีเซล B20", "ดีเซล", "เบนซิน", "แก๊สโซฮอล์ 95", "แก๊สโซฮอล์ 91", "แก๊สโซฮอล์ E20", "แก๊ส LPG", "รวม" };
            var columnIndex = 2;
            foreach (var item in productKey)
            {
                var keyIndex = ws.Cells[5, columnIndex, 5, columnIndex];
                keyIndex.Value = item;
                keyIndex.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                keyIndex.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                columnIndex++;
            }

            columnIndex++;
            foreach (var item in productKey)
            {
                var keyIndex = ws.Cells[5, columnIndex, 5, columnIndex];
                keyIndex.Value = item;
                keyIndex.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                keyIndex.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                columnIndex++;

            }
            decimal sumP01 = 0;
            decimal sumP08 = 0;
            decimal sumP09 = 0;
            decimal sumP02 = 0;
            decimal sumP04 = 0;
            decimal sumP05 = 0;
            decimal sumP06 = 0;
            decimal sumP07 = 0;
            decimal sumTotalP = 0;

            decimal sumD01 = 0;
            decimal sumD08 = 0;
            decimal sumD09 = 0;
            decimal sumD02 = 0;
            decimal sumD04 = 0;
            decimal sumD05 = 0;
            decimal sumD06 = 0;
            decimal sumD07 = 0;
            decimal sumTotalD = 0;

            var data = this.GetST315PDF(req);

            var rowIndex = 6;
            foreach (var item in data)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.docDate;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.p01;
                sumP01 += item.p01;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.p08;
                sumP08 += item.p08;
                ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = item.p09;
                sumP09 += item.p09;
                ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.p02;
                sumP02 += item.p02;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.p04;
                sumP04 += item.p04;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.p05;
                sumP05 += item.p05;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.p06;
                sumP06 += item.p06;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.p07;
                sumP07 += item.p07;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.sumOil;
                sumTotalP += item.sumOil;

                ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 11, rowIndex, 11].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 12, rowIndex, 12].Value = item.d01;
                sumD01 += item.d01;
                ws.Cells[rowIndex, 12, rowIndex, 12].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 13, rowIndex, 13].Value = item.d08;
                sumD08 += item.d08;
                ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 14, rowIndex, 14].Value = item.d09;
                sumD09 += item.d09;
                ws.Cells[rowIndex, 14, rowIndex, 14].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 15, rowIndex, 15].Value = item.d02;
                sumD02 += item.d02;
                ws.Cells[rowIndex, 15, rowIndex, 15].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 16, rowIndex, 16].Value = item.d04;
                sumD04 += item.d04;
                ws.Cells[rowIndex, 16, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 17, rowIndex, 17].Value = item.d05;
                sumD05 += item.d05;
                ws.Cells[rowIndex, 17, rowIndex, 17].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 18, rowIndex, 18].Value = item.d06;
                sumD06 += item.d06;
                ws.Cells[rowIndex, 18, rowIndex, 18].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 19, rowIndex, 19].Value = item.d07;
                sumD07 += item.d07;
                ws.Cells[rowIndex, 19, rowIndex, 19].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 20, rowIndex, 20].Value = item.percent;
                sumTotalD += item.percent;
                ws.Cells[rowIndex, 20, rowIndex, 20].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                rowIndex++;
            }


            ws.Cells[rowIndex, 1, rowIndex, 1].Value = "รวม";
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = sumP01;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 3, rowIndex, 3].Value = sumP08;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 4, rowIndex, 4].Value = sumP09;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = sumP02;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = sumP04;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = sumP05;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = sumP06;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = sumP07;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = sumTotalP;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 11, rowIndex, 11].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 11, rowIndex, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 12, rowIndex, 12].Value = sumD01;
            ws.Cells[rowIndex, 12, rowIndex, 12].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 12, rowIndex, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 13, rowIndex, 13].Value = sumD08;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 14, rowIndex, 14].Value = sumD09;
            ws.Cells[rowIndex, 14, rowIndex, 14].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 14, rowIndex, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 15, rowIndex, 15].Value = sumD02;
            ws.Cells[rowIndex, 15, rowIndex, 15].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 15, rowIndex, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 16, rowIndex, 16].Value = sumD04;
            ws.Cells[rowIndex, 16, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 16, rowIndex, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 17, rowIndex, 17].Value = sumD05;
            ws.Cells[rowIndex, 17, rowIndex, 17].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 17, rowIndex, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 18, rowIndex, 18].Value = sumD06;
            ws.Cells[rowIndex, 18, rowIndex, 18].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 18, rowIndex, 18].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 19, rowIndex, 19].Value = sumD07;
            ws.Cells[rowIndex, 19, rowIndex, 19].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 19, rowIndex, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 20, rowIndex, 20].Value = sumTotalD;
            ws.Cells[rowIndex, 20, rowIndex, 20].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 20, rowIndex, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetST316Excel(StationRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            ws.Column(11).Width = 2;

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            // Title Company
            var headerCompany = ws.Cells[1, 8, 1, 13];
            headerCompany.Merge = true;
            headerCompany.Value = $"{company.CompName}";
            headerCompany.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var headerPrintDate = ws.Cells[1, 18, 1, 20];
            headerPrintDate.Merge = true;
            headerPrintDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // Report Title
            var headerTitle = ws.Cells[2, 8, 2, 13];
            headerTitle.Merge = true;
            headerTitle.Value = "รายงานสรุปยอดขายขาดเกินน้ํามันใสที่สถานีบริการ";
            headerTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Branch Name
            ws.Cells[3, 1, 3, 1].Merge = true;
            ws.Cells[3, 1, 3, 1].Value = "สาขา :";

            ws.Cells[3, 2, 3, 7].Merge = true;
            ws.Cells[3, 2, 3, 7].Value = brn.BrnName;

            // Date Range
            var headerDateRange = ws.Cells[3, 8, 3, 13];
            headerDateRange.Merge = true;
            headerDateRange.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            headerDateRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchHeader = ws.Cells[4, 1, 5, 1];
            branchHeader.Merge = true;
            branchHeader.Value = "สาขา";
            branchHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            branchHeader.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            branchHeader.Style.Border.Top.Style = ExcelBorderStyle.Medium;


            var leftHeader = ws.Cells[4, 2, 4, 10];
            leftHeader.Merge = true;
            leftHeader.Value = "ยอดขาดเกิน";
            leftHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            leftHeader.Style.Border.Top.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 11, 4, 11].Style.Border.Top.Style = ExcelBorderStyle.Medium;

            var rightHeader = ws.Cells[4, 12, 4, 20];
            rightHeader.Merge = true;
            rightHeader.Value = "% ขาดเกินเทียบกับยอดขาย";
            rightHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rightHeader.Style.Border.Top.Style = ExcelBorderStyle.Medium;

            string[] productKey = { "ดีเซล B7", "ดีเซล B20", "ดีเซล", "เบนซิน", "แก๊สโซฮอล์ 95", "แก๊สโซฮอล์ 91", "แก๊สโซฮอล์ E20", "แก๊ส LPG", "รวม" };
            var columnIndex = 2;
            foreach (var item in productKey)
            {
                var keyIndex = ws.Cells[5, columnIndex, 5, columnIndex];
                keyIndex.Value = item;
                keyIndex.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                keyIndex.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                columnIndex++;
            }

            columnIndex++;
            foreach (var item in productKey)
            {
                var keyIndex = ws.Cells[5, columnIndex, 5, columnIndex];
                keyIndex.Value = item;
                keyIndex.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                keyIndex.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                columnIndex++;

            }
            decimal sumP01 = 0;
            decimal sumP08 = 0;
            decimal sumP09 = 0;
            decimal sumP02 = 0;
            decimal sumP04 = 0;
            decimal sumP05 = 0;
            decimal sumP06 = 0;
            decimal sumP07 = 0;
            decimal sumTotalP = 0;

            decimal sumD01 = 0;
            decimal sumD08 = 0;
            decimal sumD09 = 0;
            decimal sumD02 = 0;
            decimal sumD04 = 0;
            decimal sumD05 = 0;
            decimal sumD06 = 0;
            decimal sumD07 = 0;
            decimal sumTotalD = 0;

            var dataRaw = this.GetST315PDF(req);
            var data = dataRaw.GroupBy(x => new { x.compCode, x.compName, x.branchNo, x.brnCode, x.brnName, x.brnAddress }).Select(x => new ST316Response
            {
                compCode = x.Key.compCode,
                compName = x.Key.compName,
                brnCode = x.Key.brnCode,
                brnName = x.Key.brnName,
                branchNo = x.Key.branchNo,
                brnAddress = x.Key.brnAddress,
                p01 = x.Sum(s => s.p01),
                p02 = x.Sum(s => s.p02),
                p03 = x.Sum(s => s.p03),
                p04 = x.Sum(s => s.p04),
                p05 = x.Sum(s => s.p05),
                p06 = x.Sum(s => s.p06),
                p07 = x.Sum(s => s.p07),
                p08 = x.Sum(s => s.p08),
                p09 = x.Sum(s => s.p09),
                sumOil = x.Sum(s => s.sumOil),
                d01 = x.Sum(s => s.d01),
                d02 = x.Sum(s => s.d02),
                d03 = x.Sum(s => s.d03),
                d04 = x.Sum(s => s.d04),
                d05 = x.Sum(s => s.d05),
                d06 = x.Sum(s => s.d06),
                d07 = x.Sum(s => s.d07),
                d08 = x.Sum(s => s.d08),
                d09 = x.Sum(s => s.d09),
                sumSale = x.Sum(s => s.sumSale),
                percent = x.Sum(s => s.percent),
            }).ToList();

            var rowIndex = 6;
            foreach (var item in data)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.brnName;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.p01;
                sumP01 += item.p01;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.p08;
                sumP08 += item.p08;
                ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = item.p09;
                sumP09 += item.p09;
                ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.p02;
                sumP02 += item.p02;
                ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.p04;
                sumP04 += item.p04;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.p05;
                sumP05 += item.p05;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.p06;
                sumP06 += item.p06;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.p07;
                sumP07 += item.p07;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.sumOil;
                sumTotalP += item.sumOil;

                ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 11, rowIndex, 11].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 12, rowIndex, 12].Value = item.d01;
                sumD01 += item.d01;
                ws.Cells[rowIndex, 12, rowIndex, 12].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 13, rowIndex, 13].Value = item.d08;
                sumD08 += item.d08;
                ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 14, rowIndex, 14].Value = item.d09;
                sumD09 += item.d09;
                ws.Cells[rowIndex, 14, rowIndex, 14].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 15, rowIndex, 15].Value = item.d02;
                sumD02 += item.d02;
                ws.Cells[rowIndex, 15, rowIndex, 15].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 16, rowIndex, 16].Value = item.d04;
                sumD04 += item.d04;
                ws.Cells[rowIndex, 16, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 17, rowIndex, 17].Value = item.d05;
                sumD05 += item.d05;
                ws.Cells[rowIndex, 17, rowIndex, 17].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 18, rowIndex, 18].Value = item.d06;
                sumD06 += item.d06;
                ws.Cells[rowIndex, 18, rowIndex, 18].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 19, rowIndex, 19].Value = item.d07;
                sumD07 += item.d07;
                ws.Cells[rowIndex, 19, rowIndex, 19].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 20, rowIndex, 20].Value = item.percent;
                sumTotalD += item.percent;
                ws.Cells[rowIndex, 20, rowIndex, 20].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                rowIndex++;
            }


            ws.Cells[rowIndex, 1, rowIndex, 1].Value = "รวม";
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = sumP01;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 3, rowIndex, 3].Value = sumP08;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 4, rowIndex, 4].Value = sumP09;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = sumP02;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = sumP04;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = sumP05;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = sumP06;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = sumP07;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = sumTotalP;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 11, rowIndex, 11].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 11, rowIndex, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 12, rowIndex, 12].Value = sumD01;
            ws.Cells[rowIndex, 12, rowIndex, 12].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 12, rowIndex, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 13, rowIndex, 13].Value = sumD08;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 14, rowIndex, 14].Value = sumD09;
            ws.Cells[rowIndex, 14, rowIndex, 14].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 14, rowIndex, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 15, rowIndex, 15].Value = sumD02;
            ws.Cells[rowIndex, 15, rowIndex, 15].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 15, rowIndex, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 16, rowIndex, 16].Value = sumD04;
            ws.Cells[rowIndex, 16, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 16, rowIndex, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 17, rowIndex, 17].Value = sumD05;
            ws.Cells[rowIndex, 17, rowIndex, 17].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 17, rowIndex, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 18, rowIndex, 18].Value = sumD06;
            ws.Cells[rowIndex, 18, rowIndex, 18].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 18, rowIndex, 18].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 19, rowIndex, 19].Value = sumD07;
            ws.Cells[rowIndex, 19, rowIndex, 19].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 19, rowIndex, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 20, rowIndex, 20].Value = sumTotalD;
            ws.Cells[rowIndex, 20, rowIndex, 20].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 20, rowIndex, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetST317Excel(StationRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);


            // title company
            var headerRowLeft = ws.Cells[1, 1, 1, 8];
            headerRowLeft.Merge = true;
            headerRowLeft.Value = $"{company.CompName}";

            var headerRowRight = ws.Cells[2, 1, 2, 8];
            headerRowRight.Merge = true;
            headerRowRight.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

            ws.Cells[3, 1, 3, 8].Merge = true;
            ws.Cells[3, 1, 3, 3].Value = "สรุปปริมาณน้ำมันสถานีบริการประจำวัน";


            // start items
            var rowIndex = 4;
            //column header
            ws.Cells[rowIndex, 1, rowIndex, 1].Value = "สาขา";
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = "ชื่อสาขา";
            ws.Cells[rowIndex, 3, rowIndex, 3].Value = "วันที่";
            ws.Cells[rowIndex, 4, rowIndex, 4].Value = "กะการทำงาน";
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = "ผลิตภัณฑ์";
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = "ชื่อผลิตภัณฑ์";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = "ราคาน้ำมัน";
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = "ยอดขาย (บาท)";
            ws.Cells[rowIndex, 1, rowIndex, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, 8].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));
            rowIndex++;


            var items = (from br in this.Context.MasBranches
                         join pc in this.Context.DopPeriodCashes on new { br.CompCode, br.BrnCode } equals new { pc.CompCode, pc.BrnCode }
                         where pc.CompCode == req.CompCode
                         && pc.BrnCode == req.BrnCode
                         && pc.DocDate >= req.DateFrom
                         && pc.DocDate <= req.DateTo
                         select new { br, pc }
                      ).Select(x => new ST317Response
                      {
                          brnCode = x.br.BrnCode,
                          brnName = x.br.BrnName,
                          docDate = x.pc.DocDate.ToString("dd/MM/yyyy"),
                          periodNo = x.pc.PeriodNo,
                          pdId = x.pc.PdId,
                          pdName = x.pc.PdName,
                          unitPrice = x.pc.UnitPrice ?? 0,
                          saleAmt = x.pc.SaleAmt ?? 0
                      }).ToList();
            
            foreach (var item in items)
            {
                ws.Cells[rowIndex, 1].Value = item.brnCode;
                ws.Cells[rowIndex, 2].Value = item.brnName;
                ws.Cells[rowIndex, 3].Value = item.docDate;
                ws.Cells[rowIndex, 4].Value = item.periodNo;
                ws.Cells[rowIndex, 5].Value = item.pdId;
                ws.Cells[rowIndex, 6].Value = item.pdName;
                ws.Cells[rowIndex, 7].Value = item.unitPrice;
                ws.Cells[rowIndex, 8].Value = item.saleAmt;                
                rowIndex++;
            }
            ws.Cells[rowIndex, 7].Value = "รวมสุทธิ";
            ws.Cells[rowIndex, 8].Value = items.Sum(x=>x.saleAmt);

            return new MemoryStream(pck.GetAsByteArray());


        }
        public List<ST317Response> GetST317PDF(StationRequest req)
        {
            List<ST317Response> response = new List<ST317Response>();

            response = this.Context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo)
                                                    .Select(x => new ST317Response
                                                    {
                                                        docDate = x.DocDate.ToString("yyyy-MM-dd"),
                                                        periodNo = x.PeriodNo,
                                                        pdId = x.PdId,
                                                        pdName = x.PdName,
                                                        unitPrice = x.UnitPrice ?? 0,
                                                        saleAmt = x.SaleAmt ?? 0
                                                    }).ToList();


            var branch = (from b in this.Context.MasBranches
                          join c in this.Context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                          && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.branchNo = branch.b.BranchNo;
                x.brnAddress = branch.b.Address;
            });

            return response;
        }

        public List<ST316Response> GetST316PDF(StationRequest req)
        {
            List<ST316Response> response = new List<ST316Response>();
            var data = this.GetST315PDF(req);
            response = data.GroupBy(x => new { x.compCode, x.compName, x.branchNo, x.brnCode, x.brnName, x.brnAddress }).Select(x => new ST316Response
            {
                compCode = x.Key.compCode,
                compName = x.Key.compName,
                brnCode = x.Key.brnCode,
                brnName = x.Key.brnName,
                branchNo = x.Key.branchNo,
                brnAddress = x.Key.brnAddress,
                p01 = x.Sum(s => s.p01),
                p02 = x.Sum(s => s.p02),
                p03 = x.Sum(s => s.p03),
                p04 = x.Sum(s => s.p04),
                p05 = x.Sum(s => s.p05),
                p06 = x.Sum(s => s.p06),
                p07 = x.Sum(s => s.p07),
                p08 = x.Sum(s => s.p08),
                p09 = x.Sum(s => s.p09),
                sumOil = x.Sum(s => s.sumOil),
                d01 = x.Sum(s => s.d01),
                d02 = x.Sum(s => s.d02),
                d03 = x.Sum(s => s.d03),
                d04 = x.Sum(s => s.d04),
                d05 = x.Sum(s => s.d05),
                d06 = x.Sum(s => s.d06),
                d07 = x.Sum(s => s.d07),
                d08 = x.Sum(s => s.d08),
                d09 = x.Sum(s => s.d09),
                sumSale = x.Sum(s => s.sumSale),
                percent = x.Sum(s => s.percent),
            }).ToList();
            return response;
        }
    }
}
