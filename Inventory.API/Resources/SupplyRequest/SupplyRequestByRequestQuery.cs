using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.SupplyRequest
{
    public class SupplyRequestByRequestQuery
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
