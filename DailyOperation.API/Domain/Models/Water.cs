using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyOperation.API.Domain.Models
{
    public class Water
    {
        public Response response { get;set; }
        public Data data { get; set; }

        public class Response
        {
            public string ResCode { get; set; }
            public string ResMsg { get; set; }
        }

        public class Data
        {
            public string WsTransId { get; set; }
            public string SumWater { get; set; }
        }
    }
}
