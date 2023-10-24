using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Request
{
    public class OilPriceRequest
    {
        public string compCode { get; set; }
        public string brnCode { get; set; }
        public DateTime systemDate { get; set; }
    }
}
