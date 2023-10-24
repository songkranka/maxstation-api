using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using Report.API.Resources.ReportSummaryOilBalance;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class ReportSummaryOilBalanceRepository : SqlDataAccessHelper, IReportSummaryOilBalanceRepository
    {
        private readonly IMapper _mapper;
        public ReportSummaryOilBalanceRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public MemoryStream GetReportSummaryOilBalanceExcel(ReportSummaryOilBalanceRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();

            var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var ws = pck.Workbook.Worksheets.Add("sheet1");

            var headerRowLeft = ws.Cells[1, 1, 1, 5];
            headerRowLeft.Merge = true;
            headerRowLeft.Value = $"{company.CompName} : 52{brn.BrnCode} {brn.BrnName}";

            var headerRowRight = ws.Cells[1, 8, 1, 9];
            headerRowRight.Merge = true;
            headerRowRight.Value = $"วันที่ {docDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

            ws.Cells[2, 1, 2, 9].Merge = true;
            ws.Cells[2, 1, 2, 9].Value = "สรุปปริมาณน้ำมันสถานีบริการประจำวัน";

            var resultTank = context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate).AsNoTracking().ToList();
            var groupResultTank = resultTank.GroupBy(x => x.PeriodNo).Select(x => new { period = x.First().PeriodNo, data = x.ToList() });

            var rowIndex = 4;
            foreach (var data in groupResultTank)
            {
                var mapResultTank = _mapper.Map<List<DopPeriodTank>, List<TankModel>>(data.data);

                ws.Cells[rowIndex, 1, rowIndex, 9].Merge = true;
                ws.Cells[rowIndex, 1, rowIndex, 9].Value = $"รายการวัดปริมาณน้ำมันในถังเก็บ กะที่ {data.period}";
                rowIndex++;

                ws.Cells[rowIndex, 1, rowIndex, 1].Value = "ถัง";
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = "ชนิดน้ำมัน";
                ws.Cells[rowIndex, 3, rowIndex, 3].Value = "ยอดยกมา";
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = "รับเข้าถัง";
                ws.Cells[rowIndex, 5, rowIndex, 5].Value = "โอนเข้าคลัง";
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = "จ่ายออก";
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = "คงเหลือ";
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = "วัดได้จริง";
                ws.Cells[rowIndex, 9, rowIndex, 9].Value = "ขาด/เกิน";

                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));
                rowIndex++;

                foreach (var item in mapResultTank)
                {
                    var colIndex = 1;
                    foreach (var col in item.GetType().GetProperties())
                    {
                        var row = ws.Cells[rowIndex, colIndex, rowIndex, colIndex];
                        row.Style.Numberformat.Format = "#,##0.00";
                        row.Value = col.GetValue(item);
                        colIndex++;
                    }
                    rowIndex++;
                }

                var sumTank = new TankModel()
                {
                    TankId = "",
                    PdName = "รวม",
                    BeforeQty = mapResultTank.Sum(x => x.BeforeQty),
                    ReceiveQty = mapResultTank.Sum(x => x.ReceiveQty),
                    TransferQty = mapResultTank.Sum(x => x.TransferQty),
                    IssueQty = mapResultTank.Sum(x => x.IssueQty),
                    RemainQty = mapResultTank.Sum(x => x.RemainQty),
                    RealQty = mapResultTank.Sum(x => x.RealQty),
                    DiffQty = mapResultTank.Sum(x => x.DiffQty)
                };

                var colIndexSum = 1;
                foreach (var col in sumTank.GetType().GetProperties())
                {
                    var row = ws.Cells[rowIndex, colIndexSum, rowIndex, colIndexSum];
                    if (colIndexSum == 2)
                    {
                        row = ws.Cells[rowIndex, colIndexSum - 1, rowIndex, colIndexSum];
                        row.Merge = true;
                        row.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        row.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    row.Style.Numberformat.Format = "#,##0.00";
                    row.Value = col.GetValue(sumTank);
                    colIndexSum++;
                }
                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#3CB371"));
                rowIndex += 2;
            }

            return new MemoryStream(pck.GetAsByteArray());
        }

        public ResponseTankModelForPDF GetReportSummaryOilBalancePDF(ReportSummaryOilBalanceRequest req)
        {
            var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var response = new ResponseTankModelForPDF();

            var resultTank = context.DopPeriodTanks.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate).AsNoTracking().ToList();
            var groupResultTank = resultTank.GroupBy(x => x.PeriodNo).Select(x => new HdTankModelForPDF { period = x.First().PeriodNo, dtItems = _mapper.Map<List<DopPeriodTank>, List<DtTankModelForPDF>>(x.ToList()) }).ToList();
          
            var pfirst = resultTank.Min(x => x.PeriodNo);
            var plast = resultTank.Max(x => x.PeriodNo);
            var tankSum = resultTank.Where(x => x.PeriodNo == pfirst).ToList();
            var lastPeriod = resultTank.Where(x => x.PeriodNo == plast).ToList();

            tankSum.ForEach(x => {
                x.PeriodNo = 0;
                x.ReceiveQty = resultTank.Where(t=>t.TankId == x.TankId).Sum(s => s.ReceiveQty);
                x.TransferQty = resultTank.Where(t=>t.TankId == x.TankId).Sum(s => s.TransferQty);
                x.IssueQty = resultTank.Where(t=>t.TankId == x.TankId).Sum(s => s.IssueQty);
                x.RemainQty = x.BeforeQty + x.ReceiveQty - x.TransferQty - x.IssueQty;
                x.RealQty = lastPeriod.FirstOrDefault(t => t.TankId == x.TankId).RealQty;
                x.DiffQty = resultTank.Where(t=>t.TankId == x.TankId).Sum(s => s.DiffQty);
            });
            var resultTankSum = tankSum.GroupBy(x => x.PeriodNo).Select(x => new HdTankModelForPDF { period = x.First().PeriodNo, dtItems = _mapper.Map<List<DopPeriodTank>, List<DtTankModelForPDF>>(x.ToList()) }).ToList();
            groupResultTank.AddRange(resultTankSum);

            response.brnCode = brn.BrnCode;
            response.brnName = brn.BrnName;
            response.compName = company.CompName;
            response.compImage = company.CompImage;
            response.docDate = docDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            response.tankData = groupResultTank;
            //response.tankSum = resultTankSum;

            return response;
        }
    }
}
