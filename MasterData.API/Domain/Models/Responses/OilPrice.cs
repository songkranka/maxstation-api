using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses
{
    public class OilPrice
    {
        public List<Data> data { get; set; }
        public Response response { get; set; }
        public class Data
        {
            public string currentPrice { get; set; }
            public string opBarcode { get; set; }
            public string opBrn { get; set; }
            public string opComp { get; set; }
        }
        public class Response
        {
            public string resCode { get; set; }
            public string resMsg { get; set; }
        }
    }
}
