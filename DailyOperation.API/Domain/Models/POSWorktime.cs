using System;

namespace DailyOperation.API.Domain.Models
{
    public class POSWorktime
    {
        public string ShiftNo { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
