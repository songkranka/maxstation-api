using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Resources.POS
{
    public class SaveCreditSaleResource
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime SystemDate { get; set; }
        public List<Creditsale> _Creditsale { get; set; }
        public int TotalItem { get; set; }
        public class Creditsale
        {
            public int Row { get; set; }
            public string JournalId { get; set; }
            public string SiteId { get; set; }
            public DateTime BusinessDate { get; set; }
            public string ShiftNo { get; set; }
            public string TaxInvNo { get; set; }
            public decimal TotalGoodsAmt { get; set; }
            public decimal TotalDiscAmt { get; set; }
            public decimal TotalTaxAmt { get; set; }
            public decimal TotalPaidAmt { get; set; }
            public string PluNumber { get; set; }
            public decimal SelQty { get; set; }
            public decimal SalePrice { get; set; }
            public decimal GoodsAmt { get; set; }
            public decimal TaxAmt { get; set; }
            public decimal DiscAmt { get; set; }
            public string BillNo { get; set; }
            public string ItemName { get; set; }
            public string ItemCode { get; set; }
            public string LicensePlate { get; set; }
            public string Mile { get; set; }
            public string CustCode { get; set; }
            public string CustName { get; set; }
            public string Po { get; set; }
            public decimal SumItemAmount { get; set; }
            public decimal SubAmount { get; set; }
            public decimal TotalAmount { get; set; }
            public string EmpCode { get; set; }
            public string EmpName { get; set; }
        }
    }
}
