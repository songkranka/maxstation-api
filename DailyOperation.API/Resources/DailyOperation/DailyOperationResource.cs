using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Resources.DailyOperation
{
    public class DailyOperationResource
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? WDate { get; set; }
        public string Post { get; set; }
        public string DocType { get; set; }
        public string User { get; set; }
    }
}
