using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transferdata.API.Domain.Models;
using Transferdata.API.Domain.Models.Queries;

namespace Transferdata.API.Domain.Services
{
    public interface ISummaryService
    {
        //1. Summary ส่วนลด กลุ่มน้ำมันใส และ LPG เฉพาะการขายสด 
        Task<List<CashsaleDisc>> ListCashsaleSummaryDiscAsync(SummaryQuery query);
        //2. Summary ส่วนลด กลุ่มน้ำมันเครื่อง ทั้งการขายสดและขายเชื่อ
        Task<List<SaleEngineOil>> ListSaleEngineOilSummaryDiscAsync(SummaryQuery query);
        //3. Summary จำนวนเงินขายเชื่อ กลุ่มน้ำมันใส และ LPG
        Task<List<CreditsaleAmount>> ListCreditsaleOilAmountAsync(SummaryQuery query);
        //4. Summary จำนวนเงิน(ขายสด - ส่วนลด) กลุ่มน้ำมันเครื่อง และสินค้าอื่น (ไม่เอากลุ่มน้ำมันใส และ LPG)
        Task<List<SaleNonOil>> ListCashsaleNonOilAmountAsync(SummaryQuery query);
        //5. Summary จำนวนเงิน(ขายเชื่อ - ส่วนลด) ทุกรายการ ยกเว้น material: 08898
        Task<List<CreditsaleAmount>> ListCreditsaleAmountAsync(SummaryQuery query);

    }
}
