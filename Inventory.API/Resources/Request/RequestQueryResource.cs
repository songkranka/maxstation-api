using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API.Resources.Request
{
    public class RequestQueryResource 
    {
        public string CompCode { get; set; }
        public string BrnCode { get; set; }
        public string BrnCodeFrom { get; set; }
        public string LocCode { get; set; }
        public string PdGroupID { get; set; }
        public string DocNo { get; set; }
        public string Keyword { get; set; }
        public string Guid { get; set; }
        public string PDListID { get; set; }
        public string DocStatus { get; set; }
    }
}
