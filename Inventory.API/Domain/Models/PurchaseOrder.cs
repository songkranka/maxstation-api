using MaxStation.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models
{
    public class PurchaseOrder 
    {
        public List<InfPoHeader> InfPoHeaders { get; set; }
        public List<MasProduct> MasProducts { get; set; }
        public List<InfPoItem> InfPoItems { get; set; }
        public List<MasUnit> MasUnits { get; set; }
    }
}
