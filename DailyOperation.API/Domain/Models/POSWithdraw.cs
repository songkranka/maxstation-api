using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models
{
    public class POSWithdraw
    {
        public int Row { get; set; }
        public string JournalId { get; set; }
        public string SiteId { get; set; }
        public DateTime BusinessDate { get; set; }
        public string ShiftNo { get; set; }
        public string PluNumber { get; set; }
        public string CostCenter { get; set; }
        public string LicensePlate { get; set; }
        public decimal SellQty { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int SumWater { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
