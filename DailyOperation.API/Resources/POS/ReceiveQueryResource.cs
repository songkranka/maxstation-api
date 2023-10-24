using System;

namespace DailyOperation.API.Resources.POS
{
    public class ReceiveQueryResource : QueryResource
    {
        public DateTime FromDate { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }
    }
}
