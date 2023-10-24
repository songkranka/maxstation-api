using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models.Response
{
    public class PeriodCountResponse
    {
        public int CountPeriod { get; set; } 
    }
    public class CheckPeriodWaterResponse
    {
        public bool result { get; set; }
    }
}
