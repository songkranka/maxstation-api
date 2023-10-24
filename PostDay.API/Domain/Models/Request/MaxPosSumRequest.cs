using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostDay.API.Domain.Models.Request
{
    public class MaxPosSumRequest
    {
        public string token { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public string shopid { get; set; }
    }
}
