using Inventory.API.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models.Queries
{
    public class PurchaseOrderHdQuery
    {
        public string CompCode { get; set; }
        public string BranchCode { get; set; }
        public string PoNumber { get; set; }
        public DateTime? SystemDate { get; set; }
        public string Keyword { get; set; }

    }
}
