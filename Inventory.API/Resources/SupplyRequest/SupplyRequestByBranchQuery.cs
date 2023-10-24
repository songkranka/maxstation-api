using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.SupplyRequest
{
    public class SupplyRequestByBranchQuery
    {
        public List<string> Branchs { get; set; }
        public List<string> Products { get; set; }
    }
}
