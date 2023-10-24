using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.AdjustRequest
{
    public class AdjustRequestResource
    {
        public string DocNo { get; set; }
        public string BrnCode { get; set; }
        public DateTime? DocDate { get; set; }        
        public string DocStatus { get; set; }
        public string ReasonDesc { get; set; }
        public Guid? Guid { get; set; }
    }
}
