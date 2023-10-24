using System;

namespace DailyOperation.API.Domain.Models
{
    public class POSReceive
    {
        public int Row { get; set; }
        public string CustCode { get; set; }
        public string JournalId { get; set; }
        public string SiteId { get; set; }
        public DateTime  BusinessDate { get; set; }
        public string ShiftNo { get; set; }
        public string PluNumber { get; set; }
        public string ItemName { get; set; }
        public decimal SellQty { get; set; }
        public decimal SellPrice { get; set; }
        public decimal GoodsAmt { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal DiscAmt { get; set; }
        public decimal SumItemAmt { get; set; }
        public decimal SubAmt { get; set; }
        public decimal TotalAmt { get; set; }
       
    }
}
