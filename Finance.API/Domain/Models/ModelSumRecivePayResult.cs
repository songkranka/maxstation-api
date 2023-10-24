using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.API.Domain.Models
{
    public class ModelSumRecivePayResult
    {
        public string ReceiveTypeId { get; set; }
        public decimal SumNetAmt { get; set; }
    }

    public class ModelSumRecivePayResult2
    {
        public ModelSumRecivePayResult[] Data { get; set; }
        public int ItemCount { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
