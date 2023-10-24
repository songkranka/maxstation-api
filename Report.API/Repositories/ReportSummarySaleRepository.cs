using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Oracle.ManagedDataAccess.Client;
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
    public class ReportSummarySaleRepository : SqlDataAccessHelper, IReportSummarySaleRepository
    {
        private readonly IMapper _mapper;
       // private readonly string _connectionString;
        public ReportSummarySaleRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
           // _connectionString = posConnect.ConnectionString;
        }

        public List<GetPeriodResponse> GetPeriod(ReportSummarySaleRequest req)
        {
            try
            {
                var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var query = context.DopPeriods.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate).AsNoTracking().ToList();

                var listResponse = new List<GetPeriodResponse>();
                foreach (var item in query) 
                { 
                    var response = new GetPeriodResponse() 
                    { 
                        PeriodNo = item.PeriodNo,
                        PeriodName = $"กะที่ {item.PeriodNo}"
                    };

                    listResponse.Add(response);
                }

                if (listResponse.Count > 0) 
                {
                    var allPeriod = new GetPeriodResponse()
                    {
                        PeriodNo = 0,
                        PeriodName = $"ทั้งหมด"
                    };

                    listResponse.Insert(0, allPeriod);
                }

                return listResponse;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public MemoryStream GetReportSummarySaleExcel(ReportSummarySaleRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();

            #region get all data
            var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var queryMeter = context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryTankSum = context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryCash = context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryCashSum = context.DopPeriodCashSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryCashGl = context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryEmp = context.DopPeriodEmps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);

            if (req.PeriodNo != 0)
            {
                queryMeter = queryMeter.Where(x => x.PeriodNo == req.PeriodNo);
                queryTankSum = queryTankSum.Where(x => x.PeriodNo == req.PeriodNo);
                queryCash = queryCash.Where(x => x.PeriodNo == req.PeriodNo);
                queryCashSum = queryCashSum.Where(x => x.PeriodNo == req.PeriodNo);
                queryCashGl = queryCashGl.Where(x => x.PeriodNo == req.PeriodNo);
            }

            var dopMeter = queryMeter.OrderBy(x => x.DispId).ThenBy(x => x.PeriodNo).AsNoTracking().ToList();
            var tankSum = queryTankSum.OrderBy(x => x.PdId).ThenBy(x => x.PeriodNo).AsNoTracking().ToList();
            var dopCash = queryCash.OrderBy(x => x.PdId).ThenBy(x => x.PeriodNo).AsNoTracking().ToList();
            var cashSum = queryCashSum.OrderBy(x => x.PeriodNo).AsNoTracking().ToList();
            var cashGl = queryCashGl.OrderBy(x => x.PeriodNo).AsNoTracking().ToList();
            var emp = queryEmp.OrderBy(x => x.PeriodNo).AsNoTracking().ToList();

            //for get host id
            var queryMas = context.MasBranchDisps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).OrderBy(x => x.DispId).AsNoTracking().ToList();

            //var meter = _mapper.Map<List<DopPeriodMeter>, List<MasBranchDisp>>(dopMeter);            
            //meter.ForEach(x =>
            //{
            //    x.HoseId = queryMas.Where(y => y.DispId == x.DispId).Select(y => y.HoseId).FirstOrDefault();
            //});
            #endregion

            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            #region Header
            //for know length of end col
            var colHeaderRight = 6;
            var groupheaderColRight = tankSum.GroupBy(x => x.PdId).Select(x => x.First().PdName).ToList();

            groupheaderColRight.Insert(0, "รายการ");
            groupheaderColRight.Insert(groupheaderColRight.Count, "รวม");


            var headerRowLeft = ws.Cells[1, 1, 1, colHeaderRight + groupheaderColRight.Count];
            headerRowLeft.Merge = true;
            headerRowLeft.Value = $"{company.CompName} : 52{brn.BrnCode} {brn.BrnName}";

            if (req.PeriodNo == 0)
            {
                ws.Cells[2, 1, 2, (colHeaderRight + groupheaderColRight.Count) - 2].Merge = true;
                ws.Cells[2, 1, 2, (colHeaderRight + groupheaderColRight.Count) - 2].Value = "สรุปรายการหน้าสถานี ประจำวัน";

                var headerRowRight = ws.Cells[2, (colHeaderRight + groupheaderColRight.Count) - 1, 2, colHeaderRight + groupheaderColRight.Count];
                headerRowRight.Merge = true;
                headerRowRight.Value = $"วันที่ {docDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            }
            else 
            {
                ws.Cells[2, 1, 2, (colHeaderRight + groupheaderColRight.Count) - 7].Merge = true;
                ws.Cells[2, 1, 2, (colHeaderRight + groupheaderColRight.Count) - 7].Value = "สรุปรายการหน้าสถานี ประจำวัน";

                var headerRowCenter = ws.Cells[2, (colHeaderRight + groupheaderColRight.Count) - 3, 2, (colHeaderRight + groupheaderColRight.Count) - 2];
                headerRowCenter.Merge = true;
                headerRowCenter.Value = $"กะที่ {req.PeriodNo}";

                var headerRowRight = ws.Cells[2, (colHeaderRight + groupheaderColRight.Count) - 1, 2, colHeaderRight + groupheaderColRight.Count];
                headerRowRight.Merge = true;
                headerRowRight.Value = $"วันที่ {docDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            }

            //left
            ws.Cells[3, 1, 4, 1].Merge = true;
            ws.Cells[3, 1, 4, 1].Value = "หัวจ่ายที่";
            ws.Cells[3, 1, 4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[3, 1, 4, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[3, 2, 3, 3].Merge = true;
            ws.Cells[3, 2, 3, 3].Value = "ตัวเลขมิเตอร์";
            ws.Cells[3, 2, 3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[4, 2, 4, 2].Value = "เริ่มวัน";
            ws.Cells[4, 2, 4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[4, 3, 4, 3].Value = "จบวัน";
            ws.Cells[4, 3, 4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[3, 4, 3, 4].Value = "ผลต่าง";
            ws.Cells[3, 4, 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[4, 4, 4, 4].Value = "มิเตอร์";
            ws.Cells[4, 4, 4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[3, 5, 3, 5].Value = "ราคา";
            ws.Cells[3, 5, 3, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[4, 5, 4, 5].Value = "ต่อลิตร";
            ws.Cells[4, 5, 4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[3, 6, 4, 6].Merge = true;
            ws.Cells[3, 6, 4, 6].Value = "จำนวนเงิน";
            ws.Cells[3, 6, 4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[3, 6, 4, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            ws.Cells[3, 1, 4, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[3, 1, 4, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[3, 1, 4, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[3, 1, 4, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[3, 1, 4, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[3, 1, 4, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            colHeaderRight++;

            //right
            ws.Cells[3, colHeaderRight, 3, colHeaderRight + groupheaderColRight.Count - 1].Merge = true;
            ws.Cells[3, colHeaderRight, 3, colHeaderRight + groupheaderColRight.Count - 1].Value = $"วันและเวลาที่พิมพ์ : {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            ws.Cells[3, colHeaderRight, 4, colHeaderRight + groupheaderColRight.Count - 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[3, colHeaderRight, 4, colHeaderRight + groupheaderColRight.Count - 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[3, colHeaderRight, 4, colHeaderRight + groupheaderColRight.Count - 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[3, colHeaderRight, 4, colHeaderRight + groupheaderColRight.Count - 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[3, colHeaderRight, 4, colHeaderRight + groupheaderColRight.Count - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[3, colHeaderRight, 4, colHeaderRight + groupheaderColRight.Count - 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            foreach (var hr in groupheaderColRight) 
            {
                ws.Cells[4, colHeaderRight, 4, colHeaderRight].Value = $"{hr}";
                ws.Cells[4, colHeaderRight, 4, colHeaderRight].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                colHeaderRight++;
            }

            #endregion

            #region Body left
            var listSaleAmtPos = new List<SaleAmtByDisp>();

            var listDisp = new List<DispOfSummerySale>();
            foreach (var item in dopMeter)
            {
                var disp = new DispOfSummerySale()
                {
                    DispName = $"{item.DispId} ({item.PdName})",
                    MeterStart = item.MeterStart ?? 0,
                    MeterFinish = item.MeterFinish ?? 0,
                    TotalQty = item.TotalQty ?? 0,
                    UnitPrice = item.Unitprice ?? 0,
                    TotalAmt = item.SaleAmt ?? 0, //listSaleAmtPos.Count > 0 ? saleAmt : (item.meterFinish - item.meterStart) * unitPrice,
                };
                listDisp.Add(disp);
            }


            var rowIndex = 5;
            foreach (var item in listDisp) 
            {
                var colIndex = 1;
                foreach (var col in item.GetType().GetProperties()) 
                {
                    var row = ws.Cells[rowIndex, colIndex, rowIndex, colIndex];
                    if (colIndex > 1) 
                    {
                        row.Style.Numberformat.Format = "#,##0.00";
                    }
                    row.Value = col.GetValue(item);
                    colIndex++;
                }
                rowIndex++;
            }
            #endregion

            #region Body Right

            #region data in tanksum (issueQty, SumQty, withdrawQty)
            var bodyRightSummarySale = new List<BodyRightSummarySale>();
            var listIssue = new List<string>();
            var listSumQty = new List<string>();
            var listWithdrawQty = new List<string>();

            var groupTankSum = tankSum.GroupBy(x => x.PdId).Select(x => new 
            { 
                issueQty = x.Sum(y => y.IssueQty),
                withdrawQty = x.Sum(y => y.WithdrawQty)
            }).ToList();

            listIssue.Add("ผลต่างจากมิเตอร์");
            listSumQty.Add("ปริมาณรวม");
            listWithdrawQty.Add("(หัก)เบิกใช้ในกิจการ");
            foreach (var item in groupTankSum) 
            {
                listIssue.Add(item.issueQty.ToString());
                listSumQty.Add(item.issueQty.ToString());
                listWithdrawQty.Add(item.withdrawQty.ToString());
            }
            listIssue.Add(groupTankSum.Sum(x => x.issueQty).ToString());
            listSumQty.Add(groupTankSum.Sum(x => x.issueQty).ToString());
            listWithdrawQty.Add(groupTankSum.Sum(x => x.withdrawQty).ToString());


            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listIssue });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listSumQty });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listWithdrawQty });
            #endregion

            #region data in meter (repairQty, testQty)
            var listRepairQty = new List<string>();
            var listTestQty = new List<string>();

            var groupMeter = dopMeter.GroupBy(x => x.PdId).Select(x => new
            {
                repairQty = x.Sum(y => y.RepairQty),
                testQty = x.Sum(y => y.TestQty)
            }).ToList();

            listRepairQty.Add("(หัก)ซ่อม");
            listTestQty.Add("(หัก)ทดสอบ/เทคืน");
            foreach (var item in groupMeter)
            {
                listRepairQty.Add(item.repairQty.ToString());
                listTestQty.Add(item.testQty.ToString());
            }
            listRepairQty.Add(groupMeter.Sum(x => x.repairQty).ToString());
            listTestQty.Add(groupMeter.Sum(x => x.testQty).ToString());


            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listRepairQty });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listTestQty });
            #endregion

            #region data in cash (cashAmt, creditAmt)
            var listCashQty = new List<string>() { "ปริมาณขายสด" };
            var listCreditQty = new List<string>() { "ปริมาณขายเชื่อ" };
            var listMeterQty = new List<string>() { "ปริมาณขายสุทธิ" };
            var listUnitPrice = new List<string>() { "ราคาขาย/ลิตร" };
            var listSaleAmt = new List<string>() { "จำนวนเงินขาย" };
            var listCreditAmt = new List<string>() { "(หัก)ขายเชื่อ" };
            var listDiscAmt = new List<string>() { "(หัก)ส่วนลดเงินสด" };
            var listTotalAmt = new List<string>() { "ยอดเงินสด" };

            var groupCash = dopCash.GroupBy(x => x.PdId).Select(x => new
            {
                meterQty = x.Sum(y => y.MeterAmt),                
                creditQty = Math.Round(x.Sum(y => y.CreditAmt/y.UnitPrice)??0,2),
                cashQty = Math.Round(x.Sum(y => y.MeterAmt - (y.CreditAmt / y.UnitPrice))??0,2),
                creditAmt = x.Sum(y => y.CreditAmt),                
                cashAmt = x.Sum(y => y.CashAmt),                
                totalAmt = x.Sum(y => y.TotalAmt),
                unitPrice = x.First().UnitPrice,
                saleAmt = x.Sum(y => y.SaleAmt),
                discAmt = x.Sum(y => y.DiscAmt)
            }).ToList();

            foreach (var item in groupCash)
            {
                listCashQty.Add((item.cashQty).ToString());
                listCreditQty.Add(item.creditQty.ToString());
                listMeterQty.Add(item.meterQty.ToString());

                listUnitPrice.Add(item.unitPrice.ToString());
                listSaleAmt.Add(item.saleAmt.ToString());
                listCreditAmt.Add(item.creditAmt.ToString());                                                
                listDiscAmt.Add(item.discAmt.ToString());                
                listTotalAmt.Add(item.totalAmt.ToString());
            }

            listCashQty.Add(groupCash.Sum(x => x.cashQty).ToString());
            listCreditQty.Add(groupCash.Sum(x => x.creditQty).ToString());
            listMeterQty.Add(groupCash.Sum(x => x.meterQty).ToString());

            listSaleAmt.Add(groupCash.Sum(x => x.saleAmt).ToString());
            listCreditAmt.Add(groupCash.Sum(x => x.creditAmt).ToString());                       
            listDiscAmt.Add(groupCash.Sum(x => x.discAmt).ToString());
            listTotalAmt.Add(groupCash.Sum(x => x.totalAmt).ToString());

            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listCashQty });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listCreditQty });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listMeterQty });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listUnitPrice });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listSaleAmt });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listCreditAmt });            
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listDiscAmt });
            bodyRightSummarySale.Add(new BodyRightSummarySale() { Body = listTotalAmt });
            #endregion

            #region cashsum and cashcrdr
            colHeaderRight = 6;
            var finalSummarySale = new List<FinalSummarySale>();

            var groupCashGl = cashGl.Where(x => x.GlAmt > 0).GroupBy(x => x.GlNo).Select(x => new SumGl
            {
                GlType = x.First().GlType,
                GlDesc = x.First().GlDesc,
                GlAmt = x.Sum(y => y.GlAmt.Value)
            }).ToList();

            var listCr = groupCashGl.Where(x => x.GlType == "CR").ToList();
            var footerCr = new FooterSumGl() { Remark = "รวมรายการรับเงินสดเพิ่ม", SumGlAmt = listCr.Sum(x => x.GlAmt)};

            var listDr = groupCashGl.Where(x => x.GlType == "DR").ToList();
            var footerDr = new FooterSumGl() { Remark = "รวมรายการหักเงินสด", SumGlAmt = listDr.Sum(x => x.GlAmt) };

            finalSummarySale.Add(new FinalSummarySale() { Header = "(+) รายการเงินสดรับเพิ่ม", Body = listCr, Footer = footerCr });
            finalSummarySale.Add(new FinalSummarySale() { Header = "(-) รายการหักเงินสด", Body = listDr, Footer = footerDr });
            #endregion

            var netCashBanlance = cashSum.GroupBy(x => x.PeriodNo).Select(x => new
            {
                totalAmt = x.Sum(y => y.SumTotalAmt),
                CrAmt = x.Sum(y => y.SumCrAmt),
                DrAmt = x.Sum(y => y.SumDrAmt)
            }).ToList();

            #region total dr cr
            var totalSumCrDr = new FooterSumGl() { Remark = "รวมยอดเงินสดสุทธิประจำวัน", SumGlAmt = netCashBanlance.Sum(x => x.totalAmt.Value) + netCashBanlance.Sum(x => x.CrAmt.Value) - netCashBanlance.Sum(x => x.DrAmt.Value) };
            //var totalSumCrDr = new FooterSumGl() { Remark = "รวมยอดเงินสดสุทธิประจำวัน", SumGlAmt = (listCr.Sum(x => x.GlAmt) - listDr.Sum(x => x.GlAmt)) };
            #endregion

            #region bind data group by product
            var rowIndexRight = 5;
            foreach (var item in bodyRightSummarySale)
            {
                var colIndex = 7;
                foreach (var col in item.Body)
                {
                    var row = ws.Cells[rowIndexRight, colIndex, rowIndexRight, colIndex];
                    if (colIndex > 7)
                    {
                        row.Style.Numberformat.Format = "#,##0.00";
                        row.Value = Convert.ToDecimal(col);
                    }
                    else 
                    {
                        row.Value = col;
                    }
                    colIndex++;
                }
                rowIndexRight++;
            }
            #endregion

            #region bind data summary
            foreach (var item in finalSummarySale)
            {
                var colIndex = 7;

                var rowHd = ws.Cells[rowIndexRight, colIndex, rowIndexRight, colHeaderRight + groupheaderColRight.Count];
                rowHd.Merge = true;
                rowHd.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rowHd.Value = item.Header;

                rowIndexRight++;

                foreach (var data in item.Body)
                {
                    colIndex = 7;
                    foreach (var col in data.GetType().GetProperties())
                    {
                        var row = ws.Cells[rowIndexRight, colIndex, rowIndexRight, colIndex];
                        if (col.Name != "GlType")
                        {
                            if (colIndex > 7)
                            {
                                row.Style.Numberformat.Format = "#,##0.00";
                                row.Value = Convert.ToDecimal(col.GetValue(data));
                            }
                            else
                            {
                                row.Value = $"  {col.GetValue(data)}";
                            }
                            colIndex = colHeaderRight + groupheaderColRight.Count;
                        }
                    }
                    rowIndexRight++;
                }

                colIndex = 7;

                foreach (var footer in item.Footer.GetType().GetProperties()) 
                {
                    var row = ws.Cells[rowIndexRight, colIndex, rowIndexRight, colIndex];
                    if (colIndex > 7)
                    {
                        row.Style.Numberformat.Format = "#,##0.00";
                        row.Value = Convert.ToDecimal(footer.GetValue(item.Footer));
                    }
                    else
                    {
                        row.Value = footer.GetValue(item.Footer);
                    }
                    colIndex = colHeaderRight + groupheaderColRight.Count;
                }
                rowIndexRight++;
            }
            #endregion

            #region total cr dr
            var colIndexTotal = 7;

            var rowHdTotalCrDr = ws.Cells[rowIndexRight, colIndexTotal, rowIndexRight, colHeaderRight + groupheaderColRight.Count];
            rowHdTotalCrDr.Style.Border.Top.Style = ExcelBorderStyle.Thin;

            foreach (var footer in totalSumCrDr.GetType().GetProperties())
            {
                var row = ws.Cells[rowIndexRight, colIndexTotal, rowIndexRight, colIndexTotal];
                if (colIndexTotal > 7)
                {
                    row.Style.Numberformat.Format = "#,##0.00";
                    row.Value = Convert.ToDecimal(footer.GetValue(totalSumCrDr));
                }
                else
                {
                    row.Value = footer.GetValue(totalSumCrDr);
                }
                colIndexTotal = colHeaderRight + groupheaderColRight.Count;
            }
            rowIndexRight++;
            #endregion

            #region bind final summary
            rowIndexRight += 5;
            var rowStartFinalSummary = rowIndexRight;

            //final summary right
            var listFinalSumCash = new List<FinalSummary>();

            var groupCashSum = cashSum.GroupBy(x => x.PeriodNo).Select(x => new
            {
                cashAmt = x.Sum(y => y.CashAmt),
                dipositAmt = x.Sum(y => y.DepositAmt),
                diffAmt = x.Sum(y => y.DiffAmt)
            }).ToList();

            listFinalSumCash.Add(new FinalSummary() { Remark = "ยอดเงินตามบัญชี", Amt = groupCashSum.Sum(x => x.cashAmt.Value) });
            listFinalSumCash.Add(new FinalSummary() { Remark = "ยอดเงินขาด/เกิน", Amt = groupCashSum.Sum(x => x.diffAmt.Value) });
            listFinalSumCash.Add(new FinalSummary() { Remark = "ยอดเงินนำฝากธนาคาร", Amt = groupCashSum.Sum(x => x.cashAmt.Value + x.diffAmt.Value) });


            foreach (var item in listFinalSumCash) 
            {
                var colIndex = 7;
                foreach (var data in item.GetType().GetProperties())
                {
                    var row = ws.Cells[rowIndexRight, colIndex, rowIndexRight, colIndex];
                    if (colIndex > 7)
                    {
                        row.Style.Numberformat.Format = "#,##0.00";
                        row.Value = Convert.ToDecimal(data.GetValue(item));
                    }
                    else
                    {
                        row.Value = data.GetValue(item);
                    }
                    colIndex = colHeaderRight + groupheaderColRight.Count;
                }
                rowIndexRight++;
            }

            var groupEmp = emp.GroupBy(x => x.PeriodNo).Select(x => new
            {
                periodNo = x.First().PeriodNo,
                list = x.ToList()
            }).ToList();

            var listUnionEmp = new List<string>();

            foreach (var curEmp in groupEmp) 
            {
                var unionEmp = listUnionEmp.Union(curEmp.list.Select(x => x.EmpName)).ToList();
                listUnionEmp = unionEmp;
            }

            //final summary left
            ws.Cells[rowIndexRight - 2, 1, rowIndexRight - 2, 1].Value = "จำนวนรวม";

            ws.Cells[rowIndexRight - 2, 4, rowIndexRight - 2, 4].Value = listDisp.Sum(x => x.TotalQty);
            ws.Cells[rowIndexRight - 2, 4, rowIndexRight - 2, 4].Style.Numberformat.Format = "#,##0.00";

            ws.Cells[rowIndexRight - 2, 6, rowIndexRight - 2, 6].Value = listDisp.Sum(x => x.TotalAmt);
            ws.Cells[rowIndexRight - 2, 6, rowIndexRight - 2, 6].Style.Numberformat.Format = "#,##0.00";


            ws.Cells[rowIndexRight - 1, 1, rowIndexRight - 1, 1].Value = "แคชเชียร์ :";
            ws.Cells[rowIndexRight - 1, 2, rowIndexRight - 1, 6].Merge = true;
            ws.Cells[rowIndexRight - 1, 2, rowIndexRight - 1, 6].Value = String.Join(", ", listUnionEmp);

            ws.Cells[rowStartFinalSummary, 1, rowIndexRight - 1, colHeaderRight + groupheaderColRight.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowStartFinalSummary, 1, rowIndexRight - 1, colHeaderRight + groupheaderColRight.Count].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#C6E0B4"));


            #endregion

            #endregion

            return new MemoryStream(pck.GetAsByteArray());
        }

        public ResponseSummarySaleForPDF GetReportSummarySalePDF(ReportSummarySaleRequest req)
        {
            var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);



            #region get all data
            var queryMeter = context.DopPeriodMeters.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryTankSum = context.DopPeriodTankSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryCash = context.DopPeriodCashes.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryCashSum = context.DopPeriodCashSums.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryCashGl = context.DopPeriodCashGls.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);
            var queryEmp = context.DopPeriodEmps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode && x.DocDate == docDate);

            if (req.PeriodNo != 0)
            {
                queryMeter = queryMeter.Where(x => x.PeriodNo == req.PeriodNo);
                queryTankSum = queryTankSum.Where(x => x.PeriodNo == req.PeriodNo);
                queryCash = queryCash.Where(x => x.PeriodNo == req.PeriodNo);
                queryCashSum = queryCashSum.Where(x => x.PeriodNo == req.PeriodNo);
                queryCashGl = queryCashGl.Where(x => x.PeriodNo == req.PeriodNo);
            }

            var dopMeter = queryMeter.OrderBy(x => x.DispId).ThenBy(x => x.PeriodNo).AsNoTracking().ToList();
            var tankSum = queryTankSum.OrderBy(x => x.PdId).ThenBy(x => x.PeriodNo).AsNoTracking().ToList();
            var cash = queryCash.OrderBy(x => x.PdId).ThenBy(x => x.PeriodNo).AsNoTracking().ToList();
            var dopCashSum = queryCashSum.OrderBy(x => x.PeriodNo).AsNoTracking().ToList();
            var cashGl = queryCashGl.OrderBy(x => x.PeriodNo).AsNoTracking().ToList();
            var emp = queryEmp.OrderBy(x => x.PeriodNo).AsNoTracking().ToList();

            //for get host id
            var meter = _mapper.Map<List<DopPeriodMeter>, List<MasBranchDisp>>(dopMeter);
            var queryMas = context.MasBranchDisps.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).OrderBy(x => x.DispId).AsNoTracking().ToList();
            meter.ForEach(x =>
            {
                x.HoseId = queryMas.Where(y => y.DispId == x.DispId).Select(y => y.HoseId).FirstOrDefault();
            });
            #endregion

            #region Body left
            var groupMeterDisp = meter.GroupBy(x => x.DispId).Select(x => new
            {
                dispId = x.First().DispId,
                hostId = x.First().HoseId,
                pdId = x.First().PdId,
                pdName = x.First().PdName,
                meterStart = x.First().MeterStart,
                meterFinish = x.Last().MeterFinish,
                saleAmt = x.First().SaleAmt,
            }).ToList();

            var listSaleAmtPos = new List<SaleAmtByDisp>();
            if (!groupMeterDisp.Any(x => x.hostId == null))
            {
                //listSaleAmtPos = GetSaleAmt(req.BrnCode, req.DocDate, req.PeriodNo);
            }

            var listDisp = new List<DispOfSummerySaleForPDF>();
            foreach (var item in groupMeterDisp)
            {
                var saleAmt = 0m;
                if (listSaleAmtPos.Count > 0)
                {
                    saleAmt = listSaleAmtPos.Where(x => x.HostId == item.hostId).FirstOrDefault().SaleAmt;
                }

                var unitPrice = cash.Where(x => x.PdId == item.pdId).LastOrDefault().UnitPrice.Value;
                var disp = new DispOfSummerySaleForPDF()
                {
                    dispName = $"{item.dispId} ({item.pdName})",
                    meterStart = item.meterStart,
                    meterFinish = item.meterFinish,
                    totalQty = item.meterFinish - item.meterStart,
                    unitPrice = unitPrice,
                    totalAmt = item.saleAmt, //listSaleAmtPos.Count > 0 ? saleAmt : (item.meterFinish - item.meterStart) * unitPrice,
                };
                listDisp.Add(disp);
            }
            #endregion
            
            #region Body Right

            #region data in tanksum (issueQty, SumQty, withdrawQty)
            var listRightSummarySale = new List<BodyRightItemForPDF>();
            var listIssue = new BodyRightItemForPDF();
            var listSumQty = new BodyRightItemForPDF();
            var listWithdrawQty = new BodyRightItemForPDF();

            var groupTankSum = tankSum.GroupBy(x => x.PdId).Select(x => new
            {
                pdId = x.First().PdId,
                issueQty = x.Sum(y => y.IssueQty),
                withdrawQty = x.Sum(y => y.WithdrawQty)
            }).ToList();

            listIssue.label = "ผลต่างจากมิเตอร์";
            listSumQty.label = "ปริมาณรวม";
            listWithdrawQty.label = "(หัก)เบิกใช้ในกิจการ";

            foreach (var item in groupTankSum)
            {
                switch (item.pdId)
                {
                    case "000001": //000001:ดีเซล B7
                        listIssue.dieselB7 = item.issueQty.Value;
                        listSumQty.dieselB7 = item.issueQty.Value;
                        listWithdrawQty.dieselB7 = item.withdrawQty.Value;
                        break;     
                    case "000002"://000002:เบนซิน
                        listIssue.benzine = item.issueQty.Value;
                        listSumQty.benzine = item.issueQty.Value;
                        listWithdrawQty.benzine = item.withdrawQty.Value;
                        break;
                    case "000004"://000004:เบนซิน 91
                        listIssue.benzine91 = item.issueQty.Value;
                        listSumQty.benzine91 = item.issueQty.Value;
                        listWithdrawQty.benzine91 = item.withdrawQty.Value;
                        break;
                    case "000005"://000005:แก๊สโซฮอล์ 95
                        listIssue.gasohol95 = item.issueQty.Value;
                        listSumQty.gasohol95 = item.issueQty.Value;
                        listWithdrawQty.gasohol95 = item.withdrawQty.Value;
                        break;
                    case "000006"://000006:แก๊สโซฮอล์ 91
                        listIssue.gasohol91 = item.issueQty.Value;
                        listSumQty.gasohol91 = item.issueQty.Value;
                        listWithdrawQty.gasohol91 = item.withdrawQty.Value;
                        break;
                    case "000010"://000010:แก๊สโซฮอล์ E20
                        listIssue.gasoholE20 = item.issueQty.Value;
                        listSumQty.gasoholE20 = item.issueQty.Value;
                        listWithdrawQty.gasoholE20 = item.withdrawQty.Value;
                        break;
                    case "000011"://000011:แก๊ส LPG
                        listIssue.gasLPG = item.issueQty.Value;
                        listSumQty.gasLPG = item.issueQty.Value;
                        listWithdrawQty.gasLPG = item.withdrawQty.Value;
                        break;
                    case "000073"://000073:ดีเซล B20
                        listIssue.dieselB20 = item.issueQty.Value;
                        listSumQty.dieselB20 = item.issueQty.Value;
                        listWithdrawQty.dieselB20 = item.withdrawQty.Value;
                        break;
                    case "000074"://000074:ดีเซล
                        listIssue.diesel = item.issueQty.Value;
                        listSumQty.diesel = item.issueQty.Value;
                        listWithdrawQty.diesel = item.withdrawQty.Value;
                        break;
                }
            }
            listIssue.sumAllProduct = groupTankSum.Sum(x => x.issueQty).Value;
            listSumQty.sumAllProduct = groupTankSum.Sum(x => x.issueQty).Value;
            listWithdrawQty.sumAllProduct = groupTankSum.Sum(x => x.withdrawQty).Value;


            listRightSummarySale.Add(listIssue);
            listRightSummarySale.Add(listSumQty);
            listRightSummarySale.Add(listWithdrawQty);
            #endregion

            #region data in meter (repairQty, testQty)
            var listRepairQty = new BodyRightItemForPDF();
            var listTestQty = new BodyRightItemForPDF();

            var groupMeter = meter.GroupBy(x => x.PdId).Select(x => new
            {
                pdId = x.First().PdId,
                repairQty = x.Sum(y => y.RepairQty),
                testQty = x.Sum(y => y.TestQty)
            }).ToList();

            listRepairQty.label = "(หัก)ซ่อม";
            listTestQty.label = "(หัก)ทดสอบ/เทคืน";
            foreach (var item in groupMeter)
            {
                switch (item.pdId)
                {
                    case "000001":
                        listRepairQty.dieselB7 = item.repairQty;
                        listTestQty.dieselB7 = item.testQty;
                        break;
                    case "000002":
                        listRepairQty.benzine = item.repairQty;
                        listTestQty.benzine = item.testQty;
                        break;
                    case "000004":
                        listRepairQty.benzine91 = item.repairQty;
                        listTestQty.benzine91 = item.testQty;
                        break;
                    case "000005":
                        listRepairQty.gasohol95 = item.repairQty;
                        listTestQty.gasohol95 = item.testQty;
                        break;
                    case "000006":
                        listRepairQty.gasohol91 = item.repairQty;
                        listTestQty.gasohol91 = item.testQty;
                        break;
                    case "000010":
                        listRepairQty.gasoholE20 = item.repairQty;
                        listTestQty.gasoholE20 = item.testQty;
                        break;
                    case "000011":
                        listRepairQty.gasLPG = item.repairQty;
                        listTestQty.gasLPG = item.testQty;
                        break;
                    case "000073":
                        listRepairQty.dieselB20 = item.repairQty;
                        listTestQty.dieselB20 = item.testQty;
                        break;
                    case "000074":
                        listRepairQty.diesel = item.repairQty;
                        listTestQty.diesel = item.testQty;
                        break;
                }
            }
            listRepairQty.sumAllProduct = groupMeter.Sum(x => x.repairQty);
            listTestQty.sumAllProduct = groupMeter.Sum(x => x.testQty);


            listRightSummarySale.Add(listRepairQty);
            listRightSummarySale.Add(listTestQty);
            #endregion

            #region data in cash (cashAmt, creditAmt)
            var listCashQty = new BodyRightItemForPDF() { label = "ปริมาณขายสด" };
            var listCreditQty = new BodyRightItemForPDF() { label = "ปริมาณขายเชื่อ" };
            var listTotalQty = new BodyRightItemForPDF() { label = "ปริมาณขายสุทธิ" };

            var listUnitPrice = new BodyRightItemForPDF() { label = "ราคาขาย/ลิตร" };
            var listSaleAmt = new BodyRightItemForPDF() { label = "จำนวนเงินขาย" };
            var listCreditAmt = new BodyRightItemForPDF() { label = "(หัก)ขายเชื่อ" };
            var listDiscAmt = new BodyRightItemForPDF() { label = "(หัก)ส่วนลดเงินสด" };
            var listTotalAmt = new BodyRightItemForPDF() { label = "ยอดเงินสด" };

            var groupCash = cash.GroupBy(x => x.PdId).Select(x => new
            {
                pdId = x.First().PdId,
                cashAmt = x.Sum(y => y.CashAmt) ?? 0,
                creditAmt = x.Sum(y => y.CreditAmt) ?? 0,
                totalAmt = x.Sum(y => y.TotalAmt) ?? 0,
                unitPrice = x.First().UnitPrice ?? 0,
                saleAmt = x.Sum(y => y.SaleAmt) ?? 0,
                discAmt = x.Sum(y => y.DiscAmt) ?? 0,

                meterQty = x.Sum(y => y.MeterAmt) ?? 0,
                creditQty = Math.Round(x.Sum(y => y.CreditAmt / y.UnitPrice) ?? 0, 2),
                cashQty = Math.Round(x.Sum(y => y.MeterAmt - (y.CreditAmt / y.UnitPrice)) ?? 0, 2),
            }).ToList();

            //listCashAmt.label = "ปริมาณขายสด";
            //ListCreditAmt.label = "ปริมาณขายเชื่อ";
            //ListTotalAmt.label = "ปริมาณขายสุทธิ";
            //listUnitPrice.label = "ราคาขาย / ลิตร";
            //listSaleAmt.label = "จำนวนเงินขาย";
            //listDiscAmt.label = "(หัก)ส่วนลดเงินสด";
            foreach (var item in groupCash)
            {
                switch (item.pdId)
                {
                    case "000001":
                        listCashQty.dieselB7 = item.cashQty;
                        listCreditQty.dieselB7 = item.creditQty;
                        listTotalQty.dieselB7 = item.meterQty;
                        listUnitPrice.dieselB7 = item.unitPrice;
                        listSaleAmt.dieselB7 = item.saleAmt;
                        listCreditAmt.dieselB7 = item.creditAmt;
                        listDiscAmt.dieselB7 = item.discAmt;
                        listTotalAmt.dieselB7 = item.totalAmt;
                        break;
                    case "000002":
                        listCashQty.benzine = item.cashQty;
                        listCreditQty.benzine = item.creditQty;
                        listTotalQty.benzine = item.meterQty;
                        listUnitPrice.benzine = item.unitPrice;
                        listSaleAmt.benzine = item.saleAmt;
                        listCreditAmt.benzine = item.creditAmt;
                        listDiscAmt.benzine = item.discAmt;
                        listTotalAmt.benzine = item.totalAmt;
                        break;
                    case "000004":
                        listCashQty.benzine91 = item.cashQty;
                        listCreditQty.benzine91 = item.creditQty;
                        listTotalQty.benzine91 = item.meterQty;
                        listUnitPrice.benzine91 = item.unitPrice;
                        listSaleAmt.benzine91 = item.saleAmt;
                        listCreditAmt.benzine91 = item.creditAmt;
                        listDiscAmt.benzine91 = item.discAmt;
                        listTotalAmt.benzine91 = item.totalAmt;
                        break;
                    case "000005":
                        listCashQty.gasohol95 = item.cashQty;
                        listCreditQty.gasohol95 = item.creditQty;
                        listTotalQty.gasohol95 = item.meterQty;
                        listUnitPrice.gasohol95 = item.unitPrice;
                        listSaleAmt.gasohol95 = item.saleAmt;
                        listCreditAmt.gasohol95 = item.creditAmt;
                        listDiscAmt.gasohol95 = item.discAmt;
                        listTotalAmt.gasohol95 = item.totalAmt;
                        break;
                    case "000006":
                        listCashQty.gasohol91 = item.cashQty;
                        listCreditQty.gasohol91 = item.creditQty;
                        listTotalQty.gasohol91 = item.meterQty;
                        listUnitPrice.gasohol91 = item.unitPrice;
                        listSaleAmt.gasohol91 = item.saleAmt;
                        listCreditAmt.gasohol91 = item.creditAmt;
                        listDiscAmt.gasohol91 = item.discAmt;
                        listTotalAmt.gasohol91 = item.totalAmt;
                        break;
                    case "000010":
                        listCashQty.gasoholE20 = item.cashQty;
                        listCreditQty.gasoholE20 = item.creditQty;
                        listTotalQty.gasoholE20 = item.meterQty;
                        listUnitPrice.gasoholE20 = item.unitPrice;
                        listSaleAmt.gasoholE20 = item.saleAmt;
                        listCreditAmt.gasoholE20 = item.creditAmt;
                        listDiscAmt.gasoholE20 = item.discAmt;
                        listTotalAmt.gasoholE20 = item.totalAmt;
                        break;
                    case "000011":
                        listCashQty.gasLPG = item.cashQty;
                        listCreditQty.gasLPG = item.creditQty;
                        listTotalQty.gasLPG = item.meterQty;
                        listUnitPrice.gasLPG = item.unitPrice;
                        listSaleAmt.gasLPG = item.saleAmt;
                        listCreditAmt.gasLPG = item.creditAmt;
                        listDiscAmt.gasLPG = item.discAmt;
                        listTotalAmt.gasLPG = item.totalAmt;
                        break;
                    case "000073":
                        listCashQty.dieselB20 = item.cashQty;
                        listCreditQty.dieselB20 = item.creditQty;
                        listTotalQty.dieselB20 = item.meterQty;
                        listUnitPrice.dieselB20 = item.unitPrice;
                        listSaleAmt.dieselB20 = item.saleAmt;
                        listCreditAmt.dieselB20 = item.creditAmt;
                        listDiscAmt.dieselB20 = item.discAmt;
                        listTotalAmt.dieselB20 = item.totalAmt;
                        break;
                    case "000074":
                        listCashQty.diesel = item.cashQty;
                        listCreditQty.diesel = item.creditQty;
                        listTotalQty.diesel = item.meterQty;
                        listUnitPrice.diesel = item.unitPrice;
                        listSaleAmt.diesel = item.saleAmt;
                        listCreditAmt.diesel = item.creditAmt;
                        listDiscAmt.diesel = item.discAmt;
                        listTotalAmt.diesel = item.totalAmt;
                        break;
                }
            }

            listCashQty.sumAllProduct = groupCash.Sum(x => x.cashQty);
            listCreditQty.sumAllProduct = groupCash.Sum(x => x.creditQty);
            listTotalQty.sumAllProduct = groupCash.Sum(x => x.meterQty);

            listSaleAmt.sumAllProduct = groupCash.Sum(x => x.saleAmt);
            listCreditAmt.sumAllProduct = groupCash.Sum(x => x.creditAmt);
            listDiscAmt.sumAllProduct = groupCash.Sum(x => x.discAmt);
            listTotalAmt.sumAllProduct = groupCash.Sum(x => x.totalAmt);

            listRightSummarySale.Add(listCashQty);
            listRightSummarySale.Add(listCreditQty);
            listRightSummarySale.Add(listTotalQty);

            listRightSummarySale.Add(listUnitPrice);
            listRightSummarySale.Add(listSaleAmt);
            listRightSummarySale.Add(listCreditAmt);
            listRightSummarySale.Add(listDiscAmt);
            listRightSummarySale.Add(listTotalAmt);
            #endregion

            #region cashsum and cashcrdr
            var groupCashGl = cashGl.Where(x => x.GlAmt > 0).GroupBy(x => x.GlNo).Select(x => new SumGlForPDF
            {
                glType = x.First().GlType,
                glDesc = x.First().GlDesc,
                glAmt = x.Sum(y => y.GlAmt.Value)
            }).ToList();

            var listCr = groupCashGl.Where(x => x.glType == "CR").ToList();
            var listDr = groupCashGl.Where(x => x.glType == "DR").ToList();

            listCr.Add(new SumGlForPDF() { glDesc= "รวมรายการรับเงินสดเพิ่ม", glAmt = listCr.Sum(x=>x.glAmt)});
            listDr.Add(new SumGlForPDF() { glDesc = "รวมรายการหักเงินสด", glAmt = listDr.Sum(x => x.glAmt) });
            #endregion

            #region final summary
            // var listFinalSumCash = new List<FinalSummaryForPDF>();

            var groupCashSum = dopCashSum.GroupBy(x => x.PeriodNo).Select(x => new
            {
                cashAmt = x.Sum(y => y.CashAmt),
                dipositAmt = x.Sum(y => y.DepositAmt),
                diffAmt = x.Sum(y => y.DiffAmt)
            }).ToList();

            //listFinalSumCash.Add(new FinalSummaryForPDF() { remark = "ยอดเงินตามบัญชี", amt = groupCashSum.Sum(x => x.cashAmt.Value) });
            //listFinalSumCash.Add(new FinalSummaryForPDF() { remark = "ยอดเงินขาด/เกิน", amt = groupCashSum.Sum(x => x.diffAmt.Value) });
            //listFinalSumCash.Add(new FinalSummaryForPDF() { remark = "ยอดเงินนำฝากธนาคาร", amt = groupCashSum.Sum(x => x.cashAmt.Value + x.diffAmt.Value) });
            #endregion

            #region empName
            var groupEmp = emp.GroupBy(x => x.PeriodNo).Select(x => new
            {
                periodNo = x.First().PeriodNo,
                list = x.ToList()
            }).ToList();

            var listUnionEmp = new List<string>();

            foreach (var curEmp in groupEmp)
            {
                var unionEmp = listUnionEmp.Union(curEmp.list.Select(x => x.EmpName)).ToList();
                listUnionEmp = unionEmp;
            }
            #endregion

            #endregion

            #region header
            var header = new HeaderSummarySaleForPdf
            {
                brnCode = brn.BrnCode
            };
            List<HeaderSummarySaleForPdf> listHeaders = new List<HeaderSummarySaleForPdf>();
            listHeaders.Add(header);
            #endregion

            #region mapping data
            var response = new ResponseSummarySaleForPDF();
            response.brnCode = brn.BrnCode;
            response.brnName = brn.BrnName;
            response.compName = company.CompName;
            response.compImage = company.CompImage;
            response.docDate = docDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            response.empName = String.Join(", ", listUnionEmp);
            response.totalAmt = dopMeter.Sum(x => x.SaleAmt) ?? 0;
            response.totalQty = dopMeter.Sum(x => x.TotalQty) ?? 0;

            response.totalCashAmt = groupCashSum.Sum(x => x.cashAmt.Value);
            response.totalDiffAmt = groupCashSum.Sum(x => x.diffAmt.Value);
            response.totalDepositAmt = groupCashSum.Sum(x => x.cashAmt.Value + x.diffAmt.Value);

            response.headerSummarySale = listHeaders;
            response.bodyLeftItems = listDisp;
            response.bodyRightItems = listRightSummarySale;
            response.crItems = listCr;
            response.drItems = listDr;
            #endregion

            return response;
        }

        //private List<SaleAmtByDisp> GetSaleAmt(string brnCode, string docDate, int periodNo)
        //{
        //    var listResult = new List<SaleAmtByDisp>();
        //    OracleConnection con = new OracleConnection
        //    {
        //        ConnectionString = _connectionString
        //    };
        //    con.Open();

        //    OracleCommand cmd = new OracleCommand
        //    {
        //        Connection = con,
        //        CommandTimeout = 90
        //    };

        //    cmd.CommandText = $@"SELECT
	       //                         f2.hose_id,
	       //                         nvl(sum(f5.goods_amt + f5.tax_amt + f5.disc_amt), 0) AS sell_amt
        //                        FROM
	       //                         function4 f4
        //                        INNER JOIN function5 f5 
        //                        ON
	       //                         f4.journal_id = f5.journal_id
        //                        RIGHT JOIN function2 f2
        //                        ON
	       //                         f2.site_id = f4.site_id
	       //                         AND f2.business_date = f4.business_date
	       //                         AND f2.shift_no = f4.shift_no
	       //                         AND f2.hose_id = f5.hose_id
        //                        WHERE
	       //                         f2.site_id = '52{brnCode}'
	       //                         AND trunc(f2.business_date) = to_date('{docDate}', 'yyyy-MM-dd')";
        //    if (periodNo != 0) 
        //    {
        //        cmd.CommandText += $"AND f2.shift_no = {periodNo}";
        //    }
        //    cmd.CommandText += "GROUP BY f2.hose_id ORDER BY f2.hose_id";

        //    OracleDataReader oracleDataReader = cmd.ExecuteReader();
        //    while (oracleDataReader.Read())
        //    {
        //        var result = new SaleAmtByDisp()
        //        {
        //            HostId = oracleDataReader["HOSE_ID"] != DBNull.Value ? Convert.ToInt32(oracleDataReader["HOSE_ID"]) : 0,
        //            SaleAmt = oracleDataReader["SELL_AMT"] != DBNull.Value ? Convert.ToDecimal(oracleDataReader["SELL_AMT"]) : 0
        //        };
        //        listResult.Add(result);
        //    }

        //    con.Close();
        //    con.Dispose();
        //    return listResult;
        //}
    }
}
