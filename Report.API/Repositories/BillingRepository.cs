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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Report.API.Domain.Models.Response.BillingResponse;

namespace Report.API.Repositories
{
    public class BillingRepository : SqlDataAccessHelper, IBillingRepository
    {
        public BillingRepository(PTMaxstationContext context) : base(context)
        {

        }

        public async Task<BillingResponse> GetBillingAsync(BillingRequest request)
        {
            var response = new BillingResponse();
            var query = (from hd in this.context.SalBillingHds
                         join br in this.context.MasBranches on new { hd.CompCode, hd.BrnCode } equals new { br.CompCode, br.BrnCode }
                         join cm in this.context.MasCompanies on hd.CompCode equals cm.CompCode
                         where hd.CompCode == request.CompCode && hd.BrnCode == request.BrnCode && hd.DocNo == request.DocNo
                         select new { hd, br, cm }
                         ).AsQueryable();

            response = await query.Select(x => new BillingResponse
            {
                docNo = x.hd.DocNo,
                docDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                compCode = x.cm.CompCode,
                compName = x.cm.CompName,
                compAddress = x.cm.Address ?? "",
                compPhone = x.cm.Phone ?? "",
                compFax = x.cm.Fax ?? "",
                compRegisterId = x.cm.RegisterId ?? "",
                compImage = x.cm.CompImage,
                brnCode = x.hd.BrnCode,
                brnName = x.br.BrnName,
                branchNo = x.br.BranchNo ?? "",
                brnAddress = x.br.FullAddress ?? "",
                custCode = x.hd.CustCode,
                citizenId = x.hd.CitizenId??"",
                custName = x.hd.CustName,
                custAddress1 = x.hd.CustAddr1 ?? "",
                custAddress2 = x.hd.CustAddr2 ?? "",
                creditLimit = x.hd.CreditLimit ?? 0,
                creditTerm = x.hd.CreditTerm ?? 0,
                dueType = x.hd.DueType ?? "",
                dueDate = x.hd.DocDate.Value.ToString("yyyy-MM-dd"),
                itemCount = x.hd.ItemCount ?? 0,
                remark = x.hd.Remark ?? "",
                currency = x.hd.Currency ?? "",
                curRate = x.hd.CurRate ?? 0,
                totalAmt = x.hd.TotalAmt ?? 0,
                totalAmtCur = x.hd.TotalAmtCur ?? 0,
            }).FirstOrDefaultAsync();

            if (response != null)
            {
                var billingItems = await this.context.SalBillingDts.Where(x => x.CompCode == request.CompCode && x.BrnCode == request.BrnCode && x.DocNo == request.DocNo).ToListAsync();
                                                    

                response.countBilling = billingItems.AsEnumerable().Count();
                response.sumBilling = (decimal)billingItems.AsEnumerable().Sum(o => o.TxAmtCur);
                response.firstDate = billingItems.OrderBy(x => x.TxDate).FirstOrDefault().TxDate.Value.ToString("yyyy-MM-dd");
                response.lastDate = billingItems.OrderByDescending(x => x.TxDate).FirstOrDefault().TxDate.Value.ToString("yyyy-MM-dd");
                response.items = billingItems.Select(x => new BillingItem
                                {
                                    seqNo = x.SeqNo,
                                    txNo = x.TxNo,
                                    txDate = x.TxDate.Value.ToString("yyyy-MM-dd"),
                                    txType = x.TxType,
                                    txBrnCode = x.TxBrnCode ?? "",
                                    txAmt = x.TxAmt ?? 0,
                                    txAmtCur = x.TxAmtCur ?? 0,
                                }).ToList();
                var employee = await this.context.MasEmployees.FirstOrDefaultAsync(x => x.EmpCode == request.EmpCode);
                response.empName = (employee == null) ? "" : employee.EmpName;
            }
            return response;
        }

        public async Task<MemoryStream> ExportExcelAsync(BillingRequest req)
        {
            var data = GetBillingAsync(req).Result;
            //var brn = await context.MasBranches.FirstOrDefaultAsync(x => x.CompCode == req.CompCode && x.BrnCode == req.BrnCode);
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("sheet1");

            var companyName = ws.Cells[1, 3, 1, 13];
            companyName.Merge = true;
            companyName.Value = $"{data.compName}";
            companyName.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var companyAddress = ws.Cells[2, 3, 2, 13];
            companyAddress.Merge = true;
            companyAddress.Value = $"{data.compAddress}";
            companyAddress.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var companyPhone = ws.Cells[3, 3, 3, 13];
            companyPhone.Merge = true;
            companyPhone.Value = $"โทรศัพท์ {data.compPhone} โทรสาร {data.compFax}";
            companyPhone.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var branchTitle = ws.Cells[5, 1, 5, 13];
            branchTitle.Merge = true;
            branchTitle.Value = $"สาขา : {data.brnCode} - {data.brnName} {data.brnAddress}";

            var custName = ws.Cells[6, 1, 6, 13];
            custName.Merge = true;
            custName.Value = $"ชื่อผู้ซิ้อ {data.custName}";
            custName.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var custAddress1 = ws.Cells[7, 1, 7, 13];
            custAddress1.Merge = true;
            custAddress1.Value = $"ที่อยู่ {data.custAddress1}";
            custAddress1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var docNo = ws.Cells[6, 14, 6, 16];
            docNo.Merge = true;
            docNo.Value = $"เลขที่ {data.docNo}";
            docNo.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var custAddress2 = ws.Cells[8, 1, 8, 13];
            custAddress2.Merge = true;
            custAddress2.Value = $"        {data.custAddress2}";
            custAddress2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var docDate = ws.Cells[7, 14, 7, 16];
            docDate.Merge = true;
            docDate.Value = $"วันที่ {data.docDate}";
            docDate.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var compName = ws.Cells[10, 1, 10, 13];
            compName.Merge = true;
            compName.Value = $"{data.compName} สำหรับยอดสั่งซื้อช่วงวันที่ {data.firstDate}-{data.lastDate}";
            compName.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var sumBilling = ws.Cells[11, 1, 11, 13];
            sumBilling.Merge = true;
            sumBilling.Value = $"เป็นใบแจ้งหนี้/ใบ จำนวน {data.countBilling} ฉบับ เป็นเงินรวม {data.sumBilling} บาท (รายละเอียดตามเอกสารแนบ)";
            sumBilling.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var addBilling = ws.Cells[12, 1, 12, 13];
            addBilling.Merge = true;
            addBilling.Value = $"เป็นใบเพิ่มหนี้/ใบ จำนวน 0 ฉบับ เป็นเงินรวม 0.00 บาท (รายละเอียดตามเอกสารแนบ)";
            addBilling.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var tax = ws.Cells[14, 1, 14, 13];
            tax.Merge = true;
            tax.Value = $"ดอกเบี้ยชำระล่าช้าประจำงวดซื้อ ..................................... มูลค่า ..................................... บาท";
            tax.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var taxSum = ws.Cells[15, 1, 15, 13];
            taxSum.Merge = true;
            taxSum.Value = $"รวมเป็นเงินที่ต้องชำระ ..................................... บาท";
            taxSum.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var dueDate = ws.Cells[16, 1, 16, 13];
            dueDate.Merge = true;
            dueDate.Value = $"วันครบกำหนดชำระ {data.dueDate}";
            dueDate.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var text1 = ws.Cells[17, 1, 17, 13];
            text1.Merge = true;
            text1.Value = $"            กรุณาตรวจสอบความถูกต้องของเอกสารตามรายละเอียดข้างต้น หากพบข้อผิดพลาด";
            text1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var text2 = ws.Cells[18, 1, 18, 13];
            text2.Merge = true;
            text2.Value = $"ทั้งนี้ครบกำหนดชำระ หากเลยกำหนด บริษัทจะทำการคิดดอกเบี้ย 1.5% ต่อเดือนนับจากวันครบกำหนดชำระ";
            text2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var text3 = ws.Cells[19, 1, 19, 13];
            text3.Merge = true;
            text3.Value = $"            กรุณาชำระค่าสินค้าให้ตรงตามวันที่กำหนด";
            text3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var empname = ws.Cells[23, 2, 23, 4];
            empname.Merge = true;
            empname.Value = $"{data.empName}";
            empname.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            empname.Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;

            var empnameText = ws.Cells[24, 2, 24, 4];
            empnameText.Merge = true;
            empnameText.Value = $"ผู้วางบิล";
            empnameText.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var receivename = ws.Cells[23, 10, 23, 12];
            receivename.Merge = true;
            //receivename.Value = $"{data.empName}";
            receivename.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            receivename.Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;

            var receiveText = ws.Cells[24, 10, 24, 12];
            receiveText.Merge = true;
            receiveText.Value = $"ผู้วางบิล";
            receiveText.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            return new MemoryStream(await pck.GetAsByteArrayAsync());
        }
    }
}
