using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Domain.Models
{
    public class SupplyRequest
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string BrnName { get; set; }
        public string WhCode { get; set; }
        public string WhName { get; set; }
        public int SeqNo { get; set; }
        public string PdId { get; set; }
        public string PdName { get; set; }
        public int RequestQty { get; set; }
        public int ItemQty { get; set; }
        public string UnitName { get; set; }
    }
}
