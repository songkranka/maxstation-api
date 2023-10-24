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
    public class InventoryRepository : BaseRepositories, IInventoryRepository
    {
        private readonly IMapper _mapper;

        public InventoryRepository(MaxStation.Entities.Models.PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public List<InvReceiveProdResponse> GetReceiveProdHdPDF(InventoryRequest req)
        {
            List<InvReceiveProdResponse> response = new List<InvReceiveProdResponse>();

            var query = (from hd in this.Context.InvReceiveProdHds
                         join dt in this.Context.InvReceiveProdDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on dt.PdId equals pm.PdId
                         where hd.CompCode == req.CompCode
                         && hd.BrnCode == req.BrnCode
                         //&& hd.DocStatus != "Cancel"
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                         ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.GroupBy(x => new { x.hd.DocDate, x.hd.DocNo, x.hd.DocStatus, x.hd.SupCode, x.hd.SupName, x.hd.InvNo, x.hd.InvDate, x.hd.PoNo, x.hd.SubAmt })
                         .Select(x => new InvReceiveProdResponse
                         {
                             docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                             docNo = x.Key.DocNo,
                             docStatus = x.Key.DocStatus,
                             supCode = x.Key.SupCode,
                             supName = x.Key.SupName,
                             poNo = x.Key.PoNo,
                             invNo = x.Key.InvNo,
                             invDate = x.Key.InvDate.Value.ToString("yyyy-MM-dd"),
                             itemQty = x.Sum(s => s.dt.ItemQty) ?? 0,
                             itemRemain = x.Sum(s => s.dt.ItemRemain) ?? 0,
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public List<InvReceiveProdResponse> GetReceiveProdDtPDF(InventoryRequest req)
        {
            List<InvReceiveProdResponse> response = new List<InvReceiveProdResponse>();


            var query = (from hd in this.Context.InvReceiveProdHds
                         join dt in this.Context.InvReceiveProdDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on dt.PdId equals pm.PdId
                         where hd.CompCode == req.CompCode
                         && hd.BrnCode == req.BrnCode
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                         ).AsQueryable();
            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }
            response = query.Select(x => new InvReceiveProdResponse
            {
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.hd.DocNo,
                docStatus = x.hd.DocStatus,
                supCode = x.hd.SupCode,
                supName = x.hd.SupName,
                poNo = x.hd.PoNo,
                invNo = x.hd.InvNo,
                invDate = x.hd.InvDate.Value.ToString("yyyy-MM-dd"),
                vatType = x.hd.VatType,
                remark = x.hd.Remark,
                seqNo = x.dt.SeqNo,
                pdId = x.dt.PdId,
                pdName = x.dt.PdName,
                unitName = x.dt.UnitName,
                poQty = x.dt.PoQty ?? 0,
                itemQty = x.dt.ItemQty ?? 0,
                itemRemain = x.dt.ItemRemain ?? 0,
                subAmt = x.dt.SubAmt ?? 0,
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public MemoryStream GetReceiveProdHdExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = GetReceiveProdHdPDF(req);

            // title company
            var companyTitle = ws.Cells[1, 3, 1, 7];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var printDate = ws.Cells[1, 9, 1, 9];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // title report
            var reportTitle = ws.Cells[2, 3, 2, 7];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานสรุปการรับสินค้า";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // title date
            var dateTitle = ws.Cells[3, 3, 3, 7];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchTitle = ws.Cells[4, 1, 4, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";


            ws.Cells[5, 1, 5, 2].Value = $"ใบรับสินค้า";
            ws.Cells[5, 1, 5, 2].Merge = true;
            ws.Cells[5, 3, 5, 4].Value = $"เจ้าหนี้";
            ws.Cells[5, 3, 5, 4].Merge = true;
            ws.Cells[5, 5, 5, 5].Value = $"ใบกำกับภาษี";
            ws.Cells[5, 6, 5, 6].Value = $"วันที่ใบกำกับ";
            ws.Cells[5, 7, 5, 7].Value = $"ใบสั่งซื้อ";
            ws.Cells[5, 8, 5, 8].Value = $"จำนวนรับ";
            ws.Cells[5, 9, 5, 9].Value = $"จำนวนขาดส่ง";

            ws.Cells[5, 1, 5, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 9].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var dateRunning = req.DateFrom;
            var rowIndex = 7;
            while (dateRunning != req.DateTo.AddDays(1))
            {
                var selectItem = data.Where(x => Convert.ToDateTime(x.docDate) == dateRunning).ToList();
                if (selectItem.Count() > 0)
                {
                    ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่ใบรับสินค้า :";
                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"{dateRunning.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
                    rowIndex++;
                    foreach (var item in selectItem)
                    {
                        ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.docNo;
                        ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.docStatus == "Cancel" ? $"(ยกเลิก)" : "";
                        ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.supCode;
                        ws.Cells[rowIndex, 4, rowIndex, 4].Value = item.compName;
                        ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.invNo;
                        ws.Cells[rowIndex, 6, rowIndex, 6].Value = Convert.ToDateTime(item.invDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.poNo;
                        ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.itemQty;
                        ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.itemRemain;
                        rowIndex++;
                    }
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = $"รวม";
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = selectItem.Sum(x => x.itemQty);
                    ws.Cells[rowIndex, 9, rowIndex, 9].Value = selectItem.Sum(x => x.itemRemain);
                    ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                    rowIndex += 2;
                }
                dateRunning = dateRunning.AddDays(1);
            }
            rowIndex++;
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = $"รวมทั้งสิ้น";
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = data.Sum(x => x.itemQty);
            ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            ws.Cells[rowIndex, 9, rowIndex, 9].Value = data.Sum(x => x.itemRemain);
            ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            return new MemoryStream(pck.GetAsByteArray());
        }


        public MemoryStream GetReceiveProdDtExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetReceiveProdDtPDF(req);


            #region title
            // title company
            var companyTitle = ws.Cells[1, 3, 1, 7];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var printDate = ws.Cells[1, 8, 1, 8];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

            // title report
            var reportTitle = ws.Cells[2, 3, 2, 7];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานสรุปการรับสินค้า";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // title date
            var dateTitle = ws.Cells[3, 3, 3, 7];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var branchTitle = ws.Cells[4, 1, 4, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";
            #endregion



            ws.Cells[5, 1, 5, 1].Value = $"No.";
            ws.Cells[5, 2, 5, 2].Value = $"รหัสสินค้า";
            ws.Cells[5, 3, 5, 4].Value = $"ชื่อสินค้า";
            ws.Cells[5, 3, 5, 4].Merge = true;
            ws.Cells[5, 5, 5, 5].Value = $"หน่วยนับ";
            ws.Cells[5, 6, 5, 6].Value = $"จำนวนสั่งซื้อ";
            ws.Cells[5, 7, 5, 7].Value = $"จำนวนรับ";
            ws.Cells[5, 8, 5, 8].Value = $"จำนวนขาดส่ง";

            ws.Cells[5, 1, 5, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[5, 1, 5, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[5, 1, 5, 8].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            var rowIndex = 7;
            var docnos = data.GroupBy(x => x.docNo).Select(x => x.Key).ToList();
            foreach (var docno in docnos)
            {

                var header = data.FirstOrDefault(x => x.docNo == docno);
                var cell = ws.Cells[rowIndex, 1, rowIndex, 1];
                cell.Value = $"วันที่รับสินค้า :";
                cell = ws.Cells[rowIndex, 2, rowIndex, 2];
                cell.Value = $"{Convert.ToDateTime(header.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

                cell = ws.Cells[rowIndex, 4, rowIndex, 4];
                cell.Value = $"เลขที่ใบรับสินค้า :";
                cell = ws.Cells[rowIndex, 5, rowIndex, 5];
                var status = header.docStatus == "Cancel" ? $"(ยกเลิก)" : "";
                cell.Value = $"{header.docNo} {status}";

                rowIndex++;
                cell = ws.Cells[rowIndex, 1, rowIndex, 1];
                cell.Value = $"รหัสผู้จำหน่าย :";
                cell = ws.Cells[rowIndex, 2, rowIndex, 2];
                cell.Value = $"{header.supCode}";

                cell = ws.Cells[rowIndex, 4, rowIndex, 4];
                cell.Value = $"ชื่อผู้จำหน่าย :";
                cell = ws.Cells[rowIndex, 5, rowIndex, 5];
                cell.Value = $"{header.supName}";

                cell = ws.Cells[rowIndex, 7, rowIndex, 7];
                cell.Value = $"ใบสั่งซื้อ :";
                cell = ws.Cells[rowIndex, 8, rowIndex, 8];
                cell.Value = $"{header.poQty}";

                rowIndex++;
                cell = ws.Cells[rowIndex, 1, rowIndex, 1];
                cell.Value = $"เลขที่ใบกำกับ/ใบส่งของ :";
                cell = ws.Cells[rowIndex, 2, rowIndex, 2];
                cell.Value = $"{header.invNo}";

                cell = ws.Cells[rowIndex, 4, rowIndex, 4];
                cell.Value = $"วันที่ใบกำกับ :";
                cell = ws.Cells[rowIndex, 5, rowIndex, 5];
                cell.Value = $"{Convert.ToDateTime(header.invDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

                cell = ws.Cells[rowIndex, 7, rowIndex, 7];
                cell.Value = $"ประเภทใบกับกับ :";
                cell = ws.Cells[rowIndex, 8, rowIndex, 8];
                var type = header.vatType == "VI" ? "รวมภาษี" : (header.vatType == "VE" ? "ไม่รวมภาษี" : "ไม่มีภาษี");
                cell.Value = $"{type}";
                ws.Cells[rowIndex, 1, rowIndex, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;


                rowIndex++;
                int cnt = 0;
                var items = data.Where(x => x.docNo == docno).ToList();
                foreach (var item in items)
                {
                    ws.Cells[rowIndex, 1, rowIndex, 1].Value = ++cnt;
                    ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.pdId;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.pdName;
                    ws.Cells[rowIndex, 5, rowIndex, 5].Value = item.unitName;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.poQty;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.itemQty;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.itemRemain;
                    rowIndex++;
                }
                ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวม";
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = items.Sum(x => x.poQty);
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = items.Sum(x => x.itemQty);
                ws.Cells[rowIndex, 8, rowIndex, 8].Value = items.Sum(x => x.itemRemain);
                ws.Cells[rowIndex, 5, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                rowIndex += 2;
            }

            rowIndex++;
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมทั้งสิ้น";
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = data.Sum(x => x.poQty);
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Sum(x => x.itemQty);
            ws.Cells[rowIndex, 8, rowIndex, 8].Value = data.Sum(x => x.itemRemain);
            ws.Cells[rowIndex, 5, rowIndex, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            return new MemoryStream(pck.GetAsByteArray());
        }

        public List<InvTransferOutResponse> GetTransferOutHdPDF(InventoryRequest req)
        {
            List<InvTransferOutResponse> response = new List<InvTransferOutResponse>();

            var query = (from hd in this.Context.InvTranoutHds
                         join dt in this.Context.InvTranoutDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                         // && hd.DocStatus != "Cancel"
                         && hd.BrnCode == req.BrnCode
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                     ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.GroupBy(x => new { x.hd.DocDate, x.hd.DocNo, x.hd.DocStatus, x.hd.RefNo, x.hd.BrnCodeTo, x.hd.BrnNameTo }).Select(x => new InvTransferOutResponse
            {
                docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.Key.DocNo,
                docStatus = x.Key.DocStatus,
                refNo = x.Key.RefNo,
                brnCodeTo = x.Key.BrnCodeTo,
                brnNameTo = x.Key.BrnNameTo,
                itemQty = x.Sum(s => s.dt.ItemQty) ?? 0
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public List<InvTransferOutResponse> GetTransferOutDtPDF(InventoryRequest req)
        {
            List<InvTransferOutResponse> response = new List<InvTransferOutResponse>();

            var query = (from hd in this.Context.InvTranoutHds
                         join dt in this.Context.InvTranoutDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                        // && hd.DocStatus != "Cancel"
                        && hd.BrnCode == req.BrnCode
                        && hd.DocDate >= req.DateFrom
                        && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                     ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }
            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.Select(x => new InvTransferOutResponse
            {
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.hd.DocNo,
                docStatus = x.hd.DocStatus,
                refNo = x.hd.RefNo,
                brnCodeTo = x.hd.BrnCodeTo,
                brnNameTo = x.hd.BrnNameTo,
                seqNo = x.dt.SeqNo,
                pdId = x.dt.PdId,
                pdName = x.dt.PdName,
                unitName = x.dt.UnitName,
                itemQty = x.dt.ItemQty ?? 0
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
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }
        public List<InvTransferInResponse> GetTransferInHdPDF(InventoryRequest req)
        {
            List<InvTransferInResponse> response = new List<InvTransferInResponse>();

            var query = (from hd in this.Context.InvTraninHds
                         join dt in this.Context.InvTraninDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                        // && hd.DocStatus != "Cancel"
                        && hd.BrnCode == req.BrnCode
                        && hd.DocDate >= req.DateFrom
                        && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                     ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.GroupBy(x => new { x.hd.DocDate, x.hd.DocNo, x.hd.DocStatus, x.hd.RefNo, x.hd.BrnCodeFrom, x.hd.BrnNameFrom }).Select(x => new InvTransferInResponse
            {
                docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.Key.DocNo,
                docStatus = x.Key.DocStatus,
                refNo = x.Key.RefNo,
                brnCodeFrom = x.Key.BrnCodeFrom,
                brnNameFrom = x.Key.BrnNameFrom,
                itemQty = x.Sum(s => s.dt.ItemQty) ?? 0
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }
        public List<InvTransferInResponse> GetTransferInDtPDF(InventoryRequest req)
        {
            List<InvTransferInResponse> response = new List<InvTransferInResponse>();

            var query = (from hd in this.Context.InvTraninHds
                         join dt in this.Context.InvTraninDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                         // && hd.DocStatus != "Cancel"
                         && hd.BrnCode == req.BrnCode
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                     ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.Select(x => new InvTransferInResponse
            {
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.hd.DocNo,
                docStatus = x.hd.DocStatus,
                refNo = x.hd.RefNo,
                brnCodeFrom = x.hd.BrnCodeFrom,
                brnNameFrom = x.hd.BrnNameFrom,
                seqNo = x.dt.SeqNo,
                pdId = x.dt.PdId,
                pdName = x.dt.PdName,
                unitName = x.dt.UnitName,
                itemQty = x.dt.ItemQty ?? 0
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public List<InvWithdrawResponse> GetWithdrawHdPDF(InventoryRequest req)
        {
            List<InvWithdrawResponse> response = new List<InvWithdrawResponse>();

            var query = (from hd in this.Context.InvWithdrawHds
                         join dt in this.Context.InvWithdrawDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                         && hd.BrnCode == req.BrnCode
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.GroupBy(x => new { x.hd.DocNo, x.hd.DocStatus, x.hd.DocDate, x.hd.EmpCode, x.hd.EmpName, x.hd.UseBrnCode, x.hd.UseBrnName, x.hd.LicensePlate, x.hd.ReasonDesc, x.hd.Remark })
                                                .Select(x => new InvWithdrawResponse
                                                {
                                                    docNo = x.Key.DocNo,
                                                    docStatus = x.Key.DocStatus,
                                                    docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                                                    empCode = x.Key.EmpCode,
                                                    empName = x.Key.EmpName,
                                                    useBrnCode = x.Key.UseBrnCode,
                                                    useBrnName = x.Key.UseBrnName,
                                                    licensePlate = x.Key.LicensePlate,
                                                    reasonDesc = x.Key.ReasonDesc,
                                                    remark = x.Key.Remark,
                                                    totalQty = x.Sum(s => s.dt.ItemQty) ?? 0
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.brnAddress = branch.b.Address;
            });


            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public List<InvWithdrawResponse> GetWithdrawDtPDF(InventoryRequest req)
        {
            List<InvWithdrawResponse> response = new List<InvWithdrawResponse>();
            //InvWithdrawResponse response = new InvWithdrawResponse();

            var query = (from hd in this.Context.InvWithdrawHds
                         join dt in this.Context.InvWithdrawDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                         && hd.BrnCode == req.BrnCode
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.Select(x => new InvWithdrawResponse
            {
                docNo = x.hd.DocNo,
                docStatus = x.hd.DocStatus,
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                empCode = x.hd.EmpCode,
                empName = x.hd.EmpName,
                useBrnCode = x.hd.UseBrnCode,
                useBrnName = x.hd.UseBrnName ?? "",
                licensePlate = x.hd.LicensePlate ?? "",
                reasonDesc = x.hd.ReasonDesc,
                remark = x.hd.Remark ?? "",
                seqNo = x.dt.SeqNo,
                pdId = x.dt.PdId,
                pdName = x.dt.PdName,
                unitName = x.dt.UnitName,
                itemQty = x.dt.ItemQty ?? 0
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
                x.brnAddress = branch.b.Address;
            });


            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public List<InvReturnSupResponse> GetReturnSupHdPDF(InventoryRequest req)
        {
            List<InvReturnSupResponse> response = new List<InvReturnSupResponse>();

            var query = (from hd in this.Context.InvReturnSupHds
                         join dt in this.Context.InvReturnSupDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                         && hd.BrnCode == req.BrnCode
                         //&& hd.DocStatus != "Cancel"
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                         ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }
            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }
            response = query.GroupBy(x => new { x.hd.DocDate, x.hd.DocNo, x.hd.DocStatus, x.hd.SupCode, x.hd.SupName, x.hd.ReasonDesc }).Select(x => new InvReturnSupResponse
            {
                docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.Key.DocNo,
                docStatus = x.Key.DocStatus,
                supCode = x.Key.SupCode,
                supName = x.Key.SupName,
                reasonDesc = x.Key.ReasonDesc,
                itemQty = x.Sum(s => s.dt.ItemQty) ?? 0,
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public List<InvReturnSupResponse> GetReturnSupDtPDF(InventoryRequest req)
        {
            List<InvReturnSupResponse> response = new List<InvReturnSupResponse>();

            response = (from hd in this.Context.InvReturnSupHds
                        join dt in this.Context.InvReturnSupDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }

                        where hd.CompCode == req.CompCode
                        && hd.BrnCode == req.BrnCode
                        //&& hd.DocStatus != "Cancel"
                        && hd.DocDate >= req.DateFrom
                        && hd.DocDate <= req.DateTo
                        select new { hd, dt }
                         ).Select(x => new InvReturnSupResponse
                         {
                             docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                             docNo = x.hd.DocNo,
                             docStatus = x.hd.DocStatus,
                             supCode = x.hd.SupCode,
                             supName = x.hd.SupName,
                             reasonDesc = x.hd.ReasonDesc,
                             seqNo = x.dt.SeqNo,
                             pdId = x.dt.PdId,
                             pdName = x.dt.PdName,
                             unitName = x.dt.UnitName,
                             itemQty = x.dt.ItemQty ?? 0,
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }
        public MemoryStream GetReturnSupHdExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetReturnSupHdPDF(req);

            int rowIndex = 1;
            #region Title
            // Title Company
            var cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"{company.CompName}";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Merge = true;
            cells.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // Report Title
            cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = "รายงานสรปการคืนสินค้าให้ผู้จำหน่าย";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // Report Branch
            cells = ws.Cells[rowIndex, 1, rowIndex, 1];
            cells.Value = "สาขา";
            cells = ws.Cells[rowIndex, 2, rowIndex, 3];
            cells.Merge = true;
            cells.Value = $"{brn.BrnCode} - {brn.BrnName}";

            // Report Daterange            
            cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"ช่วงวันที่ : {req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            #endregion

            rowIndex += 2;
            #region Column header
            ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่";
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"เลขที่เอกสาร";
            ws.Cells[rowIndex, 3, rowIndex, 3].Value = $"สถานะ";
            ws.Cells[rowIndex, 4, rowIndex, 5].Value = $"ผู้จำหน่าย";
            ws.Cells[rowIndex, 4, rowIndex, 5].Merge = true;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"เหตุผลในการคืน";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = $"จำนวน";

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));
            #endregion

            rowIndex++;
            #region Items
            foreach (var item in data)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = Convert.ToDateTime(item.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = item.docNo;
                ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.docStatus == "Cancel" ? "ยกเลิก" : "";
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = $"{item.supCode} {item.supCode}";
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.reasonDesc;
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.itemQty;
                rowIndex++;
            }

            rowIndex++;
            cells = ws.Cells[rowIndex, 1, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"รวมเอกสารใบคืนสินค้าให้ผู้จําหน่าย ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy")} ถึง {req.DateTo.ToString("dd/MM/yyyy")}";
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Font.Bold = true;

            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
            cells.Style.Font.Bold = true;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            #endregion



            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetReturnSupDtExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetReturnSupDtPDF(req);

            int rowIndex = 1;
            #region Title
            // Title Company
            var cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"{company.CompName}";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Merge = true;
            cells.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // Report Title
            cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = "รายงานสรปการคืนสินค้าให้ผู้จำหน่าย";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // Report Branch
            cells = ws.Cells[rowIndex, 1, rowIndex, 1];
            cells.Value = "สาขา";
            cells = ws.Cells[rowIndex, 2, rowIndex, 3];
            cells.Merge = true;
            cells.Value = $"{brn.BrnCode} - {brn.BrnName}";

            // Report Daterange            
            cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"ช่วงวันที่ : {req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            #endregion

            rowIndex += 2;
            #region Column header
            cells = ws.Cells[rowIndex, 1, rowIndex + 1, 1];
            cells.Merge = true;
            cells.Value = "วันที่";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 2, rowIndex + 1, 2];
            cells.Merge = true;
            cells.Value = "เลขที่เอกสาร";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 3, rowIndex, 3];
            cells.Value = "สถานะ";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 3, rowIndex + 1, 3];
            cells.Value = "รหัสสินค้า";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 4, rowIndex, 5];
            cells.Value = "ผู้จำหน่าย";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 4, rowIndex + 1, 5];
            cells.Value = "ชื่อสินค้า";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 6, rowIndex, 6];
            cells.Merge = true;
            cells.Value = "เหตุผลในการคืน";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6];
            cells.Value = "หน่วยนับ";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7];
            cells.Value = "จำนวน";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            #endregion

            rowIndex++;
            #region Items
            var heads = data.GroupBy(x => new { x.docDate, x.docNo, x.docStatus, x.supCode, x.supName, x.reasonDesc })
                                .Select(x => new
                                {
                                    docDate = x.Key.docDate,
                                    docNo = x.Key.docNo,
                                    docStatus = x.Key.docStatus,
                                    supCode = x.Key.supCode,
                                    supName = x.Key.supName,
                                    reasonDesc = x.Key.reasonDesc
                                }).ToList();

            foreach (var head in heads)
            {
                rowIndex++;
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = Convert.ToDateTime(head.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = head.docNo;
                ws.Cells[rowIndex, 3, rowIndex, 3].Value = head.docStatus == "Cancel" ? "ยกเลิก" : "";
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = head.supCode;
                ws.Cells[rowIndex, 5, rowIndex, 5].Value = head.supName;
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = head.reasonDesc;

                var items = data.Where(x => x.docNo == head.docNo).Select(x => new { x.pdId, x.pdName, x.unitName, x.itemQty, }).ToList();
                foreach (var item in items)
                {
                    rowIndex++;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.pdId;
                    ws.Cells[rowIndex, 4, rowIndex, 5].Value = item.pdName;
                    ws.Cells[rowIndex, 4, rowIndex, 5].Merge = true;
                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.unitName;
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.itemQty;
                }
                rowIndex++;

                #region Sum by docNO
                cells = ws.Cells[rowIndex, 3, rowIndex, 6];
                cells.Merge = true;
                cells.Value = $"รวมใบคืนสินค้าให้ผู้จําหน่าย เลขที่ : {head.docNo}";
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                cells = ws.Cells[rowIndex, 7, rowIndex, 7];
                cells.Value = data.Where(x => x.docNo == head.docNo && x.docStatus != "Cancel").Sum(x => x.itemQty);
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                #endregion
                rowIndex++;

            }

            rowIndex++;
            cells = ws.Cells[rowIndex, 1, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"รวมเอกสารใบคืนสินค้าให้ผู้จําหน่าย ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy")} ถึง {req.DateTo.ToString("dd/MM/yyyy")}";
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Font.Bold = true;

            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
            cells.Style.Font.Bold = true;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            #endregion



            return new MemoryStream(pck.GetAsByteArray());

        }

        public List<InvReturnOilResponse> GetReturnOilHdPDF(InventoryRequest req)
        {
            List<InvReturnOilResponse> response = new List<InvReturnOilResponse>();

            var query = (from hd in this.Context.InvReturnOilHds
                         join dt in this.Context.InvReturnOilDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                         && hd.BrnCode == req.BrnCode
                         //&& hd.DocStatus != "Cancel"
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                         ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }
            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.GroupBy(x => new { x.hd.DocDate, x.hd.DocNo, x.hd.DocStatus, x.hd.BrnCodeTo, x.hd.BrnNameTo, x.hd.PoNo, x.hd.ReasonDesc }).Select(x => new InvReturnOilResponse
            {
                docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.Key.DocNo,
                docStatus = x.Key.DocStatus,
                brnCodeTo = x.Key.BrnCodeTo,
                brnNameTo = x.Key.BrnNameTo,
                poNo = x.Key.PoNo,
                reasonDesc = x.Key.ReasonDesc,
                itemQty = x.Sum(s => s.dt.ItemQty) ?? 0,
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }
        public List<InvReturnOilResponse> GetReturnOilDtPDF(InventoryRequest req)
        {
            List<InvReturnOilResponse> response = new List<InvReturnOilResponse>();

            var query = (from hd in this.Context.InvReturnOilHds
                         join dt in this.Context.InvReturnOilDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                        && hd.BrnCode == req.BrnCode
                        //&& hd.DocStatus != "Cancel"
                        && hd.DocDate >= req.DateFrom
                        && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                         ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }
            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.Select(x => new InvReturnOilResponse
            {
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.hd.DocNo,
                docStatus = x.hd.DocStatus,
                brnCodeTo = x.hd.BrnCodeTo,
                brnNameTo = x.hd.BrnNameTo,
                poNo = x.hd.PoNo,
                reasonDesc = x.hd.ReasonDesc,
                seqNo = x.dt.SeqNo,
                pdId = x.dt.PdId,
                pdName = x.dt.PdName,
                unitName = x.dt.UnitName,
                itemQty = x.dt.ItemQty ?? 0,
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }

        public MemoryStream GetReturnOilHdExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetReturnOilHdPDF(req);

            int rowIndex = 1;
            #region Title
            // Title Company
            var cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"{company.CompName}";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Merge = true;
            cells.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // Report Title
            cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = "รายงานใบโอนน้ำมันกลับเข้าคลัง";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // Report Branch
            cells = ws.Cells[rowIndex, 1, rowIndex, 1];
            cells.Value = "สาขา";
            cells = ws.Cells[rowIndex, 2, rowIndex, 3];
            cells.Merge = true;
            cells.Value = $"{brn.BrnCode} - {brn.BrnName}";

            // Report Daterange            
            cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"ช่วงวันที่ : {req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            #endregion

            rowIndex += 2;
            #region Column header
            cells = ws.Cells[rowIndex, 1, rowIndex + 1, 1];
            cells.Merge = true;
            cells.Value = "วันที่";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 2, rowIndex + 1, 2];
            cells.Merge = true;
            cells.Value = "เลขที่เอกสาร";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 3, rowIndex, 3];
            cells.Value = "สถานะ";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 3, rowIndex + 1, 3];
            cells.Value = "รหัสสินค้า";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 4, rowIndex, 5];
            cells.Value = "สาขาปลายทาง";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 4, rowIndex + 1, 6];
            cells.Value = "ชื่อสินค้า";
            cells.Merge = true;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 6, rowIndex, 6];
            cells.Value = "ใบสั่งซื้อ";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Merge = true;
            cells.Value = "เหตุผลในการโอน";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7];
            cells.Value = "หน่วยนับ";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 8, rowIndex, 8];
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 8, rowIndex + 1, 8];
            cells.Value = "จำนวน";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            #endregion

            rowIndex++;
            #region Items
            var heads = data.GroupBy(x => new { x.docDate, x.docNo, x.docStatus, x.brnCodeTo, x.brnNameTo, x.poNo, x.reasonDesc })
                                .Select(x => new
                                {
                                    docDate = x.Key.docDate,
                                    docNo = x.Key.docNo,
                                    docStatus = x.Key.docStatus,
                                    brnCodeTo = x.Key.brnCodeTo,
                                    brnNameTo = x.Key.brnNameTo,
                                    poNo = x.Key.poNo,
                                    reasonDesc = x.Key.reasonDesc
                                }).ToList();

            foreach (var head in heads)
            {
                rowIndex++;
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = Convert.ToDateTime(head.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = head.docNo;
                ws.Cells[rowIndex, 3, rowIndex, 3].Value = head.docStatus == "Cancel" ? "ยกเลิก" : "";
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = head.brnCodeTo;
                ws.Cells[rowIndex, 5, rowIndex, 5].Value = head.brnNameTo;
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = head.poNo;
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = head.reasonDesc;

                var items = data.Where(x => x.docNo == head.docNo).Select(x => new { x.pdId, x.pdName, x.unitName, x.itemQty, }).ToList();
                foreach (var item in items)
                {
                    rowIndex++;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.pdId;
                    ws.Cells[rowIndex, 4, rowIndex, 6].Value = item.pdName;
                    ws.Cells[rowIndex, 4, rowIndex, 6].Merge = true;
                    ws.Cells[rowIndex, 7, rowIndex, 6].Value = item.unitName;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.itemQty;
                }
                rowIndex++;

                #region Sum by docNO
                cells = ws.Cells[rowIndex, 3, rowIndex, 7];
                cells.Merge = true;
                cells.Value = $"รวมใบโอนน้ำมันกลับเข้าคลัง เลขที่ : {head.docNo}";
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                cells = ws.Cells[rowIndex, 8, rowIndex, 8];
                cells.Value = data.Where(x => x.docNo == head.docNo && x.docStatus != "Cancel").Sum(x => x.itemQty);
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                #endregion
                rowIndex++;

            }

            rowIndex++;
            cells = ws.Cells[rowIndex, 1, rowIndex, 7];
            cells.Merge = true;
            cells.Value = $"รวมเอกสารใบโอนน้ำมันกลับเข้าคลัง ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy")} ถึง {req.DateTo.ToString("dd/MM/yyyy")}";
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Font.Bold = true;

            cells = ws.Cells[rowIndex, 8, rowIndex, 8];
            cells.Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
            cells.Style.Font.Bold = true;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            #endregion



            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetReturnOilDtExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetReturnOilDtPDF(req);

            int rowIndex = 1;
            #region Title
            // Title Company
            var cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"{company.CompName}";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Merge = true;
            cells.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // Report Title
            cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = "รายงานใบโอนน้ำมันกลับเข้าคลัง";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // Report Branch
            cells = ws.Cells[rowIndex, 1, rowIndex, 1];
            cells.Value = "สาขา";
            cells = ws.Cells[rowIndex, 2, rowIndex, 3];
            cells.Merge = true;
            cells.Value = $"{brn.BrnCode} - {brn.BrnName}";

            // Report Daterange            
            cells = ws.Cells[rowIndex, 4, rowIndex, 6];
            cells.Merge = true;
            cells.Value = $"ช่วงวันที่ : {req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            #endregion

            rowIndex += 2;
            #region Column header
            cells = ws.Cells[rowIndex, 1, rowIndex + 1, 1];
            cells.Merge = true;
            cells.Value = "วันที่";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 2, rowIndex + 1, 2];
            cells.Merge = true;
            cells.Value = "เลขที่เอกสาร";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 3, rowIndex, 3];
            cells.Value = "สถานะ";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 3, rowIndex + 1, 3];
            cells.Value = "รหัสสินค้า";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 4, rowIndex, 5];
            cells.Value = "สาขาปลายทาง";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 4, rowIndex + 1, 6];
            cells.Value = "ชื่อสินค้า";
            cells.Merge = true;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 6, rowIndex, 6];
            cells.Value = "ใบสั่งซื้อ";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 7, rowIndex, 7];
            cells.Merge = true;
            cells.Value = "เหตุผลในการโอน";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7];
            cells.Value = "หน่วยนับ";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            cells = ws.Cells[rowIndex, 8, rowIndex, 8];
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells = ws.Cells[rowIndex + 1, 8, rowIndex + 1, 8];
            cells.Value = "จำนวน";
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            #endregion

            rowIndex++;
            #region Items
            var heads = data.GroupBy(x => new { x.docDate, x.docNo, x.docStatus, x.brnCodeTo, x.brnNameTo, x.poNo, x.reasonDesc })
                                .Select(x => new
                                {
                                    docDate = x.Key.docDate,
                                    docNo = x.Key.docNo,
                                    docStatus = x.Key.docStatus,
                                    brnCodeTo = x.Key.brnCodeTo,
                                    brnNameTo = x.Key.brnNameTo,
                                    poNo = x.Key.poNo,
                                    reasonDesc = x.Key.reasonDesc
                                }).ToList();

            foreach (var head in heads)
            {
                rowIndex++;
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = Convert.ToDateTime(head.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 2, rowIndex, 2].Value = head.docNo;
                ws.Cells[rowIndex, 3, rowIndex, 3].Value = head.docStatus == "Cancel" ? "ยกเลิก" : "";
                ws.Cells[rowIndex, 4, rowIndex, 4].Value = head.brnCodeTo;
                ws.Cells[rowIndex, 5, rowIndex, 5].Value = head.brnNameTo;
                ws.Cells[rowIndex, 6, rowIndex, 6].Value = head.poNo;
                ws.Cells[rowIndex, 7, rowIndex, 7].Value = head.reasonDesc;

                var items = data.Where(x => x.docNo == head.docNo).Select(x => new { x.pdId, x.pdName, x.unitName, x.itemQty, }).ToList();
                foreach (var item in items)
                {
                    rowIndex++;
                    ws.Cells[rowIndex, 3, rowIndex, 3].Value = item.pdId;
                    ws.Cells[rowIndex, 4, rowIndex, 6].Value = item.pdName;
                    ws.Cells[rowIndex, 4, rowIndex, 6].Merge = true;
                    ws.Cells[rowIndex, 7, rowIndex, 6].Value = item.unitName;
                    ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.itemQty;
                }
                rowIndex++;

                #region Sum by docNO
                cells = ws.Cells[rowIndex, 3, rowIndex, 7];
                cells.Merge = true;
                cells.Value = $"รวมใบโอนน้ำมันกลับเข้าคลัง เลขที่ : {head.docNo}";
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                cells = ws.Cells[rowIndex, 8, rowIndex, 8];
                cells.Value = data.Where(x => x.docNo == head.docNo && x.docStatus != "Cancel").Sum(x => x.itemQty);
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                #endregion
                rowIndex++;

            }

            rowIndex++;
            cells = ws.Cells[rowIndex, 1, rowIndex, 7];
            cells.Merge = true;
            cells.Value = $"รวมเอกสารใบโอนน้ำมันกลับเข้าคลัง ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy")} ถึง {req.DateTo.ToString("dd/MM/yyyy")}";
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Font.Bold = true;

            cells = ws.Cells[rowIndex, 8, rowIndex, 8];
            cells.Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
            cells.Style.Font.Bold = true;
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            #endregion



            return new MemoryStream(pck.GetAsByteArray());
        }
        public MemoryStream GetWithdrawHdExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetWithdrawHdPDF(req);

            int rowIndex = 1;
            // title company
            var companyTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // Print Date
            var printDate = ws.Cells[rowIndex, 6, rowIndex, 7];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // title report
            var reportTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานสรุปการเบิกใช้ในกิจการ";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // title branch
            var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";
            // title date
            var dateTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex += 2; ;

            ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่";
            ws.Cells[rowIndex, 2, rowIndex, 3].Value = $"เลขที่เอกสาร";
            ws.Cells[rowIndex, 2, rowIndex, 3].Merge = true;
            ws.Cells[rowIndex, 4, rowIndex, 4].Value = $"ทะเบียนรถ";
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"ผู้เบิก";
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"เหตุผลที่เบิก";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = $"จำนวน";

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));



            rowIndex++;
            var dateRunning = req.DateFrom;
            while (dateRunning != req.DateTo.AddDays(1))
            {
                var selectItem = data.Where(x => Convert.ToDateTime(x.docDate) == dateRunning).ToList();
                if (selectItem.Count() > 0)
                {
                    //ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่ใบรับสินค้า :";
                    //ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"{dateRunning.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
                    //rowIndex++;
                    foreach (var item in selectItem)
                    {
                        ws.Cells[rowIndex, 1].Value = Convert.ToDateTime(item.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ws.Cells[rowIndex, 2].Value = item.docNo;
                        ws.Cells[rowIndex, 3].Value = item.docStatus == "Cancel" ? "(ยกเลิก)" : "";
                        ws.Cells[rowIndex, 4].Value = item.licensePlate;
                        ws.Cells[rowIndex, 5].Value = item.empName;
                        ws.Cells[rowIndex, 6].Value = item.reasonDesc;
                        ws.Cells[rowIndex, 7].Value = item.totalQty;
                        rowIndex++;
                    }
                    ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมเอกสารใบเบิกใช้ในกิจการ วันที่";
                    ws.Cells[rowIndex, 6, rowIndex, 6].Value = dateRunning.ToString("dd/MM/yyyy");
                    ws.Cells[rowIndex, 7, rowIndex, 7].Value = selectItem.Where(x => x.docStatus != "Cancel").Sum(x => x.totalQty);
                    ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                    rowIndex += 2;
                }

                dateRunning = dateRunning.AddDays(1);
            }

            rowIndex++;

            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมเอกสารใบเบิกใช้ในกิจการ ตั้งแต่วันที่";
            //ws.Cells[rowIndex, 5, rowIndex, 5].Merge = true;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"{req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.totalQty);

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetWithdrawDtExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetWithdrawDtPDF(req);

            int rowIndex = 1;
            #region title
            // title company
            var companyTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // Print Date
            var printDate = ws.Cells[rowIndex, 7, rowIndex, 7];
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // title report
            var reportTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานรายละเอียดการเบิกใช้ในกิจการ";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // title branch
            var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";
            // title date
            var dateTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            #endregion

            rowIndex += 2;
            ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่";
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"ลำดับ";
            ws.Cells[rowIndex, 3, rowIndex, 3].Value = $"รหัสสินค้า";
            ws.Cells[rowIndex, 4, rowIndex, 5].Value = $"ชื่อสินค้า";
            ws.Cells[rowIndex, 4, rowIndex, 5].Merge = true;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"หน่วยนับ";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = $"จำนวน";

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));


            rowIndex += 2;
            var docNos = data.Select(x => x.docNo).Distinct();
            foreach (var docNo in docNos)
            {

                var head = data.FirstOrDefault(x => x.docNo == docNo);
                var cell = ws.Cells[rowIndex, 1, rowIndex, 1];
                cell.Value = $"{Convert.ToDateTime(head.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

                cell = ws.Cells[rowIndex, 2, rowIndex, 2];
                cell.Value = $"เลขที่ :";
                cell = ws.Cells[rowIndex, 3, rowIndex, 3];
                var status = head.docStatus == "Cancel" ? "(ยกเลิก)" : "";
                cell.Value = $"{head.docNo} {status}";

                cell = ws.Cells[rowIndex, 4, rowIndex, 4];
                cell.Value = $"ผู้เบิก :";
                cell = ws.Cells[rowIndex, 5, rowIndex, 5];
                cell.Value = $"{head.empName}";

                cell = ws.Cells[rowIndex, 6, rowIndex, 6];
                cell.Value = $"ทะเบียนรถ :";
                cell = ws.Cells[rowIndex, 7, rowIndex, 7];
                cell.Value = $"{head.licensePlate}";

                rowIndex++;
                cell = ws.Cells[rowIndex, 4, rowIndex, 4];
                cell.Value = $"ส่วนงาน :";
                cell = ws.Cells[rowIndex, 5, rowIndex, 5];
                cell.Value = $"{head.useBrnName}";

                cell = ws.Cells[rowIndex, 6, rowIndex, 6];
                cell.Value = $"เหตุผล :";
                cell = ws.Cells[rowIndex, 7, rowIndex, 7];
                cell.Value = $"{head.reasonDesc}";
                ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;


                var items = data.Where(x => x.docNo == docNo).ToList();
                foreach (var item in items)
                {
                    rowIndex++;
                    ws.Cells[rowIndex, 2].Value = item.seqNo;
                    ws.Cells[rowIndex, 3].Value = item.pdId;
                    ws.Cells[rowIndex, 4].Value = item.pdName;
                    ws.Cells[rowIndex, 4, rowIndex, 5].Merge = true;
                    ws.Cells[rowIndex, 6].Value = item.unitName;
                    ws.Cells[rowIndex, 7].Value = item.itemQty;
                }
                rowIndex += 2;
            }

            rowIndex++;
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมเอกสารใบเบิกใช้ในกิจการ ตั้งแต่วันที่";
            //ws.Cells[rowIndex, 5, rowIndex, 5].Merge = true;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"{req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


            return new MemoryStream(pck.GetAsByteArray());
        }
        public MemoryStream GetTransferInHdExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetTransferInHdPDF(req);

            int rowIndex = 1;
            // title company
            var companyTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // Print Date
            var printDate = ws.Cells[rowIndex, 6, rowIndex, 7];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // title report
            var reportTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานสรุปใบรับโอนสินค้า";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // title branch
            var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";
            // title date
            var dateTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex += 2; ;


            ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่";
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"เลขที่เอกสาร";
            ws.Cells[rowIndex, 4, rowIndex, 4].Value = "เลขที่อ้างอิง";
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รับโอนจากสาขา";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = $"จำนวน";
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));

            rowIndex++;
            foreach (var item in data)
            {
                ws.Cells[rowIndex, 1].Value = Convert.ToDateTime(item.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 2].Value = item.docNo;
                ws.Cells[rowIndex, 3].Value = item.docStatus == "Cancel" ? "(ยกเลิก)" : "";
                ws.Cells[rowIndex, 4].Value = $"{item.brnCodeFrom}- {item.brnNameFrom}";
                ws.Cells[rowIndex, 5].Value = item.refNo;
                ws.Cells[rowIndex, 7].Value = item.itemQty;
                rowIndex++;
            }


            rowIndex++;
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมใบรับโอนสินค้า ตั้งแต่วันที่";
            //ws.Cells[rowIndex, 5, rowIndex, 5].Merge = true;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"{req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetTransferInDtExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetTransferInDtPDF(req);

            int rowIndex = 1;
            // title company
            var companyTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // Print Date
            var printDate = ws.Cells[rowIndex, 6, rowIndex, 7];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // title report
            var reportTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานรายละเอียดใบรับโอนสินค้า";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // title branch
            var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";
            // title date
            var dateTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex += 2; ;


            ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่";
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"เลขที่เอกสาร";

            ws.Cells[rowIndex + 1, 3, rowIndex + 1, 3].Value = $"รหัสสินค้า";

            ws.Cells[rowIndex, 4, rowIndex, 4].Value = "เลขที่อ้างอิง";
            ws.Cells[rowIndex + 1, 4, rowIndex + 1, 5].Value = "รายการสินค้า";
            ws.Cells[rowIndex + 1, 4, rowIndex + 1, 5].Merge = true;

            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รับโอนจากสาขา";

            ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Value = $"หน่วยนับ";
            ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Value = $"จำนวน";

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex + 1, 1, rowIndex + 1, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex + 1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex + 1, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));


            rowIndex += 2;
            var dateRunning = req.DateFrom;
            while (dateRunning != req.DateTo.AddDays(1))
            {
                var selectItem = data.Where(x => Convert.ToDateTime(x.docDate) == dateRunning).OrderBy(x => x.docNo).ToList();
                if (selectItem.Count() > 0)
                {

                    var headers = selectItem.GroupBy(x => new { x.docDate, x.docNo, x.docStatus, x.refNo, x.brnCodeFrom, x.brnNameFrom }).Select(x => new { x.Key.docDate, x.Key.docNo, x.Key.docStatus, x.Key.refNo, x.Key.brnCodeFrom, x.Key.brnNameFrom }).ToList();
                    foreach (var head in headers)
                    {

                        ws.Cells[rowIndex, 1].Value = Convert.ToDateTime(head.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ws.Cells[rowIndex, 2].Value = head.docNo;
                        ws.Cells[rowIndex, 3].Value = head.docStatus == "Cancel" ? "(ยกเลิก)" : "";
                        ws.Cells[rowIndex, 4].Value = head.refNo;
                        ws.Cells[rowIndex, 5].Value = $"{head.brnCodeFrom}- {head.brnNameFrom}";
                        rowIndex++;

                        var items = data.Where(x => x.docNo == head.docNo).ToList();
                        foreach (var item in items)
                        {
                            ws.Cells[rowIndex, 3].Value = item.pdId;
                            ws.Cells[rowIndex, 4, rowIndex, 5].Value = item.pdName;
                            ws.Cells[rowIndex, 4, rowIndex, 5].Merge = true;
                            ws.Cells[rowIndex, 6].Value = item.unitName;
                            ws.Cells[rowIndex, 7].Value = item.itemQty;
                            rowIndex++;
                        }
                        ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมใบรับโอนสินค้า เลขที่";
                        ws.Cells[rowIndex, 6, rowIndex, 6].Value = head.docNo;
                        ws.Cells[rowIndex, 7, rowIndex, 7].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
                        ws.Cells[rowIndex, 5, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[rowIndex, 5, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rowIndex += 2;
                    }
                }
                dateRunning = dateRunning.AddDays(1);
            }

            rowIndex++;

            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมใบรับโอนสินค้า ตั้งแต่วันที่";
            //ws.Cells[rowIndex, 5, rowIndex, 5].Merge = true;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"{req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetTransferOutHdExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetTransferOutHdPDF(req);

            int rowIndex = 1;
            // title company
            var companyTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // Print Date
            var printDate = ws.Cells[rowIndex, 6, rowIndex, 7];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // title report
            var reportTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานใบโอนจ่ายสินค้า";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // title branch
            var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";
            // title date
            var dateTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex += 2; ;


            ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่";
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"เลขที่เอกสาร";
            ws.Cells[rowIndex, 3, rowIndex, 3].Value = $"";
            ws.Cells[rowIndex, 4, rowIndex, 4].Value = $"โอนจ่ายให้สาขา";
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"เลขที่อ้างอิง";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = $"จำนวน";
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));


            rowIndex++;
            foreach (var item in data)
            {
                ws.Cells[rowIndex, 1].Value = Convert.ToDateTime(item.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 2].Value = item.docNo;
                ws.Cells[rowIndex, 3].Value = item.docStatus == "Cancel" ? "(ยกเลิก)" : "";
                ws.Cells[rowIndex, 4].Value = $"{item.brnCodeTo}- {item.brnNameTo}";
                ws.Cells[rowIndex, 5].Value = item.refNo;
                ws.Cells[rowIndex, 7].Value = item.itemQty;
                rowIndex++;
            }
            rowIndex++;

            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมใบโอนจ่ายสินค้า ตั้งแต่วันที่";
            //ws.Cells[rowIndex, 5, rowIndex, 5].Merge = true;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"{req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


            return new MemoryStream(pck.GetAsByteArray());
        }

        public MemoryStream GetTransferOutDtExcel(InventoryRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var data = this.GetTransferOutDtPDF(req);

            int rowIndex = 1;
            // title company
            var companyTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            companyTitle.Merge = true;
            companyTitle.Value = $"{company.CompName}";
            companyTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // Print Date
            var printDate = ws.Cells[rowIndex, 6, rowIndex, 7];
            printDate.Merge = true;
            printDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            rowIndex++;

            // title report
            var reportTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            reportTitle.Merge = true;
            reportTitle.Value = $"รายงานใบโอนจ่ายสินค้า";
            reportTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;

            // title branch
            var branchTitle = ws.Cells[rowIndex, 1, rowIndex, 2];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขาที่ : {brn.BrnCode} - {brn.BrnName}";
            // title date
            var dateTitle = ws.Cells[rowIndex, 3, rowIndex, 5];
            dateTitle.Merge = true;
            dateTitle.Value = $"วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}  ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";
            dateTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex += 2; ;


            ws.Cells[rowIndex, 1, rowIndex, 1].Value = $"วันที่";
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = $"เลขที่เอกสาร";

            ws.Cells[rowIndex + 1, 3, rowIndex + 1, 3].Value = $"รหัสสินค้า";

            ws.Cells[rowIndex, 4, rowIndex, 4].Value = "เลขที่อ้างอิง";
            ws.Cells[rowIndex + 1, 4, rowIndex + 1, 5].Value = "รายการสินค้า";
            ws.Cells[rowIndex + 1, 4, rowIndex + 1, 5].Merge = true;

            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"โอนให้สาขา";

            ws.Cells[rowIndex + 1, 6, rowIndex + 1, 6].Value = $"หน่วยนับ";
            ws.Cells[rowIndex + 1, 7, rowIndex + 1, 7].Value = $"จำนวน";

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex + 1, 1, rowIndex + 1, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex + 1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex + 1, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));


            rowIndex += 2;
            var dateRunning = req.DateFrom;
            while (dateRunning != req.DateTo.AddDays(1))
            {
                var selectItem = data.Where(x => Convert.ToDateTime(x.docDate) == dateRunning).OrderBy(x => x.docNo).ToList();
                if (selectItem.Count() > 0)
                {

                    var headers = selectItem.GroupBy(x => new { x.docDate, x.docNo, x.docStatus, x.refNo, x.brnCodeTo, x.brnNameTo }).Select(x => new { x.Key.docDate, x.Key.docNo, x.Key.docStatus, x.Key.refNo, x.Key.brnCodeTo, x.Key.brnNameTo }).ToList();
                    foreach (var head in headers)
                    {

                        ws.Cells[rowIndex, 1].Value = Convert.ToDateTime(head.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ws.Cells[rowIndex, 2].Value = head.docNo;
                        ws.Cells[rowIndex, 3].Value = head.docStatus == "Cancel" ? "(ยกเลิก)" : "";
                        ws.Cells[rowIndex, 4].Value = head.refNo;
                        ws.Cells[rowIndex, 5].Value = $"{head.brnCodeTo}- {head.brnNameTo}";
                        rowIndex++;

                        var items = data.Where(x => x.docNo == head.docNo).ToList();
                        foreach (var item in items)
                        {
                            ws.Cells[rowIndex, 3].Value = item.pdId;
                            ws.Cells[rowIndex, 4, rowIndex, 5].Value = item.pdName;
                            ws.Cells[rowIndex, 4, rowIndex, 5].Merge = true;
                            ws.Cells[rowIndex, 6].Value = item.unitName;
                            ws.Cells[rowIndex, 7].Value = item.itemQty;
                            rowIndex++;
                        }
                        ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมใบโอนจ่ายสินค้า เลขที่";
                        ws.Cells[rowIndex, 6, rowIndex, 6].Value = head.docNo;
                        ws.Cells[rowIndex, 7, rowIndex, 7].Value = items.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);
                        ws.Cells[rowIndex, 5, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[rowIndex, 5, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rowIndex += 2;
                    }
                }
                dateRunning = dateRunning.AddDays(1);
            }

            rowIndex++;

            ws.Cells[rowIndex, 5, rowIndex, 5].Value = $"รวมใบโอนจ่ายสินค้า ตั้งแต่วันที่";
            //ws.Cells[rowIndex, 5, rowIndex, 5].Merge = true;
            ws.Cells[rowIndex, 6, rowIndex, 6].Value = $"{req.DateFrom.ToString("dd/MM/yyyy")} - {req.DateTo.ToString("dd/MM/yyyy")}";
            ws.Cells[rowIndex, 7, rowIndex, 7].Value = data.Where(x => x.docStatus != "Cancel").Sum(x => x.itemQty);

            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


            return new MemoryStream(pck.GetAsByteArray());
        }
        public List<InvAdjustResponse> GetAdjustHdPDF(InventoryRequest req)
        {
            List<InvAdjustResponse> response = new List<InvAdjustResponse>();

            var query = (from hd in this.Context.InvAdjustHds
                         join dt in this.Context.InvAdjustDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         join pm in this.Context.MasProducts on new { dt.PdId } equals new { pm.PdId }
                         where hd.CompCode == req.CompCode
                         // && hd.DocStatus != "Cancel"
                         && hd.BrnCode == req.BrnCode
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt, pm }
                     ).AsQueryable();

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd))
            {
                query = query.Where(x => string.Compare(x.dt.PdId, req.ProductIdStart) >= 0 && string.Compare(x.dt.PdId, req.ProductIdEnd) <= 0).AsQueryable();
            }
            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd))
            {
                query = query.Where(x => string.Compare(x.pm.GroupId, req.ProductGroupIdStart) >= 0 && string.Compare(x.pm.GroupId, req.ProductGroupIdEnd) <= 0).AsQueryable();
            }

            response = query.GroupBy(x => new { x.hd.DocDate, x.hd.DocNo, x.hd.DocStatus, x.hd.DocType, x.hd.EmpCode, x.hd.EmpName, x.hd.ReasonDesc, x.hd.Remark, x.hd.RefNo, x.hd.BrnCodeFrom, x.hd.BrnNameFrom }).Select(x => new InvAdjustResponse
            {
                docDate = x.Key.DocDate.Value.ToString("yyyy-MM-dd"),
                docNo = x.Key.DocNo,
                docStatus = x.Key.DocStatus,
                docType = x.Key.DocType,
                empCode = x.Key.EmpCode,
                empName = x.Key.EmpName,
                reasonDesc = x.Key.ReasonDesc,
                remark = x.Key.Remark,
                refNo = x.Key.RefNo,
                brnCodeFrom = x.Key.BrnCodeFrom,
                brnNameFrom = x.Key.BrnNameFrom,
                itemQty = x.Sum(s => s.dt.ItemQty) ?? 0,
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
                x.compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";
                x.brnCode = branch.b.BrnCode;
                x.brnName = branch.b.BrnName;
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }
        public List<InvAdjustResponse> GetAdjustDtPDF(InventoryRequest req)
        {
            List<InvAdjustResponse> response = new List<InvAdjustResponse>();

            response = (from hd in this.Context.InvAdjustHds
                        join dt in this.Context.InvAdjustDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                        where hd.CompCode == req.CompCode
                        // && hd.DocStatus != "Cancel"
                        && hd.BrnCode == req.BrnCode
                        && hd.DocDate >= req.DateFrom
                        && hd.DocDate <= req.DateTo
                        select new { hd, dt }
                     ).Select(x => new InvAdjustResponse
                     {
                         docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                         docNo = x.hd.DocNo,
                         docStatus = x.hd.DocStatus,
                         docType = x.hd.DocType,
                         empCode = x.hd.EmpCode,
                         empName = x.hd.EmpName,
                         reasonDesc = x.hd.ReasonDesc,
                         remark = x.hd.Remark,
                         refNo = x.hd.RefNo,
                         brnCodeFrom = x.hd.BrnCodeFrom,
                         brnNameFrom = x.hd.BrnNameFrom,
                         seqNo = x.dt.SeqNo,
                         pdId = x.dt.PdId,
                         pdName = x.dt.PdName,
                         unitName = x.dt.UnitName,
                         itemQty = x.dt.ItemQty ?? 0
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
            });

            return response.OrderBy(x => x.docDate).ThenBy(x => x.docNo).ToList();
        }


        public MemoryStream GetAdjustHdExcel(InventoryRequest req)
        {
            throw new NotImplementedException();
        }


        public MemoryStream GetAdjustDtExcel(InventoryRequest req)
        {
            throw new NotImplementedException();
        }

        public List<InvTransferCompareResponse> GetTransferComparePDF(InventoryRequest req)
        {
            List<InvTransferCompareResponse> response = new List<InvTransferCompareResponse>();

            var branch = (from b in this.Context.MasBranches
                          join c in this.Context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                          && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();
            string compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";

            string sql = @$"select '{branch.c.CompCode}' as compCode,'{branch.c.CompName}' as compName,'{compImage}' as compImage ,'{branch.b.BrnCode}' as brnCode,'{branch.b.BrnName}' as brnName
                            ,tro.brnCodeOut,tro.docNoOut,tro.docDateOut,tro.brnCodeOutTo,tro.itemQtyOut
                            ,isnull(tri.BRN_CODE,'') as brnCodeIn ,isnull(tri.DOC_NO,'') as docNoIn,tri.DOC_DATE as docDateIn,isnull(tri.ITEM_QTY,0) as itemQtyIn
                            from (
                            select toh.COMP_CODE as compCode,toh.BRN_CODE as brnCodeOut,toh.DOC_NO as docNoOut,format(toh.DOC_DATE,'yyyy-MM-dd') as docDateOut,toh.BRN_CODE_TO as brnCodeOutTo,sum(tod.item_qty) as itemQtyOut
                            from INV_TRANOUT_HD toh
                            inner join INV_TRANOUT_DT tod
                            on toh.COMP_CODE = tod.COMP_CODE
                            and toh.BRN_CODE = tod.BRN_CODE
                            and toh.LOC_CODE = tod.LOC_CODE
                            and toh.DOC_NO = tod.DOC_NO
                            where toh.COMP_CODE = '{req.CompCode}'                            
                            and toh.DOC_STATUS <> 'Cancel'
                            and toh.DOC_DATE between '{req.DateFrom:yyyy-MM-dd}' and '{req.DateTo:yyyy-MM-dd}'
                            group by toh.COMP_CODE,toh.BRN_CODE,toh.DOC_NO,toh.DOC_DATE,toh.BRN_CODE_TO
                            )tro
                            left join 
                            (
	                            select tih.COMP_CODE,tih.BRN_CODE,tih.DOC_NO,format(tih.DOC_DATE,'yyyy-MM-dd') as DOC_DATE ,tih.BRN_CODE_FROM,tih.REF_NO,sum(tid.ITEM_QTY) as ITEM_QTY
	                            from INV_TRANIN_HD tih
	                            inner join INV_TRANIN_DT tid
	                            on tih.COMP_CODE = tid.COMP_CODE
	                            and tih.BRN_CODE = tid.BRN_CODE
	                            and tih.LOC_CODE = tid.LOC_CODE
	                            and tih.DOC_NO = tid.DOC_NO
	                            where tih.DOC_STATUS <> 'Cancel'
                                and tih.DOC_DATE >= '{req.DateFrom:yyyy-MM-dd}'
	                            group by tih.COMP_CODE,tih.BRN_CODE,tih.DOC_NO,tih.DOC_DATE,tih.BRN_CODE_FROM,tih.REF_NO
                            )tri
                            on tro.compCode = tri.COMP_CODE
                            and tro.brnCodeOut = tri.BRN_CODE_FROM
                            and tro.docNoOut = tri.REF_NO";

            Task<List<InvTransferCompareResponse>> result = Task.Run(() => this.GetEntityFromSql<List<InvTransferCompareResponse>>(Context, sql));
            Task.WhenAll(result);
            response = result.Result;

            return response.OrderBy(x => x.docNoOut).ToList();

        }

        public MemoryStream GetTransferCompareExcel(InventoryRequest req)
        {
            var data = GetTransferComparePDF(req);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            // Title Company
            var headerCompany = ws.Cells[1, 4, 1, 9];
            headerCompany.Merge = true;
            headerCompany.Value = $"{company.CompName}";
            headerCompany.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var headerPrintDate = ws.Cells[1, 13, 1, 14];
            headerPrintDate.Merge = true;
            headerPrintDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            headerPrintDate.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // Report Title
            var headerTitle = ws.Cells[2, 4, 2, 9];
            headerTitle.Merge = true;
            headerTitle.Value = "รายงานตรวจสอบการโอนจ่ายสินค้าระหว่างสาขา";
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

            ws.Cells[4, 1, 4, 7].Merge = true;
            ws.Cells[4, 1, 4, 7].Value = "ใบโอนจ่ายสินค้า";
            ws.Cells[4, 1, 4, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 1, 4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 1, 4, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 4, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 4, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 4, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 8, 4, 13].Merge = true;
            ws.Cells[4, 8, 4, 13].Value = "ใบรับโอนสินค้า";
            ws.Cells[4, 8, 4, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 8, 4, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 8, 4, 13].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 8, 4, 13].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 8, 4, 13].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 8, 4, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 1, 5, 1].Merge = true;
            ws.Cells[5, 1, 5, 1].Value = "สาขา";
            ws.Cells[5, 1, 5, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 1, 5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 1, 5, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 1, 5, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 1, 5, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 2, 5, 2].Merge = true;
            ws.Cells[5, 2, 5, 2].Value = "วันที่";
            ws.Cells[5, 2, 5, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 2, 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 2, 5, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 2, 5, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 2, 5, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 2, 5, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 3, 5, 5].Merge = true;
            ws.Cells[5, 3, 5, 5].Value = "เลขที่เอกสาร";
            ws.Cells[5, 3, 5, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 3, 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 3, 5, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 3, 5, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 3, 5, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 3, 5, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 6, 5, 6].Merge = true;
            ws.Cells[5, 6, 5, 6].Value = "โอนให้สาขา";
            ws.Cells[5, 6, 5, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 6, 5, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 6, 5, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 6, 5, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 6, 5, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 6, 5, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 7, 5, 7].Merge = true;
            ws.Cells[5, 7, 5, 7].Value = "จำนวน(ชิ้น)";
            ws.Cells[5, 7, 5, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 7, 5, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 7, 5, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 7, 5, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 7, 5, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 7, 5, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 8, 5, 8].Merge = true;
            ws.Cells[5, 8, 5, 8].Value = "สาขา";
            ws.Cells[5, 8, 5, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 8, 5, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 8, 5, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 8, 5, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 8, 5, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 8, 5, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 9, 5, 9].Merge = true;
            ws.Cells[5, 9, 5, 9].Value = "วันที่";
            ws.Cells[5, 9, 5, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 9, 5, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 9, 5, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 9, 5, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 9, 5, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 9, 5, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 10, 5, 12].Merge = true;
            ws.Cells[5, 10, 5, 12].Value = "เลขที่เอกสาร";
            ws.Cells[5, 10, 5, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 10, 5, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 10, 5, 12].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 10, 5, 12].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 10, 5, 12].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 10, 5, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;


            ws.Cells[5, 13, 5, 13].Merge = true;
            ws.Cells[5, 13, 5, 13].Value = "จำนวน(ชิ้น)";
            ws.Cells[5, 13, 5, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 13, 5, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 13, 5, 13].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 13, 5, 13].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 13, 5, 13].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 13, 5, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;


            ws.Cells[4, 14, 5, 14].Merge = true;
            ws.Cells[4, 14, 5, 14].Value = "หมายเหตุ";
            ws.Cells[4, 14, 5, 14].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 14, 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 14, 5, 14].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 14, 5, 14].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 14, 5, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            int rowIndex = 6;
            decimal totalItemQtyOut = 0;
            decimal totalItemQtyIn = 0;

            foreach (var item in data)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.brnCodeOut;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 2, rowIndex, 2].Value = Convert.ToDateTime(item.docDateOut).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 3, rowIndex, 5].Value = item.docNoOut;
                ws.Cells[rowIndex, 3, rowIndex, 5].Merge = true;
                ws.Cells[rowIndex, 3, rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.brnCodeOutTo;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.itemQtyOut;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalItemQtyOut += item.itemQtyOut;

                ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.brnCodeIn;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 9, rowIndex, 9].Value = Convert.ToDateTime(item.docDateIn).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 10, rowIndex, 12].Value = item.docNoIn;
                ws.Cells[rowIndex, 10, rowIndex, 12].Merge = true;
                ws.Cells[rowIndex, 10, rowIndex, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 10, rowIndex, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 10, rowIndex, 12].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 10, rowIndex, 12].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 13, rowIndex, 13].Value = item.itemQtyIn;
                ws.Cells[rowIndex, 13, rowIndex, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 13, rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalItemQtyIn += item.itemQtyIn;

                ws.Cells[rowIndex, 14, rowIndex, 14].Value = item.brnCodeIn == string.Empty ? "ไม่พบใบรับโอน" : string.Empty ;
                ws.Cells[rowIndex, 14, rowIndex, 14].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 14, rowIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 14, rowIndex, 14].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 14, rowIndex, 14].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                

                rowIndex++;
            }

            ws.Cells[rowIndex, 1, rowIndex, 1].Value = "รวมทั้งสิ้น";
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 7, rowIndex, 7].Value = totalItemQtyOut;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 13, rowIndex, 13].Value = totalItemQtyIn;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 13, rowIndex, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            return new MemoryStream(pck.GetAsByteArray());
        }

        public List<InvTransferNotInResponse> GetTransferNotInPDF(InventoryRequest req)
        {
            List<InvTransferNotInResponse> response = new List<InvTransferNotInResponse>();

            var branch = (from b in this.Context.MasBranches
                          join c in this.Context.MasCompanies on b.CompCode equals c.CompCode
                          where b.CompCode == req.CompCode
                          && b.BrnCode == req.BrnCode
                          select new { b, c }).FirstOrDefault();
            string compImage = "https://maxstation.pt.co.th/assets/images/ptg_logo.png";

            //string sql = @$"select '{branch.c.CompCode}' as compCode,'{branch.c.CompName}' as compName,'{compImage}' as compImage ,'{branch.b.BrnCode}' as brnCode,'{branch.b.BrnName}' as brnName
            //                ,tro.brnCodeOut,tro.docNoOut,tro.docDateOut,tro.brnCodeOutTo,tro.itemQtyOut
            //                from (
            //                select toh.COMP_CODE as compCode,toh.BRN_CODE as brnCodeOut,toh.DOC_NO as docNoOut,format(toh.DOC_DATE,'yyyy-MM-dd') as docDateOut,toh.BRN_CODE_TO as brnCodeOutTo,sum(tod.item_qty) as itemQtyOut
            //                from INV_TRANOUT_HD toh
            //                inner join INV_TRANOUT_DT tod
            //                on toh.COMP_CODE = tod.COMP_CODE
            //                and toh.BRN_CODE = tod.BRN_CODE
            //                and toh.LOC_CODE = tod.LOC_CODE
            //                and toh.DOC_NO = tod.DOC_NO
            //                where toh.COMP_CODE = '{req.CompCode}'                                                        
            //                and toh.DOC_DATE between '{req.DateFrom:yyyy-MM-dd}' and '{req.DateTo:yyyy-MM-dd}'
            //                and toh.DOC_STATUS = 'Active'
            //                group by toh.COMP_CODE,toh.BRN_CODE,toh.DOC_NO,toh.DOC_DATE,toh.BRN_CODE_TO
            //                )tro";
            //Task<List<InvTransferNotInResponse>> result = Task.Run(() => this.GetEntityFromSql<List<InvTransferNotInResponse>>(Context, sql));
            //Task.WhenAll(result);
            //response = result.Result;

            var query = (from hd in this.Context.InvTranoutHds
                         join dt in this.Context.InvTranoutDts on new { hd.CompCode, hd.BrnCode, hd.LocCode, hd.DocNo } equals new { dt.CompCode, dt.BrnCode, dt.LocCode, dt.DocNo }
                         where hd.CompCode == req.CompCode
                         && hd.DocStatus == "Active"
                         && hd.DocDate >= req.DateFrom
                         && hd.DocDate <= req.DateTo
                         select new { hd, dt }
                         ).AsQueryable();

            response = query.Select(x => new InvTransferNotInResponse
                            {
                                compCode = branch.c.CompCode,
                                compName = branch.c.CompName,
                                compImage = compImage,
                                brnCode = branch.b.BrnCode,
                                brnName = branch.b.BrnName,
                                brnCodeFrom = x.hd.BrnCode,
                                docNo = x.hd.DocNo,
                                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                                brnCodeTo = x.hd.BrnCodeTo,
                                seqNo = x.dt.SeqNo,
                                pdId = x.dt.PdId,
                                pdName = x.dt.PdName,
                                itemQty = x.dt.ItemQty ?? 0,
                            }).ToList();


            return response.OrderBy(x => x.brnCodeFrom).ThenBy(x=>x.docNo).ToList();
        }

        public MemoryStream GetTransferNotInExcel(InventoryRequest req)
        {
            var data = GetTransferNotInPDF(req);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");
            var brn = Context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = Context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            // Title Company
            var headerCompany = ws.Cells[1, 4, 1, 9];
            headerCompany.Merge = true;
            headerCompany.Value = $"{company.CompName}";
            headerCompany.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Print Date
            var headerPrintDate = ws.Cells[1, 10, 1, 11];
            headerPrintDate.Merge = true;
            headerPrintDate.Value = $"วันที่พิมพ์ {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
            headerPrintDate.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // Report Title
            var headerTitle = ws.Cells[2, 4, 2, 9];
            headerTitle.Merge = true;
            headerTitle.Value = "รายงานตรวจสอบการโอนสินค้า ที่ยังไม่ทำ รับโอน";
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

            ws.Cells[4, 1, 4, 10].Merge = true;
            ws.Cells[4, 1, 4, 10].Value = "ใบโอนจ่ายสินค้า";
            ws.Cells[4, 1, 4, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 1, 4, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 1, 4, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 4, 10].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 4, 10].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 1, 4, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 1, 5, 1].Merge = true;
            ws.Cells[5, 1, 5, 1].Value = "สาขา";
            ws.Cells[5, 1, 5, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 1, 5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 1, 5, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 1, 5, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 1, 5, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 1, 5, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 2, 5, 2].Merge = true;
            ws.Cells[5, 2, 5, 2].Value = "วันที่";
            ws.Cells[5, 2, 5, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 2, 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 2, 5, 2].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 2, 5, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 2, 5, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 2, 5, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 3, 5, 5].Merge = true;
            ws.Cells[5, 3, 5, 5].Value = "เลขที่เอกสาร";
            ws.Cells[5, 3, 5, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 3, 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 3, 5, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 3, 5, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 3, 5, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 3, 5, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 6, 5, 6].Merge = true;
            ws.Cells[5, 6, 5, 6].Value = "โอนให้สาขา";
            ws.Cells[5, 6, 5, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 6, 5, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 6, 5, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 6, 5, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 6, 5, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 6, 5, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 7, 5, 7].Merge = true;
            ws.Cells[5, 7, 5, 7].Value = "ลำดับ";
            ws.Cells[5, 7, 5, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 7, 5, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 7, 5, 7].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 7, 5, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 7, 5, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 7, 5, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 8, 5, 8].Merge = true;
            ws.Cells[5, 8, 5, 8].Value = "รหัสสินค้า";
            ws.Cells[5, 8, 5, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 8, 5, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 8, 5, 8].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 8, 5, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 8, 5, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 8, 5, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 9, 5, 9].Merge = true;
            ws.Cells[5, 9, 5, 9].Value = "ชื่อสินค้า";
            ws.Cells[5, 9, 5, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 9, 5, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 9, 5, 9].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 9, 5, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 9, 5, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 9, 5, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[5, 10, 5, 10].Merge = true;
            ws.Cells[5, 10, 5, 10].Value = "จำนวน(ชิ้น)";
            ws.Cells[5, 10, 5, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[5, 10, 5, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[5, 10, 5, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 10, 5, 10].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 10, 5, 10].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[5, 10, 5, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[4, 11, 5, 11].Merge = true;
            ws.Cells[4, 11, 5, 11].Value = "หมายเหตุ";
            ws.Cells[4, 11, 5, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[4, 11, 5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[4, 11, 5, 11].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 11, 5, 11].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[4, 11, 5, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            int rowIndex = 6;
            decimal totalItemQty = 0;

            foreach (var item in data)
            {
                ws.Cells[rowIndex, 1, rowIndex, 1].Value = item.brnCodeFrom;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 2, rowIndex, 2].Value = Convert.ToDateTime(item.docDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 3, rowIndex, 5].Value = item.docNo;
                ws.Cells[rowIndex, 3, rowIndex, 5].Merge = true;
                ws.Cells[rowIndex, 3, rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 3, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 3, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 6, rowIndex, 6].Value = item.brnCodeTo;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 7, rowIndex, 7].Value = item.seqNo;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 7, rowIndex, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 8, rowIndex, 8].Value = item.pdId;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 8, rowIndex, 8].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 9, rowIndex, 9].Value = item.pdId;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 9, rowIndex, 9].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                ws.Cells[rowIndex, 10, rowIndex, 10].Value = item.itemQty;
                ws.Cells[rowIndex, 10, rowIndex, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 10, rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                totalItemQty += item.itemQty;

                ws.Cells[rowIndex, 11, rowIndex, 11].Value = item.remark;
                ws.Cells[rowIndex, 11, rowIndex, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[rowIndex, 11, rowIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[rowIndex, 11, rowIndex, 11].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                ws.Cells[rowIndex, 11, rowIndex, 11].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                rowIndex++;
            }

            ws.Cells[rowIndex, 1, rowIndex, 1].Value = "รวมทั้งสิ้น";
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            ws.Cells[rowIndex, 10, rowIndex, 10].Value = totalItemQty;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            ws.Cells[rowIndex, 10, rowIndex, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

            return new MemoryStream(pck.GetAsByteArray());
        }
    }
}
