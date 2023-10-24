using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transferdata.API.Resources.CashSale
{
    public class CashSaleHdResource
    {
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }        
        public string PosNo { get; set; }
    }
}
