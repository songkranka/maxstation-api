using AutoMapper;
using MaxStation.Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Models.Response;
using Report.API.Domain.Repositories;
using Report.API.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Report.API.Repositories
{
    public class ReportStockRepository : SqlDataAccessHelper, IReportStockRepository
    {
        private readonly IMapper _mapper;

        public ReportStockRepository(PTMaxstationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public MemoryStream GetReportStockExcel(ReportStockRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();

            var docDate = req.DateFrom; // DateTime.ParseExact(, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var dateNow = DateTime.ParseExact(currentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var firstDayOfMonth = new DateTime(docDate.Year, docDate.Month, 1);
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var ws = pck.Workbook.Worksheets.Add("sheet1");
            ws.Cells.Style.Font.Name = "Angsana New";
            ws.Cells.Style.Font.Size = 14;
            ws.Cells["A1"].Value = $"{company.CompName}";

            using (var r = ws.Cells["A1:L1"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                r.Style.Font.Bold = true;
            };



            using (var r = ws.Cells["A1:L1"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                r.Style.Font.Bold = true;
            };


            ws.Cells["A2"].Value = $"สถานีบริการ : {brn.BrnCode} {brn.BrnName}";
            ws.Cells["A2"].Style.Font.Size = 12;

            ws.Cells["B2"].Value = $"สรุปการเคลื่อนไหวสินค้า";
            using (var r = ws.Cells["B2:G2"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                r.Style.Font.Bold = true;
            };

            ws.Cells["H2"].Value = $"ณ วันที่ {docDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

            using (var r = ws.Cells["H2:L2"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                r.Style.Font.Bold = true;
            };

            var rowIndex = 3;
            ws.Cells[rowIndex, 1].Value = "รหัสสินค้า/รายการ";
            ws.Cells[rowIndex, 2].Value = "ยอดยกมา";
            ws.Cells[rowIndex, 3].Value = "รับสินค้า";
            ws.Cells[rowIndex, 4].Value = "รับโอน";
            ws.Cells[rowIndex, 5].Value = "ขาย";
            ws.Cells[rowIndex, 6].Value = "แถม";
            ws.Cells[rowIndex, 7].Value = "โอนจ่าย";
            ws.Cells[rowIndex, 8].Value = "เบิกใช้";
            ws.Cells[rowIndex, 9].Value = "คืนเจ้าหนี้";
            ws.Cells[rowIndex, 10].Value = "ปรับปรุง";
            ws.Cells[rowIndex, 11].Value = "ตรวจนับ";
            ws.Cells[rowIndex, 12].Value = "คงเหลือ";

            using (var r = ws.Cells["B3:L3"])
            {
                r.Style.Font.Name = "Angsana New";
                r.Style.Font.Size = 14;
                r.Style.Font.Bold = true;
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));
            };

            rowIndex++;

            var yesterday = docDate.AddDays(-1);

            var strSql = $@"SELECT
                                PD_ID AS PdId, 
                                GROUP_ID AS GroupId,
	                            ISNULL(Balance.REMAIN, 0) as Banlance, 
	                            ISNULL(ReceiveProd.STOCK_QTY, 0) as Receive,
	                            ISNULL(TranferIn.STOCK_QTY, 0) as TranIn,
	                            ISNULL(TranferOut.STOCK_QTY, 0) as TranOut,
	                            ISNULL(CashSale.STOCK_QTY, 0) as CashSale,
	                            ISNULL(CreditSale.STOCK_QTY, 0) as CreditSale,
	                            ISNULL(ReturnSup.STOCK_QTY, 0) as ReturnSup,
	                            ISNULL(WithDraw.STOCK_QTY, 0) as WithDraw,
	                            ISNULL(Adjust.STOCK_QTY, 0) as Adjust,
	                            ISNULL(Balance.REMAIN, 0) +
                                ISNULL(ReceiveProd.STOCK_QTY, 0) +
                                ISNULL(TranferIn.STOCK_QTY, 0) -
                                ISNULL(TranferOut.STOCK_QTY, 0) -
                                ISNULL(CashSale.STOCK_QTY, 0) -
                                ISNULL(CreditSale.STOCK_QTY, 0) -
                                ISNULL(ReturnSup.STOCK_QTY, 0) -
                                ISNULL(ReturnOil.STOCK_QTY, 0) -
                                ISNULL(WithDraw.STOCK_QTY, 0) +
                                ISNULL(Adjust.STOCK_QTY, 0) as Remain
                            from(
		                        select distinct * from(
			                        select isd.PD_ID, p.UNIT_ID , p.GROUP_ID from INV_STOCK_DAILY(nolock) isd
			                        join MAS_PRODUCT p on p.PD_ID = isd.PD_ID
			                        where 
				                        STOCK_DATE = '{yesterday:yyyy-MM-dd}'
				                        and COMP_CODE = '{req.CompCode}' 
				                        and BRN_CODE = '{req.BrnCode}'   
										and p.GROUP_ID <> '0000'
			                        union 
			                        select dt.PD_ID, p.UNIT_ID , p.GROUP_ID from INV_RECEIVE_PROD_HD(nolock)hd 
			                        join INV_RECEIVE_PROD_DT(nolock)dt 
			                        on hd.BRN_CODE = dt.BRN_CODE 
				                        and hd.COMP_CODE = dt.COMP_CODE 
				                        and hd.LOC_CODE = dt.LOC_CODE
				                        and hd.DOC_NO = dt.DOC_NO
			                        join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
			                        where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
				                        and hd.COMP_CODE =  '{req.CompCode}'
				                        and hd.BRN_CODE = '{req.BrnCode}' 
                                        and hd.DOC_STATUS <> 'Cancel'
										and p.GROUP_ID <> '0000'
			                        union 
			                        select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
			                        on hd.BRN_CODE = dt.BRN_CODE 
				                        and hd.COMP_CODE = dt.COMP_CODE 
				                        and hd.LOC_CODE = dt.LOC_CODE
				                        and hd.DOC_NO = dt.DOC_NO
			                        join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
			                        where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
				                        and hd.COMP_CODE = '{req.CompCode}'
				                        and hd.BRN_CODE = '{req.BrnCode}'
                                        and hd.DOC_STATUS <> 'Cancel'
										and p.GROUP_ID <> '0000'
			                        union 
			                        select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_TRANOUT_HD(nolock)hd join INV_TRANOUT_DT(nolock)dt 
			                        on hd.BRN_CODE = dt.BRN_CODE 
				                        and hd.COMP_CODE = dt.COMP_CODE 
				                        and hd.LOC_CODE = dt.LOC_CODE
				                        and hd.DOC_NO = dt.DOC_NO
			                        join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
			                        where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
				                        and hd.COMP_CODE = '{req.CompCode}'
				                        and hd.BRN_CODE = '{req.BrnCode}'
                                        and hd.DOC_STATUS <> 'Cancel'
										and p.GROUP_ID <> '0000'
                                    union
                                    select dt.PD_ID,p.UNIT_ID , p.GROUP_ID  from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
		                            on hd.BRN_CODE = dt.BRN_CODE 
		                            and hd.COMP_CODE = dt.COMP_CODE 
		                            and hd.LOC_CODE = dt.LOC_CODE
		                            and hd.DOC_NO = dt.DOC_NO
                                    join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
		                            where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			                            and hd.COMP_CODE = '{req.CompCode}'
			                            and hd.BRN_CODE = '{req.BrnCode}'
			                            and hd.DOC_STATUS <> 'Cancel'
                                        and p.GROUP_ID <> '0000'
			                        union 
			                        select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
			                        on hd.BRN_CODE = dt.BRN_CODE 
				                        and hd.COMP_CODE = dt.COMP_CODE 
				                        and hd.LOC_CODE = dt.LOC_CODE
				                        and hd.DOC_NO = dt.DOC_NO
			                        join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
			                        where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
				                        and hd.COMP_CODE = '{req.CompCode}'
				                        and hd.BRN_CODE = '{req.BrnCode}'
                                        and hd.DOC_STATUS <> 'Cancel'
										and p.GROUP_ID <> '0000'
		                        ) ap 
	                        ) allProductId
                            outer apply(
		                        select REMAIN from INV_STOCK_DAILY(nolock)
		                        where PD_ID = allProductId.PD_ID
			                        and STOCK_DATE = '{yesterday:yyyy-MM-dd}'
			                        and COMP_CODE = '{req.CompCode}'
			                        and BRN_CODE = '{req.BrnCode}'
	                        ) Balance
                            outer apply(
		                        select SUM(dt.STOCK_QTY) STOCK_QTY 
		                        from INV_RECEIVE_PROD_HD(nolock)hd join INV_RECEIVE_PROD_DT(nolock)dt 
		                        on hd.BRN_CODE = dt.BRN_CODE 
		                        and hd.COMP_CODE = dt.COMP_CODE 
		                        and hd.LOC_CODE = dt.LOC_CODE
		                        and hd.DOC_NO = dt.DOC_NO
		                        where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			                        and hd.COMP_CODE = '{req.CompCode}' 
			                        and hd.BRN_CODE = '{req.BrnCode}'
			                        and dt.PD_ID = allProductId.PD_ID
			                        and hd.DOC_STATUS <> 'Cancel'
	                        ) ReceiveProd
                            outer apply(
		                        select SUM(dt.STOCK_QTY) STOCK_QTY 
		                        from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
		                        on hd.BRN_CODE = dt.BRN_CODE 
		                        and hd.COMP_CODE = dt.COMP_CODE 
		                        and hd.LOC_CODE = dt.LOC_CODE
		                        and hd.DOC_NO = dt.DOC_NO
		                        where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
								and hd.COMP_CODE = '{req.CompCode}' 
								and hd.BRN_CODE = '{req.BrnCode}'
								and dt.PD_ID = allProductId.PD_ID
								and hd.DOC_STATUS <> 'Cancel'
	                        ) TranferIn
                            outer apply(
		                        select SUM(dt.STOCK_QTY) STOCK_QTY 
		                        from INV_TRANOUT_HD(nolock)hd join INV_TRANOUT_DT(nolock)dt 
		                        on hd.BRN_CODE = dt.BRN_CODE 
		                        and hd.COMP_CODE = dt.COMP_CODE 
		                        and hd.LOC_CODE = dt.LOC_CODE
		                        and hd.DOC_NO = dt.DOC_NO
		                        where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
								and hd.COMP_CODE = '{req.CompCode}'  
								and hd.BRN_CODE = '{req.BrnCode}'
								and dt.PD_ID = allProductId.PD_ID
								and hd.DOC_STATUS <> 'Cancel'
	                        ) TranferOut
                            outer apply(
		                        select SUM(dt.STOCK_QTY) STOCK_QTY 
		                        from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
		                        on hd.BRN_CODE = dt.BRN_CODE 
		                        and hd.COMP_CODE = dt.COMP_CODE 
		                        and hd.LOC_CODE = dt.LOC_CODE
		                        and hd.DOC_NO = dt.DOC_NO
		                        where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
								and hd.COMP_CODE = '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}'
								and dt.PD_ID = allProductId.PD_ID
								and hd.DOC_STATUS <> 'Cancel'
	                        ) CashSale
                            outer apply(
		                    select SUM(dt.STOCK_QTY) STOCK_QTY 
		                    from SAL_CREDITSALE_HD(nolock)hd join SAL_CREDITSALE_DT(nolock)dt 
		                    on hd.BRN_CODE = dt.BRN_CODE 
			                and hd.COMP_CODE = dt.COMP_CODE 
							and hd.LOC_CODE = dt.LOC_CODE
							and hd.DOC_NO = dt.DOC_NO
							where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
							and hd.COMP_CODE = '{req.CompCode}' 
							and hd.BRN_CODE = '{req.BrnCode}'
							and dt.PD_ID = allProductId.PD_ID
							and hd.DOC_STATUS <> 'Cancel'
	                    ) CreditSale
                        outer apply(
		                    select SUM(dt.STOCK_QTY) STOCK_QTY 
		                    from INV_RETURN_SUP_HD(nolock)hd join INV_RETURN_SUP_DT(nolock)dt 
		                    on hd.BRN_CODE = dt.BRN_CODE 
							and hd.COMP_CODE = dt.COMP_CODE 
							and hd.LOC_CODE = dt.LOC_CODE
							and hd.DOC_NO = dt.DOC_NO
							where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
							and hd.COMP_CODE = '{req.CompCode}' 
							and hd.BRN_CODE = '{req.BrnCode}'
							and dt.PD_ID = allProductId.PD_ID
							and hd.DOC_STATUS <> 'Cancel'
	                    ) ReturnSup
                        outer apply(
		                    select SUM(dt.STOCK_QTY) STOCK_QTY 
		                    from INV_RETURN_OIL_HD(nolock)hd join INV_RETURN_OIL_DT(nolock)dt 
		                    on hd.BRN_CODE = dt.BRN_CODE 
							and hd.COMP_CODE = dt.COMP_CODE 
							and hd.LOC_CODE = dt.LOC_CODE
							and hd.DOC_NO = dt.DOC_NO
							where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
							and hd.COMP_CODE = '{req.CompCode}' 
							and hd.BRN_CODE = '{req.BrnCode}' 
							and dt.PD_ID = allProductId.PD_ID
							and hd.DOC_STATUS <> 'Cancel'
	                    ) ReturnOil
                        outer apply(
		                    select SUM(dt.STOCK_QTY) STOCK_QTY 
		                    from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
		                    on hd.BRN_CODE = dt.BRN_CODE 
							and hd.COMP_CODE = dt.COMP_CODE 
							and hd.LOC_CODE = dt.LOC_CODE
							and hd.DOC_NO = dt.DOC_NO
							where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
							and hd.COMP_CODE = '{req.CompCode}' 
							and hd.BRN_CODE = '{req.BrnCode}'
							and dt.PD_ID = allProductId.PD_ID
							and hd.DOC_STATUS <> 'Cancel'
	                    ) WithDraw
                        outer apply(
		                    select SUM(dt.STOCK_QTY) STOCK_QTY 
		                    from INV_ADJUST_HD(nolock)hd join INV_ADJUST_DT(nolock)dt 
		                    on hd.BRN_CODE = dt.BRN_CODE 
							and hd.COMP_CODE = dt.COMP_CODE 
							and hd.LOC_CODE = dt.LOC_CODE
							and hd.DOC_NO = dt.DOC_NO
							where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
							and hd.COMP_CODE = '{req.CompCode}'
							and hd.BRN_CODE = '{req.BrnCode}'
							and dt.PD_ID = allProductId.PD_ID
							and hd.DOC_STATUS <> 'Cancel'
	                    ) Adjust
						where not (ISNULL(Balance.REMAIN, 0) = 0 
						and ISNULL(ReceiveProd.STOCK_QTY, 0) = 0
						and ISNULL(TranferIn.STOCK_QTY, 0) = 0
						and ISNULL(TranferOut.STOCK_QTY, 0) = 0
						and ISNULL(CashSale.STOCK_QTY, 0) = 0
						and ISNULL(CreditSale.STOCK_QTY, 0) = 0
						and ISNULL(ReturnSup.STOCK_QTY, 0) = 0
						and ISNULL(WithDraw.STOCK_QTY, 0) = 0
						and ISNULL(Adjust.STOCK_QTY, 0) = 0)";

            string strCon = context.Database.GetConnectionString();
            var result = GetEntityFromSql<List<StockRemain>>(strCon, strSql);

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd) && result != null)
            {
                result = result.Where(x =>
                         string.Compare(x.PdId, req.ProductIdStart) >= 0
                      && string.Compare(x.PdId, req.ProductIdEnd) <= 0).ToList();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd) && result != null)
            {
                result = result.Where(x =>
                         string.Compare(x.GroupId, req.ProductGroupIdStart) >= 0
                      && string.Compare(x.GroupId, req.ProductGroupIdEnd) <= 0).ToList();
            }

            if (result != null && result.Count() > 0)
            {
                //var stockRemain = result.Where(x => x.Remain > 0).ToList();
                var stockRemain = result.ToList();
                var productGroupIds = stockRemain.Where(x => x.GroupId != "0000").OrderBy(x => x.GroupId).Select(p => p.GroupId).Distinct();
                var groupIds = stockRemain.Select(x => x.GroupId).ToList();
                var masProductGroups = context.MasProductGroups.Where(x => groupIds.Contains(x.GroupId)).ToList();
                var productIds = stockRemain.Select(x => x.PdId).ToList();
                var masProducts = context.MasProducts.Where(x => productIds.Contains(x.PdId)).ToList();
                int seqNo = 0;
                foreach (var productGroupId in productGroupIds)
                {
                    var productGroupName = masProductGroups.FirstOrDefault(x => x.GroupId == productGroupId).GroupName;
                    var products = masProducts.Where(x => x.GroupId == productGroupId).OrderBy(x => x.PdId).ToList();
                    int row = 1;

                    ws.Cells[rowIndex, 1].Value = $"กลุ่มสินค้า : {productGroupId} {productGroupName}";

                    var sumStockBanlance = 0m;
                    var sumReceiveStock = 0m;
                    var sumTraninStock = 0m;
                    var sumSale = 0m;
                    var sumIsFree = 0m;
                    var sumTranOutStock = 0m;
                    var sumWithdrawStock = 0m;
                    var sumRetuernSupStock = 0m;
                    var sumAdjustStock = 0m;
                    var sumAuditStock = 0m;
                    var sumRemain = 0m;

                    foreach (var product in products)
                    {
                        var stock = stockRemain.FirstOrDefault(x => x.PdId.Trim() == product.PdId.Trim());
                        var banlance = stock.Banlance;
                        var receive = stock.Receive;
                        var tranin = stock.TranIn;
                        var sale = stock.CashSale + stock.CreditSale;
                        var free = 0;
                        var tranout = stock.TranOut;
                        var withdraw = stock.WithDraw;
                        var returnSup = stock.ReturnSup;
                        var adjust = stock.Adjust;
                        var audit = 0;
                        var remain = stock.Remain;

                        ws.Cells[rowIndex + 1, 1].Value = $"{++seqNo}  {product.PdId} {product.PdName}";
                        ws.Cells[rowIndex + 1, 2].Value = banlance; //ยอดยก
                        ws.Cells[rowIndex + 1, 3].Value = receive; //รับสินค้า
                        ws.Cells[rowIndex + 1, 4].Value = tranin; //รับโอน
                        ws.Cells[rowIndex + 1, 5].Value = sale; //ขาย
                        ws.Cells[rowIndex + 1, 6].Value = free; //แถม
                        ws.Cells[rowIndex + 1, 7].Value = tranout; //โอน
                        ws.Cells[rowIndex + 1, 8].Value = withdraw; //เบิกใช้
                        ws.Cells[rowIndex + 1, 9].Value = returnSup;//คืนเจ้าหนี้
                        ws.Cells[rowIndex + 1, 10].Value = adjust; //ปรับปรุง
                        ws.Cells[rowIndex + 1, 11].Value = audit; //ตรวจนับ
                        ws.Cells[rowIndex + 1, 12].Value = remain; //คงเหลือ


                        sumStockBanlance += banlance;
                        sumReceiveStock += receive;
                        sumTraninStock += tranin;
                        sumSale += sale;
                        sumIsFree += free;
                        sumTranOutStock += tranout;
                        sumWithdrawStock += withdraw;
                        sumRetuernSupStock += returnSup;
                        sumAdjustStock += adjust;
                        sumAuditStock += audit;
                        sumRemain += remain;

                        rowIndex++;
                        row++;
                    }
                    rowIndex++;

                    ws.Cells[rowIndex, 1].Value = $"รวมกลุ่มสินค้า : {productGroupId} {productGroupName}";
                    ws.Cells[rowIndex, 1].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 2].Value = sumStockBanlance; //รวมยอดยก
                    ws.Cells[rowIndex, 2].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 3].Value = sumReceiveStock; //รวมรับสินค้า
                    ws.Cells[rowIndex, 3].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 4].Value = sumTraninStock; //รวมรับโอน
                    ws.Cells[rowIndex, 4].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 5].Value = sumSale; //รวมขาย
                    ws.Cells[rowIndex, 5].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 6].Value = sumIsFree; //รวมแถม
                    ws.Cells[rowIndex, 6].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 7].Value = sumTranOutStock; //รวมโอน
                    ws.Cells[rowIndex, 7].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 8].Value = sumWithdrawStock; //รวมเบิกใช้
                    ws.Cells[rowIndex, 8].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 9].Value = sumRetuernSupStock; //รวมคืนเจ้าหนี้
                    ws.Cells[rowIndex, 9].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 10].Value = sumAdjustStock; //รวมปรับปรุง
                    ws.Cells[rowIndex, 10].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 11].Value = sumAuditStock; //รวมตรวจนับ
                    ws.Cells[rowIndex, 11].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 12].Value = sumRemain; //รวมคงเหลือ
                    ws.Cells[rowIndex, 12].Style.Font.Bold = true;
                    rowIndex++;
                }
                ws.Cells[rowIndex, 1].Value = $"หมายเหตุ : รายการสินค้าที่ขีดเส้นใต้ คือ รายการสินค้าที่ระงับการใช้";
            }
            else
            {
                ws.Cells["A" + rowIndex].Value = $"ไม่พบข้อมูล";

                using (var r = ws.Cells["A" + rowIndex + ":L" + rowIndex])
                {
                    r.Merge = true;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                };
            }



            return new MemoryStream(pck.GetAsByteArray());
        }

        public async Task<ReportStockResponse> GetReportStockPDF(ReportStockRequest req)
        {
            var response = new ReportStockResponse();
            var docDate = req.DateFrom; // DateTime.ParseExact(, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var dateNow = DateTime.ParseExact(currentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var firstDayOfMonth = new DateTime(docDate.Year, docDate.Month, 1);
            var brn = await context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).FirstOrDefaultAsync();
            var company = await context.MasCompanies.Where(x => x.CompCode == req.CompCode).FirstOrDefaultAsync();
            var yesterday = docDate.AddDays(-1);
            var stockResponses = new List<ReportStockResponse.Stock>();

			var strSql = $@"SELECT
			                    PD_ID AS PdId, 
			                    GROUP_ID AS GroupId,
			                 ISNULL(Balance.REMAIN, 0) as Banlance, 
			                 ISNULL(ReceiveProd.STOCK_QTY, 0) as Receive,
			                 ISNULL(TranferIn.STOCK_QTY, 0) as TranIn,
			                 ISNULL(TranferOut.STOCK_QTY, 0) as TranOut,
			                 ISNULL(CashSale.STOCK_QTY, 0) as CashSale,
			                 ISNULL(CreditSale.STOCK_QTY, 0) as CreditSale,
			              ISNULL( FreeCashSale.STOCK_QTY,0) FreeCashSale,
			              ISNULL( FreeCreditSale.STOCK_QTY,0) FreeCreditSale,
			                 ISNULL(ReturnSup.STOCK_QTY, 0) as ReturnSup,
			                 ISNULL(WithDraw.STOCK_QTY, 0) as WithDraw,
			                 ISNULL(Adjust.STOCK_QTY, 0) as Adjust,
			                 ISNULL(Balance.REMAIN, 0) 
			                    + ISNULL(ReceiveProd.STOCK_QTY, 0) 
			                    + ISNULL(TranferIn.STOCK_QTY, 0) 
			                    - ISNULL(TranferOut.STOCK_QTY, 0) 
			                    - ISNULL(CashSale.STOCK_QTY, 0) 
			                    - ISNULL(CreditSale.STOCK_QTY, 0) 
			                    - ISNULL(FreeCashSale.STOCK_QTY, 0) 
			                    - ISNULL(FreeCreditSale.STOCK_QTY, 0) 
			                    - ISNULL(ReturnSup.STOCK_QTY, 0) 
			                    - ISNULL(ReturnOil.STOCK_QTY, 0) 
			                    - ISNULL(WithDraw.STOCK_QTY, 0) 
			                    + ISNULL(Adjust.STOCK_QTY, 0) as Remain
			                from(
			              select distinct * from(
								select isd.PD_ID, p.UNIT_ID , p.GROUP_ID from INV_STOCK_DAILY(nolock) isd
								join MAS_PRODUCT p on p.PD_ID = isd.PD_ID
								where 
								STOCK_DATE = '{yesterday:yyyy-MM-dd}'
								and COMP_CODE = '{req.CompCode}' 
								and BRN_CODE = '{req.BrnCode}' 
								and p.GROUP_ID <> '0000'
								union 
								select dt.PD_ID, p.UNIT_ID , p.GROUP_ID from INV_RECEIVE_PROD_HD(nolock)hd 
								join INV_RECEIVE_PROD_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
								and hd.COMP_CODE = dt.COMP_CODE 
								and hd.LOC_CODE = dt.LOC_CODE
								and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
								and p.GROUP_ID <> '0000'
								union 
								select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
								and hd.COMP_CODE = dt.COMP_CODE 
								and hd.LOC_CODE = dt.LOC_CODE
								and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
								and hd.COMP_CODE = '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}'
								and p.GROUP_ID <> '0000'
								union 
								select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_TRANOUT_HD(nolock)hd join INV_TRANOUT_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
								and hd.COMP_CODE = dt.COMP_CODE 
								and hd.LOC_CODE = dt.LOC_CODE
								and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
								and hd.COMP_CODE = '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}'
								and hd.DOC_STATUS <> 'Cancel'
								and p.GROUP_ID <> '0000'
								union
								select dt.PD_ID,p.UNIT_ID , p.GROUP_ID  from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
								and hd.COMP_CODE = dt.COMP_CODE 
								and hd.LOC_CODE = dt.LOC_CODE
								and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
								and hd.COMP_CODE = '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}'
								and hd.DOC_STATUS <> 'Cancel'
								and p.GROUP_ID <> '0000'
								union 
								select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
								and hd.COMP_CODE = dt.COMP_CODE 
								and hd.LOC_CODE = dt.LOC_CODE
								and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
								and hd.COMP_CODE = '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}'
								and hd.DOC_STATUS <> 'Cancel'
								and p.GROUP_ID <> '0000'
			              ) ap 
			             ) allProductId
			                outer apply(
			              select REMAIN from INV_STOCK_DAILY(nolock)
			              where PD_ID = allProductId.PD_ID
			               and STOCK_DATE = '{yesterday:yyyy-MM-dd}'
			               and COMP_CODE = '{req.CompCode}'
			               and BRN_CODE = '{req.BrnCode}'
			             ) Balance
			                outer apply(
			              select SUM(dt.STOCK_QTY) STOCK_QTY 
			              from INV_RECEIVE_PROD_HD(nolock)hd join INV_RECEIVE_PROD_DT(nolock)dt 
			              on hd.BRN_CODE = dt.BRN_CODE 
			              and hd.COMP_CODE = dt.COMP_CODE 
			              and hd.LOC_CODE = dt.LOC_CODE
			              and hd.DOC_NO = dt.DOC_NO
			              where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			               and hd.COMP_CODE = '{req.CompCode}' 
			               and hd.BRN_CODE = '{req.BrnCode}'
			               and dt.PD_ID = allProductId.PD_ID
			               and hd.DOC_STATUS <> 'Cancel'
			             ) ReceiveProd
			                outer apply(
			              select SUM(dt.STOCK_QTY) STOCK_QTY 
			              from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
			              on hd.BRN_CODE = dt.BRN_CODE 
			              and hd.COMP_CODE = dt.COMP_CODE 
			              and hd.LOC_CODE = dt.LOC_CODE
			              and hd.DOC_NO = dt.DOC_NO
			              where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			               and hd.COMP_CODE = '{req.CompCode}' 
			               and hd.BRN_CODE = '{req.BrnCode}'
			               and dt.PD_ID = allProductId.PD_ID
			               and hd.DOC_STATUS <> 'Cancel'
			             ) TranferIn
			                outer apply(
			              select SUM(dt.STOCK_QTY) STOCK_QTY 
			              from INV_TRANOUT_HD(nolock)hd join INV_TRANOUT_DT(nolock)dt 
			              on hd.BRN_CODE = dt.BRN_CODE 
			              and hd.COMP_CODE = dt.COMP_CODE 
			              and hd.LOC_CODE = dt.LOC_CODE
			              and hd.DOC_NO = dt.DOC_NO
			              where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			               and hd.COMP_CODE = '{req.CompCode}'  
			               and hd.BRN_CODE = '{req.BrnCode}'
			               and dt.PD_ID = allProductId.PD_ID
			               and hd.DOC_STATUS <> 'Cancel'
			             ) TranferOut
			                outer apply(
			              select SUM(dt.STOCK_QTY) STOCK_QTY 
			              from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
			              on hd.BRN_CODE = dt.BRN_CODE 
			              and hd.COMP_CODE = dt.COMP_CODE 
			              and hd.LOC_CODE = dt.LOC_CODE
			              and hd.DOC_NO = dt.DOC_NO
			              where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			               and hd.COMP_CODE = '{req.CompCode}'
			               and hd.BRN_CODE = '{req.BrnCode}'
			               and dt.PD_ID = allProductId.PD_ID
			               and hd.DOC_STATUS <> 'Cancel'
						   and dt.IS_FREE <> 1
			             ) CashSale
			                outer apply(
			          select SUM(dt.STOCK_QTY) STOCK_QTY 
			          from SAL_CREDITSALE_HD(nolock)hd join SAL_CREDITSALE_DT(nolock)dt 
			          on hd.BRN_CODE = dt.BRN_CODE 
			           and hd.COMP_CODE = dt.COMP_CODE 
			           and hd.LOC_CODE = dt.LOC_CODE
			           and hd.DOC_NO = dt.DOC_NO
			          where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			           and hd.COMP_CODE = '{req.CompCode}' 
			           and hd.BRN_CODE = '{req.BrnCode}'
			           and dt.PD_ID = allProductId.PD_ID
			           and hd.DOC_STATUS <> 'Cancel'
			                    and dt.IS_FREE <> 1
			         ) CreditSale
			         outer apply(
			          select SUM(dt.STOCK_QTY) STOCK_QTY 
			          from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
			          on hd.BRN_CODE = dt.BRN_CODE 
			          and hd.COMP_CODE = dt.COMP_CODE 
			          and hd.LOC_CODE = dt.LOC_CODE
			          and hd.DOC_NO = dt.DOC_NO
			          where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			           and hd.COMP_CODE = '{req.CompCode}'  
			           and hd.BRN_CODE = '{req.BrnCode}' 
			           and dt.PD_ID = allProductId.PD_ID
			           and hd.DOC_STATUS <> 'Cancel'
			           and dt.IS_FREE = 1
			         ) FreeCashSale
			         outer apply(
			          select SUM(dt.STOCK_QTY) STOCK_QTY 
			          from SAL_CREDITSALE_HD(nolock)hd join SAL_CREDITSALE_DT(nolock)dt 
			          on hd.BRN_CODE = dt.BRN_CODE 
			           and hd.COMP_CODE = dt.COMP_CODE 
			           and hd.LOC_CODE = dt.LOC_CODE
			           and hd.DOC_NO = dt.DOC_NO
			          where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			           and hd.COMP_CODE = '{req.CompCode}'  
			           and hd.BRN_CODE = '{req.BrnCode}' 			            
			           and dt.PD_ID = allProductId.PD_ID
			           and hd.DOC_STATUS <> 'Cancel'
			           and dt.IS_FREE = 1
			         ) FreeCreditSale
			            outer apply(
			          select SUM(dt.STOCK_QTY) STOCK_QTY 
			          from INV_RETURN_SUP_HD(nolock)hd join INV_RETURN_SUP_DT(nolock)dt 
			          on hd.BRN_CODE = dt.BRN_CODE 
			           and hd.COMP_CODE = dt.COMP_CODE 
			           and hd.LOC_CODE = dt.LOC_CODE
			           and hd.DOC_NO = dt.DOC_NO
			          where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			           and hd.COMP_CODE = '{req.CompCode}' 
			           and hd.BRN_CODE = '{req.BrnCode}'
			           and dt.PD_ID = allProductId.PD_ID
			           and hd.DOC_STATUS <> 'Cancel'
			         ) ReturnSup
			            outer apply(
			          select SUM(dt.STOCK_QTY) STOCK_QTY 
			          from INV_RETURN_OIL_HD(nolock)hd join INV_RETURN_OIL_DT(nolock)dt 
			          on hd.BRN_CODE = dt.BRN_CODE 
			           and hd.COMP_CODE = dt.COMP_CODE 
			           and hd.LOC_CODE = dt.LOC_CODE
			           and hd.DOC_NO = dt.DOC_NO
			          where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			           and hd.COMP_CODE = '{req.CompCode}' 
			           and hd.BRN_CODE = '{req.BrnCode}' 
			           and dt.PD_ID = allProductId.PD_ID
			           and hd.DOC_STATUS <> 'Cancel'
			         ) ReturnOil
			            outer apply(
			          select SUM(dt.STOCK_QTY) STOCK_QTY 
			          from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
			          on hd.BRN_CODE = dt.BRN_CODE 
			           and hd.COMP_CODE = dt.COMP_CODE 
			           and hd.LOC_CODE = dt.LOC_CODE
			           and hd.DOC_NO = dt.DOC_NO
			          where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			           and hd.COMP_CODE = '{req.CompCode}' 
			           and hd.BRN_CODE = '{req.BrnCode}'
			           and dt.PD_ID = allProductId.PD_ID
			           and hd.DOC_STATUS <> 'Cancel'
			         ) WithDraw
			            outer apply(
			          select SUM(dt.STOCK_QTY) STOCK_QTY 
			          from INV_ADJUST_HD(nolock)hd join INV_ADJUST_DT(nolock)dt 
			          on hd.BRN_CODE = dt.BRN_CODE 
			           and hd.COMP_CODE = dt.COMP_CODE 
			           and hd.LOC_CODE = dt.LOC_CODE
			           and hd.DOC_NO = dt.DOC_NO
			          where hd.DOC_DATE = '{docDate:yyyy-MM-dd}'
			           and hd.COMP_CODE = '{req.CompCode}'
			           and hd.BRN_CODE = '{req.BrnCode}'
			           and dt.PD_ID = allProductId.PD_ID
			           and hd.DOC_STATUS <> 'Cancel'
			         ) Adjust
			where not (ISNULL(Balance.REMAIN, 0) = 0 
			and ISNULL(ReceiveProd.STOCK_QTY, 0) = 0
			and ISNULL(TranferIn.STOCK_QTY, 0) = 0
			and ISNULL(TranferOut.STOCK_QTY, 0) = 0
			and ISNULL(CashSale.STOCK_QTY, 0) = 0
			and ISNULL(CreditSale.STOCK_QTY, 0) = 0
			and ISNULL( FreeCashSale.STOCK_QTY,0) = 0
			and ISNULL( FreeCreditSale.STOCK_QTY,0) = 0
			and ISNULL(ReturnSup.STOCK_QTY, 0) = 0
			and ISNULL(WithDraw.STOCK_QTY, 0) = 0
			and ISNULL(Adjust.STOCK_QTY, 0) = 0)
			";

			string strCon = context.Database.GetConnectionString();
			var result = GetEntityFromSql<List<StockRemain>>(strCon, strSql);

			if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd) && result != null)
			{
				result = result.Where(x =>
							string.Compare(x.PdId, req.ProductIdStart) >= 0
						&& string.Compare(x.PdId, req.ProductIdEnd) <= 0).ToList();
			}

			if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd) && result != null)
			{
				result = result.Where(x =>
							string.Compare(x.GroupId, req.ProductGroupIdStart) >= 0
						&& string.Compare(x.GroupId, req.ProductGroupIdEnd) <= 0).ToList();
			}

			if (result != null && result.Count() > 0)
            {
                var stockRemain = result.ToList();
                var productGroupIds = stockRemain.Where(x => x.GroupId != "0000").OrderBy(x => x.GroupId).Select(p => p.GroupId).Distinct();
                var groupIds = stockRemain.Select(x => x.GroupId).ToList();
                var masProductGroups = await context.MasProductGroups.Where(x => groupIds.Contains(x.GroupId)).ToListAsync();
                var productIds = stockRemain.Select(x => x.PdId).ToList();
                var masProducts = await context.MasProducts.Where(x => productIds.Contains(x.PdId)).ToListAsync();
                int seqNo = 0;

                foreach (var productGroupId in productGroupIds)
                {
                    var stockResponse = new ReportStockResponse.Stock();
                    var productGroupName = masProductGroups.FirstOrDefault(x => x.GroupId == productGroupId).GroupName;
                    var products = masProducts.Where(x => x.GroupId == productGroupId).OrderBy(x => x.PdId).ToList();
                    var sumStockBanlance = 0m;
                    var sumReceiveStock = 0m;
                    var sumTraninStock = 0m;
                    var sumSale = 0m;
                    var sumIsFree = 0m;
                    var sumTranOutStock = 0m;
                    var sumWithdrawStock = 0m;
                    var sumReturnSupStock = 0m;
                    var sumAdjustStock = 0m;
                    var sumAuditStock = 0m;
                    var sumRemain = 0m;

                    stockResponse.productGroupId = productGroupId;
                    stockResponse.productGroupName = productGroupName;
                    var stockDetails = new List<ReportStockResponse.StockDt>();

                    foreach (var product in products)
                    {
                        var stockDetail = new ReportStockResponse.StockDt();
                        var stock = stockRemain.FirstOrDefault(x => x.PdId.Trim() == product.PdId.Trim());
                        var banlance = stock.Banlance;
                        var receive = stock.Receive;
                        var tranin = stock.TranIn;
                        var sale = stock.CashSale + stock.CreditSale;
                        var free = stock.FreeCashSale + stock.FreeCreditSale;
                        var tranout = stock.TranOut;
                        var withdraw = stock.WithDraw;
                        var returnSup = stock.ReturnSup;
                        var adjust = stock.Adjust;
                        var audit = 0;
                        var remain = stock.Remain;

                        stockDetail.seqNo = ++seqNo;
                        stockDetail.productId = product.PdId;
                        stockDetail.productName = product.PdName;
                        stockDetail.stockBanlance = (decimal)banlance;
                        stockDetail.receiveStock = (decimal)receive;
                        stockDetail.traninStock = (decimal)tranin;
                        stockDetail.sale = (decimal)sale;
                        stockDetail.isFree = (decimal)free;
                        stockDetail.tranoutStock = (decimal)tranout;
                        stockDetail.withdrawStock = (decimal)withdraw;
                        stockDetail.returnSupStock = (decimal)returnSup;
                        stockDetail.adjustStock = (decimal)adjust;
                        stockDetail.auditStock = (decimal)audit;
                        stockDetail.balance = (decimal)remain;

                        sumStockBanlance += banlance;
                        sumReceiveStock += receive;
                        sumTraninStock += tranin;
                        sumSale += sale;
                        sumIsFree += free;
                        sumTranOutStock += tranout;
                        sumWithdrawStock += withdraw;
                        sumReturnSupStock += returnSup;
                        sumAdjustStock += adjust;
                        sumAuditStock += audit;
                        sumRemain += remain;

                        stockDetail.productGroupId = productGroupId;
                        stockDetail.productGroupName = productGroupName;
                        stockDetail.sumStockBanlance = (decimal)sumStockBanlance;
                        stockDetail.sumReceiveStock = (decimal)sumReceiveStock;
                        stockDetail.sumTraninStock = (decimal)sumTraninStock;
                        stockDetail.sumSale = (decimal)sumSale;
                        stockDetail.sumIsFree = (decimal)sumIsFree;
                        stockDetail.sumTranOutStock = (decimal)sumTranOutStock;
                        stockDetail.sumWithdrawStock = (decimal)sumWithdrawStock;
                        stockDetail.sumReturnSupStock = (decimal)sumReturnSupStock;
                        stockDetail.sumAdjustStock = (decimal)sumAdjustStock;
                        stockDetail.sumAuditStock = (decimal)sumAuditStock;
                        stockDetail.sumBalance = (decimal)sumRemain;
                        stockDetails.Add(stockDetail);
                        
                        stockResponse.stockDts = stockDetails;
                    }
                    stockResponses.Add(stockResponse);
                }
            }

            response.brnCode = brn.BrnCode;
            response.brnName = brn.BrnName;
            response.compName = company.CompName;
            response.compImage = company.CompImage;
            response.docDate = docDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            response.stocks = stockResponses;

            return response;
        }

        private static T GetEntityFromSql<T>(string pStrConnection, string pStrSql)
        {
            T result = default(T);
            using (DataTable dt = GetDataTable(pStrConnection, pStrSql))
            {
                result = GetEntityFromDataTable<T>(dt);
            }
            return result;
        }

        private static DataTable GetDataTable(string pStrConnection, string pStrSql)
        {
            pStrConnection = GetString(pStrConnection);
            pStrSql = GetString(pStrSql);
            if (0.Equals(pStrConnection.Length) || 0.Equals(pStrSql.Length))
            {
                return null;
            }
            DataTable result = new DataTable();
            using (var da = new SqlDataAdapter(pStrSql, pStrConnection))
            {
                da.Fill(result);
            }
            return result;
        }

        private static string GetString(object pInput, string pStrDefault = "")
        {
            if (pInput == null || Convert.IsDBNull(pInput))
            {
                return pStrDefault;
            }
            string result = (pInput?.ToString() ?? string.Empty).Trim();
            if (0.Equals(result.Length))
            {
                return pStrDefault;
            }
            return result;
        }

        private static T GetEntityFromDataTable<T>(DataTable pDataTable)
        {
            if (pDataTable == null || 0.Equals(pDataTable.Rows.Count))
            {
                return default(T);
            }
            Func<string, string> funcMapColName = null;
            funcMapColName = x =>
            {
                string[] arrSplit = x.Split("_");
                for (int i = 0; i < arrSplit.Length; i++)
                {
                    arrSplit[i] = arrSplit[i].ToLower();
                    char[] arrChar = arrSplit[i].ToCharArray();
                    arrChar[0] = arrChar[0].ToString().ToUpper()[0];
                    arrSplit[i] = new string(arrChar);
                }
                return string.Join(string.Empty, arrSplit);
            };
            List<DataColumn> arrOriginalCol = pDataTable.Columns.OfType<DataColumn>().ToList();
            foreach (DataColumn item in pDataTable.Columns)
            {
                item.ColumnName = funcMapColName(item.ColumnName);
            }
            T result = ConvertObject<T>(pDataTable);
            return result;
        }

        public static T ConvertObject<T>(object pObjInput)
        {
            if (pObjInput == null)
            {
                return default(T);
            }
            string strSerialized = JsonConvert.SerializeObject(pObjInput);
            T result = JsonConvert.DeserializeObject<T>(strSerialized);
            return result;
        }

        public ReportStockResponse GetReportStockTest(ReportStockRequest req)
        {
            var response = new ReportStockResponse();
            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var docDate = req.DateFrom; // DateTime.ParseExact(, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            response.brnCode = brn.BrnCode;
            response.brnName = brn.BrnName;
            response.docDate = docDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            //response.stocks = stocks;

            return response;
        }

        public ReportStockResponse GetMonthlyPDF(ReportStockRequest req)
        {
            var response = new ReportStockResponse();

            var dateFrom = req.DateFrom.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var dateTo = req.DateTo.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var yesterday = req.DateFrom.AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); //docDate.AddDays(-1);

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            //var dateNow = DateTime.ParseExact(currentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var firstDayOfMonth = new DateTime(req.DateFrom.Year, req.DateFrom.Month, 1);

            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            var stockResponses = new List<ReportStockResponse.Stock>();

            var strSql = $@"SELECT
                        PD_ID AS PdId, 
                        GROUP_ID AS GroupId,
	                    ISNULL(Balance.REMAIN, 0) as Banlance, 
	                    ISNULL(ReceiveProd.STOCK_QTY, 0) as Receive,
	                    ISNULL(TranferIn.STOCK_QTY, 0) as TranIn,
	                    ISNULL(TranferOut.STOCK_QTY, 0) as TranOut,
	                    ISNULL(CashSale.STOCK_QTY, 0) as CashSale,
	                    ISNULL(CreditSale.STOCK_QTY, 0) as CreditSale,
		                ISNULL( FreeCashSale.STOCK_QTY,0) FreeCashSale,
		                ISNULL( FreeCreditSale.STOCK_QTY,0) FreeCreditSale,
	                    ISNULL(ReturnSup.STOCK_QTY, 0) as ReturnSup,
	                    ISNULL(WithDraw.STOCK_QTY, 0) as WithDraw,
	                    ISNULL(Adjust.STOCK_QTY, 0) as Adjust,
	                    ISNULL(Balance.REMAIN, 0) 
                        + ISNULL(ReceiveProd.STOCK_QTY, 0) 
                        + ISNULL(TranferIn.STOCK_QTY, 0) 
                        - ISNULL(TranferOut.STOCK_QTY, 0) 
                        - ISNULL(CashSale.STOCK_QTY, 0) 
                        - ISNULL(CreditSale.STOCK_QTY, 0) 
                        - ISNULL(FreeCashSale.STOCK_QTY, 0) 
                        - ISNULL(FreeCreditSale.STOCK_QTY, 0) 
                        - ISNULL(ReturnSup.STOCK_QTY, 0) 
                        - ISNULL(ReturnOil.STOCK_QTY, 0) 
                        - ISNULL(WithDraw.STOCK_QTY, 0) 
                        + ISNULL(Adjust.STOCK_QTY, 0) as Remain
                    from(
		                select distinct * from(
			                select isd.PD_ID, p.UNIT_ID , p.GROUP_ID from INV_STOCK_DAILY(nolock) isd
			                join MAS_PRODUCT p on p.PD_ID = isd.PD_ID
			                where 
				                STOCK_DATE = '{yesterday}'
				                and COMP_CODE = '{req.CompCode}' 
				                and BRN_CODE = '{req.BrnCode}' 
								and p.GROUP_ID <> '0000'
			                union 
								select dt.PD_ID, p.UNIT_ID , p.GROUP_ID from INV_RECEIVE_PROD_HD(nolock)hd 
								join INV_RECEIVE_PROD_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
									and hd.COMP_CODE = dt.COMP_CODE 
									and hd.LOC_CODE = dt.LOC_CODE
									and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
								and p.GROUP_ID <> '0000'
			                union 
								select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
									and hd.COMP_CODE = dt.COMP_CODE 
									and hd.LOC_CODE = dt.LOC_CODE
									and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
								and p.GROUP_ID <> '0000'
                            union 
	                            select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_TRANOUT_HD(nolock)hd join INV_TRANOUT_DT(nolock)dt 
	                            on hd.BRN_CODE = dt.BRN_CODE 
		                            and hd.COMP_CODE = dt.COMP_CODE 
		                            and hd.LOC_CODE = dt.LOC_CODE
		                            and hd.DOC_NO = dt.DOC_NO
	                            join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
								and p.GROUP_ID <> '0000'
                            union
                                select dt.PD_ID,p.UNIT_ID , p.GROUP_ID  from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
	                            on hd.BRN_CODE = dt.BRN_CODE 
	                            and hd.COMP_CODE = dt.COMP_CODE 
	                            and hd.LOC_CODE = dt.LOC_CODE
	                            and hd.DOC_NO = dt.DOC_NO
                                join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
                                and p.GROUP_ID <> '0000'
	                        union 
	                            select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
	                            on hd.BRN_CODE = dt.BRN_CODE 
		                            and hd.COMP_CODE = dt.COMP_CODE 
		                            and hd.LOC_CODE = dt.LOC_CODE
		                            and hd.DOC_NO = dt.DOC_NO
	                            join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
								and p.GROUP_ID <> '0000'
		                ) ap 
	                ) allProductId
                    outer apply(
		                select REMAIN from INV_STOCK_DAILY(nolock)
		                where PD_ID = allProductId.PD_ID
			                and STOCK_DATE = '{yesterday}'
			                and COMP_CODE = '{req.CompCode}'
			                and BRN_CODE = '{req.BrnCode}'
	                ) Balance
                    outer apply(
		                select SUM(dt.STOCK_QTY) STOCK_QTY 
		                from INV_RECEIVE_PROD_HD(nolock)hd join INV_RECEIVE_PROD_DT(nolock)dt 
		                on hd.BRN_CODE = dt.BRN_CODE 
		                and hd.COMP_CODE = dt.COMP_CODE 
		                and hd.LOC_CODE = dt.LOC_CODE
		                and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
	                ) ReceiveProd
                    outer apply(
		                select SUM(dt.STOCK_QTY) STOCK_QTY 
		                from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
		                on hd.BRN_CODE = dt.BRN_CODE 
		                and hd.COMP_CODE = dt.COMP_CODE 
		                and hd.LOC_CODE = dt.LOC_CODE
		                and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
	                ) TranferIn
                    outer apply(
		                select SUM(dt.STOCK_QTY) STOCK_QTY 
		                from INV_TRANOUT_HD(nolock)hd join INV_TRANOUT_DT(nolock)dt 
		                on hd.BRN_CODE = dt.BRN_CODE 
		                and hd.COMP_CODE = dt.COMP_CODE 
		                and hd.LOC_CODE = dt.LOC_CODE
		                and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
	                ) TranferOut
                    outer apply(
		                select SUM(dt.STOCK_QTY) STOCK_QTY 
		                from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
		                on hd.BRN_CODE = dt.BRN_CODE 
		                and hd.COMP_CODE = dt.COMP_CODE 
		                and hd.LOC_CODE = dt.LOC_CODE
		                and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
                        and dt.IS_FREE <> 1
	                ) CashSale
                    outer apply(
						select SUM(dt.STOCK_QTY) STOCK_QTY 
						from SAL_CREDITSALE_HD(nolock)hd join SAL_CREDITSALE_DT(nolock)dt 
						on hd.BRN_CODE = dt.BRN_CODE 
						and hd.COMP_CODE = dt.COMP_CODE 
						and hd.LOC_CODE = dt.LOC_CODE
						and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
						and dt.PD_ID = allProductId.PD_ID
						and dt.IS_FREE <> 1
					) CreditSale
					outer apply(
						select SUM(dt.STOCK_QTY) STOCK_QTY 
						from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
						on hd.BRN_CODE = dt.BRN_CODE 
						and hd.COMP_CODE = dt.COMP_CODE 
						and hd.LOC_CODE = dt.LOC_CODE
						and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}'  
			            and dt.PD_ID = allProductId.PD_ID
			            and dt.IS_FREE = 1
	            ) FreeCashSale
	            outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from SAL_CREDITSALE_HD(nolock)hd join SAL_CREDITSALE_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 			            
			            and dt.PD_ID = allProductId.PD_ID
			            and dt.IS_FREE = 1
	            ) FreeCreditSale
                outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from INV_RETURN_SUP_HD(nolock)hd join INV_RETURN_SUP_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
			            and hd.DOC_STATUS <> 'Cancel'
	            ) ReturnSup
                outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from INV_RETURN_OIL_HD(nolock)hd join INV_RETURN_OIL_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
			            and hd.DOC_STATUS <> 'Cancel'
	            ) ReturnOil
                outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
			            and hd.DOC_STATUS <> 'Cancel'
	            ) WithDraw
                outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from INV_ADJUST_HD(nolock)hd join INV_ADJUST_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
			            and hd.DOC_STATUS <> 'Cancel'
	            ) Adjust
				where not (ISNULL(Balance.REMAIN, 0) = 0 
				and ISNULL(ReceiveProd.STOCK_QTY, 0) = 0
				and ISNULL(TranferIn.STOCK_QTY, 0) = 0
				and ISNULL(TranferOut.STOCK_QTY, 0) = 0
				and ISNULL(CashSale.STOCK_QTY, 0) = 0
				and ISNULL(CreditSale.STOCK_QTY, 0) = 0
				and ISNULL( FreeCashSale.STOCK_QTY,0) = 0
				and ISNULL( FreeCreditSale.STOCK_QTY,0) = 0
				and ISNULL(ReturnSup.STOCK_QTY, 0) = 0
				and ISNULL(WithDraw.STOCK_QTY, 0) = 0
				and ISNULL(Adjust.STOCK_QTY, 0) = 0)";

            string strCon = context.Database.GetConnectionString();
            var result = GetEntityFromSql<List<StockRemain>>(strCon, strSql);

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd) && result != null)
            {
                result = result.Where(x =>
                            string.Compare(x.PdId, req.ProductIdStart) >= 0
                        && string.Compare(x.PdId, req.ProductIdEnd) <= 0).ToList();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd) && result != null)
            {
                result = result.Where(x =>
                            string.Compare(x.GroupId, req.ProductGroupIdStart) >= 0
                        && string.Compare(x.GroupId, req.ProductGroupIdEnd) <= 0).ToList();
            }

            if (result != null && result.Count() > 0)
            {
                //var stockRemain = result.Where(x => x.Remain > 0).ToList();
                var stockRemain = result.ToList();
                var productGroupIds = stockRemain.Where(x => x.GroupId != "0000").OrderBy(x => x.GroupId).Select(p => p.GroupId).Distinct();
                var groupIds = stockRemain.Select(x => x.GroupId).ToList();
                var masProductGroups = context.MasProductGroups.Where(x => groupIds.Contains(x.GroupId)).ToList();
                var productIds = stockRemain.Select(x => x.PdId).ToList();
                var masProducts = context.MasProducts.Where(x => productIds.Contains(x.PdId)).ToList();
                int seqNo = 0;

                foreach (var productGroupId in productGroupIds)
                {
                    var stockResponse = new ReportStockResponse.Stock();
                    var productGroupName = masProductGroups.FirstOrDefault(x => x.GroupId == productGroupId).GroupName;
                    var products = masProducts.Where(x => x.GroupId == productGroupId).OrderBy(x => x.PdId).ToList();
                    var sumStockBanlance = 0m;
                    var sumReceiveStock = 0m;
                    var sumTraninStock = 0m;
                    var sumSale = 0m;
                    var sumIsFree = 0m;
                    var sumTranOutStock = 0m;
                    var sumWithdrawStock = 0m;
                    var sumReturnSupStock = 0m;
                    var sumAdjustStock = 0m;
                    var sumAuditStock = 0m;
                    var sumRemain = 0m;

                    stockResponse.productGroupId = productGroupId;
                    stockResponse.productGroupName = productGroupName;
                    var stockDetails = new List<ReportStockResponse.StockDt>();

                    foreach (var product in products)
                    {
                        var stockDetail = new ReportStockResponse.StockDt();
                        var stock = stockRemain.FirstOrDefault(x => x.PdId.Trim() == product.PdId.Trim());
                        var banlance = stock.Banlance;
                        var receive = stock.Receive;
                        var tranin = stock.TranIn;
                        var sale = stock.CashSale + stock.CreditSale;
                        var free = stock.FreeCashSale + stock.FreeCreditSale;
                        var tranout = stock.TranOut;
                        var withdraw = stock.WithDraw;
                        var returnSup = stock.ReturnSup;
                        var adjust = stock.Adjust;
                        var audit = 0;
                        var remain = stock.Remain;

                        stockDetail.seqNo = ++seqNo;
                        stockDetail.productId = product.PdId;
                        stockDetail.productName = product.PdName;
                        stockDetail.stockBanlance = (decimal)banlance;
                        stockDetail.receiveStock = (decimal)receive;
                        stockDetail.traninStock = (decimal)tranin;
                        stockDetail.sale = (decimal)sale;
                        stockDetail.isFree = (decimal)free;
                        stockDetail.tranoutStock = (decimal)tranout;
                        stockDetail.withdrawStock = (decimal)withdraw;
                        stockDetail.returnSupStock = (decimal)returnSup;
                        stockDetail.adjustStock = (decimal)adjust;
                        stockDetail.auditStock = (decimal)audit;
                        stockDetail.balance = (decimal)remain;

                        sumStockBanlance += banlance;
                        sumReceiveStock += receive;
                        sumTraninStock += tranin;
                        sumSale += sale;
                        sumIsFree += free;
                        sumTranOutStock += tranout;
                        sumWithdrawStock += withdraw;
                        sumReturnSupStock += returnSup;
                        sumAdjustStock += adjust;
                        sumAuditStock += audit;
                        sumRemain += remain;

                        stockDetail.productGroupId = productGroupId;
                        stockDetail.productGroupName = productGroupName;
                        stockDetail.sumStockBanlance = (decimal)sumStockBanlance;
                        stockDetail.sumReceiveStock = (decimal)sumReceiveStock;
                        stockDetail.sumTraninStock = (decimal)sumTraninStock;
                        stockDetail.sumSale = (decimal)sumSale;
                        stockDetail.sumIsFree = (decimal)sumIsFree;
                        stockDetail.sumTranOutStock = (decimal)sumTranOutStock;
                        stockDetail.sumWithdrawStock = (decimal)sumWithdrawStock;
                        stockDetail.sumReturnSupStock = (decimal)sumReturnSupStock;
                        stockDetail.sumAdjustStock = (decimal)sumAdjustStock;
                        stockDetail.sumAuditStock = (decimal)sumAuditStock;
                        stockDetail.sumBalance = (decimal)sumRemain;
                        stockDetails.Add(stockDetail);
                        stockResponse.stockDts = stockDetails;
                    }
                    stockResponses.Add(stockResponse);
                }
            }

            response.brnCode = brn.BrnCode;
            response.brnName = brn.BrnName;
            response.compName = company.CompName;
            response.compImage = company.CompImage;
            response.dateFrom = req.DateFrom.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            response.dateTo = req.DateTo.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            response.stocks = stockResponses;

            return response;
        }

        public MemoryStream GetMonthlyExcel(ReportStockRequest req)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();

            var dateFrom = req.DateFrom.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var dateTo = req.DateTo.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var yesterday = req.DateFrom.AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); //docDate.AddDays(-1);

            //var docDate = DateTime.ParseExact(req.DocDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            //var dateNow = DateTime.ParseExact(currentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var firstDayOfMonth = new DateTime(req.DateFrom.Year, req.DateFrom.Month, 1);

            var brn = context.MasBranches.Where(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode).AsNoTracking().FirstOrDefault();
            var company = context.MasCompanies.FirstOrDefault(x => x.CompCode == req.CompCode);

            //var stockResponses = new List<ReportStockResponse.Stock>();

            var strSql = $@"SELECT
                        PD_ID AS PdId, 
                        GROUP_ID AS GroupId,
	                    ISNULL(Balance.REMAIN, 0) as Banlance, 
	                    ISNULL(ReceiveProd.STOCK_QTY, 0) as Receive,
	                    ISNULL(TranferIn.STOCK_QTY, 0) as TranIn,
	                    ISNULL(TranferOut.STOCK_QTY, 0) as TranOut,
	                    ISNULL(CashSale.STOCK_QTY, 0) as CashSale,
	                    ISNULL(CreditSale.STOCK_QTY, 0) as CreditSale,
		                ISNULL( FreeCashSale.STOCK_QTY,0) FreeCashSale,
		                ISNULL( FreeCreditSale.STOCK_QTY,0) FreeCreditSale,
	                    ISNULL(ReturnSup.STOCK_QTY, 0) as ReturnSup,
	                    ISNULL(WithDraw.STOCK_QTY, 0) as WithDraw,
	                    ISNULL(Adjust.STOCK_QTY, 0) as Adjust,
	                    ISNULL(Balance.REMAIN, 0) 
                        + ISNULL(ReceiveProd.STOCK_QTY, 0) 
                        + ISNULL(TranferIn.STOCK_QTY, 0) 
                        - ISNULL(TranferOut.STOCK_QTY, 0) 
                        - ISNULL(CashSale.STOCK_QTY, 0) 
                        - ISNULL(CreditSale.STOCK_QTY, 0) 
                        - ISNULL(FreeCashSale.STOCK_QTY, 0) 
                        - ISNULL(FreeCreditSale.STOCK_QTY, 0) 
                        - ISNULL(ReturnSup.STOCK_QTY, 0) 
                        - ISNULL(ReturnOil.STOCK_QTY, 0) 
                        - ISNULL(WithDraw.STOCK_QTY, 0) 
                        + ISNULL(Adjust.STOCK_QTY, 0) as Remain
                    from(
		                select distinct * from(
			                select isd.PD_ID, p.UNIT_ID , p.GROUP_ID from INV_STOCK_DAILY(nolock) isd
			                join MAS_PRODUCT p on p.PD_ID = isd.PD_ID
			                where 
				                STOCK_DATE = '{yesterday}'
				                and COMP_CODE = '{req.CompCode}' 
				                and BRN_CODE = '{req.BrnCode}' 
			                union 
								select dt.PD_ID, p.UNIT_ID , p.GROUP_ID from INV_RECEIVE_PROD_HD(nolock)hd 
								join INV_RECEIVE_PROD_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
									and hd.COMP_CODE = dt.COMP_CODE 
									and hd.LOC_CODE = dt.LOC_CODE
									and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
			                union 
								select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
								on hd.BRN_CODE = dt.BRN_CODE 
									and hd.COMP_CODE = dt.COMP_CODE 
									and hd.LOC_CODE = dt.LOC_CODE
									and hd.DOC_NO = dt.DOC_NO
								join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
                            union 
	                            select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_TRANOUT_HD(nolock)hd join INV_TRANOUT_DT(nolock)dt 
	                            on hd.BRN_CODE = dt.BRN_CODE 
		                            and hd.COMP_CODE = dt.COMP_CODE 
		                            and hd.LOC_CODE = dt.LOC_CODE
		                            and hd.DOC_NO = dt.DOC_NO
	                            join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
                            union
                                select dt.PD_ID,p.UNIT_ID , p.GROUP_ID  from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
	                            on hd.BRN_CODE = dt.BRN_CODE 
	                            and hd.COMP_CODE = dt.COMP_CODE 
	                            and hd.LOC_CODE = dt.LOC_CODE
	                            and hd.DOC_NO = dt.DOC_NO
                                join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
                                and p.GROUP_ID <> '0000'
	                        union 
	                            select dt.PD_ID,p.UNIT_ID , p.GROUP_ID from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
	                            on hd.BRN_CODE = dt.BRN_CODE 
		                            and hd.COMP_CODE = dt.COMP_CODE 
		                            and hd.LOC_CODE = dt.LOC_CODE
		                            and hd.DOC_NO = dt.DOC_NO
	                            join MAS_PRODUCT p on p.PD_ID = dt.PD_ID
								where hd.DOC_STATUS <> 'Cancel'
								and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
								and hd.COMP_CODE =  '{req.CompCode}'
								and hd.BRN_CODE = '{req.BrnCode}' 
		                ) ap 
	                ) allProductId
                    outer apply(
		                select REMAIN from INV_STOCK_DAILY(nolock)
		                where PD_ID = allProductId.PD_ID
			                and STOCK_DATE = '{yesterday}'
			                and COMP_CODE = '{req.CompCode}'
			                and BRN_CODE = '{req.BrnCode}'
	                ) Balance
                    outer apply(
		                select SUM(dt.STOCK_QTY) STOCK_QTY 
		                from INV_RECEIVE_PROD_HD(nolock)hd join INV_RECEIVE_PROD_DT(nolock)dt 
		                on hd.BRN_CODE = dt.BRN_CODE 
		                and hd.COMP_CODE = dt.COMP_CODE 
		                and hd.LOC_CODE = dt.LOC_CODE
		                and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
	                ) ReceiveProd
                    outer apply(
		                select SUM(dt.STOCK_QTY) STOCK_QTY 
		                from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
		                on hd.BRN_CODE = dt.BRN_CODE 
		                and hd.COMP_CODE = dt.COMP_CODE 
		                and hd.LOC_CODE = dt.LOC_CODE
		                and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
	                ) TranferIn
                    outer apply(
		                select SUM(dt.STOCK_QTY) STOCK_QTY 
		                from INV_TRANOUT_HD(nolock)hd join INV_TRANOUT_DT(nolock)dt 
		                on hd.BRN_CODE = dt.BRN_CODE 
		                and hd.COMP_CODE = dt.COMP_CODE 
		                and hd.LOC_CODE = dt.LOC_CODE
		                and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
	                ) TranferOut
                    outer apply(
		                select SUM(dt.STOCK_QTY) STOCK_QTY 
		                from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
		                on hd.BRN_CODE = dt.BRN_CODE 
		                and hd.COMP_CODE = dt.COMP_CODE 
		                and hd.LOC_CODE = dt.LOC_CODE
		                and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
                        and dt.IS_FREE <> 1
	                ) CashSale
                    outer apply(
						select SUM(dt.STOCK_QTY) STOCK_QTY 
						from SAL_CREDITSALE_HD(nolock)hd join SAL_CREDITSALE_DT(nolock)dt 
						on hd.BRN_CODE = dt.BRN_CODE 
						and hd.COMP_CODE = dt.COMP_CODE 
						and hd.LOC_CODE = dt.LOC_CODE
						and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
						and dt.PD_ID = allProductId.PD_ID
						and dt.IS_FREE <> 1
					) CreditSale
					outer apply(
						select SUM(dt.STOCK_QTY) STOCK_QTY 
						from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
						on hd.BRN_CODE = dt.BRN_CODE 
						and hd.COMP_CODE = dt.COMP_CODE 
						and hd.LOC_CODE = dt.LOC_CODE
						and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}'  
			            and dt.PD_ID = allProductId.PD_ID
			            and dt.IS_FREE = 1
	            ) FreeCashSale
	            outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from SAL_CREDITSALE_HD(nolock)hd join SAL_CREDITSALE_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 			            
			            and dt.PD_ID = allProductId.PD_ID
			            and dt.IS_FREE = 1
	            ) FreeCreditSale
                outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from INV_RETURN_SUP_HD(nolock)hd join INV_RETURN_SUP_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
			            and hd.DOC_STATUS <> 'Cancel'
	            ) ReturnSup
                outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from INV_RETURN_OIL_HD(nolock)hd join INV_RETURN_OIL_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
			            and hd.DOC_STATUS <> 'Cancel'
	            ) ReturnOil
                outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
			            and hd.DOC_STATUS <> 'Cancel'
	            ) WithDraw
                outer apply(
		            select SUM(dt.STOCK_QTY) STOCK_QTY 
		            from INV_ADJUST_HD(nolock)hd join INV_ADJUST_DT(nolock)dt 
		            on hd.BRN_CODE = dt.BRN_CODE 
			            and hd.COMP_CODE = dt.COMP_CODE 
			            and hd.LOC_CODE = dt.LOC_CODE
			            and hd.DOC_NO = dt.DOC_NO
						where hd.DOC_STATUS <> 'Cancel'
						and hd.DOC_DATE between '{dateFrom}' and '{dateTo}'
						and hd.COMP_CODE =  '{req.CompCode}'
						and hd.BRN_CODE = '{req.BrnCode}' 
			            and dt.PD_ID = allProductId.PD_ID
			            and hd.DOC_STATUS <> 'Cancel'
	            ) Adjust
				where not (ISNULL(Balance.REMAIN, 0) = 0 
				and ISNULL(ReceiveProd.STOCK_QTY, 0) = 0
				and ISNULL(TranferIn.STOCK_QTY, 0) = 0
				and ISNULL(TranferOut.STOCK_QTY, 0) = 0
				and ISNULL(CashSale.STOCK_QTY, 0) = 0
				and ISNULL(CreditSale.STOCK_QTY, 0) = 0
				and ISNULL(FreeCashSale.STOCK_QTY,0) = 0
				and ISNULL(FreeCreditSale.STOCK_QTY,0) = 0
				and ISNULL(ReturnSup.STOCK_QTY, 0) = 0
				and ISNULL(WithDraw.STOCK_QTY, 0) = 0
				and ISNULL(Adjust.STOCK_QTY, 0) = 0)";

            string strCon = context.Database.GetConnectionString();
            var result = GetEntityFromSql<List<StockRemain>>(strCon, strSql);

            if (!string.IsNullOrEmpty(req.ProductIdStart) && !string.IsNullOrEmpty(req.ProductIdEnd) && result != null)
            {
                result = result.Where(x =>
                         string.Compare(x.PdId, req.ProductIdStart) >= 0
                      && string.Compare(x.PdId, req.ProductIdEnd) <= 0).ToList();
            }

            if (!string.IsNullOrEmpty(req.ProductGroupIdStart) && !string.IsNullOrEmpty(req.ProductGroupIdEnd) && result != null)
            {
                result = result.Where(x =>
                         string.Compare(x.GroupId, req.ProductGroupIdStart) >= 0
                      && string.Compare(x.GroupId, req.ProductGroupIdEnd) <= 0).ToList();
            }




            var ws = pck.Workbook.Worksheets.Add("sheet1");
            ws.Cells.Style.Font.Name = "Angsana New";
            ws.Cells.Style.Font.Size = 14;
            ws.Cells["A1"].Value = $"{company.CompName}";

            using (var r = ws.Cells["A1:L1"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                r.Style.Font.Bold = true;
            };



            using (var r = ws.Cells["A1:L1"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                r.Style.Font.Bold = true;
            };


            ws.Cells["A2"].Value = $"สถานีบริการ : {brn.BrnCode} {brn.BrnName}";
            ws.Cells["A2"].Style.Font.Size = 12;

            ws.Cells["B2"].Value = $"สรุปการเคลื่อนไหวสินค้า";
            using (var r = ws.Cells["B2:G2"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                r.Style.Font.Bold = true;
            };

            ws.Cells["H2"].Value = $"ตั้งแต่วันที่ {req.DateFrom.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} ถึง {req.DateTo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

            using (var r = ws.Cells["H2:L2"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                r.Style.Font.Bold = true;
            };

            var rowIndex = 3;
            ws.Cells[rowIndex, 1].Value = "รหัสสินค้า/รายการ";
            ws.Cells[rowIndex, 2].Value = "ยอดยกมา";
            ws.Cells[rowIndex, 3].Value = "รับสินค้า";
            ws.Cells[rowIndex, 4].Value = "รับโอน";
            ws.Cells[rowIndex, 5].Value = "ขาย";
            ws.Cells[rowIndex, 6].Value = "แถม";
            ws.Cells[rowIndex, 7].Value = "โอน";
            ws.Cells[rowIndex, 8].Value = "เบิกใช้";
            ws.Cells[rowIndex, 9].Value = "คืนเจ้าหนี้";
            ws.Cells[rowIndex, 10].Value = "ปรับปรุง";
            ws.Cells[rowIndex, 11].Value = "ตรวจนับ";
            ws.Cells[rowIndex, 12].Value = "คงเหลือ";

            using (var r = ws.Cells["B3:L3"])
            {
                r.Style.Font.Name = "Angsana New";
                r.Style.Font.Size = 14;
                r.Style.Font.Bold = true;
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));
            };

            rowIndex++;



            if (result != null && result.Count() > 0)
            {
                //var stockRemain = result.Where(x => x.Remain > 0).ToList();
                var stockRemain = result.ToList();
                var productGroupIds = stockRemain.Where(x => x.GroupId != "0000").OrderBy(x => x.GroupId).Select(p => p.GroupId).Distinct();
                var groupIds = stockRemain.Select(x => x.GroupId).ToList();
                var masProductGroups = context.MasProductGroups.Where(x => groupIds.Contains(x.GroupId)).ToList();
                var productIds = stockRemain.Select(x => x.PdId).ToList();
                var masProducts = context.MasProducts.Where(x => productIds.Contains(x.PdId)).ToList();
                int seqNo = 0;

                foreach (var productGroupId in productGroupIds)
                {
                    var productGroupName = masProductGroups.FirstOrDefault(x => x.GroupId == productGroupId).GroupName;
                    var products = masProducts.Where(x => x.GroupId == productGroupId).OrderBy(x => x.PdId).ToList();


                    ws.Cells[rowIndex, 1].Value = $"กลุ่มสินค้า : {productGroupId} {productGroupName}";

                    var sumStockBanlance = 0m;
                    var sumReceiveStock = 0m;
                    var sumTraninStock = 0m;
                    var sumSale = 0m;
                    var sumIsFree = 0m;
                    var sumTranOutStock = 0m;
                    var sumWithdrawStock = 0m;
                    var sumRetuernSupStock = 0m;
                    var sumAdjustStock = 0m;
                    var sumAuditStock = 0m;
                    var sumRemain = 0m;

                    foreach (var product in products)
                    {
                        var stock = stockRemain.FirstOrDefault(x => x.PdId.Trim() == product.PdId.Trim());
                        var banlance = stock.Banlance;
                        var receive = stock.Receive;
                        var tranin = stock.TranIn;
                        var sale = stock.CashSale + stock.CreditSale;
                        var free = 0;
                        var tranout = stock.TranOut;
                        var withdraw = stock.WithDraw;
                        var returnSup = stock.ReturnSup;
                        var adjust = stock.Adjust;
                        var audit = 0;
                        var remain = stock.Remain;

                        ws.Cells[rowIndex + 1, 1].Value = $"{++seqNo}  {product.PdId} {product.PdName}";
                        ws.Cells[rowIndex + 1, 2].Value = banlance; //ยอดยก
                        ws.Cells[rowIndex + 1, 3].Value = receive; //รับสินค้า
                        ws.Cells[rowIndex + 1, 4].Value = tranin; //รับโอน
                        ws.Cells[rowIndex + 1, 5].Value = sale; //ขาย
                        ws.Cells[rowIndex + 1, 6].Value = free; //แถม
                        ws.Cells[rowIndex + 1, 7].Value = tranout; //โอน
                        ws.Cells[rowIndex + 1, 8].Value = withdraw; //เบิกใช้
                        ws.Cells[rowIndex + 1, 9].Value = returnSup;//คืนเจ้าหนี้
                        ws.Cells[rowIndex + 1, 10].Value = adjust; //ปรับปรุง
                        ws.Cells[rowIndex + 1, 11].Value = audit; //ตรวจนับ
                        ws.Cells[rowIndex + 1, 12].Value = remain; //คงเหลือ

                        sumStockBanlance += banlance;
                        sumReceiveStock += receive;
                        sumTraninStock += tranin;
                        sumSale += sale;
                        sumIsFree += free;
                        sumTranOutStock += tranout;
                        sumWithdrawStock += withdraw;
                        sumRetuernSupStock += returnSup;
                        sumAdjustStock += adjust;
                        sumAuditStock += audit;
                        sumRemain += remain;

                        rowIndex++;
                    }
                    rowIndex++;

                    ws.Cells[rowIndex, 1].Value = $"รวมกลุ่มสินค้า : {productGroupId} {productGroupName}";
                    ws.Cells[rowIndex, 1].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 2].Value = sumStockBanlance; //รวมยอดยก
                    ws.Cells[rowIndex, 2].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 3].Value = sumReceiveStock; //รวมรับสินค้า
                    ws.Cells[rowIndex, 3].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 4].Value = sumTraninStock; //รวมรับโอน
                    ws.Cells[rowIndex, 4].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 5].Value = sumSale; //รวมขาย
                    ws.Cells[rowIndex, 5].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 6].Value = sumIsFree; //รวมแถม
                    ws.Cells[rowIndex, 6].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 7].Value = sumTranOutStock; //รวมโอน
                    ws.Cells[rowIndex, 7].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 8].Value = sumWithdrawStock; //รวมเบิกใช้
                    ws.Cells[rowIndex, 8].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 9].Value = sumRetuernSupStock; //รวมคืนเจ้าหนี้
                    ws.Cells[rowIndex, 9].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 10].Value = sumAdjustStock; //รวมปรับปรุง
                    ws.Cells[rowIndex, 10].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 11].Value = sumAuditStock; //รวมตรวจนับ
                    ws.Cells[rowIndex, 11].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 12].Value = sumRemain; //รวมคงเหลือ
                    ws.Cells[rowIndex, 12].Style.Font.Bold = true;
                    rowIndex++;
                }
                ws.Cells[rowIndex, 1].Value = $"หมายเหตุ : รายการสินค้าที่ขีดเส้นใต้ คือ รายการสินค้าที่ระงับการใช้";
            }
            else
            {
                ws.Cells["A" + rowIndex].Value = $"ไม่พบข้อมูล";

                using (var r = ws.Cells["A" + rowIndex + ":L" + rowIndex])
                {
                    r.Merge = true;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                };
            }



            return new MemoryStream(pck.GetAsByteArray());
        }
    }
}
