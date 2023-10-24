using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Report.API.Domain.Enums;
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

namespace Report.API.Repositories
{
    public class SalesRepository : SqlDataAccessHelper, ISalesRepository
    {
        private readonly IMapper _mapper;

        public SalesRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<MemoryStream> GetSale02ExcelAsync(SalesRequest req)
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);
            if (brn == null || company == null) return new MemoryStream(await pck.GetAsByteArrayAsync());

            var companyTitle = ws.Cells[1, 3, 1, 7];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var printDate = ws.Cells[1, 9, 1, 10];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // title date
            var dateTitle = ws.Cells[3, 3, 3, 7];
            dateTitle.Merge = true;
            dateTitle.Value = $"ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchTitle = ws.Cells[4, 1, 4, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";

            var data = await this.GetSale02PDF(req);

            switch (req.ReportType)
            {
                case ReportType.Summary:
                    GenerateExcelSale02ReportSummary(data, ws);
                    break;
                case ReportType.Detail:
                    GenerateExcelSale02ReportDetail(data, ws);
                    break;
            }

            return new MemoryStream(await pck.GetAsByteArrayAsync());
        }

        private void GenerateExcelSale02ReportSummary(List<Sale02Response> data, ExcelWorksheet ws)
        {
            // title report
            var reportTitle = ws.Cells[2, 3, 2, 7];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานสมุดรายวัน (สรุปยอดรวม)";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            ws.Cells[5, 1, 5, 1].Value = "รหัสบัญชี";
            ws.Cells[5, 2, 5, 6].Value = "รายการ";
            ws.Cells[5, 7, 5, 7].Value = "ปริมาณ";
            ws.Cells[5, 8, 5, 8].Value = "มูลค่าสินค้า";
            ws.Cells[5, 9, 5, 9].Value = "ภาษีมูลค่าเพิ่ม";
            ws.Cells[5, 10, 5, 10].Value = "รวม";

            ws.Cells[5, 2, 5, 6].Merge = true;
            ws.Cells[5, 1, 5, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var rowIndex = 6;
            foreach (var item in data)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.acctCode;
                ws.Cells[rowIndex, 2, rowIndex, 6].Value = item.acctName;
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.itemQty;
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.saleAmt;
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.vatAmt;
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.subAmt;

                ws.Cells[rowIndex, 2, rowIndex, 6].Merge = true;
                ws.Cells[rowIndex, 7, rowIndex, 10].Style.Numberformat.Format = "#,##0.00";

                ws.Cells[rowIndex, 1, rowIndex, 10].AutoFitColumns();
                rowIndex++;
            }


            ws.Cells[rowIndex, 6, rowIndex, 6].Value = "รวมทั้งสิ้น";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Sum(s => s.itemQty);
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = data.Sum(s => s.saleAmt);
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = data.Sum(s => s.vatAmt);
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = data.Sum(s => s.subAmt);
            ws.Cells[rowIndex, 7, rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[rowIndex, 1, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            ws.Cells[rowIndex, 1, rowIndex, 10].AutoFitColumns();
        }

        private void GenerateExcelSale02ReportDetail(List<Sale02Response> data, ExcelWorksheet ws)
        {
            // title report
            var reportTitle = ws.Cells[2, 3, 2, 7];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานสมุดรายวัน (สรุปรายวัน)";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[5, 1, 5, 1].Value = "วันที่";
            ws.Cells[5, 2, 5, 2].Value = "รหัสบัญชี";
            ws.Cells[5, 3, 5, 6].Value = "รายการ";
            ws.Cells[5, 7, 5, 7].Value = "ปริมาณ";
            ws.Cells[5, 8, 5, 8].Value = "มูลค่าสินค้า";
            ws.Cells[5, 9, 5, 9].Value = "ภาษีมูลค่าเพิ่ม";
            ws.Cells[5, 10, 5, 10].Value = "รวม";

            ws.Cells[5, 3, 5, 6].Merge = true;
            ws.Cells[5, 1, 5, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var groupData = data.GroupBy(g => new { g.docDate, g.compCode, g.compName, g.brnCode, g.brnName })
                .Select(s => new
                {
                    DocDate = s.Key.docDate,
                    Items = s.Select(i => new
                    {
                        AccCode = i.acctCode,
                        AccName = i.acctName,
                        ItemQty = i.itemQty,
                        SaleAmt = i.saleAmt,
                        VatAmt = i.vatAmt,
                        SummaryAmt = i.subAmt
                    })
                });

            var rowIndex = 6;
            foreach (var date in groupData)
            {
                var currentDate = Convert.ToDateTime(DateTime.ParseExact(date.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"{currentDate}";
                foreach (var item in date.Items)
                {
                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.AccCode;
                    ws.Cells[rowIndex, 3, rowIndex, 6].Value = item.AccName;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.ItemQty;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.SaleAmt;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.VatAmt;
                    ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.SummaryAmt;

                    ws.Cells[rowIndex, 3, rowIndex, 6].Merge = true;
                    ws.Cells[rowIndex, 7, rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells[rowIndex, 1, rowIndex, 10].AutoFitColumns();
                    rowIndex++;
                }

                ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"รวมวันที่ {currentDate}";
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = date.Items.Sum(s => s.ItemQty);
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = date.Items.Sum(s => s.SaleAmt);
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = date.Items.Sum(s => s.VatAmt);
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = date.Items.Sum(s => s.SummaryAmt);

                ws.Cells[rowIndex, 1, rowIndex, 10].AutoFitColumns();
                ws.Cells[rowIndex, 7, rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[rowIndex, 1, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[rowIndex, 6, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                rowIndex = rowIndex + 2;
            }


            ws.Cells[rowIndex, 6, rowIndex, 6].Value = "รวมทั้งสิ้น";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = groupData.Sum(s => s.Items.Sum(i => i.ItemQty));
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = groupData.Sum(s => s.Items.Sum(i => i.SaleAmt));
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = groupData.Sum(s => s.Items.Sum(i => i.VatAmt));
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = groupData.Sum(s => s.Items.Sum(i => i.SummaryAmt));
            ws.Cells[rowIndex, 7, rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[rowIndex, 1, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 6, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            ws.Cells[rowIndex, 1, rowIndex, 10].AutoFitColumns();
        }

        public async Task<List<Sale02Response>> GetSale02PDF(SalesRequest req)
        {
            List<Sale02Response> response = new List<Sale02Response>();


            List<Sale02Response> cashsales = await (from hd in this.context.SalCashsaleHds
                                                    join dt in this.context.SalCashsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                                                    join mp in this.context.MasProducts on new { dt.PdId } equals new { mp.PdId }
                                                    from ac in this.context.MasGlAccounts.Where(x => x.CompCode == hd.CompCode && x.AcctCode == mp.AcctCode).DefaultIfEmpty()
                                                    where hd.CompCode == req.CompCode
                                                          && hd.BrnCode == req.BrnCode
                                                          && hd.DocDate >= req.DateFrom
                                                          && hd.DocDate <= req.DateTo
                                                          && hd.DocStatus != "Cancel"
                                                          && mp.GroupId != "0000"
                                                    select new { hd, dt, mp, ac }
                ).GroupBy(x => new { x.hd.DocDate, x.mp.AcctCode, x.ac.AcctName }).Select(x => new Sale02Response
                {
                    docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    acctCode = x.Key.AcctCode,
                    acctName = x.Key.AcctName ?? "",
                    itemQty = x.Sum(s => s.dt.ItemQty) ?? 0,
                    subAmt = x.Sum(s => s.dt.SubAmt) ?? 0,
                }).ToListAsync();

            List<Sale02Response> periodcashs = await (from hd in this.context.DopPeriodCashes
                                                      join mp in this.context.MasProducts on new { hd.PdId } equals new { mp.PdId }
                                                      from ac in this.context.MasGlAccounts.Where(x => x.CompCode == hd.CompCode && x.AcctCode == mp.AcctCode).DefaultIfEmpty()
                                                      where hd.CompCode == req.CompCode
                                                            && hd.BrnCode == req.BrnCode
                                                            && hd.DocDate >= req.DateFrom
                                                            && hd.DocDate <= req.DateTo
                                                      select new { hd, mp, ac }
                ).GroupBy(x => new { x.hd.DocDate, x.mp.AcctCode, x.ac.AcctName }).Select(x => new Sale02Response
                {
                    docDate = x.Key.DocDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    acctCode = x.Key.AcctCode,
                    acctName = x.Key.AcctName ?? "",
                    itemQty = x.Sum(s => s.hd.MeterAmt) ?? 0,
                    subAmt = x.Sum(s => s.hd.SaleAmt - s.hd.CouponAmt - s.hd.DiscAmt) ?? 0,
                }).ToListAsync();

            List<Sale02Response> periodtanks = await (from hd in this.context.DopPeriodTanks
                                                      join mp in this.context.MasProducts on new { hd.PdId } equals new { mp.PdId }
                                                      from ac in this.context.MasGlAccounts.Where(x => x.CompCode == hd.CompCode && x.AcctCode == mp.AcctCode).DefaultIfEmpty()
                                                      where hd.CompCode == req.CompCode
                                                            && hd.BrnCode == req.BrnCode
                                                            && hd.DocDate >= req.DateFrom
                                                            && hd.DocDate <= req.DateTo
                                                      select new { hd, mp, ac }
                ).GroupBy(x => new { x.hd.DocDate, x.mp.AcctCode, x.ac.AcctName }).Select(x => new Sale02Response
                {
                    docDate = x.Key.DocDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    acctCode = x.Key.AcctCode,
                    acctName = x.Key.AcctName ?? "",
                    itemQty = (x.Sum(s => s.hd.WithdrawQty) ?? 0) * -1,
                    subAmt = (Math.Round(x.Sum(s => s.hd.Unitprice * s.hd.WithdrawQty) ?? 0, 0)) * -1
                }).ToListAsync();

            cashsales.AddRange(periodcashs);
            cashsales.AddRange(periodtanks);

            response = cashsales.GroupBy(x => new { x.docDate, x.acctCode, x.acctName })
                .Select(x => new Sale02Response
                {
                    docDate = x.Key.docDate,
                    acctCode = x.Key.acctCode,
                    acctName = x.Key.acctName,
                    itemQty = Math.Round(x.Sum(s => s.itemQty), 2),
                    subAmt = Math.Round(x.Sum(s => s.subAmt), 2)
                }).ToList();


            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                                && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();


            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.saleAmt = Math.Round(x.subAmt * 100 / 107, 2);
                x.vatAmt = Math.Round(x.subAmt - (x.subAmt * 100 / 107), 2);
            });

            if (req.ReportType == ReportType.Detail)
            {
                return response.OrderBy(x => x.docDate).ThenBy(x => x.acctCode).ToList();
            }
            else
            {
                return response.OrderBy(x => x.acctCode).ThenBy(x => x.docDate).ToList();
            }
        }

        public async Task<MemoryStream> GetSale03ExcelAsync(SalesRequest req)
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            if (brn == null || company == null) return new MemoryStream(await pck.GetAsByteArrayAsync());

            var data = this.GetSale03PDF(req);

            // title company
            var companyTitle = ws.Cells[1, 3, 1, 7];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var printDate = ws.Cells[1, 9, 1, 11];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // title report
            var reportTitle = ws.Cells[2, 3, 2, 7];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานการขายสินค้า";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // title date
            var dateTitle = ws.Cells[3, 3, 3, 7];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchTitle = ws.Cells[4, 1, 4, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";


            ws.Cells[5, 1, 5, 1].Value = $"เลขที่";
            ws.Cells[5, 2, 5, 2].Value = $"สถานะ";
            ws.Cells[5, 3, 5, 3].Value = $"รหัสสินค้า";
            ws.Cells[5, 4, 5, 4].Value = $"ชื่อสินค้า";
            ws.Cells[5, 5, 5, 5].Value = $"ปริมาณ";
            ws.Cells[5, 6, 5, 6].Value = $"ราคา/หน่วย";
            ws.Cells[5, 7, 5, 7].Value = $"ราคารวม";
            ws.Cells[5, 8, 5, 8].Value = $"มูลค่าสินค้า";
            ws.Cells[5, 9, 5, 9].Value = $"ส่วนลด";
            ws.Cells[5, 10, 5, 10].Value = $"VAT";
            ws.Cells[5, 11, 5, 11].Value = $"คงเหลือ";

            ws.Cells[5, 1, 5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 11].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 11].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 11].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var rowIndex = 6;
            var docDate = data.Select(x => x.docDate).Distinct();
            foreach (var date in docDate)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่ :";
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"{Convert.ToDateTime(date).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
                rowIndex++;

                var items = data.Where(x => x.docDate == date).ToList();
                foreach (var item in items)
                {
                    ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.docNo;
                    var status = (item.docType == "Credit" && item.refDocNo != "") ? "(A+)" : (item.docStatus == "Cancel" ? "(C)" : "");
                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = status; //item.docStatus == "Cancel" ? $"(ยกเลิก)" : "";
                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.pdId;
                    ws.Cells[rowIndex, 4, rowIndex, 4].Value = item.pdName;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.itemQty;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.unitPrice;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.sumItemAmt;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.taxBaseAmt;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.discAmt;
                    ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.vatAmt;
                    ws.Cells[rowIndex, 11, rowIndex, 11].Value = item.subAmt;
                    rowIndex++;
                }

                ws.Cells[rowIndex, 3, rowIndex, 3].Value = $"รวม :";
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = $"{Convert.ToDateTime(date).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
                ws.Cells[rowIndex, 5, rowIndex, 5].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.sumItemAmt);
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.taxBaseAmt);
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.discAmt);
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.vatAmt);
                ws.Cells[rowIndex, 11, rowIndex, 11].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.subAmt);
                ws.Cells[rowIndex, 3, rowIndex, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                rowIndex++;
            }

            rowIndex++;
            ws.Cells[rowIndex, 3, rowIndex, 3].Value = $"รวมทั้งสิ้น";
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.sumItemAmt);
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.taxBaseAmt);
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.discAmt);
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.vatAmt);
            ws.Cells[rowIndex, 11, rowIndex, 11].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.subAmt);
            ws.Cells[rowIndex, 3, rowIndex, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            return new MemoryStream(await pck.GetAsByteArrayAsync());
        }

        public List<Sale03Response> GetSale03PDF(SalesRequest req)
        {
            List<Sale03Response> response = new List<Sale03Response>();

            if (req.DocType == SalesRequest.EDocType.CashAndCredit || req.DocType == SalesRequest.EDocType.Cash)
            {

                var queryCash = (from hd in this.context.SalCashsaleHds
                                 join dt in this.context.SalCashsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                                 where hd.CompCode == req.CompCode
                                       && hd.BrnCode == req.BrnCode
                                       //&& hd.DocStatus != "Cancel"
                                       && hd.DocDate >= req.DateFrom
                                       && hd.DocDate <= req.DateTo
                                 select new { hd, dt }
                    ).Select(x => new Sale03Response
                    {
                        docType = "Cash",
                        docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                        docNo = x.hd.DocNo,
                        docStatus = x.hd.DocStatus,
                        refDocNo = "",
                        remark = "(ย่อ-สด)",
                        pdId = x.dt.PdId,
                        pdName = x.dt.PdName,
                        unitPrice = x.dt.UnitPrice ?? 0,
                        itemQty = x.dt.ItemQty ?? 0,
                        sumItemAmt = x.dt.SumItemAmt ?? 0,
                        discAmt = x.dt.DiscAmt ?? 0,
                        vatAmt = x.dt.VatAmt ?? 0,
                        taxBaseAmt = x.dt.TaxBaseAmt ?? 0,
                        subAmt = x.dt.SubAmt ?? 0
                    }).AsQueryable();
                if (!string.IsNullOrEmpty(req.PdIdFrom) && !string.IsNullOrEmpty(req.PdIdTo) && queryCash != null)
                {
                    queryCash = queryCash.Where(x => string.Compare(x.pdId, req.PdIdFrom) >= 0 && string.Compare(x.pdId, req.PdIdTo) <= 0);
                }
                //cashs = queryCash.ToList();
                response.AddRange(queryCash.ToList());
            }

            if(req.DocType == SalesRequest.EDocType.CashAndCredit || req.DocType == SalesRequest.EDocType.Credit)
            {

                var queryCredit = (from hd in this.context.SalTaxinvoiceHds
                                   join dt in this.context.SalTaxinvoiceDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                                   where hd.CompCode == req.CompCode
                                         && hd.BrnCode == req.BrnCode
                                         //&& hd.DocStatus != "Cancel"
                                         && hd.DocDate >= req.DateFrom
                                         && hd.DocDate <= req.DateTo
                                   select new { hd, dt }
                                ).Select(x => new Sale03Response
                                {
                                    docType = "Credit",
                                    docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                                    docNo = x.hd.DocNo,
                                    docStatus = x.hd.DocStatus,
                                    refDocNo = x.hd.RefDocNo ?? "",
                                    custCode = x.hd.CustCode ?? "",
                                    remark = $"(เชื่อ-{x.hd.CustName})",
                                    pdId = x.dt.PdId,
                                    pdName = x.dt.PdName,
                                    unitPrice = x.dt.UnitPrice ?? 0,
                                    itemQty = x.dt.ItemQty ?? 0,
                                    sumItemAmt = x.dt.SumItemAmt ?? 0,
                                    discAmt = x.dt.DiscAmt ?? 0,
                                    vatAmt = x.dt.VatAmt ?? 0,
                                    taxBaseAmt = x.dt.TaxBaseAmt ?? 0,
                                    subAmt = x.dt.SubAmt ?? 0
                                }).AsQueryable();

                if (!string.IsNullOrEmpty(req.CustCodeFrom) && !string.IsNullOrEmpty(req.CustCodeTo) && queryCredit != null)
                {
                    queryCredit = queryCredit.Where(x => string.Compare(x.custCode, req.CustCodeFrom) >= 0 && string.Compare(x.custCode, req.CustCodeTo) <= 0);
                }
                if (!string.IsNullOrEmpty(req.PdIdFrom) && !string.IsNullOrEmpty(req.PdIdTo) && queryCredit != null)
                {
                    queryCredit = queryCredit.Where(x => string.Compare(x.pdId, req.PdIdFrom) >= 0 && string.Compare(x.pdId, req.PdIdTo) <= 0);
                }
                //credits = queryCredit.ToList();
                response.AddRange(queryCredit.ToList());
            }

           
            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                                && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();


            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public async Task<MemoryStream> GetSale04ExcelAsync(SalesRequest req)
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            if (brn == null || company == null) return new MemoryStream(await pck.GetAsByteArrayAsync());

            var data = this.GetSale04PDF(req);

            // title company
            var companyTitle = ws.Cells[1, 3, 1, 7];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var printDate = ws.Cells[1, 9, 1, 10];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // title report
            var reportTitle = ws.Cells[2, 3, 2, 7];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานการขายเชื่อ";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // title date
            var dateTitle = ws.Cells[3, 3, 3, 7];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchTitle = ws.Cells[4, 1, 4, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";


            ws.Cells[5, 1, 5, 1].Value = $"วันที่";
            ws.Cells[5, 2, 5, 2].Value = $"ใบกำกับ";
            ws.Cells[5, 3, 5, 3].Value = $"สถานะ";
            ws.Cells[5, 4, 5, 4].Value = $"ใบสั่งซื้อ";
            ws.Cells[5, 5, 5, 5].Value = $"ทะเบียนรถ";
            ws.Cells[5, 6, 5, 6].Value = $"รายการ";
            ws.Cells[5, 7, 5, 7].Value = $"ปริมาณ";
            ws.Cells[5, 8, 5, 8].Value = $"ราคาสินค้า";
            ws.Cells[5, 9, 5, 9].Value = $"หักส่วนลด";
            ws.Cells[5, 10, 5, 10].Value = $"ภาษีมูลค่าเพิ่ม";

            ws.Cells[5, 1, 5, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var rowIndex = 6;
            var custs = data.GroupBy(x => new { x.custCode, x.custName }).Select(x => new { x.Key.custCode, x.Key.custName });
            foreach (var cust in custs)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"ลูกค้า :";
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"{cust.custCode}";
                ws.Cells[rowIndex, 3, rowIndex, 4].Value = $"{cust.custName}";
                ws.Cells[rowIndex, 3, rowIndex, 4].Merge = true;

                rowIndex++;
                var items = data.Where(x => x.custCode == cust.custCode).ToList();
                foreach (var item in items)
                {
                    ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"{Convert.ToDateTime(item.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.docNo;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.docStatus == "Cancel" ? $"(ยกเลิก)" : "";
                    ws.Cells[rowIndex, 4, rowIndex, 4].Value = item.poNo;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.licensePlate;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.pdName;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.itemQty;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.sumItemAmt;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.discAmt;
                    ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.vatAmt;
                    rowIndex++;
                }

                ws.Cells[rowIndex, 3, rowIndex, 3].Value = $"รวม :";
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = $"{cust.custName}";

                ws.Cells[rowIndex, 7, rowIndex, 7].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.sumItemAmt);
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.discAmt);
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.vatAmt);
                ws.Cells[rowIndex, 3, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                rowIndex += 2;
            }

            ws.Cells[rowIndex, 3, rowIndex, 3].Value = $"รวมทั้งสิ้น";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.sumItemAmt);
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.discAmt);
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.vatAmt);
            ws.Cells[rowIndex, 3, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            return new MemoryStream(pck.GetAsByteArray());
        }

        public List<Sale04Response> GetSale04PDF(SalesRequest req)
        {
            List<Sale04Response> response = new List<Sale04Response>();

            response = (from hd in this.context.SalCreditsaleHds
                        join dt in this.context.SalCreditsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                        where hd.CompCode == req.CompCode
                              && hd.BrnCode == req.BrnCode
                              //&& hd.DocStatus != "Cancel"
                              && hd.DocDate >= req.DateFrom
                              && hd.DocDate <= req.DateTo
                        select new { hd, dt }
                ).Select(x => new Sale04Response
                {
                    docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                    docNo = x.hd.DocNo,
                    docStatus = x.hd.DocStatus,
                    custCode = x.hd.CustCode,
                    custName = x.hd.CustName,
                    poNo = x.dt.PoNo,
                    licensePlate = x.dt.LicensePlate,
                    pdId = x.dt.PdId,
                    pdName = x.dt.PdName,
                    itemQty = x.dt.ItemQty ?? 0,
                    sumItemAmt = x.dt.SumItemAmt ?? 0,
                    discAmt = x.dt.DiscAmt ?? 0,
                    subAmt = x.dt.SubAmt ?? 0,
                    vatAmt = x.dt.VatAmt ?? 0,
                }).ToList();

            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                                && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();

            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.custCode).ThenBy(x => x.docNo).ToList();
        }

        public async Task<MemoryStream> GetSale06ExcelAsync(SalesRequest req)
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = GetSale06PDF(req);

            var companyTitle = ws.Cells[1, 3, 1, 7];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var printDate = ws.Cells[1, 9, 1, 10];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // title report
            var reportTitle = ws.Cells[2, 3, 2, 7];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานการขายสินค้า สรุปตามรายลูกค้า";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // title date
            var dateTitle = ws.Cells[3, 3, 3, 7];
            dateTitle.Merge = true;
            dateTitle.Value = $"ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchTitle = ws.Cells[4, 1, 4, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";

            ws.Cells[5, 1, 5, 1].Value = "รหัสลูกค้า";
            ws.Cells[5, 2, 5, 6].Value = "ชื่อลูกค้า";
            ws.Cells[5, 7, 5, 7].Value = "ปริมาณรวม";
            ws.Cells[5, 8, 5, 8].Value = "ราคารวม";
            ws.Cells[5, 9, 5, 9].Value = "ส่วนลด";
            ws.Cells[5, 10, 5, 10].Value = "คงเหลือ";

            ws.Cells[5, 2, 5, 6].Merge = true;
            ws.Cells[5, 1, 5, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var rowIndex = 6;
            foreach (var item in data)
            {
                //รหัสลูกค้า
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.brnCode;
                //ชื่อลูกค้า
                ws.Cells[rowIndex, 2, rowIndex, 6].Value = item.brnName;
                ws.Cells[rowIndex, 2, rowIndex, 6].Merge = true;
                //ปริมาณรวม
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.itemQty;
                //ราคารวม
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.subAmt;
                //ส่วนลด
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.discAmt;
                //คงเหลือ
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.sumItemAmt;


                rowIndex++;
            }

            //Summary

            ws.Cells[rowIndex, 1, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = "รวมทั้งสิ้น";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Sum(s => s.itemQty);
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = data.Sum(s => s.subAmt);
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = data.Sum(s => s.discAmt);
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = data.Sum(s => s.sumItemAmt);

            ws.Cells[rowIndex, 6, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            return new MemoryStream(await pck.GetAsByteArrayAsync());
        }

        public List<Sale06Response> GetSale06PDF(SalesRequest req)
        {
            List<Sale06Response> response = new List<Sale06Response>();

            response = (from hd in this.context.SalTaxinvoiceHds
                        join dt in this.context.SalTaxinvoiceDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                        where hd.CompCode == req.CompCode
                              && hd.BrnCode == req.BrnCode
                              && hd.DocStatus != "Cancel"
                              && hd.DocDate >= req.DateFrom
                              && hd.DocDate <= req.DateTo
                        select new { hd, dt }
                ).GroupBy(x => new { x.hd.CustCode, x.hd.CustName }).Select(x => new Sale06Response
                {
                    custCode = x.Key.CustCode,
                    custName = x.Key.CustName,
                    itemQty = x.Sum(s => s.dt.ItemQty) ?? 0,
                    sumItemAmt = x.Sum(s => s.dt.SumItemAmt) ?? 0,
                    discAmt = x.Sum(s => s.dt.DiscAmt) ?? 0,
                    subAmt = x.Sum(s => s.dt.SubAmt) ?? 0,
                }).ToList();


            var branch = (from b in this.context.MasBranches
                          join c in this.context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                                && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();


            response.ForEach(x =>
            {
                x.compCode = branch.c.CompCode;
                x.compName = branch.c.CompName;
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.custCode).ToList();
        }
    }
}