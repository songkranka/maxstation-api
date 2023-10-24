using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Domain.Models
{
    public class SaleEngineOil
    {
        public string BrnCode { get; set; }
        public string CompCode { get; set; }
        public DateTime? DocDate { get; set; }
        public decimal? DiscAmt { get; set; }
    }
}
