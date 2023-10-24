using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.Adjust
{
    public class AdjustResource
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }        
        public string DocStatus { get; set; }
        public string BrnCodeFrom { get; set; }
        public string BrnNameFrom { get; set; }
        public Guid? Guid { get; set; }
    }
}
