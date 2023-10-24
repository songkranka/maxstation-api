using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Resources.POS
{
    public class CashQueryResource
    {
        public DateTime FromDate { get; set; }
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string LocCode { get; set; }

    }
}
