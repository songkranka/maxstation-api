using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Domain.Models.Response
{
    public class MaxmeResponse
    {
        public string ResCode { get; set; }
        public string Status { get; set; }
        public string Shopid { get; set; }
        public decimal Summary { get; set; }
    }
}
