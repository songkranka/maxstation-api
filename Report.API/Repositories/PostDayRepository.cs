using MaxStation.Entities.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Report.API.Domain.Models.Requests;
using Report.API.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Report.API.Repositories
{

    public class PostDayRepository : BaseRepositories, IPostDayRepository //SqlDataAccessHelper 
    {



        public PostDayRepository(PTMaxstationContext context) : base(context)
        {
        }

        public MemoryStream GetWorkDateExcel(PostDayRequest req)
        {
            CultureInfo culture = new CultureInfo("en-US");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");


            // start items
            var rowIndex = 1;
            //column header
            ws.Cells[rowIndex, 1, rowIndex, 1].Value = "ลำดับ";
            ws.Cells[rowIndex, 2, rowIndex, 2].Value = "ปิดสิ้นวันถึงวันที่";
            ws.Cells[rowIndex, 3, rowIndex, 3].Value = "รหัสสาขา";
            ws.Cells[rowIndex, 4, rowIndex, 4].Value = "ชื่อสาขา";
            ws.Cells[rowIndex, 5, rowIndex, 5].Value = "วันและเวลาที่ปิดสิ้นวัน";

            ws.Cells[rowIndex, 1, rowIndex, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D3D3D3"));


            var sql = $@"select c.BRN_CODE as BrnCode,b.BRN_NAME as BrnName,c.CTRL_VALUE as DocDate,c.UPDATED_DATE as UpdatedDate
						from MAS_CONTROL c
						inner join MAS_BRANCH b
						on c.COMP_CODE = b.COMP_CODE
						and c.BRN_CODE = b.BRN_CODE						
						and b.BRN_STATUS <> 'Cancel'";
            if (req.Type == PostDayRequest.DocType.Equal)
            {
                sql += $" and convert(date, c.CTRL_VALUE,103) = '{req.DocDate?.ToString("yyyy-MM-dd")}'";
            }
            else if (req.Type == PostDayRequest.DocType.Lessthan)
            {
                sql += $" and convert(date, c.CTRL_VALUE,103) < '{req.DocDate?.ToString("yyyy-MM-dd")}'";
            }

            string strCompCode = GetString(req.CompCode);
            if (strCompCode.Length > 0)
            {
                sql += $" and c.COMP_CODE ='{strCompCode}'";
            }

            sql += $" order by b.BRN_CODE";

            var items = this.RawSqlQuery(sql, x => new
            {
                BrnCode = Convert.ToString(x[0]),
                BrnName = Convert.ToString(x[1]),
                DocDate = Convert.ToString(x[2]),
                UpdatedDate = x[3] == DBNull.Value ?"": Convert.ToDateTime(x[3]).ToString("dd/MM/yyyy HH:mm:ss"),
            }).ToList();


            rowIndex++;
            int seqno = 0;
            foreach (var item in items)
            {
                ws.Cells[rowIndex, 1].Value = ++seqno;
                ws.Cells[rowIndex, 2].Value = item.DocDate; //DateTime.ParseExact(item.DocDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) ;
                ws.Cells[rowIndex, 3].Value = item.BrnCode;
                ws.Cells[rowIndex, 4].Value = item.BrnName;
                ws.Cells[rowIndex, 5].Value = item.UpdatedDate;
                rowIndex++;
            }

            return new MemoryStream(pck.GetAsByteArray());

        }
    }
}
