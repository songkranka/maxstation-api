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
    public class CustomerRepository : SqlDataAccessHelper, ICustomerRepository
    {
        private readonly IMapper _mapper;

        public CustomerRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }


        public async Task<MemoryStream> GetDebtor02ExcelAsync(CustomerRequest req)
        {
            var data = this.GetDebtor02PDF(req);

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

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
            ws.Cells[5, 2, 5, 4].Value = "ชื่อลูกค้า";
            ws.Cells[5, 5, 5, 5].Value = "วันที่เอกสาร";
            ws.Cells[5, 6, 5, 6].Value = "เลขที่เอกสาร";
            ws.Cells[5, 7, 5, 7].Value = "ยอดขาย";
            ws.Cells[5, 8, 5, 8].Value = "ยอดตัดหนี้";
            ws.Cells[5, 9, 5, 9].Value = "เพิ่มหนี้/ลดหนี้";
            ws.Cells[5, 10, 5, 10].Value = "คงค้าง";

            ws.Cells[5, 2, 5, 4].Merge = true;
            ws.Cells[5, 1, 5, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 10].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var groupData = data.GroupBy(g => new { g.custCode, g.custName })
                .Select(s => new
                {
                    s.Key.custCode,
                    s.Key.custName,
                    Dates = s.Select(i => new
                    {
                        DocDate = Convert.ToDateTime(DateTime.ParseExact(i.docDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        i.docNo,
                        i.netAmt,
                        i.balanceAmt
                    }).GroupBy(g => new { g.DocDate })
                    .Select(i => new
                    {
                        i.Key.DocDate,
                        Docs = i.Select(k => new
                        {
                            k.docNo,
                            k.netAmt,
                            k.balanceAmt
                        })
                    })
                });

            var rowIndex = 6;
            foreach (var customer in groupData)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = customer.custCode;
                ws.Cells[rowIndex, 2, rowIndex, 4].Value = customer.custName;
                ws.Cells[rowIndex, 2, rowIndex, 4].Merge = true;

                foreach (var date in customer.Dates)
                {
                    ws.Cells[rowIndex, 5, rowIndex, 5].Value = date.DocDate;
                    foreach (var item in date.Docs)
                    {
                        ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.docNo;
                        ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.netAmt;
                        ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.netAmt - item.balanceAmt;
                        ws.Cells[rowIndex, 9, rowIndex, 9].Value = string.Empty;
                        ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.balanceAmt;
                        ws.Cells[rowIndex, 7, rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
                        rowIndex++;
                    }

                    ws.Cells[rowIndex, 1, rowIndex, 10].AutoFitColumns();
                    //rowIndex++;
                }

                ws.Cells[rowIndex, 2, rowIndex, 6].Value = $"รวมลูกค้า: {customer.custName}";
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = customer.Dates.Sum(s => s.Docs.Sum(i => i.netAmt));
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = customer.Dates.Sum(s => s.Docs.Sum(i => i.netAmt - i.balanceAmt));
                ws.Cells[rowIndex, 10, rowIndex, 10].Value = customer.Dates.Sum(s => s.Docs.Sum(i => i.balanceAmt));

                ws.Cells[rowIndex, 2, rowIndex, 6].Merge = true;
                ws.Cells[rowIndex, 1, rowIndex, 10].AutoFitColumns();
                ws.Cells[rowIndex, 7, rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[rowIndex, 2, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[rowIndex, 2, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                rowIndex = rowIndex + 2;
            }

            ws.Cells[rowIndex, 2, rowIndex, 6].Value = "รวมทั้งสิ้นทุกลูกค้า: ";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = groupData.Sum(c => c.Dates.Sum(s => s.Docs.Sum(i => i.netAmt)));
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = groupData.Sum(c => c.Dates.Sum(s => s.Docs.Sum(i => i.netAmt - i.balanceAmt)));
            ws.Cells[rowIndex, 10, rowIndex, 10].Value = groupData.Sum(c => c.Dates.Sum(s => s.Docs.Sum(i => i.balanceAmt)));

            ws.Cells[rowIndex, 7, rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[rowIndex, 2, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 2, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            ws.Cells[rowIndex, 1, rowIndex, 10].AutoFitColumns();
            ws.Cells[rowIndex, 2, rowIndex, 6].Merge = true;

            return new MemoryStream(await pck.GetAsByteArrayAsync());
        }

        public List<Debtor02Response> GetDebtor02PDF(CustomerRequest req)
        {
            List<Debtor02Response> response = new List<Debtor02Response>();

            var query = (from hd in this.context.FinBalances
                         join mc in this.context.MasCustomers on new { hd.CustCode } equals new { mc.CustCode }
                         where hd.CompCode == req.CompCode
                         && hd.BrnCode == req.BrnCode
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         && hd.BalanceAmt > 0
                         select new { hd, mc }
                         ).AsQueryable();

            if (!string.IsNullOrEmpty(req.CustCodeFrom) && !string.IsNullOrEmpty(req.CustCodeTo))
            {
                query = query.Where(x => string.Compare(x.hd.CustCode, req.CustCodeFrom) >= 0 && string.Compare(x.hd.CustCode, req.CustCodeTo) <= 0).AsQueryable();
            }

            response = query.Select(x => new Debtor02Response
            {
                compCode = x.hd.CompCode,
                brnCode = x.hd.BrnCode,
                docType = x.hd.DocType,
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                docNo = x.hd.DocNo,
                custCode = x.hd.CustCode,
                custName = x.mc.CustName,
                netAmt = x.hd.NetAmt ?? 0,
                balanceAmt = x.hd.BalanceAmt ?? 0
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

            return response.OrderBy(x => x.custCode).ThenBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public async Task<MemoryStream> ExportExcelAsync(ExportExcelRequest req)
        {
            var data = context.MasCustomers.FirstOrDefault(x => x.CustCode == req.CustCode);
            var customerCars = context.MasCustomerCars.Where(x => x.CustCode == req.CustCode);

            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            var cell = ws.Cells[1, 1, 1, 1];
            cell.Merge = false;
            cell.Value = $"รหัสลูกค้า :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[1, 2, 1, 2];
            cell.Merge = false;
            cell.Value = data.CustCode;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[1, 4, 1, 4];
            cell.Merge = false;
            cell.Value = $"SAP CODE :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[1, 5, 1, 5];
            cell.Merge = false;
            cell.Value = data.MapCustCode;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            //row 2
            cell = ws.Cells[2, 1, 2, 1];
            cell.Merge = false;
            cell.Value = $"คำนำหน้า :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[2, 2, 2, 2];
            cell.Merge = false;
            cell.Value = data.CustPrefix;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[2, 4, 2, 4];
            cell.Merge = false;
            cell.Value = $"ชื่อลูกค้า :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[2, 5, 2, 5];
            cell.Merge = false;
            cell.Value = data.CustName;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[3, 1, 3, 1];
            cell.Merge = false;
            cell.Value = $"ที่อยู่ :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[3, 2, 3, 5];
            cell.Merge = true;
            cell.Value = data.Address;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[4, 1, 4, 1];
            cell.Merge = false;
            cell.Value = $"จังหวัด :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[4, 2, 4, 2];
            cell.Merge = false;
            cell.Value = data.ProvName;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[4, 4, 4, 4];
            cell.Merge = false;
            cell.Value = $"อำเภอ :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[4, 5, 4, 5];
            cell.Merge = false;
            cell.Value = data.District;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[5, 1, 5, 1];
            cell.Merge = false;
            cell.Value = $"เบอร์โทรศัพท์ :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[5, 2, 5, 2];
            cell.Merge = false;
            cell.Value = data.Phone;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[5, 4, 5, 4];
            cell.Merge = false;
            cell.Value = $"Fax :";
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            cell = ws.Cells[5, 5, 5, 5];
            cell.Merge = false;
            cell.Value = data.Fax;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            //start license_plate
            var rowIndex = 7;
            cell = ws.Cells[rowIndex, 1, rowIndex, 1];
            cell.Value = $"ลำดับ";
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //cell.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            //cell.Style.Border.Right.Style = ExcelBorderStyle.Medium;
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cell = ws.Cells[rowIndex, 2, rowIndex, 2];
            cell.Value = $"ทะเบียนรถ";
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //cell.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            //cell.Style.Border.Right.Style = ExcelBorderStyle.Medium;
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            rowIndex++;
            int seqNo = 1;
            foreach (var customerCar in customerCars)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = seqNo++;
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = customerCar.LicensePlate;
                rowIndex++;
            }


            return new MemoryStream(await pck.GetAsByteArrayAsync());
        }
    }
}
