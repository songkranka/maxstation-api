using AutoMapper;
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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class FinanceRepository : SqlDataAccessHelper, IFinanceRepository
    {
        private readonly IMapper _mapper;

        public FinanceRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<MemoryStream> GetFin03ExcelAsync(FinanceRequest req)
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            if (brn == null && company == null) return new MemoryStream(await pck.GetAsByteArrayAsync());

            var companyTitle = ws.Cells[1, 3, 1, 12];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // title report
            var reportTitle = ws.Cells[2, 3, 2, 12];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานทะเบียนรับรวม";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var printDate = ws.Cells[1, 13, 1, 14];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // title date
            var dateTitle = ws.Cells[3, 3, 3, 12];
            dateTitle.Merge = true;
            dateTitle.Value = $"ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchTitle = ws.Cells[4, 1, 4, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";

            ws.Cells[5, 1, 5, 1].Value = "เลขที่";
            ws.Cells[5, 2, 5, 2].Value = "รหัสลูกค้า";
            ws.Cells[5, 3, 5, 4].Value = "ชื่อลูกค้า";
            ws.Cells[5, 5, 5, 5].Value = "ใบเสร็จ";
            ws.Cells[5, 6, 5, 6].Value = "ประเภทการรับ";
            ws.Cells[5, 7, 5, 7].Value = "ประเภทตราสาร";
            ws.Cells[5, 8, 5, 8].Value = "ธนาคาร";
            ws.Cells[5, 9, 5, 9].Value = "หมายเลขตราสาร";
            ws.Cells[5, 10, 5, 10].Value = "ลงวันที่";
            ws.Cells[5, 11, 5, 11].Value = "จำนวนเงินรับสุทธิ";
            ws.Cells[5, 12, 5, 12].Value = "ภาษีถูกหักฯ";
            ws.Cells[5, 13, 5, 13].Value = "Vat";
            ws.Cells[5, 14, 5, 14].Value = "เงินสดสุทธิ";

            ws.Cells[5, 3, 5, 4].Merge = true;
            ws.Cells[5, 1, 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 14].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 14].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));


            var data = GetFin03PDF(req);
            var groupData = data.GroupBy(g => g.docDate).Select(s => new
            {
                DocDate = s.Key,
                Docs = s.Select(d => new
                {
                    d.docNo,
                    d.custCode,
                    d.custName,
                    d.billNo,
                    d.receiveType,
                    d.payType,
                    d.bankName,
                    d.payNo,
                    d.payDate,
                    d.netAmt,
                    d.whtAmt,
                    d.vatAmt,
                    d.subAmt
                })
            }).OrderBy(o => o.DocDate);

            var rowIndex = 6;
            foreach (var item in groupData)
            {
                var docDate = Convert.ToDateTime(DateTime.ParseExact(item.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = "วันที่ ";
                ws.Cells[rowIndex, 2, rowIndex, 14].Value = docDate;
                ws.Cells[rowIndex, 2, rowIndex, 14].Merge = true;
                ws.Cells[rowIndex, 2, rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                rowIndex++;

                foreach (var doc in item.Docs)
                {
                    ws.Cells[rowIndex, 1, rowIndex, 1].Value = doc.docNo;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = doc.custCode;
                    ws.Cells[rowIndex, 3, rowIndex, 4].Value = doc.custName;
                    ws.Cells[rowIndex, 3, rowIndex, 4].Merge = true;

                    ws.Cells[rowIndex, 5, rowIndex, 5].Value = doc.billNo;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = doc.receiveType;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = doc.payType;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = doc.bankName;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Value = doc.payNo;
                    ws.Cells[rowIndex, 10, rowIndex, 10].Value = doc.payDate;
                    ws.Cells[rowIndex, 11, rowIndex, 11].Value = doc.netAmt;
                    ws.Cells[rowIndex, 12, rowIndex, 12].Value = doc.whtAmt;
                    ws.Cells[rowIndex, 13, rowIndex, 13].Value = doc.vatAmt;
                    ws.Cells[rowIndex, 14, rowIndex, 14].Value = doc.subAmt;

                    ws.Cells[rowIndex, 11, rowIndex, 14].Style.Numberformat.Format = "#,##0.00";
                }

                rowIndex++;
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = $"รวมวันที่: {docDate}";
                ws.Cells[rowIndex, 11, rowIndex, 11].Value = item.Docs.Sum(s => s.netAmt);
                ws.Cells[rowIndex, 12, rowIndex, 12].Value = item.Docs.Sum(s => s.whtAmt);
                ws.Cells[rowIndex, 13, rowIndex, 13].Value = item.Docs.Sum(s => s.vatAmt);
                ws.Cells[rowIndex, 14, rowIndex, 14].Value = item.Docs.Sum(s => s.subAmt);

                ws.Cells[rowIndex, 1, rowIndex, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[rowIndex, 11, rowIndex, 14].Style.Numberformat.Format = "#,##0.00";

                rowIndex++;
            }

            ws.Cells[rowIndex, 8, rowIndex, 8].Value = "รวมทั้งสิ้น : ";
            ws.Cells[rowIndex, 11, rowIndex, 11].Value = groupData.Sum(s => s.Docs.Sum(i => i.netAmt));
            ws.Cells[rowIndex, 12, rowIndex, 12].Value = groupData.Sum(s => s.Docs.Sum(i => i.whtAmt));
            ws.Cells[rowIndex, 13, rowIndex, 13].Value = groupData.Sum(s => s.Docs.Sum(i => i.vatAmt));
            ws.Cells[rowIndex, 14, rowIndex, 14].Value = groupData.Sum(s => s.Docs.Sum(i => i.subAmt));

            ws.Cells[rowIndex, 11, rowIndex, 14].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[rowIndex, 1, rowIndex, 14].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 1, rowIndex, 14].AutoFitColumns();

            return new MemoryStream(await pck.GetAsByteArrayAsync());
        }

        public List<Fin03Response> GetFin03PDF(FinanceRequest req)
        {
            List<Fin03Response> response = new List<Fin03Response>();

            var query = this.context.FinReceiveHds.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate >= req.DateFrom && x.DocDate <= req.DateTo)
                .Select(x => new Fin03Response
                {
                    custCode = x.CustCode,
                    custName = x.CustName,
                    accountNo = x.AccountNo,
                    billNo = x.BillNo,
                    bankNo = x.BankNo ?? "",
                    bankName = x.BankName ?? "",
                    docStatus = x.DocStatus,
                    docNo = x.DocNo,
                    docDate = x.DocDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    receiveType = x.ReceiveType,
                    payType = x.PayType,
                    payNo = x.PayNo ?? "",
                    payDate = x.PayDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    remark = x.Remark ?? "",
                    subAmt = x.SubAmt ?? 0,
                    whtAmt = x.WhtAmt ?? 0,
                    vatAmt = x.VatAmt ?? 0,
                    netAmt = x.NetAmt ?? 0,
                }).AsQueryable();

            if (!string.IsNullOrEmpty(req.CustCodeFrom) && !string.IsNullOrEmpty(req.CustCodeTo))
            {
                query = query.Where(x => string.Compare(x.custCode, req.CustCodeFrom) >= 0 && string.Compare(x.custCode, req.CustCodeTo) <= 0).AsQueryable();
            }

            response = query.ToList();

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

        public async Task<MemoryStream> GetFin08ExcelAsync(FinanceRequest req)
        {
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            if (brn == null && company == null) return new MemoryStream(await pck.GetAsByteArrayAsync());

            var companyTitle = ws.Cells[1, 3, 1, 9];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // title report
            var reportTitle = ws.Cells[2, 3, 2, 9];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานรายละเอียด รายได้ ค่าเช่า ภาษีโรงเรือน น้ำ ไฟ";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // title date
            var dateTitle = ws.Cells[3, 3, 3, 9];
            dateTitle.Merge = true;
            dateTitle.Value = $"ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Branch
            var branchTitle = ws.Cells[4, 1, 4, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";

            // Print Date
            var printDate = ws.Cells[1, 11, 1, 12];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            ws.Cells[5, 1, 6, 1].Value = "วันที่เอกสาร";
            ws.Cells[5, 1, 6, 1].Merge = true;

            ws.Cells[5, 2, 6, 2].Value = "เลขที่เอกสาร";
            ws.Cells[5, 2, 6, 2].Merge = true;

            ws.Cells[5, 3, 6, 3].Value = "รหัสลูกค้า";
            ws.Cells[5, 3, 6, 3].Merge = true;

            ws.Cells[5, 4, 6, 5].Value = "ชื่อลูกค้า";
            ws.Cells[5, 4, 6, 5].Merge = true;

            ws.Cells[5, 6, 6, 6].Value = "รายได้ค่าเช่า";
            ws.Cells[5, 6, 6, 6].Merge = true;

            ws.Cells[5, 7, 6, 7].Value = "ภาษีโรงเรือน";
            ws.Cells[5, 7, 6, 7].Merge = true;

            ws.Cells[5, 8, 5, 9].Value = "น้ำประปา";
            ws.Cells[5, 8, 5, 9].Merge = true;

            ws.Cells[5, 10, 5, 11].Value = "ไฟฟ้า";
            ws.Cells[5, 10, 5, 11].Merge = true;

            ws.Cells[5, 12, 6, 12].Value = "สถานะ";
            ws.Cells[5, 12, 6, 12].Merge = true;

            ws.Cells[6, 8, 6, 8].Value = "ก่อน VAT";
            ws.Cells[6, 9, 6, 9].Value = "รวม VAT";
            ws.Cells[6, 10, 6, 10].Value = "ก่อน VAT";
            ws.Cells[6, 11, 6, 11].Value = "รวม VAT";

            ws.Cells[5, 1, 6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 6, 12].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 6, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 6, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 6, 12].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 6, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 6, 12].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var data = GetFin08PDF(req);
            var groupItems = data.GroupBy(g => g.docDate)
                .Select(s => new
                {
                    DocDate = s.Key,
                    Docs = s.Select(i => new
                    {
                        i.docNo,
                        i.custCode,
                        i.custName,
                        i.docStatus,
                        i.pdId,
                        i.vatAmt,
                        i.taxBaseAmt,
                        i.subAmt
                    }).GroupBy(g => new
                    {
                        g.docNo,
                        g.custCode,
                        g.custName,
                        g.docStatus,
                    }).Select(s => new
                    {
                        s.Key.docNo,
                        s.Key.custCode,
                        s.Key.custName,
                        s.Key.docStatus,
                        Rent = s.Where(r => r.pdId == "90552" && r.docStatus != "Cancel").Sum(r => r.subAmt),
                        Building = s.Where(r => r.pdId == "90569" && r.docStatus != "Cancel").Sum(r => r.subAmt),
                        WaterBeforVat = s.Where(r => r.pdId == "90581" && r.docStatus != "Cancel").Sum(r => r.taxBaseAmt),
                        WaterVat = s.Where(r => r.pdId == "90581" && r.docStatus != "Cancel").Sum(r => r.subAmt),
                        ElectricBeforVat = s.Where(r => r.pdId == "90575" && r.docStatus != "Cancel").Sum(r => r.taxBaseAmt),
                        ElectricVat = s.Where(r => r.pdId == "90575" && r.docStatus != "Cancel").Sum(r => r.subAmt)
                    })
                }).ToList();


            var rowIndex = 7;
            foreach (var item in groupItems)
            {
                var docDate = Convert.ToDateTime(DateTime.ParseExact(item.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                ws.Cells[rowIndex, 1, rowIndex, 1].Value = docDate;
                foreach (var doc in item.Docs)
                {
                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = doc.docNo;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = doc.custCode;
                    ws.Cells[rowIndex, 4, rowIndex, 5].Value = doc.custName;
                    ws.Cells[rowIndex, 4, rowIndex, 5].Merge = true;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = doc.Rent;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = doc.Building;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = doc.WaterBeforVat;
                    ws.Cells[rowIndex, 9, rowIndex, 9].Value = doc.WaterVat;
                    ws.Cells[rowIndex, 10, rowIndex, 10].Value = doc.ElectricBeforVat;
                    ws.Cells[rowIndex, 11, rowIndex, 11].Value = doc.ElectricVat;
                    ws.Cells[rowIndex, 12, rowIndex, 12].Value = doc.docStatus == "Cancel" ? "ยกเลิก" : string.Empty;

                    rowIndex++;
                }

                ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมวันที่: {docDate}";
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.Docs.Sum(s => s.Rent);
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.Docs.Sum(s => s.Building);
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.Docs.Sum(s => s.WaterBeforVat);
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.Docs.Sum(s => s.WaterVat);
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.Docs.Sum(s => s.ElectricBeforVat);
                ws.Cells[rowIndex, 11, rowIndex, 11].Value = item.Docs.Sum(s => s.ElectricVat);

                ws.Cells[rowIndex, 1, rowIndex, 12].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rowIndex++;
            }

            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมทั้งสิ้น";
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = groupItems.Sum(s => s.Docs.Sum(d => d.Rent));
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = groupItems.Sum(s => s.Docs.Sum(d => d.Building));
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = groupItems.Sum(s => s.Docs.Sum(d => d.WaterBeforVat));
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = groupItems.Sum(s => s.Docs.Sum(d => d.WaterVat));
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = groupItems.Sum(s => s.Docs.Sum(d => d.ElectricBeforVat));
            ws.Cells[rowIndex, 11, rowIndex, 11].Value = groupItems.Sum(s => s.Docs.Sum(d => d.ElectricVat));

            ws.Cells[7, 6, rowIndex, 11].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[rowIndex, 1, rowIndex, 12].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[1, 1, rowIndex, 12].AutoFitColumns();

            return new MemoryStream(await pck.GetAsByteArrayAsync());
        }

        public List<Fin08Response> GetFin08PDF(FinanceRequest req)
        {
            List<Fin08Response> response = new List<Fin08Response>();

            var query = (from hd in this.context.SalCreditsaleHds
                         join dt in this.context.SalCreditsaleDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         where hd.CompCode == req.CompCode
                               && hd.BrnCode == req.BrnCode
                               && hd.DocDate >= req.DateFrom
                               && hd.DocDate <= req.DateTo
                               && hd.DocType == "Invoice"
                         select new { hd, dt }
                ).AsQueryable();

            if (!string.IsNullOrEmpty(req.CustCodeFrom) && !string.IsNullOrEmpty(req.CustCodeTo))
            {
                query = query.Where(x => string.Compare(x.hd.CustCode, req.CustCodeFrom) >= 0 && string.Compare(x.hd.CustCode, req.CustCodeTo) <= 0).AsQueryable();
            }

            response = query.Select(x => new Fin08Response
            {
                docNo = x.hd.DocNo,
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                docStatus = x.hd.DocStatus,
                custCode = x.hd.CustCode,
                custName = x.hd.CustName,
                pdId = x.dt.PdId,
                vatAmt = x.dt.VatAmt ?? 0,
                taxBaseAmt = x.dt.TaxBaseAmt ?? 0,
                subAmt = x.dt.SubAmt ?? 0,
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

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ThenBy(x => x.pdId).ToList();
        }
    }
}