using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterData.API.Domain.Models.Responses.Price
{
    public class OilPrice
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string PdId { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
