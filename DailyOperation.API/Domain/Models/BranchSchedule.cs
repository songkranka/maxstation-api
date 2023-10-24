using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models
{
    public class BranchSchedule
    {
        public string SITE_ID { get; set; }
        public DateTime BUSINESS_DATE { get; set; }
    }
}
