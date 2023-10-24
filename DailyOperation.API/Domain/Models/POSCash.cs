using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models
{
    public class POSCash
    {
        public int Row { get; set; }
        public string JournalId { get; set; } 
        public string SiteId { get; set; }
        public DateTime BusinessDate { get; set; }
        public string ShiftNo { get; set; }
        public string TaxInvNo { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public decimal TotalGoodAmt { get; set; }
        public decimal TotalDiscAmt { get; set; }
        public decimal TotalTaxAmt { get; set; }
        public decimal TotalPaidAmt { get; set; }
        public string PluNumber { get; set; }
        public decimal SellQty { get; set; }
        public decimal SellPrice { get; set; }
        public decimal GoodsAmt { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal DiscAmt { get; set; }
        public string BillNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal SumItemAmt { get; set; }
        public decimal SubAmt { get; set; }
        public decimal TotalAmt { get; set; }
    }
}
