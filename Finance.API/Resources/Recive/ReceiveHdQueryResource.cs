using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Resources.Recive
{
    public class ReceiveHdQueryResource : QueryResource
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Keyword { get; set; }
        public Guid Guid { get; set; }
    }
}
